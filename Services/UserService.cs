using Microsoft.AspNetCore.Mvc;
using DraganaMakeup.Context;
using DraganaMakeup.Models;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using DraganaMakeup.Records;

namespace DraganaMakeup.Services;

public class UserService
{
    private readonly ApplicationDbContext _context;

    public UserService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<string> SignInUser(UserSignInRecord userRecord)
    {
        try
        {
            User? foundUser = await _context.Users.FirstOrDefaultAsync(u => u.Phone == userRecord.Phone && u.SessionID != "");
            if (foundUser == null)
            {
                return "Pogresna sifra ili broj telefona";
            }
            bool passwordVerified = BCrypt.Net.BCrypt.Verify(userRecord.Password, foundUser.Password);
            if (passwordVerified == false)
            {
                return "Pogresna sifra ili broj telefona";
            }
            var newSessionID = Guid.NewGuid().ToString();
            foundUser.SessionID = newSessionID;
            _context.Users.Update(foundUser);
            await _context.SaveChangesAsync();
            return newSessionID;
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    public async Task<string> SignUpUser(UserRecord userRecord, string sessionID)
    {
        try
        {
            User? userPhone = _context.Users.FirstOrDefault(u => u.Phone == userRecord.Phone);
            if (userPhone != null)
            {
                if (userPhone.SessionID == "")
                {
                    _context.Users.Remove(userPhone);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    return "Ovaj broj telefona je zauzet";
                }
            }
            User? userUsername = _context.Users.FirstOrDefault(u => u.Username == userRecord.Username);
            if (userUsername != null)
            {
                return "Korisnicko ime vec postoji";
            }
            if (!VerifyPassword(userRecord.Password))
            {
                return "Lozinka mora imati 8 karaktera i najmanje jedno veliko slovo";
            }
            var hashedPassword = HashPassword(userRecord.Password);
            User newUser = new User
            {
                Phone = userRecord.Phone,
                Password = hashedPassword,
                Username = userRecord.Username,
                CreatedAt = userRecord.CreatedAt,
                Role = 'U',
                SessionID = sessionID
            };
            await _context.Users.AddAsync(newUser);
            await _context.SaveChangesAsync();
            return "Nalog uspesno kreiran";
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    public async Task<bool> GetAdmin(string sessionID)
    {
        try
        {
            FoundUserRecord? userRecord = await _context.Users.Where(u => u.SessionID == sessionID).Select(u => new FoundUserRecord(u.ID, u.SessionID, u.Username, u.Phone, u.Role)).FirstOrDefaultAsync();
            return userRecord!.Role == 'A';
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return false;
        }
    }

    public async Task<object> GetUser(string sessionID)
    {
        try
        {
            var foundUser = await _context.Users.Select(u => new { u.ID, u.SessionID, u.Username, u.Phone }).FirstOrDefaultAsync(u => u.SessionID == sessionID);
            if (foundUser == null)
            {
                return "Korisnik nije pronadjen";
            }
            return foundUser;
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
    public async Task<string> VerifySessionID(string sessionID)
    {
        try
        {
            User? createdUser = _context.Users.FirstOrDefault(u => u.SessionID == sessionID);
            if ((DateTime.UtcNow - createdUser!.CreatedAt).TotalMinutes > 30)
            {
                createdUser!.SessionID = "";
                _context.Users.Update(createdUser);
                await _context.SaveChangesAsync();
                return "Sesija je istekla";
            }
            else
            {
                return "Sesija je validna";
            }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    private bool VerifyPassword(string password)
    {
        if (password.Length < 8)
        {
            return false;
        }
        return password.Any(char.IsUpper);
    }

    private string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }
}
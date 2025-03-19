using System.Globalization;
using DraganaMakeup.Context;
using DraganaMakeup.Models;
using DraganaMakeup.Records;
using EduSchedule.Services;
using Microsoft.EntityFrameworkCore;

namespace DraganaMakeup.Services;

public class AppointmentService
{
    private readonly ApplicationDbContext _context;
    private readonly EmailService _emailService;

    public AppointmentService(ApplicationDbContext context, EmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }

    public async Task<object> GetAppointments()
    {
        try
        {
            var appointments = await _context.Appointments.Where(a => a.StartTime > DateTime.UtcNow).Select(a => new { ID = a.ID, Service = a.Service, Duration = a.Duration, StartTime = a.StartTime, User = a.User! }).ToListAsync();
            if (appointments == null)
            {
                return "Termini nisu pronadjeni";
            }
            return appointments;
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    public async Task<object> GetAppointmentsForAdmin()
    {
        try
        {
            var appointments = await _context.Appointments.Where(a => a.StartTime > DateTime.UtcNow).Select(a => new { ID = a.ID, Service = a.Service, Duration = a.Duration, StartTime = a.StartTime, Comment = a.Comment, User = a.User! }).ToListAsync();
            if (appointments == null)
            {
                return "Termini nisu pronadjeni";
            }
            return appointments;
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    public async Task<object> GetAppointmentByUserID(int userID)
    {
        var user = await _context.Appointments.Where(a => a.UserID == userID && a.StartTime > DateTime.UtcNow).Select(a => new { ID = a.ID, Service = a.Service, StartTime = a.StartTime, User = a.User! }).FirstOrDefaultAsync();
        return user!;
    }

    public async Task<object> GetAppointmentByID(int appID)
    {
        var user = await _context.Appointments.Where(a => a.ID == appID).Select(a => new { ID = a.ID, Service = a.Service, StartTime = a.StartTime, User = a.User!, Comment = a.Comment }).FirstOrDefaultAsync();
        return user!;
    }

    public async Task<object> GetAppointmentByDate(string date)
    {
        DateTime parsedDate = DateTime.ParseExact(date, "dd MMM yyyy", CultureInfo.InvariantCulture);
        string formattedDate = parsedDate.ToString("dd.MM.yyyy HH:mm:ss");
        DateTime finalDateTime = DateTime.ParseExact(formattedDate, "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture);
        var user = await _context.Appointments.Where(a => a.StartTime.Date == finalDateTime).Select(a => new { ID = a.ID, Service = a.Service, Duration = a.Duration, StartTime = a.StartTime, User = a.User!.ID }).ToListAsync();
        return user!;
    }

    public async Task<string> DeleteAppointment(int appointmentID)
    {
        try
        {
            var appointment = await _context.Appointments.FirstOrDefaultAsync(a => a.ID == appointmentID);
            if (appointment == null)
            {
                return "Termin nije pronadjen";
            }
            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();
            return "Termin otkazan";
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    public async Task<string> ScheduleAppointment(AppointmentRecord appointmentRecord, string sessionID)
    {
        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.SessionID == sessionID);
            if (user == null)
            {
                return "Korisnik nije pronadjen";
            }
            var appointment = await _context.Appointments.FirstOrDefaultAsync(a => a.UserID == user.ID);
            if (appointment != null)
            {
                _context.Appointments.Remove(appointment);
            }
            Appointment newAppointment = new Appointment
            {
                Service = appointmentRecord.Service,
                Duration = appointmentRecord.Duration,
                StartTime = appointmentRecord.startTime,
                User = user,
                Comment = appointmentRecord.Comment == "" ? null : appointmentRecord.Comment
            };
            await _context.Appointments.AddAsync(newAppointment);
            await _context.SaveChangesAsync();
            var body = $"<p>Korisnicko ime: <span>{user.Username}</span></p></br> <p>Broj telefona: <span>{user.Phone}</span></p></br> <p>Datum: <span>{TimeZoneInfo.ConvertTimeFromUtc(newAppointment.StartTime, TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time"))}</span></p></br> <p>Usluga: <span>{newAppointment.Service}</span></p></br>";
            await _emailService.SendEmail(body);
            return "Termin zakazan";
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
}
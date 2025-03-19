using Microsoft.AspNetCore.Mvc;
using DraganaMakeup.Services;
using Microsoft.Net.Http.Headers;
using DraganaMakeup.Models;
namespace DraganaMakeup.Records;

[Route("[controller]/auth/")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly UserService _userService;

    public UserController(UserService userService)
    {
        _userService = userService;
    }

    [HttpPost("sign-in")]
    public async Task<ActionResult> SignInUser([FromBody] UserSignInRecord userRecord)
    {
        try
        {
            var result = await _userService.SignInUser(userRecord);
            if (result == "Pogresna sifra ili broj telefona")
            {
                return BadRequest(result);
            }
            bool admin = await _userService.GetAdmin(result);
            if (admin)
            {
                SetAdminSessionIDInCookies(result);
            }
            else
            {
                SetSessionIDInCookies(result);
            }
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("sign-out")]
    public ActionResult SignOutUser()
    {
        try
        {
            RemoveSessionIDFromCookies();
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("sign-up")]
    public async Task<ActionResult> SignUpUser([FromBody] UserRecord userRecord)
    {
        try
        {
            var sessionID = Guid.NewGuid().ToString();
            var result = await _userService.SignUpUser(userRecord, sessionID);
            if (result.StartsWith("Nalog uspesno"))
            {
                SetSessionIDInCookies(sessionID);
                return Ok(new { sessionID });
            }
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }


    [HttpGet("get-user")]
    public async Task<ActionResult> GetUser()
    {
        try
        {
            var sessionID = Request.Cookies["SessionID"];
            if (!string.IsNullOrEmpty(sessionID))
            {
                var result = await _userService.GetUser(sessionID);
                return Ok(result);
            }
            return BadRequest("Sesija je istekla ili nije stavljena");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("get-admin")]
    public async Task<ActionResult> GetAdmin()
    {
        try
        {
            var adminSessionID = Request.Cookies["AdminSessionID"];
            if (!string.IsNullOrEmpty(adminSessionID))
            {
                var result = await _userService.GetUser(adminSessionID);
                return Ok(result);
            }
            return BadRequest("Sesija je istekla ili nije stavljena");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    private void SetSessionIDInCookies(string sessionID)
    {
        var cookieOptions = new CookieOptions()
        {
            HttpOnly = true,
            Secure = true,
            SameSite = Microsoft.AspNetCore.Http.SameSiteMode.None,
            Expires = DateTime.UtcNow.AddHours(1)
        };

        Response.Cookies.Append("SessionID", sessionID, cookieOptions);

    }

    private void SetAdminSessionIDInCookies(string sessionID)
    {
        var cookieOptions = new CookieOptions()
        {
            HttpOnly = true,
            Secure = true,
            SameSite = Microsoft.AspNetCore.Http.SameSiteMode.None,
        };
        Response.Cookies.Append("AdminSessionID", sessionID, cookieOptions);
    }

    private void RemoveSessionIDFromCookies()
    {
        Response.Cookies.Append("SessionID", "", new CookieOptions()
        {
            Expires = DateTime.UtcNow.AddDays(-1),
            Secure = true,
            HttpOnly = true,
            SameSite = Microsoft.AspNetCore.Http.SameSiteMode.None,
        });
    }
}
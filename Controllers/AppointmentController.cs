using DraganaMakeup.Services;
using Microsoft.AspNetCore.Mvc;
namespace DraganaMakeup.Records;

[Route("[controller]")]
[ApiController]
public class AppointmentController : ControllerBase
{
    private readonly AppointmentService _appointmentService;

    public AppointmentController(AppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
    }

    [HttpGet("get-appointments")]
    public async Task<ActionResult> GetAppointments()
    {
        try
        {
            var result = await _appointmentService.GetAppointments();
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

     [HttpGet("get-appointments-admin")]
    public async Task<ActionResult> GetAppointmentsForAdmin()
    {
        try
        {
            var result = await _appointmentService.GetAppointmentsForAdmin();
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("get-appointment/{userID}")]
    public async Task<ActionResult> GetAppointmentByUserID(int userID)
    {
        try
        {
            var result = await _appointmentService.GetAppointmentByUserID(userID);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("get-appointment-by/{appID}")]
    public async Task<ActionResult> GetAppointmentByID(int appID)
    {
        try
        {
            var result = await _appointmentService.GetAppointmentByID(appID);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("get-appointment-by-date/{date}")]
    public async Task<ActionResult> GetAppointmentByDate(string date)
    {
        try
        {
            var result = await _appointmentService.GetAppointmentByDate(date);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("schedule-appointment/{sessionID}")]
    public async Task<ActionResult> ScheduleAppointment(AppointmentRecord appointmentRecord, string sessionID)
    {
        try
        {
            var result = await _appointmentService.ScheduleAppointment(appointmentRecord, sessionID);
            Console.WriteLine(appointmentRecord.startTime);
            if (result == "Termin zakazan")
            {
                return Ok(new { result });
            }
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("delete-appointment/{appointmentID}")]
    public async Task<ActionResult> DeleteAppointment(int appointmentID)
    {
        try
        {
            var result = await _appointmentService.DeleteAppointment(appointmentID);
            return Ok(new { result });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
namespace DraganaMakeup.Records{
public record AppointmentRecord(DateTime startTime, int Duration, string Service, string? Comment, string? Username, string? Phone);
};
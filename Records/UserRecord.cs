namespace DraganaMakeup.Records
{
public record UserRecord(string Username, string Phone, string Password, DateTime CreatedAt);
public record UserSignInRecord(string Phone,string Password);

public record FoundUserRecord(int ID, string SessionID, string Username, string Phone, char Role);
}
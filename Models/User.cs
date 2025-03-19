using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DraganaMakeup.Models;
public class User
{
    [Key]
    public int ID { get; set; }
    public required string Username { get; set; }
    public required string Phone { get; set; }
    public required string Password { get; set; }
    public char Role { get; set; }
    public required string SessionID { get; set; }
    [JsonIgnore]
    public DateTime CreatedAt { get; set; }
    [JsonIgnore]
    public Appointment? Appointment { get; set; }
}
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DraganaMakeup.Models;
public class Appointment
{
    [Key]
    public int ID { get; set; }
    [JsonIgnore]
    public DateTime StartTime { get; set; }
    public int Duration { get; set; }
    public bool AdminAdded {get; set;} = false;
    public required string Service { get; set; }
    public string? Comment { get; set; }
    [JsonIgnore]
    public int UserID { get; set; }
    [JsonIgnore]
    public User? User { get; set; }
}
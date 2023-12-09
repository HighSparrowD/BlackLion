using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApi.Main.Enums.General;
using WebApi.Main.Enums.User;

#nullable enable
namespace WebApi.Main.Models.User;

public class Request
{
    [Key]
    public long Id { get; set; }
    [ForeignKey("Receiver")]
    public long UserId { get; set; }
    [ForeignKey("Sender")]
    public long SenderId { get; set; }
    public bool IsMatch { get; set; }
    public string? Message { get; set; } // Message from user
    public string? SystemMessage { get; set; } // Message from user
    public MessageType Type { get; set; } // Type of message from user

    public DateTime? AnsweredTimeStamp { get; set; }
    public RequestAnswer? Answer { get; set; }

    public virtual User? Receiver { get; set; }
    public virtual User? Sender { get; set; }
}

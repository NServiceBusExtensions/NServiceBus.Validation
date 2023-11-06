using System.ComponentModel.DataAnnotations;

public class MessageWithValidator :
    IMessage
{
    [Required]
    public string? Content { get; set; }
}
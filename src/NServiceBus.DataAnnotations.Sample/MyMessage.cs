public class MyMessage :
    IMessage
{
    [Required]
    public string? Content { get; set; }
}
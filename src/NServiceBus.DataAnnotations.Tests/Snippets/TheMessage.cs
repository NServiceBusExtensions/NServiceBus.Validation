#region DataAnnotations_message
public class TheMessage :
    IMessage
{
    [Required]
    public string Content { get; set; } = null!;
}
#endregion
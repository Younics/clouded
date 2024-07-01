namespace WebSupport.Library.Dtos;

public class UserOutput
{
    public long Id { get; set; }
    public long ParentLd { get; set; }
    public string Login { get; set; } = null!;
    public bool Active { get; set; }
    public int CreateTime { get; set; }
    public string? Group { get; set; }
    public string Email { get; set; } = null!;
    public string? Phone { get; set; }
    public string? ContactPerson { get; set; }
    public int AwaitingTosConfirmation { get; set; }
    public string UserLanguage { get; set; } = null!;
    public double Credit { get; set; }
    public string VerifyUrl { get; set; } = null!;
}

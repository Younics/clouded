namespace WebSupport.Library.Dtos;

public class RecordOutput
{
    public long Id { get; set; }
    public string Type { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Content { get; set; } = null!;
    public int Ttl { get; set; }
}

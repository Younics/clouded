namespace WebSupport.Library.Dtos;

public class ActionOutput<T>
    where T : class, new()
{
    public string Status { get; set; } = null!;
    public T? Item { get; set; }
    public object Errors { get; set; } = null!;
}

namespace Clouded.Platform.App.Web.Dtos;

public class FunctionProject
{
    public string Name { get; set; }
    public IEnumerable<FunctionProjectExecutable> Executables { get; set; }
}

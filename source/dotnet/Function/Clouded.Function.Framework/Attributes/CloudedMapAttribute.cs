using System.ComponentModel.DataAnnotations;

namespace Clouded.Function.Framework.Attributes;

public class CloudedMapAttribute(string name) : Attribute
{
    [Required]
    public string Name { get; set; } = name;
}

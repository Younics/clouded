namespace Clouded.Core.DataSource.Shared.Input;

public class OrderInput
{
    public required string Alias { get; set; }
    public required string Column { get; set; }

    public OrderType Direction { get; set; } = OrderType.Asc;
}

namespace EventHubMessageTransceiver;

public class Deal
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public DateTime CreatedOnDate { get; set; }
    public int NumberOfYearsToKeepOnRecord { get; set; }
    public bool IsGoodDeal { get; set; }

}
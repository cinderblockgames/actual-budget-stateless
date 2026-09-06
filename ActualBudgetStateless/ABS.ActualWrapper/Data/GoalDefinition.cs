namespace ABS.ActualWrapper.Data;

public class GoalDefinition
{
    public required string Directive { get; set; }
    public required string Type { get; set; }
    public required decimal Amount { get; set; }
    public GoalDefinitionPeriod? Period { get; set; }
    public string? Month { get; set; }
    public bool Annual { get; set; }
    public int? Repeat { get; set; }
    public int Priority { get; set; }
}

public class GoalDefinitionPeriod
{
    public string Period { get; set; }
    public int Amount { get; set; }
}

using System.Text.Json;

namespace ABS.ActualWrapper.Data;

public class GoalDefinition
{
    public required string Directive { get; set; }
    public required string Type { get; set; }
    public decimal? Amount { get; set; }
    public JsonElement? Period { get; set; }
    public GoalDefinitionPeriod? PeriodDetails { get; set; }
    public string? PeriodType { get; set; }
    public string? Month { get; set; }
    public bool Annual { get; set; }
    public int? Repeat { get; set; }
    public bool Hold { get; set; }
    public int? Priority { get; set; }
}

public class GoalDefinitionPeriod
{
    public string Period { get; set; }
    public int Amount { get; set; }
}

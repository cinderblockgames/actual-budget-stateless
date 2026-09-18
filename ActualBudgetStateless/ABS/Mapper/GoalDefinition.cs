using System.Text;
using ABS.ActualWrapper.Data;
using ABS.Models;

namespace ABS.Mapper;

public static partial class Mapper
{
    public static string Map(GoalDefinition input)
    {
        StringBuilder sb = new();
        if ("periodic".Equals(input.Type, StringComparison.OrdinalIgnoreCase))
        {
            sb.Append($"Budget {input.Amount:#,##0.00} every ");
            var period = input.PeriodDetails!;
            sb.Append(period.Amount > 1 ? $"{period.Amount} {period.Period}s" : period.Period);
        }
        else if ("by".Equals(input.Type, StringComparison.OrdinalIgnoreCase))
        {
            sb.Append($"Save {input.Amount:#,##0.00} by ");
            sb.Append(new MonthViewModel(input.Month!).Display);
            if (input.Annual)
            {
                sb.Append(", repeating every ");
                sb.Append(input.Repeat > 1 ? $"{input.Repeat} years" : "year");
            }
        }
        else if ("refill".Equals(input.Type, StringComparison.OrdinalIgnoreCase))
        {
            sb.Append("Refill to balance limit");
        }
        else if ("limit".Equals(input.Type, StringComparison.OrdinalIgnoreCase))
        {
            sb.Append($"Set a balance limit of {input.Amount:#,##0.00}/{input.PeriodType} (");
            sb.Append(input.Hold ? "soft" : "hard");
            sb.Append(" cap)");
        }
        return sb.ToString();
    }

    public static string Map(GoalDefinition[] input)
    {
        return string.Join("\n", input.OrderBy(def => def.Priority.HasValue).Select(Map));
    }
}
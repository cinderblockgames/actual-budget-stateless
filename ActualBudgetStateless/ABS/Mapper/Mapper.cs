namespace ABS.Mapper;

public static partial class Mapper
{
    public static string? ToDollars(decimal? input)
    {
        if (input.HasValue)
        {
            return $"{input / 100:#,##0.00}";
        }

        return null;
    }

    public static string? Truncate(string? input)
    {
        // This should probably be handled with stylesheets, honestly.
        if (input?.Length > 22)
        {
            return $"{input[..20]}…";
        }

        return input;
    }
}
namespace ABS.Models;

public class MonthViewModel
{
    public string Month { get; set; }
    public string Year { get; set; }
    public string Display { get; set; }
    
    public string Joined => $"{Year}-{Month}";

    public MonthViewModel(string month, int offset = 0)
    {
        var split = month.Split("-");
        var date = new DateTime(
            int.Parse(split[0]),
            int.Parse(split[1]),
            1);
        date = date.AddMonths(offset);
        Year = date.ToString("yyyy");
        Month = date.ToString("MM");
        Display = $"{date:MMMM} {Year}";
    }
}
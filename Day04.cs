using System.Text.RegularExpressions;

public partial class Day04
{
    private int GetScore(string line)
    {
        var numberRegex = MyRegex();
        
        var gameSplit = line.Split(":");
        var cardNum = int.Parse(numberRegex.Matches(gameSplit[0])[0].Value);

        var numbersSplit = gameSplit[1].Split("|");

        var winningNumbers = numberRegex.Matches(numbersSplit[0]).Select(x => int.Parse(x.Value));
        var myNumbers = numberRegex.Matches(numbersSplit[1]).Select(x => int.Parse(x.Value));

        var results = winningNumbers.Intersect(myNumbers).ToList();
        return (int)Math.Pow(2, results.Count-1);
    }
    
    [Test]
    public void Part1_sample_equals_13()
    {
        var lines = File.ReadAllLines("Day04.sample.txt");
        Assert.That(lines.Sum(GetScore), Is.EqualTo(13));
    }
    
    [Test]
    public void Part1_input_equals_20117()
    {
        var lines = File.ReadAllLines("Day04.input.txt");
        Assert.That(lines.Sum(GetScore), Is.EqualTo(13));
    }
    
    [GeneratedRegex(@"\d+")]
    private static partial Regex MyRegex();
}
using System.Text.RegularExpressions;

public partial class Day04
{
    private class ScratchCard
    {
        public int CardNum;

        public List<int> WinningNumbers;

        public int Score => (int)Math.Pow(2, WinningNumbers.Count - 1);

        public List<ScratchCard> Copies = [];
    }

    private Dictionary<int, ScratchCard> ScratchCards = new();
    
    private ScratchCard GetCard(string line)
    {
        var numberRegex = MyRegex();
        
        var gameSplit = line.Split(":");
        var numbersSplit = gameSplit[1].Split("|");

        var winningNumbers = numberRegex.Matches(numbersSplit[0]).Select(x => int.Parse(x.Value));
        var myNumbers = numberRegex.Matches(numbersSplit[1]).Select(x => int.Parse(x.Value));

        return new ScratchCard
        {
            CardNum = int.Parse(numberRegex.Matches(gameSplit[0])[0].Value),
            WinningNumbers = winningNumbers.Intersect(myNumbers).ToList()
        };
    }
    
    [Test]
    public void Part1_sample_equals_13()
    {
        var lines = File.ReadAllLines("Day04.sample.txt");
        Assert.That(lines.Sum(line => GetCard(line).Score), Is.EqualTo(13));
    }
    
    [Test]
    public void Part1_input_equals_20117()
    {
        var lines = File.ReadAllLines("Day04.input.txt");
        Assert.That(lines.Sum(line => GetCard(line).Score), Is.EqualTo(20117));
    }

    private List<ScratchCard> Descend(List<ScratchCard> rootWinningCards, ScratchCard card)
    {
        var startRange = Math.Clamp(card.CardNum, 0, rootWinningCards.Count);
        var endRange = Math.Clamp(card.CardNum + card.WinningNumbers.Count, 0, rootWinningCards.Count);
        var count = endRange - startRange;

        var cardCopies = rootWinningCards.GetRange(startRange, count).ToList();

        var result = new List<ScratchCard>(cardCopies);
        
        foreach (var copy in cardCopies)
        {
            result.AddRange(Descend(rootWinningCards, copy));
        }

        return result;
    }
    
    [Test]
    public void Part2_sample_equals_30()
    {
        var lines = File.ReadAllLines("Day04.sample.txt");
        var rootWinningCards = lines.Select(GetCard).ToList();

        var result = new List<ScratchCard>(rootWinningCards);
        
        foreach (var card in rootWinningCards)
        {
            result.AddRange(Descend(rootWinningCards, card));
        }
        
        Assert.That(result.Count, Is.EqualTo(30));
    }
    
    [Test]
    public void Part2_input_equals_13768818()
    {
        var lines = File.ReadAllLines("Day04.input.txt");
        var rootWinningCards = lines.Select(GetCard).ToList();

        var result = new List<ScratchCard>(rootWinningCards);
        
        foreach (var card in rootWinningCards)
        {
            result.AddRange(Descend(rootWinningCards, card));
        }
        
        Assert.That(result.Count, Is.EqualTo(30));
    }
    
    [GeneratedRegex(@"\d+")]
    private static partial Regex MyRegex();
}
namespace AdventOfCode23;

public class Day02
{
    private static readonly Dictionary<string, int> _cubeInventory = new()
    {
        { "red", 12 },
        { "green", 13 },
        { "blue", 14 }
    };
    
    private class RoundItem
    {
        public RoundItem(string roundItemString) // i.e. "3 blue"
        {
            var split = roundItemString.Trim().Split(" ");
            CubeCount = int.Parse(split[0]);
            CubeType = split[1];
        }
        
        public string CubeType { get; init; }
 
        public int CubeCount { get; init; }

        public bool IsInvalid => CubeCount > _cubeInventory[CubeType];
    }

    private class Round
    {
        public Round(string roundString)
        {
            Items = roundString.Split(",").Select(roundItemString => new RoundItem(roundItemString));
        }
        
        public IEnumerable<RoundItem> Items { get; init; }
    }

    private class Game
    {
        public Game(string gameString)
        {
            // gameString looks like: "Game 1: 3 blue, 4 red; 1 red, 2 green, 6 blue; 2 green"
            
            var splitLine = gameString.Split(":"); // i.e. "[Game 1", "3 blue, 4 red; 1 red, 2 green, 6 blue; 2 green"]
            
            var splitGame = splitLine[0].Split(" "); // i.e. ["Game", "1"]
            Id = int.Parse(splitGame[1]); // i.e. 1
            
            var roundStrings = splitLine[1].Split(";"); // i.e. ["3 blue, 4 red" "1 red, 2 green", "6 blue; 2 green"]

            Rounds = roundStrings.Select(rs => new Round(rs));
        }
        
        public int Id { get; init; }
        
        public IEnumerable<Round> Rounds { get; init; }

        public bool IsInvalid => Rounds.Any(x => x.Items.Any(y => y.IsInvalid));
    }

    private static int GetPower(Game game)
    {
        Dictionary<string, int> maxValues = new();

        foreach (var round in game.Rounds)
        {
            foreach (var roundItem in round.Items)
            {
                maxValues.TryGetValue(roundItem.CubeType, out var maxValue);

                if (roundItem.CubeCount > maxValue)
                {
                    maxValues[roundItem.CubeType] = roundItem.CubeCount;
                }
            }
        }

        var power = maxValues.Values.First();
        return maxValues.Values.Skip(1).Aggregate(power, (current, p) => current * p);
    }

    private static int GetSumValidGames(IEnumerable<Game> games)
    {
        return games.Where(g => !g.IsInvalid).Sum(g => g.Id);
    }

    private static IEnumerable<Game> ReadGames(string filename)
    {
        var gameStrings = File.ReadLines(filename);
        return gameStrings.Select(g => new Game(g));
    }
    
    [Test]
    public void Sample_equals_8()
    {
        var games = ReadGames("Day02.sample.txt");
        Assert.That(GetSumValidGames(games), Is.EqualTo(8));
    }
    
    [Test]
    public void Input_equals_2776()
    {
        var games = ReadGames("Day02.input.txt");
        Assert.That(GetSumValidGames(games), Is.EqualTo(2776));
    }

    [Test]
    public void Sample_power_equals_2286()
    {
        var games = ReadGames("Day02.sample.txt");
        Assert.That(games.Sum(GetPower), Is.EqualTo(2286));
    }
    
    [Test]
    public void Input_power_equals_68638()
    {
        var games = ReadGames("Day02.input.txt");
        Assert.That(games.Sum(GetPower), Is.EqualTo(68638));
    }
}
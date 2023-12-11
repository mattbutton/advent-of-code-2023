using System.Text.RegularExpressions;

namespace AdventOfCode23;

public class Day03
{
    private class GetPartsResult
    {
        public IEnumerable<int> ValidParts = null!;
        public int GearRatio;
    }
    
    private GetPartsResult GetParts(string filename)
    {
        var lines = File.ReadAllLines(filename);
        var lineLength = lines[0].Length;

        var symbolsRegex = new Regex("[^0-9.\r\n]");
        var starRegex = new Regex("[*]");
        var numbersRegex = new Regex(@"\d+");
        
        var potentialGears = new Dictionary<string, List<int>>();
        var validPartNumbers = new List<int>();

        // Get all numbers, along with their line number, start index and end index relative to the line that they are on.
        var numbers = lines.SelectMany(
                (line, lineNumber) => numbersRegex.Matches(line)
                    .Select(x => new
                    {
                        Value = int.Parse(x.Value),
                        LineNumber = lineNumber,
                        StartIndex = x.Index, x.Length,
                        EndIndex = x.Index + x.Length
                    }));

        foreach (var number in numbers)
        {
            // Build line scan range
            var start = Math.Clamp(number.StartIndex - 1, 0, lineLength);
            var end = Math.Clamp(number.EndIndex + 1, 0, lineLength);
            var length = end - start;

            var lineNumbersToCheck = new List<int>(3) { number.LineNumber };
            if (number.LineNumber > 0) lineNumbersToCheck.Add(number.LineNumber - 1);
            if (number.LineNumber + 1 < lines.Length) lineNumbersToCheck.Add(number.LineNumber + 1);

            foreach (var lineNumber in lineNumbersToCheck)
            {
                var range = lines[lineNumber].Substring(start, length);
                if (symbolsRegex.Match(range).Success)
                {
                    validPartNumbers.Add(number.Value);
                }

                var starMatch = starRegex.Match(range);
                if (starMatch.Success)
                {
                    var potentialGearKey = $"{start + starMatch.Index},{lineNumber}";
                    if (potentialGears.ContainsKey(potentialGearKey))
                    {
                        potentialGears[potentialGearKey].Add(number.Value);
                    }
                    else
                    {
                        potentialGears[potentialGearKey] = [number.Value];
                    }
                }
            }
        }

        // A particular star is only a gear if exactly two part numbers are touching it.
        var gears = potentialGears
            .Where(x => x.Value.Count == 2)
            .Select(x => x.Value);

        return new GetPartsResult
        {
            ValidParts = validPartNumbers,
            GearRatio = gears.Select(x => x[0] * x[1]).Sum()
        };
    }

    [Test]
    public void Part1_Sample_Equals_4361()
    {
        Assert.That(GetParts("Day03.sample.txt").ValidParts.Sum(), Is.EqualTo(4361));
    }

    [Test]
    public void Part1_Input_Equals_520135()
    {
        Assert.That(GetParts("Day03.input.txt").ValidParts.Sum(), Is.EqualTo(520135));
    }
    
    [Test]
    public void Part2_Sample_Equals_4361()
    {
        Assert.That(GetParts("Day03.sample.txt").GearRatio, Is.EqualTo(467835));
    }

    [Test]
    public void Part2_Input_Equals_520135()
    {
        Assert.That(GetParts("Day03.input.txt").GearRatio, Is.EqualTo(72514855));
    }
}
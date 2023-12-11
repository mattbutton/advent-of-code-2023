using System.Text;

namespace AdventOfCode23;

public class Day01
{
    private static List<string> _words =
        ["zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine"];

    private static string ReplaceWordsWithNumbers(string text)
    {
        var sbText = new StringBuilder(text);
        
        for (var i = 0; i < _words.Count; i++)
        {
            var word = _words[i];

            for (var wordIndex = text.IndexOf(word, StringComparison.CurrentCulture);
                 wordIndex > -1;
                 wordIndex = text.IndexOf(word, wordIndex + 1, StringComparison.CurrentCulture))
            {
                // We don't want to replace the whole word, since we may encounter lines containing i.e. 'eightwothree',
                // where 'eight' and 'two' overlap.
                //
                // So instead of replacing the word with a number, we replace the first character of the word with
                // a number. i.e. instead of 'eigh23' we have '8igh2wo3hree'
                sbText[wordIndex] = i.ToString()[0];
            }
        }

        return sbText.ToString();
    }
    
    private static int FirstDigit(string n)
    {
        return (int)char.GetNumericValue(n.First(char.IsNumber));
    }

    private static int LastDigit(string n)
    {
        return (int)char.GetNumericValue(n.Last(char.IsNumber));
    }

    [Test]
    public void Part1_sums_to_55816()
    {
        var lines = File.ReadLines("Day01.input.txt").ToList();
        var sum = lines.Sum(n => FirstDigit(n) * 10 + LastDigit(n));
        Assert.That(sum, Is.EqualTo(55816));
    }
    
    [Test]
    public void Part2_sums_to_()
    {
        var original = File.ReadLines("Day01.input.txt").ToList();
        var lines = original.Select(ReplaceWordsWithNumbers);
        var sum = lines.Sum(n => FirstDigit(n) * 10 + LastDigit(n));
        
        Assert.That(sum, Is.EqualTo(54980));
    }
}
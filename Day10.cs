using System.Numerics;
using System.Text.RegularExpressions;

namespace AdventOfCode23;

public class Day10
{
    private static readonly Vector2 North = new (0, -1);
    private static readonly Vector2 South = new (0, 1);
    private static readonly Vector2 East = new (1, 0);
    private static readonly Vector2 West = new (-1, 0);

    private static readonly Dictionary<string, List<Vector2>> PipeSupport = new()
    {
        { "|", [North, South] },
        { "-", [West, East] },
        { "L", [North, East] },
        { "J", [North, West] },
        { "7", [West, South] },
        { "F", [East, South] },
        { ".", [] },
        { "S", [North, South, East, West] },
    };

    private class Node(string type, int x, int y)
    {
        public Vector2 Location { get; } = new (x, y);
        
        public string Type => type;
        
        public int? Distance { get; set; }

        public bool IsPartOfLoop => Distance != null;
    }

    private class Nodes
    {
        private readonly Dictionary<Vector2, Node> _nodes = new();

        public int MaxDistance { get; }

        public int NumEnclosed { get; }
        
        public Nodes(IEnumerable<Node> nodes)
        {
            Node? startingPoint = null;

            foreach (var node in nodes)
            {
                if (node.Type == "S") startingPoint = node;
                _nodes[node.Location] = node;
            }

            if (startingPoint == null)
            {
                throw new Exception("Cannot initialise: Starting point not found");
            }
            
            MaxDistance = TraverseLoop(startingPoint);
            NumEnclosed = CalculateNumEnclosed();
        }

        private int TraverseLoop(Node StartingPoint)
        {
            List<Node> nextToCheck = [StartingPoint];

            var distance = -1;
            
            while (nextToCheck.Count > 0)
            {
                distance++;
                
                var connections = new List<Node>();
                
                foreach (var currentNode in nextToCheck)
                {
                    currentNode.Distance = distance;

                    foreach (var direction in PipeSupport[currentNode.Type])
                    {
                        var potentialNext = _nodes.GetValueOrDefault(currentNode.Location + direction);
                        if (potentialNext == null) continue;

                        var canConnect = potentialNext.Distance == null && PipeSupport[potentialNext.Type].Contains(direction * -1);
                        if (canConnect)
                        {
                            connections.Add(potentialNext);
                        }
                    }
                }
                
                nextToCheck = connections;
            }

            return distance;
        }

        // https://en.wikipedia.org/wiki/Even%E2%80%93odd_rule
        private int CalculateNumEnclosed()
        {
            var outerNodeKeys = new HashSet<Node>();
            var insideNodeKeys = new HashSet<Node>();

            foreach (var node in _nodes.Values.Where(node => !node.IsPartOfLoop))
            {
                int loopCollisions = 0;
                var xCoord = node.Location.X + 1;
                
                while (_nodes.TryGetValue(new Vector2(xCoord, node.Location.Y), out var hitNode))
                {
                    if (hitNode.IsPartOfLoop && hitNode.Type != "-" && hitNode.Type != "J" && hitNode.Type != "L") loopCollisions++;
                    xCoord++;
                }

                if (loopCollisions % 2 == 0)
                {
                    outerNodeKeys.Add(node);
                }
                else
                {
                    insideNodeKeys.Add(node);
                }
            }

            return insideNodeKeys.Count;
        }
   }

    private static Nodes ReadAllNodes(string filename)
    {
        var lines = File.ReadAllLines(filename);
        var regex = new Regex("[^\r\n]");
        
        var nodes = new List<Node>();
        
        for (var lineNum = 0; lineNum < lines.Length; lineNum++)
        {
            var line = lines[lineNum];
            var matches = regex.Matches(line);

            foreach (Match match in matches)
            {
                nodes.Add(new Node(match.Value, match.Index, lineNum));
            }
        }

        return new Nodes(nodes);
    }

    [Test]
    public void Part1_sample1_equals_4()
    {
        var nodes = ReadAllNodes("Day10.part1.sample1.txt");
        Assert.That(nodes.MaxDistance, Is.EqualTo(4));
    }
    
    [Test]
    public void Part1_sample2_equals_8()
    {
        var nodes = ReadAllNodes("Day10.part1.sample2.txt");
        Assert.That(nodes.MaxDistance, Is.EqualTo(8));
    }
    
    [Test]
    public void Part1_input_equals_6786()
    {
        var nodes = ReadAllNodes("Day10.input.txt");
        Assert.That(nodes.MaxDistance, Is.EqualTo(6786));
    }
    
    [Test]
    public void Part2_sample1_equals_4()
    {
        var nodes = ReadAllNodes("Day10.part2.sample1.txt");
        Assert.That(nodes.NumEnclosed, Is.EqualTo(4));
    }
    
    [Test]
    public void Part2_sample2_equals_8()
    {
        var nodes = ReadAllNodes("Day10.part2.sample2.txt");
        Assert.That(nodes.NumEnclosed, Is.EqualTo(8));
    }
    
    [Test]
    public void Part2_input_equals_495()
    {
        var nodes = ReadAllNodes("Day10.input.txt");
        Assert.That(nodes.NumEnclosed, Is.EqualTo(495));
    }
}
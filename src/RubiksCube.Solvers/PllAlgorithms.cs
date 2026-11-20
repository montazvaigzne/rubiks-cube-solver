using RubiksCube.Core;

namespace RubiksCube.Solvers;

// All PLL (permutation of the last layer) algorithms in one place. (2 look PLL, beginner version)
// The algorithms are written for the solver's fixed orientation:
//   Up    = White
//   Down  = Yellow 
//   Front = Green

// 2 look PLL:
//   1. SolveCornerPermutation: put the 4 corners in the correct slots (Adjacent / Diagonal swap)
//   2. SolveEdgePermutation:   put the 4 edges in the correct slots (3-edge cycles / 4-edge swaps)

// The dictionaries store ONE algorithm per case. A case can show up in 4 rotations,
// so the lookup tables below add the D', D2 and D turns automatically (like the OLL file).
public static class PllAlgorithms
{
    // CORNER PERMUTATION
    // Each algorithm moves only 2 of the 4 corners and leaves the other 2 in place.
    // "Diagonal" and "Adjacent" describe where those 2 corners sit relative to each other.
    public static readonly Dictionary<string, string> CornerCases = new()
    {
        // Diagonal pair: Down-Left-Front <-> Down-Right-Back
        ["Diagonal"] = "F L D' L' D' L D L' F' L D L' D' L' F L F'",

        // Adjacent pair: Down-Left-Front <-> Down-Back-Left
        ["Adjacent"] = "L D L' D' L' F L2 D' L' D' L D L' F'"
    };

    // EDGE PERMUTATION
    // "3-Edges" cycles 3 of the Down edges (Down-Back is the one left alone), "4-Edges" swaps 2 pairs.
    public static readonly Dictionary<string, string> EdgeCases = new()
    {
        // Down-Front -> Down-Left -> Down-Right -> Down-Front
        ["3-Edges-1"] = "L D' L D L D L D' L' D' L2",

        // Down-Front -> Down-Right -> Down-Left -> Down-Front
        ["3-Edges-2"] = "L2 D L D L' D' L' D' L' D L'",

        // Swaps the 2 opposite pairs: Down-Front <-> Down-Back, Down-Right <-> Down-Left
        ["4-Edges-1"] = "M2 D' M2 D2 M2 D' M2",

        // Swaps the 2 adjacent pairs: Down-Front <-> Down-Left, Down-Right <-> Down-Back
        ["4-Edges-2"] = "M2 D M2 D M D2 M2 D2 M D2"
    };

    // Lookup tables: Down layer permutation -> move sequence that solves it
    // Every case and every rotation is written out as one word, so no searching is needed
    // (24 corner permutations and 12 edge permutations are covered)
    public static readonly Dictionary<string, string> CornerLookup = BuildCornerLookup();
    public static readonly Dictionary<string, string> EdgeLookup = BuildEdgeLookup();

    // 4 characters that describe which corner piece sits in each Down corner slot
    // Order: Down-Front-Right, Down-Left-Front, Down-Back-Left, Down-Right-Back
    // A solved cube reads "0123"
    public static string CornerSignature(Cube cube)
    {
        char downFrontRight = CornerId(cube, 26, 15);
        char downLeftFront = CornerId(cube, 24, 44);
        char downBackLeft = CornerId(cube, 53, 42);
        char downRightBack = CornerId(cube, 17, 51);

        return string.Concat(downFrontRight, downLeftFront, downBackLeft, downRightBack);
    }

    // Identifies a corner by its two non-yellow stickers
    // After OLL the whole Down face is yellow, so these two stickers are on the sides
    private static char CornerId(Cube cube, int side1, int side2)
    {
        Color first = cube.GetColor(side1);
        Color second = cube.GetColor(side2);

        if (IsPair(first, second, Color.Green, Color.Red)) return '0';      // Down-Front-Right
        if (IsPair(first, second, Color.Green, Color.Orange)) return '1';   // Down-Left-Front
        if (IsPair(first, second, Color.Blue, Color.Orange)) return '2';    // Down-Back-Left
        if (IsPair(first, second, Color.Red, Color.Blue)) return '3';       // Down-Right-Back

        return '?';
    }

    // A corner is the same corner no matter which of its two stickers is read first
    private static bool IsPair(Color first, Color second, Color color1, Color color2)
    {
        return (first == color1 && second == color2) || (first == color2 && second == color1);
    }

    // 4 characters that describe which edge piece sits in each Down edge slot
    // Order: Down-Front, Down-Right, Down-Back, Down-Left
    // A solved cube reads "0123"
    public static string EdgeSignature(Cube cube)
    {
        char downFront = EdgeId(cube.GetColor(25));
        char downRight = EdgeId(cube.GetColor(16));
        char downBack = EdgeId(cube.GetColor(52));
        char downLeft = EdgeId(cube.GetColor(43));

        return string.Concat(downFront, downRight, downBack, downLeft);
    }

    // An edge is identified by the sticker that is not yellow
    private static char EdgeId(Color color)
    {
        if (color == Color.Green) return '0';
        if (color == Color.Red) return '1';
        if (color == Color.Blue) return '2';
        if (color == Color.Orange) return '3';

        return '?';
    }

    // A corner case can show up in 4 rotations, and after the algorithm the last layer
    // may still need to be turned back into place, so every algorithm gets 4 starting
    // turns and 4 ending turns (16 words). That way one algorithm is always enough.
    private static Dictionary<string, string> BuildCornerLookup()
    {
        var words = new List<string>();

        foreach (var algorithm in CornerCases.Values)
        {
            for (int position = 0; position < 4; position++)
            {
                for (int suffix = 0; suffix < 4; suffix++)
                {
                    words.Add(WithPrefix(algorithm, position) + WithSuffix(suffix));
                }
            }
        }

        // These 3 states are already solved, only the last layer is turned
        words.Add("D");
        words.Add("D2");
        words.Add("D'");

        var lookup = LookupBuilder.Build(words, CornerSignature);
        lookup["0123"] = "";      // already solved, nothing to do
        return lookup;
    }

    // Edge algorithms only move edges, so a rotation would break the solved corners
    // Each rotated version therefore gets a D turn in front and the opposite D turn behind
    private static Dictionary<string, string> BuildEdgeLookup()
    {
        var words = new List<string>();

        foreach (var algorithm in EdgeCases.Values)
        {
            for (int position = 0; position < 4; position++)
            {
                words.Add(WithPrefix(algorithm, position) + WithSuffix(position));
            }
        }

        var lookup = LookupBuilder.Build(words, EdgeSignature);
        lookup["0123"] = "";      // already solved, nothing to do
        return lookup;
    }

    private static string WithPrefix(string algorithm, int position)
    {
        if (position == 0) return algorithm;
        if (position == 1) return "D' " + algorithm;
        if (position == 2) return "D2 " + algorithm;
        return "D " + algorithm;
    }

    private static string WithSuffix(int position)
    {
        if (position == 0) return "";
        if (position == 1) return " D";
        if (position == 2) return " D2";
        return " D'";
    }
}

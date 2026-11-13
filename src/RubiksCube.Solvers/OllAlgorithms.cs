using RubiksCube.Core;

namespace RubiksCube.Solvers;

// All OLL (last layer orientation) algorithms in one place. (2 look OLL, the beginner version)
// The algorithms are already written for the solver's fixed orientation:
//   Up    = White 
//   Down  = Yellow 
//   Front = Green

// The dictionaries below store ONE algorithm per case (the position 0 one).
// Every case can show up in 4 rotations on the Down face, so BuildLookup expands
// each algorithm into its 4 positions by pre-turning the last layer with D', D2 and D.

public static class OllAlgorithms
{
    // EDGE ORIENTATION (the yellow cross on the Down face)
    public static readonly Dictionary<string, string> EdgeCases = new()
    {
        ["Dot"] = "L D2 L2 F L F' D2 L' F L F'",   // Dot is symmetric, the extra positions collapse into it
        ["L"] = "F D L D' L' F'",
        ["Line"] = "F L D L' D' F'"
    };

    // CORNER ORIENTATION (the whole yellow face)
    public static readonly Dictionary<string, string> CornerCases = new()
    {
        ["Sune"] = "L D L' D L D2 L'",
        ["Antisune"] = "R' D' R D' R' D2 R",
        ["H"] = "F L D L' D' L D L' D' L D L' D' F'",
        ["Pi"] = "L D2 L2 D' L2 D' L2 D2 L",
        ["L"] = "L' F L B' L' F' L B",
        ["T"] = "R F L' F' R' F L F'",
        ["U"] = "L2 U L' D2 L U' L' D2 L'"
    };

    // Lookup tables: orientation signature of the cube TO algorithm to execute.
    // They are built from the case tables above, so every case and every position is covered.
    public static readonly Dictionary<string, string> EdgeLookup = BuildLookup(EdgeCases, EdgeSignature);
    public static readonly Dictionary<string, string> CornerLookup = BuildLookup(CornerCases, CornerSignature);

    // 4 characters that describe which of the 4 Down edges are already oriented.
    // Order: Front(28), Right(32), Back(34), Left(30). Y = yellow on the Down face.
    public static string EdgeSignature(Cube cube)
    {
        return string.Concat(
            cube.GetColor(28) == Color.Yellow ? "Y" : "n",
            cube.GetColor(32) == Color.Yellow ? "Y" : "n",
            cube.GetColor(34) == Color.Yellow ? "Y" : "n",
            cube.GetColor(30) == Color.Yellow ? "Y" : "n");
    }

    // 4 characters that describe where the yellow sticker of each Down corner is.
    // D = Down, F/B/L/R = the side face that holds the yellow sticker.
    // Order: DFR, DLF, DBL, DRB.
    public static string CornerSignature(Cube cube)
    {
        return string.Concat(
            CornerYellowFace(cube, 29, 26, 15, 'F', 'R'), // Down-Front-Right
            CornerYellowFace(cube, 27, 24, 44, 'F', 'L'), // Down-Left-Front
            CornerYellowFace(cube, 33, 53, 42, 'B', 'L'), // Down-Back-Left
            CornerYellowFace(cube, 35, 17, 51, 'R', 'B')); // Down-Right-Back
    }

    // Returns the face that holds the yellow sticker of one corner
    private static char CornerYellowFace(Cube cube, int down, int side1, int side2, char face1, char face2)
    {
        if (cube.GetColor(down) == Color.Yellow) return 'D';
        if (cube.GetColor(side1) == Color.Yellow) return face1;
        return face2;
    }

    // Turns one algorithm into the algorithms for the 4 positions of its case.
    // Position 0 is the algorithm as written. For the other positions the last layer
    // is first turned back to position 0 (D', D2 or D) and then the algorithm runs.
    // Only D turns are used, so the solved first two layers are never touched.

    // Position 0 : Front green
    // Position 1 : Front red
    // Position 2 : Front blue
    // Position 3 : Front orange
    private static string[] AllPositions(string algorithm)
    {
        return
        [
            algorithm,              // position 0
            "D' " + algorithm,      // position 1
            "D2 " + algorithm,      // position 2
            "D " + algorithm        // position 3
        ];
    }

    // Builds a "signature TO algorithm" lookup from a case table.
    // Applying the inverse algorithm to a solved cube creates the exact case that
    // the algorithm solves, so we can read the signature of every position from it.
    private static Dictionary<string, string> BuildLookup(
        Dictionary<string, string> cases,
        Func<Cube, string> signature)
    {
        var lookup = new Dictionary<string, string>();

        foreach (var algorithm in cases.Values)
        {
            foreach (var position in AllPositions(algorithm))
            {
                var cube = new Cube();
                MoveInterpreter.ExecuteMove(cube, Invert(position));

                // TryAdd keeps the first algorithm if two positions have the same
                // signature (this happens for symmetric cases like Dot and H)
                lookup.TryAdd(signature(cube), position);
            }
        }

        return lookup;
    }

    // Reverses a move sequence and inverts every move (e.g. "R U2 F'" -> "F U2 R'")
    private static string Invert(string moves)
    {
        return string.Join(" ", moves
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Reverse()
            .Select(move =>
            {
                if (move.Contains('2')) return move[0] + "2";
                return move.Contains('\'') ? move[0].ToString() : move + "'";
            }));
    }
}

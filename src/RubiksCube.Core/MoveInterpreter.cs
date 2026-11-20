namespace RubiksCube.Core;

public static class MoveInterpreter
{
    // Method to execute one or more moves on the cube based on the move string
    // (e.g. "R", "R'", "R2", or a whole sequence like "R2 D' F")
    public static void ExecuteMove(Cube cube, string moves)
    {
        if (string.IsNullOrWhiteSpace(moves)) return;

        foreach (var move in moves.Split(' ', StringSplitOptions.RemoveEmptyEntries))
        {
            // Check if the move is clockwise or counter-clockwise
            bool clockwise = !move.Contains('\'');

            // A "2" suffix means a double turn (the face is rotated twice)
            int turns = move.Contains('2') ? 2 : 1;

            // Get the face character (U, R, F, D, L, B)
            char face = move[0];

            for (int i = 0; i < turns; i++)
            {
                switch (face)
                {
                    case 'U': cube.MoveU(clockwise); break;
                    case 'D': cube.MoveD(clockwise); break;
                    case 'R': cube.MoveR(clockwise); break;
                    case 'L': cube.MoveL(clockwise); break;
                    case 'M': cube.MoveM(clockwise); break;
                    case 'F': cube.MoveF(clockwise); break;
                    case 'B': cube.MoveB(clockwise); break;
                }
            }
        }
    }

    // Returns the inverse of a move sequence (e.g. "R U2 F'" -> "F U2 R'")
    public static string Invert(string moves)
    {
        return string.Join(" ", moves
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Reverse()
            .Select(move =>
            {
                // A double turn is its own inverse
                if (move.Contains('2')) return move[0] + "2";

                return move.Contains('\'') ? move[0].ToString() : move + "'";
            }));
    }
}

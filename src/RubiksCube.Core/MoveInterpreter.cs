namespace RubiksCube.Core;

public static class MoveInterpreter
{
    // Method to execute a move on the cube based on the move string
    public static void ExecuteMove(Cube cube, string move)
    {
        if (string.IsNullOrWhiteSpace(move)) return;

        // Check if the move is clockwise or counter-clockwise
        bool clockwise = !move.Contains('\'');

        // Get the face character (U, R, F, D, L, B)
        char face = move[0];

        switch (face)
        {
            case 'U': cube.MoveU(clockwise); break;
            case 'D': cube.MoveD(clockwise); break;
            case 'R': cube.MoveR(clockwise); break;
            case 'L': cube.MoveL(clockwise); break;
            case 'F': cube.MoveF(clockwise); break;
            case 'B': cube.MoveB(clockwise); break;
        }
    }
}
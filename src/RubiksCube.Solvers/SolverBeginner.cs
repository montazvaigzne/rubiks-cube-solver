using RubiksCube.Core;

namespace RubiksCube.Solvers;

public class SolverBeginner
{
    // Finds the edge piece on the cube that matches the two specified colors
    public static (int, int) FindEdge(Cube cube, Color color1, Color color2)
    {
        // Iterate through all edge positions defined in CubeMap
        foreach (var edge in CubeMap.EdgePositions)
        {
            if (IsEdgeMatch(cube, edge[0], edge[1], color1, color2))
            {
                return (edge[0], edge[1]);
            }
        }

        return (-1, -1); // If no matching edge is found, return an invalid index pair
    }

    // Checks if the colors at the specified indices of the cube match the given colors, regardless of order
    private static bool IsEdgeMatch(Cube cube, int index1, int index2, Color color1, Color color2)
    {
        if ((cube.GetColor(index1) == color1 && cube.GetColor(index2) == color2) ||
           (cube.GetColor(index1) == color2 && cube.GetColor(index2) == color1))
        {
            return true;
        }
        return false;
    }

    // Checks if the colors at the specified indices of the cube match the given colors, regardless of order    
    private static bool IsCornerMatch(Cube cube, int index1, int index2, int index3, Color color1, Color color2, Color color3)
    {
        var colors = new[] { cube.GetColor(index1), cube.GetColor(index2), cube.GetColor(index3) };
        return colors.Contains(color1) && colors.Contains(color2) && colors.Contains(color3);
    }

    // Finds the corner piece on the cube that matches the three specified colors
    public static (int, int, int) FindCorner(Cube cube, Color color1, Color color2, Color color3)
    {
        foreach (var corner in CubeMap.CornerPositions)
        {
            if (IsCornerMatch(cube, corner[0], corner[1], corner[2], color1, color2, color3))
            {
                return (corner[0], corner[1], corner[2]);
            }
        }

        return (-1, -1, -1);
    }

    //Helper method to rotate the Down face until the specified index is free of a white edge piece
    public static void FreeUpSpace(Cube cube, int index)
    {
        while (cube.GetColor(index) == Color.White) cube.MoveD(true);
    }
    // Solves the Daisy pattern on the cube, ensuring that all four white edge pieces are positioned correctly on the Down face
    public static void SolveDaisy(Cube cube)
    {
        int failsafe = 0;

        while (!(cube.GetColor(28) == Color.White && cube.GetColor(30) == Color.White &&
                 cube.GetColor(32) == Color.White && cube.GetColor(34) == Color.White))
        {
            if (++failsafe > 50) break; // Failsafe to prevent infinite loops

            bool movedAnEdge = false;

            foreach (var edge in CubeMap.EdgePositions)
            {
                int wIdx = cube.GetColor(edge[0]) == Color.White ? edge[0] :
                          (cube.GetColor(edge[1]) == Color.White ? edge[1] : -1);

                if (wIdx == -1) continue;
                if (wIdx == 28 || wIdx == 30 || wIdx == 32 || wIdx == 34) continue;

                // Use ONLY wIdx to decide what to do
                switch (wIdx)
                {
                    // 1. DIRECT DROPS FROM MIDDLE LAYER TO DAISY
                    case 12: FreeUpSpace(cube, 28); cube.MoveF(true); break;
                    case 14: FreeUpSpace(cube, 34); cube.MoveB(true); break;
                    case 21: FreeUpSpace(cube, 30); cube.MoveL(true); break;
                    case 23: FreeUpSpace(cube, 32); cube.MoveR(true); break;

                    case 39: FreeUpSpace(cube, 34); cube.MoveB(false); break;
                    case 41: FreeUpSpace(cube, 28); cube.MoveF(false); break;
                    case 48: FreeUpSpace(cube, 32); cube.MoveR(true); break; //added a fix here, it was previously cube.MoveL(false) // right version:cube.MoveR(true);
                    case 50: FreeUpSpace(cube, 30); cube.MoveL(false); break; //added a fix here, it was previously cube.MoveR(false); //right version:cube.MoveL(false);


                    // 2. DIRECT DROPS FROM UP FACE (Facing UP)
                    case 7: FreeUpSpace(cube, 28); cube.MoveF(true); cube.MoveF(true); break;
                    case 5: FreeUpSpace(cube, 32); cube.MoveR(true); cube.MoveR(true); break;
                    case 1: FreeUpSpace(cube, 34); cube.MoveB(true); cube.MoveB(true); break;
                    case 3: FreeUpSpace(cube, 30); cube.MoveL(true); cube.MoveL(true); break;


                    // 3. CHAINED MOVES (Move to middle layer, then drop immediately)
                    case 19: cube.MoveF(false); FreeUpSpace(cube, 30); cube.MoveL(true); break;
                    case 10: cube.MoveR(false); FreeUpSpace(cube, 28); cube.MoveF(true); break;
                    case 46: cube.MoveB(false); FreeUpSpace(cube, 32); cube.MoveL(true); break;
                    case 37: cube.MoveL(true); FreeUpSpace(cube, 28); cube.MoveF(false); break;

                    // Bottom layer facing sideways (Push up to Middle, then drop immediately)
                    case 25: cube.MoveF(true); FreeUpSpace(cube, 30); cube.MoveL(true); break;
                    case 16: cube.MoveR(true); FreeUpSpace(cube, 28); cube.MoveF(true); break;
                    case 52: cube.MoveB(true); FreeUpSpace(cube, 32); cube.MoveR(true); break;
                    case 43: cube.MoveL(true); FreeUpSpace(cube, 28); cube.MoveF(false); break;
                }

                movedAnEdge = true;
                break; // Restart the while loop to re-evaluate
            }

            if (!movedAnEdge) break;
        }
    }

    // Checks if the white cross is already solved (all 4 edges are in place)
    private static bool IsWhiteCrossSolved(Cube cube)
    {
        return cube.GetColor(7) == Color.White && cube.GetColor(19) == Color.Green &&
               cube.GetColor(5) == Color.White && cube.GetColor(10) == Color.Red &&
               cube.GetColor(1) == Color.White && cube.GetColor(46) == Color.Blue &&
               cube.GetColor(3) == Color.White && cube.GetColor(37) == Color.Orange;
    }

    // Main method to solve the white cross on the cube
    public static void SolveWhiteCross(Cube cube)
    {
        // If the cross is already solved, we don't need to do anything
        if (IsWhiteCrossSolved(cube)) return;

        // Ensure Daisy is built first
        SolveDaisy(cube);

        // 1. Insert Front edge (White-Green)
        // Rotate D until the side color at 25 is Green AND the bottom color at 28 is White
        while (cube.GetColor(25) != Color.Green || cube.GetColor(28) != Color.White)
        {
            cube.MoveD(true);
        }
        // Insert to Up face
        cube.MoveF(true);
        cube.MoveF(true);
        
        // 2. Insert Right edge (White-Red)
        // Rotate D until the side color at 16 is Red AND the bottom color at 32 is White
        while (cube.GetColor(16) != Color.Red || cube.GetColor(32) != Color.White)
        {
            cube.MoveD(true);
        }
        // Insert to Up face
        cube.MoveR(true);
        cube.MoveR(true);

        // 3. Insert Back edge (White-Blue)
        // Rotate D until the side color at 52 is Blue AND the bottom color at 34 is White
        while (cube.GetColor(52) != Color.Blue || cube.GetColor(34) != Color.White)
        {
            cube.MoveD(true);
        }
        // Insert to Up face
        cube.MoveB(true);
        cube.MoveB(true);

        // 4. Insert Left edge (White-Orange)
        // Rotate D until the side color at 43 is Orange AND the bottom color at 30 is White
        while (cube.GetColor(43) != Color.Orange || cube.GetColor(30) != Color.White)
        {
            cube.MoveD(true);
        }
        // Insert to Up face
        cube.MoveL(true);
        cube.MoveL(true);
    }

    // Main method to solve all 4 white corners of the first layer
    public static void SolveFirstLayer(Cube cube)
    {
        // 1. White-Green-Red corner (Front-Right)
        // U layer: Up(8), Front(20), Right(9)
        // D layer: Down(29), Front(26), Side=Right(15)
        SolveCorner(cube, Color.Green, Color.Red,
            8, 20, 9,
            29, 26, 15,
            "R' D D R D R' D' R", "F D F'", "R' D' R");

        // 2. White-Green-Orange corner (Front-Left)
        // U layer: Up(6), Front(18), Right(38) (Right here means the side face of the corner)
        // D layer: Down(27), Front(24), Side=Left(44)
        SolveCorner(cube, Color.Green, Color.Orange,
            6, 18, 38,
            27, 24, 44,
            "F' D D F D F' D' F", "F' D' F", "L D L'");

        // 3. White-Blue-Orange corner (Back-Left)
        // U layer: Up(0), Front(47), Right(36)
        // D layer: Down(33), Front(53), Side=Left(42)
        SolveCorner(cube, Color.Blue, Color.Orange,
            0, 47, 36,
            33, 53, 42,
            "L' D D L D L' D' L", "B D B'", "L' D' L");

        // 4. White-Blue-Red corner (Back-Right)
        // U layer: Up(2), Front(45), Right(11)
        // D layer: Down(35), Front(51), Side=Right(17)
        SolveCorner(cube, Color.Blue, Color.Red,
            2, 45, 11,
            35, 51, 17,
            "B' D D B D B' D' B", "B' D' B", "R D R'");
    }

    // Helper method to find, position, and insert a specific corner piece
    private static void SolveCorner(Cube cube, Color colorF, Color colorR,
                                    int uUp, int uFront, int uRight,       // U layer target (e.g., 8, 20, 9)
                                    int dDown, int dFront, int dSide,      // D layer target (e.g., 29, 26, 15)
                                    string algWhiteDown, string algWhiteFront, string algWhiteRight)
    {
        int failsafe = 0;

        // Check if the corner is already solved (using U layer targets)
        while (!(cube.GetColor(uUp) == Color.White &&
                 cube.GetColor(uFront) == colorF &&
                 cube.GetColor(uRight) == colorR))
        {
            if (++failsafe > 100) break;

            var pos = FindCorner(cube, Color.White, colorF, colorR);

            // Check if the corner is in the Up layer (0-8)
            bool inULayer = (pos.Item1 <= 8 || pos.Item2 <= 8 || pos.Item3 <= 8);

            if (inULayer)
            {
                if (pos == (8, 9, 20)) MoveInterpreter.ExecuteMove(cube, "R' D R");
                else if (pos == (6, 18, 38)) MoveInterpreter.ExecuteMove(cube, "L D L'");
                else if (pos == (2, 45, 11)) MoveInterpreter.ExecuteMove(cube, "R D R'");
                else MoveInterpreter.ExecuteMove(cube, "L' D L");
            }
            else
            {
                // If in D layer, rotate D until it aligns with the target D coordinates
                while (!((pos.Item1 == dDown || pos.Item2 == dDown || pos.Item3 == dDown) &&
                         (pos.Item1 == dFront || pos.Item2 == dFront || pos.Item3 == dFront) &&
                         (pos.Item1 == dSide || pos.Item2 == dSide || pos.Item3 == dSide)))
                {
                    cube.MoveD(true);
                    pos = FindCorner(cube, Color.White, colorF, colorR);
                }

                // Now bring it UP to the target U coordinates based on orientation
                // Check which D-index has the white color
                if (cube.GetColor(dDown) == Color.White)
                {
                    // White is on the Down face
                    MoveInterpreter.ExecuteMove(cube, algWhiteDown);
                }
                else if (cube.GetColor(dFront) == Color.White)
                {
                    // White is on the Front/Back face
                    MoveInterpreter.ExecuteMove(cube, algWhiteFront);
                }
                else
                {
                    // White is on the Side face
                    MoveInterpreter.ExecuteMove(cube, algWhiteRight);
                }
            }
        }
    }
}

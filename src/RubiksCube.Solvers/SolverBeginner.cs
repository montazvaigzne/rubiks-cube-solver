using System;
using System.Collections.Generic;
using System.Text;
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

    // The Daisy is already built (all white edges are on the Down face).
    // need to align the side colors with the centers and insert them to the Up face.
    public static void SolveWhiteCross(Cube cube)
    {
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
}

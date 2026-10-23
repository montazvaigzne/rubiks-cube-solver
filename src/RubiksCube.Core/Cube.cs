namespace RubiksCube.Core;

public class Cube
{
    public Color[] State { get; private set; }
    public Cube()
    {
        State = new Color[54];
        for (int i = 0; i < 54; i++)
        {
            if (i < 9) State[i] = Color.White;          // Up
            else if (i < 18) State[i] = Color.Red;      // Right
            else if (i < 27) State[i] = Color.Green;    // Front
            else if (i < 36) State[i] = Color.Yellow;   // Down
            else if (i < 45) State[i] = Color.Orange;   // Left
            else State[i] = Color.Blue;                 // Back
        }
    }
    // This method returns the color of the tile at the given index (0-53)
    public Color GetColor(int index)
    {
        if (index < 0 || index >= 54)
            throw new ArgumentOutOfRangeException(nameof(index), "Index must be between 0 and 53.");
        return State[index];
    }
    // This method sets the color of the tile at the given index (0-53)
    public void SetColor(int index, Color color)
    {
        if (index < 0 || index >= 54)
            throw new ArgumentOutOfRangeException(nameof(index), "Index must be between 0 and 53.");
        State[index] = color;
    }
    // This method rotates a face of the cube (0-5) either clockwise or counter-clockwise
    public void RotateFace (Face face, bool clockwise)
    {
        int startIndex = (int)face * 9;
        Color[] temp = new Color[9];

        for (int i = 0; i < 9; i++)
        {
            temp[i] = GetColor(startIndex + i);
        }

        // Rotate clockwise
        if (clockwise)
        {
            for (int i = 0; i < 9; i++)
            {
                int newRow = i / 3; 
                int newCol = i % 3; 
                int oldRow = 2 - newCol; 
                int oldCol = newRow; 
                int oldIndex = oldRow * 3 + oldCol; 

                SetColor(startIndex + i, temp[oldIndex]);
            }

        }
        // Rotate counter-clockwise
        else
        {
            for (int i = 0; i < 9; i++)
            {
                int newRow = i / 3;
                int newCol = i % 3;
                int oldRow = newCol;
                int oldCol = 2 - newRow;
                int oldIndex = oldRow * 3 + oldCol;

                SetColor(startIndex + i, temp[oldIndex]);
            }
        }
    }

    // This method cycles the colors of the tiles on the edges of four faces of the cube that are around the face being rotated.
    // The order of the faces is important and should be specified in a clockwise manner.
    private void RotateAdjacentOuterLinesOfFace(int[] face1, int[] face2, int[] face3, int[] face4, bool clockwise)
    {
        Color[] temp = new Color[3];

        for (int i = 0; i < 3; i++)
        {
            temp[i] = GetColor(face1[i]);
        }

        if (clockwise)
        {
            // Move the colors in a clockwise direction 
            // face4 -> face1
            for (int i = 0; i < 3; i++) SetColor(face1[i], GetColor(face4[i]));

            // face3 -> face4
            for (int i = 0; i < 3; i++) SetColor(face4[i], GetColor(face3[i]));

            // face2 -> face3
            for (int i = 0; i < 3; i++) SetColor(face3[i], GetColor(face2[i]));

            // temp (kas bija face1) -> face2
            for (int i = 0; i < 3; i++) SetColor(face2[i], temp[i]);
        }
        else
        {
            // Move the colors in a counter-clockwise direction 
            // face2 -> face1
            for (int i = 0; i < 3; i++) SetColor(face1[i], GetColor(face2[i]));

            // face3 -> face2
            for (int i = 0; i < 3; i++) SetColor(face2[i], GetColor(face3[i]));

            // face4 -> face3
            for (int i = 0; i < 3; i++) SetColor(face3[i], GetColor(face4[i]));

            // temp (kas bija face1) -> face4
            for (int i = 0; i < 3; i++) SetColor(face4[i], temp[i]);
        }
    }

    // =================================================================================
    // MOVE METHODS
    // The logic for these arrays is based on a 3D cube visualization (see cubeHelper.html).
    // RULE: Arrays must be ordered so that index [0] of one face physically touches 
    // index [0] of the next face.
    // SOME ARRAYS ARE DESCENDING (REVERSED): This happens when moving pieces across
    // faces that are oriented differently (e.g., moving from Right to Back, or Up to Down).
    // Viewing the cube from the front, some faces act like a mirror, so their indices
    // must be read backwards to keep the corner pieces (cubies) from breaking apart.
    // =================================================================================
    public void MoveU(bool clockwise)
    {
        RotateFace(Face.Up, clockwise);

        int[] topFront = [18, 19, 20];
        int[] topLeft = [36, 37, 38];
        int[] topBack = [45, 46, 47];
        int[] topRight = [9, 10, 11];

        RotateAdjacentOuterLinesOfFace(topFront, topLeft, topBack, topRight, clockwise);
    }

    public void MoveD(bool clockwise)
    {
        RotateFace(Face.Down, clockwise);

        int[] bottomFront = [24, 25, 26];
        int[] bottomRight = [15, 16, 17];
        int[] bottomBack = [51, 52, 53];
        int[] bottomLeft = [42, 43, 44];

        RotateAdjacentOuterLinesOfFace(bottomFront, bottomRight, bottomBack, bottomLeft, clockwise);
    }

    public void MoveR(bool clockwise)
    {
        RotateFace(Face.Right, clockwise);

        int[] rightFront = [20, 23, 26];
        int[] rightUp = [2, 5, 8];
        int[] leftBack = [51, 48, 45]; // Reversed: connects Up (2) to Back (51)
        int[] rightDown = [29, 32, 35];

        RotateAdjacentOuterLinesOfFace(rightFront, rightUp, leftBack, rightDown, clockwise);
    }

    public void MoveL(bool clockwise)
    {
        RotateFace(Face.Left, clockwise);

        int[] leftUp = [0, 3, 6];
        int[] leftFront = [18, 21, 24];
        int[] leftDown = [27, 30, 33];
        int[] rightBack = [53, 50, 47]; // Reversed: connects Down (33) to Back (53)

        RotateAdjacentOuterLinesOfFace(leftUp, leftFront, leftDown, rightBack, clockwise);
    }

    public void MoveF(bool clockwise)
    {
        RotateFace(Face.Front, clockwise);

        int[] bottomUp = [6, 7, 8];
        int[] leftRight = [9, 12, 15];
        int[] topDown = [29, 28, 27]; // Reversed: connects Right (15) to Down (29)
        int[] rightLeft = [44, 41, 38];

        RotateAdjacentOuterLinesOfFace(bottomUp, leftRight, topDown, rightLeft, clockwise);
    }

    public void MoveB(bool clockwise)
    {
        RotateFace(Face.Back, clockwise);

        int[] topUp = [0, 1, 2];
        int[] leftLeft = [42, 39, 36]; // Reversed: connects Up (2) to Left (42)
        int[] bottomDown = [35, 34, 33]; // Reversed: connects Left (36) to Down (35)
        int[] rightRight = [11, 14, 17];

        RotateAdjacentOuterLinesOfFace(topUp, leftLeft, bottomDown, rightRight, clockwise);
    }

    public bool IsSolved()
    {
        for (int i = 0; i < 54; i+=9) // Check each face (0, 9, 18, 27, 36, 45)
        {
            for (int j = 1; j < 9; j++) // Check the other 8 tiles of the face
            {
                if (State[i] != State[i + j])
                    return false;
            }
        }
        return true;
    }

    // 2 methods for future use, maybe later put not here, but in the solver class.
    //public Cube Clone()
    //{
    //    Cube newCube = new Cube();
    //    for (int i = 0; i < 54; i++)
    //    {
    //        newCube.State[i] = this.State[i];
    //    }
    //    return newCube;
    //}

    //public string ToFaceletString() 
    //{ 
    //}
}

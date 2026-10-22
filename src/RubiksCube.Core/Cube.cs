namespace RubiksCube.Core;

public enum Color
{
    White,
    Yellow,
    Red,
    Orange,
    Blue,
    Green
} 

public enum Face
{
    Up,     //Indeksi 0-8
    Right,  //Indeksi 9-17
    Front,  //Indeksi 18-26
    Down,   //Indeksi 27-35
    Left,   //Indeksi 36-44
    Back    //Indeksi 45-53
}

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
    
}

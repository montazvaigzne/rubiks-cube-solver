using RubiksCube.Core;
using RubiksCube.Solvers;
using Xunit;

namespace RubiksCube.Tests.Solvers;

public class SolverTests
{
    // TEST: FindEdge, checking if it correctly identifies the edge pieces based on their colors
    [Theory]
    [InlineData(Color.White, Color.Green, 7, 19)]   // UF 
    [InlineData(Color.White, Color.Red, 5, 10)]     // UR 
    [InlineData(Color.Red, Color.Green, 23, 12)]    // FR 
    [InlineData(Color.Yellow, Color.Blue, 34, 52)]  // DB 
    public void FindEdge_OnSolvedCube_ShouldFindEdge(Color color1, Color color2, int expectedIndex1, int expectedIndex2)
    {
        var cube = new Cube();

        var result = SolverBeginner.FindEdge(cube, color1, color2);

        Assert.Equal(expectedIndex1, result.Item1);
        Assert.Equal(expectedIndex2, result.Item2);
    }

    // TEST: FindEdge, checking if it still finds the edge when the colors are reversed
    [Fact]
    public void FindEdge_WhenColorsAreReversed_ShouldStillFindEdge()
    {
        var cube = new Cube();

        // logically, the edge (7, 19) is the same as (19, 7), just with colors reversed
        var result = SolverBeginner.FindEdge(cube, Color.Green, Color.White);

        Assert.Equal(7, result.Item1);
        Assert.Equal(19, result.Item2);
    }

    // TEST: FindEdge, checking if it can find the edge after a move has been made
    [Fact]
    public void FindEdge_AfterMoveF_ShouldFindMovedEdge()
    {
        var cube = new Cube();

        // After an F move, the white-green edge (7, 19) moves to (23, 12)
        cube.MoveF(true);

        var result = SolverBeginner.FindEdge(cube, Color.White, Color.Green);

        Assert.Equal(23, result.Item1);
        Assert.Equal(12, result.Item2);
    }

    // TEST: FindEdge, checking if it can find the edge after a U move has been made
    [Fact]
    public void FindEdge_AfterMoveU_ShouldFindMovedEdge()
    {
        var cube = new Cube();

        // After a U move, the white-green edge (7, 19) moves to (3, 37)
        cube.MoveU(true);

        var result = SolverBeginner.FindEdge(cube, Color.White, Color.Green);

        Assert.Equal(3, result.Item1);
        Assert.Equal(37, result.Item2);
    }

    // TEST: FindEdge, checking if it returns (-1, -1) when the edge does not exist
    [Fact]
    public void FindEdge_WhenEdgeDoesNotExist_ShouldReturnMinusOne()
    {
        var cube = new Cube();

        // The edge with colors White and Yellow does not exist on a cube
        var result = SolverBeginner.FindEdge(cube, Color.White, Color.Yellow);

        Assert.Equal(-1, result.Item1);
        Assert.Equal(-1, result.Item2);
    }

    // TEST: SolveDaisy should create the Daisy pattern from a scrambled cube
    [Fact]
    public void SolveDaisy_OnScrambledCube_ShouldCreateDaisy()
    {
        var cube = new Cube();

        // Scramble the cube with a few moves
        MoveInterpreter.ExecuteMove(cube, "R U F");

        SolverBeginner.SolveDaisy(cube);

        // Check that the four white edge pieces are on the Down face (indices 28, 30, 32, 34)
        Assert.True(cube.GetColor(28) == Color.White, "Daisy error: Index 28 is not white.");
        Assert.True(cube.GetColor(30) == Color.White, "Daisy error: Index 30 is not white.");
        Assert.True(cube.GetColor(32) == Color.White, "Daisy error: Index 32 is not white.");
        Assert.True(cube.GetColor(34) == Color.White, "Daisy error: Index 34 is not white.");
    }

    // TEST GROUP: SolveFirstLayer (full first layer including Cross and Corners) on various scrambles
    [Theory]
    [InlineData("")]                                     // Already solved state
    [InlineData("R")]                                    // Single move
    [InlineData("R U")]                                  // Two moves
    [InlineData("R U F L D B")]                          // All faces once
    [InlineData("F R U R' U' F'")]                       // I-Shape (OLL)
    [InlineData("R U2 R' U' R U' R'")]                   // Antisune (OLL)
    [InlineData("R2 D2 F2 U2 L2 B2")]                    // Double moves
    public void SolveFirstLayer_OnVariousScrambles_ShouldSolveFirstLayer(string scramble)
    {
        var cube = new Cube();

        // Scramble the cube based on the provided sequence
        MoveInterpreter.ExecuteMove(cube, scramble);

        SolverBeginner.SolveWhiteCross(cube);

       // 1. Check if all 4 white edges are in their correct positions on the Up face
        Assert.True(cube.GetColor(7) == Color.White && cube.GetColor(19) == Color.Green,
            "Cross error: Up-Front edge is not placed correctly.");
        Assert.True(cube.GetColor(5) == Color.White && cube.GetColor(10) == Color.Red,
            "Cross error: Up-Right edge is not placed correctly.");
        Assert.True(cube.GetColor(1) == Color.White && cube.GetColor(46) == Color.Blue,
            "Cross error: Up-Back edge is not placed correctly.");
        Assert.True(cube.GetColor(3) == Color.White && cube.GetColor(37) == Color.Orange,
            "Cross error: Up-Left edge is not placed correctly.");

        SolverBeginner.SolveFirstLayer(cube);

        // 2. FINAL CHECK: Ensure the entire first layer (Up face + side T-shapes) is solved
         // Check if the entire Up face is white
        for (int i = 0; i < 9; i++)
        {
            Assert.True(cube.GetColor(i) == Color.White, $"First layer error: Index {i} on Up face is not white.");
        }

        // Check the middle layer colors of the white edges (RE-CHECKING THE CROSS!)
        Assert.True(cube.GetColor(19) == Color.Green, "First layer broke the Up-Front edge side color!");
        Assert.True(cube.GetColor(10) == Color.Red, "First layer broke the Up-Right edge side color!");
        Assert.True(cube.GetColor(46) == Color.Blue, "First layer broke the Up-Back edge side color!");
        Assert.True(cube.GetColor(37) == Color.Orange, "First layer broke the Up-Left edge side color!");

        // Check if all 4 side colors of the corners are solved (T-shapes)
        Assert.True(cube.GetColor(20) == Color.Green && cube.GetColor(9) == Color.Red, "URF corner error.");
        Assert.True(cube.GetColor(18) == Color.Green && cube.GetColor(38) == Color.Orange, "UFL corner error.");
        Assert.True(cube.GetColor(47) == Color.Blue && cube.GetColor(36) == Color.Orange, "ULB corner error.");
        Assert.True(cube.GetColor(45) == Color.Blue && cube.GetColor(11) == Color.Red, "UBR corner error.");
    }

    // TEST GROUP: SolveMiddleLayer (the 4 edges without white/yellow) on various scrambles
    [Theory]
    [InlineData("")]                                     // Already solved state
    [InlineData("R")]                                    // Single move
    [InlineData("R U")]                                  // Two moves
    [InlineData("R U F L D B")]                          // All faces once
    [InlineData("F R U R' U' F'")]                       // I-Shape (OLL)
    [InlineData("R U2 R' U' R U' R'")]                   // Antisune (OLL)
    [InlineData("R2 D2 F2 U2 L2 B2")]                    // Double moves
    public void SolveMiddleLayer_OnVariousScrambles_ShouldSolveMiddleLayer(string scramble)
    {
        var cube = new Cube();

        // Scramble the cube based on the provided sequence
        MoveInterpreter.ExecuteMove(cube, scramble);

        SolverBeginner.SolveWhiteCross(cube);
        SolverBeginner.SolveFirstLayer(cube);
        SolverBeginner.SolveMiddleLayer(cube);

        // 1. Check that the first layer was not broken by the middle layer algorithms
        for (int i = 0; i < 9; i++)
        {
            Assert.True(cube.GetColor(i) == Color.White, $"First layer error: Index {i} on Up face is not white.");
        }
        Assert.True(cube.GetColor(19) == Color.Green && cube.GetColor(10) == Color.Red &&
                    cube.GetColor(46) == Color.Blue && cube.GetColor(37) == Color.Orange,
            "Middle layer error: The white cross side colors were broken.");
        Assert.True(cube.GetColor(20) == Color.Green && cube.GetColor(9) == Color.Red &&
                    cube.GetColor(18) == Color.Green && cube.GetColor(38) == Color.Orange &&
                    cube.GetColor(47) == Color.Blue && cube.GetColor(36) == Color.Orange &&
                    cube.GetColor(45) == Color.Blue && cube.GetColor(11) == Color.Red,
            "Middle layer error: The first layer corners were broken.");

        // 2. Check that all 4 middle-layer edges are in their correct slots and orientation
        Assert.True(cube.GetColor(23) == Color.Green && cube.GetColor(12) == Color.Red, "Middle layer error: FR edge is not solved.");
        Assert.True(cube.GetColor(21) == Color.Green && cube.GetColor(41) == Color.Orange, "Middle layer error: FL edge is not solved.");
        Assert.True(cube.GetColor(14) == Color.Red && cube.GetColor(48) == Color.Blue, "Middle layer error: BR edge is not solved.");
        Assert.True(cube.GetColor(39) == Color.Orange && cube.GetColor(50) == Color.Blue, "Middle layer error: BL edge is not solved.");
    }

    // TEST GROUP: SolveOLL (orientation of the last layer) on various scrambles
    [Theory]
    [InlineData("")]                                     // Already solved state
    [InlineData("R")]                                    // Single move
    [InlineData("R U")]                                  // Two moves
    [InlineData("R U F L D B")]                          // All faces once
    [InlineData("F R U R' U' F'")]                       // I-Shape (OLL)
    [InlineData("R U2 R' U' R U' R'")]                   // Antisune (OLL)
    [InlineData("R2 D2 F2 U2 L2 B2")]                    // Double moves
    public void SolveOLL_OnVariousScrambles_ShouldOrientLastLayer(string scramble)
    {
        var cube = new Cube();

        // Scramble the cube based on the provided sequence
        MoveInterpreter.ExecuteMove(cube, scramble);

        SolverBeginner.SolveWhiteCross(cube);
        SolverBeginner.SolveFirstLayer(cube);
        SolverBeginner.SolveMiddleLayer(cube);
        SolverBeginner.SolveOLL(cube);

        // 1. Check that the first two layers were not broken by the OLL algorithms
        for (int i = 0; i < 9; i++)
        {
            Assert.True(cube.GetColor(i) == Color.White, $"OLL error: Index {i} on Up face is not white.");
        }
        Assert.True(cube.GetColor(19) == Color.Green && cube.GetColor(10) == Color.Red &&
                    cube.GetColor(46) == Color.Blue && cube.GetColor(37) == Color.Orange,
            "OLL error: The white cross side colors were broken.");
        Assert.True(cube.GetColor(20) == Color.Green && cube.GetColor(9) == Color.Red &&
                    cube.GetColor(18) == Color.Green && cube.GetColor(38) == Color.Orange &&
                    cube.GetColor(47) == Color.Blue && cube.GetColor(36) == Color.Orange &&
                    cube.GetColor(45) == Color.Blue && cube.GetColor(11) == Color.Red,
            "OLL error: The first layer corners were broken.");
        Assert.True(cube.GetColor(23) == Color.Green && cube.GetColor(12) == Color.Red &&
                    cube.GetColor(21) == Color.Green && cube.GetColor(41) == Color.Orange &&
                    cube.GetColor(14) == Color.Red && cube.GetColor(48) == Color.Blue &&
                    cube.GetColor(39) == Color.Orange && cube.GetColor(50) == Color.Blue,
            "OLL error: The middle layer was broken.");

        // 2. Check that the whole Down face is yellow
        for (int i = 27; i < 36; i++)
        {
            Assert.True(cube.GetColor(i) == Color.Yellow, $"OLL error: Index {i} on Down face is not yellow.");
        }
    }

    // TEST GROUP: SolvePLL (permutation of the last layer) on various scrambles
    [Theory]
    [InlineData("")]                                     // Already solved state
    [InlineData("R")]                                    // Single move
    [InlineData("R U")]                                  // Two moves
    [InlineData("R U F L D B")]                          // All faces once
    [InlineData("F R U R' U' F'")]                       // I-Shape (OLL)
    [InlineData("R U2 R' U' R U' R'")]                   // Antisune (OLL)
    [InlineData("R2 D2 F2 U2 L2 B2")]                    // Double moves
    public void SolvePLL_OnVariousScrambles_ShouldSolveWholeCube(string scramble)
    {
        var cube = new Cube();

        // Scramble the cube based on the provided sequence
        MoveInterpreter.ExecuteMove(cube, scramble);

        SolverBeginner.SolveWhiteCross(cube);
        SolverBeginner.SolveFirstLayer(cube);
        SolverBeginner.SolveMiddleLayer(cube);
        SolverBeginner.SolveOLL(cube);
        SolverBeginner.SolvePLL(cube);

        // Every sticker must be back on its original face
        Assert.True(cube.IsSolved(), "PLL error: The cube is not solved, some stickers are still on the wrong face.");
    }
}
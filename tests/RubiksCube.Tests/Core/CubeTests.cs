using RubiksCube.Core;
using Xunit;

namespace RubiksCube.Tests.Core;

public class CubeTests
{

    //TEST: If doing the same move 4 times returns the cube to its solved state
    [Theory]
    [InlineData("U")]
    [InlineData("D")]
    [InlineData("R")]
    [InlineData("L")]
    [InlineData("F")]
    [InlineData("B")]
    public void MoveFourTimes_ShouldReturnToSolvedState(string move)
    {
        var cube = new Cube();

        MoveInterpreter.ExecuteMove(cube, $"{move} {move} {move} {move}"); // Rotate clockwise 4 times

        Assert.True(cube.IsSolved());
    }

    // TEST: A double move (e.g. R2) should be the same as doing the move twice (R R)
    [Theory]
    [InlineData("U")]
    [InlineData("D")]
    [InlineData("R")]
    [InlineData("L")]
    [InlineData("F")]
    [InlineData("B")]
    public void DoubleMove_ShouldEqualTwoSingleMoves(string face)
    {
        var doubleMove = new Cube();
        MoveInterpreter.ExecuteMove(doubleMove, face + "2");

        var twoMoves = new Cube();
        MoveInterpreter.ExecuteMove(twoMoves, $"{face} {face}");

        for (int i = 0; i < 54; i++)
        {
            Assert.Equal(twoMoves.GetColor(i), doubleMove.GetColor(i));
        }
    }

    // TEST: If doing a move and then its inverse returns the cube to its solved state
    [Theory]
    [InlineData("U")]
    [InlineData("D")]
    [InlineData("R")]
    [InlineData("L")]
    [InlineData("F")]
    [InlineData("B")]
    public void MoveTrueThenFalse_ShouldReturnToSolvedState(string move)
    {
        var cube = new Cube();

        MoveInterpreter.ExecuteMove(cube, $"{move} {move}'"); // Example: "R R'"

        Assert.True(cube.IsSolved());
    }

    // TEST: If doing a move and then its inverse returns the cube to its solved state by checking the colors of the corners
    [Theory]
    [InlineData("U")]
    [InlineData("D")]
    [InlineData("R")]
    [InlineData("L")]
    [InlineData("F")]
    [InlineData("B")]
    public void SingleMove_ShouldPreserveAllCorners(string move)
    {
        var initialCube = new Cube();

        var cornerPositions = CubeMap.CornerPositions;

        // Read the original colors of the corners from the initial cube
        var originalColorSets = cornerPositions.Select(pos => new[]
        {
            initialCube.GetColor(pos[0]),
            initialCube.GetColor(pos[1]),
            initialCube.GetColor(pos[2])
        }).ToList();

     
        var movedCube = new Cube();
        MoveInterpreter.ExecuteMove(movedCube, move);


        // Go through each corner position and check if the colors match any of the original corner color sets
        foreach (var pos in cornerPositions)
        {
            var currentColors = new[]
            {
                movedCube.GetColor(pos[0]),
                movedCube.GetColor(pos[1]),
                movedCube.GetColor(pos[2])
            };

            bool isMatch = originalColorSets.Any(originalSet =>
                originalSet.OrderBy(c => c).SequenceEqual(currentColors.OrderBy(c => c))
            );

            Assert.True(isMatch, $"Error! Corner with indices {pos[0]}, {pos[1]}, {pos[2]} after move '{move}' has invalid colors: {currentColors[0]}, {currentColors[1]}, {currentColors[2]}");
        }
    }

    //TEST: If doing a move on a scrambled cube preserves the integrity of the corners
    [Theory]
    [InlineData("U")]
    [InlineData("D")]
    [InlineData("R")]
    [InlineData("L")]
    [InlineData("F")]
    [InlineData("B")]
    public void Move_OnScrambledCube_ShouldPreserveCornerIntegrity(string move)
    {
        var cube = new Cube();

        // Scramble the cube with a series of moves so the colors are not in their original positions
        MoveInterpreter.ExecuteMove(cube, "R U F");

        // Do the move we want to test
        MoveInterpreter.ExecuteMove(cube, move);

        // Check that the corners still have valid color combinations
        var cornerPositions = CubeMap.CornerPositions;

        var originalCube = new Cube();
        var validColorSets = cornerPositions.Select(pos => new[]
        {
            originalCube.GetColor(pos[0]),
            originalCube.GetColor(pos[1]),
            originalCube.GetColor(pos[2])
        }).ToList();

        foreach (var pos in cornerPositions)
        {
            var currentColors = new[]
            {
                cube.GetColor(pos[0]),
                cube.GetColor(pos[1]),
                cube.GetColor(pos[2])
            };

            bool isMatch = validColorSets.Any(validSet =>
                validSet.OrderBy(c => c).SequenceEqual(currentColors.OrderBy(c => c))
            );

            Assert.True(isMatch, $"Corner at indices {pos[0]}, {pos[1]}, {pos[2]} is broken after move '{move}'! Current colors: {string.Join(", ", currentColors)}");
        }
    }
    // TEST: If doing a move on a scrambled cube preserves the integrity of all of the colors on the cube
    [Fact]
    public void MoveSequence_F_R_U_B_L_D_ShouldMatchExpectedState()
    {
        var cube = new Cube();
        MoveInterpreter.ExecuteMove(cube, "F R U B L D");

        // Define the expected colors for each position on the cube after the sequence of moves
        var expectedColors = new Color[]
        {
            // Up (0-8)
            Color.Yellow, Color.Red, Color.Red,
            Color.Orange, Color.White, Color.White,
            Color.Orange, Color.Green, Color.Green,
            
            // Right (9-17)
            Color.Orange, Color.Blue, Color.Blue,
            Color.Red, Color.Red, Color.Yellow,
            Color.Green, Color.Green, Color.Yellow,
            
            // Front (18-26)
            Color.Blue, Color.White, Color.White,
            Color.Orange, Color.Green, Color.Yellow,
            Color.Yellow, Color.Yellow, Color.Red,
            
            // Down (27-35)
            Color.Green, Color.Green, Color.White,
            Color.Orange, Color.Yellow, Color.Red,
            Color.Orange, Color.Blue, Color.Blue,
            
            // Left (36-44)
            Color.Orange, Color.White, Color.White,
            Color.Orange, Color.Orange, Color.Green,
            Color.Blue, Color.Blue, Color.Red,
            
            // Back (45-53)
            Color.White, Color.White, Color.Green,
            Color.Blue, Color.Blue, Color.Yellow,
            Color.Red, Color.Red, Color.Yellow
        };

        for (int i = 0; i < 54; i++)
        {
            Assert.Equal(expectedColors[i], cube.GetColor(i));
        }
    }
}

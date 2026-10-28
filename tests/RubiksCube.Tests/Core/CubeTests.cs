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

        for (int i = 0; i < 4; i++)
        {
            MoveInterpreter.ExecuteMove(cube, move); // Always rotate clockwise for this test
        }

        Assert.True(cube.IsSolved());
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

        MoveInterpreter.ExecuteMove(cube, move);      // Example: "R" (clockwise)
        MoveInterpreter.ExecuteMove(cube, move + "'"); // Example: "R'" (counter-clockwise)

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
        MoveInterpreter.ExecuteMove(cube, "R");
        MoveInterpreter.ExecuteMove(cube, "U");
        MoveInterpreter.ExecuteMove(cube, "F");

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
}

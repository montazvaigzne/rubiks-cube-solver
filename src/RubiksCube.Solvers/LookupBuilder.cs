using RubiksCube.Core;

namespace RubiksCube.Solvers;

// Builds a "signature -> move sequence" lookup table.
// Applying the INVERSE of a move sequence to a solved cube creates exactly the state
// that sequence solves, so every sequence can be recorded without any searching.
public static class LookupBuilder
{
    public static Dictionary<string, string> Build(IEnumerable<string> words, Func<Cube, string> signature)
    {
        Dictionary<string, string> lookup = [];

        foreach (var word in words)
        {
            var cube = new Cube();
            MoveInterpreter.ExecuteMove(cube, MoveInterpreter.Invert(word));

            // TryAdd keeps the first word if two words reach the same signature
            // (this happens for symmetric cases like Dot and H)
            lookup.TryAdd(signature(cube), word);
        }

        return lookup;
    }
}

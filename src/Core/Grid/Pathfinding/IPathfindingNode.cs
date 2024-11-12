using Godot;
using System.Collections.Generic;

namespace HexTactics.Core
{
    // Interface for any node that can be pathfound through
    public interface IPathfindingNode
    {
        int Index { get; }
        Vector3 Position { get; }
        bool IsTraversable { get; }
        IEnumerable<IPathfindingNode> Neighbors { get; }
    }
}
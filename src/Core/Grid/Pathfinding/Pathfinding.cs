using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HexTactics.Core
{
    public class Pathfinder
    {
        private readonly AStar3D _astar;
        private readonly List<HexCell> _nodes;
        private bool _initialized;
        public event Action OnPathfindingUpdated;

        public Pathfinder()
        {
            _astar = new AStar3D();
            _nodes = new List<HexCell>();
            _initialized = false;
        }

        public void Initialize(IEnumerable<HexCell> nodes)
        {
            _nodes.Clear();
            _nodes.AddRange(nodes);
            SetupPathfinding();
            _initialized = true;
            OnPathfindingUpdated?.Invoke();
        }

        public void SetupPathfinding()
        {
            _astar.Clear();
            AddPoints();
            ConnectPoints();
            OnPathfindingUpdated?.Invoke();
        }

        private void AddPoints()
        {
            _astar.Clear(); // Make sure we start fresh
            foreach (var node in _nodes)
            {
                if (node.IsTraversable)
                {
                    _astar.AddPoint(node.Index, node.Position);
                }
            }
        }

        private void ConnectPoints()
        {
            foreach (var node in _nodes)
            {
                // Skip if this node wasn't added to the graph
                if (!_astar.HasPoint(node.Index))
                    continue;

                foreach (var neighbor in node.Neighbors)
                {
                    // Only connect if both points exist in the graph
                    if (_astar.HasPoint(neighbor.Index) &&
                        !_astar.ArePointsConnected(node.Index, neighbor.Index))
                    {
                        _astar.ConnectPoints(node.Index, neighbor.Index);
                    }
                }
            }
        }

        public List<HexCell> FindPath(int fromIndex, int toIndex)
        {
            if (!IsValidPath(fromIndex, toIndex))
                return new List<HexCell>();

            var pathPoints = _astar.GetPointPath(fromIndex, toIndex);
            return pathPoints.Select(point =>
                FindNodeAtPosition(point)
            ).Where(node =>
                node != null
            ).ToList();
        }

        public List<HexCell> GetReachableNodes(HexCell start, int range)
        {
            if (!_initialized || !start.IsTraversable)
                return new List<HexCell>();

            var reachable = new List<HexCell>();
            var visited = new HashSet<int>();
            var queue = new Queue<(HexCell node, int distance)>();

            queue.Enqueue((start, 0));
            visited.Add(start.Index);

            while (queue.Count > 0)
            {
                var (current, distance) = queue.Dequeue();
                reachable.Add(current);

                if (distance >= range)
                    continue;

                foreach (var neighbor in current.Neighbors)
                {
                    if (!visited.Contains(neighbor.Index) && neighbor.IsTraversable)
                    {
                        visited.Add(neighbor.Index);
                        queue.Enqueue((neighbor, distance + 1));
                    }
                }
            }

            return reachable;
        }

        private bool IsValidPath(int fromIndex, int toIndex)
        {
            return _initialized &&
                   _astar.HasPoint(fromIndex) &&
                   _astar.HasPoint(toIndex);
        }

        private HexCell FindNodeAtPosition(Vector3 position)
        {
            return _nodes.MinBy(n => position.DistanceTo(n.Position));
        }

        public void Clear()
        {
            _nodes.Clear();
            _astar.Clear();
            _initialized = false;
        }
    }
}
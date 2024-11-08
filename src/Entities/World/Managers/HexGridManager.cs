using Godot;
using System.Collections.Generic;
using System.Linq;
using HexTactics.Core.HexGrid;
using HexTactics.Core.Debug;
using HexTactics.Core.Pathfinding;

namespace HexTactics.Entities.World.Managers
{
    public partial class HexGridManager : Node3D
    {
        [Export] public int MapSize { get; set; } = 5;
        [Export] public float HexSize { get; set; } = 1.1f;
        [Export] public PackedScene HexScene { get; set; }

        private bool _showDebugVisualizer;
        [Export]
        public bool ShowDebugVisualizer
        {
            get => _showDebugVisualizer;
            set
            {
                _showDebugVisualizer = value;
                UpdateDebugVisualizer();
                if (_showDebugVisualizer && _debugVisualizer != null)
                {
                    VisualizePathfinding();
                }
            }
        }

        private List<HexCell> _cells = new();
        private Node3D _gridContainer;
        private AStarDebugVisualizer _debugVisualizer;
        private Pathfinder _pathfinder;

        private const string DEBUG_VISUALIZER_PATH = "res://src/Core/Debug/AStarDebugVisualizer.tscn";

        public override void _Ready()
        {
            _pathfinder = new Pathfinder();
            SetupGridContainer();
            UpdateDebugVisualizer();
        }

        private void SetupGridContainer()
        {
            _gridContainer = new Node3D { Name = "HexGridContainer" };
            AddChild(_gridContainer);
        }

        public void Initialize()
        {
            Clear();
            GenerateGrid();
            GenerateNeighbors();
            InitializePathfinding();
        }

        private void GenerateGrid()
        {
            var coordinates = HexGrid.GenerateHexCoordinates(MapSize);

            for (int i = 0; i < coordinates.Count; i++)
            {
                var coord = coordinates[i];
                var location = HexGrid.HexToWorld(coord, HexSize);
                var hex = CreateHexCell(i, coord, location);
                _cells.Add(hex);
            }
        }

        private HexCell CreateHexCell(int index, Vector3I coord, Vector3 location)
        {
            var hex = HexScene.Instantiate<HexCell>();
            hex.Name = $"Hex_{index}";
            hex.Index = index;
            hex.Coordinates = coord;

            _gridContainer.AddChild(hex);
            hex.GlobalPosition = location;

            return hex;
        }

        private void GenerateNeighbors()
        {
            foreach (var cell in _cells)
            {
                var neighborCoords = HexGrid.GetNeighbors(cell.Coordinates);
                var neighbors = new List<HexCell>();

                foreach (var coord in neighborCoords)
                {
                    var neighbor = FindHexByCoord(coord);
                    if (neighbor != null)
                    {
                        neighbors.Add(neighbor);
                    }
                }

                cell.Neighbors = neighbors;
            }
        }

        private void InitializePathfinding()
        {
            _pathfinder.Initialize(_cells);

            if (_showDebugVisualizer && _debugVisualizer != null)
            {
                VisualizePathfinding();
            }
        }

        private void UpdateDebugVisualizer()
        {
            if (_showDebugVisualizer && _debugVisualizer == null)
            {
                SetupDebugVisualizer();
            }
            else if (!_showDebugVisualizer && _debugVisualizer != null)
            {
                _debugVisualizer.QueueFree();
                _debugVisualizer = null;
            }
        }

        private void SetupDebugVisualizer()
        {
            try
            {
                var visualizerScene = ResourceLoader.Load<PackedScene>(DEBUG_VISUALIZER_PATH);
                if (visualizerScene != null)
                {
                    _debugVisualizer = visualizerScene.Instantiate<AStarDebugVisualizer>();
                    AddChild(_debugVisualizer);
                }
                else
                {
                    GD.PrintErr($"Failed to load AStarDebugVisualizer scene from {DEBUG_VISUALIZER_PATH}");
                    _showDebugVisualizer = false;
                }
            }
            catch (System.Exception e)
            {
                GD.PrintErr($"Error loading debug visualizer: {e.Message}");
                _showDebugVisualizer = false;
            }
        }

        private void VisualizePathfinding()
        {
            if (_debugVisualizer != null && _pathfinder != null)
            {
                var astarField = typeof(Pathfinder).GetField("_astar",
                    System.Reflection.BindingFlags.NonPublic |
                    System.Reflection.BindingFlags.Instance);

                if (astarField != null)
                {
                    var astar = astarField.GetValue(_pathfinder) as AStar3D;
                    if (astar != null)
                    {
                        _debugVisualizer.VisualizeAstar(astar);
                    }
                }
            }
        }
        private HexCell FindHexByCoord(Vector3I coord)
        {
            return _cells.FirstOrDefault(hex => hex.Coordinates == coord);
        }

        public void Clear()
        {
            foreach (var hex in _cells)
            {
                hex.QueueFree();
            }
            _cells.Clear();
            _pathfinder?.Clear();
            _debugVisualizer?.Clear();
        }

        public override void _ExitTree()
        {
            base._ExitTree();
            _debugVisualizer?.QueueFree();
        }

        // Helper methods for pathfinding
        public List<HexCell> FindPath(HexCell from, HexCell to)
        {
            return _pathfinder.FindPath(from.Index, to.Index)
                .Select(node => node as HexCell)
                .Where(cell => cell != null)
                .ToList();
        }

        public List<HexCell> GetReachableNodes(HexCell start, int range)
        {
            return _pathfinder.GetReachableNodes(start, range)
                .Select(node => node as HexCell)
                .Where(cell => cell != null)
                .ToList();
        }
    }
}
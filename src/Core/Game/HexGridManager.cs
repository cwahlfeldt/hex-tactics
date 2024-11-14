using Godot;
using System.Collections.Generic;
using System.Linq;
using HexTactics.Utils.Debug;

namespace HexTactics.Core
{
    public class HexGridManager
    {
        private float HexSize { get; set; } = 1.1f;
        public readonly PackedScene HexScene = ResourceLoader.Load<PackedScene>("res://src/Core/Grid/HexCell/HexCell.tscn");
        private readonly List<HexCell> _cells = new();
        private Node3D _gridContainer = new();
        private AStarDebugVisualizer _debugVisualizer = new();
        private readonly Pathfinder _pathfinder = new();
        private readonly Dictionary<int, Label3D> _indexLabels = new();
        private const string DEBUG_VISUALIZER_PATH = "res://src/Utils/Debug/AStarDebugVisualizer.tscn";

        public HexGridManager()
        {
            Initialize();
        }

        private void Initialize()
        {
            SetupGridContainer();
            GenerateGrid();
            DisableRandomCells();
            GenerateNeighbors();
            InitializePathfinding();
            UpdateDebugVisualizer();
            UpdateIndexLabels();
        }

        private void SetupGridContainer()
        {
            _gridContainer = new Node3D { Name = "HexGrid" };
            GameManager.Instance.AddChild(_gridContainer);
        }

        private void GenerateGrid()
        {
            var coordinates = HexGrid.GenerateHexCoordinates(GameManager.Instance.MapSize);

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
            hex.Name = $"HexCell_{index}";
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
                    if (neighbor != null && neighbor.IsTraversable)
                    {
                        neighbors.Add(neighbor);
                    }
                }

                cell.Neighbors = neighbors;
            }
        }

        public void InitializePathfinding()
        {
            _pathfinder.Initialize(_cells);

            if (GameManager.Instance.ShowPathDebug && _debugVisualizer != null)
            {
                _pathfinder.OnPathfindingUpdated += UpdateDebugVisualizer;
                SetupDebugVisualizer();
            }
        }

        public List<HexCell> FindPath(HexCell from, HexCell to, int moveRange = 1)
        {
            var path = _pathfinder.FindPath(from.Index, to.Index).Take(moveRange + 1).ToList();

            return path;
        }

        public void DisableRandomCells()
        {
            if (GameManager.Instance.DisabledCells <= 0 || _cells.Count <= GameManager.Instance.DisabledCells) return;

            // Get random cells without units
            var cellsToDisable = _cells
                .Where(cell => cell.Unit == null && cell.IsTraversable)
                .Where(cell => cell.Index != GameManager.Instance.PlayerStartHexIndex) // Only select currently traversable cells without units
                .OrderBy(x => GD.RandRange(0, 1.0f))
                .Take(GameManager.Instance.DisabledCells)
                .ToList();

            foreach (var cell in cellsToDisable)
            {
                DisableCell(cell);
            }
        }

        public void DisableCell(HexCell cell)
        {
            if (cell == null || !_cells.Contains(cell)) return;
            cell.IsTraversable = false;
        }

        private void UpdateIndexLabels()
        {
            foreach (var label in _indexLabels.Values)
            {
                label.QueueFree();
            }
            _indexLabels.Clear();

            if (!GameManager.Instance.ShowHexDebug) return;

            foreach (var cell in _cells)
            {
                if (!cell.IsTraversable) continue;
                var label = new Label3D
                {
                    Text = cell.Index.ToString(),
                    Name = $"IndexLabel_{cell.Index}",
                    Position = new Vector3(0, 0.45f, 0.2f),
                    FontSize = 90,
                    Modulate = Colors.Black,
                    NoDepthTest = true,
                };

                label.Rotate(new Vector3(1, 0, 0), 30);

                cell.AddChild(label);
                _indexLabels[cell.Index] = label;
            }
        }

        public void EnableCell(HexCell cell)
        {
            if (cell == null || !_cells.Contains(cell)) return;
            cell.IsTraversable = true;
        }

        public void ResetAllCells()
        {
            foreach (var cell in _cells)
            {
                EnableCell(cell);
            }
            InitializePathfinding();
        }

    private void UpdateDebugVisualizer()
    {
        if (GameManager.Instance.ShowPathDebug && _debugVisualizer != null)
        {
            VisualizePathfinding();
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
                    GameManager.Instance.AddChild(_debugVisualizer);
                }
                else
                {
                    GD.PrintErr($"Failed to load AStarDebugVisualizer scene from {DEBUG_VISUALIZER_PATH}");
                    GameManager.Instance.ShowPathDebug = false;
                }
            }
            catch (System.Exception e)
            {
                GD.PrintErr($"Error loading debug visualizer: {e.Message}");
                GameManager.Instance.ShowPathDebug = false;
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
                    if (astarField.GetValue(_pathfinder) is AStar3D astar)
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

        // Update your Clear method to remove the event listener
        public void Clear()
        {
            foreach (var hex in _cells)
            {
                hex.QueueFree();
            }
            _cells.Clear();
            if (_pathfinder != null)
            {
                _pathfinder.OnPathfindingUpdated -= UpdateDebugVisualizer;
                _pathfinder.Clear();
            }
            _debugVisualizer?.Clear();
        }

        public void UpdatePathfinding() => _pathfinder.SetupPathfinding();
        public Pathfinder GetPathfinder() => _pathfinder;
        public List<HexCell> GetGrid() => _cells;
        public HexCell GetCellByIndex(int index) => _cells[index];
        public List<HexCell> GetReachableNodes(HexCell start, int range) => _pathfinder.GetReachableNodes(start, range);
    }
}
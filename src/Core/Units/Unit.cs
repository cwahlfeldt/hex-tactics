// using System.Collections.Generic;
// using Godot;

// namespace HexTactics.Core
// {
//     public partial class Unit : Node3D
//     {
//         public UnitType UnitType { get; set; } = UnitType.Player;
//         public Dictionary<RangeType, int> AttackRangeTypes { get; } = new()
//         {
//             { RangeType.Melee, 1 },
//             { RangeType.Ranged, 4 },
//             { RangeType.Magic, 8 },
//             { RangeType.Bomb, 3 }
//         };
//         public List<HexCell> MoveRangeHexes = new();
//         public List<HexCell> AttackRangeHexes = new();

//         [Export] public int MoveRange { get; set; } = 1;
//         [Export] public RangeType AttackRange { get; set; } = RangeType.Melee;
//         [Export] public int AttackPower { get; set; } = 1;
//         [Export] public int MaxHealth { get; set; } = 3;
//         public HexCell CurrentHex { get; set; }
//         public int CurrentHealth { get; private set; }

//         public override void _Ready()
//         {
//             AddToGroup("Unit");
//             CurrentHealth = MaxHealth;
//         }

//         public virtual void TakeDamage(int amount)
//         {
//             CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
//             if (CurrentHealth <= 0)
//             {
//                 QueueFree();
//             }
//         }


//     }
// }
using System.Collections.Generic;
using Godot;
using System.Linq;

namespace HexTactics.Core
{
    public partial class Unit : Node3D
    {
        public UnitType UnitType { get; set; } = UnitType.Player;

        public enum RangeType { Melee, Ranged, Magic, Bomb }

        public Dictionary<RangeType, int> AttackRangeTypes { get; } = new()
        {
            { RangeType.Melee, 1 },
            { RangeType.Ranged, 4 },
            { RangeType.Magic, 8 },
            { RangeType.Bomb, 3 }
        };

        [Export] public int MoveRange { get; set; } = 1;
        [Export] public RangeType AttackRange { get; set; } = RangeType.Melee;
        [Export] public int AttackPower { get; set; } = 1;
        [Export] public int MaxHealth { get; set; } = 3;

        private HexCell _currentHex;
        public HexCell CurrentHex
        {
            get => _currentHex;
            set
            {
                _currentHex = value;
                UpdateRanges();
            }
        }

        public int CurrentHealth { get; private set; }

        // Change to HashSets for more efficient operations
        private HashSet<HexCell> _attackRangeHexes = new();
        private HashSet<HexCell> _moveRangeHexes = new();

        // Public read-only access to ranges
        public IReadOnlySet<HexCell> AttackRangeHexes => _attackRangeHexes;
        public IReadOnlySet<HexCell> MoveRangeHexes => _moveRangeHexes;

        public override void _Ready()
        {
            AddToGroup("Unit");
            CurrentHealth = MaxHealth;
        }

        public void UpdateRanges()
        {
            if (CurrentHex == null) return;

            UpdateMoveRange();
            UpdateAttackRange();
        }

        private void UpdateMoveRange()
        {
            _moveRangeHexes.Clear();

            if (GameManager.Instance?.HexGridManager != null)
            {
                var reachableNodes = GameManager.Instance.HexGridManager
                    .GetReachableNodes(CurrentHex, MoveRange);

                foreach (var node in reachableNodes)
                {
                    _moveRangeHexes.Add(node);
                }
            }
        }

        private void UpdateAttackRange()
        {
            _attackRangeHexes.Clear();

            if (CurrentHex == null || !AttackRangeTypes.TryGetValue(AttackRange, out int range))
                return;

            var grid = GameManager.Instance?.HexGridManager?.GetGrid();
            if (grid == null) return;

            // Get all cells within attack range
            for (int r = 1; r <= range; r++)
            {
                var ringCoords = HexGrid.GetRing(CurrentHex.Coordinates, r);
                foreach (var coord in ringCoords)
                {
                    var cell = grid.FirstOrDefault(c => c.Coordinates == coord);
                    if (cell != null && cell.IsTraversable)
                    {
                        _attackRangeHexes.Add(cell);
                    }
                }
            }
        }

        public virtual void TakeDamage(int amount)
        {
            CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
            if (CurrentHealth <= 0)
            {
                QueueFree();
            }
        }

        // Utility methods for range checking
        public bool IsInAttackRange(HexCell cell) => _attackRangeHexes.Contains(cell);
        public bool IsInMoveRange(HexCell cell) => _moveRangeHexes.Contains(cell);

        public bool CanAttack(Unit target) =>
            target != null &&
            target != this &&
            target.CurrentHex != null &&
            IsInAttackRange(target.CurrentHex);

        public void ShowMoveRange(Color color)
        {
            foreach (HexCell cell in MoveRangeHexes)
            {
                cell.Highlight(color);
            }
        }

        public void HideMoveRange()
        {
            foreach (HexCell cell in MoveRangeHexes)
            {
                cell.ClearHighlight();
            }
        }

        public void ShowAttackRange(Color color)
        {
            foreach (HexCell cell in AttackRangeHexes)
            {
                cell.SetColor(color);
            }
        }
        public void HideAttackRange()
        {
            foreach (HexCell cell in AttackRangeHexes)
            {
                cell.ClearHighlight();
            }
        }
    }
}
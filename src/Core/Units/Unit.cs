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
        public List<HexCell> MoveRangeHexes = new();
        public List<HexCell> AttackRangeHexes = new();

        public override void _Ready()
        {
            AddToGroup("Unit");
            CurrentHealth = MaxHealth;
        }

        public void UpdateRanges()
        {
            if (CurrentHex == null) return;

            MoveRangeHexes = CurrentHex.Neighbors;
            AttackRangeHexes = CurrentHex.Neighbors;
        }

        public List<Unit> GetUnitsInRange()
        {
            return AttackRangeHexes
                .Where(hex => hex.Unit != null && hex.Unit != this)
                .Select(hex => hex.Unit)
                .ToList();
        }

        public bool IsUnitInRange(Unit target)
        {
            return AttackRangeHexes.Any(hex => hex.Unit == target);
        }

        public List<HexCell> GetOverlappingRange(List<HexCell> range)
        {
            return AttackRangeHexes.Intersect(range).ToList();
        }

        public virtual void TakeDamage(int amount)
        {
            CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
            if (CurrentHealth <= 0)
            {
                QueueFree();
            }
        }

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

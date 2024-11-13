using System.Collections.Generic;
using Godot;

namespace HexTactics.Core
{
    public partial class Unit : Node3D
    {
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
        public HexCell CurrentHex { get; set; }
        public int CurrentHealth { get; private set; }

        public override void _Ready()
        {
            AddToGroup("Unit");
            CurrentHealth = MaxHealth;
        }

        public virtual void TakeDamage(int amount)
        {
            CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
            if (CurrentHealth <= 0)
            {
                QueueFree();
            }
        }
    }
}
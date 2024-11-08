using Godot;
using HexTactics.Core.ECS;
using HexTactics.Core.Enums;
using HexTactics.Entities.World;

namespace HexTactics.Entities.Units
{
    public partial class Unit : Entity
    {
        [Export] public int MoveRange { get; set; } = 2;
        [Export] public int AttackPower { get; set; } = 1;
        [Export] public int MaxHealth { get; set; } = 3;

        public HexCell CurrentHex { get; set; }
        public int CurrentHealth { get; private set; }

        [Export] public UnitType Type { get; set; }

        public override void _Ready()
        {
            base._Ready();
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
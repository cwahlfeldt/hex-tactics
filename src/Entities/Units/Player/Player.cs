using HexTactics.Core.Enums;

namespace HexTactics.Entities.Units
{
    public partial class Player : Unit
    {
        public override void _Ready()
        {
            base._Ready();
            Type = UnitType.Player;
            MoveRange = 3;  // Players might move further
            AttackPower = 2; // Players might hit harder
            MaxHealth = 5;   // Players might be tougher
        }
    }
}
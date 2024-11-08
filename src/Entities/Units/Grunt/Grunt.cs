using HexTactics.Core.Enums;

namespace HexTactics.Entities.Units
{
    public partial class Enemy : Unit
    {
        public override void _Ready()
        {
            base._Ready();
            Type = UnitType.Grunt;
            // Use default values from Unit or set specific enemy values
        }
    }
}
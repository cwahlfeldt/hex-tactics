namespace HexTactics.Core
{
    public partial class Player : Unit
    {
        public override void _Ready()
        {
            UnitType = UnitType.Player;
            AddToGroup("Player");
        }
    }
}
namespace HexTactics.Core.Grunt
{
    public partial class Grunt : Unit
    {
        public override void _Ready()
        {
            UnitType = UnitType.Grunt;
            AddToGroup("Enemy");
        }
    }
}
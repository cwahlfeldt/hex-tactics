using Godot;

namespace HexTactics.Core.Grunt
{
    public partial class Grunt : Unit
    {

        public override void _Ready()
        {
            AddToGroup("Enemy");
            GD.Print("Grunt ready");
        }
    }
}
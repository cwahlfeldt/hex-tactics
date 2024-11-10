using Godot;

namespace HexTactics.Core
{
    public partial class Enemy : Unit
    {
        public override void _Ready()
        {
            GD.Print("Grunt ready");
        }
    }
}
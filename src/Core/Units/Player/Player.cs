using Godot;

namespace HexTactics.Core
{
    public partial class Player : Unit
    {
        public override void _Ready()
        {
            GD.Print("Player ready");
        }
    }
}
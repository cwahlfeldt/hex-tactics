using Godot;

namespace HexTactics.Core.Player
{
    public partial class Player : Unit
    {
        public override void _Ready()
        {
            AddToGroup("Player");
            GD.Print("Player ready");
        }
    }
}
using Godot;
using HexTactics.Core;

namespace HexTactics.Scenes
{
    public partial class TestScene : Node3D
    {
        public override void _Ready()
        {
            GameManager.Instance.StartGame();
        }
    }
}
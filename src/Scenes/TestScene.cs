using Godot;
using HexTactics.Entities.World.Managers;

namespace HexTactics.Scenes
{
    public partial class TestScene : Node3D
    {
        private HexGridManager _hexGridManager;

        public override void _Ready()
        {
            _hexGridManager = GetNode<HexGridManager>("HexGridManager");
            _hexGridManager.Initialize(); 
        }
    }
}
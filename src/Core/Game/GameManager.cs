using Godot;

namespace HexTactics.Core
{
    public partial class GameManager : Node3D
    {
        public static GameManager Instance { get; private set; }
        public HexGridManager HexGridManager { get; private set; }

        [Export] public int MapSize { get; set; } = 5;
        [Export] public int PlayerStartHexIndex { get; set; } = 0;
        [Export] public int DisabledCells { get; set; } = 8;
        [Export] public bool ShowPathDebug { get; set; } = false;
        [Export] public bool ShowHexDebug { get; set; } = true;

        public override void _Ready()
        {
            Instance = this;
            PlayerStartHexIndex = MapSize + 2;
        }

        public void Start()
        {
            ClearCurrentGame();
            HexGridManager = new HexGridManager();
        }

        private void ClearCurrentGame()
        {
            HexGridManager?.Clear();
            foreach (var child in GetChildren())
            {
                child.QueueFree();
            }
        }
    }
}
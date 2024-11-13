using System.Collections.Generic;
using Godot;

namespace HexTactics.Core
{
    public partial class GameManager : Node3D
    {
        public static GameManager Instance { get; private set; }
        public HexGridManager HexGridManager { get; private set; }
        public UnitManager UnitManager { get; private set; }
        public TurnManager TurnManager { get; private set; }

        [Export] public int MapSize { get; set; } = 5;
        [Export] public int PlayerStartHexIndex { get; set; } = 0;
        [Export] public int DisabledCells { get; set; } = 8;
        [Export] public bool ShowPathDebug { get; set; } = false;
        [Export] public bool ShowHexDebug { get; set; } = true;

        public override void _Ready()
        {
            SignalBus.Instance.HexSelected += OnHexSelected;

            Instance = this;
            
            PlayerStartHexIndex = MapSize + 2;
        }

        public void StartGame()
        {
            ClearCurrentGame();

            HexGridManager = new HexGridManager();

            UnitManager = new UnitManager(new Dictionary<string, HexCell> {
                {"Player", HexGridManager.GetGrid()[PlayerStartHexIndex]},
                {"Enemy_1", HexGridManager.GetGrid()[PlayerStartHexIndex + 40]},
            });

            TurnManager = new TurnManager(UnitManager.GetAllUnits());
        }

        private void OnHexSelected(HexCell cell)
        {
            if (cell == null) return;
            GD.Print("Hex selected: " + cell.Index);
        }

        private void ClearCurrentGame()
        {
            // HexGridManager?.Clear();
            foreach (var child in GetChildren())
            {
                child.QueueFree();
            }
        }
    }
}
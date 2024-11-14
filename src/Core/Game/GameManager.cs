using System.Collections.Generic;
using System.Linq;
using Godot;

namespace HexTactics.Core
{
    public partial class GameManager : Node3D
    {
        public static GameManager Instance { get; private set; }
        public HexGridManager HexGridManager { get; private set; }
        public UnitManager UnitManager { get; private set; }
        public TurnManager TurnManager { get; private set; }
        public Unit player;
        public HexCell selectedHex;
        private Dictionary<GameState, GameStateBase> states = new();
        private GameStateBase currentState;

        [Export] public int MapSize { get; set; } = 5;
        [Export] public int PlayerStartHexIndex { get; set; } = 0;
        [Export] public int DisabledCells { get; set; } = 8;
        [Export] public bool ShowPathDebug { get; set; } = true;
        [Export] public bool ShowHexDebug { get; set; } = false;

        public override void _Ready()
        {
            SignalBus.Instance.HexSelected += OnHexSelected;
            SignalBus.Instance.TurnStarted += OnTurnStarted;
            SignalBus.Instance.TurnEnded += OnTurnEnded;
            Instance = this;
            PlayerStartHexIndex = MapSize + 2;
            InitializeStateMachine();
        }

        public void StartGame()
        {
            ClearCurrentGame();
            HexGridManager = new HexGridManager();
            UnitManager = new UnitManager();
            player = UnitManager.SpawnPlayer(HexGridManager.GetGrid()[PlayerStartHexIndex]);
            UnitManager.SpawnEnemy(HexGridManager.GetGrid()[PlayerStartHexIndex + 40], "Grunt");
            UnitManager.SpawnEnemy(HexGridManager.GetGrid()[PlayerStartHexIndex + 57], "Grunt");
            TurnManager = new TurnManager(UnitManager.GetAllUnits());
            TurnManager.StartTurn();
        }

        private void InitializeStateMachine()
        {
            states = new Dictionary<GameState, GameStateBase>
            {
                { GameState.Start, new StartState() },
                { GameState.Move, new MoveState() },
                { GameState.Action, new ActionState() },
                { GameState.End, new EndState() }
            };
        }

        public void ChangeState(GameState newState)
        {
            GD.Print($"{TurnManager.CurrentUnit.Name}: {newState}");

            currentState?.Exit();
            currentState = states[newState];
            currentState.Enter();
        }

        private void OnHexSelected(HexCell cell)
        {
            selectedHex = cell;

            if (TurnManager.IsPlayerTurn())
            {
                ChangeState(GameState.Move);
            }
        }

        private void OnTurnStarted(Unit unit)
        {
            foreach (HexCell neighbor in unit.CurrentHex.Neighbors)
            {
                neighbor.ClearHighlight();
            }
            ChangeState(GameState.Start);
        }

        private void OnTurnEnded(Unit unit)
        {
            foreach (HexCell neighbor in unit.CurrentHex.Neighbors)
            {
                neighbor.Highlight(Colors.Red);
            }
        }

        private void ClearCurrentGame()
        {
            foreach (var child in GetChildren())
            {
                child.QueueFree();
            }
        }
    }
}
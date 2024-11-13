using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using static HexTactics.Core.TurnManager;

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
        public enum GameState
        {
            Start,
            Move,
            Action,
            End
        }
        public GameState gameState = GameState.Start;
        [Export] public int MapSize { get; set; } = 5;
        [Export] public int PlayerStartHexIndex { get; set; } = 0;
        [Export] public int DisabledCells { get; set; } = 8;
        [Export] public bool ShowPathDebug { get; set; } = true;
        [Export] public bool ShowHexDebug { get; set; } = false;

        public override void _Ready()
        {
            SignalBus.Instance.HexSelected += OnHexSelected;
            // SignalBus.Instance.GameStarted += OnGameStarted;
            // SignalBus.Instance.UnitMoved += OnUnitMoved;
            SignalBus.Instance.TurnStarted += OnTurnStarted;
            // SignalBus.Instance.TurnEnded += OnTurnEnded;
            Instance = this;
            PlayerStartHexIndex = MapSize + 2;
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
            ChangeState(GameState.Start);
        }

        // private void OnTurnEnded(Unit unit)
        // {
        //     ChangeState(GameState.Start);
        // }

        private void ChangeState(GameState newState)
        {
            var currentState = gameState;
            gameState = newState;


            GD.Print(TurnManager.CurrentUnit.Name + ": " + currentState + " -> " + gameState);

            switch (gameState)
            {
                case GameState.Start:
                    HandleStart();
                    break;
                case GameState.Action:
                    HandleAction();
                    break;
                case GameState.Move:
                    HandleMove();
                    break;
                case GameState.End:
                    HandleEnd();
                    break;
                default:
                    break;
            }
        }

        public void HandleStart()
        {
            ChangeState(GameState.Action);
        }

        public void HandleEnd()
        {
            TurnManager.EndTurn();

        }

        public void HandleMove()
        {
            var currentUnit = TurnManager.GetCurrentUnit();

            if (currentUnit != null)
            {
                if (TurnManager.IsPlayerTurn())
                {
                    if (selectedHex != null && selectedHex.Unit == null)  // Make sure destination is empty
                    {
                        UnitManager.MoveUnit(currentUnit, selectedHex, () => ChangeState(GameState.End));
                        selectedHex = null;
                    }
                }
                else // Enemy turn
                {
                    // Get all available moves within range
                    var availableMoves = HexGridManager.GetReachableNodes(currentUnit.CurrentHex, currentUnit.MoveRange)
                        .Where(hex => hex.Unit == null)  // Filter to only empty hexes
                        .ToList();

                    if (availableMoves.Any())
                    {
                        // Move to hex closest to player
                        var bestMove = availableMoves
                            .OrderBy(hex => HexGrid.GetDistance(hex.Coordinates, player.CurrentHex.Coordinates))
                            .First();

                        UnitManager.MoveUnit(currentUnit, bestMove, () => ChangeState(GameState.End));
                    }
                    else
                    {
                        ChangeState(GameState.End);
                    }
                }
            }
        }

        public void HandleAction()
        {
            if (!TurnManager.IsPlayerTurn())
                ChangeState(GameState.Move);
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
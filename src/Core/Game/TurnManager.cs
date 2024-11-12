using Godot;
using System.Collections.Generic;
using System.Linq;

namespace HexTactics.Core
{
    public partial class TurnManager : Node
    {
        [Signal]
        public delegate void TurnStartedEventHandler(int unitIndex);

        [Signal]
        public delegate void TurnEndedEventHandler(int unitIndex);

        [Signal]
        public delegate void PhaseChangedEventHandler(string phase);

        private readonly List<Unit> units = new();
        private int currentUnitIndex = 0;
        private TurnPhase currentPhase = TurnPhase.Idle;

        public Unit CurrentUnit => units[currentUnitIndex];

        public TurnPhase CurrentPhase => currentPhase;

        public enum TurnPhase
        {
            Idle,
            Start,
            Action,
            End
        }

        public override void _Ready()
        {
            // Initialize turn system when the node is ready
            InitializeTurnSystem();
        }

        private void InitializeTurnSystem()
        {
            // Get all players in the scene
            var unitsInScene = GetTree().GetNodesInGroup("Unit").Cast<Unit>();
            units.AddRange(unitsInScene);

            if (!units.Any())
            {
                GD.PrintErr("No players found in the scene!");
                return;
            }

            StartNewTurn();
        }

        public void StartNewTurn()
        {
            SetPhase(TurnPhase.Start);
            EmitSignal(SignalName.TurnStarted, CurrentUnit);
            // Auto-progress to main phase after start phase
            SetPhase(TurnPhase.Action);
        }

        public void EndCurrentTurn()
        {
            SetPhase(TurnPhase.End);
            EmitSignal(SignalName.TurnEnded, CurrentUnit);

            // Move to next player
            currentUnitIndex = (currentUnitIndex + 1) % units.Count;

            // Start the next player's turn
            StartNewTurn();
        }

        private void SetPhase(TurnPhase newPhase)
        {
            currentPhase = newPhase;
            EmitSignal(SignalName.PhaseChanged, nameof(CurrentPhase));
        }

        // Helper method to check if it's a specific player's turn
        public bool IsPlayerTurn(Unit player) => CurrentUnit == player;
    }
}
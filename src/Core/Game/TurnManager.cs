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

        private List<Unit> units = new();
        private int currentUnitIndex = 0;
        private TurnPhase currentPhase = TurnPhase.Idle;

        public Unit CurrentUnit => units[currentUnitIndex];

        public TurnPhase CurrentPhase => currentPhase;

        public enum TurnPhase
        {
            Idle,
            Start,
            Move,
            Action,
            End
        }

        public TurnManager(List<Unit> _units)
        {
            units.AddRange(_units);

            if (!units.Any())
            {
                GD.PrintErr("No units found in the scene!");
                return;
            }
        }

        public void StartTurn()
        {
            SetPhase(TurnPhase.Start);
            SignalBus.Instance.EmitSignal(SignalBus.SignalName.TurnStarted, CurrentUnit);
            SetPhase(TurnPhase.Action);
        }

        public void EndTurn()
        {
            SetPhase(TurnPhase.End);
            SignalBus.Instance.EmitSignal(SignalBus.SignalName.TurnEnded, CurrentUnit);

            // Move to next player
            currentUnitIndex = (currentUnitIndex + 1) % units.Count;

            // Start the next player's turn
            StartTurn();
        }

        public void SetPhase(TurnPhase newPhase)
        {
            currentPhase = newPhase;
            SignalBus.Instance.EmitSignal(SignalBus.SignalName.TurnChanged, CurrentUnit);
        }

        public TurnPhase GetTurnPhase() => currentPhase;

        // Helper method to check if it's a specific player's turn
        public bool IsPlayerTurn() => CurrentUnit.Name == GameManager.Instance.player.Name;

        internal Unit GetCurrentUnit()
        {
            return CurrentUnit;
        }
    }
}

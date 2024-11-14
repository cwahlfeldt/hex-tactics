using System.Linq;
using Godot;

namespace HexTactics.Core
{
    public partial class MoveState : GameStateBase
    {
        public override void Enter()
        {
            var currentUnit = GameManager.Instance.TurnManager.GetCurrentUnit();

            if (currentUnit != null)
            {
                if (GameManager.Instance.TurnManager.IsPlayerTurn())
                {
                    if (GameManager.Instance.selectedHex != null && GameManager.Instance.selectedHex.Unit == null)  // Make sure destination is empty
                    {
                        GameManager.Instance.UnitManager.MoveUnit(currentUnit, GameManager.Instance.selectedHex, () => GameManager.Instance.ChangeState(GameState.End));
                        GameManager.Instance.selectedHex = null;
                    }
                }
                else // Enemy turn
                {
                    // Get all available moves within range
                    var availableMoves = GameManager.Instance.HexGridManager.GetReachableNodes(currentUnit.CurrentHex, currentUnit.MoveRange)
                        .Where(hex => hex.Unit == null)  // Filter to only empty hexes
                        .ToList();

                    if (availableMoves.Any())
                    {
                        // Move to hex closest to player
                        var bestMove = availableMoves
                            .OrderBy(hex => HexGrid.GetDistance(hex.Coordinates, GameManager.Instance.player.CurrentHex.Coordinates))
                            .First();

                        GameManager.Instance.UnitManager.MoveUnit(currentUnit, bestMove, () => GameManager.Instance.ChangeState(GameState.End));
                    }
                    else
                    {
                        GameManager.Instance.ChangeState(GameState.End);
                    }
                }
            }
        }
    }
}
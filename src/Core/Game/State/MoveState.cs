using System.Linq;
using Godot;

namespace HexTactics.Core
{
    public class MoveState : GameStateBase
    {
        public MoveState(GameManager gameManager) : base(gameManager) { }
        public override void Enter()
        {
            var selectedHex = GameManager.selectedHex;
            var currentUnit = GameManager.TurnManager.GetCurrentUnit();
            currentUnit.HideMoveRange();

            if (currentUnit != null)
            {
                if (GameManager.TurnManager.IsPlayerTurn())
                {
                    PlayerMove(currentUnit, selectedHex);
                }
                else
                {
                    EnemyMove(currentUnit);
                }
            }
        }

        private void PlayerMove(Unit player, HexCell selectedHex)
        {
            if (selectedHex != null && selectedHex.Unit == null)
            {
                selectedHex.Highlight(Colors.Aqua);
                GameManager.UnitManager.MoveUnit(player, selectedHex, () =>
                {
                    selectedHex.ClearHighlight();
                    GameManager.ChangeState(GameState.End);
                });
            }
        }

        private void EnemyMove(Unit enemy)
        {
            var player = GameManager.player;
            var targetHex = GameManager.HexGridManager.FindTargetHex(enemy.CurrentHex, player.CurrentHex, enemy);
            targetHex.Highlight(Colors.Red);

            GameManager.UnitManager.MoveUnit(enemy, targetHex, () =>
            {
                targetHex.ClearHighlight();
                GameManager.ChangeState(GameState.End);
            });
        }
    }
}
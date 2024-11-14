using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Godot;

namespace HexTactics.Core
{
    public partial class ActionState : GameStateBase
    {
        public ActionState(GameManager gameManager) : base(gameManager) { }
        public override void Enter()
        {
            var currentUnit = GameManager.TurnManager.CurrentUnit;
            var selectedHex = GameManager.selectedHex;

            if (GameManager.TurnManager.IsPlayerTurn())
                PlayerAction(currentUnit, selectedHex);
            else
                EnemyAction(currentUnit);
        }

        private void PlayerAction(Unit player, HexCell selectedHex)
        {
            if (selectedHex != null)
            {
                var enemies = GameManager.UnitManager.GetAllEnemies();
                foreach (var enemy in enemies)
                {
                    if (player.CanAttack(enemy))
                    {
                        GD.Print("Can attack");
                    }
                }
                GameManager.ChangeState(GameState.Move);
            }
        }

        private void EnemyAction(Unit enemy)
        {
            GameManager.ChangeState(GameState.Move);
        }
    }
}
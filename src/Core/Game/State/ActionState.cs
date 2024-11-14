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

        private async void PlayerAction(Unit player, HexCell selectedHex)
        {
            if (selectedHex != null)
            {
                var unitsInRange = player.GetUnitsInRange();
                var attackableHexes = new List<HexCell>();
                foreach (var unit in unitsInRange)
                {
                    var overlappingRanges = player.GetOverlappingRange(unit.AttackRangeHexes);
                    foreach (var range in overlappingRanges)
                    {
                        attackableHexes.Add(range);
                    }
                }
                GameManager.ChangeState(GameState.Move);
                if (attackableHexes.Contains(selectedHex))
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(300));
                    unitsInRange[0].TakeDamage(player.AttackPower);
                }
                
            }
        }

        private void EnemyAction(Unit enemy)
        {
            GameManager.ChangeState(GameState.Move);
        }
    }
}
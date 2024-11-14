using System;
using System.Threading.Tasks;
using Godot;

namespace HexTactics.Core
{
    public partial class StartState : GameStateBase
    {
        public StartState(GameManager gameManager) : base(gameManager) { }
        public override void Enter()
        {
            var currentUnit = GameManager.TurnManager.GetCurrentUnit();

            if (GameManager.TurnManager.IsPlayerTurn())
                PlayerStart(currentUnit);
            else
                EnemyStart(currentUnit);
        }


        private void PlayerStart(Unit player)
        {
            player.ShowMoveRange(Colors.Aqua);
            var unitsInRange = player.GetUnitsInRange();
            foreach (var unit in unitsInRange)
            {
                var overlappingRanges = player.GetOverlappingRange(unit.AttackRangeHexes);
                foreach (var range in overlappingRanges)
                {
                    range.Highlight(Colors.Red);
                }
            }
        }

        private async void EnemyStart(Unit enemy)
        {
            enemy.ShowMoveRange(Colors.Red);

            await Task.Delay(TimeSpan.FromMilliseconds(500));
            GameManager.ChangeState(GameState.Action);
        }
    }
}
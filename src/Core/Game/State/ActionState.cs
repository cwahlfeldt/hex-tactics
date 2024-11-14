using Godot;

namespace HexTactics.Core
{
    public partial class ActionState : GameStateBase
    {
        public override void Enter()
        {
            if (!GameManager.Instance.TurnManager.IsPlayerTurn())
                GameManager.Instance.ChangeState(GameState.Move);
        }
    }
}
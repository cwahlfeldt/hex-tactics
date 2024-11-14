using Godot;

namespace HexTactics.Core
{
    public partial class StartState : GameStateBase
    {
        public override void Enter()
        {
            GameManager.Instance.ChangeState(GameState.Action);
        }
    }
}
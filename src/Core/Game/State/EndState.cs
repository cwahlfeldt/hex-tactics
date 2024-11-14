using Godot;

namespace HexTactics.Core
{
    public partial class EndState : GameStateBase
    {
        public override void Enter()
        {
            GameManager.Instance.TurnManager.EndTurn();
        }
    }
}
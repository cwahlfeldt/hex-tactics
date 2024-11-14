using Godot;

namespace HexTactics.Core
{
    public partial class EndState : GameStateBase
    {
        public EndState(GameManager gameManager) : base(gameManager) { }

        public override void Enter()
        {
            if (GameManager.UnitManager.GetAllUnits().Count == 0)
            {
                GD.Print("All units have been eliminated!");
                return;
            }
            GameManager.selectedHex = null;
            GameManager.TurnManager.EndTurn();
        }
    }
}
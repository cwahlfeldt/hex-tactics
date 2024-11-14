namespace HexTactics.Core
{
    public partial class EndState : GameStateBase
    {
        public EndState(GameManager gameManager) : base(gameManager) { }

        public override void Enter()
        { 
            GameManager.selectedHex = null;    
            GameManager.TurnManager.EndTurn();

        }
    }
}
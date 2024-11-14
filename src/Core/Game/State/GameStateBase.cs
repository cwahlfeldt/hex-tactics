namespace HexTactics.Core
{
    public abstract class GameStateBase
    {
        protected readonly GameManager GameManager;
        protected GameStateBase(GameManager gameManager) => GameManager = gameManager;
        public abstract void Enter();
        public virtual void Exit() {}
    }
}

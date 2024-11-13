namespace HexTactics.Core
{
    public abstract class GameStateBase
    {
        public abstract void Enter();
        public abstract void Update();
        public abstract void Exit();
    }
}

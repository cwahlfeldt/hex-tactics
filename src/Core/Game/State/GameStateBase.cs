using System;

namespace HexTactics.Core
{
    public abstract class GameStateBase
    {
        public abstract void Enter();
        public virtual void Exit() {}
    }
}

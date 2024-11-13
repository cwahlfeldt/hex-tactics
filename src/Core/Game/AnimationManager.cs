using Godot;

namespace HexTactics.Core {
    public class AnimationManager {

        private readonly Tween _tween;
        public AnimationManager() { 
            _tween = new Tween();
        }
        public void MoveThroughPath() {
            if (_tween == null) {
                GD.PrintErr("Tween is null");
                return;
            }
        }
    }
}
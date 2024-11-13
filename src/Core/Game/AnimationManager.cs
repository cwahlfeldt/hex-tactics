using Godot;
using System;
using System.Collections.Generic;

namespace HexTactics.Core
{
    public partial class AnimationManager : Node
    {
        public static AnimationManager Instance { get; private set; }
        
        private const float DEFAULT_MOVEMENT_DURATION = 0.3f;
        private const float DEFAULT_ROTATION_DURATION = 0.15f;
        private const Tween.TransitionType DEFAULT_TRANS_TYPE = Tween.TransitionType.Sine;
        private const Tween.EaseType DEFAULT_EASE_TYPE = Tween.EaseType.InOut;
        
        private readonly Dictionary<Node, Tween> _activeTweens = new();
        
        public override void _Ready() => Instance = this;
        
        public void MoveThrough(
            Node3D target, 
            List<Vector3> positions, 
            Action onComplete = null, 
            float moveDuration = DEFAULT_MOVEMENT_DURATION,
            float rotationDuration = DEFAULT_ROTATION_DURATION)
        {
            if (positions == null || positions.Count == 0 || target == null)
            {
                onComplete?.Invoke();
                return;
            }

            StopAnimation(target);
            MoveToNextPosition(target, new Queue<Vector3>(positions), moveDuration, rotationDuration, onComplete);
        }

        private void MoveToNextPosition(
            Node3D target, 
            Queue<Vector3> remainingPositions, 
            float moveDuration,
            float rotationDuration,
            Action onComplete)
        {
            if (remainingPositions.Count == 0)
            {
                onComplete?.Invoke();
                return;
            }

            var nextPosition = remainingPositions.Dequeue();
            if (target.GlobalPosition.DistanceSquaredTo(nextPosition) <= 0.001f)
            {
                MoveToNextPosition(target, remainingPositions, moveDuration, rotationDuration, onComplete);
                return;
            }

            LookAt(target, nextPosition, () => 
            {
                var moveTween = CreateTween();
                _activeTweens[target] = moveTween;

                moveTween.TweenProperty(
                    target,
                    "global_position",
                    nextPosition,
                    moveDuration
                ).SetTrans(DEFAULT_TRANS_TYPE)
                .SetEase(DEFAULT_EASE_TYPE);

                moveTween.Finished += () =>
                {
                    MoveToNextPosition(target, remainingPositions, moveDuration, rotationDuration, onComplete);
                };
            }, rotationDuration);
        }

        public void LookAt(Node3D target, Vector3 point, Action onComplete = null, float duration = DEFAULT_ROTATION_DURATION)
        {
            if (target == null)
            {
                onComplete?.Invoke();
                return;
            }

            var tween = CreateTween();
            _activeTweens[target] = tween;

            tween.TweenProperty(
                target,
                "basis",
                target.GlobalTransform.LookingAt(point, Vector3.Up).Basis,
                duration
            ).SetTrans(DEFAULT_TRANS_TYPE)
            .SetEase(DEFAULT_EASE_TYPE);

            tween.Finished += () =>
            {
                _activeTweens.Remove(target);
                onComplete?.Invoke();
            };
        }
        
        public void StopAnimation(Node target)
        {
            if (_activeTweens.TryGetValue(target, out var tween))
            {
                tween.Kill();
                _activeTweens.Remove(target);
            }
        }
        
        public void StopAllAnimations()
        {
            foreach (var tween in _activeTweens.Values)
            {
                tween.Kill();
            }
            _activeTweens.Clear();
        }
        
        public bool IsAnimating(Node target) => _activeTweens.ContainsKey(target);
    }
}
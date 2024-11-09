using Godot;
using System.Collections.Immutable;
using System.Linq;

namespace HexTactics.Core.ECS
{
    public abstract partial class Entity : Node3D
    {
        // Immutable list of components
        private ImmutableList<Component> _components = ImmutableList<Component>.Empty;

        public Entity()
        {
            _components = ImmutableList<Component>.Empty;
        }

        public T AddComponent<T>() where T : Component, new()
        {
            var component = new T();
            AddChild(component);
            _components = _components.Add(component);
            return component;
        }

        public T GetComponent<T>() where T : Component
        {
            return _components.Find(c => c is T) as T;
        }

        public bool HasComponent<T>() where T : Component
        {
            return _components.Any(c => c is T);
        }

        public void RemoveComponent<T>() where T : Component
        {
            var component = GetComponent<T>();
            if (component != null)
            {
                _components = _components.Remove(component);
                component.QueueFree();
            }
        }
    }
}
using Godot;
using System.Collections.Immutable;

namespace HexTactics.Core.ECS
{
    public abstract partial class System : Node3D
    {
        // Immutable list of registered entities
        private ImmutableList<Entity> _entities = ImmutableList<Entity>.Empty;

        // Register an entity with this system
        public void RegisterEntity(Entity entity)
        {
            if (!_entities.Contains(entity))
            {
                _entities = _entities.Add(entity);
            }
        }

        // Unregister an entity from this system
        public void UnregisterEntity(Entity entity)
        {
            _entities = _entities.Remove(entity);
        }

        // Get current entities (returns immutable copy)
        protected ImmutableList<Entity> GetEntities()
        {
            return _entities;
        }

        // Abstract update method that systems must implement
        public abstract void Update(double delta);
    }
}
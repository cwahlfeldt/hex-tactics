using Godot;

namespace HexTactics.Core.ECS
{
    public abstract partial class Component : Node
    {
        public new Entity Owner => GetParent<Entity>();

        protected Component() { }

        // Create clean copy of component
        public abstract Component Clone();
    }
}
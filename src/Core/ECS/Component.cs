using Godot;

namespace HexTactics.Core.ECS
{
    public abstract partial class Component : Node
    {
        public new Entity Owner => GetParent() as Entity;
    }
}
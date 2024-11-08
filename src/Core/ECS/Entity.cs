using Godot;
using System;
using System.Collections.Generic;

namespace HexTactics.Core.ECS
{
    public abstract partial class Entity : Node3D
    {
        protected Dictionary<Type, Component> Components = new();

        public T AddComponent<T>() where T : Component, new()
        {
            var component = new T();
            AddChild(component);
            Components[typeof(T)] = component;
            return component;
        }

        public T GetComponent<T>() where T : Component
        {
            return Components.GetValueOrDefault(typeof(T)) as T;
        }

        public bool HasComponent<T>() where T : Component
        {
            return Components.ContainsKey(typeof(T));
        }
    }
}
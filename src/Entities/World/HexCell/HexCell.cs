using Godot;
using System.Collections.Generic;
using HexTactics.Core.ECS;
using HexTactics.Core.Pathfinding;

namespace HexTactics.Entities.World
{
    public partial class HexCell : Entity, IPathfindingNode
    {
        public int Index { get; set; }
        public Vector3I Coordinates { get; set; }
        public List<HexCell> Neighbors { get; set; } = new();
        public bool IsTraversable { get; set; } = true;

        // IPathfindingNode implementation
        Vector3 IPathfindingNode.Position => GlobalPosition;
        IEnumerable<IPathfindingNode> IPathfindingNode.Neighbors => Neighbors;

        private MeshInstance3D _meshInstance;
        private StandardMaterial3D _material;

        public override void _Ready()
        {
            base._Ready();
            SetupVisuals();
        }

        private void SetupVisuals()
        {
            _meshInstance = GetNode<MeshInstance3D>("Mesh");
            if (_meshInstance != null)
            {
                _material = _meshInstance.GetSurfaceOverrideMaterial(0) as StandardMaterial3D;
                if (_material != null)
                {
                    _material = _material.Duplicate() as StandardMaterial3D;
                    _meshInstance.SetSurfaceOverrideMaterial(0, _material);
                }
            }
        }

        public void Highlight(Color color)
        {
            if (_material != null)
            {
                _material.AlbedoColor = color;
            }
        }

        public void ClearHighlight()
        {
            if (_material != null)
            {
                _material.AlbedoColor = Colors.White;
            }
        }
    }
}
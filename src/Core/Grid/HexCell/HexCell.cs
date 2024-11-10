using Godot;
using System.Collections.Generic;


namespace HexTactics.Core
{
    public partial class HexCell : Node3D
    {
        public int Index { get; set; }
        public Vector3I Coordinates { get; set; }
        public bool IsTraversable { get; set; } = true;
        public List<HexCell> Neighbors { get; set; } = new();
        public Unit Unit { get; set; }
        private MeshInstance3D _meshInstance;
        private StandardMaterial3D _material;

        public override void _Ready()
        {
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
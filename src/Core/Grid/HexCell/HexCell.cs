using Godot;
using System.Collections.Generic;

namespace HexTactics.Core
{
    public partial class HexCell : Area3D
    {
        public int Index { get; set; }
        public Vector3I Coordinates { get; set; }
        public List<HexCell> Neighbors { get; set; } = new();
        public Unit Unit { get; set; }
        private MeshInstance3D _meshInstance;
        private StandardMaterial3D _material;
        private static readonly float _opacity = 0.8f;
        private Color _color = new(Colors.White.R, Colors.White.G, Colors.White.B, _opacity);
        private bool _isTraversable = true;
        [Export] public bool IsTraversable { get => _isTraversable; set => SetTraversable(value); }

        public override void _Ready()
        {
            SetupVisuals();
        }

        private void SetupVisuals()
        {
            _meshInstance = GetNode<MeshInstance3D>("Mesh");
            if (_meshInstance != null)
            {
                _material = _meshInstance.GetActiveMaterial(0) as StandardMaterial3D;
                if (_material != null)
                {
                    _material = _material.Duplicate() as StandardMaterial3D;
                    _meshInstance.SetSurfaceOverrideMaterial(0, _material);
                    _material.Transparency = BaseMaterial3D.TransparencyEnum.Alpha;
                    SetColor(_color, _opacity);
                }
            }
        }

        public void SetTraversable(bool isTraversable)
        {
            _isTraversable = isTraversable;
            SetColor(_color, _isTraversable ? _opacity : 0.1f);
        }

        public void Highlight(Color color)
        {
            SetColor(color, _opacity);
        }

        public void ClearHighlight()
        {
            SetColor(_color, _opacity);
        }

        public void SetColor(Color color, float alpha = 1.0f)
        {
            if (_material == null) return;

            _material.AlbedoColor = new Color(
                color.R,
                color.G,
                color.B,
                alpha
            );
        }

        public void OnInputEvent(Node camera, InputEvent @event, Vector3 position, Vector3 normal, int shapeIdx)
        {
            if (@event is InputEventMouseButton mouseEvent)
            {
                if (mouseEvent.ButtonIndex == MouseButton.Left && mouseEvent.Pressed)
                {
                    SignalBus.Instance.EmitSignal(SignalBus.SignalName.HexSelected, this);
                }
            }
        }
    }
}
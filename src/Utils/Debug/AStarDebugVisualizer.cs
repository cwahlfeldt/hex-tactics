using Godot;

namespace HexTactics.Utils.Debug
{
	public partial class AStarDebugVisualizer : Node3D
	{
		[ExportGroup("Visualization Settings")]
		[Export] public float PointSize { get; set; } = 0.2f;
		[Export] public float LineThickness { get; set; } = 0.04f;
		[Export] public Color ActiveColor { get; set; } = Colors.Blue;
		[Export] public Color DisabledColor { get; set; } = Colors.Red;
		[Export] public Color ConnectionColor { get; set; } = Colors.DarkCyan;
		[Export] public float Opacity { get; set; } = 0.8f;

		private Node3D _debugPoints;
		private Node3D _debugConnections;
		private Mesh _pointMesh;
		private Mesh _connectionMesh;
		private StandardMaterial3D _activeMaterial;
		private StandardMaterial3D _disabledMaterial;
		private StandardMaterial3D _connectionMaterial;

		public override void _Ready()
		{
			SetupMaterials();
			SetupContainers();
			CreateMeshes();
			this.Position = new Vector3(0.035f, 0.5f, 0.28f);
		}

		private void SetupMaterials()
		{
			_activeMaterial = CreateMaterial(ActiveColor);
			_disabledMaterial = CreateMaterial(DisabledColor);
			_connectionMaterial = CreateMaterial(ConnectionColor);
		}

		private StandardMaterial3D CreateMaterial(Color color)
		{
			return new StandardMaterial3D
			{
				NoDepthTest = true,
				ShadingMode = StandardMaterial3D.ShadingModeEnum.Unshaded,
				AlbedoColor = new Color(color, Opacity),
				Transparency = BaseMaterial3D.TransparencyEnum.Alpha
			};
		}

		private void SetupContainers()
		{
			_debugPoints = new Node3D { Name = "DebugPoints" };
			_debugConnections = new Node3D { Name = "DebugConnections" };

			AddChild(_debugPoints);
			AddChild(_debugConnections);
		}

		private void CreateMeshes()
		{
			_pointMesh = new SphereMesh
			{
				Radius = PointSize,
				Height = PointSize * 2
			};

			_connectionMesh = new CylinderMesh
			{
				TopRadius = LineThickness,
				BottomRadius = LineThickness,
				Height = 1.0f
			};
		}

		public void Clear()
		{
			foreach (Node child in _debugPoints.GetChildren())
			{
				child.QueueFree();
			}

			foreach (Node child in _debugConnections.GetChildren())
			{
				child.QueueFree();
			}
		}

		public void VisualizeAstar(AStar3D astar)
		{
			Clear();

			var points = astar.GetPointIds();

			// Draw connections first (so they appear behind points)
			foreach (long pointId in points)
			{
				var fromPos = astar.GetPointPosition(pointId);
				var connections = astar.GetPointConnections(pointId);

				foreach (long connectedPoint in connections)
				{
					var toPos = astar.GetPointPosition(connectedPoint);
					DrawConnection(fromPos, toPos);
				}
			}

			// Then draw points
			foreach (long pointId in points)
			{
				var pos = astar.GetPointPosition(pointId);
				var isDisabled = astar.IsPointDisabled(pointId);
				DrawPoint(pos, isDisabled);
			}
		}

		private void DrawPoint(Vector3 point, bool isDisabled)
		{
			var meshInstance = new MeshInstance3D
			{
				Mesh = _pointMesh,
				MaterialOverride = isDisabled ? _disabledMaterial : _activeMaterial,
				Position = point
			};

			_debugPoints.AddChild(meshInstance);
		}

		private void DrawConnection(Vector3 fromPos, Vector3 toPos)
		{
			var meshInstance = new MeshInstance3D
			{
				Mesh = _connectionMesh,
				MaterialOverride = _connectionMaterial
			};

			// Calculate the middle point and look at target
			float distance = fromPos.DistanceTo(toPos);
			meshInstance.Position = fromPos.Lerp(toPos, 0.5f);

			// Look at the target position
			meshInstance.LookAtFromPosition(
				meshInstance.Position,
				toPos,
				Vector3.Up
			);

			// Rotate to align with connection direction
			meshInstance.RotateObjectLocal(Vector3.Right, Mathf.Pi / 2);

			// Scale the cylinder to match the distance
			meshInstance.Scale = new Vector3(1, distance, 1);

			_debugConnections.AddChild(meshInstance);
		}
	}
}
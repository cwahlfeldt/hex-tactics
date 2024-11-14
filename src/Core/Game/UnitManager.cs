using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace HexTactics.Core
{
    public partial class UnitManager
    {
        private readonly PackedScene _playerScene = ResourceLoader.Load<PackedScene>("res://src/Core/Units/Player/Player.tscn");
        private readonly PackedScene _enemyScene = ResourceLoader.Load<PackedScene>("res://src/Core/Units/Grunt/Grunt.tscn");
        private Unit _playerUnit;
        private readonly List<Unit> _enemyUnits = new();
        // private readonly Dictionary<Unit, HexCell> _unitPositions = new();
        // private readonly Dictionary<HexCell, List<Unit>> _hexUnits = new();

        public Unit SpawnPlayer(HexCell hex)
        {
            _playerUnit = _playerScene.Instantiate<Unit>();
            GameManager.Instance.AddChild(_playerUnit); // Add to scene first
            _playerUnit.GlobalPosition = hex.GlobalPosition;
            RegisterUnit(_playerUnit, hex);
            return _playerUnit;
        }

        public Unit SpawnEnemy(HexCell hex, string type = "Grunt")
        {
            var enemyScene = ResourceLoader.Load<PackedScene>($"res://src/Core/Units/{type}/{type}.tscn");
            var enemy = enemyScene.Instantiate<Unit>();
            GameManager.Instance.AddChild(enemy);
            enemy.GlobalPosition = hex.GlobalPosition;
            RegisterUnit(enemy, hex);
            _enemyUnits.Add(enemy);
            enemy.Rotate(new Vector3(0, 1, 0), Mathf.DegToRad(180));
            enemy.Name = $"{type}_{_enemyUnits.Count}";

            return enemy;
        }

        public static void RegisterUnit(Unit unit, HexCell hex)
        {
            if (unit.IsInsideTree())
            {
                unit.GlobalPosition = hex.GlobalPosition;
            }
            unit.CurrentHex = hex;
            hex.Unit = unit;
        }

        public static void UnregisterUnit(Unit unit)
        {
            unit.CurrentHex.Unit = null;
            unit.CurrentHex = null;
        }

        public static void RemoveUnit(Unit unit)
        {
            UnregisterUnit(unit);
            unit.QueueFree();
        }

        public void MoveUnit(Unit unit, HexCell toHex, Action OnUnitMoved = null)
        {
            var fromHex = unit.CurrentHex;
            if (fromHex == toHex) return;

            var path = GameManager.Instance.HexGridManager.FindPath(fromHex, toHex, unit.MoveRange);
            if (!path.Any())
                GD.PrintErr("No path found for unit " + unit.Name);

            var locations = path.Select(h => h.GlobalPosition).ToList();
            AnimationManager.Instance.MoveThrough(unit, locations, () =>
            {
                UpdateUnitPosition(unit, fromHex, path.Last());
                OnUnitMoved();
            });
        }

        private static void UpdateUnitPosition(Unit unit, HexCell fromHex, HexCell targetHex)
        {
            fromHex.Unit = null;
            targetHex.Unit = unit;
            unit.CurrentHex = targetHex;
        }

        public List<Unit> GetAllUnits()
        {
            var allUnits = new List<Unit> { _playerUnit };
            allUnits.AddRange(_enemyUnits);
            return allUnits;
        }
    }
}
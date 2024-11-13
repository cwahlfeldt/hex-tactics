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
        private readonly Dictionary<Unit, HexCell> _unitPositions = new();
        private readonly Dictionary<HexCell, List<Unit>> _hexUnits = new();

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

        public void RegisterUnit(Unit unit, HexCell hex)
        {
            if (!_unitPositions.ContainsKey(unit))
            {
                _unitPositions[unit] = hex;

                if (!_hexUnits.ContainsKey(hex))
                {
                    _hexUnits[hex] = new List<Unit>();
                }

                _hexUnits[hex].Add(unit);

                if (unit.IsInsideTree())
                {
                    unit.GlobalPosition = hex.GlobalPosition;
                }
                unit.CurrentHex = hex;
                hex.Unit = unit;
            }
        }

        public void UnregisterUnit(Unit unit)
        {
            if (_unitPositions.TryGetValue(unit, out HexCell currentHex))
            {
                _hexUnits[currentHex].Remove(unit);
                if (!_hexUnits[currentHex].Any())
                {
                    _hexUnits.Remove(currentHex);
                }
                _unitPositions.Remove(unit);

                unit.CurrentHex.Unit = null;
            }
        }

        public void RemoveUnit(Unit unit)
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
                // GameManager.Instance.HexGridManager.UpdatePathfinding();
            });
        }

        private void UpdateUnitPosition(Unit unit, HexCell fromHex, HexCell targetHex)
        {
            if (fromHex != null && _hexUnits.ContainsKey(fromHex))
            {
                _hexUnits[fromHex].Remove(unit);
                if (!_hexUnits[fromHex].Any())
                {
                    _hexUnits.Remove(fromHex);
                }
            }

            _unitPositions[unit] = targetHex;
            unit.CurrentHex = targetHex;

            // Update hex unit references
            targetHex.Unit = unit;  // Changed from fromHex.Unit to just unit
            fromHex.Unit = null;
            // targetHex.IsUnitTurn = false;

            if (!_hexUnits.ContainsKey(targetHex))
            {
                _hexUnits[targetHex] = new List<Unit>();
            }
            _hexUnits[targetHex].Add(unit);
        }

        public bool HasUnits(HexCell hex) =>
            _hexUnits.ContainsKey(hex) && _hexUnits[hex].Any();

        public List<Unit> GetUnitsInAttackRange(Unit fromUnit, int attackRange)
        {
            var unitsInRange = new List<Unit>();
            var fromHex = fromUnit.CurrentHex;

            foreach (var kvp in _hexUnits)
            {
                var hex = kvp.Key;
                // You'll need to implement hex distance calculation
                if (HexGrid.GetDistance(fromHex.Coordinates, hex.Coordinates) <= attackRange)
                {
                    unitsInRange.AddRange(kvp.Value.Where(u => u != fromUnit));
                }
            }

            return unitsInRange;
        }

        public Unit GetPlayer() => _playerUnit;

        public List<Unit> GetEnemies() => _enemyUnits;

        public List<Unit> GetAllUnits()
        {
            var allUnits = new List<Unit> { _playerUnit };
            allUnits.AddRange(_enemyUnits);
            return allUnits;
        }
    }
}
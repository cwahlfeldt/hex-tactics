// src/Core/Events/SignalBus.cs
using Godot;
using HexTactics.Entities.World;
using HexTactics.Entities.Units;

namespace HexTactics.Core.SignalBus
{
	public partial class SignalBus : Node
	{
		public static SignalBus Instance { get; private set; }

		[Signal] public delegate void HexSelectedEventHandler(HexCell cell);
		[Signal] public delegate void UnitMovedEventHandler(Unit unit);
		[Signal] public delegate void TurnEndedEventHandler(Unit unit);
		[Signal] public delegate void UnitAttackEventHandler(Unit attacker, Unit target);
		[Signal] public delegate void GameStartedEventHandler();

		public override void _Ready()
		{
			if (Instance == null)
			{
				Instance = this;
			}
			else
			{
				QueueFree();
			}
		}
	}
}
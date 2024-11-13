using Godot;
using static HexTactics.Core.TurnManager;

namespace HexTactics.Core
{
	public partial class SignalBus : Node
	{	
		[Signal] public delegate void HexSelectedEventHandler(HexCell cell);
		[Signal] public delegate void UnitMovedEventHandler(Unit unit);
		[Signal] public delegate void TurnStartedEventHandler(Unit unit);
		[Signal] public delegate void TurnChangedEventHandler(Unit unit);
		[Signal] public delegate void TurnEndedEventHandler(Unit unit);
		[Signal] public delegate void UnitAttackEventHandler(Unit attacker, Unit target);
		[Signal] public delegate void GameStartedEventHandler();

		public static SignalBus Instance { get; private set; }

		public override void _Ready()
		{
			Instance = this;
		}
	}
}
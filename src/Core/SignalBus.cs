using Godot;

namespace HexTactics.Core
{
	public partial class SignalBus : Node
	{
		[Signal] public delegate void HexSelectedEventHandler(HexCell cell);
		[Signal] public delegate void UnitMovedEventHandler(Unit unit);
		[Signal] public delegate void TurnEndedEventHandler(Unit unit);
		[Signal] public delegate void UnitAttackEventHandler(Unit attacker, Unit target);
		[Signal] public delegate void GameStartedEventHandler();
	}
}
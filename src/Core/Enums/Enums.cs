namespace HexTactics.Core
{
    public enum GameState
    {
        Start,
        Move,
        Action,
        End
    }

    public enum UnitType
    {
        Player,
        Enemy
    }

    public enum RangeType 
    { 
        Melee, 
        Ranged, 
        Magic, 
        Bomb 
    }
}
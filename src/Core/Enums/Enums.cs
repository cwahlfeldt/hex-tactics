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
        Grunt
    }

    public enum RangeType 
    { 
        Melee, 
        Ranged, 
        Magic, 
        Bomb 
    }
}
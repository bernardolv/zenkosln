using System.Collections;
using System.Collections.Generic;
namespace Zenko
{
    public enum PieceType
    {
        Wall,
        Left, Right, Up, Down,
        PortalLeft, PortalRight, PortalUp, PortalDown,
        WallSeed,
        LeftSeed, RightSeed, UpSeed, DownSeed
    }
    public enum EnvironmentType
    {
        Ice,
        Wall,
        Start,
        Goal,
        Wood,
        Fragile,
        Hole
    }
    [System.Serializable]
    public class LevelSettings
    {
        public int outerDimension;
        public int innerDimension;
        public int wallsAmount;
        public int holesAmount;
        public int fragilesAmount;
        public int woodsAmount;
        public List<PieceType> pieceTypes = new List<PieceType>();
    }
    public enum ConditionType { Wind, Portal, NoSeedStop, PieceStop, MinTurns, NoWindFrontal, PortalBlocked };
}

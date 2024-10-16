using System.Collections;
using System.Collections.Generic;
using Zenko.Entities;
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
        public int innerDimension;
        public int outerDimension;
        public int wallsAmount;
        public int holesAmount;
        public int fragilesAmount;
        public int woodsAmount;
        public List<PieceType> pieceTypes = new List<PieceType>();

        public LevelSettings()
        {

        }

        public LevelSettings(int innerDimension = 0, int outerDimension = 0, int wallsAmount = 0, int holesAmount = 0, int fragilesAmount = 0, int woodsAmount = 0, List<PieceType> pieceTypes = default)
        {
            this.innerDimension = innerDimension;
            this.outerDimension = outerDimension;
            this.wallsAmount = wallsAmount;
            this.holesAmount = holesAmount;
            this.fragilesAmount = fragilesAmount;
            this.woodsAmount = woodsAmount;
            this.pieceTypes = pieceTypes ?? new List<PieceType>();
        }
    }
    public enum ConditionType { Wind, Portal, NoSeedStop, PieceStop, MinTurns, NoWindFrontal, PortalBlocked };
}

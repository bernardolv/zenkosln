using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        public int wallsAmountMin;
        public int wallsAmountMax;
        public int holesAmountMin;
        public int holesAmountMax;
        public int fragilesAmountMin;
        public int fragilesAmountMax;
        public int woodsAmountMin;
        public int woodsAmountMax;
        public int broadPieces = 0; //If this value is 0, it uses pieceTypes, if value is higher than 0 then it picks random pieces
        public List<PieceType> pieceTypes = new List<PieceType>();

        public LevelSettings()
        {

        }

        public LevelSettings(int innerDimension = 0, int outerDimension = 0, int wallsAmountMin = 0, int wallsAmountMax = 0, int holesAmountMin = 0, int holesAmountMax = 0, int fragilesAmountMin = 0, int fragilesAmountMax = 0, int woodsAmountMin = 0, int woodsAmountMax = 0, int broadPieces = 0, List<PieceType> pieceTypes = default)
        {
            this.innerDimension = innerDimension;
            this.outerDimension = outerDimension;
            this.wallsAmountMin = wallsAmountMin;
            this.wallsAmountMax = wallsAmountMax;
            this.holesAmountMin = holesAmountMin;
            this.holesAmountMax = holesAmountMax;
            this.fragilesAmountMin = fragilesAmountMin;
            this.fragilesAmountMax = fragilesAmountMax;
            this.woodsAmountMin = woodsAmountMin;
            this.woodsAmountMax = woodsAmountMax;
            this.broadPieces = broadPieces;
            this.pieceTypes = pieceTypes ?? new List<PieceType>();
        }

        public List<PieceType> GeneratePieceTypes()
        {
            if (broadPieces == 0)
            {
                return pieceTypes;
            }

            PieceType[] portals = new PieceType[4] { PieceType.PortalLeft, PieceType.PortalRight, PieceType.PortalUp, PieceType.PortalDown };

            List<PieceType> result = new List<PieceType>();
            bool portalsAdded = false;
            for (int i = 0; i < broadPieces; i++)
            {
                bool found = false;
                while (!found)
                {
                    Array values = Enum.GetValues(typeof(PieceType));
                    Random random = new Random();
                    PieceType randomPieceType = (PieceType)values.GetValue(random.Next(values.Length));

                    if (portals.Contains(randomPieceType))
                    {
                        if (portalsAdded)
                        {
                            //Cant add a portal when portals have already been added
                        }
                        else if (i + 1 == broadPieces)
                        {
                            //Cant add a portal when its the last piece...
                        }
                        else
                        {
                            result.Add(randomPieceType);

                            //Add the matching portal
                            randomPieceType = (PieceType)portals.GetValue(random.Next(portals.Length));
                            result.Add(randomPieceType);
                            found = true;
                            portalsAdded = true;
                            i++;
                        }
                    }
                    else
                    {
                        found = true;
                        result.Add(randomPieceType);
                    }
                }
            }

            return result;
        }
    }
    public enum ConditionType { Wind, Portal, NoSeedStop, PieceStop, MinTurns, NoWindFrontal, PortalBlocked };
}

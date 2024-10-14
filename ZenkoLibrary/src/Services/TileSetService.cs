using System.Collections.Generic;
using Zenko.Entities;

namespace Zenko.Services
{
    public class TileSetService
    {
        //Refactor pls.
        public static List<V2Int> GetOuterWallPositions(TileSet tileSet, int outerDimension, int innerDimension)
        {
            List<V2Int> result = new List<V2Int>();
            System.Random rnd = new System.Random();
            // int difference = (totaldimensions - icedimensions);

            int amount = outerDimension - innerDimension;

            int wallsleft;
            int wallsright;
            int wallsup;
            int wallsdown;

            if (amount % 2 == 0)
            { //if pair amount, equal sides on each)
                wallsleft = amount / 2;
                wallsright = amount / 2;
                wallsup = amount / 2;
                wallsdown = amount / 2;
            }
            else
            {
                int randomizer = rnd.Next(0, 2);
                int randomizer2 = rnd.Next(0, 2);
                if (randomizer == 1)
                {
                    wallsleft = amount / 2;
                    wallsright = amount / 2 + 1;
                }
                else
                {
                    wallsleft = amount / 2 + 1;
                    wallsright = amount / 2;
                }
                if (randomizer2 == 1)
                {
                    wallsup = amount / 2;
                    wallsdown = amount / 2 + 1;
                }
                else
                {
                    wallsup = amount / 2 + 1;
                    wallsdown = amount / 2;
                }
            }
            for (int i = 0; i < outerDimension; i++)
            {
                int var;
                if (i < wallsup || i > outerDimension - 1 - wallsdown)
                {
                    var = 0;
                }
                else
                {
                    var = 1;
                }
                switch (var)
                {
                    case 0:
                        for (int j = 0; j < outerDimension; j++)
                        {
                            result.Add(new V2Int(j, i));
                        }
                        break;
                    case 1:
                        for (int j = 0; j < outerDimension; j++)
                        {
                            if (j < wallsleft || j > (outerDimension - 1 - wallsright))
                            {
                                result.Add(new V2Int(j, i));
                            }
                            else
                            {
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            return result;
        }

        public static List<V2Int> GetDoorablePositions(TileSet tileSet, int outerDimension)
        {
            int xDimension = tileSet.GetXDimension();
            int yDimension = tileSet.GetYDimension();

            List<V2Int> doorablePositions = new List<V2Int>();

            for (int i = 0; i < outerDimension; i++)
            {
                for (int j = 0; j < outerDimension; j++)
                {
                    if (tileSet.GetTile(j, i).GetTileType() == "Wall")
                    {
                        CheckSides(j, i);
                    }
                }
            }
            void CheckSides(int x, int y)
            { //This checks if a tile is suitable for a Door
                int icenextcounter = 0;
                GetWallTag(x - 1, y);
                GetWallTag(x + 1, y);
                GetWallTag(x, y + 1);
                GetWallTag(x, y - 1);
                if (icenextcounter > 0)
                {
                    int myx = x;
                    int myy = y;
                    doorablePositions.Add(new V2Int(myx, myy));
                    //			Debug.Log("doorable" + myx + " " + myy);
                }

                void GetWallTag(int x, int y)
                {//currently only checks for ice (for doorables)
                    if (x >= xDimension || x < 0)
                    {

                    }
                    else if (y >= yDimension || y < 0)
                    {

                    }
                    else if (tileSet.GetTile(x, y).GetTileType() == "Ice")
                    {
                        icenextcounter++;
                    }
                    else
                    {

                    }
                }
            }
            return doorablePositions;
        }

        public static List<V2Int> GetIcePositions(TileSet tileSet, int dimensions)
        {
            List<V2Int> icePositions = new List<V2Int>();
            for (int y = 0; y < dimensions; y++)
            {
                for (int x = 0; x < dimensions; x++)
                {
                    if (tileSet.GetTile(x, y).GetTileType() == "Ice")
                    {
                        icePositions.Add(new V2Int(x, y));
                    }
                }
            }
            return icePositions;
        }
    }
}

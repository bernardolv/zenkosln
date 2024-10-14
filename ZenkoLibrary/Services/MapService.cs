using System.Collections;
using System.Collections.Generic;
using Zenko.Presets;

namespace Zenko.Services
{
    public class MapService
    {
        public static void SplitIdentifier(string identifier, out string identifierType, out string identifierNumber)
        {
            int identifierLength = identifier.Length;
            identifierType = "";
            identifierNumber = "";
            if (identifierLength == 2) //T1 up to T9
            {
                identifierType = identifier.Substring(0, 1);
                identifierNumber = identifier.Substring(1, 1);
            }
            else if (identifierLength == 3) //T10 plus and also P12,U99 etc.
            {
                identifierType = identifier.Substring(0, 1);
                identifierNumber = identifier.Substring(1, 2);
            }
            else if (identifierLength == 4)//PU12 PD99
            {
                identifierType = identifier.Substring(0, 2);
                identifierNumber = identifier.Substring(2, 2);
            }
            else if (identifierLength == 6)//P12_24
            {
                identifierType = identifier.Substring(0, 1);
                identifierNumber = identifier.Substring(1, 5);
            }
            else if (identifierLength == 7)//PU12_24
            {
                identifierType = identifier.Substring(0, 2);
                identifierNumber = identifier.Substring(2, 5);
            }
        }
        public static List<string> ConvertToStringArray(Entities.Map map)
        {
            List<string> fullString = new List<string>();
            string mapString = "";
            int dimension = map.GetYDimension();

            fullString.Add("Map" + dimension);
            for (int i = 0; i < dimension; i++)
            {
                mapString = "";
                for (int j = 0; j < dimension; j++)
                {
                    if (TileType.mapByType.ContainsKey(map.GetTileSet().GetType(new V2Int(j, i))))
                    {
                        mapString += TileType.mapByType[map.GetTileSet().GetType(new V2Int(j, i))] + " ";
                    }
                    else
                    {
                        mapString += TileType.mapByType["Ice"] + " ";
                    }
                }
                fullString.Add(mapString);
            }


            Entities.Solution solution = map.GetSolution();

            //Could use solution pieces instead, need to revisit and figure out exactly when is it used to see if we should really do that.
            //When generated with a solution its cool but what if there is no solution? or if we want for some reason to hold a non-solution one?
            Entities.Piece[] pieces = map.GetPieces();
            mapString = "";
            for (int i = 0; i < pieces.Length; i++)
            {
                Entities.Piece piece = pieces[i];
                mapString += piece.GetIdentifier();
                mapString += piece.GetX();
                mapString += piece.GetY();
                mapString += " ";
            }
            mapString += "T" + map.GetTurnsAmount();
            fullString.Add(mapString);
            fullString.Add("");
            return fullString;
        }
    }
}


using System.Collections;
using System.Collections.Generic;
using Zenko;
using Zenko.Entities;
using Zenko.Services;


namespace Zenko.Factories
{
    public static class MapFactory
    {
        const string TURNS = "T";

        //construct from string format
        //Map 10
        //1 1 0 1
        //1 0 1 1 etc...
        //last line example P11 T4

        //Creates the map, without pieces placed in the board, although the pieces are populated in the Pieces array.
        //tl;dr pieces are not placed in the tileset in this process
        public static Map? Map(string[] mapLines, int mapNumber = 0, bool ignoreLastLine = false)
        {
            Map map = new Map();
            if (mapLines.Length - 2 == 0)
            {
                Logger.LogError("Trying to create a Map from 0 dimensions");
                return null;
            }

            string line;
            int y;
            int x;
            string[] identifiers;

            //1. Create TileSet
            TileSet tileSet = new TileSet(mapLines.Length - 2, mapLines.Length - 2);
            //ignore first line since it contains non-board data
            for (int i = 1; i < mapLines.Length - 1; i++)
            {
                y = i - 1;
                // Add identifiers to tiles
                identifiers = mapLines[i].Split(new char[] { ' ' });
                x = 0;
                foreach (string identifier in identifiers)
                {
                    //validate identifier not empty
                    if (string.IsNullOrWhiteSpace(identifier))
                    {
                        continue;
                    }
                    //Identifier: '1','0','S'
                    //tileType: 'Wall' 'Ice' 'Start'
                    tileSet.SetTile(new V2Int(x, y), identifier);
                    x++;
                }
            }
            map.SetTileSet(tileSet);

            List<Piece> piecesList = new List<Piece>();
            //2. Create Pieces
            if (!ignoreLastLine)
            {
                //process last line
                line = mapLines[mapLines.Length - 1];
                identifiers = line.Split(new char[] { ' ' });
                foreach (string identifier in identifiers)
                {
                    //validate identifier not empty
                    if (string.IsNullOrWhiteSpace(identifier))
                    {
                        continue;
                    }
                    //TODO: IMPROVE,WONT WORK FOR 10+ dim
                    string type;
                    string number;
                    MapService.SplitIdentifier(identifier, out type, out number);
                    if (type == TURNS)
                    {
                        if (!int.TryParse(number, out int turnsAmount))
                        {
                            Logger.LogError("Invalid Turns");
                            return null;
                        }
                        map.GetSolution().SetTurns(turnsAmount);
                    }
                    else
                    {
                        if (!int.TryParse(number.Substring(0, 1), out x))
                        {
                            Logger.LogError("Invalid piece x value");
                            x = 0;
                            // return;
                        }
                        if (!int.TryParse(number.Substring(1, 1), out y))
                        {
                            Logger.LogError("Invalid x value");
                            y = 0;
                            // return;
                        }
                        piecesList.Add(new Piece(x, y, type));
                    }
                }
            }


            //3. Create TileSet

            //4. Set To variables

            //pieces,hints,turns

            //pieces
            //turns
            // Debug.Log(mapLines.Length - 2);
            //TODO: Alter if map rules are altered
            map.GetSolution().SetPieces(piecesList.ToArray());

            //Makes sure they are deep copied and separate from solutions' pieces
            Piece[] mapPieces = new Piece[piecesList.Count];
            for (int i = 0; i < piecesList.Count; i++)
            {
                mapPieces[i] = piecesList[i].Clone();
            }
            map.SetPieces(mapPieces);

            // Debug.Log(JsonUtility.ToJson(this, true));
            //either set solution here or most likely set it externally.

            return map;
        }

        public static Map Map(TileSet tileSet, Solution solution = null)
        {
            Map map = new Map();
            map.SetTileSet(tileSet.Clone());
            map.SetSolution(solution.Clone());

            //Since we're initializing from a tileset+solution, then the pieces we can either get from solution.getpieces OR from tileset.placedpieces
            //since its not safe to assume pieces will always be placed in the tileset, we get from solution
            //TODO: Could test if tileset usually has or not...
            //Performs a DEEP COPY, this makes sure that solution.pieces != map.pieces which is the desired effect so that we can move map.pieces and tileset.placedpieces around without affecting the given solution
            if (solution != null)
            {
                Piece[] pieces = new Piece[solution.GetPieces().Length];
                for (int i = 0; i < pieces.Length; i++)
                {
                    pieces[i] = solution.GetPieces()[i].Clone();
                }
                map.SetPieces(pieces);
            }

            return map;
        }

        public static Map Clone(Map map)
        {
            Map clone = new Map();

            //Revisit
            if (map.GetTileSet() != null)
            {
                clone.SetTileSet(map.GetTileSet().Clone());
            }

            if (map.GetSolution() != null)
            {
                clone.SetSolution(map.GetSolution().Clone());
            }


            //clone pieces
            List<Piece> pieceList = new List<Piece>();
            foreach (Piece piece in map.GetPieces())
            {
                pieceList.Add(piece.Clone());
            }
            clone.SetPieces(pieceList.ToArray());

            return clone;
        }
    }
}


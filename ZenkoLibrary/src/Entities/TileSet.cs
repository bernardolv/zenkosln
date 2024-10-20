using System.Collections;
using System.Collections.Generic;
using Zenko.Presets;
using Zenko.Extensions;

namespace Zenko.Entities
{
    //holds tiles without pieces
    //all environment
    [System.Serializable]
    public class TileSet
    {
        /////////////////////////
        //      PROPERTIES     //
        /////////////////////////

        protected Tile[,] tiles;
        protected V2Int startPosition;
        protected V2Int goalPosition;
        protected V2Int playerPosition;

        public List<V3> modifiedPositions = new List<V3>();


        ////////////////////////
        //  BOARD PROPERTIES  //
        ////////////////////////

        //Eventually send these up to Map
        public List<V2Int> activePortalPositions = new List<V2Int>();
        public List<Piece> placedPieces = new List<Piece>();
        public List<V2Int> placedPiecePositions = new List<V2Int>();


        /////////////////////////
        //  HELPER PROPERTIES  //
        /////////////////////////

        public List<V2Int> poppedSeedPositions = new List<V2Int>();
        public List<V2Int> fragileConvertedPositions = new List<V2Int>();


        /////////////////////////
        //     CONSTRUCTORS    //
        /////////////////////////

        public TileSet() { }

        public TileSet(int xDimension, int yDimension)
        {
            tiles = new Tile[xDimension, yDimension];
        }

        //returns a clone of self, Double check last 3 methods
        public TileSet Clone()
        {
            TileSet clone = new TileSet();
            clone.SetTiles(CloneTiles());
            clone.SetStartPosition(GetStartPosition());
            clone.SetPlayerPosition(GetPlayerPosition());
            clone.SetGoalPosition(GetGoalPosition());

            //REVISIT TO MAKE SURE ITS DEEP CLONING ALL
            clone.SetActivePortalPositions(new List<V2Int>(GetActivePortalPositions()));
            clone.placedPieces = new List<Piece>();
            foreach (Piece piece in GetPlacedPieces())
            {
                clone.placedPieces.Add(piece.Clone());
            }
            clone.SetPlacedPiecePositions(new List<V2Int>(GetPlacedPiecePositions()));
            return clone;
        }

        public Tile[,] CloneTiles()
        {
            int xDimension = GetXDimension();
            int yDimension = GetYDimension();
            Tile[,] newTiles = new Tile[xDimension, yDimension];
            for (int y = 0; y < yDimension; y++)
            {
                for (int x = 0; x < xDimension; x++)
                {
                    newTiles[x, y] = GetTile(x, y).Clone();
                }
            }
            return newTiles;
        }

        /////////////////////////
        //  BOARD CONSTRUCTORS //
        /////////////////////////


        /////////////////////////
        //  GETTERS & SETTERS  //
        /////////////////////////

        public Tile GetTile(int x, int y)
        {
            return tiles[x, y];
        }
        public Tile GetTile(V2Int position)
        {
            return tiles[position.x, position.y];
        }
        //This has so much logic it could maybe be in a controller or factory?
        public void SetTile(V2Int position, string identifier)
        {
            if (tiles[position.x, position.y] == null)
            {
                tiles[position.x, position.y] = new Tile();
            }
            Tile tile = tiles[position.x, position.y];
            tile.SetX(position.x);
            tile.SetY(position.y);
            tile.SetIdentifier(identifier);
            tile.SetTileType(TileType.mapByIdentifier[identifier]);

            if (identifier == TileType.START)
            {
                SetStartPosition(position);
                SetPlayerPosition(position);
            }
            if (identifier == TileType.GOAL)
            {
                SetGoalPosition(position);
            }
            if (identifier != TileType.ICE)
            {
                tile.SetIsTaken(true);
            }
        }
        public void SetTile(V2Int position, Tile tile)
        {
            tiles[position.x, position.y] = tile.Clone();
        }
        public void SetTiles(Tile[,] tiles)
        {
            this.tiles = tiles;
        }
        public V2Int GetStartPosition()
        {
            return startPosition;
        }
        public void SetStartPosition(V2Int startPosition)
        {
            this.startPosition = startPosition;
        }
        public V2Int GetGoalPosition()
        {
            return goalPosition;
        }
        public void SetGoalPosition(V2Int goalPosition)
        {
            this.goalPosition = goalPosition;
        }
        public V2Int GetPlayerPosition()
        {
            return playerPosition;
        }
        public void SetPlayerPosition(V2Int playerPosition)
        {
            this.playerPosition = playerPosition;
        }

        public string GetType(V2Int position)
        {
            return GetTile(position).GetTileType();
        }
        public int GetXDimension()
        {
            return tiles.GetLength(0);
        }
        public int GetYDimension()
        {
            return tiles.GetLength(1);
        }


        /////////////////////////
        //      BOARD G&S      //
        /////////////////////////

        public List<Piece> GetPlacedPieces()
        {
            return placedPieces;
        }
        public void SetPlacedPieces(List<Piece> placedPieces)
        {
            this.placedPieces = placedPieces;
        }
        public List<V2Int> GetActivePortalPositions()
        {
            return activePortalPositions;
        }

        public void SetActivePortalPositions(List<V2Int> activePortalPositions)
        {
            this.activePortalPositions = activePortalPositions;
        }
        public List<V2Int> GetPlacedPiecePositions()
        {
            return placedPiecePositions;
        }
        public void SetPlacedPiecePositions(List<V2Int> placedPiecePositions)
        {
            this.placedPiecePositions = placedPiecePositions;
        }

        //////////////////////////
        //    HELPER METHODS    //
        //////////////////////////

        public Piece GetPieceAt(V2Int position)
        {
            foreach (Piece piece in placedPieces)
            {
                if (piece.GetPosition() == position)
                {
                    return piece;
                }
            }
            throw new System.Exception("No piece at " + position);
        }
    }
}

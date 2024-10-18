using System.Collections.Generic;
using Zenko.Entities;
using Zenko;
using Zenko.Presets;
using Zenko.Services;

namespace Zenko.Factories
{
    public static class TileSetFactory
    {
        // Creates a random tileset from levelsettings
        // For LevelGenerator purposes
        public static TileSet TileSet(LevelSettings levelSettings)
        {
            TileSet tileSet = new TileSet();
            int xDimension = levelSettings.outerDimension;
            int yDimension = levelSettings.outerDimension;

            tileSet.SetTiles(new Tile[xDimension, yDimension]);

            for (int y = 0; y < yDimension; y++)
            {
                for (int x = 0; x < xDimension; x++)
                {
                    tileSet.SetTile(new V2Int(x, y), TileType.ICE);
                }
            }

            foreach (V2Int pos in TileSetService.GetOuterWallPositions(tileSet, xDimension, levelSettings.innerDimension))
            {
                tileSet.SetTile(pos, TileType.WALL);
            }

            List<V2Int> doorablePositions = TileSetService.GetDoorablePositions(tileSet, levelSettings.outerDimension);
            System.Random rnd = new System.Random();

            //Start Pos
            int randomIndex = rnd.Next(0, doorablePositions.Count);
            V2Int newStartPosition = doorablePositions[randomIndex];
            doorablePositions.RemoveAt(randomIndex);
            tileSet.SetTile(newStartPosition, TileType.START);

            //Goal Pos
            randomIndex = rnd.Next(0, doorablePositions.Count);
            V2Int newGoalPosition = doorablePositions[randomIndex];
            doorablePositions.RemoveAt(randomIndex);
            tileSet.SetTile(newGoalPosition, TileType.GOAL);

            //candidate positions updated in function
            List<V2Int> candidatePositions = TileSetService.GetIcePositions(tileSet, levelSettings.outerDimension);

            rnd = new System.Random();
            int wallsAmount = rnd.Next(levelSettings.wallsAmountMin, levelSettings.wallsAmountMax + 1);
            for (int i = 0; i < wallsAmount; i++)
            {
                int num = rnd.Next(0, candidatePositions.Count);
                V2Int newPosition = candidatePositions[num];
                candidatePositions.RemoveAt(num);
                tileSet.SetTile(newPosition, TileType.WALL);
            }

            rnd = new System.Random();
            int holesAmount = rnd.Next(levelSettings.holesAmountMin, levelSettings.holesAmountMax + 1);
            for (int i = 0; i < holesAmount; i++)
            {
                int num = rnd.Next(0, candidatePositions.Count);
                V2Int newPosition = candidatePositions[num];
                candidatePositions.RemoveAt(num);
                tileSet.SetTile(newPosition, TileType.HOLE);
            }

            rnd = new System.Random();
            int woodsAmount = rnd.Next(levelSettings.woodsAmountMin, levelSettings.woodsAmountMax + 1);
            for (int i = 0; i < woodsAmount; i++)
            {
                int num = rnd.Next(0, candidatePositions.Count);
                V2Int newPosition = candidatePositions[num];
                candidatePositions.RemoveAt(num);
                tileSet.SetTile(newPosition, TileType.FLOWER);
            }

            rnd = new System.Random();
            int fragilesAmount = rnd.Next(levelSettings.fragilesAmountMin, levelSettings.fragilesAmountMax + 1);
            for (int i = 0; i < fragilesAmount; i++)
            {
                int num = rnd.Next(0, candidatePositions.Count);
                V2Int newPosition = candidatePositions[num];
                candidatePositions.RemoveAt(num);
                tileSet.SetTile(newPosition, TileType.FRAGILE);
            }

            return tileSet;
        }

        //for testing purposes
        public static TileSet TileSet(V2Int startPos, V2Int endPos)
        {
            TileSet tileSet = new TileSet();
            int xDimension = 6;
            int yDimension = 6;
            tileSet.SetTiles(new Tile[xDimension, yDimension]);

            for (int y = 0; y < yDimension; y++)
            {
                for (int x = 0; x < xDimension; x++)
                {
                    tileSet.SetTile(new V2Int(x, y), TileType.ICE);
                }
            }
            foreach (V2Int pos in TileSetService.GetOuterWallPositions(tileSet, xDimension, 4))
            {
                tileSet.SetTile(pos, TileType.WALL);
            }

            tileSet.SetTile(startPos, TileType.START);
            tileSet.SetTile(endPos, TileType.GOAL);

            return tileSet;
        }
    }
}
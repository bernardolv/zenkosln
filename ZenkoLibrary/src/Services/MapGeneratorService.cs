using System.Collections.Generic;
using System;
using Zenko.Controllers;
using Zenko.Entities;
using Zenko;
using Zenko.Factories;
using System.Diagnostics;

namespace Zenko.Services
{
    public class MapGeneratorService
    {
        const bool DEBUG = true;
        int intervalToPrint = 0;
        int attempts;
        LevelSettings levelSettings;
        List<Condition> conditions;

        /// <summary>
        /// This callback is done when finding a valid map. The bool specifies if the generator should keep making maps upon finding a valid one
        /// </summary>
        Func<Map, int, bool> onMapFoundCallback = (Map map, int turns) => { return true; }; //True is continue, false is break

        public MapGeneratorService(LevelSettings levelSettings, int attempts = 0, List<Condition> conditions = default)
        {
            this.levelSettings = levelSettings;
            this.attempts = attempts;
            this.conditions = conditions;
        }

        public void SetOnMapFoundCallBack(Func<Map, int, bool> callBack)
        {
            this.onMapFoundCallback = callBack;
        }

        public void SetIntervalToPrint(int interval)
        {
            this.intervalToPrint = interval;
        }

        public void Generate()
        {
            int i = 0;
            while (i < attempts)
            {
                i++;
                try
                {
                    if (intervalToPrint != 0 && i % intervalToPrint == 0)
                    {
                        Logger.Log(i);
                    }
                    string log = "";
                    log += "attempting ";
                    TileSet tileSet = TileSetFactory.TileSet(levelSettings);
                    PieceType[] pieceTypes = levelSettings.GeneratePieceTypes().ToArray();

                    //Print map to test
                    if (DEBUG)
                    {
                        string pieces = "";
                        foreach (PieceType pieceType in pieceTypes)
                        {
                            pieces += pieceType.ToString() + ",";
                        }
                        Logger.Log(pieces.Remove(pieces.Length - 1));

                        foreach (string line in MapService.ConvertToStringArray(MapFactory.Map(tileSet, new Solution())))
                        {
                            Logger.Log(line);
                        }
                    }

                    if (SolutionController.TrySolveWithPiecesNew(tileSet, levelSettings.GeneratePieceTypes().ToArray(), out Solution solution, 1))
                    {
                        // log += ", Found with turns: " + solution.GetTurns();
                        //TODO: Conditions
                        if (!onMapFoundCallback.Invoke(MapFactory.Map(tileSet, solution), solution.GetTurns()))
                        {
                            // Logger.Log(log);
                            Logger.Log("Process finished since callback calls it so.");
                            return;
                        }
                    }
                    // Logger.Log(log);
                }
                catch (Exception e)
                {
                    Logger.Log("Skipped map generator service iteration due to error in process");
                }

            }
            Logger.Log("Process finished since attempt count has been reached.");
        }
    }
}
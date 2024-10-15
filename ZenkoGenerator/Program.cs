
using Zenko;
using Zenko.Entities;
using Zenko.Services;

public class Program
{
    public static void Main()
    {
        Console.WriteLine("HI");

        //Create level settings
        LevelSettings levelSettings = new LevelSettings();
        levelSettings.outerDimension = 6;
        levelSettings.innerDimension = 4;
        levelSettings.wallsAmount = 2;

        //Holds current best map
        Map bestMap = null;

        //Create callback
        Func<Map, int, bool> callback = new Func<Map, int, bool>((Map map, int turns) =>
        {

            if (bestMap == null || bestMap.GetSolution().GetTurns() < map.GetSolution().GetTurns())
            {
                bestMap = map;
            }

            return true;
        });

        MapGeneratorService generatorService = new MapGeneratorService(levelSettings, 100000);
        generatorService.SetOnMapFoundCallBack(callback);
        generatorService.Generate();


        //Print result
        if (bestMap != null)
        {
            foreach (string line in MapService.ConvertToStringArray(bestMap))
            {
                Logger.Log(line);
            }

        }
    }
}

using Zenko;
using Zenko.Entities;
using Zenko.Services;

public class Program
{
    public static void Main()
    {
        Console.WriteLine("HI");
        LevelSettings levelSettings = new LevelSettings();
        levelSettings.outerDimension = 6;
        levelSettings.innerDimension = 4;
        levelSettings.wallsAmount = 2;

        MapGeneratorService generatorService = new MapGeneratorService(levelSettings, 1000);

        Func<Map, int, bool> callback = new Func<Map, int, bool>((Map map, int turns) =>
        {

            foreach (string line in MapService.ConvertToStringArray(map))
            {
                Logger.Log(line);
            }

            return false;
        });

        generatorService.SetOnMapFoundCallBack(callback);

        generatorService.Generate();

    }
}
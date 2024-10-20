
using System.Text.Json;
using System.Text.Json.Serialization;
using Zenko;
using Zenko.Entities;
using Zenko.Services;

public class GeneratorController
{
    public static void Generate()
    {
        bool generating = false;
        while (!generating)
        {
            Console.WriteLine("HI, would you like to make a specific (S) generator or a broad (B) Generator?");
            string generatorMode = Console.ReadLine();
            switch (generatorMode)
            {
                case "S":
                    generating = true;
                    SpecificGenerator();
                    break;
                case "B":
                    generating = true;
                    BroadGenerator();
                    break;
                default:
                    Console.WriteLine("Invalid Selection");
                    break;
            }
        }


    }

    static void BroadGenerator()
    {
        string fileName = "generatedMaps" + DateTime.Now.ToString("yyyyMMddHHmm") + ".txt";

        LevelSettings levelSettings = CreateLevelSettingsBroad();

        //Holds current best map
        Map bestMap = null;
        List<string> mapLines = new List<string>();

        //Create callback
        Func<Map, int, bool> callback = new Func<Map, int, bool>((Map map, int turns) =>
        {
            //TODO: If conditions are met
            File.AppendAllLines(fileName, MapService.ConvertToStringArray(map));
            // Console.WriteLine("Found map with turns " + turns);

            if (bestMap == null || bestMap.GetSolution().GetTurns() < map.GetSolution().GetTurns())
            {
                bestMap = map;
            }

            return true;
        });

        Console.WriteLine("Starting Map Creation");
        Console.WriteLine("how many attempts should we try?");
        int attempts = Convert.ToInt32(Console.ReadLine());
        MapGeneratorService generatorService = new MapGeneratorService(levelSettings, attempts);

        Console.WriteLine("How often should we print a notice? (Periodicity of attempts alert?)");
        int printInterval = Convert.ToInt32(Console.ReadLine());
        generatorService.SetIntervalToPrint(printInterval);

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

    static void SpecificGenerator()
    {
        string fileName = "generatedMaps" + DateTime.Now.ToString("yyyyMMddHHmm") + ".txt";

        LevelSettings levelSettings = CreateLevelSettingsSpecific();

        //Holds current best map
        Map bestMap = null;
        List<string> mapLines = new List<string>();

        //Create callback
        Func<Map, int, bool> callback = new Func<Map, int, bool>((Map map, int turns) =>
        {
            //TODO: If conditions are met
            File.AppendAllLines(fileName, MapService.ConvertToStringArray(map));
            // Console.WriteLine("Found map with turns " + turns);

            if (bestMap == null || bestMap.GetSolution().GetTurns() < map.GetSolution().GetTurns())
            {
                bestMap = map;
            }

            return true;
        });

        Console.WriteLine("Starting Map Creation");
        Console.WriteLine("how many attempts should we try?");
        int attempts = Convert.ToInt32(Console.ReadLine());
        MapGeneratorService generatorService = new MapGeneratorService(levelSettings, attempts);

        Console.WriteLine("How often should we print a notice? (Periodicity of attempts alert?)");
        int printInterval = Convert.ToInt32(Console.ReadLine());
        generatorService.SetIntervalToPrint(printInterval);

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

    static LevelSettings CreateLevelSettingsBroad()
    {
        Console.WriteLine("What size should the tile board be? (4-8)");
        int size = Convert.ToInt16(Console.ReadLine());
        int outerSize = size + 2;
        Console.WriteLine("How many (inner) walls should it have MINIMUM?");
        int wallsAmountMin = Convert.ToInt16(Console.ReadLine());
        Console.WriteLine("How many (inner) walls should it have MAXIMUM?");
        int wallsAmountMax = Convert.ToInt16(Console.ReadLine());
        Console.WriteLine("How many holes should it have MINIMUM?");
        int holesAmountMin = Convert.ToInt16(Console.ReadLine());
        Console.WriteLine("How many holes should it have MAXIMUM?");
        int holesAmountMax = Convert.ToInt16(Console.ReadLine());
        Console.WriteLine("How many fragiles should it have MINIMUM?");
        int fragilesAmountMin = Convert.ToInt16(Console.ReadLine());
        Console.WriteLine("How many fragiles should it have MAXIMUM?");
        int fragilesAmountMax = Convert.ToInt16(Console.ReadLine());
        Console.WriteLine("How many woods should it have MINIMUM?");
        int woodsAmountMin = Convert.ToInt16(Console.ReadLine());
        Console.WriteLine("How many woods should it have MAXIMUM?");
        int woodsAmountMax = Convert.ToInt16(Console.ReadLine());

        Console.WriteLine("How many pieces should it use? (TODO: Exclude/include specific piecetypes)");
        int pieceCount = Convert.ToInt16(Console.ReadLine());

        return new LevelSettings(size, outerSize, wallsAmountMin, wallsAmountMax, holesAmountMin, holesAmountMax, fragilesAmountMin, fragilesAmountMax, woodsAmountMin, woodsAmountMax, pieceCount);
    }

    static LevelSettings CreateLevelSettingsSpecific()
    {
        Console.WriteLine("What size should the tile board be? (4-8)");
        int size = Convert.ToInt16(Console.ReadLine());
        int outerSize = size + 2;
        Console.WriteLine("How many (inner) walls should it have?");
        int wallsAmount = Convert.ToInt16(Console.ReadLine());
        Console.WriteLine("How many holes should it have?");
        int holesAmount = Convert.ToInt16(Console.ReadLine());
        Console.WriteLine("How many fragiles should it have?");
        int fragilesAmount = Convert.ToInt16(Console.ReadLine());
        Console.WriteLine("How many woods should it have?");
        int woodsAmount = Convert.ToInt16(Console.ReadLine());

        List<PieceType> pieceTypes = new List<PieceType>();
        bool addPieces = true;
        while (addPieces)
        {
            Console.WriteLine("Add a piece? (N/Y)");
            string result = Console.ReadLine();
            switch (result)
            {
                case "Y":
                    Console.WriteLine("What type? (P,L,R,U,D,p,l,r,u,d,PU,Pl,PR,PU,PD)");
                    string pieceIdentifier = Console.ReadLine();
                    if (!Zenko.Presets.PieceType.mapByIdentifier.ContainsKey(pieceIdentifier))
                    {
                        Console.WriteLine("Invalid piece type " + pieceIdentifier);
                        break;
                    }
                    pieceTypes.Add((PieceType)System.Enum.Parse(typeof(PieceType), Zenko.Presets.PieceType.mapByIdentifier[pieceIdentifier]));
                    break;
                case "N":
                    addPieces = false;
                    break;
                default:
                    Console.WriteLine("Invalid Response");
                    break;
            }
            if (pieceTypes.Count > 3)
            {
                Console.WriteLine("Thanks for adding pieces, 4 have been reached so thats that.");
                addPieces = false;
            }
        }

        return new LevelSettings(size, outerSize, wallsAmount, wallsAmount, holesAmount, holesAmount, fragilesAmount, fragilesAmount, woodsAmount, woodsAmount, 0, pieceTypes);
    }
}

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using Repositories;
using Zenko;
using Zenko.Controllers;
using Zenko.Entities;
using Zenko.Extensions;
using Zenko.Factories;
using Zenko.Utilities;

public class Program
{
    //Todo get args to do -v
    public static void Main(string[] args)
    {
        bool v = false;
        string filePath = "";

        //parse args
        if (!args.Contains("-f"))
        {
            Console.WriteLine("You need to specify the file with the -f flag");
            return;
        }

        for (int i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "-v":
                    v = true;
                    break;
                case "-f":
                    if (i + 1 >= args.Length)
                    {
                        Console.WriteLine("You need to specify the file with the -f flag");
                        return;
                    }
                    filePath = args[i + 1];
                    i++;
                    break;
                default:
                    Console.WriteLine("Invalid arg " + args[i]);
                    return;
            }
        }


        RepositoryService repositoryService = new RepositoryService();
        repositoryService.InitializeRepository(filePath);

        //Tesst first map only
        for (int i = 1; i <= repositoryService.LevelCount; i++)
        {
            //1. Parse map
            Map map = repositoryService.GetMap(i);
            //1.1 Print relevant data to debug
            if (v)
            {
                map.Print();
                PieceType[] pieceTypes = map.GetPieceTypes();
                foreach (PieceType p in pieceTypes)
                {
                    Console.WriteLine(p.ToString());
                }
            }

            //First get solution
            if (!SolutionController.TrySolveWithPiecesNew(map.GetTileSet(), map.GetPieceTypes(), out Solution solution, 1))
            {
                //No solution was found we can skip
                Console.Error.WriteLine("SKIPPING MAP: ");
                foreach (string line in map.GetPrintLines())
                {
                    Console.Error.WriteLine(line);
                }
                continue;
            }

            Conditions conditions = ConditionsController.GetConditions(map, solution);

            if (v)
            {
                //Now so
                solution.Print();
                foreach (V2Int move in solution.GetMoves())
                {
                    Console.WriteLine(move.ToString());
                }

            }
            conditions.Print();
        }
    }
}
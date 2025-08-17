
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
    public static void Main()
    {
        bool v = false;
        string filePath = "testmap.txt";
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
                    // Console.WriteLine(p.ToString());
                }
            }


            //Does not modify tileset, it clones it and modifies that one once inside
            SolutionController.TrySolveWithPiecesNew(map.GetTileSet(), map.GetPieceTypes(), out Solution solution, 1);

            // Console.WriteLine("CONDITIONS NOW");

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

    static string GetBackendJson(Map map, Solution solution, Conditions conditions)
    {
        string result = "";

        return result;
    }

    // static Conditions GetMapConditions(Map map, Solution solution)
    // {

    // }
}
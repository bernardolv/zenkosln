
using System.Text.Json;
using System.Text.Json.Serialization;
using Repositories;
using Zenko;
using Zenko.Controllers;
using Zenko.Entities;
using Zenko.Services;

public class Program
{
    public static void Main()
    {
        RepositoryService testRepositoryService = new RepositoryService();
        testRepositoryService.InitializeRepository("test.txt");
        foreach (string line in testRepositoryService.GetLevelLines(1))
        {
            Console.WriteLine(line);
        }

        Map map = testRepositoryService.GetMap(1);

        SolutionController.TrySolveWithPiecesNew(map.GetTileSet(), new PieceType[0], out Solution solution, 1);

        //Read text file
        //Select x map from text file
        //Run Solver Strategy while logging the active map count

        // GeneratorController.Generate();
    }
}
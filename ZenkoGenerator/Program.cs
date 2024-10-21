
using System.Diagnostics;
using System.Runtime.CompilerServices;
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
        // TestAlgorhithmSpeeds();
        // TestAlgorhithm();
        TestSpecificMap("test.txt", 1);
        // RepositoryService testRepositoryService = new RepositoryService();
        // testRepositoryService.InitializeRepository("test.txt");
        // foreach (string line in testRepositoryService.GetLevelLines(1))
        // {
        //     Console.WriteLine(line);
        // }

        // Map map = testRepositoryService.GetMap(1);

        // SolutionController.TrySolveWithPiecesNew(map.GetTileSet(), new PieceType[0], out Solution solution, 1);

        //Read text file
        //Select x map from text file
        //Run Solver Strategy while logging the active map count

        // GeneratorController.Generate();
    }


    static void TestAlgorhithm(int algorhithmNumber)
    {
        string errorMaps = "errorMaps " + DateTime.Now.ToString("yyyyMMddHHmm") + ".txt";
        string[] textFiles = new string[5] { "generatedMaps202410161303.txt", "generatedMaps202410162201.txt", "generatedMaps202410162208.txt", "generatedMaps202410172131.txt", "test.txt" };
        foreach (string file in textFiles)
        {
            Console.WriteLine(file);
            RepositoryService repositoryService = new RepositoryService();
            repositoryService.InitializeRepository(file);



            for (int i = 1; i < repositoryService.pointers.Length; i++)
            {
                Map map = repositoryService.GetMap(i);

                bool solved = SolutionController.TrySolveWithPiecesNew(map.GetTileSet(), map.GetPieceTypes(), out Solution solution, algorhithmNumber);
                // // Console.WriteLine(solved);
                // if (!solved)
                // {
                //     Console.WriteLine(repositoryService.pointers.Length);
                //     File.AppendAllLines(errorMaps, repositoryService.GetLevelLines(i));
                //     File.AppendAllLines(errorMaps, new string[1] { "" });
                //     foreach (string line in repositoryService.GetLevelLines(i))
                //     {
                //         Console.WriteLine(line);
                //     }
                //     Console.WriteLine("Could not solve " + i);
                //     // throw new System.Exception("Could not solve " + i);
                // }
            }
        }
    }

    static void TestSpecificMap(string file, int mapNumber)
    {
        RepositoryService repositoryService = new RepositoryService();
        repositoryService.InitializeRepository(file);
        Map map = repositoryService.GetMap(mapNumber);

        // foreach (Piece piece in map.GetSolution().GetPieces())
        // {
        //     Logger.Log(piece.GetIdentifier() + piece.GetPosition().ToString());
        // }

        bool solved = SolutionController.TrySpecificSolution(map.GetTileSet(), map.GetSolution().GetPieces(), out Solution solution);
        Console.WriteLine(solved);
        if (!solved)
        {
            Console.WriteLine(repositoryService.pointers.Length);
            foreach (string line in repositoryService.GetLevelLines(mapNumber))
            {
                Console.WriteLine(line);
            }
            throw new System.Exception(mapNumber + " cannot be solved");
        }
    }

    static void TestAlgorhithmSpeeds()
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();

        TestAlgorhithm(2);

        sw.Stop();
        TimeSpan ts = sw.Elapsed;
        // Format and display the TimeSpan value.
        string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);
        Logger.Log(elapsedTime);

        sw.Restart();

        TestAlgorhithm(1);


        sw.Stop();
        ts = sw.Elapsed;
        // Format and display the TimeSpan value.
        elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
           ts.Hours, ts.Minutes, ts.Seconds,
           ts.Milliseconds / 10);
        Logger.Log(elapsedTime);

    }
}
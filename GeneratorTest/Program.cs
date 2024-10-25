
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using Repositories;
using Zenko;
using Zenko.Controllers;
using Zenko.Entities;
using Zenko.Services;
using Zenko.Utilities;

public class Program
{
    public static void Main()
    {
        // TestAlgorhithmSpeeds();
        // TestAlgorhithm(1);
        // TestSpecificMap("processkiller.txt", 2);
        // Console.WriteLine(GC.GetTotalMemory(true));
        // TestComboMemory("processkiller.txt", 1);
        // Console.WriteLine(GC.GetTotalMemory(true));
        // PieceComboMemory("processkiller.txt", 1, 0);
        // TestComboModesAreEqual("processkiller.txt", 1);
        TestComboService("processkiller.txt", 1);
    }

    static void TestComboService(string file, int mapNumber)
    {
        RepositoryService repositoryService = new RepositoryService();
        repositoryService.InitializeRepository(file);
        Map map = repositoryService.GetMap(mapNumber);

        List<Combo> combos = SolverUtilities.Test(map.GetPieceTypes(), map.GetTileSet(), 4);
        Logger.Log(combos.Count);

        ComboService comboService = new ComboService(map, 4);
        if (!comboService.GetCurrentCombo().Equals(combos[0]))
        {
            Logger.Log("MISMATCH 0");
        }
        int i = 1;
        while (comboService.TryNextRecursive(3, out Combo combo))
        {
            // Logger.Log(combo.GetPositionsString());
            // Logger.Log(combos[i].GetPositionsString());
            if (!combo.Equals(combos[i]))
            {
                Logger.Log(combo.GetPositionsString());
                Logger.Log(combos[i].GetPositionsString());
                Logger.Log("MISMATCH " + i);

                return;
            }

            i++;
        }
        Logger.Log(i);

    }

    static void TestComboModesAreEqual(string file, int mapNumber)
    {
        RepositoryService repositoryService = new RepositoryService();
        repositoryService.InitializeRepository(file);
        Map map = repositoryService.GetMap(mapNumber);

        List<Combo> combos = SolverUtilities.Test(map.GetPieceTypes(), map.GetTileSet(), 4);
        List<Dictionary<V2Int, string>> piecePositionCombos = SolverUtilities.GetPiecePositionCombinations(map.GetPieceTypes(), map.GetTileSet(), 4);

        Logger.Log(combos.Count);
        Logger.Log(piecePositionCombos.Count);
        if (combos.Count != piecePositionCombos.Count)
        {
            throw new System.Exception("Mismatch count of combos");
        }

        int i = 0;
        foreach (Dictionary<V2Int, string> combo in piecePositionCombos)
        {
            if (combos[i].positions.Count != combos[i].pieceTypes.Count)
            {
                Logger.Log("Combo positions and types are mismatched count");
            }
            int j = 0;
            foreach (KeyValuePair<V2Int, string> entry in combo)
            {
                if (combos[i].positions[j] != entry.Key)
                {
                    Logger.Log("Mismatch position" + i + ", " + j);
                }
                if (combos[i].pieceTypes[j] != entry.Value)
                {
                    Logger.Log("Mismatch pieceTypes" + i + ", " + j);
                }
                j++;
            }

            i++;
        }

    }

    static void PieceComboMemory(string file, int mapNumber, int mode)
    {
        RepositoryService repositoryService = new RepositoryService();
        repositoryService.InitializeRepository(file);
        Map map = repositoryService.GetMap(mapNumber);
        switch (mode)
        {
            case 0:
                List<Combo> combos = SolverUtilities.Test(map.GetPieceTypes(), map.GetTileSet(), 4);
                Console.WriteLine(combos.Count);
                Console.WriteLine(GC.GetTotalMemory(false));
                Console.WriteLine(GC.GetTotalAllocatedBytes());

                GC.Collect();
                GC.WaitForPendingFinalizers();

                List<Dictionary<V2Int, string>> piecePositionCombos = SolverUtilities.GetPiecePositionCombinations(map.GetPieceTypes(), map.GetTileSet(), 4);
                Console.WriteLine(piecePositionCombos.Count);
                Console.WriteLine(GC.GetTotalMemory(false));
                Console.WriteLine(GC.GetTotalAllocatedBytes());

                break;
            case 1:

                combos = SolverUtilities.Test(map.GetPieceTypes(), map.GetTileSet(), 4);
                Console.WriteLine(combos.Count);
                Console.WriteLine(GC.GetTotalMemory(false));
                Console.WriteLine(GC.GetTotalAllocatedBytes());
                break;
            case 2:
                piecePositionCombos = SolverUtilities.GetPiecePositionCombinations(map.GetPieceTypes(), map.GetTileSet(), 4);
                Console.WriteLine(piecePositionCombos.Count);
                Console.WriteLine(GC.GetTotalMemory(false));
                Console.WriteLine(GC.GetTotalAllocatedBytes());

                break;
        }
        GC.Collect();
        GC.WaitForPendingFinalizers();
        Console.WriteLine(GC.GetTotalMemory(false));
        Console.WriteLine(GC.GetTotalAllocatedBytes());
    }

    static void TestComboMemory(string file, int mapNumber)
    {
        Process proc = Process.GetCurrentProcess();
        Console.WriteLine(proc.PrivateMemorySize64);
        Console.WriteLine(GC.GetTotalMemory(true));

        RepositoryService repositoryService = new RepositoryService();
        repositoryService.InitializeRepository(file);
        Map map = repositoryService.GetMap(mapNumber);

        List<Dictionary<V2Int, string>> piecePositionCombos = SolverUtilities.GetPiecePositionCombinations(map.GetPieceTypes(), map.GetTileSet(), 4);

        Console.WriteLine(proc.PrivateMemorySize64);
        Console.WriteLine(GC.GetTotalMemory(true));
        Console.WriteLine("Test finished, Press any key to finish");
        Console.ReadLine();
        int i = 0;
        int j = 0;
        foreach (Dictionary<V2Int, string> piecePositionCombo in piecePositionCombos)
        {
            i++;
            foreach (KeyValuePair<V2Int, string> entry in piecePositionCombo)
            {
                j++;
            }
        }
        Console.WriteLine(i);
        Console.WriteLine(j);
        // Console.WriteLine(proc.PrivateMemorySize64);
        Console.WriteLine(GC.GetTotalAllocatedBytes());
        Console.WriteLine(GC.GetTotalMemory(true));
        GC.Collect();
        Console.WriteLine(GC.GetTotalMemory(true));
        Console.WriteLine(GC.GetTotalAllocatedBytes());

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
                // Console.WriteLine(solved);
                if (!solved)
                {
                    Console.WriteLine(repositoryService.pointers.Length);
                    File.AppendAllLines(errorMaps, repositoryService.GetLevelLines(i));
                    File.AppendAllLines(errorMaps, new string[1] { "" });
                    foreach (string line in repositoryService.GetLevelLines(i))
                    {
                        Console.WriteLine(line);
                    }
                    Console.WriteLine("Could not solve " + i);
                    // throw new System.Exception("Could not solve " + i);
                }
            }
        }
    }

    static void TestSpecificMap(string file, int mapNumber)
    {
        RepositoryService repositoryService = new RepositoryService();
        repositoryService.InitializeRepository(file);
        Map map = repositoryService.GetMap(mapNumber);

        bool solved = SolutionController.TrySolveWithPiecesNew(map.GetTileSet(), map.GetPieceTypes(), out Solution solution, 2);







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
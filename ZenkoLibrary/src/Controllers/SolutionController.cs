using System.Collections.Generic;
using Zenko.Services;
using Zenko.Utilities;
using Zenko.Entities;

namespace Zenko.Controllers
{
    public class SolutionController
    {
        const bool DEBUG = false;
        const bool DEBUG_GOOD = false;


        /////////////////////////
        //       PUBLIC        //
        /////////////////////////

        public static bool SolveForSpecificPiecesNew(TileSet tileSet, PieceType[] pieceTypes, out Solution solution)
        {
            SolverService solverService = new SolverService();
            solution = new Solution();

            bool found = false;
            List<Dictionary<V2Int, string>> piecePositionCombos = SolverUtilities.GetPiecePositionCombinations(pieceTypes, tileSet, pieceTypes.Length);
            foreach (Dictionary<V2Int, string> combo in piecePositionCombos)
            {
                TileSet tileSetToTest = tileSet.Clone();
                foreach (KeyValuePair<V2Int, string> piecePosition in combo)
                {
                    TileSetController.PlacePiece(tileSetToTest, new Piece(piecePosition.Key, piecePosition.Value));
                }
                if (solverService.TrySolveBoardGeneral(tileSetToTest, out Solution newSolution))
                {
                    if (solution == null || newSolution.GetTurns() < solution.GetTurns())
                    {
                        solution = newSolution.Clone();
                    }
                    found = true;
                }
            }
            if (found)
            {
                if (DEBUG || DEBUG_GOOD)
                {
                    Logger.Log("MAP SOLVABLE");
                }
                return true;
            }
            Logger.Log("MAP NOT SOLVABLE");
            return false;
        }

        public static bool SolveForSpecificPieces(TileSet tileSet, PieceType[] pieceTypes, out Solution solution)
        {
            solution = new Solution();

            bool found = false;
            List<Dictionary<V2Int, string>> piecePositionCombos = SolverUtilities.GetPiecePositionCombinations(pieceTypes, tileSet, pieceTypes.Length);
            foreach (Dictionary<V2Int, string> combo in piecePositionCombos)
            {
                TileSet tileSetToTest = tileSet.Clone();
                foreach (KeyValuePair<V2Int, string> piecePosition in combo)
                {
                    TileSetController.PlacePiece(tileSetToTest, new Piece(piecePosition.Key, piecePosition.Value));
                }
                if (TrySolveBoard(tileSetToTest, out Solution newSolution))
                {
                    if (solution == null || newSolution.GetTurns() < solution.GetTurns())
                    {
                        solution = newSolution.Clone();
                    }
                    found = true;
                }
            }
            if (found)
            {
                if (DEBUG || DEBUG_GOOD)
                {
                    Logger.Log("MAP SOLVABLE");
                }
                return true;
            }
            Logger.Log("MAP NOT SOLVABLE");
            return false;
        }

        //Handle up to how many pieces?
        public static bool TrySolveWithPiecesNew(TileSet tileSet, PieceType[] pieceTypes, out Solution solution, int method = 0)
        {
            solution = null;
            SolverService solverService = new SolverService();

            for (int i = 0; i <= pieceTypes.Length; i++)
            {
                bool found = false;
                List<Dictionary<V2Int, string>> piecePositionCombos = SolverUtilities.GetPiecePositionCombinations(pieceTypes, tileSet, i);
                foreach (Dictionary<V2Int, string> combo in piecePositionCombos)
                {
                    TileSet tileSetToTest = tileSet.Clone();
                    if (DEBUG || DEBUG_GOOD) Logger.Log("New combo");
                    foreach (KeyValuePair<V2Int, string> piecePosition in combo)
                    {
                        if (DEBUG || DEBUG_GOOD) Logger.Log(piecePosition.Value + " " + piecePosition.Key);
                        TileSetController.PlacePiece(tileSetToTest, new Piece(piecePosition.Key, piecePosition.Value));
                    }
                    // solverService.TrySolveBoardOld(tileSetToTest, out Solution newSolution);
                    // TrySolveBoard(tileSetToTest, out newSolution);
                    if (method == 0)
                    {
                        if (TrySolveBoard(tileSetToTest, out Solution newSolution))
                        {
                            if (solution == null || newSolution.GetTurns() < solution.GetTurns())
                            {
                                solution = newSolution.Clone();
                            }
                            found = true;
                        }

                    }
                    else if (method == 1)
                    {
                        if (solverService.TrySolveBoardGeneral(tileSetToTest, out Solution newSolution))
                        {
                            if (solution == null || newSolution.GetTurns() < solution.GetTurns())
                            {
                                solution = newSolution.Clone();
                            }
                            found = true;
                        }
                    }
                    // else if (method == 2)
                    // {
                    //     if (solverService.TrySolveBoardNew4(tileSetToTest, out Solution newSolution))
                    //     {
                    //         if (solution == null || newSolution.GetTurns() < solution.GetTurns())
                    //         {
                    //             solution = newSolution.Clone();
                    //         }
                    //         found = true;
                    //     }
                    // }
                    // else if (method == 3)
                    // {
                    //     if (solverService.TrySolveBoardNew5(tileSetToTest, out Solution newSolution))
                    //     {
                    //         if (solution == null || newSolution.GetTurns() < solution.GetTurns())
                    //         {
                    //             solution = newSolution.Clone();
                    //         }
                    //         found = true;
                    //     }
                    // }

                }
                if (found)
                {
                    if (DEBUG || DEBUG_GOOD) Logger.LogWarning("FOUND");
                    if (i == pieceTypes.Length)
                    {
                        if (DEBUG || DEBUG_GOOD)
                        {
                            Logger.Log("GOOD MAP: map with " + pieceTypes.Length + " piece solvable with " + pieceTypes.Length + " piece and " + solution.GetTurns() + " turns");
                        }
                        return true;
                    }
                    else
                    {
                        if (DEBUG)
                        {
                            Logger.Log("BAD MAP: map with " + pieceTypes.Length + " solvable with " + i + " piece");
                        }
                        return false;
                    }
                }
            }
            if (DEBUG) Logger.Log("BAD MAP: no solution found for " + pieceTypes.Length + " piece with " + pieceTypes.Length + " piece");

            // if (DEBUG) Logger.Log("No case for " + pieceTypes.Length + " amount of pieces yet");
            return false;
        }

        // //returns false if tileSet can't be solved with said pieces
        // //it will exclusively try to use all pieces
        // tries all combinations up to n=4
        public static bool TrySolveAllCombinationsNew(TileSet tileSet, out List<Solution> solutions)
        {
            solutions = new List<Solution>();
            Solution mostRecentSolution = new Solution();
            List<PieceType> possiblePieces = PieceTypeService.GetAllPieceTypes();
            //Right now trying all but portals.
            possiblePieces = new List<PieceType>() {
                PieceType.Wall,
                PieceType.WallSeed,
                //   PieceType.Left,
                PieceType.Right,
                //    PieceType.Up,
                PieceType.Down,
                //  PieceType.LeftSeed,
                PieceType.RightSeed,
                //    PieceType.UpSeed,
                PieceType.DownSeed };
            List<List<PieceType>> exclusions = new List<List<PieceType>>();
            List<List<PieceType>> filteredCombos = new List<List<PieceType>>();
            //0 pieces

            if (TrySolveWithPiecesNew(tileSet, new PieceType[0], out mostRecentSolution))
            {
                // solved with 0 pieces so stop
                return false;
            }

            //1 piece
            List<PieceType> piecesToLoop = new List<PieceType>(possiblePieces);
            //check if can remove from possiblepieces inside the foreach
            foreach (PieceType piece in piecesToLoop)
            {
                // Logger.Log("trying " + piece);
                if (TrySolveWithPiecesNew(tileSet, new PieceType[1] { piece }, out mostRecentSolution))
                {
                    solutions.Add(mostRecentSolution);
                    exclusions.Add(new List<PieceType>() { piece });
                }
            }

            //2 pieces
            List<List<PieceType>> twoPieceCombos = PieceTypeService.GetAllPieceTypeCombinations(possiblePieces, 2);
            filteredCombos = PieceTypeService.Filter(twoPieceCombos, exclusions);
            foreach (List<PieceType> combo in filteredCombos)
            {
                // Logger.Log("trying " + combo[0] + " " + combo[1]);
                if (TrySolveWithPiecesNew(tileSet, combo.ToArray(), out mostRecentSolution))
                {
                    solutions.Add(mostRecentSolution);
                    exclusions.Add(combo);
                    //TODO: Remove this piece-combo
                }
            }

            //3 pieces
            List<List<PieceType>> threePieceCombos = PieceTypeService.GetAllPieceTypeCombinations(possiblePieces, 3);
            filteredCombos = PieceTypeService.Filter(threePieceCombos, exclusions);
            foreach (List<PieceType> combo in filteredCombos)
            {
                // Logger.Log("trying " + combo[0] + " " + combo[1] + " " + combo[2]);
                if (TrySolveWithPiecesNew(tileSet, combo.ToArray(), out mostRecentSolution))
                {
                    solutions.Add(mostRecentSolution);
                    exclusions.Add(combo);
                    //TODO: Remove this piece-combo
                }
            }

            //4 pieces
            List<List<PieceType>> fourPieceCombos = PieceTypeService.GetAllPieceTypeCombinations(possiblePieces, 4);
            filteredCombos = PieceTypeService.Filter(fourPieceCombos, exclusions);
            foreach (List<PieceType> combo in filteredCombos)
            {
                // Logger.Log("trying " + combo[0] + " " + combo[1] + " " + combo[2] + " " + combo[3]);
                if (TrySolveWithPiecesNew(tileSet, combo.ToArray(), out mostRecentSolution))
                {
                    solutions.Add(mostRecentSolution);
                    exclusions.Add(combo);
                    //TODO: Remove this piece-combo
                }
            }
            if (solutions.Count > 0)
            {
                return true;
            }
            return false;
        }

        //Gets a piece-less tile set and some pieces and tries to solve in the positions of those pieces
        public static bool TrySpecificSolution(TileSet tileSet, Piece[] pieces, out Solution solution)
        {
            solution = new Solution();
            TileSet board = tileSet.Clone();
            foreach (Piece piece in pieces)
            {
                TileSetController.PlacePiece(board, piece);
            }
            SolverService solverService = new SolverService();
            if (solverService.TrySolveBoardGeneral(board, out Solution newSolution))
            {
                if (solution.GetTurns() == 0 || newSolution.GetTurns() < solution.GetTurns())
                {
                    solution = newSolution.Clone();
                    solution.SetPieces(pieces);
                    return true;
                }
                else
                {
                    Logger.LogError("cant solve specific solution");
                    return false;
                }
            }
            Logger.LogError("oops, cant solve specific solution");
            return false;
        }


        /////////////////////////
        //       PRIVATE       //
        /////////////////////////

        static bool TrySolveBoard(TileSet tileSet, out Solution solution)
        {
            int totalStates = 0;
            solution = new Solution();

            List<SolutionBot> activeBots = new List<SolutionBot>();
            List<SolutionBot> exploredBots = new List<SolutionBot>();

            activeBots.Add(new SolutionBot(tileSet));
            while (activeBots.Count > 0)
            {
                List<SolutionBot> newBots = new List<SolutionBot>();
                List<SolutionBot> validBots = new List<SolutionBot>();
                //get all new bots
                foreach (SolutionBot solutionBot in activeBots)
                {
                    newBots.AddRange(SolutionBotController.TryMoveAllDirections(solutionBot));
                    totalStates += 4;
                }
                int i = 0;
                //validate and add new bots
                foreach (SolutionBot solutionBot in newBots)
                {
                    if (i > 10000)
                    {
                        Logger.LogError("toomuch");
                        return false;
                    }
                    if (solutionBot.IsValid() && !solutionBot.IsInList(validBots) && !solutionBot.IsInList(exploredBots) && !solutionBot.IsInList(activeBots)) //&& is valid (check if not on hole, or other losing conditions)
                    {
                        if (solutionBot.IsPlayerOnGoal())
                        {
                            // Logger.Log("found" + totalStates);
                            //Deep clone piece in case tileset changes, we want solution pieces to be FIXED
                            Piece[] solutionPieces = new Piece[tileSet.GetPlacedPieces().Count];
                            for (int pieceIndex = 0; pieceIndex < tileSet.GetPlacedPieces().Count; pieceIndex++)
                            {
                                solutionPieces[pieceIndex] = tileSet.GetPlacedPieces()[pieceIndex].Clone();
                            }
                            solution.SetPieces(solutionPieces);

                            solution.SetTurns(solutionBot.GetTurns());
                            solution.SetMoves(new List<V2Int>(solutionBot.GetMoves()).ToArray());
                            return true;
                        }
                        validBots.Add(solutionBot);
                    }
                    i++;
                }
                //update lists and go back to loop
                exploredBots.AddRange(activeBots);
                activeBots = new List<SolutionBot>(validBots);
            }
            // Logger.Log(totalStates);
            return false;
        }
    }
}
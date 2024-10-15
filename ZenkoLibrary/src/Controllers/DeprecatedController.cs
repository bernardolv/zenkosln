using System.Collections.Generic;
using Zenko.Entities;
using Zenko.Services;
using Zenko.Controllers;
using Zenko.Utilities;
using Zenko;

namespace Zenko.Controllers
{
    public class DeprecatedController
    {
        const bool DEBUG = true;
        const bool DEBUG_GOOD = false;

        // //returns false if tileSet can't be solved with said pieces
        // //it will exclusively try to use all pieces
        // tries all combinations up to n=4
        public static bool TrySolveAllCombinations(TileSet tileSet, out List<Solution> solutions)
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

            if (TrySolveFor0Pieces(tileSet, out mostRecentSolution))
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
                if (TrySolveFor1Piece(tileSet, piece, out mostRecentSolution))
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
                if (TrySolveFor2Pieces(tileSet, combo.ToArray(), out mostRecentSolution))
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
                if (TrySolveFor3Pieces(tileSet, combo.ToArray(), out mostRecentSolution))
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
                if (TrySolveFor4Pieces(tileSet, combo.ToArray(), out mostRecentSolution))
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

        //DEPRECATED
        public static bool TrySolveWithPieces(TileSet tileSet, PieceType[] pieceTypes, out Solution solution)
        {
            solution = new Solution();
            int pieceAmount = pieceTypes.Length;

            if (TrySolveFor0Pieces(tileSet, out solution))
            {
                if (pieceAmount == 0)
                {
                    if (DEBUG || DEBUG_GOOD) Logger.Log("GOOD MAP: map with 0 pieces solvable with 0 pieces and " + solution.GetTurns() + " turns");
                    return true;
                }
                else
                {
                    if (DEBUG) Logger.Log("BAD MAP: map with " + pieceAmount + " solvable with 0 pieces");
                }
                return false;
            }
            if (pieceAmount == 0)
            {
                if (DEBUG) Logger.Log("BAD MAP: no solutions found for 0 pieces with 0 pieces");
                return false;
            }

            //TODO: TRY FOR EACH OF ALL THE PIECES
            foreach (PieceType pieceType in pieceTypes)
            {
                if (TrySolveFor1Piece(tileSet, pieceType, out solution))
                {

                    if (pieceAmount == 1)
                    {
                        if (DEBUG || DEBUG_GOOD) Logger.Log("GOOD MAP: map with 1 piece solvable with 1 piece and " + solution.GetTurns() + " turns");
                        return true;
                    }
                    else
                    {
                        if (DEBUG) Logger.Log("BAD MAP: map with " + pieceAmount + " solvable with 1 piece");
                        return false;
                    }
                }
            }

            if (pieceAmount == 1)
            {
                if (DEBUG) Logger.Log("BAD MAP: no solution found for 1 piece with 1 piece");
                return false;
            }

            //TODO: Implement each 2 piece combo
            //order does not matter so dont send 2 different combinations of the same
            //order is avoided by initially making the right order request
            // wwrr > wrwr (have to manually set for now)
            List<List<PieceType>> pieceTypeCombos = PieceTypeService.GetPossiblePieceCombinations(pieceTypes, 2);
            foreach (List<PieceType> pieceTypeCombo in pieceTypeCombos)
            {
                if (TrySolveFor2Pieces(tileSet, pieceTypeCombo.ToArray(), out solution))
                {
                    if (pieceAmount == 2)
                    {
                        if (DEBUG || DEBUG_GOOD) Logger.Log("GOOD MAP: map with 2 pieces solvable with 2 pieces and " + solution.GetTurns() + " turns");
                        return true;
                    }
                    else
                    {
                        if (DEBUG) Logger.Log("BAD MAP: map with " + pieceAmount + " solvable with 2 pieces");
                        return false;
                    }
                }
            }

            if (pieceAmount == 2)
            {
                if (DEBUG) Logger.Log("BAD MAP: no solution found for 2 piece with 2 piece");
                return false;
            }

            pieceTypeCombos = PieceTypeService.GetPossiblePieceCombinations(pieceTypes, 3);
            foreach (List<PieceType> pieceTypeCombo in pieceTypeCombos)
            {
                if (TrySolveFor3Pieces(tileSet, pieceTypeCombo.ToArray(), out solution))
                {
                    if (pieceAmount == 3)
                    {
                        if (DEBUG || DEBUG_GOOD) Logger.Log("GOOD MAP: map with 3 pieces solvable with 3 pieces and " + solution.GetTurns() + " turns");
                        return true;
                    }
                    else
                    {
                        if (DEBUG) Logger.Log("BAD MAP: map with " + pieceAmount + " solvable with 3 pieces");
                        return false;
                    }
                }
            }
            if (pieceAmount == 3)
            {
                if (DEBUG) Logger.Log("BAD MAP: no solution found for 3 piece with 3 piece");
                return false;
            }

            pieceTypeCombos = PieceTypeService.GetPossiblePieceCombinations(pieceTypes, 4);
            foreach (List<PieceType> pieceTypeCombo in pieceTypeCombos)
            {
                if (TrySolveFor4Pieces(tileSet, pieceTypeCombo.ToArray(), out solution))
                {
                    if (pieceAmount == 4)
                    {
                        if (DEBUG || DEBUG_GOOD) Logger.Log("GOOD MAP: map with 4 pieces solvable with 4 pieces and " + solution.GetTurns() + " turns");
                        return true;
                    }
                    else
                    {
                        if (DEBUG) Logger.Log("BAD MAP: map with " + pieceAmount + " solvable with 4 pieces");
                        return false;
                    }
                }
            }
            if (pieceAmount == 4)
            {
                if (DEBUG) Logger.Log("BAD MAP: no solution found for 4 piece with 4 piece");
                return false;
            }
            if (DEBUG) Logger.Log("No case for " + pieceTypes.Length + " amount of pieces yet");
            return false;
        }


        static bool TrySolveFor0Pieces(TileSet tileSet, out Solution solution)
        {
            return TrySolveBoard(tileSet.Clone(), out solution);
        }

        static bool TrySolveFor1Piece(TileSet tileSet, PieceType pieceType, out Solution solution)
        {
            solution = new Solution();
            List<V2Int> candidatePositions = TileSetUtilities.GetCandidateTilePositions(tileSet);

            foreach (V2Int position in candidatePositions)
            {
                TileSet board = tileSet.Clone();
                PieceType[] currentPieceTypes = new PieceType[] { pieceType };
                V2Int[] currentPositions = new V2Int[] { position };
                TileSetController.PlacePieces(board, currentPieceTypes, currentPositions);
                if (TrySolveBoard(board, out Solution newSolution))
                {
                    if (solution.GetTurns() == 0 || newSolution.GetTurns() < solution.GetTurns())
                    {
                        solution = newSolution.Clone();
                        Piece[] pieces = new Piece[] { new Piece(position.x, position.y, Zenko.Presets.PieceType.mapByType[pieceType.ToString()]) };
                        solution.SetPieces(pieces);
                    }
                }
            }

            if (solution.GetTurns() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        static bool TrySolveFor2Pieces(TileSet tileSet, PieceType[] pieceTypes, out Solution solution)
        {
            solution = new Solution();
            List<V2Int> candidatePositions = TileSetUtilities.GetCandidateTilePositions(tileSet);
            for (int i = 0; i < candidatePositions.Count; i++)
            {
                //if piece is same as previous, start higher up to avoid repeat maps
                int j = 0;
                if (pieceTypes[0] == pieceTypes[1])
                {
                    j = i + 1;
                }

                for (; j < candidatePositions.Count; j++)
                {
                    V2Int positionOne = candidatePositions[i];
                    V2Int positionTwo = candidatePositions[j];

                    //TODO: Turn this into a guard clause
                    if (positionOne != positionTwo)
                    {
                        TileSet board = tileSet.Clone();
                        V2Int[] positions = new V2Int[] { positionOne, positionTwo };
                        TileSetController.PlacePieces(board, pieceTypes, positions);
                        if (TrySolveBoard(board, out Solution newSolution))
                        {
                            if (solution.GetTurns() == 0 || newSolution.GetTurns() < solution.GetTurns())
                            {
                                solution = newSolution.Clone();
                                Piece[] pieces = new Piece[2];
                                pieces[0] = new Piece(positionOne.x, positionOne.y, Zenko.Presets.PieceType.mapByType[pieceTypes[0].ToString()]);
                                pieces[1] = new Piece(positionTwo.x, positionTwo.y, Zenko.Presets.PieceType.mapByType[pieceTypes[1].ToString()]);
                                solution.SetPieces(pieces);
                            }
                        }
                    }
                }
            }
            if (solution.GetTurns() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        static bool TrySolveFor3Pieces(TileSet tileSet, PieceType[] pieceTypes, out Solution solution)
        {
            solution = new Solution();
            List<V2Int> candidatePositions = TileSetUtilities.GetCandidateTilePositions(tileSet);
            for (int i = 0; i < candidatePositions.Count; i++)
            {
                //if piece is same as previous, start higher up to avoid repeat maps
                int j = 0;
                if (pieceTypes[0] == pieceTypes[1])
                {
                    j = i + 1;
                }

                for (; j < candidatePositions.Count; j++)
                {
                    int k = 0;
                    if (pieceTypes[1] == pieceTypes[2])
                    {
                        k = j + 1;
                    }
                    for (; k < candidatePositions.Count; k++)
                    {
                        V2Int positionOne = candidatePositions[i];
                        V2Int positionTwo = candidatePositions[j];
                        V2Int positionThree = candidatePositions[k];

                        if (positionOne != positionTwo && positionOne != positionThree && positionTwo != positionThree)
                        {
                            TileSet board = tileSet.Clone();
                            V2Int[] positions = new V2Int[] { positionOne, positionTwo, positionThree };
                            TileSetController.PlacePieces(board, pieceTypes, positions);
                            if (TrySolveBoard(board, out Solution newSolution))
                            {
                                if (solution.GetTurns() == 0 || newSolution.GetTurns() < solution.GetTurns())
                                {
                                    solution = newSolution.Clone();
                                    Piece[] pieces = new Piece[3];
                                    pieces[0] = new Piece(positionOne.x, positionOne.y, Zenko.Presets.PieceType.mapByType[pieceTypes[0].ToString()]);
                                    pieces[1] = new Piece(positionTwo.x, positionTwo.y, Zenko.Presets.PieceType.mapByType[pieceTypes[1].ToString()]);
                                    pieces[2] = new Piece(positionThree.x, positionThree.y, Zenko.Presets.PieceType.mapByType[pieceTypes[2].ToString()]);
                                    solution.SetPieces(pieces);
                                }
                            }
                        }
                    }
                }
            }
            if (solution.GetTurns() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        static bool TrySolveFor4Pieces(TileSet tileSet, PieceType[] pieceTypes, out Solution solution)
        {
            solution = new Solution();
            List<V2Int> candidatePositions = TileSetUtilities.GetCandidateTilePositions(tileSet);
            for (int i = 0; i < candidatePositions.Count; i++)
            {
                //if piece is same as previous, start higher up to avoid repeat maps
                int j = 0;
                if (pieceTypes[0] == pieceTypes[1])
                {
                    j = i + 1;
                }

                for (; j < candidatePositions.Count; j++)
                {
                    int k = 0;
                    if (pieceTypes[1] == pieceTypes[2])
                    {
                        k = j + 1;
                    }
                    for (; k < candidatePositions.Count; k++)
                    {
                        int l = 0;
                        if (pieceTypes[2] == pieceTypes[3])
                        {
                            l = k + 1;
                        }
                        for (; l < candidatePositions.Count; l++)
                        {
                            V2Int positionOne = candidatePositions[i];
                            V2Int positionTwo = candidatePositions[j];
                            V2Int positionThree = candidatePositions[k];
                            V2Int positionFour = candidatePositions[l];

                            //if no duplicate positions
                            //could make a new array and remove position and see if still in array
                            if (positionOne != positionTwo && positionOne != positionThree && positionOne != positionFour && positionTwo != positionThree && positionTwo != positionFour && positionThree != positionFour)
                            {
                                TileSet board = tileSet.Clone();
                                V2Int[] positions = new V2Int[] { positionOne, positionTwo, positionThree, positionFour };
                                TileSetController.PlacePieces(board, pieceTypes, positions);
                                if (TrySolveBoard(board, out Solution newSolution))
                                {
                                    if (solution.GetTurns() == 0 || newSolution.GetTurns() < solution.GetTurns())
                                    {
                                        solution = newSolution.Clone();
                                        Piece[] pieces = new Piece[4];
                                        pieces[0] = new Piece(positionOne.x, positionOne.y, Zenko.Presets.PieceType.mapByType[pieceTypes[0].ToString()]);
                                        pieces[1] = new Piece(positionTwo.x, positionTwo.y, Zenko.Presets.PieceType.mapByType[pieceTypes[1].ToString()]);
                                        pieces[2] = new Piece(positionThree.x, positionThree.y, Zenko.Presets.PieceType.mapByType[pieceTypes[2].ToString()]);
                                        pieces[3] = new Piece(positionFour.x, positionFour.y, Zenko.Presets.PieceType.mapByType[pieceTypes[3].ToString()]);

                                        solution.SetPieces(pieces);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (solution.GetTurns() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        static bool TrySolveBoard(TileSet tileSet, out Solution solution)
        {
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
            // activeBots.Add(new SolutionBot(map))
            // Logger.Log("Solution not found");
            return false;
        }
    }
}
using System.Collections.Generic;
using Zenko.Entities;
using Zenko.Extensions;
using Zenko;
using Zenko.Utilities;
using Zenko.Controllers;
using System.Linq;
using System;

namespace Zenko.Services
{
    //POSSIBLE VARIATIONS
    //STATEHOLDER tileset vs modified tiles (tileset seems to be best since theres no need to create extra steps...)
    //HASHTABLE vs LIST and iterate/equals vs bigint hashmap
    //Old move (solutionbot.move) vs new move (tilesetutilities.getnextaction && mapcontroller.move)
    //get new states first then iterate over them and do logic vs do logic as it iterates (maybe a dots thing helps? i dont think so)
    //try to get existing new state by looking through the active ones...
    public class SolverService
    {
        const bool DEBUG = false;

        public bool TrySolveBoardGeneral(TileSet tileSet, out Solution solution)
        {
            int totalStates = 0;
            solution = new Solution();

            TileSetStateHolder originalTileSetStateHolder = new TileSetStateHolder(tileSet);
            originalTileSetStateHolder.playerPosition = tileSet.GetPlayerPosition();

            Dictionary<TileSet, Dictionary<V2Int, bool>> exploredGameStates = new Dictionary<TileSet, Dictionary<V2Int, bool>>();

            exploredGameStates.Add(originalTileSetStateHolder.tileSet, new Dictionary<V2Int, bool>() { { originalTileSetStateHolder.playerPosition, true } });

            List<TileSetStateHolder> activeGameStates = new List<TileSetStateHolder>();
            activeGameStates.Add(originalTileSetStateHolder);

            while (activeGameStates.Count > 0)
            {
                // Logger.Log(activeGameStates.Count + " " + exploredGameStates.Count);
                List<TileSetStateHolder> validGameStates = new List<TileSetStateHolder>();

                foreach (TileSetStateHolder currentGameState in activeGameStates)
                {
                    List<V3> directions = new List<V3>() { V3.left, V3.right, V3.forward, V3.back };

                    // Dictionary<V3, V2Int> directionMap = new Dictionary
                    foreach (V3 direction in directions)
                    {
                        //shallow copy, only make a deep copy if state changes
                        TileSet newTileSet = currentGameState.tileSet;
                        newTileSet.SetPlayerPosition(currentGameState.playerPosition);
                        V3 currentDirection = direction;

                        //Only move when movable tile
                        V3 nextPosition = newTileSet.GetPlayerPosition().ToViewCoordinates() + currentDirection;
                        if (nextPosition.x < 0 || nextPosition.x >= tileSet.GetXDimension() || -nextPosition.z < 0 || -nextPosition.z >= tileSet.GetYDimension())
                        {
                            if (DEBUG) Logger.LogWarning("out of bounds, SKIP");
                            continue;
                        }

                        //TODO: Skip bad portals too

                        totalStates++;
                        TileSetStateHolder newGameState = currentGameState.Copy();

                        V2Int startPosition = currentGameState.playerPosition;

                        newGameState.moves.Add(direction.ToModelCoordinates());
                        bool traveling = true;
                        int actionTurns = 0;
                        while (traveling)
                        {
                            actionTurns++;

                            V3 targetPosition = TileSetUtilities.TryGetNextActions(newTileSet, newTileSet.GetPlayerPosition().ToViewCoordinates(), currentDirection, out List<BoardAction> boardActions);
                            newTileSet.SetPlayerPosition(targetPosition.ToModelCoordinates());

                            if (DEBUG)
                            {
                                // Logger.Log(targetPosition.ToString());
                                foreach (BoardAction boardAction in boardActions)
                                {
                                    Logger.Log(boardAction.ToString());
                                }
                            }

                            //Special case for solutionhelper, could maybe be on a subscriber (onFragileStepped)
                            //TODO: Implement for seed as well
                            if (boardActions.Contains(BoardAction.Fragile) || boardActions.Contains(BoardAction.Seed))
                            {

                                if (DEBUG) Logger.Log("Seed on " + targetPosition);
                                newTileSet = newTileSet.Clone();

                                // //Try to get an exiting tileset
                                // newTileSet.modifiedPositions.Add(targetPosition);
                                // newTileSet.modifiedPositions = newTileSet.modifiedPositions.OrderBy(x => x.x).ThenBy(x => x.y).ThenBy(x => x.z).ToList();
                                // foreach (KeyValuePair<TileSet, Dictionary<V2Int, bool>> entry in exploredGameStates)
                                // {
                                //     bool same = true;
                                //     if (entry.Key.modifiedPositions.Count != newTileSet.modifiedPositions.Count)
                                //     {
                                //         continue;
                                //     }
                                //     for (int i = 0; i < newTileSet.modifiedPositions.Count; i++)
                                //     {
                                //         if (entry.Key.modifiedPositions[i].CompareTo(newTileSet.modifiedPositions[i]) != 0)
                                //         {
                                //             continue;
                                //         }
                                //     }
                                //     entry.Key.SetTile(targetPosition.ToModelCoordinates(), newTileSet.GetTile(targetPosition.ToModelCoordinates()));
                                //     newTileSet = entry.Key;
                                //     newTileSet.SetPlayerPosition(targetPosition.ToModelCoordinates());
                                //     Console.WriteLine("HI " + newTileSet.GetTile(targetPosition.ToModelCoordinates()).GetTileType());
                                //     if (targetPosition == new V3(3, 0, -1))
                                //     {
                                //         Console.WriteLine("ON IT " + currentDirection.ToModelCoordinates());
                                //     }
                                //     break;
                                // }
                                newGameState.tileSet = newTileSet;
                            }

                            traveling = MapController.ModelTakeBoardAction(newTileSet, targetPosition, boardActions, ref currentDirection);

                            if (boardActions.Contains(BoardAction.Portal))
                            {
                                if (PieceUtilities.IsPortalPortable(direction.ToModelCoordinates(), tileSet.GetPieceAt(targetPosition.ToModelCoordinates()), tileSet))
                                {
                                    Piece matchingPortal = PieceUtilities.GetMatchingPortal(tileSet, targetPosition.ToModelCoordinates());
                                    newGameState.playerPosition = matchingPortal.GetPosition();
                                    newTileSet.SetPlayerPosition(matchingPortal.GetPosition());
                                }
                            }
                            // Logger.Log(targetPosition + " position post board action");
                            // Logger.Log(currentDirection + " direction post board action");
                            newGameState.playerPosition = targetPosition.ToModelCoordinates();
                            if (actionTurns > 20)
                            {
                                // Logger.LogError("Probably stuck in an infinite loop");
                                newGameState.playerPosition = currentGameState.playerPosition;
                                traveling = false;
                            }
                            if (boardActions.Contains(BoardAction.Fragile) || boardActions.Contains(BoardAction.Seed))
                            {
                                if (DEBUG) Logger.Log("Seed on " + targetPosition);
                                newTileSet.GetTile((int)targetPosition.x, -(int)targetPosition.z).GetTileType();
                                // newTileSet = newTileSet.Clone();
                            }
                        }

                        if (startPosition != newGameState.playerPosition)
                        {
                            if (DEBUG) Logger.Log("Player at " + startPosition.ToString() + " going " + direction.ToModelCoordinates().ToString() + " ended at " + newTileSet.GetPlayerPosition().ToString());
                            if (newTileSet.GetPlayerPosition() != newGameState.playerPosition)
                            {
                                Logger.LogError("PLAYER MISMATCH " + newTileSet.GetPlayerPosition() + " " + newGameState.playerPosition);
                            }
                        }

                        //Maybe check if board didnt change and playerposition didnt change and skip accordingly

                        if (newTileSet.GetTile(newTileSet.GetPlayerPosition()).GetTileType() == "Hole" || newTileSet.GetTile(newTileSet.GetPlayerPosition()).GetTileType() == "Fragile")
                        {
                            if (DEBUG) Logger.Log("Player in hole");
                            continue;
                        }

                        if (exploredGameStates.ContainsKey(newGameState.tileSet) && exploredGameStates[newGameState.tileSet].ContainsKey(newGameState.playerPosition))
                        {
                            if (DEBUG) Logger.Log("Explored state exists");
                            continue;
                        }

                        //Add explored state
                        if (!exploredGameStates.ContainsKey(newGameState.tileSet))
                        {
                            exploredGameStates.Add(newGameState.tileSet, new Dictionary<V2Int, bool>());
                        }
                        exploredGameStates[newGameState.tileSet].Add(newGameState.playerPosition, true);

                        if (newTileSet.GetPlayerPosition() == newTileSet.GetGoalPosition())
                        {
                            // Logger.Log("found" + totalStates);
                            // Logger.LogWarning("solution found");
                            //Deep clone piece in case tileset changes, we want solution pieces to be FIXED
                            Piece[] solutionPieces = new Piece[tileSet.GetPlacedPieces().Count];
                            for (int pieceIndex = 0; pieceIndex < tileSet.GetPlacedPieces().Count; pieceIndex++)
                            {
                                solutionPieces[pieceIndex] = tileSet.GetPlacedPieces()[pieceIndex].Clone();
                            }
                            solution.SetPieces(solutionPieces);
                            solution.SetMoves(new List<V2Int>(newGameState.moves).ToArray());
                            solution.SetTurns(newGameState.moves.Count);
                            return true;
                        }
                        validGameStates.Add(newGameState);
                    }
                }
                activeGameStates = new List<TileSetStateHolder>(validGameStates);
            }
            // Logger.Log(totalStates);
            return false;
        }
    }
}

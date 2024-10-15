using System.Collections.Generic;
using Zenko.Entities;
using Zenko.Extensions;
using Zenko;
using Zenko.Utilities;
using Zenko.Controllers;

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

            //Key = PlayerPos, Value = Dictionary<modifiedTiles, moves>
            // Dictionary<TileSet, Dictionary<V2Int, bool>> exploredStates = new Dictionary<TileSet, Dictionary<V2Int, bool>>();
            exploredGameStates.Add(originalTileSetStateHolder.tileSet, new Dictionary<V2Int, bool>() { { originalTileSetStateHolder.playerPosition, true } });

            List<TileSetStateHolder> activeGameStates = new List<TileSetStateHolder>();
            activeGameStates.Add(originalTileSetStateHolder);

            while (activeGameStates.Count > 0)
            {
                if (DEBUG) Logger.Log(activeGameStates.Count);
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
                                Logger.LogError("Probably stuck in an infinite loop");
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
                            if (DEBUG) Logger.Log("Player at " + startPosition + " going " + direction.ToModelCoordinates() + " ended at " + newTileSet.GetPlayerPosition());
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

        // //like new4 but use equal checking instead of hash
        // public bool TrySolveBoardNew6(TileSet tileSet, out Solution solution)
        // {
        //     int totalStates = 0;
        //     solution = new Solution();

        //     TileSetStateHolder originalTileSetStateHolder = new TileSetStateHolder(tileSet);
        //     originalTileSetStateHolder.playerPosition = tileSet.GetPlayerPosition();

        //     Dictionary<V2Int, Dictionary<TileSet, bool>> exploredGameStates = new Dictionary<V2Int, Dictionary<TileSet, bool>>();

        //     //Key = PlayerPos, Value = Dictionary<modifiedTiles, moves>
        //     // Dictionary<V2Int, Dictionary<List<V2Int>, bool>> exploredStates = new Dictionary<V2Int, Dictionary<List<V2Int>, bool>>();
        //     // exploredStates.Add(originalTileSetStateHolder.playerPosition, new Dictionary<List<V2Int>, bool>() { { originalTileSetStateHolder.modifiedTiles, true } });

        //     List<TileSetStateHolder> activeGameStates = new List<TileSetStateHolder>();
        //     activeGameStates.Add(originalTileSetStateHolder);

        //     while (activeGameStates.Count > 0)
        //     {
        //         // Logger.Log(activeHashes.Count);
        //         List<TileSetStateHolder> validGameStates = new List<TileSetStateHolder>();

        //         foreach (TileSetStateHolder currentGameState in activeGameStates)
        //         {
        //             List<V3> directions = new List<V3>() { V3.left, V3.right, V3.forward, V3.back };

        //             // Dictionary<V3, V2Int> directionMap = new Dictionary
        //             foreach (V3 direction in directions)
        //             {
        //                 //shallow copy, only make a deep copy if state changes
        //                 TileSet newTileSet = currentGameState.tileSet;
        //                 newTileSet.SetPlayerPosition(currentGameState.playerPosition);
        //                 V3 currentDirection = direction;

        //                 //Only move when movable tile
        //                 V3 nextPosition = newTileSet.GetPlayerPosition().ToViewCoordinates() + currentDirection;
        //                 if (nextPosition.x < 0 || nextPosition.x >= tileSet.GetXDimension() || -nextPosition.z < 0 || -nextPosition.z >= tileSet.GetYDimension())
        //                 {
        //                     // Logger.LogWarning("out of bounds, SKIP");
        //                     continue;
        //                 }

        //                 //TODO: Skip bad portals too

        //                 totalStates++;
        //                 TileSetStateHolder newGameState = currentGameState.Copy();
        //                 // Logger.Log("Player at " + newSolutionHelper.playerPosition + " going " + direction.ToModelCoordinates());

        //                 newGameState.moves.Add(direction.ToModelCoordinates());
        //                 bool traveling = true;
        //                 int actionTurns = 0;
        //                 while (traveling)
        //                 {
        //                     actionTurns++;

        //                     V3 targetPosition = TileSetUtilities.TryGetNextActions(newTileSet, newTileSet.GetPlayerPosition().ToViewCoordinates(), currentDirection, out List<BoardAction> boardActions);
        //                     newTileSet.SetPlayerPosition(targetPosition.ToModelCoordinates());

        //                     //Special case for solutionhelper, could maybe be on a subscriber (onFragileStepped)
        //                     //TODO: Implement for seed as well
        //                     if (boardActions.Contains(BoardAction.Fragile) || boardActions.Contains(BoardAction.Seed))
        //                     {
        //                         newTileSet = newTileSet.Clone();
        //                     }

        //                     traveling = MapController.ModelTakeBoardAction(newTileSet, targetPosition, boardActions, ref currentDirection);
        //                     newGameState.playerPosition = targetPosition.ToModelCoordinates();
        //                     if (actionTurns > 20)
        //                     {
        //                         Logger.LogError("Probably stuck in an infinite loop");
        //                         newGameState.playerPosition = currentGameState.playerPosition;
        //                         traveling = false;
        //                     }
        //                 }

        //                 //Maybe check if board didnt change and playerposition didnt change and skip accordingly

        //                 if (newTileSet.GetTile(newTileSet.GetPlayerPosition()).GetTileType() == "Hole" || newTileSet.GetTile(newTileSet.GetPlayerPosition()).GetTileType() == "Fragile")
        //                 {
        //                     // Logger.Log("Player in hole");
        //                     continue;
        //                 }

        //                 if (exploredGameStates.ContainsKey(newGameState.playerPosition) && exploredGameStates[newGameState.playerPosition].ContainsKey(newGameState.tileSet))
        //                 {
        //                     continue;
        //                 }

        //                 //Add explored state
        //                 if (!exploredGameStates.ContainsKey(newGameState.playerPosition))
        //                 {
        //                     exploredGameStates.Add(newGameState.playerPosition, new Dictionary<TileSet, bool>());
        //                 }
        //                 exploredGameStates[newGameState.playerPosition].Add(newGameState.tileSet, true);

        //                 if (newTileSet.GetPlayerPosition() == newTileSet.GetGoalPosition())
        //                 {
        //                     // Logger.Log("found" + totalStates);
        //                     // Logger.LogWarning("solution found");
        //                     //Deep clone piece in case tileset changes, we want solution pieces to be FIXED
        //                     Piece[] solutionPieces = new Piece[tileSet.GetPlacedPieces().Count];
        //                     for (int pieceIndex = 0; pieceIndex < tileSet.GetPlacedPieces().Count; pieceIndex++)
        //                     {
        //                         solutionPieces[pieceIndex] = tileSet.GetPlacedPieces()[pieceIndex].Clone();
        //                     }
        //                     solution.SetPieces(solutionPieces);
        //                     solution.SetMoves(new List<V2Int>(newGameState.moves).ToArray());
        //                     solution.SetTurns(newGameState.moves.Count);
        //                     return true;
        //                 }
        //                 validGameStates.Add(newGameState);
        //             }
        //         }
        //         activeGameStates = new List<TileSetStateHolder>(validGameStates);
        //     }
        //     // Logger.Log(totalStates);
        //     return false;
        // }
        // //like new4 but inversed order of exploredgamestates dictionary
        // public bool TrySolveBoardNew5(TileSet tileSet, out Solution solution)
        // {
        //     int totalStates = 0;
        //     solution = new Solution();

        //     TileSetStateHolder originalTileSetStateHolder = new TileSetStateHolder(tileSet);
        //     originalTileSetStateHolder.playerPosition = tileSet.GetPlayerPosition();

        //     Dictionary<V2Int, Dictionary<TileSet, bool>> exploredGameStates = new Dictionary<V2Int, Dictionary<TileSet, bool>>();

        //     //Key = PlayerPos, Value = Dictionary<modifiedTiles, moves>
        //     // Dictionary<V2Int, Dictionary<TileSet, bool>> exploredStates = new Dictionary<V2Int, Dictionary<TileSet, bool>>();
        //     exploredGameStates.Add(originalTileSetStateHolder.playerPosition, new Dictionary<TileSet, bool>() { { originalTileSetStateHolder.tileSet, true } });

        //     List<TileSetStateHolder> activeGameStates = new List<TileSetStateHolder>();
        //     activeGameStates.Add(originalTileSetStateHolder);

        //     while (activeGameStates.Count > 0)
        //     {
        //         if (DEBUG) Logger.Log(activeGameStates.Count);
        //         List<TileSetStateHolder> validGameStates = new List<TileSetStateHolder>();

        //         foreach (TileSetStateHolder currentGameState in activeGameStates)
        //         {
        //             List<V3> directions = new List<V3>() { V3.left, V3.right, V3.forward, V3.back };

        //             // Dictionary<V3, V2Int> directionMap = new Dictionary
        //             foreach (V3 direction in directions)
        //             {
        //                 //shallow copy, only make a deep copy if state changes
        //                 TileSet newTileSet = currentGameState.tileSet;
        //                 newTileSet.SetPlayerPosition(currentGameState.playerPosition);
        //                 V3 currentDirection = direction;

        //                 //Only move when movable tile
        //                 V3 nextPosition = newTileSet.GetPlayerPosition().ToViewCoordinates() + currentDirection;
        //                 if (nextPosition.x < 0 || nextPosition.x >= tileSet.GetXDimension() || -nextPosition.z < 0 || -nextPosition.z >= tileSet.GetYDimension())
        //                 {
        //                     // Logger.LogWarning("out of bounds, SKIP");
        //                     continue;
        //                 }

        //                 //TODO: Skip bad portals too

        //                 totalStates++;
        //                 TileSetStateHolder newGameState = currentGameState.Copy();

        //                 V2Int startPosition = currentGameState.playerPosition;

        //                 newGameState.moves.Add(direction.ToModelCoordinates());
        //                 bool traveling = true;
        //                 int actionTurns = 0;
        //                 while (traveling)
        //                 {
        //                     actionTurns++;

        //                     V3 targetPosition = TileSetUtilities.TryGetNextActions(newTileSet, newTileSet.GetPlayerPosition().ToViewCoordinates(), currentDirection, out List<BoardAction> boardActions);
        //                     newTileSet.SetPlayerPosition(targetPosition.ToModelCoordinates());

        //                     //Special case for solutionhelper, could maybe be on a subscriber (onFragileStepped)
        //                     //TODO: Implement for seed as well
        //                     if (boardActions.Contains(BoardAction.Fragile) || boardActions.Contains(BoardAction.Seed))
        //                     {
        //                         if (DEBUG) Logger.Log("Seed on " + targetPosition);
        //                         newTileSet = newTileSet.Clone();
        //                         newGameState.tileSet = newTileSet;
        //                     }

        //                     traveling = MapController.ModelTakeBoardAction(newTileSet, targetPosition, boardActions, ref currentDirection);
        //                     newGameState.playerPosition = targetPosition.ToModelCoordinates();
        //                     if (actionTurns > 20)
        //                     {
        //                         Logger.LogError("Probably stuck in an infinite loop");
        //                         newGameState.playerPosition = currentGameState.playerPosition;
        //                         traveling = false;
        //                     }
        //                     if (boardActions.Contains(BoardAction.Fragile) || boardActions.Contains(BoardAction.Seed))
        //                     {
        //                         if (DEBUG) Logger.Log("Seed on " + targetPosition);
        //                         newTileSet.GetTile((int)targetPosition.x, -(int)targetPosition.z).GetTileType();
        //                         // newTileSet = newTileSet.Clone();
        //                     }
        //                 }

        //                 if (startPosition != newGameState.playerPosition)
        //                 {
        //                     if (DEBUG) Logger.Log("Player at " + startPosition + " going " + direction.ToModelCoordinates() + " ended at " + newTileSet.GetPlayerPosition());
        //                     if (newTileSet.GetPlayerPosition() != newGameState.playerPosition)
        //                     {
        //                         Logger.LogError("PLAYER MISMATCH " + newTileSet.GetPlayerPosition() + " " + newGameState.playerPosition);
        //                     }
        //                 }

        //                 //Maybe check if board didnt change and playerposition didnt change and skip accordingly

        //                 if (newTileSet.GetTile(newTileSet.GetPlayerPosition()).GetTileType() == "Hole" || newTileSet.GetTile(newTileSet.GetPlayerPosition()).GetTileType() == "Fragile")
        //                 {
        //                     // Logger.Log("Player in hole");
        //                     continue;
        //                 }

        //                 if (exploredGameStates.ContainsKey(newGameState.playerPosition) && exploredGameStates[newGameState.playerPosition].ContainsKey(newGameState.tileSet))
        //                 {
        //                     continue;
        //                 }

        //                 //Add explored state
        //                 if (!exploredGameStates.ContainsKey(newGameState.playerPosition))
        //                 {
        //                     exploredGameStates.Add(newGameState.playerPosition, new Dictionary<TileSet, bool>());
        //                 }
        //                 exploredGameStates[newGameState.playerPosition].Add(newGameState.tileSet, true);

        //                 if (newTileSet.GetPlayerPosition() == newTileSet.GetGoalPosition())
        //                 {
        //                     // Logger.Log("found" + totalStates);
        //                     // Logger.LogWarning("solution found");
        //                     //Deep clone piece in case tileset changes, we want solution pieces to be FIXED
        //                     Piece[] solutionPieces = new Piece[tileSet.GetPlacedPieces().Count];
        //                     for (int pieceIndex = 0; pieceIndex < tileSet.GetPlacedPieces().Count; pieceIndex++)
        //                     {
        //                         solutionPieces[pieceIndex] = tileSet.GetPlacedPieces()[pieceIndex].Clone();
        //                     }
        //                     solution.SetPieces(solutionPieces);
        //                     solution.SetMoves(new List<V2Int>(newGameState.moves).ToArray());
        //                     solution.SetTurns(newGameState.moves.Count);
        //                     return true;
        //                 }
        //                 validGameStates.Add(newGameState);
        //             }
        //         }
        //         activeGameStates = new List<TileSetStateHolder>(validGameStates);
        //     }
        //     // Logger.Log(totalStates);
        //     return false;
        // }
        // public bool TrySolveBoardNew4(TileSet tileSet, out Solution solution)
        // {
        //     int totalStates = 0;
        //     solution = new Solution();

        //     TileSetStateHolder originalTileSetStateHolder = new TileSetStateHolder(tileSet);
        //     originalTileSetStateHolder.playerPosition = tileSet.GetPlayerPosition();

        //     Dictionary<TileSet, Dictionary<V2Int, bool>> exploredGameStates = new Dictionary<TileSet, Dictionary<V2Int, bool>>();

        //     //Key = PlayerPos, Value = Dictionary<modifiedTiles, moves>
        //     // Dictionary<TileSet, Dictionary<V2Int, bool>> exploredStates = new Dictionary<TileSet, Dictionary<V2Int, bool>>();
        //     exploredGameStates.Add(originalTileSetStateHolder.tileSet, new Dictionary<V2Int, bool>() { { originalTileSetStateHolder.playerPosition, true } });

        //     List<TileSetStateHolder> activeGameStates = new List<TileSetStateHolder>();
        //     activeGameStates.Add(originalTileSetStateHolder);

        //     while (activeGameStates.Count > 0)
        //     {
        //         if (DEBUG) Logger.Log(activeGameStates.Count);
        //         List<TileSetStateHolder> validGameStates = new List<TileSetStateHolder>();

        //         foreach (TileSetStateHolder currentGameState in activeGameStates)
        //         {
        //             List<V3> directions = new List<V3>() { V3.left, V3.right, V3.forward, V3.back };

        //             // Dictionary<V3, V2Int> directionMap = new Dictionary
        //             foreach (V3 direction in directions)
        //             {
        //                 //shallow copy, only make a deep copy if state changes
        //                 TileSet newTileSet = currentGameState.tileSet;
        //                 newTileSet.SetPlayerPosition(currentGameState.playerPosition);
        //                 V3 currentDirection = direction;

        //                 //Only move when movable tile
        //                 V3 nextPosition = newTileSet.GetPlayerPosition().ToViewCoordinates() + currentDirection;
        //                 if (nextPosition.x < 0 || nextPosition.x >= tileSet.GetXDimension() || -nextPosition.z < 0 || -nextPosition.z >= tileSet.GetYDimension())
        //                 {
        //                     // Logger.LogWarning("out of bounds, SKIP");
        //                     continue;
        //                 }

        //                 //TODO: Skip bad portals too

        //                 totalStates++;
        //                 TileSetStateHolder newGameState = currentGameState.Copy();

        //                 V2Int startPosition = currentGameState.playerPosition;

        //                 newGameState.moves.Add(direction.ToModelCoordinates());
        //                 bool traveling = true;
        //                 int actionTurns = 0;
        //                 while (traveling)
        //                 {
        //                     actionTurns++;

        //                     V3 targetPosition = TileSetUtilities.TryGetNextActions(newTileSet, newTileSet.GetPlayerPosition().ToViewCoordinates(), currentDirection, out List<BoardAction> boardActions);
        //                     newTileSet.SetPlayerPosition(targetPosition.ToModelCoordinates());

        //                     //Special case for solutionhelper, could maybe be on a subscriber (onFragileStepped)
        //                     //TODO: Implement for seed as well
        //                     if (boardActions.Contains(BoardAction.Fragile) || boardActions.Contains(BoardAction.Seed))
        //                     {
        //                         if (DEBUG) Logger.Log("Seed on " + targetPosition);
        //                         newTileSet = newTileSet.Clone();
        //                         newGameState.tileSet = newTileSet;
        //                     }

        //                     traveling = MapController.ModelTakeBoardAction(newTileSet, targetPosition, boardActions, ref currentDirection);
        //                     newGameState.playerPosition = targetPosition.ToModelCoordinates();
        //                     if (actionTurns > 20)
        //                     {
        //                         Logger.LogError("Probably stuck in an infinite loop");
        //                         newGameState.playerPosition = currentGameState.playerPosition;
        //                         traveling = false;
        //                     }
        //                     if (boardActions.Contains(BoardAction.Fragile) || boardActions.Contains(BoardAction.Seed))
        //                     {
        //                         if (DEBUG) Logger.Log("Seed on " + targetPosition);
        //                         newTileSet.GetTile((int)targetPosition.x, -(int)targetPosition.z).GetTileType();
        //                         // newTileSet = newTileSet.Clone();
        //                     }
        //                 }

        //                 if (startPosition != newGameState.playerPosition)
        //                 {
        //                     if (DEBUG) Logger.Log("Player at " + startPosition + " going " + direction.ToModelCoordinates() + " ended at " + newTileSet.GetPlayerPosition());
        //                     if (newTileSet.GetPlayerPosition() != newGameState.playerPosition)
        //                     {
        //                         Logger.LogError("PLAYER MISMATCH " + newTileSet.GetPlayerPosition() + " " + newGameState.playerPosition);
        //                     }
        //                 }

        //                 //Maybe check if board didnt change and playerposition didnt change and skip accordingly

        //                 if (newTileSet.GetTile(newTileSet.GetPlayerPosition()).GetTileType() == "Hole" || newTileSet.GetTile(newTileSet.GetPlayerPosition()).GetTileType() == "Fragile")
        //                 {
        //                     // Logger.Log("Player in hole");
        //                     continue;
        //                 }

        //                 if (exploredGameStates.ContainsKey(newGameState.tileSet) && exploredGameStates[newGameState.tileSet].ContainsKey(newGameState.playerPosition))
        //                 {
        //                     continue;
        //                 }

        //                 //Add explored state
        //                 if (!exploredGameStates.ContainsKey(newGameState.tileSet))
        //                 {
        //                     exploredGameStates.Add(newGameState.tileSet, new Dictionary<V2Int, bool>());
        //                 }
        //                 exploredGameStates[newGameState.tileSet].Add(newGameState.playerPosition, true);

        //                 if (newTileSet.GetPlayerPosition() == newTileSet.GetGoalPosition())
        //                 {
        //                     // Logger.Log("found" + totalStates);
        //                     // Logger.LogWarning("solution found");
        //                     //Deep clone piece in case tileset changes, we want solution pieces to be FIXED
        //                     Piece[] solutionPieces = new Piece[tileSet.GetPlacedPieces().Count];
        //                     for (int pieceIndex = 0; pieceIndex < tileSet.GetPlacedPieces().Count; pieceIndex++)
        //                     {
        //                         solutionPieces[pieceIndex] = tileSet.GetPlacedPieces()[pieceIndex].Clone();
        //                     }
        //                     solution.SetPieces(solutionPieces);
        //                     solution.SetMoves(new List<V2Int>(newGameState.moves).ToArray());
        //                     solution.SetTurns(newGameState.moves.Count);
        //                     return true;
        //                 }
        //                 validGameStates.Add(newGameState);
        //             }
        //         }
        //         activeGameStates = new List<TileSetStateHolder>(validGameStates);
        //     }
        //     // Logger.Log(totalStates);
        //     return false;
        // }

        // public bool TrySolveBoardNew3(TileSet tileSet, out Solution solution)
        // {
        //     int totalStates = 0;
        //     solution = new Solution();

        //     TileSetStateHolder originalTileSetStateHolder = new TileSetStateHolder(tileSet);
        //     originalTileSetStateHolder.playerPosition = tileSet.GetPlayerPosition();

        //     List<TileSetStateHolder> exploredGameStates = new List<TileSetStateHolder>();

        //     //Key = PlayerPos, Value = Dictionary<modifiedTiles, moves>
        //     // Dictionary<V2Int, Dictionary<List<V2Int>, bool>> exploredStates = new Dictionary<V2Int, Dictionary<List<V2Int>, bool>>();
        //     // exploredStates.Add(originalTileSetStateHolder.playerPosition, new Dictionary<List<V2Int>, bool>() { { originalTileSetStateHolder.modifiedTiles, true } });

        //     List<TileSetStateHolder> activeGameStates = new List<TileSetStateHolder>();
        //     activeGameStates.Add(originalTileSetStateHolder);

        //     while (activeGameStates.Count > 0)
        //     {
        //         // Logger.Log(activeHashes.Count);
        //         List<TileSetStateHolder> validGameStates = new List<TileSetStateHolder>();

        //         foreach (TileSetStateHolder currentGameState in activeGameStates)
        //         {
        //             List<V3> directions = new List<V3>() { V3.left, V3.right, V3.forward, V3.back };

        //             // Dictionary<V3, V2Int> directionMap = new Dictionary
        //             foreach (V3 direction in directions)
        //             {
        //                 //shallow copy, only make a deep copy if state changes
        //                 TileSet newTileSet = currentGameState.tileSet;
        //                 newTileSet.SetPlayerPosition(currentGameState.playerPosition);
        //                 V3 currentDirection = direction;

        //                 //Only move when movable tile
        //                 V3 nextPosition = newTileSet.GetPlayerPosition().ToViewCoordinates() + currentDirection;
        //                 if (nextPosition.x < 0 || nextPosition.x >= tileSet.GetXDimension() || -nextPosition.z < 0 || -nextPosition.z >= tileSet.GetYDimension())
        //                 {
        //                     // Logger.LogWarning("out of bounds, SKIP");
        //                     continue;
        //                 }

        //                 //TODO: Skip bad portals too

        //                 totalStates++;
        //                 TileSetStateHolder newGameState = currentGameState.Copy();
        //                 // Logger.Log("Player at " + newSolutionHelper.playerPosition + " going " + direction.ToModelCoordinates());

        //                 newGameState.moves.Add(direction.ToModelCoordinates());
        //                 bool traveling = true;
        //                 int actionTurns = 0;
        //                 while (traveling)
        //                 {
        //                     actionTurns++;

        //                     V3 targetPosition = TileSetUtilities.TryGetNextActions(newTileSet, newTileSet.GetPlayerPosition().ToViewCoordinates(), currentDirection, out List<BoardAction> boardActions);
        //                     newTileSet.SetPlayerPosition(targetPosition.ToModelCoordinates());

        //                     //Special case for solutionhelper, could maybe be on a subscriber (onFragileStepped)
        //                     //TODO: Implement for seed as well
        //                     if (boardActions.Contains(BoardAction.Fragile) || boardActions.Contains(BoardAction.Seed))
        //                     {
        //                         newTileSet = newTileSet.Clone();
        //                     }

        //                     traveling = MapController.ModelTakeBoardAction(newTileSet, targetPosition, boardActions, ref currentDirection);
        //                     newGameState.playerPosition = targetPosition.ToModelCoordinates();
        //                     if (actionTurns > 20)
        //                     {
        //                         Logger.LogError("Probably stuck in an infinite loop");
        //                         newGameState.playerPosition = currentGameState.playerPosition;
        //                         traveling = false;
        //                     }
        //                 }

        //                 //Maybe check if board didnt change and playerposition didnt change and skip accordingly

        //                 if (newTileSet.GetTile(newTileSet.GetPlayerPosition()).GetTileType() == "Hole" || newTileSet.GetTile(newTileSet.GetPlayerPosition()).GetTileType() == "Fragile")
        //                 {
        //                     // Logger.Log("Player in hole");
        //                     continue;
        //                 }

        //                 bool explored = false;
        //                 foreach (TileSetStateHolder exploredState in exploredGameStates)
        //                 {
        //                     if (newGameState.tileSet == exploredState.tileSet && newGameState.playerPosition == exploredState.playerPosition)
        //                     {
        //                         explored = true;
        //                         break;
        //                     }
        //                 }

        //                 if (explored)
        //                 {
        //                     continue;
        //                 }

        //                 exploredGameStates.Add(newGameState);

        //                 // if (exploredStates.ContainsKey(newGameState.playerPosition) && exploredStates[newGameState.playerPosition].ContainsKey(newGameState.modifiedTiles))
        //                 // {
        //                 //     continue;
        //                 // }

        //                 // //Add explored state
        //                 // if (!exploredStates.ContainsKey(newGameState.playerPosition))
        //                 // {
        //                 //     exploredStates.Add(newGameState.playerPosition, new Dictionary<List<V2Int>, bool>());
        //                 // }
        //                 // exploredStates[newGameState.playerPosition].Add(newGameState.modifiedTiles, true);

        //                 if (newTileSet.GetPlayerPosition() == newTileSet.GetGoalPosition())
        //                 {
        //                     // Logger.Log("found" + totalStates);
        //                     // Logger.LogWarning("solution found");
        //                     //Deep clone piece in case tileset changes, we want solution pieces to be FIXED
        //                     Piece[] solutionPieces = new Piece[tileSet.GetPlacedPieces().Count];
        //                     for (int pieceIndex = 0; pieceIndex < tileSet.GetPlacedPieces().Count; pieceIndex++)
        //                     {
        //                         solutionPieces[pieceIndex] = tileSet.GetPlacedPieces()[pieceIndex].Clone();
        //                     }
        //                     solution.SetPieces(solutionPieces);
        //                     solution.SetMoves(new List<V2Int>(newGameState.moves).ToArray());
        //                     solution.SetTurns(newGameState.moves.Count);
        //                     return true;
        //                 }
        //                 validGameStates.Add(newGameState);
        //             }
        //         }
        //         activeGameStates = new List<TileSetStateHolder>(validGameStates);
        //     }
        //     // Logger.Log(totalStates);
        //     return false;
        // }
        // public bool TrySolveBoardNew2(TileSet tileSet, out Solution solution)
        // {
        //     int totalStates = 0;
        //     solution = new Solution();

        //     GameStateHolder originalGameStateHolder = new GameStateHolder();
        //     originalGameStateHolder.playerPosition = tileSet.GetPlayerPosition();

        //     List<GameStateHolder> exploredGameStates = new List<GameStateHolder>();

        //     //Key = PlayerPos, Value = Dictionary<modifiedTiles, moves>
        //     // Dictionary<V2Int, Dictionary<List<V2Int>, bool>> exploredStates = new Dictionary<V2Int, Dictionary<List<V2Int>, bool>>();
        //     // exploredStates.Add(originalGameStateHolder.playerPosition, new Dictionary<List<V2Int>, bool>() { { originalGameStateHolder.modifiedTiles, true } });

        //     List<GameStateHolder> activeGameStates = new List<GameStateHolder>();
        //     activeGameStates.Add(originalGameStateHolder);

        //     while (activeGameStates.Count > 0)
        //     {
        //         // Logger.Log(activeHashes.Count);
        //         List<GameStateHolder> validGameStates = new List<GameStateHolder>();

        //         foreach (GameStateHolder currentGameState in activeGameStates)
        //         {
        //             TileSet currentTileSet = tileSet.Clone();

        //             //Modify currentTileSet according to solution helper
        //             foreach (V2Int pos in currentGameState.modifiedTiles)
        //             {
        //                 if (currentTileSet.GetTile(pos).GetTileType() == "Seed")
        //                 {
        //                     currentTileSet.GetTile(pos).SetTileType("Wall");
        //                     switch (currentTileSet.GetTile(pos).GetSeedType())
        //                     {
        //                         case "Left":
        //                         case "Right":
        //                         case "Up":
        //                         case "Down":
        //                             Tile affectedTile = currentTileSet.GetTile(pos + DataTransformer.DirectionStringToV2Int(currentTileSet.GetTile(pos).GetSeedType()));
        //                             affectedTile.SetSideways(currentTileSet.GetTile(pos).GetSeedType());
        //                             break;
        //                     }
        //                 }
        //                 if (currentTileSet.GetTile(pos).GetTileType() == "Fragile")
        //                 {
        //                     currentTileSet.GetTile(pos).SetTileType("Hole");
        //                 }
        //             }

        //             List<V3> directions = new List<V3>() { V3.left, V3.right, V3.forward, V3.back };
        //             // Dictionary<V3, V2Int> directionMap = new Dictionary
        //             foreach (V3 direction in directions)
        //             {
        //                 //shallow copy, only make a deep copy if state changes
        //                 TileSet newTileSet = currentTileSet;
        //                 newTileSet.SetPlayerPosition(currentGameState.playerPosition);
        //                 V3 currentDirection = direction;

        //                 //Only move when movable tile
        //                 V3 nextPosition = newTileSet.GetPlayerPosition().ToViewCoordinates() + currentDirection;
        //                 if (nextPosition.x < 0 || nextPosition.x >= tileSet.GetXDimension() || -nextPosition.z < 0 || -nextPosition.z >= tileSet.GetYDimension())
        //                 {
        //                     // Logger.LogWarning("out of bounds, SKIP");
        //                     continue;
        //                 }


        //                 if (newTileSet.GetTile(nextPosition.ToModelCoordinates()).GetTileType() == "Wall")
        //                 {
        //                     // Logger.LogWarning("Walking into wall, skip");
        //                     continue;
        //                 }

        //                 if (newTileSet.GetTile(nextPosition.ToModelCoordinates()).GetTileType() == "Hole")
        //                 {
        //                     // Logger.LogWarning("Walking into wall, skip");
        //                     continue;
        //                 }

        //                 //TODO: Skip bad portals too

        //                 totalStates++;
        //                 GameStateHolder newGameState = currentGameState.Copy();
        //                 // Logger.Log("Player at " + newSolutionHelper.playerPosition + " going " + direction.ToModelCoordinates());

        //                 newGameState.moves.Add(direction.ToModelCoordinates());
        //                 bool traveling = true;
        //                 int actionTurns = 0;
        //                 while (traveling)
        //                 {
        //                     actionTurns++;

        //                     V3 targetPosition = TileSetUtilities.TryGetNextActions(newTileSet, newTileSet.GetPlayerPosition().ToViewCoordinates(), currentDirection, out List<BoardAction> boardActions);
        //                     newTileSet.SetPlayerPosition(targetPosition.ToModelCoordinates());

        //                     //Special case for solutionhelper, could maybe be on a subscriber (onFragileStepped)
        //                     //TODO: Implement for seed as well
        //                     if (boardActions.Contains(BoardAction.Fragile) || boardActions.Contains(BoardAction.Seed))
        //                     {
        //                         newTileSet = newTileSet.Clone();
        //                         newGameState.modifiedTiles = new List<V2Int>(newGameState.modifiedTiles);
        //                         newGameState.modifiedTiles.Add(targetPosition.ToModelCoordinates());
        //                     }

        //                     traveling = MapController.ModelTakeBoardAction(newTileSet, targetPosition, boardActions, ref currentDirection);
        //                     newGameState.playerPosition = targetPosition.ToModelCoordinates();
        //                     if (actionTurns > 20)
        //                     {
        //                         Logger.LogError("Probably stuck in an infinite loop");
        //                         newGameState.playerPosition = currentGameState.playerPosition;
        //                         traveling = false;
        //                     }
        //                 }

        //                 //Maybe check if board didnt change and playerposition didnt change and skip accordingly

        //                 if (newTileSet.GetTile(newTileSet.GetPlayerPosition()).GetTileType() == "Hole" || newTileSet.GetTile(newTileSet.GetPlayerPosition()).GetTileType() == "Fragile")
        //                 {
        //                     // Logger.Log("Player in hole");
        //                     continue;
        //                 }

        //                 bool explored = false;
        //                 foreach (GameStateHolder exploredState in exploredGameStates)
        //                 {
        //                     if (newGameState.modifiedTiles == exploredState.modifiedTiles && newGameState.playerPosition == exploredState.playerPosition)
        //                     {
        //                         explored = true;
        //                         break;
        //                     }
        //                 }

        //                 if (explored)
        //                 {
        //                     continue;
        //                 }

        //                 exploredGameStates.Add(newGameState);

        //                 // if (exploredStates.ContainsKey(newGameState.playerPosition) && exploredStates[newGameState.playerPosition].ContainsKey(newGameState.modifiedTiles))
        //                 // {
        //                 //     continue;
        //                 // }

        //                 // //Add explored state
        //                 // if (!exploredStates.ContainsKey(newGameState.playerPosition))
        //                 // {
        //                 //     exploredStates.Add(newGameState.playerPosition, new Dictionary<List<V2Int>, bool>());
        //                 // }
        //                 // exploredStates[newGameState.playerPosition].Add(newGameState.modifiedTiles, true);

        //                 if (newTileSet.GetPlayerPosition() == newTileSet.GetGoalPosition())
        //                 {
        //                     // Logger.Log("found" + totalStates);
        //                     // Logger.LogWarning("solution found");
        //                     //Deep clone piece in case tileset changes, we want solution pieces to be FIXED
        //                     Piece[] solutionPieces = new Piece[tileSet.GetPlacedPieces().Count];
        //                     for (int pieceIndex = 0; pieceIndex < tileSet.GetPlacedPieces().Count; pieceIndex++)
        //                     {
        //                         solutionPieces[pieceIndex] = tileSet.GetPlacedPieces()[pieceIndex].Clone();
        //                     }
        //                     solution.SetPieces(solutionPieces);
        //                     solution.SetMoves(new List<V2Int>(newGameState.moves).ToArray());
        //                     solution.SetTurns(newGameState.moves.Count);
        //                     return true;
        //                 }
        //                 validGameStates.Add(newGameState);
        //             }
        //         }
        //         activeGameStates = new List<GameStateHolder>(validGameStates);
        //     }
        //     // Logger.Log(totalStates);
        //     return false;
        // }

        // //Use a list of game states which hold modified tiles instead of tilesets
        // public bool TrySolveBoardNew(TileSet tileSet, out Solution solution)
        // {
        //     int totalStates = 0;
        //     solution = new Solution();

        //     GameStateHolder originalGameStateHolder = new GameStateHolder();
        //     originalGameStateHolder.playerPosition = tileSet.GetPlayerPosition();

        //     List<GameStateHolder> exploredGameStates = new List<GameStateHolder>();

        //     //Key = PlayerPos, Value = Dictionary<modifiedTiles, moves>
        //     // Dictionary<V2Int, Dictionary<List<V2Int>, bool>> exploredStates = new Dictionary<V2Int, Dictionary<List<V2Int>, bool>>();
        //     // exploredStates.Add(originalGameStateHolder.playerPosition, new Dictionary<List<V2Int>, bool>() { { originalGameStateHolder.modifiedTiles, true } });

        //     List<GameStateHolder> activeGameStates = new List<GameStateHolder>();
        //     activeGameStates.Add(originalGameStateHolder);

        //     while (activeGameStates.Count > 0)
        //     {
        //         // Logger.Log(activeHashes.Count);
        //         List<GameStateHolder> validGameStates = new List<GameStateHolder>();

        //         foreach (GameStateHolder currentGameState in activeGameStates)
        //         {
        //             TileSet currentTileSet = tileSet.Clone();

        //             //Modify currentTileSet according to solution helper
        //             foreach (V2Int pos in currentGameState.modifiedTiles)
        //             {
        //                 if (currentTileSet.GetTile(pos).GetTileType() == "Seed")
        //                 {
        //                     currentTileSet.GetTile(pos).SetTileType("Wall");
        //                     switch (currentTileSet.GetTile(pos).GetSeedType())
        //                     {
        //                         case "Left":
        //                         case "Right":
        //                         case "Up":
        //                         case "Down":
        //                             Tile affectedTile = currentTileSet.GetTile(pos + DataTransformer.DirectionStringToV2Int(currentTileSet.GetTile(pos).GetSeedType()));
        //                             affectedTile.SetSideways(currentTileSet.GetTile(pos).GetSeedType());
        //                             break;
        //                     }
        //                 }
        //                 if (currentTileSet.GetTile(pos).GetTileType() == "Fragile")
        //                 {
        //                     currentTileSet.GetTile(pos).SetTileType("Hole");
        //                 }
        //             }

        //             List<V3> directions = new List<V3>() { V3.left, V3.right, V3.forward, V3.back };
        //             // Dictionary<V3, V2Int> directionMap = new Dictionary
        //             foreach (V3 direction in directions)
        //             {
        //                 //shallow copy, only make a deep copy if state changes
        //                 TileSet newTileSet = currentTileSet;
        //                 newTileSet.SetPlayerPosition(currentGameState.playerPosition);
        //                 V3 currentDirection = direction;

        //                 //Only move when movable tile
        //                 V3 nextPosition = newTileSet.GetPlayerPosition().ToViewCoordinates() + currentDirection;
        //                 if (nextPosition.x < 0 || nextPosition.x >= tileSet.GetXDimension() || -nextPosition.z < 0 || -nextPosition.z >= tileSet.GetYDimension())
        //                 {
        //                     // Logger.LogWarning("out of bounds, SKIP");
        //                     continue;
        //                 }

        //                 if (newTileSet.GetTile(nextPosition.ToModelCoordinates()).GetTileType() == "Wall")
        //                 {
        //                     // Logger.LogWarning("Walking into wall, skip");
        //                     continue;
        //                 }

        //                 if (newTileSet.GetTile(nextPosition.ToModelCoordinates()).GetTileType() == "Hole")
        //                 {
        //                     // Logger.LogWarning("Walking into wall, skip");
        //                     continue;
        //                 }

        //                 //TODO: Skip bad portals too

        //                 totalStates++;
        //                 GameStateHolder newGameState = currentGameState.Copy();
        //                 // Logger.Log("Player at " + newSolutionHelper.playerPosition + " going " + direction.ToModelCoordinates());

        //                 newGameState.moves.Add(direction.ToModelCoordinates());
        //                 bool traveling = true;
        //                 int actionTurns = 0;
        //                 while (traveling)
        //                 {
        //                     actionTurns++;

        //                     V3 targetPosition = TileSetUtilities.TryGetNextActions(newTileSet, newTileSet.GetPlayerPosition().ToViewCoordinates(), currentDirection, out List<BoardAction> boardActions);
        //                     newTileSet.SetPlayerPosition(targetPosition.ToModelCoordinates());

        //                     //Special case for solutionhelper, could maybe be on a subscriber (onFragileStepped)
        //                     //TODO: Implement for seed as well
        //                     if (boardActions.Contains(BoardAction.Fragile) || boardActions.Contains(BoardAction.Seed))
        //                     {
        //                         newTileSet = newTileSet.Clone();
        //                         newGameState.modifiedTiles = new List<V2Int>(newGameState.modifiedTiles);
        //                         newGameState.modifiedTiles.Add(targetPosition.ToModelCoordinates());
        //                     }

        //                     traveling = MapController.ModelTakeBoardAction(newTileSet, targetPosition, boardActions, ref currentDirection);
        //                     newGameState.playerPosition = targetPosition.ToModelCoordinates();
        //                     if (actionTurns > 20)
        //                     {
        //                         Logger.LogError("Probably stuck in an infinite loop");
        //                         newGameState.playerPosition = currentGameState.playerPosition;
        //                         traveling = false;
        //                     }
        //                 }

        //                 //Maybe check if board didnt change and playerposition didnt change and skip accordingly

        //                 if (newTileSet.GetTile(newTileSet.GetPlayerPosition()).GetTileType() == "Hole" || newTileSet.GetTile(newTileSet.GetPlayerPosition()).GetTileType() == "Fragile")
        //                 {
        //                     // Logger.Log("Player in hole");
        //                     continue;
        //                 }

        //                 bool explored = false;
        //                 foreach (GameStateHolder exploredState in exploredGameStates)
        //                 {
        //                     if (newGameState.modifiedTiles == exploredState.modifiedTiles && newGameState.playerPosition == exploredState.playerPosition)
        //                     {
        //                         explored = true;
        //                         break;
        //                     }
        //                 }

        //                 if (explored)
        //                 {
        //                     continue;
        //                 }

        //                 exploredGameStates.Add(newGameState);

        //                 // if (exploredStates.ContainsKey(newGameState.playerPosition) && exploredStates[newGameState.playerPosition].ContainsKey(newGameState.modifiedTiles))
        //                 // {
        //                 //     continue;
        //                 // }

        //                 // //Add explored state
        //                 // if (!exploredStates.ContainsKey(newGameState.playerPosition))
        //                 // {
        //                 //     exploredStates.Add(newGameState.playerPosition, new Dictionary<List<V2Int>, bool>());
        //                 // }
        //                 // exploredStates[newGameState.playerPosition].Add(newGameState.modifiedTiles, true);

        //                 if (newTileSet.GetPlayerPosition() == newTileSet.GetGoalPosition())
        //                 {
        //                     // Logger.Log("found" + totalStates);
        //                     // Logger.LogWarning("solution found");
        //                     //Deep clone piece in case tileset changes, we want solution pieces to be FIXED
        //                     Piece[] solutionPieces = new Piece[tileSet.GetPlacedPieces().Count];
        //                     for (int pieceIndex = 0; pieceIndex < tileSet.GetPlacedPieces().Count; pieceIndex++)
        //                     {
        //                         solutionPieces[pieceIndex] = tileSet.GetPlacedPieces()[pieceIndex].Clone();
        //                     }
        //                     solution.SetPieces(solutionPieces);
        //                     solution.SetMoves(new List<V2Int>(newGameState.moves).ToArray());
        //                     solution.SetTurns(newGameState.moves.Count);
        //                     return true;
        //                 }
        //                 validGameStates.Add(newGameState);
        //             }
        //         }
        //         activeGameStates = new List<GameStateHolder>(validGameStates);
        //     }
        //     // Logger.Log(totalStates);
        //     return false;
        // }
        // public bool TrySolveBoard(TileSet tileSet, out Solution solution)
        // {
        //     int totalStates = 0;
        //     solution = new Solution();

        //     SolutionHelper originalHelper = new SolutionHelper();
        //     originalHelper.playerPosition = tileSet.GetPlayerPosition();

        //     Dictionary<System.Numerics.BigInteger, bool> exploredStates = new Dictionary<System.Numerics.BigInteger, bool>();

        //     List<System.Numerics.BigInteger[]> activeHashes = new List<System.Numerics.BigInteger[]>();
        //     exploredStates.Add(originalHelper.ToBigInt(), true);

        //     activeHashes.Add(originalHelper.ToBigIntArray());
        //     while (activeHashes.Count > 0)
        //     {
        //         // Logger.Log(activeHashes.Count);
        //         List<System.Numerics.BigInteger[]> validHashes = new List<System.Numerics.BigInteger[]>();

        //         foreach (System.Numerics.BigInteger[] currentHash in activeHashes)
        //         {
        //             SolutionHelper currentSolutionHelper = new SolutionHelper(currentHash);
        //             TileSet currentTileSet = tileSet.Clone();

        //             currentTileSet.SetPlayerPosition(currentSolutionHelper.playerPosition);

        //             //Modify tileset according to solution helper
        //             foreach (V2Int pos in currentSolutionHelper.modifiedTiles)
        //             {
        //                 if (currentTileSet.GetTile(pos).GetTileType() == "Seed")
        //                 {
        //                     currentTileSet.GetTile(pos).SetTileType(currentTileSet.GetTile(pos).GetSeedType());
        //                 }
        //                 if (currentTileSet.GetTile(pos).GetTileType() == "Fragile")
        //                 {
        //                     currentTileSet.GetTile(pos).SetTileType("Hole");
        //                 }
        //             }

        //             List<V3> directions = new List<V3>() { V3.left, V3.right, V3.forward, V3.back };

        //             foreach (V3 direction in directions)
        //             {
        //                 totalStates++;
        //                 //Only move when movable tile
        //                 try
        //                 {
        //                     TileSet newTileSet = currentTileSet.Clone();
        //                     V3 currentDirection = direction;
        //                     SolutionHelper newSolutionHelper = currentSolutionHelper.Clone();
        //                     // Logger.Log("Player at " + newSolutionHelper.playerPosition + " going " + direction.ToModelCoordinates());

        //                     newSolutionHelper.moves.Add(direction.ToModelCoordinates());
        //                     bool traveling = true;
        //                     int actionTurns = 0;
        //                     while (traveling)
        //                     {
        //                         actionTurns++;
        //                         if (actionTurns > 100)
        //                         {
        //                             throw new System.Exception("LOOPING");
        //                         }
        //                         V3 targetPosition = TileSetUtilities.TryGetNextActions(newTileSet, newTileSet.GetPlayerPosition().ToViewCoordinates(), currentDirection, out List<BoardAction> boardActions);
        //                         newTileSet.SetPlayerPosition(targetPosition.ToModelCoordinates());
        //                         newSolutionHelper.playerPosition = targetPosition.ToModelCoordinates();
        //                         //Special case for solutionhelper, could maybe be on a subscriber (onFragileStepped)
        //                         if (boardActions.Contains(BoardAction.Fragile))
        //                         {
        //                             newSolutionHelper.modifiedTiles.Add(targetPosition.ToModelCoordinates());
        //                         }
        //                         traveling = MapController.ModelTakeBoardAction(newTileSet, targetPosition, boardActions, ref currentDirection);
        //                     }

        //                     if (newTileSet.GetTile(newTileSet.GetPlayerPosition()).GetTileType() == "Hole" || newTileSet.GetTile(newTileSet.GetPlayerPosition()).GetTileType() == "Fragile")
        //                     {
        //                         // Logger.Log("Player in hole");
        //                         continue;
        //                     }

        //                     System.Numerics.BigInteger[] arrayHash = newSolutionHelper.ToBigIntArray();
        //                     System.Numerics.BigInteger intHash = newSolutionHelper.ToBigInt();


        //                     if (exploredStates.ContainsKey(intHash))
        //                     {
        //                         // Logger.Log("State exists" + intHash.ToString());
        //                         continue;
        //                     }

        //                     exploredStates.Add(intHash, true);
        //                     // Logger.Log("New state added " + intHash.ToString());

        //                     if (newTileSet.GetPlayerPosition() == newTileSet.GetGoalPosition())
        //                     {
        //                         // Logger.Log("found" + totalStates);
        //                         // Logger.LogWarning("solution found");
        //                         //Deep clone piece in case tileset changes, we want solution pieces to be FIXED
        //                         Piece[] solutionPieces = new Piece[tileSet.GetPlacedPieces().Count];
        //                         for (int pieceIndex = 0; pieceIndex < tileSet.GetPlacedPieces().Count; pieceIndex++)
        //                         {
        //                             solutionPieces[pieceIndex] = tileSet.GetPlacedPieces()[pieceIndex].Clone();
        //                         }
        //                         solution.SetPieces(solutionPieces);
        //                         solution.SetMoves(new List<V2Int>(newSolutionHelper.moves).ToArray());
        //                         solution.SetTurns(newSolutionHelper.moves.Count);
        //                         return true;
        //                     }
        //                     validHashes.Add(arrayHash);
        //                 }
        //                 catch
        //                 {
        //                     // Logger.LogWarning("SKIPA");
        //                     continue;
        //                 }
        //             }
        //         }
        //         activeHashes = new List<System.Numerics.BigInteger[]>(validHashes);
        //     }
        //     // Logger.Log(totalStates);
        //     return false;
        // }

        // public bool TrySolveBoardOld(TileSet tileSet, out Solution solution)
        // {
        //     int totalStates = 0;
        //     solution = new Solution();

        //     List<SolutionBot> activeBots = new List<SolutionBot>();
        //     List<SolutionBot> exploredBots = new List<SolutionBot>();

        //     activeBots.Add(new SolutionBot(tileSet));
        //     while (activeBots.Count > 0)
        //     {
        //         List<SolutionBot> newBots = new List<SolutionBot>();
        //         List<SolutionBot> validBots = new List<SolutionBot>();
        //         //get all new bots
        //         foreach (SolutionBot solutionBot in activeBots)
        //         {
        //             // Logger.LogWarning("BYE");
        //             newBots.AddRange(SolutionBotController.TryMoveAllDirectionsNew(solutionBot));
        //             totalStates += 4;
        //         }

        //         int i = 0;
        //         //validate and add new bots
        //         foreach (SolutionBot solutionBot in newBots)
        //         {
        //             if (i > 10000)
        //             {
        //                 Logger.LogError("toomuch");
        //                 return false;
        //             }
        //             if (solutionBot.IsValid() && !solutionBot.IsInList(validBots) && !solutionBot.IsInList(exploredBots) && !solutionBot.IsInList(activeBots)) //&& is valid (check if not on hole, or other losing conditions)
        //             {
        //                 if (solutionBot.IsPlayerOnGoal())
        //                 {
        //                     // Logger.Log(totalStates);
        //                     //Deep clone piece in case tileset changes, we want solution pieces to be FIXED
        //                     Piece[] solutionPieces = new Piece[tileSet.GetPlacedPieces().Count];
        //                     for (int pieceIndex = 0; pieceIndex < tileSet.GetPlacedPieces().Count; pieceIndex++)
        //                     {
        //                         solutionPieces[pieceIndex] = tileSet.GetPlacedPieces()[pieceIndex].Clone();
        //                     }
        //                     solution.SetPieces(solutionPieces);

        //                     solution.SetTurns(solutionBot.GetTurns());
        //                     solution.SetMoves(new List<V2Int>(solutionBot.GetMoves()).ToArray());
        //                     return true;
        //                 }
        //                 validBots.Add(solutionBot);
        //             }
        //             i++;
        //         }
        //         //update lists and go back to loop
        //         exploredBots.AddRange(activeBots);
        //         activeBots = new List<SolutionBot>(validBots);
        //     }
        //     // Logger.Log(totalStates);
        //     return false;
        // }
    }
}

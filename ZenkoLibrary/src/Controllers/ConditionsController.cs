using Zenko.Factories;
using Zenko.Entities;
using Zenko.Utilities;
using System;
using Zenko.Extensions;
using System.Collections.Generic;

namespace Zenko.Controllers
{
    public class ConditionsController
    {
        public static Conditions GetConditions(Map map, Solution solution)
        {
            Conditions conditions = new Conditions();

            //Prepare map with solution pieces
            Map mapToTest = MapFactory.Clone(map);
            foreach (Piece piece in solution.GetPieces())
            {
                TileSetController.PlacePiece(mapToTest.GetTileSet(), piece);
            }

            conditions.SetTurns(solution.GetTurns());

            //Simulate movement and listen to events
            Action<V2Int> OnPiecesHit = (V2Int position) =>
            {
                conditions.AddPieceHit();
            };
            mapToTest.GetTileSet().OnPieceHit += OnPiecesHit;

            Action<V2Int> OnPushedByWind = (V2Int position) =>
            {
                conditions.AddWindUsed();
            };
            mapToTest.GetTileSet().OnPushedByWind += OnPushedByWind;

            Action<V2Int> OnPortalUsed = (V2Int position) =>
            {
                conditions.AddPortalUsed();
            };
            mapToTest.GetTileSet().OnPortalUsed += OnPortalUsed;

            Action<V2Int> OnStoppedOnSeed = (V2Int position) =>
            {
                conditions.AddStoppedOnSeed();
            };
            mapToTest.GetTileSet().OnStoppedOnSeed += OnStoppedOnSeed;

            Action<V2Int> OnFrontalWind = (V2Int position) =>
            {
                conditions.AddFrontalWind();
            };
            mapToTest.GetTileSet().OnFrontalWind += OnFrontalWind;

            Action<V2Int> OnPortalBlocked = (V2Int position) =>
            {
                conditions.AddPortalBlocked();
            };
            mapToTest.GetTileSet().OnPortalBlocked += OnPortalBlocked;


            bool traveling = true;
            foreach (V2Int direction in solution.GetMoves())
            {
                V3 curDirection = direction.ToViewCoordinates();
                traveling = true;
                while (traveling)
                {
                    V3 pastDirection = curDirection;
                    V3 targetPosition = TileSetUtilities.TryGetNextActions(mapToTest.GetTileSet(), mapToTest.GetTileSet().GetPlayerPosition().ToViewCoordinates(), curDirection, out List<BoardAction> boardActions);
                    mapToTest.GetTileSet().SetPlayerPosition(targetPosition.ToModelCoordinates());
                    traveling = MapController.ModelTakeBoardAction(mapToTest.GetTileSet(), mapToTest.GetTileSet().GetPlayerPosition().ToViewCoordinates(), boardActions, ref curDirection);
                    if (boardActions.Contains(BoardAction.Portal))
                    {
                        if (PieceUtilities.IsPortalPortable(pastDirection.ToModelCoordinates(), mapToTest.GetTileSet().GetPieceAt(targetPosition.ToModelCoordinates()), mapToTest.GetTileSet()))
                        {
                            Piece matchingPortal = PieceUtilities.GetMatchingPortal(mapToTest.GetTileSet(), targetPosition.ToModelCoordinates());
                            mapToTest.GetTileSet().SetPlayerPosition(matchingPortal.GetPosition());
                        }
                    }
                }
            }

            //Unsubscribe
            mapToTest.GetTileSet().OnPieceHit -= OnPiecesHit;
            mapToTest.GetTileSet().OnPushedByWind -= OnPushedByWind;
            mapToTest.GetTileSet().OnPortalUsed -= OnPortalUsed;
            mapToTest.GetTileSet().OnStoppedOnSeed -= OnStoppedOnSeed;
            mapToTest.GetTileSet().OnFrontalWind -= OnFrontalWind;
            mapToTest.GetTileSet().OnPortalBlocked -= OnPortalBlocked;

            return conditions;
        }
    }
}
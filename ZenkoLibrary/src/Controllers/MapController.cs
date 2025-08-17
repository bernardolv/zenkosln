using System.Collections.Generic;
using Zenko.Extensions;
using Zenko;
using Zenko.Utilities;
using Zenko.Entities;
using System;

namespace Zenko.Controllers
{
    public class MapController
    {

        //Note: maybe for multiplayer will need to pass in the player?
        public static bool ModelTakeBoardAction(TileSet tileSet, V3 position, List<BoardAction> boardActions, ref V3 direction)
        {
            CallEvents(tileSet, position.ToModelCoordinates(), boardActions, direction.ToModelCoordinates());

            V2Int tilePosition = new V2Int((int)position.x, -(int)position.z);
            Tile actionTile = tileSet.GetTile(new V2Int((int)position.x, -(int)position.z));


            //These first 2 can also be affected by sideways so they dont return
            if (boardActions.Contains(BoardAction.Seed))
            {
                //MODEL (maybe do it at another time?)
                // Debug.Log("Seed turned something else");
                actionTile.SetTileType("Wall");
                switch (actionTile.GetSeedType())
                {
                    case "Left":
                    case "Right":
                    case "Up":
                    case "Down":
                        Tile affectedTile = tileSet.GetTile(tilePosition + DataTransformer.DirectionStringToV2Int(actionTile.GetSeedType()));
                        affectedTile.SetSideways(actionTile.GetSeedType());
                        break;
                }
            }
            if (boardActions.Contains(BoardAction.Fragile))
            {
                //MODEL (maybe do it at another time?) what worries me is how one would play with another if were doing it ahead of time
                actionTile.SetTileType("Hole");
            }

            if (boardActions.Contains(BoardAction.Hole))
            {
                return false;
            }
            else if (boardActions.Contains(BoardAction.Goal))
            {
                return false;
            }
            else if (boardActions.Contains(BoardAction.Portal))
            {
                if (PieceUtilities.IsPortalPortable(direction.ToModelCoordinates(), tileSet.GetPieceAt(position.ToModelCoordinates()), tileSet))
                {
                    Piece matchingPortal = PieceUtilities.GetMatchingPortal(tileSet, position.ToModelCoordinates());
                    direction = matchingPortal.GetIdentifierDirectionVector().ToViewCoordinates();
                }
                else
                {
                    //Shouldn't get here since in theory TileSetUtilities already checked that its portable...
                    return false;
                }
            }
            else if (boardActions.Contains(BoardAction.Stop))
            {
                return false;
            }
            //LRUD is done at the end since others take priority. example: if you hit a rock but a icarus is affecting it, we only care that it hit a rock and not the lrud aspect.
            else if (boardActions.Contains(BoardAction.Left))
            {
                direction = V3.left;
            }
            else if (boardActions.Contains(BoardAction.Right))
            {
                direction = V3.right;
            }
            else if (boardActions.Contains(BoardAction.Up))
            {
                direction = V3.forward;
            }
            else if (boardActions.Contains(BoardAction.Down))
            {
                direction = V3.back;
            }
            return true;
        }

        static void CallEvents(TileSet tileSet, V2Int position, List<BoardAction> boardActions, V2Int direction)
        {
            if (boardActions.Contains(BoardAction.Portal))
            {
                // Console.WriteLine("PORTAL");
                if (PieceUtilities.IsPortalPortable(direction, tileSet.GetPieceAt(position), tileSet))
                {
                    // Debug.Log("Portald correctly");
                    tileSet.InvokeOnPortalUsed(position);
                }
                else
                {
                    //Shouldn't get here since in theory TileSetUtilities already checked that its portable...
                    tileSet.InvokeOnPieceHit(position);
                }
            }
            else if (boardActions.Contains(BoardAction.Stop))
            {
                if (tileSet.GetTile(position + direction).GetTileType() == "Portal" && direction == -tileSet.GetPieceAt(position + direction).GetIdentifierDirectionVector())
                {
                    tileSet.InvokeOnPortalBlocked(position);
                }
                //If we stopped and the next tile was a piece it is safe to assume that we hit a piece.
                else if (tileSet.placedPiecePositions.Contains(position + direction))
                {
                    tileSet.InvokeOnPieceHit(position);
                }

                //We can safely assume that if it stops on a piece it MUST be a seed piece since its the only walkable ones.
                if (tileSet.placedPiecePositions.Contains(position))
                {
                    tileSet.InvokeOnStoppedOnSeed(position);
                }

            }
            else if (boardActions.Contains(BoardAction.Left))
            {
                tileSet.InvokeOnPushedByWind(position);
                V3 newDirection = V3.left;
                TileSetUtilities.TryGetNextActions(tileSet, position.ToViewCoordinates(), newDirection, out List<BoardAction> newBoardActions);
                if (newBoardActions.Contains(BoardAction.Stop))
                {
                    tileSet.InvokeOnFrontalWind(position);
                }
            }
            else if (boardActions.Contains(BoardAction.Right))
            {
                tileSet.InvokeOnPushedByWind(position);
                V3 newDirection = V3.right;
                TileSetUtilities.TryGetNextActions(tileSet, position.ToViewCoordinates(), newDirection, out List<BoardAction> newBoardActions);
                if (newBoardActions.Contains(BoardAction.Stop))
                {
                    tileSet.InvokeOnFrontalWind(position);
                }
            }
            else if (boardActions.Contains(BoardAction.Up))
            {
                tileSet.InvokeOnPushedByWind(position);
                V3 newDirection = V3.forward;
                TileSetUtilities.TryGetNextActions(tileSet, position.ToViewCoordinates(), newDirection, out List<BoardAction> newBoardActions);
                if (newBoardActions.Contains(BoardAction.Stop))
                {
                    tileSet.InvokeOnFrontalWind(position);
                }
            }
            else if (boardActions.Contains(BoardAction.Down))
            {
                tileSet.InvokeOnPushedByWind(position);
                V3 newDirection = V3.back;
                TileSetUtilities.TryGetNextActions(tileSet, position.ToViewCoordinates(), newDirection, out List<BoardAction> newBoardActions);
                if (newBoardActions.Contains(BoardAction.Stop))
                {
                    tileSet.InvokeOnFrontalWind(position);
                }
            }
        }
    }
}
using System.Collections.Generic;
using Zenko.Extensions;
using Zenko;
using Zenko.Utilities;
using Zenko.Entities;

namespace Zenko.Controllers
{
    public class MapController
    {
        //Note: maybe for multiplayer will need to pass in the player?
        public static bool ModelTakeBoardAction(TileSet tileSet, V3 position, List<BoardAction> boardActions, ref V3 direction)
        {
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
                    // Debug.Log("Portald correctly");
                    Piece matchingPortal = PieceUtilities.GetMatchingPortal(tileSet, position.ToModelCoordinates());
                    direction = matchingPortal.GetIdentifierDirectionVector().ToViewCoordinates();
                }
                else
                {
                    // Debug.Log("Couldnt portal");
                    return false;
                }
            }
            else if (boardActions.Contains(BoardAction.Stop))
            {
                return false;
            }
            //LRUD is done at the end since others take priority
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
    }
}
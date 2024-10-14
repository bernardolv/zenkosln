using System;
using System.Collections;
using System.Collections.Generic;
using Zenko.Presets;
using Zenko.Extensions;
using Zenko.Entities;

namespace Zenko.Utilities
{
    public class TileSetUtilities
    {
        public static V3 TryGetNextActions(TileSet tileSet, V3 origin, V3 direction, out List<BoardAction> boardActions)
        {
            boardActions = new List<BoardAction>();
            BoardAction boardAction;
            V3 lastPosition = origin;
            while (true)
            {
                V3 targetPosition = lastPosition + direction;
                if (targetPosition.x < 0 || targetPosition.x >= tileSet.GetXDimension() || -targetPosition.z < 0 || -targetPosition.z >= tileSet.GetYDimension())
                {
                    throw new Exception("Position outside board or null tile.");
                }
                Tile targetTile = tileSet.GetTile(new V2Int((int)targetPosition.x, -(int)targetPosition.z));

                if (!IsInside(tileSet, targetPosition) || targetTile == null)
                {
                    throw new Exception("Position outside board or null tile");
                }

                //Add board action for LRUD cases
                if (targetTile.GetSideways() != null)
                {
                    if (!Enum.TryParse(targetTile.GetSideways(), out boardAction))
                    {
                        throw new Exception("Could not parse " + targetTile.GetSideways());
                    }
                    boardActions.Add(boardAction); //BoardAction.Left,Right,Up,Down

                    //if ice or wood, then do nothing else
                    switch (targetTile.GetTileType())
                    {
                        case "Ice":
                        case "Wood":
                            return targetPosition;
                    }
                }

                switch (targetTile.GetTileType())
                {
                    case "Ice":
                    case "Wood":
                        break;
                    case "Wall":
                        boardActions.Add(BoardAction.Stop);
                        return lastPosition;
                    case "Start":
                        boardActions.Add(BoardAction.Stop);
                        return lastPosition;
                    case "Goal":
                        boardActions.Add(BoardAction.Goal);
                        return targetPosition;
                    case "Hole":
                        boardActions.Add(BoardAction.Hole);
                        return targetPosition;
                    case "Fragile":
                        boardActions.Add(BoardAction.Fragile);
                        return targetPosition;
                    case "Seed":
                        boardActions.Add(BoardAction.Seed);
                        return targetPosition;
                    case "Portal":
                        if (PieceUtilities.IsPortalPortable(direction.ToModelCoordinates(), tileSet.GetPieceAt(targetPosition.ToModelCoordinates()), tileSet))
                        {
                            boardActions.Add(BoardAction.Portal);
                            return targetPosition;
                        }
                        else
                        {
                            boardActions.Add(BoardAction.Stop);
                            return lastPosition;
                        }
                }

                lastPosition = targetPosition;
            }
        }

        public static V2Int GetMatchingPortalPosition(TileSet tileSet, V2Int position)
        {
            foreach (V2Int portalPosition in tileSet.GetActivePortalPositions())
            {
                if (portalPosition != position)
                {
                    return portalPosition;
                }
            }
            return position;
        }

        public static V2Int GetPortalDirection(TileSet tileSet, V2Int position)
        {
            switch (tileSet.GetTile(position).GetPortalType())
            {
                case "Left":
                    return V2Int.left;
                case "Right":
                    return V2Int.right;
                case "Up":
                    return V2Int.down;
                case "Down":
                    return V2Int.up;
                default:
                    return V2Int.zero;
            }
        }

        public static List<V2Int> GetCandidateTilePositions(TileSet tileSet)
        {
            int xDimension = tileSet.GetXDimension();
            int yDimension = tileSet.GetYDimension();

            List<V2Int> list = new List<V2Int>();
            for (int y = 0; y < yDimension; y++)
            {
                for (int x = 0; x < xDimension; x++)
                { //loop through all tiles ogtiles
                    if (tileSet.GetTile(new V2Int(x, y)).GetTileType() == "Ice")
                    {
                        list.Add(new V2Int(x, y));
                    }
                }
            }
            return list;
        }

        public static bool TryGetSideWaysDirection(TileSet tileSet, V2Int position, out V2Int direction)
        {
            direction = V2Int.zero;
            switch (tileSet.GetTile(position.x, position.y).GetSideways())
            {
                case "Left":
                    direction = V2Int.left;
                    return true;
                case "Right":
                    direction = V2Int.right;
                    return true;
                case "Up":
                    direction = V2Int.down;
                    return true;
                case "Down":
                    direction = V2Int.up;
                    return true;
                default:
                    return false;
            }
        }

        public static string GetPortalTypeAt(TileSet tileSet, V2Int position)
        {
            return tileSet.GetTile(position.x, position.y).GetPortalType();
        }

        //View
        public static bool IsStoppingOrChangingDirectionToGoal(TileSet tileSet, V2Int position, V2Int currentDirection, List<BoardAction> boardActions)
        {
            // foreach (BoardAction action in boardActions) Logger.Log(action.ToString());
            //return true aslong as it doesnt stop on a fragile or hole tile
            if (boardActions.Contains(BoardAction.Stop) && tileSet.GetTile(position).GetTileType() != "Fragile" && tileSet.GetTile(position).GetTileType() != "Hole")
            {
                return true;
            }
            if (boardActions.Contains(BoardAction.Portal))
            {
                if (PieceUtilities.IsPortalPortable(currentDirection, tileSet.GetPieceAt(position), tileSet))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            if (boardActions.Contains(BoardAction.Left) && tileSet.GetGoalPosition().x < position.x && tileSet.GetGoalPosition().y == position.y)
            {
                return true;
            }
            if (boardActions.Contains(BoardAction.Right) && tileSet.GetGoalPosition().x > position.x && tileSet.GetGoalPosition().y == position.y)
            {
                return true;
            }
            if (boardActions.Contains(BoardAction.Up) && tileSet.GetGoalPosition().y < position.y && tileSet.GetGoalPosition().x == position.x)
            {
                return true;
            }
            if (boardActions.Contains(BoardAction.Down) && tileSet.GetGoalPosition().y > position.y && tileSet.GetGoalPosition().x == position.x)
            {
                return true;
            }
            return false;
        }

        public static bool HasStraightLineOfSightToGoal(TileSet tileSet, V2Int position)
        {
            V2Int goalPosition = tileSet.GetGoalPosition();
            if (HasLineOfSightToGoalInDirection(tileSet, position, V2Int.left, goalPosition))
            {
                return true;
            }
            if (HasLineOfSightToGoalInDirection(tileSet, position, V2Int.right, goalPosition))
            {
                return true;
            }
            if (HasLineOfSightToGoalInDirection(tileSet, position, V2Int.up, goalPosition))
            {
                return true;
            }
            if (HasLineOfSightToGoalInDirection(tileSet, position, V2Int.down, goalPosition))
            {
                return true;
            }
            return false;
        }

        public static bool HasLineOfSightToGoalInDirection(TileSet tileSet, V2Int position, V2Int direction, V2Int goalPosition)
        {
            //If theres a wall or a hole in the way, no line of sight
            V2Int currentPosition = position;
            while (currentPosition != goalPosition)
            {
                currentPosition += direction;
                //NEED BETTER PROTECTION
                if (!IsInside(tileSet, currentPosition.ToViewCoordinates()))
                {
                    return false;
                }
                switch (tileSet.GetTile(currentPosition).GetTileType())
                {
                    case "Wall":
                    case "Hole":
                        return false;
                    case "Portal":
                        if (PieceUtilities.IsPortalPortable(direction, tileSet.GetPieceAt(currentPosition), tileSet))
                        {
                            Piece matchingPortal = PieceUtilities.GetMatchingPortal(tileSet, currentPosition);
                            currentPosition = matchingPortal.GetPosition();
                            direction = matchingPortal.GetIdentifierDirectionVector();
                        }
                        else
                        {
                            return false;
                        }
                        break;
                        //TODO: Implement wind
                }
            }

            return true;
        }

        public static bool IsWalkable(TileSet tileSet, V3 targetPosition, V3 directionV, int turns)
        {
            if (!IsInside(tileSet, targetPosition))
            {
                throw new System.Exception("Not inside");
            }
            int x = (int)targetPosition.x;
            int y = -(int)targetPosition.z;
            Tile tile = tileSet.GetTile(x, y);

            string tileType = tile.GetTileType();
            if (tileType == "Wall" || tileType == "Start")
            {
                return false;
            }
            if (tileType == "Goal" && turns == 0)
            {
                return false;
            }
            if (tileType == "Portal")
            {
                return PieceUtilities.IsPortalPortable(directionV.ToModelCoordinates(), tileSet.GetPieceAt(targetPosition.ToModelCoordinates()), tileSet);
            }
            return true;
        }

        public static bool IsInside(TileSet tileSet, V3 tilePosition)
        {
            int dimension = tileSet.GetYDimension();

            int x = (int)tilePosition.x;
            int y = (int)-tilePosition.z;
            if (x >= 0 && x < dimension &&
                y >= 0 && y < dimension)
            {
                return true;
            }
            else
                return false;
        }

        public static bool IsInside(TileSet tileSet, V2Int tilePosition)
        {
            if (tilePosition.x < 0 || tilePosition.y < 0)
            {
                return false;
            }
            if (tilePosition.x >= tileSet.GetXDimension() || tilePosition.y >= tileSet.GetYDimension())
            {
                return false;
            }
            return true;
        }
    }
}

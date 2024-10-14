using Zenko.Controllers;
using Zenko;
using Zenko.Utilities;
using Zenko.Entities;

namespace Zenko.Entities
{
    public class ConditionBot
    {
        /////////////////////////
        //      PROPERTIES     //
        /////////////////////////

        Conditions conditions;
        TileSet tileSet;
        V2Int position;
        V2Int direction;

        /////////////////////////
        //  GETTERS & SETTERS  //
        /////////////////////////

        public Conditions GetConditions()
        {
            return conditions;
        }

        /////////////////////////
        //     CONSTRUCTORS    //
        /////////////////////////

        public ConditionBot(Map map)
        {
            this.tileSet = map.GetTileSet().Clone();
            conditions = new Conditions();
            position = this.tileSet.GetStartPosition();
        }

        //////////////////////////
        //       MOVEMENT       //
        //////////////////////////

        public void Move(V2Int moveDirection)
        {
            conditions.AddTurn();
            // Debug.Log("Moving starting at " + position);
            // Debug.Log("Moving " + moveDirection);
            direction = moveDirection;
            V2Int newDirection = direction;
            bool moving = true;
            int counter = 0;

            while (moving)
            {

                direction = newDirection;
                V2Int candidatePosition = position + direction;
                if (IsOutOfBounds(candidatePosition))
                {
                    // Debug.LogError("attaband");
                    return;
                }

                if (TileSetUtilities.TryGetSideWaysDirection(tileSet, candidatePosition, out V2Int sideWaysDirection))
                {
                    newDirection = sideWaysDirection;
                }
                // Debug.Log(board.GetTile(candidatePosition).GetTileType());
                // Debug.Log("checking at " + candidatePosition + " of type " + map.tileSet.GetTypeAt(candidatePosition));
                switch (tileSet.GetTile(candidatePosition).GetTileType())
                {
                    default:
                        return;
                    case "Start":
                        return;
                    case "Wall":
                        if (tileSet.GetPlacedPiecePositions().Contains(candidatePosition))
                        {
                            conditions.AddPieceHit();
                        }
                        return;
                    case "Ice":
                        position = candidatePosition;
                        if (newDirection != direction) AddWind();
                        break;
                    case "Goal":
                        return;
                    case "Hole":
                        throw new System.Exception("Shouldnt be hole in conditioncheck");
                    case "Fragile":
                        position = candidatePosition;
                        tileSet.GetTile(candidatePosition).SetTileType("Hole");
                        if (newDirection != direction) AddWind();
                        break;
                    case "Wood":
                        position = candidatePosition;
                        if (newDirection != direction) AddWind();
                        break;
                    case "Seed":
                        TileSetController.PopSeedAt(tileSet, candidatePosition);
                        position = candidatePosition;
                        if (IsNextMoveStop(position, newDirection)) conditions.AddStoppedOnSeed();
                        if (newDirection != direction) AddWind();
                        break;
                    case "Portal":
                        // Debug.Log("checking portal at " + candidatePosition);
                        if (IsPortalFacingPlayer(direction, candidatePosition))
                        {
                            V2Int matchingPortalPosition = TileSetUtilities.GetMatchingPortalPosition(tileSet, candidatePosition);
                            V2Int matchingPortalDirection = TileSetUtilities.GetPortalDirection(tileSet, matchingPortalPosition);
                            //NEED TO REFACTOR HOW TO  HANDLE NO MATCHING PORTALCASES
                            //RIGHT NOW IT WORKS SINCE ISWALKABLE WILLRETURN FALSE SINCE IT CHECKS PORTAL POSITION
                            if (IsWalkable(matchingPortalPosition + matchingPortalDirection))
                            {
                                position = matchingPortalPosition;
                                newDirection = matchingPortalDirection;
                                conditions.AddPortalUsed();
                            }
                            else
                            {
                                conditions.AddPortalBlocked();
                                return;
                            }
                        }
                        //if not allowed, act like wall
                        else
                        {
                            conditions.AddPieceHit();
                            return;
                        }
                        break;
                }
                counter++;
                if (counter > 20)
                {
                    Logger.LogWarning("Loop most likely at conditionbot");
                    return;
                }
                //returns true, checked after sideways change

            }

            void AddWind()
            {
                conditions.AddWindUsed();
                if (newDirection == -direction)
                {
                    conditions.AddFrontalWind();
                }
            }
        }

        /////////////////////////
        //   HELPER METHODS    //
        /////////////////////////
        bool IsNextMoveStop(V2Int position, V2Int direction)
        {
            string nextTileType = tileSet.GetTile(position + direction).GetTileType();
            if (nextTileType == "" || nextTileType == "Start" || nextTileType == "Wall" || (nextTileType == "Portal" && !IsPortalFacingPlayer(direction, position + direction)))
            {
                return true;
            }
            return false;
        }
        bool IsOutOfBounds(V2Int position)
        {
            if (position.x < 0 || position.y < 0 || tileSet.GetXDimension() <= position.x || tileSet.GetYDimension() <= position.y)
            {
                return true;
            }
            return false;
        }
        bool IsPortalFacingPlayer(V2Int direction, V2Int position)
        {
            string portalType = TileSetUtilities.GetPortalTypeAt(tileSet, position);
            if (direction == V2Int.left && portalType == "Right")
            {
                return true;
            }
            else if (direction == V2Int.right && portalType == "Left")
            {
                return true;
            }
            else if (direction == V2Int.up && portalType == "Up")
            {
                return true;
            }
            else if (direction == V2Int.down && portalType == "Down")
            {
                return true;
            }
            else return false;
        }
        bool IsWalkable(V2Int position)
        {
            string tileType = tileSet.GetTile(position).GetTileType();
            // Debug.Log("Trying to move from " + origin + " towards " + direction + " which has a " + board.tileSet.tiles[candidatePosition.x, candidatePosition.y].type);
            switch (tileType)
            {
                case "Start":
                    return false;
                case "Wall":
                    return false;
                case "Portal":
                    //TODO: specific logic for when as wall
                    //Right now should be false all the time since only 1 pair of portals is active
                    return false;
                default:
                    return true;
            }
        }
    }
}


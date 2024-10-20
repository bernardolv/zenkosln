using System.Collections.Generic;
using Zenko.Controllers;
using Zenko.Utilities;

namespace Zenko.Entities
{
    public class SolutionBot
    {
        /////////////////////////
        //      PROPERTIES     //
        /////////////////////////

        bool logError = false;
        TileSet tileSet;
        V2Int playerPosition;
        int turns;
        List<V2Int> moves;

        /////////////////////////
        //     CONSTRUCTORS    //
        /////////////////////////


        public int GetTurns()
        {
            return turns;
        }

        public List<V2Int> GetMoves()
        {
            return moves;
        }

        /////////////////////////
        //     CONSTRUCTORS    //
        /////////////////////////

        public SolutionBot() { }

        public SolutionBot(TileSet tileSet)
        {
            this.tileSet = tileSet;
            this.playerPosition = tileSet.GetStartPosition();
            this.turns = 0;
            this.moves = new List<V2Int>();
        }

        //shallow copy of board since it is used for is equal to
        //everything else deep copy
        public SolutionBot Copy()
        {
            SolutionBot copy = new SolutionBot();
            copy.tileSet = this.tileSet;
            copy.playerPosition = this.playerPosition;
            copy.turns = this.turns;
            copy.moves = new List<V2Int>(this.moves);
            return copy;
        }

        //////////////////////////
        //       MOVEMENT       //
        //////////////////////////


        //void could return false or null if didn't move
        //this would filter it out asap instead of in SolutionController.TrySolve
        public void Move(V2Int direction)
        {
            moves.Add(direction);
            //keeps track of turn count
            turns++;

            V2Int currentDirection = direction;
            bool moving = true;
            int counter = 0;
            V2Int initialPosition = playerPosition;
            while (moving)
            {
                // if can move keep moving
                if (TryMove(playerPosition, currentDirection, out playerPosition, out currentDirection))
                {
                    //FIND A WAY TO DO THIS BETTER AND REMOVE
                    //LOOP PROTECTION
                    counter++;
                    if (counter > 20)
                    {
                        if (logError) Logger.LogError("Probably stuck in an infinite loop");
                        moving = false;
                        //this means it will be filtered out in SolutionController
                        //could use a different way..
                        playerPosition = initialPosition;
                    }
                }
                // if it cant move then itsdone moving
                else
                {
                    moving = false;
                }
            }
        }

        //returns negative if it stops moving
        //be it with wall, goal, etc.
        bool TryMove(V2Int origin, V2Int direction, out V2Int newPosition, out V2Int newDirection)
        {
            V2Int candidatePosition = origin + direction;
            // Debug.Log(candidatePosition + "going " + direction);
            newPosition = origin;
            newDirection = direction;

            if (IsOutOfBounds(candidatePosition))
            {
                return false;
            }

            string tileType = tileSet.GetTile(candidatePosition).GetTileType();

            //would this conflict with anythign else? Lookslike not so far
            //only applies to things with return true
            //overriden by portal direction in portal case
            if (TileSetUtilities.TryGetSideWaysDirection(tileSet, candidatePosition, out V2Int sideWaysDirection))
            {
                if (tileType != "Portal")
                {
                    newDirection = sideWaysDirection;
                }
            }

            // Debug.Log("Trying to move from " + origin + " towards " + direction + " which has a " + board.tileSet.tiles[candidatePosition.x, candidatePosition.y].type);
            switch (tileType)
            {
                case "Start":
                    return false;
                case "Wall":
                    return false;
                case "Goal":
                    newPosition = candidatePosition;
                    return false;
                case "Hole":
                    newPosition = candidatePosition;
                    return false;
                case "Ice":
                    newPosition = candidatePosition;
                    return true;
                case "Wood":
                    newPosition = candidatePosition;
                    return true;
                case "Fragile":
                    //creates a new board to avoid changing the original one
                    //also allows for IsEqualTo to be negative
                    tileSet = tileSet.Clone();
                    newPosition = candidatePosition;
                    tileSet.GetTile(candidatePosition).SetTileType("Hole");
                    return true;
                case "Seed":
                    //creates a new board to avoid changing the original one
                    //also allowsfor IsEqualTo to be negative
                    tileSet = tileSet.Clone();
                    TileSetController.PopSeedAt(tileSet, candidatePosition);
                    newPosition = candidatePosition;
                    return true;
                case "Portal":
                    if (IsPortalFacingPlayer(direction, candidatePosition))
                    {
                        V2Int matchingPortalPosition = TileSetUtilities.GetMatchingPortalPosition(tileSet, candidatePosition);
                        V2Int matchingPortalDirection = TileSetUtilities.GetPortalDirection(tileSet, matchingPortalPosition);
                        //NEED TO REFACTOR HOW TO  HANDLE NO MATCHING PORTALCASES
                        //RIGHT NOW IT WORKS SINCE ISWALKABLE WILLRETURN FALSE SINCE IT CHECKS PORTAL POSITION
                        if (IsWalkable(matchingPortalPosition + matchingPortalDirection))
                        {
                            newPosition = matchingPortalPosition;
                            newDirection = matchingPortalDirection;
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                default:
                    return false;
            }


        }

        /////////////////////////
        //   HELPER METHODS    //
        /////////////////////////
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
        public bool IsPlayerOnGoal()
        {
            if (tileSet.GetTile(playerPosition).GetTileType() == "Goal")
            {
                return true;
            }
            return false;
        }
        //TODO: If board is cloned, this would need to change
        //probably dont need to clone since it only changes when fragile etc,and its cloned manually there
        public bool IsEqualTo(SolutionBot solverBot)
        {
            if (this.tileSet == solverBot.tileSet && this.playerPosition == solverBot.playerPosition)
            {
                return true;
            }
            return false;
        }
        public bool IsValid()
        {
            string tileType = tileSet.GetTile(playerPosition).GetTileType();
            if (tileType == "Hole" || tileType == "Fragile")
            {
                return false;
            }
            return true;
        }
        bool IsOutOfBounds(V2Int position)
        {
            if (position.x < 0 || position.y < 0 || tileSet.GetXDimension() <= position.x || tileSet.GetYDimension() <= position.y)
            {
                return true;
            }
            return false;
        }
        public bool IsInList(List<SolutionBot> list)
        {
            foreach (SolutionBot solutionBot in list)
            {
                if (this.IsEqualTo(solutionBot))
                {
                    return true;
                }
            }
            return false;
        }
        public TileSet GetTileSet()
        {
            return tileSet;
        }
        public void SetPlayerPosition(V2Int position)
        {
            playerPosition = position;
        }
        public V2Int GetPlayerPosition()
        {
            return playerPosition;
        }
    }

}

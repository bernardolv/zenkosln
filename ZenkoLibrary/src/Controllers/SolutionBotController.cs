using System.Collections.Generic;
using Zenko.Entities;
using Zenko;
using Zenko.Utilities;
using Zenko.Extensions;
using Zenko.Controllers;

namespace Zenko.Controllers
{
    public class SolutionBotController
    {
        //returns a list of solution bots post moves
        public static List<SolutionBot> TryMoveAllDirections(SolutionBot solutionBot)
        {
            List<SolutionBot> newBots = new List<SolutionBot>();
            List<V2Int> directions = new List<V2Int>() { V2Int.left, V2Int.right, V2Int.up, V2Int.down };
            foreach (V2Int direction in directions)
            {
                SolutionBot newBot = solutionBot.Copy();
                newBot.Move(direction);
                newBots.Add(newBot);
            }
            return newBots;
        }

        //returns a list of solution bots post moves
        public static List<SolutionBot> TryMoveAllDirectionsNew(SolutionBot solutionBot)
        {
            // Debug.LogWarning("HI");
            List<SolutionBot> newBots = new List<SolutionBot>();
            List<V2Int> directions = new List<V2Int>() { V2Int.left, V2Int.right, V2Int.up, V2Int.down };
            foreach (V2Int direction in directions)
            {
                // Debug.LogWarning(direction);
                V3 currentDirection = direction.ToViewCoordinates();

                SolutionBot newBot = solutionBot.Copy();
                // Debug.Log("Player at " + newBot.GetPlayerPosition() + " going " + direction);
                // Debug.Log((newBot == solutionBot) + " " + (newBot.GetTileSet() == solutionBot.GetTileSet()));

                bool traveling = true;
                while (traveling)
                {
                    // Debug.Log("Player at " + newBot.GetPlayerPosition() + " going " + direction);
                    V3 nextPosition = newBot.GetPlayerPosition().ToViewCoordinates() + currentDirection;
                    if (nextPosition.x < 0 || nextPosition.x >= newBot.GetTileSet().GetXDimension() || -nextPosition.z < 0 || -nextPosition.z >= newBot.GetTileSet().GetYDimension())
                    {
                        // Debug.LogWarning("Out of bounds");
                        continue;
                    }
                    V3 targetPosition = TileSetUtilities.TryGetNextActions(newBot.GetTileSet(), newBot.GetPlayerPosition().ToViewCoordinates(), currentDirection, out List<BoardAction> boardActions);

                    newBot.SetPlayerPosition(targetPosition.ToModelCoordinates());
                    // //Special case for solutionhelper, could maybe be on a subscriber (onFragileStepped)
                    // if (boardActions.Contains(BoardAction.Fragile))
                    // {
                    //     newSolutionHelper.modifiedTiles.Add(targetPosition.ToModelCoordinates());
                    // }
                    traveling = MapController.ModelTakeBoardAction(newBot.GetTileSet(), targetPosition, boardActions, ref currentDirection);
                }
                // Debug.Log("Player finished at " + newBot.GetPlayerPosition());

                newBots.Add(newBot);
            }
            return newBots;
        }
    }
}
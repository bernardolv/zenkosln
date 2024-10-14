using Zenko.Factories;
using Zenko.Entities;

namespace Zenko.Controllers
{
    public class ConditionsController
    {
        public static Conditions GetConditions(Map map, Solution solution)
        {
            Map conditionMap = MapFactory.Clone(map);
            foreach (Piece piece in solution.GetPieces())
            {
                TileSetController.PlacePiece(conditionMap.GetTileSet(), piece);
            }
            ConditionBot conditionBot = new ConditionBot(conditionMap);

            foreach (V2Int direction in solution.GetMoves())
            {
                conditionBot.Move(direction);
            }
            return conditionBot.GetConditions();
        }
    }
}
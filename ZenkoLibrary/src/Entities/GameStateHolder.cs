using System.Collections.Generic;

namespace Zenko.Entities
{
    //Ideally subscribes to on Pop, on Fragile stepped (to keep track of changing tileset)
    public class GameStateHolder
    {
        public V2Int playerPosition;
        public List<V2Int> modifiedTiles = new List<V2Int>(); //this should keep track of fragile tiles gone over and seeds popped. basically anything that changes
        public List<V2Int> moves = new List<V2Int>();

        //shallow copies all elements except for moves which are deep copied list
        public GameStateHolder Copy()
        {
            GameStateHolder copy = new GameStateHolder();
            copy.playerPosition = playerPosition;
            copy.modifiedTiles = this.modifiedTiles;
            copy.moves = new List<V2Int>(moves);

            return copy;
        }
    }
}
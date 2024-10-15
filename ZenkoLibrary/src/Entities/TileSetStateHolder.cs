using System.Collections.Generic;
using Zenko.Entities;

namespace Zenko.Entities
{
    //Ideally subscribes to on Pop, on Fragile stepped (to keep track of changing tileset)
    public class TileSetStateHolder
    {
        public V2Int playerPosition;
        public TileSet tileSet; //this should keep track of fragile tiles gone over and seeds popped. basically anything that changes
        public List<V2Int> moves;
        public int turns;

        //shallow copies all elements except for moves which are deep copied list
        public TileSetStateHolder Copy()
        {
            TileSetStateHolder copy = new TileSetStateHolder();
            copy.playerPosition = this.playerPosition;
            copy.tileSet = this.tileSet;
            copy.moves = new List<V2Int>(moves);
            copy.turns = this.turns;

            return copy;
        }
        public TileSetStateHolder() { }

        public TileSetStateHolder(TileSet tileSet)
        {
            this.playerPosition = tileSet.GetPlayerPosition();
            this.tileSet = tileSet;
            this.moves = new List<V2Int>();
            this.turns = 0;
        }
    }
}
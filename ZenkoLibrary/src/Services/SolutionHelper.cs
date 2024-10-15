using System.Collections.Generic;
using System.Numerics;
using Zenko.Extensions;
using System.Text;

namespace Zenko.Services
{
    //Ideally subscribes to on Pop, on Fragile stepped (to keep track of changing tileset)
    public class SolutionHelper
    {
        public V2Int playerPosition;
        public List<V2Int> modifiedTiles = new List<V2Int>(); //this should keep track of fragile tiles gone over and seeds popped. basically anything that changes
        public List<V2Int> moves = new List<V2Int>();

        public SolutionHelper Clone()
        {
            SolutionHelper clone = new SolutionHelper();
            clone.playerPosition = playerPosition;
            clone.modifiedTiles = new List<V2Int>(this.modifiedTiles);
            clone.moves = new List<V2Int>(this.moves);

            return clone;
        }

        //used for backwards able hashing
        public BigInteger ToBigInt()
        {
            StringBuilder stringBuilder = new StringBuilder("");

            stringBuilder.Append(playerPosition.GetV2Hash());
            stringBuilder.Append(modifiedTiles.Count.GetIntHash());
            foreach (V2Int pos in modifiedTiles)
            {
                stringBuilder.Append(pos.GetV2Hash());
            }
            // stringBuilder.Append(moves.Count.GetIntHash());
            // foreach (V2Int pos in moves)
            // {
            //     stringBuilder.Append(pos.GetV2Hash());
            // }
            return BigInteger.Parse(stringBuilder.ToString());
        }

        //used for non-backwards hashing
        public BigInteger[] ToBigIntArray()
        {
            BigInteger[] hash = new BigInteger[3];
            hash[0] = BigInteger.Parse(playerPosition.GetV2Hash());

            StringBuilder stringBuilder = new StringBuilder("");
            stringBuilder.Append(modifiedTiles.Count.GetIntHash());
            foreach (V2Int pos in modifiedTiles)
            {
                stringBuilder.Append(pos.GetV2Hash());
            }
            hash[1] = BigInteger.Parse(stringBuilder.ToString());

            stringBuilder = new StringBuilder("");
            stringBuilder.Append(moves.Count.GetIntHash());
            foreach (V2Int pos in moves)
            {
                stringBuilder.Append(pos.GetV2Hash());
            }
            hash[2] = BigInteger.Parse(stringBuilder.ToString());

            return hash;
        }

        public SolutionHelper()
        {

        }

        public SolutionHelper(BigInteger[] hash)
        {
            int[] playerData = hash[0].GetInts();
            playerPosition.x = playerData[0] - 1;
            playerPosition.y = playerData[1] - 1;

            int[] modifiedTilesData = hash[1].GetInts();
            int modifiedTileCount = modifiedTilesData[0];
            for (int i = 0; i < modifiedTileCount; i++)
            {
                V2Int modifiedTilePos = new V2Int(modifiedTilesData[i * 2 + 1] - 1, modifiedTilesData[i * 2 + 2] - 1);
                modifiedTiles.Add(modifiedTilePos);
            }

            int[] movesData = hash[2].GetInts();
            int moveCount = movesData[0];
            for (int i = 0; i < moveCount; i++)
            {
                V2Int moveTilePos = new V2Int(movesData[i * 2 + 1] - 1, movesData[i * 2 + 2] - 1);
                moves.Add(moveTilePos);
            }
        }
    }
}
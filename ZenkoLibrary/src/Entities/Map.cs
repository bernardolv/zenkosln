using System.Collections;
using System.Collections.Generic;
using Zenko.Services;

namespace Zenko.Entities
{
    public class Map
    {
        /////////////////////////
        //      PROPERTIES     //
        /////////////////////////

        //starts from top left being 0,0
        TileSet tileSet = new TileSet();
        Solution solution = new Solution();


        /////////////////////////
        // OBJECT RELATIONSHIP //
        /////////////////////////

        Piece[] pieces = new Piece[0];


        /////////////////////////
        // GETTERS AND SETTERS //
        /////////////////////////

        public int GetTurnsAmount()
        {
            return solution.GetTurns();
        }

        public int GetYDimension()
        {
            return tileSet.GetYDimension();
        }

        /////////////////////////
        //     OBJECTS G&S     //
        /////////////////////////

        public Solution GetSolution()
        {
            return solution;
        }

        public void SetSolution(Solution solution)
        {
            this.solution = solution;
        }

        public Piece[] GetPieces()
        {
            return pieces;
        }

        public void SetPieces(Piece[] pieces)
        {
            this.pieces = pieces;
        }

        public TileSet GetTileSet()
        {
            return tileSet;
        }

        public void SetTileSet(TileSet tileSet)
        {
            this.tileSet = tileSet;
        }


        ////////////////////////
        //   HELPER METHODS   //
        ////////////////////////

        public PieceType[] GetPieceTypes()
        {
            List<PieceType> pieceTypes = new List<PieceType>();
            for (int i = 0; i < pieces.Length; i++)
            {
                string pieceTypeString = Presets.PieceType.mapByIdentifier[pieces[i].GetIdentifier()];
                pieceTypes.Add((PieceType)System.Enum.Parse(typeof(PieceType), pieceTypeString));
            }
            return pieceTypes.ToArray();
        }

        public void Print()
        {
            List<string> lines = MapService.ConvertToStringArray(this);
            foreach (string line in lines)
            {
                Logger.Log(line);
            }
        }
    }
}

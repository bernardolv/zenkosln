using System.Collections;
using System.Collections.Generic;
using Zenko;

namespace Zenko.Entities
{
    [System.Serializable]
    public class Solution
    {
        /////////////////////////
        //      PROPERTIES     //
        /////////////////////////


        int turns;
        V2Int[] moves; //maybe as V3?
        Conditions conditions;
        Piece[] pieces;

        /////////////////////////
        //     CONSTRUCTORS    //
        /////////////////////////

        public Solution Clone()
        {
            Solution clone = new Solution();
            clone.turns = turns;
            clone.moves = moves;
            clone.conditions = new Conditions();//TODO: CLONE
            if (pieces == null)
            {
                pieces = new Piece[0];
            }
            clone.pieces = new Piece[pieces.Length];
            for (int i = 0; i < pieces.Length; i++)
            {
                clone.pieces[i] = pieces[i].Clone();
            }

            return clone;
        }

        /////////////////////////
        //  GETTERS & SETTERS  //
        /////////////////////////

        public void SetTurns(int value)
        {
            turns = value;
        }

        public int GetTurns()
        {
            return turns;
        }

        public void SetMoves(V2Int[] value)
        {
            moves = value;
        }

        public V2Int[] GetMoves()
        {
            return moves;
        }
        public void SetConditions(Conditions value)
        {
            conditions = value;
        }

        public Conditions GetConditions()
        {
            return conditions;
        }
        public void SetPieces(Piece[] value)
        {
            pieces = value;
        }

        public Piece[] GetPieces()
        {
            return pieces;
        }

        /////////////////////////
        //    HELPER METHODS   //
        /////////////////////////


        public void Print()
        {
            string line = "";
            foreach (Piece piece in pieces)
            {
                line += piece.GetIdentifier() + " " + piece.GetX() + "" + piece.GetY() + " ";
            }
            line += "T" + turns;
            Logger.Log(line);
        }
        public string GetAsString()
        {
            string line = "";
            foreach (Piece piece in pieces)
            {
                line += piece.GetIdentifier() + piece.GetX() + "" + piece.GetY() + " ";
            }
            line += "T" + turns;

            return line;
        }
        public void PrintMoves()
        {
            foreach (V2Int move in moves)
            {
                Logger.Log(move);
            }
        }
    }

}

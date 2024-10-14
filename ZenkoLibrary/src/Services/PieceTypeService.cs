using System.Collections.Generic;
using Zenko;
using System;
using System.Linq;

namespace Zenko.Services
{
    public class PieceTypeService
    {

        public static List<PieceType> GetAllPieceTypes()
        {
            List<PieceType> possiblePieces = new List<PieceType>();

            foreach (PieceType piece in Enum.GetValues(typeof(PieceType)))
            {
                possiblePieces.Add(piece);
            }
            return possiblePieces;
        }

        public static List<List<PieceType>> GetAllPieceTypeCombinations(List<PieceType> possiblePieces, int k)
        {
            List<List<PieceType>> pieceCombos = new List<List<PieceType>>();
            List<PieceType> currentCombo = new List<PieceType>();


            MakeComboUtil(possiblePieces.Count, 1, k);

            return pieceCombos;

            void MakeComboUtil(int curN, int left, int curK)
            {
                // Pushing this vector to a vector of vector
                if (curK == 0)
                {
                    List<PieceType> newCombo = new List<PieceType>(currentCombo);
                    pieceCombos.Add(newCombo);
                    return;
                }

                // i iterates from left to n. First time
                // left will be 1
                for (int i = left; i <= curN; ++i)
                {
                    currentCombo.Add(possiblePieces[i - 1]);
                    MakeComboUtil(curN, i, curK - 1);

                    currentCombo.RemoveAt(currentCombo.Count - 1);
                }
            }
        }

        public static List<List<PieceType>> GetPossiblePieceCombinations(PieceType[] pieces, int k)
        {
            List<List<PieceType>> answer = new List<List<PieceType>>();
            List<PieceType> temp = new List<PieceType>();
            int n = pieces.Length;
            MakeComboUtil(n, 1, k);
            void MakeComboUtil(int curN, int left, int curK)
            {
                // Pushing this vector to a vector of vector
                if (curK == 0)
                {
                    List<PieceType> newCombo = new List<PieceType>(temp);

                    if (AnswerContains(newCombo))
                    {
                        return;
                    }
                    answer.Add(newCombo);
                    // Debug.Log(temp);
                    string printInt = "";
                    for (int i = 0; i < temp.Count; i++)
                    {
                        printInt += temp[i].ToString();
                        // Console.Write(temp[i] + " ");
                    }
                    // Debug.Log(printInt);
                    // Console.WriteLine();
                    return;
                }

                // i iterates from left to n. First time
                // left will be 1
                for (int i = left; i <= curN; ++i)
                {
                    temp.Add(pieces[i - 1]);
                    MakeComboUtil(curN, i + 1, curK - 1);

                    // Popping out last inserted element
                    // from the vector
                    // if (curK != 1)
                    temp.RemoveAt(temp.Count - 1);
                }
            }
            bool AnswerContains(List<PieceType> pieceCombo)
            {
                foreach (List<PieceType> currentPieceCombo in answer)
                {
                    if (currentPieceCombo.SequenceEqual(pieceCombo))
                    {
                        // Debug.Log("Already there");
                        return true;
                    }
                }
                return false;

            }
            return answer;
        }

        public static List<List<PieceType>> Filter(List<List<PieceType>> combos, List<List<PieceType>> exclusions)
        {
            if (exclusions.Count == 0)
            {
                return combos;
            }
            List<List<PieceType>> filteredCombos = new List<List<PieceType>>(combos);
            foreach (List<PieceType> combo in combos)
            {
                foreach (List<PieceType> exclusion in exclusions)
                {
                    // validate exclusion has Pieces
                    if (exclusion.Count == 0)
                    {
                        continue;
                    }
                    List<PieceType> exclusionCache = new List<PieceType>(exclusion);
                    foreach (PieceType comboPiece in combo)
                    {
                        if (exclusion.Contains(comboPiece))
                        {
                            exclusionCache.Remove(comboPiece);
                        }
                    }
                    int matchCount = 0;
                    foreach (PieceType piece in exclusionCache)
                    {
                        if (combo.Contains(piece))
                        {
                            matchCount++;
                            break;
                        }
                    }
                    // Debug.Log(exclusionCache.Count + " " + matchCount);
                    if (exclusionCache.Count == 0 && matchCount == 0)
                    {
                        filteredCombos.Remove(combo);
                        // continue;
                    }
                    // filteredCombos.Add(combo);
                    // if (exclusionCache.Count > 0 && matchCount == 0)
                    // {
                    //     filteredCombos.Add(combo);
                    // }
                }
            }
            return filteredCombos;
        }
        public static void PrintCombos(List<List<PieceType>> pieceCombos)
        {
            foreach (List<PieceType> combo in pieceCombos)
            {
                string t = "";
                foreach (PieceType pieceType in combo)
                {
                    t += pieceType + ",";
                }
                Logger.Log(t);
            }
        }
        public static void PrintPieces(List<PieceType> pieces)
        {
            string t = "";
            foreach (PieceType pieceType in pieces)
            {
                t += pieceType + ",";
            }
            Logger.Log(t);
        }
    }
}


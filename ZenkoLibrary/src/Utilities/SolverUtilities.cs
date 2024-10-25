using System.Collections.Generic;
using Zenko.Entities;
using System.Linq;



namespace Zenko.Utilities
{
    public static class SolverUtilities
    {
        /// <summary>
        /// Returns a List of Dictionarys where the key is the position and the value is the piece type
        /// Each of the Dictionarys represents where the pieces should be placed in the board
        /// It should cover all possible combinations of piece placements on ice tiles in the tileset
        /// </summary>
        /// <param name="pieceTypes"></param>
        /// <param name="tileSet"></param>
        /// <param name="piecesAmount"></param>
        /// <returns></returns>
        /// TODO: recursion
        public static List<Dictionary<V2Int, string>> GetPiecePositionCombinations(PieceType[] pieceTypes, TileSet tileSet, int piecesAmount)
        {
            List<Dictionary<V2Int, string>> result = new List<Dictionary<V2Int, string>>();
            if (piecesAmount == 0)
            {
                result.Add(new Dictionary<V2Int, string>());
                return result;
            }

            if (piecesAmount == 1)
            {
                List<V2Int> candidatePositions = TileSetUtilities.GetCandidateTilePositions(tileSet);
                PieceType[] uniquePieceTypes = pieceTypes.Distinct().ToArray();
                foreach (PieceType pieceType in uniquePieceTypes)
                {
                    foreach (V2Int icePosition in TileSetUtilities.GetCandidateTilePositions(tileSet))
                    {
                        Dictionary<V2Int, string> combo = new Dictionary<V2Int, string>();
                        combo.Add(icePosition, Zenko.Presets.PieceType.mapByType[pieceType.ToString()]);
                        result.Add(combo);
                    }
                }
            }

            if (piecesAmount == 2)
            {
                List<V2Int> candidatePositions = TileSetUtilities.GetCandidateTilePositions(tileSet);
                List<List<PieceType>> pieceTypeCombos = GetPossiblePieceCombinations(pieceTypes, 2);
                foreach (List<PieceType> pieceTypeCombo in pieceTypeCombos)
                {
                    for (int i = 0; i < candidatePositions.Count; i++)
                    {
                        //if piece is same as previous, start higher up to avoid repeat maps
                        int j = 0;
                        if (pieceTypeCombo[0] == pieceTypeCombo[1])
                        {
                            j = i + 1;
                        }

                        for (; j < candidatePositions.Count; j++)
                        {
                            //this happens when pieces are not the same type
                            //otherwise it doesnt since j = i+1 so it would be out of scope when i+1 = candidatePositions.Count (last i iteration)
                            if (i == j)
                            {
                                continue;
                            }

                            Dictionary<V2Int, string> combo = new Dictionary<V2Int, string>();
                            combo.Add(candidatePositions[i], Zenko.Presets.PieceType.mapByType[pieceTypeCombo[0].ToString()]);
                            combo.Add(candidatePositions[j], Zenko.Presets.PieceType.mapByType[pieceTypeCombo[1].ToString()]);
                            result.Add(combo);
                        }
                    }
                }
            }

            if (piecesAmount == 3)
            {
                List<V2Int> candidatePositions = TileSetUtilities.GetCandidateTilePositions(tileSet);
                List<List<PieceType>> pieceTypeCombos = GetPossiblePieceCombinations(pieceTypes, 3);
                foreach (List<PieceType> pieceTypeCombo in pieceTypeCombos)
                {
                    for (int i = 0; i < candidatePositions.Count; i++)
                    {
                        //if piece is same as previous, start higher up to avoid repeat maps
                        int j = 0;
                        if (pieceTypeCombo[0] == pieceTypeCombo[1])
                        {
                            j = i + 1;
                        }

                        for (; j < candidatePositions.Count; j++)
                        {
                            //this happens when pieces are not the same type
                            //otherwise it doesnt since j = i+1 so it would be out of scope when i+1 = candidatePositions.Count (last i iteration)
                            if (i == j)
                            {
                                continue;
                            }

                            //if piece is same as previous, start higher up to avoid repeat maps
                            int k = 0;
                            if (pieceTypeCombo[1] == pieceTypeCombo[2])
                            {
                                k = j + 1;
                            }

                            for (; k < candidatePositions.Count; k++)
                            {
                                //this happens when pieces are not the same type
                                //otherwise it doesnt since j = i+1 so it would be out of scope when i+1 = candidatePositions.Count (last i iteration)
                                if (j == k)
                                {
                                    continue;
                                }

                                //this happens when pieces are not the same type
                                //otherwise it doesnt since j = i+1 so it would be out of scope when i+1 = candidatePositions.Count (last i iteration)
                                if (i == k)
                                {
                                    continue;
                                }

                                Dictionary<V2Int, string> combo = new Dictionary<V2Int, string>();
                                combo.Add(candidatePositions[i], Zenko.Presets.PieceType.mapByType[pieceTypeCombo[0].ToString()]);
                                combo.Add(candidatePositions[j], Zenko.Presets.PieceType.mapByType[pieceTypeCombo[1].ToString()]);
                                combo.Add(candidatePositions[k], Zenko.Presets.PieceType.mapByType[pieceTypeCombo[2].ToString()]);
                                result.Add(combo);
                            }
                        }
                    }
                }
            }

            if (piecesAmount == 4)
            {
                List<V2Int> candidatePositions = TileSetUtilities.GetCandidateTilePositions(tileSet);
                List<List<PieceType>> pieceTypeCombos = GetPossiblePieceCombinations(pieceTypes, 4);
                foreach (List<PieceType> pieceTypeCombo in pieceTypeCombos)
                {
                    for (int i = 0; i < candidatePositions.Count; i++)
                    {
                        //if piece is same as previous, start higher up to avoid repeat maps
                        int j = 0;
                        if (pieceTypeCombo[0] == pieceTypeCombo[1])
                        {
                            j = i + 1;
                        }

                        for (; j < candidatePositions.Count; j++)
                        {
                            //this happens when pieces are not the same type
                            //otherwise it doesnt since j = i+1 so it would be out of scope when i+1 = candidatePositions.Count (last i iteration)
                            if (i == j)
                            {
                                continue;
                            }

                            //if piece is same as previous, start higher up to avoid repeat maps
                            int k = 0;
                            if (pieceTypeCombo[1] == pieceTypeCombo[2])
                            {
                                k = j + 1;
                            }

                            for (; k < candidatePositions.Count; k++)
                            {

                                //this happens when pieces are not the same type
                                //otherwise it doesnt since j = i+1 so it would be out of scope when i+1 = candidatePositions.Count (last i iteration)
                                if (j == k)
                                {
                                    continue;
                                }

                                //this happens when pieces are not the same type
                                //otherwise it doesnt since j = i+1 so it would be out of scope when i+1 = candidatePositions.Count (last i iteration)
                                if (i == k)
                                {
                                    continue;
                                }

                                //if piece is same as previous, start higher up to avoid repeat maps
                                int l = 0;
                                if (pieceTypeCombo[2] == pieceTypeCombo[3])
                                {
                                    l = k + 1;
                                }

                                for (; l < candidatePositions.Count; l++)
                                {

                                    //this happens when pieces are not the same type
                                    //otherwise it doesnt since j = i+1 so it would be out of scope when i+1 = candidatePositions.Count (last i iteration)
                                    if (i == l)
                                    {
                                        continue;
                                    }

                                    //this happens when pieces are not the same type
                                    //otherwise it doesnt since j = i+1 so it would be out of scope when i+1 = candidatePositions.Count (last i iteration)
                                    if (j == l)
                                    {
                                        continue;
                                    }

                                    //this happens when pieces are not the same type
                                    //otherwise it doesnt since j = i+1 so it would be out of scope when i+1 = candidatePositions.Count (last i iteration)
                                    if (k == l)
                                    {
                                        continue;
                                    }


                                    Dictionary<V2Int, string> combo = new Dictionary<V2Int, string>();
                                    combo.Add(candidatePositions[i], Zenko.Presets.PieceType.mapByType[pieceTypeCombo[0].ToString()]);
                                    combo.Add(candidatePositions[j], Zenko.Presets.PieceType.mapByType[pieceTypeCombo[1].ToString()]);
                                    combo.Add(candidatePositions[k], Zenko.Presets.PieceType.mapByType[pieceTypeCombo[2].ToString()]);
                                    combo.Add(candidatePositions[l], Zenko.Presets.PieceType.mapByType[pieceTypeCombo[3].ToString()]);
                                    result.Add(combo);
                                }
                            }
                        }
                    }
                }
            }

            return result;
        }


        /// TODO: Test struct{Listv2int Liststring} vs Dictionary memory
        //TODO: Test struct{litv2int liststring} vs list<struct{v2int string}>
        public static List<Combo> Test(PieceType[] pieceTypes, TileSet tileSet, int piecesAmount)
        {
            List<Combo> result = new List<Combo>();
            if (piecesAmount == 0)
            {
                result.Add(new Combo(new List<V2Int>(), new List<string>()));
                return result;
            }

            if (piecesAmount == 1)
            {
                List<V2Int> candidatePositions = TileSetUtilities.GetCandidateTilePositions(tileSet);
                PieceType[] uniquePieceTypes = pieceTypes.Distinct().ToArray();
                foreach (PieceType pieceType in uniquePieceTypes)
                {
                    foreach (V2Int icePosition in TileSetUtilities.GetCandidateTilePositions(tileSet))
                    {
                        result.Add(new Combo(new List<V2Int>() { icePosition }, new List<string> { Zenko.Presets.PieceType.mapByType[pieceType.ToString()] }));
                    }
                }
            }

            if (piecesAmount == 2)
            {
                List<V2Int> candidatePositions = TileSetUtilities.GetCandidateTilePositions(tileSet);
                List<List<PieceType>> pieceTypeCombos = GetPossiblePieceCombinations(pieceTypes, 2);
                foreach (List<PieceType> pieceTypeCombo in pieceTypeCombos)
                {
                    for (int i = 0; i < candidatePositions.Count; i++)
                    {
                        //if piece is same as previous, start higher up to avoid repeat maps
                        int j = 0;
                        if (pieceTypeCombo[0] == pieceTypeCombo[1])
                        {
                            j = i + 1;
                        }

                        for (; j < candidatePositions.Count; j++)
                        {
                            //this happens when pieces are not the same type
                            //otherwise it doesnt since j = i+1 so it would be out of scope when i+1 = candidatePositions.Count (last i iteration)
                            if (i == j)
                            {
                                continue;
                            }

                            List<V2Int> positionsList = new List<V2Int>() { candidatePositions[i], candidatePositions[j] };
                            List<string> pieceTypeList = new List<string>() { Zenko.Presets.PieceType.mapByType[pieceTypeCombo[0].ToString()], Zenko.Presets.PieceType.mapByType[pieceTypeCombo[1].ToString()] };

                            result.Add(new Combo(positionsList, pieceTypeList));
                        }
                    }
                }
            }

            if (piecesAmount == 3)
            {
                List<V2Int> candidatePositions = TileSetUtilities.GetCandidateTilePositions(tileSet);
                List<List<PieceType>> pieceTypeCombos = GetPossiblePieceCombinations(pieceTypes, 3);
                foreach (List<PieceType> pieceTypeCombo in pieceTypeCombos)
                {
                    for (int i = 0; i < candidatePositions.Count; i++)
                    {
                        //if piece is same as previous, start higher up to avoid repeat maps
                        int j = 0;
                        if (pieceTypeCombo[0] == pieceTypeCombo[1])
                        {
                            j = i + 1;
                        }

                        for (; j < candidatePositions.Count; j++)
                        {
                            //this happens when pieces are not the same type
                            //otherwise it doesnt since j = i+1 so it would be out of scope when i+1 = candidatePositions.Count (last i iteration)
                            if (i == j)
                            {
                                continue;
                            }

                            //if piece is same as previous, start higher up to avoid repeat maps
                            int k = 0;
                            if (pieceTypeCombo[1] == pieceTypeCombo[2])
                            {
                                k = j + 1;
                            }

                            for (; k < candidatePositions.Count; k++)
                            {
                                //this happens when pieces are not the same type
                                //otherwise it doesnt since j = i+1 so it would be out of scope when i+1 = candidatePositions.Count (last i iteration)
                                if (j == k)
                                {
                                    continue;
                                }

                                //this happens when pieces are not the same type
                                //otherwise it doesnt since j = i+1 so it would be out of scope when i+1 = candidatePositions.Count (last i iteration)
                                if (i == k)
                                {
                                    continue;
                                }

                                List<V2Int> positionsList = new List<V2Int>() { candidatePositions[i], candidatePositions[j], candidatePositions[k] };
                                List<string> pieceTypeList = new List<string>() { Zenko.Presets.PieceType.mapByType[pieceTypeCombo[0].ToString()], Zenko.Presets.PieceType.mapByType[pieceTypeCombo[1].ToString()], Zenko.Presets.PieceType.mapByType[pieceTypeCombo[2].ToString()] };
                                result.Add(new Combo(positionsList, pieceTypeList));
                            }
                        }
                    }
                }
            }

            if (piecesAmount == 4)
            {
                List<V2Int> candidatePositions = TileSetUtilities.GetCandidateTilePositions(tileSet);
                List<List<PieceType>> pieceTypeCombos = GetPossiblePieceCombinations(pieceTypes, 4);
                foreach (List<PieceType> pieceTypeCombo in pieceTypeCombos)
                {
                    for (int i = 0; i < candidatePositions.Count; i++)
                    {
                        //if piece is same as previous, start higher up to avoid repeat maps
                        int j = 0;
                        if (pieceTypeCombo[0] == pieceTypeCombo[1])
                        {
                            j = i + 1;
                        }

                        for (; j < candidatePositions.Count; j++)
                        {
                            //this happens when pieces are not the same type
                            //otherwise it doesnt since j = i+1 so it would be out of scope when i+1 = candidatePositions.Count (last i iteration)
                            if (i == j)
                            {
                                continue;
                            }

                            //if piece is same as previous, start higher up to avoid repeat maps
                            int k = 0;
                            if (pieceTypeCombo[1] == pieceTypeCombo[2])
                            {
                                k = j + 1;
                            }

                            for (; k < candidatePositions.Count; k++)
                            {

                                //this happens when pieces are not the same type
                                //otherwise it doesnt since j = i+1 so it would be out of scope when i+1 = candidatePositions.Count (last i iteration)
                                if (j == k)
                                {
                                    continue;
                                }

                                //this happens when pieces are not the same type
                                //otherwise it doesnt since j = i+1 so it would be out of scope when i+1 = candidatePositions.Count (last i iteration)
                                if (i == k)
                                {
                                    continue;
                                }

                                //if piece is same as previous, start higher up to avoid repeat maps
                                int l = 0;
                                if (pieceTypeCombo[2] == pieceTypeCombo[3])
                                {
                                    l = k + 1;
                                }

                                for (; l < candidatePositions.Count; l++)
                                {

                                    //this happens when pieces are not the same type
                                    //otherwise it doesnt since j = i+1 so it would be out of scope when i+1 = candidatePositions.Count (last i iteration)
                                    if (i == l)
                                    {
                                        continue;
                                    }

                                    //this happens when pieces are not the same type
                                    //otherwise it doesnt since j = i+1 so it would be out of scope when i+1 = candidatePositions.Count (last i iteration)
                                    if (j == l)
                                    {
                                        continue;
                                    }

                                    //this happens when pieces are not the same type
                                    //otherwise it doesnt since j = i+1 so it would be out of scope when i+1 = candidatePositions.Count (last i iteration)
                                    if (k == l)
                                    {
                                        continue;
                                    }


                                    List<V2Int> positionsList = new List<V2Int>() { candidatePositions[i], candidatePositions[j], candidatePositions[k], candidatePositions[l] };
                                    List<string> pieceTypeList = new List<string>() { Zenko.Presets.PieceType.mapByType[pieceTypeCombo[0].ToString()], Zenko.Presets.PieceType.mapByType[pieceTypeCombo[1].ToString()], Zenko.Presets.PieceType.mapByType[pieceTypeCombo[2].ToString()], Zenko.Presets.PieceType.mapByType[pieceTypeCombo[3].ToString()] };
                                    result.Add(new Combo(positionsList, pieceTypeList));
                                }
                            }
                        }
                    }
                }
            }

            return result;
        }

        //Eventually revisit
        //TODO: SHOULD RETURN SORTED AND NO DUPLICATES
        static List<List<PieceType>> GetPossiblePieceCombinations(PieceType[] pieces, int k)
        {
            List<List<PieceType>> answer = new List<List<PieceType>>();
            List<PieceType> temp = new List<PieceType>();
            int n = pieces.Length;
            MakeComboUtil(n, 1, k);
            void MakeComboUtil(int curN, int left, int curK)
            {
                //Last iteration
                if (curK == 0)
                {
                    List<PieceType> newCombo = new List<PieceType>(temp);

                    if (AnswerContains(newCombo))
                    {
                        return;
                    }

                    //TODO: SORT BY REPEATED?
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
                    // from the V
                    // if (curK != 1)
                    temp.RemoveAt(temp.Count - 1);
                }
            }
            bool AnswerContains(List<PieceType> pieceCombo)
            {
                foreach (List<PieceType> currentPieceCombo in answer)
                {
                    if (ScrambledEquals(currentPieceCombo, pieceCombo))
                    {
                        return true;
                    }
                }
                return false;

            }
            return answer;
        }

        //Check if lists match regardless of order
        public static bool ScrambledEquals<T>(IEnumerable<T> list1, IEnumerable<T> list2)
        {
            var cnt = new Dictionary<T, int>();
            foreach (T s in list1)
            {
                if (cnt.ContainsKey(s))
                {
                    cnt[s]++;
                }
                else
                {
                    cnt.Add(s, 1);
                }
            }
            foreach (T s in list2)
            {
                if (cnt.ContainsKey(s))
                {
                    cnt[s]--;
                }
                else
                {
                    return false;
                }
            }
            return cnt.Values.All(c => c == 0);
        }
    }
}
using System.Collections.Generic;
using Zenko.Entities;
using System.Linq;
using Zenko.Utilities;

namespace Zenko.Services
{
    //Gets all the available piece combos for the board
    public class ComboService
    {
        int[] initialIteratorPositions;
        int[] iterators;
        int currentPieceTypeComboIndex;
        List<V2Int> positions;
        List<List<PieceType>> pieceTypeCombos;


        public ComboService(Map map, int pieceAmountToUse)
        {
            positions = TileSetUtilities.GetCandidateTilePositions(map.GetTileSet());
            if (positions.Count < map.GetPieceTypes().Length)
            {
                throw new System.Exception("Cannot have more pieces than available positions");
            }
            pieceTypeCombos = PieceTypeService.GetPossiblePieceCombinations(map.GetPieceTypes(), pieceAmountToUse);
            iterators = new int[pieceAmountToUse];

            //initialize iterators ie. 0,1,2,3
            for (int i = 0; i < iterators.Length; i++)
            {
                iterators[i] = i;
            }

            currentPieceTypeComboIndex = 0;
        }

        public Combo GetCurrentCombo()
        {
            List<V2Int> comboPositions = new List<V2Int>();
            List<string> comboPieceTypes = new List<string>();
            for (int i = 0; i < iterators.Length; i++)
            {
                comboPositions.Add(positions[iterators[i]]);
                comboPieceTypes.Add(Zenko.Presets.PieceType.mapByType[pieceTypeCombos[currentPieceTypeComboIndex][i].ToString()]);
            }

            return new Combo(comboPositions, comboPieceTypes);
        }

        public bool TryNextRecursive(int k, out Combo combo)
        {
            combo = new Combo();

            //if iterator already at last it means previous one is calling it since it reached positions.Count
            if (iterators[k] == positions.Count)
            {
                // Logger.Log("Same");
                if (k == 0)
                {
                    throw new System.Exception("Should not be 0");
                }
                //initialize at beginning of list
                iterators[k] = 0;
                if (k != 0 && pieceTypeCombos[currentPieceTypeComboIndex][k] == pieceTypeCombos[currentPieceTypeComboIndex][k - 1])
                {
                    // Logger.Log("Increase");
                    iterators[k] = iterators[k - 1] + 1;
                }

                bool skipping = true;
                while (skipping)
                {
                    skipping = false;
                    //if same as any of the earlier iterators, then go to the next
                    for (int i = 0; i < k; i++)
                    {
                        if (iterators[i] == iterators[k])
                        {
                            // Logger.Log("IncreaseB1");
                            iterators[k]++;
                            skipping = true;
                        }
                    }
                }

            }
            else
            {
                //increase iterator by 1
                iterators[k]++;

                bool skipping = true;
                while (skipping)
                {
                    skipping = false;
                    //if same as any of the earlier iterators, then go to the next
                    for (int i = 0; i < k; i++)
                    {
                        if (iterators[i] == iterators[k])
                        {
                            // Logger.Log("IncreaseB2");
                            iterators[k]++;
                            skipping = true;
                        }
                    }
                }
            }

            if (iterators[k] == positions.Count)
            {
                //if iterator was the first, then we reached the end
                if (k == 0)
                {
                    //if it was the last pieceTypeCombo then return false
                    if (currentPieceTypeComboIndex == pieceTypeCombos.Count - 1)
                    {
                        // Logger.Log("A");
                        return false;
                    }
                    else //go to the next piecetype combo and return the first entry
                    {
                        currentPieceTypeComboIndex++;

                        //initialize iterators ie. 0,1,2,3
                        for (int i = 0; i < iterators.Length; i++)
                        {
                            iterators[i] = i;
                        }

                        combo = GetCurrentCombo();
                        // Logger.Log("B");

                        return true;
                    }
                }
                // Logger.Log("RECURSE" + iterators[0] + iterators[1]);
                return TryNextRecursive(k - 1, out combo);
            }

            //if not the last iterator, then make sure to modify the next ones accordingly (since if we got here it means we reached the end in n of the next ones)
            if (k != iterators.Length - 1)
            {
                return TryNextRecursive(k + 1, out combo);
            }

            combo = GetCurrentCombo();
            // Logger.Log("C");

            return true;
        }
    }
}
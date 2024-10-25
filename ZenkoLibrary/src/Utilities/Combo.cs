using System.Collections.Generic;
using Zenko.Entities;
using System.Linq;


public struct Combo
{
    public List<V2Int> positions;
    public List<string> pieceTypes;
    public Combo(List<V2Int> positions, List<string> pieceTypes)
    {
        this.positions = positions;
        this.pieceTypes = pieceTypes;

        if (positions.Count != pieceTypes.Count)
        {
            throw new System.Exception("Combo mismatch count of positions and piecetypes");
        }
    }

    public bool Equals(Combo combo)
    {
        if (positions.Count != combo.positions.Count)
        {
            return false;
        }
        if (pieceTypes.Count != combo.pieceTypes.Count)
        {
            return false;
        }
        for (int i = 0; i < positions.Count; i++)
        {
            if (positions[i] != combo.positions[i])
            {
                return false;
            }
        }
        for (int i = 0; i < pieceTypes.Count; i++)
        {
            if (pieceTypes[i] != combo.pieceTypes[i])
            {
                return false;
            }
        }
        return true;
    }

    public string GetPositionsString()
    {
        string result = "";
        foreach (V2Int position in positions)
        {
            result += "(" + position.x + "," + position.y + "),";
        }
        return result;
    }
}
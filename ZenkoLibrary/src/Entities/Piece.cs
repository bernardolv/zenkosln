using System.Collections;
using System.Collections.Generic;
using Zenko.Presets;
using Zenko.Utilities;

namespace Zenko.Entities
{
    [System.Serializable]
    public class Piece
    {
        /////////////////////////
        //      PROPERTIES     //
        /////////////////////////

        string identifier;
        int y;
        int x;
        bool placed;
        /////////////////////////
        //     CONSTRUCTOR     //
        /////////////////////////

        public Piece Clone()
        {
            Piece clone = new Piece();
            clone.identifier = identifier;
            clone.y = y;
            clone.x = x;
            return clone;
        }

        public Piece()
        {

        }

        public Piece(int x, int y, string identifier)
        {
            SetIdentifier(identifier);
            SetX(x);
            SetY(y);
        }

        public Piece(V2Int position, string identifier)
        {
            SetX(position.x);
            SetY(position.y);
            SetIdentifier(identifier);
        }


        /////////////////////////
        // GETTERS AND SETTERS //
        /////////////////////////

        public V2Int GetPosition()
        {
            return new V2Int(x, y);
        }

        public PieceType GetPieceType()
        {
            return (PieceType)System.Enum.Parse(typeof(PieceType), Presets.PieceType.mapByIdentifier[identifier]);
        }

        public string GetIdentifier()
        {
            return identifier;
        }
        public void SetIdentifier(string identifier)
        {
            this.identifier = identifier;
        }

        public int GetX()
        {
            return x;
        }

        public void SetX(int x)
        {
            this.x = x;
        }

        public int GetY()
        {
            return y;
        }

        public void SetY(int y)
        {
            this.y = y;
        }

        public bool IsPlaced()
        {
            return this.placed;
        }

        public void SetPlaced(bool placed)
        {
            this.placed = placed;
        }

        public V2Int GetBoardCoords()
        {
            return new V2Int(x, y);
        }
        public string GetIdentifierName()
        {
            return Presets.PieceType.mapByIdentifier[GetIdentifier()];
        }
        public bool IsPortal()
        {
            switch (identifier)
            {
                case "PL":
                case "PR":
                case "PU":
                case "PD":
                    return true;
                default:
                    return false;
            }
        }
        public bool IsIcarus()
        {
            switch (identifier)
            {
                case "L":
                case "R":
                case "U":
                case "D":
                    return true;
                default:
                    return false;
            }
        }
        public string GetIdentifierDirection()
        {
            return PieceUtilities.GetIdentifierDirection(identifier);
        }
        public V2Int GetIdentifierDirectionVector()
        {
            switch (this.GetIdentifier())
            {
                case "PL":
                case "L":
                    return V2Int.left;
                case "PR":
                case "R":
                    return V2Int.right;
                case "PU":
                case "U":
                    return V2Int.down;
                case "PD":
                case "D":
                    return V2Int.up;
                default:
                    throw new System.Exception("Incorrect portal direction" + GetIdentifier());
            }
        }
        public string GetSeedType()
        {
            switch (identifier)
            {
                case "p":
                    return "Wall";

                case "l":
                    return "Left";

                case "r":
                    return "Right";

                case "u":
                    return "Up";

                case "d":
                    return "Down";

                default:
                    throw new System.Exception("No valid seed type");
            }
        }
    }
}

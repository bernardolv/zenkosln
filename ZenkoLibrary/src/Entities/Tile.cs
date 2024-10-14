using System.Collections;
using System.Collections.Generic;

namespace Zenko.Entities
{
    public class Tile
    {
        /////////////////////////
        //      PROPERTIES     //
        /////////////////////////

        string identifier;
        int x;
        int y;


        /////////////////////////
        //  HELPER PROPERTIES  //
        /////////////////////////

        string pieceType;


        /////////////////////////
        //  DYNAMIC PROPERTIES //
        /////////////////////////

        string tileType;
        bool isTaken; // used for gameboard stuff, when placing pieces, should probably be used everywhere eventually, right now only view/dragger uses it
        string seedType;
        string isSideways;
        string portalType;


        /////////////////////////
        //     CONSTRUCTORS    //
        /////////////////////////

        public Tile()
        {

        }

        public Tile Clone()
        {
            Tile clone = new Tile();
            clone.identifier = this.identifier;
            clone.x = this.x;
            clone.y = this.y;
            clone.pieceType = this.pieceType;
            clone.tileType = this.tileType;
            clone.isTaken = this.isTaken;
            clone.seedType = this.seedType;
            clone.isSideways = this.isSideways;
            clone.portalType = this.portalType;

            return clone;
        }


        /////////////////////////
        // GETTERS AND SETTERS //
        /////////////////////////

        public string GetIdentifier()
        {
            return identifier;
        }
        public void SetIdentifier(string value)
        {
            identifier = value;
        }

        public int GetX()
        {
            return x;
        }
        public void SetX(int value)
        {
            x = value;
        }

        public int GetY()
        {
            return y;
        }
        public void SetY(int value)
        {
            y = value;
        }

        public string GetPieceType()
        {
            return pieceType;
        }
        public void SetPieceType(string pieceType)
        {
            this.pieceType = pieceType;
        }

        public string GetTileType()
        {
            return tileType;
        }
        public void SetTileType(string value)
        {
            if (value == "Left" || value == "Right" || value == "Up" || value == "Down")
            {
                throw new System.Exception("Deprecated tile type");
            }
            tileType = value;
        }

        public bool GetIsTaken()
        {
            return isTaken;
        }
        public void SetIsTaken(bool value)
        {
            isTaken = value;
        }

        public string GetSeedType()
        {
            return seedType;
        }
        public void SetSeedType(string value)
        {
            seedType = value;
        }

        public string GetSideways()
        {
            return isSideways;
        }
        public void SetSideways(string direction)
        {
            isSideways = direction;
        }

        public string GetPortalType()
        {
            return portalType;
        }
        public void SetPortalType(string direction)
        {
            portalType = direction;
        }
        public V2Int GetPosition()
        {
            return new V2Int(x, y);
        }
    }
}

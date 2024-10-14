using System.Collections;
using System.Collections.Generic;
using Zenko.Entities;

namespace Zenko.Utilities
{
    public class PieceUtilities
    {
        //returns an ice position for a piece
        //makes sure the ice position is NOT a valid piece position
        public static V2Int GetRandomPiecePosition(Piece piece, TileSet tileSet, Piece[] pieces)
        {
            //Populate candidate positions with ice tiles
            List<V2Int> candidatePositions = new List<V2Int>();
            for (int x = 0; x < tileSet.GetXDimension(); x++)
            {
                for (int y = 0; y < tileSet.GetYDimension(); y++)
                {
                    if (tileSet.GetTile(x, y).GetTileType() == "Ice")
                    {
                        candidatePositions.Add(new V2Int(x, y));
                    }
                }
            }

            //Remove any valid hint positions from candidate tiles
            foreach (Piece pieceToCheck in pieces)
            {
                if (pieceToCheck == piece || piece.GetIdentifier() == pieceToCheck.GetIdentifier())
                {
                    candidatePositions.Remove(piece.GetPosition());
                }
            }

            //Return a random position
            System.Random random = new System.Random();
            return candidatePositions[random.Next(0, candidatePositions.Count)];
        }
        public static string GetIdentifierDirection(string identifier)
        {
            switch (identifier)
            {
                case "L":
                    return "Left";
                case "U":
                    return "Up";
                case "R":
                    return "Right";
                case "D":
                    return "Down";
                case "l":
                    return "Left";
                case "u":
                    return "Up";
                case "r":
                    return "Right";
                case "d":
                    return "Down";
                case "PL":
                    return "Left";
                case "PU":
                    return "Up";
                case "PR":
                    return "Right";
                case "PD":
                    return "Down";
                default:
                    return null;
            }
        }

        public static int GetRotation(string direction)
        {
            switch (direction)
            {
                case "Left":
                    return 270;
                case "Up":
                    return 0;
                case "Right":
                    return 90;
                case "Down":
                    return 180;
            }
            return 0;
        }

        public static Piece GetMatchingPortal(TileSet tileSet, V2Int portalEntrancePosition)
        {
            //Get matching portal
            Piece matchingPortal = null;
            for (int i = 0; i < tileSet.GetPlacedPieces().Count; i++)
            {
                PieceType pieceType = tileSet.GetPlacedPieces()[i].GetPieceType();
                if (pieceType == PieceType.PortalLeft || pieceType == PieceType.PortalRight || pieceType == PieceType.PortalUp || pieceType == PieceType.PortalDown)
                {
                    V2Int piecePosition = portalEntrancePosition;
                    if (tileSet.GetPlacedPiecePositions()[i] != piecePosition)
                    {
                        matchingPortal = tileSet.GetPlacedPieces()[i];
                    }
                }
            }

            if (matchingPortal == null)
            {
                return null;
            }

            return matchingPortal;
        }

        public static bool IsPortalPortable(V2Int playerDirection, Piece entrancePortal, TileSet tileSet)
        {
            //First check if portal and player direction are opposite
            if (playerDirection != -entrancePortal.GetIdentifierDirectionVector())
            {
                return false;
            }
            //Get matching portal
            Piece matchingPortal = null;
            for (int i = 0; i < tileSet.GetPlacedPieces().Count; i++)
            {
                PieceType pieceType = tileSet.GetPlacedPieces()[i].GetPieceType();
                if (pieceType == PieceType.PortalLeft || pieceType == PieceType.PortalRight || pieceType == PieceType.PortalUp || pieceType == PieceType.PortalDown)
                {
                    if (tileSet.GetPlacedPiecePositions()[i] != entrancePortal.GetPosition())
                    {
                        matchingPortal = tileSet.GetPlacedPieces()[i];
                    }
                }
            }
            if (matchingPortal == null)
            {
                return false;
            }

            Tile nextTile = tileSet.GetTile(matchingPortal.GetPosition() + matchingPortal.GetIdentifierDirectionVector());
            switch (nextTile.GetTileType())
            {
                case "Wall":
                    return false;
                case "Portal":
                    Piece nextTilePortal = tileSet.GetPieceAt(matchingPortal.GetPosition() + matchingPortal.GetIdentifierDirectionVector());
                    //TODO: Find portal piece at nextTile
                    //TODO: Need to add Tile.Piece
                    if (matchingPortal.GetIdentifierDirectionVector() == -nextTilePortal.GetIdentifierDirectionVector())
                    {
                        //Logger.LogError("CAREFUL, POTENTIAL INFINITY");
                        return false;
                    }
                    else
                        return true;
                case "Start":
                    return false;
                default:
                    return true;
            }
        }
    }
}

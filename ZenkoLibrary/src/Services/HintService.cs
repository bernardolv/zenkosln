using System.Collections.Generic;
using Zenko;

using Zenko.Entities;

namespace Zenko.Services
{
    public class HintService
    {
        public static void GetHintData(Map map, out HintType hintType, out Dictionary<Piece, V2Int> targetPositions)
        {
            hintType = HintType.None;

            //Store found pieces
            targetPositions = new Dictionary<Piece, V2Int>();

            if (map.GetTileSet().GetPlacedPieces().Count != map.GetPieces().Length)
            {
                hintType = HintType.Unplaced;
            }
            else if (AllPiecesAtValidHintSpot(map.GetTileSet().GetPlacedPieces(), map))
            {
                hintType = HintType.AllGood;
            }
            else if (HasPieceWithValidTargetSpot(map.GetTileSet().GetPlacedPieces(), map))
            {
                hintType = HintType.Single;
                GetHintPiece(map.GetTileSet().GetPlacedPieces(), map, out Piece piece, out V2Int position);
                targetPositions.Add(piece, new V2Int((int)position.x, (int)position.y));
            }
            else
            {
                hintType = HintType.Swap;

                List<V2Int> selectedHintPositions = new List<V2Int>();
                List<Piece> placedPieces = map.GetTileSet().GetPlacedPieces();

                for (int i = 0; i < map.GetTileSet().GetPlacedPieces().Count; i++)//3
                {
                    Piece piece = placedPieces[i];
                    if (!IsPieceAtValidHintPosition(piece, map))
                    {
                        V2Int posToGo = GetValidPosition(selectedHintPositions, piece.GetIdentifier(), map);
                        selectedHintPositions.Add(posToGo);

                        targetPositions.Add(piece, new V2Int((int)posToGo.x, (int)posToGo.y));
                    }
                }
            }
        }

        public static HintType GetHintType(Map map)
        {
            //could iterate over map pieces and if one is placed = false then return unplaced
            if (map.GetTileSet().GetPlacedPieces().Count != map.GetPieces().Length)
            {
                return HintType.Unplaced;
            }
            else if (AllPiecesAtValidHintSpot(map.GetTileSet().GetPlacedPieces(), map))
            {
                return HintType.AllGood;
            }
            else if (HasPieceWithValidTargetSpot(map.GetTileSet().GetPlacedPieces(), map))
            {
                return HintType.Single;
            }
            else
            {
                return HintType.Swap;
            }
        }

        public static bool AllPiecesAtValidHintSpot(List<Piece> placedPieces, Map map)
        {
            //loop through all pieces,if any not at valid place, return false
            for (int i = 0; i < placedPieces.Count; i++)
            {
                if (!IsPieceAtValidHintPosition(placedPieces[i], map))
                {
                    return false;
                }
            }
            //if all pieces at valid positions, return true
            return true;
        }

        public static bool IsPieceAtValidHintPosition(Piece piece, Map map)
        {
            //loop through all hints and see if theres a type/position match with current piece
            for (int i = 0; i < map.GetSolution().GetPieces().Length; i++)
            {
                Piece otherPiece = map.GetSolution().GetPieces()[i];
                if (piece.GetIdentifier() == otherPiece.GetIdentifier() && otherPiece.GetPosition() == piece.GetPosition())
                {
                    return true;
                }
            }
            return false;
        }

        public static bool HasPieceWithValidTargetSpot(List<Piece> placedPieces, Map map)
        {
            for (int i = 0; i < placedPieces.Count; i++)
            {
                Piece piece = placedPieces[i];
                //second pass, to correct the first one found in wrong place.
                if (!IsPieceAtValidHintPosition(piece, map))
                {
                    //checks if target is occupied as well.
                    V2Int posToGo = LookforPosition(piece.GetIdentifier(), map);
                    if (posToGo != new V2Int(0, 0))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        static V2Int LookforPosition(string type, Map map)
        {
            for (int i = 0; i < map.GetSolution().GetPieces().Length; i++)
            {
                Piece piece = map.GetSolution().GetPieces()[i];
                if (type == piece.GetIdentifier())
                {
                    if (!map.GetTileSet().GetTile(piece.GetX(), piece.GetY()).GetIsTaken())
                    {
                        return piece.GetPosition();
                    }
                }
            }
            return new V2Int(0, 0);
        }

        //Phase dragger out
        public static bool GetHintPiece(List<Piece> placedPieces, Map map, out Piece piece, out V2Int posToGo)
        {
            piece = null;
            posToGo = V2Int.zero;
            for (int i = 0; i < placedPieces.Count; i++)
            {
                piece = placedPieces[i];
                //second pass, to correct the first one found in wrong place.
                if (!IsPieceAtValidHintPosition(piece, map))
                {
                    //checks if target is occupied as well.
                    posToGo = LookforPosition(piece.GetIdentifier(), map);
                    if (posToGo != new V2Int(0, 0))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static V2Int GetValidPosition(List<V2Int> selectedHintPositions, string pieceIdentifier, Map map)
        {
            Piece[] solutionPieces = map.GetSolution().GetPieces();

            for (int i = 0; i < solutionPieces.Length; i++)
            {
                if (pieceIdentifier == solutionPieces[i].GetIdentifier())
                {
                    V2Int position = solutionPieces[i].GetPosition();

                    //Get Piece Identifier currently at target position
                    string targetPieceIdentifier = "";
                    foreach (Piece piece in map.GetTileSet().GetPlacedPieces())
                    {
                        if (piece.GetPosition() == position)
                        {
                            targetPieceIdentifier = piece.GetIdentifier();
                            Logger.Log(piece.GetIdentifier());
                            break;
                        }
                    }

                    if (!selectedHintPositions.Contains(position))
                    {
                        if (targetPieceIdentifier != pieceIdentifier)
                        {
                            return map.GetSolution().GetPieces()[i].GetPosition();
                        }
                    }
                }
            }
            return V2Int.zero;
        }
    }
}


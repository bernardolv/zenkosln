using System.Collections.Generic;
using Zenko.Entities;
using Zenko.Services;
using System;

namespace Zenko.Controllers
{
    public static class HintController
    {
        public static Action<HintType, Map, List<Piece>> OnHint;



        public static void Hint(Map map)
        {
            //Store found pieces
            Dictionary<Piece, V2Int> targetPositions = new Dictionary<Piece, V2Int>();

            if (map.GetTileSet().GetPlacedPieces().Count != map.GetPieces().Length)
            {
            }
            else if (HintService.AllPiecesAtValidHintSpot(map.GetTileSet().GetPlacedPieces(), map))
            {
            }
            else if (HintService.HasPieceWithValidTargetSpot(map.GetTileSet().GetPlacedPieces(), map))
            {
                HintService.GetHintPiece(map.GetTileSet().GetPlacedPieces(), map, out Piece piece, out V2Int position);
                targetPositions.Add(piece, new V2Int((int)position.x, (int)position.y));
            }
            else
            {
                List<V2Int> selectedHintPositions = new List<V2Int>();
                List<Piece> placedPieces = map.GetTileSet().GetPlacedPieces();

                for (int i = 0; i < map.GetTileSet().GetPlacedPieces().Count; i++)//3
                {
                    Piece piece = placedPieces[i];
                    if (!HintService.IsPieceAtValidHintPosition(piece, map))
                    {
                        V2Int posToGo = HintService.GetValidPosition(selectedHintPositions, piece.GetIdentifier(), map);
                        selectedHintPositions.Add(posToGo);

                        targetPositions.Add(piece, new V2Int((int)posToGo.x, (int)posToGo.y));
                    }
                }
            }

            //Cant do in parallel or they mess with each other when swapping....

            //Remove Pieces
            foreach (KeyValuePair<Piece, V2Int> entry in targetPositions)
            {
                Piece piece = entry.Key;
                TileSetController.RemovePiece(piece, piece.GetPosition(), map.GetTileSet());
            }
            //Place Pieces
            foreach (KeyValuePair<Piece, V2Int> entry in targetPositions)
            {
                Piece piece = entry.Key;
                V2Int targetPosition = entry.Value;

                piece.SetX(targetPosition.x);
                piece.SetY(targetPosition.y);
                TileSetController.PlacePiece(map.GetTileSet(), piece);
            }
        }
    }
}
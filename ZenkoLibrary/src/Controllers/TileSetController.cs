using Zenko.Entities;

namespace Zenko.Controllers
{
    public class TileSetController
    {
        const float SNOW_HEIGHT = .04f;
        const float GOAL_HEIGHT = 0.162f;

        public static void PlacePieces(TileSet tileSet, PieceType[] pieces, V2Int[] positions)
        {
            if (pieces.Length != positions.Length)
            {
                throw new System.Exception("Piece amount does not match position amount");
            }
            for (int i = 0; i < pieces.Length; i++)
            {
                PlacePiece(tileSet, new Piece(positions[i].x, positions[i].y, Zenko.Presets.PieceType.mapByType[pieces[i].ToString()]));
            }
        }

        //NOTE: PIECETYPE AND PRESETS.PIECETYPE ARE NOT THE SAME (YET)
        //Maybe Receive Piece eventually
        public static void PlacePiece(TileSet tileSet, Piece piece, bool gameBoard = false)
        {
            Tile tile = tileSet.GetTile(piece.GetPosition());
            V2Int position = piece.GetPosition();
            PieceType pieceType = piece.GetPieceType();

            //this shouldn't be necessary if all states handled accordingly prior to this, like piece removal or tileset resetting
            tile.SetSeedType(null);
            tile.SetPortalType(null);

            string identifier = Zenko.Presets.PieceType.mapByType[pieceType.ToString()];

            switch (pieceType)
            {
                case PieceType.Wall:
                    tile.SetTileType("Wall");
                    break;
                case PieceType.Left:
                case PieceType.Right:
                case PieceType.Up:
                case PieceType.Down:
                    tile.SetTileType("Wall");
                    Tile affectedTile = tileSet.GetTile(position + piece.GetIdentifierDirectionVector());
                    affectedTile.SetSideways(piece.GetIdentifierDirection());
                    break;
                case PieceType.WallSeed:
                case PieceType.LeftSeed:
                case PieceType.RightSeed:
                case PieceType.UpSeed:
                case PieceType.DownSeed:
                    tile.SetTileType("Seed");
                    tile.SetSeedType(piece.GetSeedType());
                    break;
                case PieceType.PortalLeft:
                case PieceType.PortalRight:
                case PieceType.PortalUp:
                case PieceType.PortalDown:
                    tile.SetTileType("Portal");
                    tile.SetPortalType(piece.GetIdentifierDirection());
                    tileSet.activePortalPositions.Add(position);
                    break;
                default:
                    throw new System.Exception("Invalid PieceType");
            }
            tile.SetIsTaken(true);

            tileSet.placedPieces.Add(piece);
            tileSet.placedPiecePositions.Add(position);
        }

        public static void PopSeedAt(TileSet tileSet, V2Int position)
        {
            PieceType pieceType = (PieceType)System.Enum.Parse(typeof(PieceType), tileSet.GetTile(position).GetSeedType());
            tileSet.GetTile(position).SetSeedType(null);
            PlacePiece(tileSet, new Piece(position.x, position.y, Zenko.Presets.PieceType.mapByType[pieceType.ToString()]));
        }

        public static void RemovePiece(Piece piece, V2Int boardCoords, TileSet tileSet)
        {
            //Change tile data
            Tile tile = tileSet.GetTile(boardCoords);
            tile.SetTileType("Ice");
            tile.SetPortalType(null);
            tile.SetIsTaken(false);

            // Remove from gameboard lists
            int gameBoardIndex = tileSet.GetPlacedPiecePositions().IndexOf(boardCoords);
            tileSet.GetPlacedPiecePositions().RemoveAt(gameBoardIndex);
            tileSet.GetPlacedPieces().RemoveAt(gameBoardIndex);

            if (piece.IsPortal())
            {
                tileSet.activePortalPositions.Remove(boardCoords);
            }

            //if removed piece is icarus do special logic
            if (piece.IsIcarus())
            {
                V2Int affectedPosition = piece.GetIdentifierDirectionVector() + boardCoords;
                Tile affectedTile = tileSet.GetTile(affectedPosition);
                affectedTile.SetSideways(null);

                //KEEP ADJACENT ICARI
                foreach (Piece otherPiece in tileSet.GetPlacedPieces())
                {
                    if (piece.IsIcarus())
                    {
                        if (affectedPosition == otherPiece.GetBoardCoords() + otherPiece.GetIdentifierDirectionVector())
                        {
                            tile = tileSet.GetTile(affectedPosition);
                            tile.SetSideways(otherPiece.GetIdentifierDirection());
                        }
                    }
                }
            }
        }
    }
}
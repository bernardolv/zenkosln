using System;
using System.Collections.Generic;
using System.Linq;
using Zenko.Entities;

public struct BackendMap
{
    public string[] board { get; set; }
    public string solution { get; set; }

    public int yDimension { get; set; }
    public int xDimension { get; set; }

    public int turnsAmount { get; set; }
    public int piecesAmount { get; set; }

    public int windsUsedAmount { get; set; }
    public int portalsUsedAmount { get; set; }
    public int stoppedOnSeedAmount { get; set; }
    public int piecesHitAmount { get; set; }
    public int frontalWindAmount { get; set; }
    public int portalBlockedAmount { get; set; }

    public int wallAmount { get; set; }
    public int leftAmount { get; set; }
    public int rightAmount { get; set; }
    public int upAmount { get; set; }
    public int downAmount { get; set; }
    public int wallSeedAmount { get; set; }
    public int leftSeedAmount { get; set; }
    public int rightSeedAmount { get; set; }
    public int upSeedAmount { get; set; }
    public int downSeedAmount { get; set; }
    public int leftPortalAmount { get; set; }
    public int rightPortalAmount { get; set; }
    public int upPortalAmount { get; set; }
    public int downPortalAmount { get; set; }

    public BackendMap(Map map, Solution solutionObject, Conditions conditions)
    {
        solution = solutionObject.GetAsString();

        List<string> mapLines = map.GetPrintLines().ToList();
        mapLines.RemoveAt(0);
        mapLines.RemoveAt(mapLines.Count - 1);
        mapLines.RemoveAt(mapLines.Count - 1);

        board = mapLines.ToArray();

        yDimension = map.GetTileSet().GetYDimension();

        xDimension = map.GetTileSet().GetXDimension();

        turnsAmount = solutionObject.GetTurns();

        piecesAmount = map.GetPieces().Count();

        windsUsedAmount = conditions.GetWindsUsed();

        portalsUsedAmount = conditions.GetPortalsUsed();

        stoppedOnSeedAmount = conditions.GetStoppedOnSeed();

        piecesHitAmount = conditions.GetPiecesHit();

        frontalWindAmount = conditions.GetFrontalWind();

        portalBlockedAmount = conditions.GetPortalBlocked();

        wallAmount = 0;

        leftAmount = 0;

        rightAmount = 0;

        upAmount = 0;

        downAmount = 0;

        wallSeedAmount = 0;

        leftSeedAmount = 0;

        rightSeedAmount = 0;

        upSeedAmount = 0;

        downSeedAmount = 0;

        leftPortalAmount = 0;

        rightPortalAmount = 0;

        upPortalAmount = 0;

        downPortalAmount = 0;

        foreach (Piece piece in map.GetPieces())
        {
            switch (piece.GetIdentifier())
            {
                case "P":
                    wallAmount++;
                    break;
                case "L":
                    leftAmount++;
                    break;
                case "R":
                    rightAmount++;
                    break;
                case "U":
                    upAmount++;
                    break;
                case "D":
                    downAmount++;
                    break;
                case "p":
                    wallSeedAmount++;
                    break;
                case "l":
                    leftSeedAmount++;
                    break;
                case "r":
                    rightSeedAmount++;
                    break;
                case "u":
                    upSeedAmount++;
                    break;
                case "d":
                    downSeedAmount++;
                    break;
                case "PL":
                    leftPortalAmount++;
                    break;
                case "PR":
                    rightPortalAmount++;
                    break;
                case "PU":
                    upPortalAmount++;
                    break;
                case "PD":
                    downPortalAmount++;
                    break;
            }
        }
    }
}
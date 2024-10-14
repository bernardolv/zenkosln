using System.Collections;
using System.Collections.Generic;

namespace Zenko.Entities
{
    public class Conditions
    {
        /////////////////////////
        //      PROPERTIES     //
        /////////////////////////


        int windsUsed;
        int portalsUsed;
        int stoppedOnSeed;
        int piecesHit;
        int turns;
        int frontalWind;
        int portalBlocked;

        /////////////////////////
        //     CONSTRUCTORS    //
        /////////////////////////




        /////////////////////////
        //  GETTERS & SETTERS  //
        /////////////////////////


        public void AddWindUsed()
        {
            // Logger.Log("Wind");
            windsUsed++;
        }

        public void AddPortalUsed()
        {
            // Logger.Log("Portal used");
            portalsUsed++;
        }

        public void AddStoppedOnSeed()
        {
            // Logger.Log("Stopped on seed");
            stoppedOnSeed++;
        }

        public void AddPieceHit()
        {
            // Logger.Log("Piece hit");
            piecesHit++;
        }
        public void AddTurn()
        {
            // Logger.Log("Turn");
            turns++;
        }
        public void AddFrontalWind()
        {
            // Logger.Log("Wind Frontal");
            frontalWind++;
        }
        public void AddPortalBlocked()
        {
            // Logger.Log("Portal blocked");
            portalBlocked++;
        }
        /////////////////////////
        //    HELPER METHODS   //
        /////////////////////////

        public void Print()
        {
            Logger.Log("Winds used: " + windsUsed + " | Portals used: " + portalsUsed + " | Stopped on seeds: " + stoppedOnSeed + " | Pieces hit: " + piecesHit + " | Turns: " + turns + " | Frontal winds used: " + frontalWind + " | Portals blocked: " + portalBlocked);
        }
        public bool MeetsConditions(List<Condition> requirements)
        {
            foreach (Condition condition in requirements)
            {
                //"AVOID" Conditions
                if (condition.type == ConditionType.NoSeedStop)
                {
                    if (stoppedOnSeed > condition.amount)
                    {
                        return false;
                    }
                }
                else if (condition.type == ConditionType.NoWindFrontal)
                {
                    if (frontalWind > condition.amount)
                    {
                        return false;
                    }
                }
                //"WANTED" Conditions
                else if (condition.type == ConditionType.Wind)
                {
                    if (windsUsed < condition.amount)
                    {
                        return false;
                    }
                    continue;
                }
                else if (condition.type == ConditionType.Portal)
                {
                    if (portalsUsed < condition.amount)
                    {
                        return false;
                    }
                    continue;
                }
                else if (condition.type == ConditionType.PieceStop)
                {
                    if (piecesHit < condition.amount)
                    {
                        return false;
                    }
                    continue;
                }
                else if (condition.type == ConditionType.MinTurns)
                {
                    if (turns < condition.amount)
                    {
                        return false;
                    }
                    continue;
                }
                else if (condition.type == ConditionType.PortalBlocked)
                {
                    if (portalBlocked < condition.amount)
                    {
                        return false;
                    }
                    continue;
                }
            }
            return true;
        }
    }
}

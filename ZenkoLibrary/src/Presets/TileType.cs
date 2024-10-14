using System.Collections;
using System.Collections.Generic;

namespace Zenko.Presets
{
    public class TileType
    {
        public const string START = "S";
        public const string GOAL = "G";
        public const string WALL = "1";
        public const string ICE = "0";
        public const string HOLE = "H";
        public const string FLOWER = "#";
        public const string FRAGILE = "F";

        public static Dictionary<string, string> mapByIdentifier = new Dictionary<string, string>()
        {
            {"S","Start"},
            {"G","Goal"},
            {"1","Wall"},
            {"0","Ice"},
            {"H","Hole"},
            {"#","Wood"},
            {"F","Fragile"}
        };
        public static Dictionary<string, string> mapByType = new Dictionary<string, string>()
        {
            {"Start","S"},
            {"Goal","G"},
            {"Wall","1"},
            {"Ice","0"},
            {"Hole","H"},
            {"Wood","#"},
            {"Fragile","F"}
        };
    }
}


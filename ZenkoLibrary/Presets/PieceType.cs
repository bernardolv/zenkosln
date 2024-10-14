using System.Collections;
using System.Collections.Generic;


namespace Zenko.Presets
{
    public class PieceType
    {
        //icari
        public const string LEFT = "L";
        public const string RIGHT = "R";
        public const string UP = "U";
        public const string DOWN = "D";

        //pedro
        public const string WALL = "P";

        //icari seed
        public const string LEFT_SEED = "l";
        public const string RIGHT_SEED = "r";
        public const string UP_SEED = "u";
        public const string DOWN_SEED = "d";

        //pedro seed
        public const string WALL_SEED = "w";

        //tanukis
        public const string PORTAL_LEFT = "PL";
        public const string PORTAL_RIGHT = "PR";
        public const string PORTAL_UP = "PU";
        public const string PORTAL_DOWN = "PD";

        public static Dictionary<string, string> mapByIdentifier = new Dictionary<string, string>()
        {
            {"L","Left"},
            {"R","Right"},
            {"U","Up"},
            {"D","Down"},
            {"P","Wall"},
            {"l","LeftSeed"},
            {"r","RightSeed"},
            {"u","UpSeed"},
            {"d","DownSeed"},
            {"p","WallSeed"},
            {"PL","PortalLeft"},
            {"PR","PortalRight"},
            {"PU","PortalUp"},
            {"PD","PortalDown"}
        };
        public static Dictionary<string, string> mapByType = new Dictionary<string, string>()
        {
            {"Left","L"},
            {"Right","R"},
            {"Up","U"},
            {"Down","D"},
            {"Wall","P"},
            {"LeftSeed","l"},
            {"RightSeed","r"},
            {"UpSeed","u"},
            {"DownSeed","d"},
            {"WallSeed","p"},
            {"PortalLeft","PL"},
            {"PortalRight","PR"},
            {"PortalUp","PU"},
            {"PortalDown","PD"}
        };
    }
}


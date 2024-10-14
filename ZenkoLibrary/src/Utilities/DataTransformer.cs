namespace Zenko
{
    public static class DataTransformer
    {
        //Up and Down are switched because of board/tileset fuckery
        public static V2Int DirectionStringToV2Int(string direction)
        {
            switch (direction)
            {
                case "Left":
                    return new V2Int(-1, 0);
                case "Right":
                    return new V2Int(1, 0);
                case "Up":
                    return new V2Int(0, -1);
                case "Down":
                    return new V2Int(0, 1);
                default:
                    throw new System.Exception("Invalid direction string");
            }
        }
    }
}
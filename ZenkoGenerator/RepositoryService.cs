using System.Text.RegularExpressions;
using System.IO;
using Zenko.Factories;
using Zenko.Entities;
using Zenko;

namespace Repositories
{
    public class RepositoryService
    {
        /////////////////////////
        //      PROPERTIES     //
        /////////////////////////

        public string[] lines;
        public int[] pointers;


        ////////////////////////
        //       METHODS      //
        ////////////////////////

        public void InitializeRepository(string filePath)
        {
            lines = SplitString(File.ReadAllText(filePath));
            SetPointers();
        }

        /////////////////////////
        // GETTERS AND SETTERS //
        /////////////////////////

        //detects white space and sets index to place after it
        public void SetPointers()
        {
            List<int> pointerList = new List<int>();
            pointerList.Add(0);
            for (int i = 1; i < lines.Length - 1; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i]))
                {
                    pointerList.Add(i + 1);
                }
            }

            //setpointers
            pointers = pointerList.ToArray();
        }

        public Map GetMap(int levelNumber)
        {
            Map map = MapFactory.Map(GetLevelLines(levelNumber), levelNumber);
            return map;
        }

        public string[] GetLevelLines(int levelNumber)
        {
            int levelIndex = levelNumber - 1;
            //validate levelNumber
            if (levelIndex < 0)
            {
                Logger.LogError("Level number should be higher than 0");
                return null;
            }
            else if (pointers.Length <= levelIndex)
            {
                Logger.LogError("Level number should be  " + pointers.Length + " or lower");
                return null;
            }

            List<string> levelLines = new List<string>();
            int initialLine = pointers[levelIndex];
            int i = 0;
            while (initialLine + i < lines.Length && !string.IsNullOrWhiteSpace(lines[initialLine + i]))
            {
                levelLines.Add(lines[initialLine + i]);
                i++;
            }

            return levelLines.ToArray();
        }

        /////////////////////////
        //    HELPER METHODS   //
        /////////////////////////
        public static string[] SplitString(string data)
        {
            string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
            string[] newLiens = Regex.Split(data, LINE_SPLIT_RE);
            return newLiens;
        }
    }
}

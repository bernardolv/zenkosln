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

        public int[] pointers;
        string filePath;

        public int LevelCount
        {
            get
            {
                return pointers.Length;
            }
        }


        ////////////////////////
        //       METHODS      //
        ////////////////////////

        public void InitializeRepository(string filePath)
        {
            this.filePath = filePath;

            List<int> pointerList = new List<int>();
            pointerList.Add(0);
            int lineCount = 0;
            bool lastWasEmpty = false;
            foreach (string line in File.ReadLines(filePath))
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    //This is a fail safe for bad formatting
                    if (lastWasEmpty)
                    {
                        Console.Error.WriteLine("We had two empty lines next to each other so we stop pointer seeking there at line " + lineCount);
                        break;
                    }
                    lastWasEmpty = true;
                }
                else
                {
                    if (lastWasEmpty)
                    {
                        pointerList.Add(lineCount);
                    }
                    lastWasEmpty = false;
                }
                lineCount++;
            }
            pointers = pointerList.ToArray();
        }

        /////////////////////////
        // GETTERS AND SETTERS //
        /////////////////////////

        public Map GetMap(int levelNumber)
        {
            string[] lines = GetLevelLines(levelNumber);
            Map map = MapFactory.Map(lines, levelNumber);
            return map;
        }

        public string[] GetLevelLines(int levelNumber)
        {
            int levelIndex = levelNumber - 1;

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

            int lineNumber = 0;
            foreach (string line in File.ReadLines(filePath))
            {
                if (lineNumber < initialLine)
                {
                    //Do nothing
                }
                else if (lineNumber == initialLine)
                {
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        Console.Error.WriteLine("Level is empty");
                        return null;
                    }
                    levelLines.Add(line);
                }
                else
                {
                    //We have reached the end if its a whitespace
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        break;
                    }

                    levelLines.Add(line);
                }

                lineNumber++;
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

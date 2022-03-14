
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TechnicalAssessment.Matching
{
    public class MatchingNames
    {
        // finding each character frequencies  and returning the result as a string 
        public static async Task<string> CharacterCount(string Player1, string Player2)
        {
            try
            {
                string match = Player1.ToLower() + "matches" + Player2.ToLower();
                string resultAfterMatch = string.Empty;
                Dictionary<char, int> characterOccur = new Dictionary<char, int>();

                foreach (char myChar in match)
                {
                    if (characterOccur.ContainsKey(myChar))
                    {
                        characterOccur[myChar] = characterOccur[myChar] + 1;
                    }
                    else
                    {
                        characterOccur[myChar] = 1;
                    }
                }

                
                foreach (KeyValuePair<char, int> myChar in characterOccur)
                {
                    resultAfterMatch += myChar.Value.ToString();
                }
                return resultAfterMatch;
            }
            catch (Exception ex)
            {

                await Logs("Error:" + ex.Message);
                return string.Empty;
            }

        }
        //function that accepts character frequencies to find match percentage.Breaks when length of character frequencies equals two
        public static async Task<string> Percentage(string occurancesOfChars)
        {
            try
            {
                if (occurancesOfChars.Length == 2)
                {
                    return occurancesOfChars;
                }

                string _countChars = "";
                char[] strArray = occurancesOfChars.ToArray();
                int len = 0;

                if (strArray.Length % 2 == 0)
                {
                    len = (strArray.Length / 2);
                }
                else
                {
                    len = Convert.ToInt32((strArray.Length / 2));
                }

                for (int i = 0; i < len; i++)
                {
                    if (!(strArray.Length % 2 == 0) && (i + 1 == len))
                    {
                        char leftMostNumber = strArray[len - 1];
                        char rightMostNumber = strArray[len + 1];
                        _countChars += (Convert.ToInt32(leftMostNumber.ToString()) + Convert.ToInt32(rightMostNumber.ToString()));
                        char middleNumber = strArray[len];
                        _countChars += (Convert.ToInt32(middleNumber.ToString()));
                        break;
                    }
                    else
                    {
                        char leftMostNumber = strArray[i];
                        char rightMostNumber = strArray[strArray.Length - 1 - i];

                        _countChars += (Convert.ToInt32(leftMostNumber.ToString()) + Convert.ToInt32(rightMostNumber.ToString()));
                    }
                }
                return Percentage(_countChars).Result;
            }
            catch (Exception ex)
            {
                await Logs("Error:" + ex.Message);
                return string.Empty;
            }

        }
        // Funtion that check if players firstnames contains only alphabets
        public static async Task<bool> ContainsOnlyAlphabets(string Player1, string Player2)
        {
            try
            {
                return (Player1.All(mychar => Char.IsLetter(mychar)) && Player2.All(mychar => Char.IsLetter(mychar)));
            }
            catch (Exception ex)
            {
                await Logs("Error:" + ex.Message);
                return false;
            }
        }
        //Writting previous tested matches into a file
        public static async Task<string> writesResultsIntoFile(string Player1, string Player2, string Percent, string groupMatch = "No")
        {
            try
            {
                string macthResult = string.Empty;
                if (Convert.ToInt32(Percent) >= 80)
                {
                    macthResult = Player1 + " matches " + Player2 + " " + Percent + "%, Good match";
                }
                else
                {
                    macthResult = Player1 + " matches " + Player2 + " " + Percent + "%";
                }
                if (groupMatch.Equals("Yes"))
                {
                    using StreamWriter file = new StreamWriter("Output.txt", append: true);
                    await file.WriteLineAsync(macthResult);
                }
                else
                {
                    using StreamWriter file = new StreamWriter("Output.txt", append: false);
                    await file.WriteLineAsync(macthResult);
                }

                return macthResult;
            }
            catch (Exception ex)
            {
                await Logs("Error:" + ex.Message);
                return string.Empty;
            }
        }

        // Reading player names from csv file and grouping them using gender 
        public static async Task<Dictionary<string, List<string>>> ReadCSVFile()
        {
            try
            {
                Dictionary<string, List<string>> Players = new Dictionary<string, List<string>>();
                Players["Boys"] = new List<string>();
                Players["Girls"] = new List<string>();
                using (StreamReader reader = new StreamReader("NameLog.csv"))
                {
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        string[] values = line.Split(',');
                        if (values[1].ToLower().Trim().Equals("m"))
                        {
                            
                            if (!Players["Boys"].Contains(values[0].Trim()))
                            {
                                Players["Boys"].Add(values[0].Trim());
                            }
                            else
                            {
                                await Logs(" Duplicate found: " + line);
                            }


                        }
                        else
                        {
                            
                            if (!Players["Girls"].Contains(values[0].Trim()))
                            {
                                Players["Girls"].Add(values[0].Trim());
                            }
                            else
                            {
                                await Logs(" Duplicate found: " + line);
                            }
                        }
                    }
                }
                return Players;
            }
            catch (Exception ex)
            {
                await Logs("Error:" + ex.Message);
                return new Dictionary<string, List<string>>();
            }
        }

        // Writing logs into a text file
        public static async Task Logs(string log)
        {
            try
            {
                using StreamWriter file = new StreamWriter("Logs.txt", append: true);
                await file.WriteLineAsync(DateTime.Now + "-" + log);
            }
            catch (Exception ex)
            {

                await Logs("Error:" + ex.Message);

            }
        }
        // Sorting previous tested matches
        public static IOrderedEnumerable<KeyValuePair<int, List<string>>> ReadResultsFile()
        {
            Dictionary<int, List<string>> resultlist = new Dictionary<int, List<string>>();
            FileStream fileStream = new FileStream("Output.txt", FileMode.Open, FileAccess.Read);
            using (StreamReader streamReader = new StreamReader(fileStream, Encoding.UTF8))
            {
                string line;
                while ((line = streamReader.ReadLine()) != null)
                {

                    string[] token = line.Split(' ');
                    //Chenking if the pecentage already exist
                    if (resultlist.ContainsKey(Convert.ToInt32(token[3].Trim().Substring(0, 2))))
                    {
                        string myKey = token[3].Trim().Substring(0, 2);
                        resultlist[Convert.ToInt32(myKey)].Add(line);
                        resultlist[Convert.ToInt32(myKey)].Sort();
                    }
                    else
                    {
                        string myKey = token[3].Trim().Substring(0, 2);
                        resultlist[Convert.ToInt32(myKey)] = new List<string>();
                        resultlist[Convert.ToInt32(myKey)].Add(line);
                    }

                }
            }
            // Sorting the Result  
            return resultlist.OrderByDescending(key => key.Key);
        }
        //writing sorted results in output file
        public static async Task SortedResults(IOrderedEnumerable<KeyValuePair<int, List<string>>> sortedResultDictionary)
        {
            try
            {
                ClearOutputFile();
                using StreamWriter file = new StreamWriter("Output.txt", append: false);
                foreach (KeyValuePair<int, List<string>> results in sortedResultDictionary)
                {
                    foreach (string result in results.Value)
                    {
                        await file.WriteLineAsync(result);
                    }
                }

            }
            catch (Exception ex)
            {
                await Logs("Error:" + ex.Message);
            }

        }
        //getting log history from file and putting results in a list
        public static List<string> getLogs()
        {

            List<string> listOsLogs = new List<string>();
            FileStream fileStream = new FileStream("Logs.txt", FileMode.Open, FileAccess.Read);
            using (StreamReader streamReader = new StreamReader(fileStream, Encoding.UTF8))
            {
                string line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    listOsLogs.Add(line);
                }
            }
            return listOsLogs;

        }
        /*public static List<string> getOutPutResults()
        {

            List<string> listoutputResults = new List<string>();
            FileStream fileStream = new FileStream("output.txt", FileMode.Open, FileAccess.Read);
            using (StreamReader streamReader = new StreamReader(fileStream, Encoding.UTF8))
            {
                string line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    listoutputResults.Add(line);
                }
            }
            return listoutputResults;

        }
        */
        private static void ClearOutputFile()
        {
            if (!File.Exists("Output.txt"))
                File.Create("Output.txt");

            TextWriter tw = new StreamWriter("Output.txt", append: false);
            tw.Write("");
            tw.Close();
        }
    }
}


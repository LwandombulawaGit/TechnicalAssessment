using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TechnicalAssessment.Matching;

namespace TechnicalAssessment.Pages
{
    public class FileResults : PageModel
    {

        public IOrderedEnumerable<KeyValuePair<int, List<string>>> sortedResultDictionary { get; set; }
        public async Task OnGet()
        {
            try
            {
                Dictionary<string, List<string>> groupOfPlayers = MatchingNames.ReadCSVFile().Result;
                for (int i = 0; i < groupOfPlayers["Boys"].Count; i++)
                {
                    foreach (string girlName in groupOfPlayers["Girls"])
                    {
                        if (MatchingNames.ContainsOnlyAlphabets(groupOfPlayers["Boys"][i], girlName).Result)
                        {
                            string charOccurances = MatchingNames.CharacterCount(groupOfPlayers["Boys"][i], girlName).Result;
                            await MatchingNames.writesResultsIntoFile(groupOfPlayers["Boys"][i], girlName, MatchingNames.Percentage(charOccurances).Result, "Yes");
                        }
                        else
                        {
                            await MatchingNames.Logs("Error: Invalid inputs between " + groupOfPlayers["Boys"][i] + " and " + girlName);
                        }
                    }
                }
                sortedResultDictionary = MatchingNames.ReadResultsFile();
            }
            catch (Exception ex)
            {
                await MatchingNames.Logs("Error:" + ex.Message);
            }

        }
    }
}

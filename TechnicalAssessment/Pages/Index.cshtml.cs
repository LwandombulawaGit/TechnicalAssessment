using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechnicalAssessment.Matching;

namespace TechnicalAssessment.Pages
{

    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        public string Player1 { get; set; }
        public string Player2 { get; set; }
        public string singleMatchResult { get; set; }
        public bool isError { get; set; }
        public IOrderedEnumerable<KeyValuePair<int, List<string>>> sortedResultDictionary { get; set; }
        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet(bool error = false)
        {
            isError = error;
        }
        public async Task OnPost(string player1, string player2)
        {
            try
            {
                if (player1 != null && player2 != null && MatchingNames.ContainsOnlyAlphabets(player1, player2).Result)
                {
                    string charOccurances = MatchingNames.CharacterCount(player1, player2).Result;
                    singleMatchResult = await MatchingNames.writesResultsIntoFile(player1, player2, MatchingNames.Percentage(charOccurances).Result);
                    sortedResultDictionary = MatchingNames.ReadResultsFile();
                    await MatchingNames.SortedResults(sortedResultDictionary);

                }
                else if (player1 == null && player2 == null)
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
                                isError = true;
                            }
                        }
                    }
                    sortedResultDictionary = MatchingNames.ReadResultsFile();
                }
                else
                {
                    await MatchingNames.Logs("Error: Invalid inputs between " + player1 + " and " + player2);
                    isError = true;
                }
            }
            catch (Exception ex)
            {
                await MatchingNames.Logs("Error:" + ex.Message);
                isError = true;
            }
        }
    }
}

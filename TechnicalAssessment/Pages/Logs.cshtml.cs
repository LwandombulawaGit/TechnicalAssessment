using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TechnicalAssessment.Matching;

namespace TechnicalAssessment.Pages
{
    public class LogsModel : PageModel
    {
        public List<string> listLogs;
        public void OnGet()
        {
            listLogs = MatchingNames.getLogs();
        }
    }
}

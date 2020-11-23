using Microsoft.ClearScript.V8;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TvpMain.Reference;

namespace TvpMain.Check
{
    public class CheckAndFixRunner : ICheckAndFixRunner
    {
        public List<CheckResultItem> ExecCheckAndFix(string text, CheckAndFixItem checkAndFixItem)
        {
            Regex checkRegex = new Regex( checkAndFixItem.CheckRegex);
            MatchCollection matches = checkRegex.Matches(text);
            List<CheckResultItem> checkResultItems = new List<CheckResultItem>();

            foreach ( Match match in matches)
            {
                CheckResultItem checkResultItem = new CheckResultItem(checkAndFixItem.Description, 
                    match.Value,
                    match.Index, 
                    CheckType.MissingSentencePunctuation,
                    (int) ScriptureReferenceErrorType.LooseFormatting
                    );

                if (!String.IsNullOrEmpty(checkAndFixItem.FixRegex))
                {
                    checkResultItem.FixText = match.Result(checkAndFixItem.FixRegex);
                }
                checkResultItems.Add(checkResultItem);
            }

            using (var engine = new V8ScriptEngine())
            {
                engine.AddHostObject("checkResultItems", checkResultItems);
                engine.Execute(checkAndFixItem.CheckScript);
                checkResultItems = engine.Script.checkResultItems;
            }

            return checkResultItems;
            //return new List<CheckResultItem>(); 
        }
    }
}

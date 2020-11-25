using Microsoft.ClearScript.V8;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TvpMain.Reference;

namespace TvpMain.Check
{
    /// <summary>
    /// This is the main check/fix runner
    /// Given the text to check, and the check/fix model, it'll run the check/fix against the text,
    /// returning the result set.
    /// </summary>
    public class CheckAndFixRunner : ICheckAndFixRunner
    {
        /// <summary>
        /// Executes the check/fix against the given text
        /// </summary>
        /// <param name="text">The text (BCV) to be checked.</param>
        /// <param name="checkAndFixItem">The model that describes the check/fix</param>
        /// <returns>The list of check result items. This includes the suggested fixes if they are available.</returns>
        public List<CheckResultItem> ExecCheckAndFix(string text, CheckAndFixItem checkAndFixItem)
        {
            // First, run the check regex, looking for matches
            Regex checkRegex = new Regex(@checkAndFixItem.CheckRegex.Trim());
            MatchCollection matches = checkRegex.Matches(text);
            List<CheckResultItem> checkResultItems = new List<CheckResultItem>();

            // Now, loop through the matches
            foreach (Match match in matches)
            {
                // Create a result item
                CheckResultItem checkResultItem = new CheckResultItem(checkAndFixItem.Description,
                    match.Value,
                    match.Index,
                    CheckType.MissingSentencePunctuation,
                    (int)ScriptureReferenceErrorType.LooseFormatting
                    );

                // If there is a replacement regex, apply that to the result
                if (!String.IsNullOrEmpty(checkAndFixItem.FixRegex))
                {
                    checkResultItem.FixText = match.Result(checkAndFixItem.FixRegex.Trim());
                }
                checkResultItems.Add(checkResultItem);
            }

            // if we have a script to run, run it
            if (!String.IsNullOrEmpty(checkAndFixItem.CheckScript))
            {
                // start up the V8 engine to run the script
                // can set various flags
                // See [V8ScriptEngineFlags] The most interesting are
                // V8ScriptEngineFlags.EnableRemoteDebugging - for remote debugging using something like VS Code
                // V8ScriptEngineFlags.EnableDebugging - again, enable debugging
                // V8ScriptEngineFlags.AwaitDebuggerAndPauseOnStart - used to allow the debugger to attach as soon 
                // as the engine is set for the script
                using (var engine = new V8ScriptEngine())
                {
                    // passing data into and out of the script is done by JSON to limit issues with conversions
                    var resultsString = JsonConvert.SerializeObject(checkResultItems);

                    // This entry script demarshals the input and marshals the response
                    // The script in the check/fix need only have the 'checkAndFix' function defined
                    var entryScript = @"
                        function entry(jsonCheckResultItems) {
                            let checkResultItems = JSON.parse(jsonCheckResultItems)
                            return JSON.stringify(checkAndFix(checkResultItems))
                        }
                    ";

                    // Here we can add a bunch of different data and bind context variables
                    // For example, this is where we'd bind project settings
                    // Right now, just the console
                    engine.AddHostType("Console", typeof(Console));
                    // execute the script
                    engine.Execute(@checkAndFixItem.CheckScript + entryScript);
                    // convert the response to json
                    var outResultItems = engine.Script.entry(resultsString);
                    checkResultItems = JsonConvert.DeserializeObject<List<CheckResultItem>>(outResultItems);
                }
            }

            // return the results
            return checkResultItems;
        }
    }
}

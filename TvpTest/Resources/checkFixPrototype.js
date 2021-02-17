
// Here is the full input
// The input text
const inputText = 'Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.'

// The regular expression for the check
const checkRegex = /\w+/gm

// The check javascript
function checkAndFix( items) {
  // A simple example that could probably be done with just regex, but...
  return items.filter(i => i[0].length === 5).map(i => ({
    Description: '5 character word was not capitalized',
    MatchText: i[0],
    MatchStart: i.index,
    MatchLength: i[0].length,
    CheckType: 'LooseFormatting',
    FixText: i[0].toUpperCase(),
    ResultState: 'Found'
  }))
}

// Example processing
// items found by regex... the results have a standard set of match fields
let items = [...inputText.matchAll(checkRegex)]

// results from running check
let checkItems = checkAndFix( items)

console.info()
console.info('These are the checkAndFix results:')
console.info(checkItems)

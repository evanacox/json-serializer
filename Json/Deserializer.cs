using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Json 
{
    /// <summary>
    /// Handles parsing and de-serializing JSON data
    /// </summary>
    internal class Deserializer
    {
        /// <summary>
        /// Holds the JSON string 
        /// </summary>
        private string _jsonString;

        /// <summary>
        /// The index of the current character being parsed inside _jsonString
        /// </summary>
        private int _currentChar;

        /// <summary>
        /// Creates the deserializer
        /// </summary>
        /// <param name="data">The JSON string</param>
        public Deserializer(string data)
        {
            // strips whitespace, except from inside double quotes
            _jsonString = RemoveWhitespace(data);
        }

        /// <summary>
        /// Deserializes the JSON string the class holds
        /// </summary>
        /// <returns>A dictionary containing the JSON data</returns>
        public dynamic Deserialize()
        {
            // doing it this way handles valid JSON that doesn't start with {}
            // technically, just an element is perfectly valid (though I don't
            // know why you would ever use that)
            return Element();
        }

        /// <summary>
        /// Root JSON parsing method, begins parsing any type
        /// </summary>
        /// <returns>The object the JSON represents</returns>
        private dynamic Element()
        {
            var peeked = Peek();

            return (peeked.Type) switch
            {
                TokenType.OPEN_CURLY => Object(),
                TokenType.OPEN_BRACKET => Array(),
                TokenType.DOUBLE_QUOTE => String(),
                TokenType.NUMBER => Number(),
                // if it's past this point, its a character that *may* be a bool/null, or could be invalid
                _ => peeked.Character switch
                {
                    't' => Boolean(),
                    'f' => Boolean(),
                    'n' => Null(),
                    _ => throw new FormatException($"Unexpected character {peeked.Character}")
                }
            };
        }

        /// <summary>
        /// Transforms a JSON object ('{ ... }') into a Dictionary
        /// </summary>
        /// <returns>A Dictionary containing the JSON data</returns>
        private IDictionary<string, dynamic> Object()
        {
            var dict = new Dictionary<string, dynamic>();
            
            // whether or not there's a comma, and thus, whether new members
            // need to keep getting scanned
            var isComma = true;

            Consume('{');

            while (isComma)
            {
                isComma = false;

                // consumes a member's key inside the object
                var key = String();

                Consume(':');

                // consume whatever the value is, set it inside the dict
                dict[key] = Element();

                // consume comma if its present, and set isComma to true again
                if (Match(','))
                {
                    Consume(',');
                    isComma = true;
                }
            }

            Consume('}');

            return dict;
        }

        /// <summary>
        /// Transforms a JSON array ('[ ... ]') into a List
        /// </summary>
        /// <returns>A List containing the JSON data</returns>
        private IList<dynamic> Array()
        {
            var list = new List<dynamic>();

            // I'm doing basically the same thing I did 
            // with how I parsed {}s here. if there isn't a comma
            // after an element, I'm assuming its the last one
            // (since the JSON spec requires that)
            var isComma = true;

            Consume('[');

            // if there's a comma, the loop needs to continue grabbing elements
            while (isComma)
            {
                isComma = false;
                
                list.Add(Element());

                // set isComma to true if there's another comma
                if (Match(','))
                {
                    Consume(',');
                    isComma = true;
                }
            }

            Consume(']');

            return list;
        }

        /// <summary>
        /// Transforms a JSON string into a .NET string
        /// </summary>
        /// <returns>A .NET string</returns>
        private string String()
        {
            // Consume the first quote
            Consume('"');

            var readString = "";

            while (!Match('"'))
            {
                // if its the last character and it's in here, there wasn't a closing quote
                if (_currentChar + 1 == _jsonString.Length)
                {
                    throw new FormatException("Unexpected EOF while reading String!");
                }

                var consumed = Consume();

                // if the char isn't a backslash, just add and continue
                if (consumed.Character != '\\')
                {
                    readString += consumed.Character;

                    continue;
                }

                // if it IS a backslash, it might be escaping the next char
                var next = Consume();

                // if it IS a valid escape sequence, the character is added manually to the string
                readString += next.Character switch
                {
                    'b' => '\b',
                    'f' => '\f',
                    'n' => '\n',
                    'r' => '\r',
                    't' => '\t',
                    '"' => '"',
                    '\\' => '\\',
                    // Else, its just a slash and a character. Concat the two, and return
                    _ => consumed.Character.ToString() + next.Character.ToString()
                }; 
            }

            // consume the second quote
            Consume('"');

            return readString;
        }

        /// <summary>
        /// Transforms a JSON number into a .NET number. If there's a decimal 
        /// point, the method returns a `double`. If not, it returns a `long`
        /// </summary>
        /// <returns></returns>
        private dynamic Number()
        {
            var numberStr = "";

            // check for EOF has to come first, or the Match() will error out
            // only actually matters when parsing a raw 'x' that isnt inside an object
            while (!MatchesEnd())
            {
                var consumed = Consume();

                if (consumed.Type != TokenType.NUMBER && consumed.Type != TokenType.PERIOD)
                {
                    // why does `'}'` need to be interpolated? its weird
                    throw new FormatException($"Unexpected character '{consumed.Character}', expected a digit, period, comma, or {'}'} ");
                }

                numberStr += consumed.Character;
            }

            // don't consume the comma/curly, that's the job of the parent

            // if the number is a decimal, return a double
            if (numberStr.Contains('.'))
            {
                if (numberStr.Count(c => c == '.') > 1)
                {
                    throw new FormatException($"You can't have more than one period inside a number literal! Got: '{numberStr}'");
                }

                return double.Parse(numberStr);
            }

            // if it's not, return a long
            return long.Parse(numberStr);
        }

        /// <summary>
        /// Consumes a boolean
        /// </summary>
        /// <returns></returns>
        private bool Boolean()
        {
            var boolStr = "";

            while (!MatchesEnd())
            {
                var consumed = Consume();

                if (consumed.Type != TokenType.UNKNOWN)
                {
                    // why does `'}'` need to be interpolated? its weird
                    throw new FormatException($"Unexpected character '{consumed.Character}', expected a character that was part of a bool, comma, or {'}'}");
                }

                boolStr += consumed.Character;
            }

            return (boolStr == "true");
        }

        /// <summary>
        /// Returns `null` for JSON. This method pnly exists on the 
        /// off chance I ever change what JSON `null` translates to in .NET
        /// </summary>
        /// <returns>Returns `null`</returns>
        private object Null() => null;
        

        /// <summary>
        /// Consumes a character, and adds to the current index.
        /// </summary>
        /// <returns>A JsonToken object, containing the character and its data</returns>
        private JsonToken Consume()
        {
            var got = _jsonString[_currentChar++];

            return new JsonToken(got, _currentChar - 1);
        }

        /// <summary>
        /// Exactly like Consume(), but checks for an expected character. 
        /// 
        /// Throws a FormatException if its not the expected
        /// </summary>
        /// <param name="expected">The expected character</param>
        /// <returns>A JsonToken</returns>
        private JsonToken Consume(char expected)
        {
            var got = Consume();

            if (got.Character != expected)
            {
                throw new FormatException($"Expected '{expected}', got '{got.Character}' (index #{_currentChar})");
            }

            return got;
        }

        /// <summary>
        /// Acts like Consume, but doesn't actually add one to _currentChar
        /// </summary>
        /// <returns>A JsonToken</returns>
        private JsonToken Peek()
        {
            var token = Consume();

            --_currentChar;

            return token;
        }

        /// <summary>
        /// Matches the next character to consume
        /// </summary>
        /// <param name="toMatch">The character to compare to</param>
        /// <returns>Whether `toMatch` and the next character are the same</returns>
        private bool Match(char toMatch) => _jsonString[_currentChar] == toMatch;

        /// <summary>
        /// Override of Match, but with TokenType
        /// </summary>
        /// <param name="token">The TokenType to match against</param>
        /// <returns>Whether the next token matches that type</returns>
        private bool Match(TokenType token) => Peek().Type == token;

        /// <summary>
        /// Whether or not the next character is the end of something, e.g end of the field, 
        /// e.g end of an object, field, array, or EOF
        /// </summary>
        /// <returns>Returns whether the next char is the end of an object, field, array, or EOF</returns>
        private bool MatchesEnd() => _currentChar == _jsonString.Length || Match(',') || Match('}') || Match(']');

        /// <summary>
        /// Strips the input string of all whitespace, EXCEPT when it's between double quotes
        /// </summary>
        /// <param name="toRemoveFrom">The input string</param>
        /// <returns>The fixed string</returns>
        private string RemoveWhitespace(string toRemoveFrom)
        {
            // matches everything that isn't whitespace. also matches 
            // *everything* inside "s, and the "s themselves
            var matches = Regex.Matches(toRemoveFrom, "[^\\s\"]+|\"[^\"]*\"");
            var fixedString = "";

            foreach (Match match in matches)
            {
                fixedString += match.Value;
            }

            return fixedString;
        }
    }
}

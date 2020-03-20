using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Serialization
{
    /// <summary>
    /// Handles de-serializing JSON data
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
            _jsonString = RemoveWhitespace(data);
        }

        /// <summary>
        /// Deserializes the JSON string the class holds
        /// </summary>
        /// <returns>A dictionary containing the JSON data</returns>
        public dynamic Deserialize()
        {
            // TODO: handle JSON strings that don't start with objects
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
                _ => peeked.Character switch
                {
                    't' => Boolean(),
                    'f' => Boolean(),
                    'n' => Null(),
                    _ => throw new FormatException($"Unexpected character {peeked.Character}")
                }
            };
        }

        private IReadOnlyDictionary<string, dynamic> Object()
        {
            var dict = new Dictionary<string, dynamic>();

            // consume opening curly
            Consume('{');

            while (!Match('}'))
            {
                // consumes a member's key inside the object
                var key = String();

                Consume(':');

                dict[key] = Element();

                if (Match(','))
                {
                    Consume(',');
                }
            }

            Consume('}');

            return dict;
        }

        private IReadOnlyList<dynamic> Array()
        {
            return null;
        }

        private string String()
        {
            // Consume the first quote
            Consume('"');

            var str = "";

            while (!Match('"'))
            {
                if (_currentChar + 1 == _jsonString.Length)
                {
                    throw new FormatException("Unexpected EOF while reading String!");
                }

                var consumed = Consume();

                if (consumed.Character != '\\')
                {
                    str += consumed.Character;

                    continue;
                }

                // character might be escaping something
                var next = Consume();

                str += next.Character switch
                {
                    'b' => '\b',
                    'f' => '\f',
                    'n' => '\n',
                    'r' => '\r',
                    't' => '\t',
                    '"' => '"',
                    '\\' => '\\',
                    // I know its only necessary with one, but its much more clear whats going on
                    // when both are .ToString()-ed
                    _ => consumed.Character.ToString() + next.Character.ToString()
                }; 
            }

            // consume the second quote
            Consume('"');

            return str;
        }

        private float Number()
        {
            var numberStr = "";

            // check for EOF has to come first, or the Match() will error out
            // only actually matters when parsing a raw 'x' that isnt inside an object
            while (_currentChar != _jsonString.Length && !Match(',') && !Match('}'))
            {
                var consumed = Consume();

                if (consumed.Type != TokenType.NUMBER && consumed.Type != TokenType.PERIOD)
                {
                    throw new FormatException($"Unexpected character '{consumed.Character}', expected a digit or '.'");
                }

                numberStr += consumed.Character;
            }

            return float.Parse(numberStr);
        }

        private bool Boolean()
        {
            var boolStr = "";

            while (_currentChar != _jsonString.Length && !Match(',') && !Match('}'))
            {
                var consumed = Consume();

                if (consumed.Type != TokenType.UNKNOWN)
                {
                    throw new FormatException($"Unexpected character '{consumed.Character}', expected a character that was part of a bool");
                }

                boolStr += consumed.Character;
            }

            return (boolStr == "true");
        }

        private object Null()
        {
            return null;
        }

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
        /// Strips the input string of all whitespace
        /// </summary>
        /// <param name="toRemoveFrom">The input string</param>
        /// <returns>The string, but without any whitespace</returns>
        private string RemoveWhitespace(string toRemoveFrom) => Regex.Replace(toRemoveFrom, @"\s+", System.String.Empty);
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Serialization
{
    internal enum TokenType
    {
        OPEN_CURLY,
        CLOSE_CURLY,
        OPEN_BRACKET,
        CLOSE_BRACKET,
        DOUBLE_QUOTE,
        COLON,
        COMMA,
        NUMBER,
        PERIOD,
        UNKNOWN
    }

    /// <summary>
    /// Represents a single token in a JSON string
    /// </summary>
    internal struct JsonToken
    {
        public readonly TokenType Type;

        public readonly char Character;

        public readonly int Index;

        public JsonToken(char tok, int idx)
        {
            Character = tok;
            Index = idx;

            if (char.IsDigit(tok))
            {
                Type = TokenType.NUMBER;

                return;
            }

            Type = tok switch
            {
                '{' => TokenType.OPEN_CURLY,
                '}' => TokenType.CLOSE_CURLY,
                '[' => TokenType.OPEN_BRACKET,
                ']' => TokenType.CLOSE_BRACKET,
                '"' => TokenType.DOUBLE_QUOTE,
                ':' => TokenType.COLON,
                '.' => TokenType.PERIOD,
                ',' => TokenType.COMMA,
                _ => TokenType.UNKNOWN
            };
        }
    }
}

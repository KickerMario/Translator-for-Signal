using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Compiler
{
    class Tables
    {
        public struct tsymbol
        {
            public char value;
            public byte attr;
        }

        //Dict of reserved words
        public readonly Dictionary<string, int> keyWords = new Dictionary<string, int>()
        { { "PROGRAM", 501 }, {"VAR", 502 }, {"BEGIN", 503 }, {"END", 504 }, {"WHILE", 505 }, {"ENDWHILE", 506 },
        {"INTEGER", 507 }, {"FLOAT", 508 }, {"DO" , 509 } };

        //Dict of multi-character delimiters
        public readonly Dictionary<string, int> compOperators = new Dictionary<string, int>()
        { { ">=", 601 }, { "<=", 603 }, { "<>", 604 }, { "<", 60}, { "=", 61 }, { ">", 62} };

        //Dict of constants
        public Dictionary<string, int> constTable;

        //Dict of all character (alphabet)
        public ReadOnlyDictionary<char, byte> attributes = new ReadOnlyDictionary<char, byte>(
            new Dictionary<char, byte>()
            {
                {' ',  0 }, {'0', 1 }, {'a', 2 }, {'A', 2 }, {'(', 3 }, {',', 4 },
                {'\r', 0 }, {'1', 1 }, {'b', 2 }, {'B', 2 },            {';', 4 },
                {'\t', 0 }, {'2', 1 }, {'c', 2 }, {'C', 2 },            {'=', 4 },
                {'\n', 0 }, {'3', 1 }, {'d', 2 }, {'D', 2 },            {':', 4 },
                            {'4', 1 }, {'e', 2 }, {'E', 2 },            {'>', 4 },
                            {'5', 1 }, {'f', 2 }, {'F', 2 },            {'<', 4 },
                            {'6', 1 }, {'g', 2 }, {'G', 2 },
                            {'7', 1 }, {'h', 2 }, {'H', 2 },
                            {'8', 1 }, {'i', 2 }, {'I', 2 },
                            {'9', 1 }, {'j', 2 }, {'J', 2 },
                                       {'k', 2 }, {'K', 2 },
                                       {'l', 2 }, {'L', 2 },
                                       {'m', 2 }, {'M', 2 },
                                       {'n', 2 }, {'N', 2 },
                                       {'o', 2 }, {'O', 2 },
                                       {'p', 2 }, {'P', 2 },
                                       {'q', 2 }, {'Q', 2 },
                                       {'r', 2 }, {'R', 2 },
                                       {'s', 2 }, {'S', 2 },
                                       {'t', 2 }, {'T', 2 },
                                       {'u', 2 }, {'U', 2 },
                                       {'v', 2 }, {'V', 2 },
                                       {'w', 2 }, {'W', 2 },
                                       {'x', 2 }, {'X', 2 },
                                       {'y', 2 }, {'Y', 2 },
                                       {'z', 2 }, {'Z', 2 }
            }
        );

        //Dict for variables
        public Dictionary<string, int> variables = new Dictionary<string, int>() { };

        public byte AttrTabSearch(char symbol)
        {
            if (attributes.ContainsKey(symbol))
                return attributes[symbol];
            return 5;
        }

        public Queue<int> lexList;
    }
}

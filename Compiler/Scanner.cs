using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using static Compiler.Tables;

namespace Compiler
{
    class Scanner
    {
        StreamWriter fileToWrite;
        Stream file;
        Tables tables = new Tables();

        public Scanner(string fileName)
        {
            file = new FileStream(fileName, FileMode.Open);
            fileToWrite = new StreamWriter(fileName.Remove((int)fileName.Length - 4, 4) + ".txt");
        }

        tsymbol Gets()
        {
            tsymbol newSymbol;
            int newByte = file.ReadByte();

            if (newByte == -1)
            {
                newSymbol.value = ' ';
                newSymbol.attr = 6;
                return newSymbol;
            }

            newSymbol.value = Convert.ToChar(newByte);
            newSymbol.attr = tables.AttrTabSearch(newSymbol.value);
            return newSymbol;
        }

        public void Scan(ref Queue<int> lexList, ref Dictionary<string, int> variables, ref Dictionary<string, int> constTable)
        {
            tsymbol newSymbol = Gets();
            bool outputFlag;
            bool pointFlag;
            int lexCode = 0;
            StringBuilder buffer = new StringBuilder("");
            string strBuffer = "";

            while (newSymbol.attr != 6)
            {
                buffer.Clear();
                outputFlag = false;
                switch (newSymbol.attr)
                {
                    case 0:
                        {
                            while ((newSymbol.attr != 6) && (newSymbol.attr == 0))
                                newSymbol = Gets();
                            outputFlag = true;
                            break;
                        }
                    case 1:
                        {
                            pointFlag = false;
                            while ((newSymbol.attr != 6) && ((newSymbol.attr == 1) || ((newSymbol.value == '.')
                                                                                  && (pointFlag != true))))
                            {
                                if (newSymbol.value == '.')
                                    pointFlag = true;
                                buffer.Append(newSymbol.value);
                                newSymbol = Gets();
                            }
                            strBuffer = buffer.ToString();

                            if (constTable.ContainsKey(strBuffer))
                                lexCode = constTable[strBuffer];
                            else
                            {
                                lexCode = constTable.Count + 301;
                                constTable[strBuffer] = lexCode;
                            }
                            break;
                        }
                    case 2:
                        {
                            while ((newSymbol.attr != 6) && ((newSymbol.attr == 1) || (newSymbol.attr == 2)))
                            {
                                buffer.Append(newSymbol.value);
                                newSymbol = Gets();
                            }
                            strBuffer = buffer.ToString();

                            if (tables.keyWords.ContainsKey(strBuffer))
                                lexCode = tables.keyWords[strBuffer];
                            else
                            {
                                if (variables.ContainsKey(strBuffer))
                                    lexCode = variables[strBuffer];
                                else
                                {
                                    lexCode = variables.Count + tables.keyWords.Count + 501;
                                    variables[strBuffer] = lexCode;
                                }
                            }
                            break;
                        }
                    case 3:
                        {
                            newSymbol = Gets();
                            if (newSymbol.attr == 6)
                            {
                                lexCode = '(';
                                strBuffer = "(";
                            }
                            else
                            {
                                if (newSymbol.value == '*')
                                {
                                    do
                                    {
                                        while ((newSymbol.value != '*') && (newSymbol.attr != 6))
                                        {
                                            newSymbol = Gets();
                                        }
                                        if (newSymbol.attr == 6)
                                        {
                                            throw new Exception("'*)' expected, but end of file found");
                                            newSymbol.value = '+';
                                            break;
                                        }
                                        else
                                            newSymbol = Gets();
                                    } while (newSymbol.value != ')');

                                    if (newSymbol.value == ')')
                                    {
                                        outputFlag = true;
                                        newSymbol = Gets();
                                    }
                                }
                                else
                                {
                                    lexCode = '(';
                                    strBuffer = "(";
                                }
                            }
                            break;
                        }
                    case 4:
                        {
                            lexCode = newSymbol.value;
                            buffer.Append(newSymbol.value);
                            newSymbol = Gets();
                            if (newSymbol.attr == 4)
                            {
                                buffer.Append(newSymbol.value);
                                strBuffer = buffer.ToString();

                                if (tables.compOperators.ContainsKey(strBuffer))
                                {
                                    lexCode = tables.compOperators[strBuffer];
                                    newSymbol = Gets();
                                }
                                else
                                    throw new Exception("There is no such multi-character delimiter");
                            }
                            strBuffer = buffer.ToString();
                            break;
                        }
                    case 5:
                        {
                            throw new Exception("Illegal symbol");
                            newSymbol = Gets();
                            break;
                        }
                }

                if (!outputFlag)
                {
                    fileToWrite.WriteLine("{0} : {1}", strBuffer, lexCode);
                    lexList.Enqueue(lexCode);
                }

            }
            fileToWrite.WriteLine();
            fileToWrite.WriteLine();
            fileToWrite.WriteLine("Constants table");
            fileToWrite.WriteLine("----------------------------");
            fileToWrite.WriteLine("|Name\t\t|\t\tCode\t|");
            fileToWrite.WriteLine("----------------------------");

            foreach (KeyValuePair<string, int> kvp in constTable)
                fileToWrite.WriteLine("|\t{0}\t\t\t{1}\t", kvp.Key, kvp.Value);
            fileToWrite.WriteLine("----------------------------");

            fileToWrite.WriteLine();
            fileToWrite.WriteLine();
            fileToWrite.WriteLine("Variables table");
            fileToWrite.WriteLine("----------------------------");
            fileToWrite.WriteLine("|Name\t\t|\t\tCode\t|");
            fileToWrite.WriteLine("----------------------------");

            foreach (KeyValuePair<string, int> kvp in variables)
                fileToWrite.WriteLine("|\t{0}\t\t\t{1}\t", kvp.Key, kvp.Value);
            fileToWrite.WriteLine("----------------------------");

            file.Dispose();
            fileToWrite.Dispose();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Compiler
{
    class Program
    {
        static void Main(string[] args)
        {
            Scanner scanner = new Scanner("PTEST.DAT");
            Parser parser = new Parser();
            Generator generator = new Generator();
            Tables tables = new Tables();

            tables.lexList = new Queue<int>();
            tables.variables = new Dictionary<string, int>();
            tables.constTable = new Dictionary<string, int>() { };

            TreeView treeView = new TreeView();
            try
            {
                scanner.Scan(ref tables.lexList, ref tables.variables, ref tables.constTable);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            try
            {
                parser.SingleProgram(tables.lexList, tables.variables, ref treeView, tables.constTable);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            try
            {
                generator.Generate(treeView.Nodes[0]);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            generator.fileToWrite.Close();
        }
    }
}

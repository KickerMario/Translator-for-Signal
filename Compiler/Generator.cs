using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace Compiler
{
    class Generator
    {
        public StreamWriter fileToWrite;
        List<string> vars;

        public Generator()
        {
            fileToWrite = new StreamWriter("listing.asm");
            vars = new List<string>();
        }

        ~Generator()
        {
            fileToWrite.Close();
        }

        public void Generate(TreeNode tree, int labelsCount = 1)
        {
            string rule = tree.Text;
            

            switch (rule)
            {
                case "<program>":
                    {
                        Generate(tree.Nodes[3], labelsCount);
                        break;
                    }
                case "<block>":
                    {
                        Generate(tree.Nodes[0], labelsCount);
                        fileToWrite.WriteLine("CODE SEGMENT");
                        fileToWrite.WriteLine("START:");
                        Generate(tree.Nodes[2], labelsCount);
                        fileToWrite.WriteLine("CODE ENDS");
                        fileToWrite.WriteLine("END START");
                        break;
                    }
                case "<variable-declarations>":
                    {
                        fileToWrite.WriteLine("DATA SEGMENT");
                        Generate(tree.Nodes[1], labelsCount);
                        fileToWrite.WriteLine("DATA ENDS\n");
                        break;
                    }
                case "<declarations-list>":
                    {
                        Generate(tree.Nodes[0], labelsCount);
                        if (tree.Nodes.Count > 1)
                            Generate(tree.Nodes[1], labelsCount);
                        break;
                    }
                case "<declaration>":
                    {
                        if (vars.Contains(tree.Nodes[0].Text))
                            throw new Exception("Double declaration");
                        vars.Add(tree.Nodes[0].Text);
                        fileToWrite.WriteLine("\t" + tree.Nodes[0].Text + "\tDD\t?");
                        break;
                    }
                case "<statements-list>":
                    {
                        Generate(tree.Nodes[0], labelsCount);
                        if (tree.Nodes.Count > 1)
                            Generate(tree.Nodes[1], labelsCount + 2);
                        break;
                    }
                case "<statement>":
                    {
                        fileToWrite.WriteLine();    
                        fileToWrite.WriteLine("L" + labelsCount.ToString() + ": NOP");
                        Generate(tree.Nodes[1], labelsCount + 1);
                        Generate(tree.Nodes[3], labelsCount + 2);
                        fileToWrite.WriteLine("JMP L" + labelsCount.ToString());
                        fileToWrite.WriteLine("L" + (labelsCount + 1).ToString() + ": NOP");
                        fileToWrite.WriteLine();
                        break;
                    }
                case "<conditional-expression>":
                    {
                        fileToWrite.WriteLine("MOV AX, " + tree.Nodes[0].Text);
                        fileToWrite.WriteLine("MOV BX, " + tree.Nodes[2].Text);
                        fileToWrite.WriteLine("CMP AX, BX");
                        Generate(tree.Nodes[1], labelsCount);
                        break;
                    }
                case "=":
                    {
                        fileToWrite.WriteLine("JE L" + labelsCount.ToString());
                        break;
                    }
                case "<":
                    {
                        fileToWrite.WriteLine("JL L" + labelsCount.ToString());
                        break;
                    }
                case ">":
                    {
                        fileToWrite.WriteLine("JG L" + labelsCount.ToString());
                        break;
                    }
                case "<=":
                    {
                        fileToWrite.WriteLine("JLE L" + labelsCount.ToString());
                        break;
                    }
                case ">=":
                    {
                        fileToWrite.WriteLine("JGE L" + labelsCount.ToString());
                        break;
                    }
                case "<>":
                    {
                        fileToWrite.WriteLine("JNE L" + labelsCount.ToString());
                        break;
                    }
                case "<empty>":
                    {
                        break;
                    }
            }
        }
    }
}

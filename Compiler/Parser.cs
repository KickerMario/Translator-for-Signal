using Compiler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Compiler
{
    class Parser
    {
        Tables tables = new Tables();

        public TreeView SingleProgram(Queue<int> lexList, Dictionary<string, int> variables, ref TreeView tree, Dictionary<string, int> constTable)
        {
            tree.Nodes.Add(Program(lexList, variables, constTable));
            return tree;
        }

        public TreeNode Program(Queue<int> lexList, Dictionary<string, int> variables, Dictionary<string, int> constTable)
        {
            TreeNode node = new TreeNode("<program>");
            if (lexList.Dequeue() != 501)
                throw new Exception("'PROGRAM' hasn't been found");

            node.Nodes.Add("PROGRAM");
            node.Nodes.Add(ProcedureIdentifier(lexList, variables));

            if (lexList.Dequeue() != 59)
                throw new Exception("Semi-colon is required");
            node.Nodes.Add(";");

            node.Nodes.Add(Block(lexList, variables, constTable));
            return node;
        }

        TreeNode ProcedureIdentifier(Queue<int> lexList, Dictionary<string, int> variables)
        {
            int lexCode = lexList.Dequeue();
            if (!variables.ContainsValue(lexCode))
                throw new Exception("There is no procedure identifier");
            TreeNode node = new TreeNode("<procedure-identifier>");
            node.Nodes.Add(variables.FirstOrDefault(x => x.Value == lexCode).Key);
            return node;
        }

        TreeNode Block(Queue<int> lexList, Dictionary<string, int> variables, Dictionary<string, int> constTable)
        {
            TreeNode node = new TreeNode("<block>");

            node.Nodes.Add(VariableDeclarations(lexList, variables));

            if (lexList.Dequeue() != 503)
                throw new Exception("'BEGIN' expected");
            node.Nodes.Add("BEGIN");

            node.Nodes.Add(StatementsList(lexList, variables, constTable));

            if (lexList.Dequeue() != 504)
                throw new Exception("'END' expected");
            node.Nodes.Add("END");

            return node;
        }

        TreeNode VariableDeclarations(Queue<int> lexList, Dictionary<string, int> variables)
        {
            TreeNode node = new TreeNode("<variable-declarations>");
            if (lexList.First() == 502)
            {
                node.Nodes.Add("VAR");
                lexList.Dequeue();
                node.Nodes.Add(DeclarationsList(lexList, variables));
                return node;
            }
            node.Nodes.Add("<empty>");
            return node;
        }

        TreeNode DeclarationsList(Queue<int> lexList, Dictionary<string, int> variables, int recLevel = 1)
        {
            TreeNode node = new TreeNode("<declarations-list>");
            if (!variables.ContainsValue(lexList.First()))
            {
                if (recLevel == 1)
                    throw new Exception("Identifier expected");
                else
                {
                    node.Nodes.Add("<empty>");
                    return node;
                }
            }
            else
            {
                node.Nodes.Add(Declaration(lexList, variables));
                node.Nodes.Add(DeclarationsList(lexList, variables, ++recLevel));
            }
            return node;
        }

        TreeNode Declaration(Queue<int> lexList, Dictionary<string, int> variables)
        {
            TreeNode node = new TreeNode("<declaration>");

            int lexCode = lexList.Dequeue();
            if (!variables.ContainsValue(lexCode))
                throw new Exception("Identifier expected");
            node.Nodes.Add(variables.FirstOrDefault(x => x.Value == lexCode).Key);

            lexCode = lexList.Dequeue();
            if (lexCode != 58)
                throw new Exception("':' expected");
            node.Nodes.Add(":");

            lexCode = lexList.Dequeue();
            if ((lexCode != 507) && (lexCode != 508))
                throw new Exception("Type of variable expected");
            node.Nodes.Add(tables.keyWords.FirstOrDefault(x => x.Value == lexCode).Key);

            if (lexList.Dequeue() != 59)
                throw new Exception("Semi-colon is required");
            node.Nodes.Add(";");

            return node;
        }

        TreeNode StatementsList(Queue<int> lexList, Dictionary<string, int> variables, Dictionary<string, int> constTable)
        {
            TreeNode node = new TreeNode("<statements-list>");

            if ((lexList.First() == 504) || (lexList.First() == 506))
            {
                node.Nodes.Add("<empty>");
                return node;
            }
            else
            {
                node.Nodes.Add(Statement(lexList, variables, constTable));
                node.Nodes.Add(StatementsList(lexList, variables, constTable));
            }
            return node;
        }

        TreeNode Statement(Queue<int> lexList, Dictionary<string, int> variables, Dictionary<string, int> constTable)
        {
            TreeNode node = new TreeNode("<statement>");

            if (lexList.Dequeue() != 505)
                throw new Exception("'WHILE' expected");
            node.Nodes.Add("WHILE");

            node.Nodes.Add(ConditionalExpression(lexList, variables, constTable));

            if (lexList.Dequeue() != 509)
                throw new Exception("'DO' expected");
            node.Nodes.Add("DO");

            node.Nodes.Add(StatementsList(lexList, variables, constTable));

            if (lexList.Dequeue() != 506)
                throw new Exception("'ENDWHILE' expected");
            node.Nodes.Add("ENDWHILE");

            if (lexList.Dequeue() != 59)
                throw new Exception("';' expected");
            node.Nodes.Add(";");

            return node;
        }

        TreeNode ConditionalExpression(Queue<int> lexList, Dictionary<string, int> variables, Dictionary<string, int> constTable)
        {
            TreeNode node = new TreeNode("<conditional-expression>");

            node.Nodes.Add(Expression(lexList, variables, constTable));
            node.Nodes.Add(ComparisonOperator(lexList));
            node.Nodes.Add(Expression(lexList, variables, constTable));

            return node;
        }

        TreeNode Expression(Queue<int> lexList, Dictionary<string, int> variables, Dictionary<string, int> constTable)
        {
            TreeNode node = new TreeNode("<expression>");
            int lexCode = lexList.Dequeue();

            if (variables.ContainsValue(lexCode))
                return node.Nodes.Add(variables.FirstOrDefault(x => x.Value == lexCode).Key);

            if (constTable.ContainsValue(lexCode))
                return node.Nodes.Add(constTable.FirstOrDefault(x => x.Value == lexCode).Key);

            throw new Exception("Expression expected");
        }

        TreeNode ComparisonOperator(Queue<int> lexList)
        {
            TreeNode node = new TreeNode("<comparison-operator>");

            int lexCode = lexList.Dequeue();
            if (tables.compOperators.ContainsValue(lexCode))
                return node.Nodes.Add(tables.compOperators.FirstOrDefault(x => x.Value == lexCode).Key);
            throw new Exception("Comparison operator expected");
        }
    }
}

using System;
using System.Collections.Generic;

namespace QuestFormula
{
    public static class FormulaEvaluator
    {
        public static int Evaluate(string expr, Dictionary<string, int> vars)
        {
            Parser parser = new Parser(expr, vars);
            int result = parser.ParseExpression();
            parser.EnsureEnd();
            return result;
        }

        public static bool EvaluateBool(string expr, Dictionary<string, int> vars)
        {
            return Evaluate(expr, vars) != 0;
        }

        private class Parser
        {
            private readonly string s;
            private readonly Dictionary<string, int> vars;
            private int pos;

            public Parser(string s, Dictionary<string, int> vars)
            {
                this.s = s;
                this.vars = vars;
            }

            public void EnsureEnd()
            {
                SkipSpaces();

                if (pos < s.Length)
                    throw new Exception("Unexpected tail: " + s.Substring(pos));
            }

            private char Peek() => pos < s.Length ? s[pos] : '\0';

            private char Next() => pos < s.Length ? s[pos++] : '\0';

            private void SkipSpaces()
            {
                while (char.IsWhiteSpace(Peek()))
                    pos++;
            }

            public int ParseExpression()
            {
                SkipSpaces();

                int condition = ParseOr();

                SkipSpaces();

                if (Peek() == '?')
                {
                    Next();

                    int trueExpr = ParseExpression();

                    SkipSpaces();
                    if (Next() != ':')
                        throw new Exception("Expected ':'");

                    int falseExpr = ParseExpression();

                    return condition != 0 ? trueExpr : falseExpr;
                }

                return condition;
            }

            private int ParseOr()
            {
                int left = ParseAnd();

                while (true)
                {
                    SkipSpaces();

                    if (Match("||"))
                    {
                        int right = ParseAnd();
                        left = Bool(left != 0 || right != 0);
                    }
                    else
                    {
                        break;
                    }
                }

                return left;
            }

            private int ParseAnd()
            {
                int left = ParseComparison();

                while (true)
                {
                    SkipSpaces();

                    if (Match("&&"))
                    {
                        int right = ParseComparison();
                        left = Bool(left != 0 && right != 0);
                    }
                    else
                    {
                        break;
                    }
                }

                return left;
            }

            private int ParseComparison()
            {
                int left = ParseAddSub();

                while (true)
                {
                    SkipSpaces();

                    if (Match("=="))
                        left = Bool(left == ParseAddSub());
                    else if (Match("!="))
                        left = Bool(left != ParseAddSub());
                    else if (Match(">="))
                        left = Bool(left >= ParseAddSub());
                    else if (Match("<="))
                        left = Bool(left <= ParseAddSub());
                    else if (Match(">"))
                        left = Bool(left > ParseAddSub());
                    else if (Match("<"))
                        left = Bool(left < ParseAddSub());
                    else
                        break;
                }

                return left;
            }

            private int ParseAddSub()
            {
                int value = ParseMulDiv();

                while (true)
                {
                    SkipSpaces();

                    if (Peek() == '+')
                    {
                        Next();
                        value += ParseMulDiv();
                    }
                    else if (Peek() == '-')
                    {
                        Next();
                        value -= ParseMulDiv();
                    }
                    else
                    {
                        break;
                    }
                }

                return value;
            }

            private int ParseMulDiv()
            {
                int value = ParseUnary();

                while (true)
                {
                    SkipSpaces();

                    if (Peek() == '*')
                    {
                        Next();
                        value *= ParseUnary();
                    }
                    else if (Peek() == '/')
                    {
                        Next();

                        int divisor = ParseUnary();

                        if (divisor == 0)
                            throw new Exception("Division by zero");

                        value /= divisor;
                    }
                    else
                    {
                        break;
                    }
                }

                return value;
            }

            private int ParseUnary()
            {
                SkipSpaces();

                if (Peek() == '!')
                {
                    Next();
                    return Bool(ParseUnary() == 0);
                }

                if (Peek() == '-')
                {
                    Next();
                    return -ParseUnary();
                }

                return ParsePrimary();
            }

            private int ParsePrimary()
            {
                SkipSpaces();

                if (Peek() == '(')
                {
                    Next();

                    int value = ParseExpression();

                    SkipSpaces();
                    if (Next() != ')')
                        throw new Exception("Expected ')'");

                    return value;
                }

                if (char.IsDigit(Peek()))
                    return ParseNumber();

                if (char.IsLetter(Peek()))
                    return ParseIdentifier();

                throw new Exception("Unexpected character: " + Peek());
            }

            private int ParseNumber()
            {
                int start = pos;

                while (char.IsDigit(Peek()))
                    Next();

                return int.Parse(s.Substring(start, pos - start));
            }

            private int ParseIdentifier()
            {
                int start = pos;

                while (char.IsLetterOrDigit(Peek()))
                    Next();

                string name = s.Substring(start, pos - start);

                SkipSpaces();

                if (name == "rnd" && Peek() == '(')
                {
                    Next();

                    int min = ParseExpression();

                    SkipSpaces();
                    if (Next() != ',')
                        throw new Exception("Expected ','");

                    int max = ParseExpression();

                    SkipSpaces();
                    if (Next() != ')')
                        throw new Exception("Expected ')'");

                    return UnityEngine.Random.Range(min, max + 1);
                }

                if (vars.TryGetValue(name, out int value))
                    return value;

                throw new Exception("Unknown variable: " + name);
            }

            private bool Match(string token)
            {
                SkipSpaces();

                if (pos + token.Length > s.Length)
                    return false;

                for (int i = 0; i < token.Length; i++)
                {
                    if (s[pos + i] != token[i])
                        return false;
                }

                pos += token.Length;
                return true;
            }

            private int Bool(bool value)
            {
                return value ? 1 : 0;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Calculator_Wpf.Models
{
    public class CalculatorEngine
    {
        public static string Evaluate(string expression)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(expression))
                    return "0";

                string normalized = expression
                    .Replace("×", "*")
                    .Replace("÷", "/")
                    .Replace("−", "-");

                normalized = Regex.Replace(normalized, @"(\d+(?:\.\d+)?)%", "($1/100)");
                normalized = Regex.Replace(normalized, @"(\d)\(", "$1*(");
                normalized = Regex.Replace(normalized, @"\)(\d)", ")*$1");
                normalized = normalized.Replace(")(", ")*(");

                double result = EvaluateExpression(normalized);
                return FormatResult(result);
            }
            catch
            {
                return "Error";
            }
        }

        private static string FormatResult(double value)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
                return "Error";

            if (Math.Abs(value % 1) < 1e-10)
                return ((long)Math.Round(value)).ToString(CultureInfo.InvariantCulture);

            return value.ToString("G15", CultureInfo.InvariantCulture);
        }

        private static double EvaluateExpression(string expression)
        {
            var tokens = Tokenize(expression);
            int index = 0;
            double result = ParseExpression(tokens, ref index);
            if (index < tokens.Count)
                throw new FormatException("Unexpected token");
            return result;
        }

        private static List<string> Tokenize(string expression)
        {
            var tokens = new List<string>();
            int i = 0;

            while (i < expression.Length)
            {
                char c = expression[i];

                if (char.IsWhiteSpace(c))
                {
                    i++;
                    continue;
                }

                if (char.IsDigit(c) || c == '.')
                {
                    int start = i;
                    while (i < expression.Length && (char.IsDigit(expression[i]) || expression[i] == '.'))
                        i++;
                    tokens.Add(expression[start..i]);
                    continue;
                }

                if (char.IsLetter(c))
                {
                    int start = i;
                    while (i < expression.Length && char.IsLetter(expression[i]))
                        i++;
                    tokens.Add(expression[start..i].ToLowerInvariant());
                    continue;
                }

                if ("+-*/^()!".Contains(c))
                {
                    tokens.Add(c.ToString());
                    i++;
                    continue;
                }

                throw new FormatException($"Invalid character: {c}");
            }

            return tokens;
        }

        private static double ParseExpression(List<string> tokens, ref int index)
        {
            double value = ParseTerm(tokens, ref index);

            while (index < tokens.Count && (tokens[index] == "+" || tokens[index] == "-"))
            {
                string op = tokens[index++];
                double right = ParseTerm(tokens, ref index);
                value = op == "+" ? value + right : value - right;
            }

            return value;
        }

        private static double ParseTerm(List<string> tokens, ref int index)
        {
            double value = ParsePower(tokens, ref index);

            while (index < tokens.Count && (tokens[index] == "*" || tokens[index] == "/"))
            {
                string op = tokens[index++];
                double right = ParsePower(tokens, ref index);
                value = op == "*" ? value * right : value / right;
            }

            return value;
        }

        private static double ParsePower(List<string> tokens, ref int index)
        {
            double value = ParseUnary(tokens, ref index);

            if (index < tokens.Count && tokens[index] == "^")
            {
                index++;
                double exponent = ParseUnary(tokens, ref index);
                value = Math.Pow(value, exponent);
            }

            return value;
        }

        private static double ParseUnary(List<string> tokens, ref int index)
        {
            if (index < tokens.Count && tokens[index] == "-")
            {
                index++;
                return -ParseUnary(tokens, ref index);
            }

            if (index < tokens.Count && tokens[index] == "+")
            {
                index++;
                return ParseUnary(tokens, ref index);
            }

            return ParsePrimary(tokens, ref index);
        }

        private static double ParsePrimary(List<string> tokens, ref int index)
        {
            if (index >= tokens.Count)
                throw new FormatException("Unexpected end of expression");

            if (tokens[index] == "(")
            {
                index++;
                double value = ParseExpression(tokens, ref index);
                if (index >= tokens.Count || tokens[index] != ")")
                    throw new FormatException("Missing closing parenthesis");
                index++;
                return value;
            }

            if (tokens[index] == "sqrt")
            {
                index++;
                if (index >= tokens.Count || tokens[index] != "(")
                    throw new FormatException("Expected '(' after sqrt");
                index++;
                double value = ParseExpression(tokens, ref index);
                if (index >= tokens.Count || tokens[index] != ")")
                    throw new FormatException("Missing closing parenthesis");
                index++;
                if (value < 0)
                    throw new FormatException("Invalid sqrt operand");
                return Math.Sqrt(value);
            }

            if (double.TryParse(tokens[index], NumberStyles.Float, CultureInfo.InvariantCulture, out double number))
            {
                index++;

                if (index < tokens.Count && tokens[index] == "!")
                {
                    index++;
                    if (number < 0 || Math.Abs(number % 1) > 1e-10)
                        throw new FormatException("Invalid factorial operand");
                    return Factorial((int)Math.Round(number));
                }

                return number;
            }

            throw new FormatException($"Unexpected token: {tokens[index]}");
        }

        private static double Factorial(int n)
        {
            if (n < 0)
                throw new FormatException("Invalid factorial operand");

            double result = 1;
            for (int i = 2; i <= n; i++)
                result *= i;
            return result;
        }
    }
}

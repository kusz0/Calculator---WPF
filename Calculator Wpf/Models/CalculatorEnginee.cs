using System;
using System.Data; 

namespace Calculator_Wpf.Models
{
    public static class CalculatorEngine
    {
        public static string Evaluate(string expression)
        {
            try
            {
               
                string formattedExpression = expression
                    .Replace("×", "*")
                    .Replace("÷", "/");

                
                formattedExpression = formattedExpression.Replace("(", "*(");
                
                formattedExpression = formattedExpression.Replace("**", "*");

                
                var table = new DataTable();
                var result = table.Compute(formattedExpression, "");

              
                return Convert.ToDouble(result).ToString();
            }
            catch (Exception)
            {
                return "Error";
            }
        }
    }
}
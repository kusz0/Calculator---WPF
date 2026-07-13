using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Calculator_Wpf
{
    class CalculatorEnginee 
    {
            public static string Evaluate(string expression)
            {
                try
                {
                    string formattedExpression = expression
                        .Replace("×", "*")
                        .Replace("÷", "/");


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





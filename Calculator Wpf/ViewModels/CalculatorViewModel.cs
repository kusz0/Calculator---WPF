using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows.Input;
using Calculator_Wpf.Models;

namespace Calculator_Wpf.ViewModels
{
    public class CalculatorViewModel : INotifyPropertyChanged
    {
        private string _displayText = "0";

        public string DisplayText
        {
            get => _displayText;
            set { _displayText = value; OnPropertyChanged(); }
        }

        public ICommand InputCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand CalculateCommand { get; }
        public ICommand SignCommand { get; }
        public ICommand PercentCommand { get; }

        public CalculatorViewModel()
        {
            InputCommand = new RelayCommand(ExecuteInput);
            ClearCommand = new RelayCommand(_ => DisplayText = "0");
            CalculateCommand = new RelayCommand(_ => CalculateResult());
            SignCommand = new RelayCommand(_ => ExecuteSign());
            PercentCommand = new RelayCommand(_ => ExecutePercent());
        }

        private void ExecuteInput(object parameter)
        {
            if (parameter is not string input)
                return;

            if (DisplayText == "Error")
                DisplayText = "0";

            if (DisplayText == "0" && input != "." && input != "(" && input != "-" && !IsOperator(input))
            {
                DisplayText = input;
                return;
            }

            if (DisplayText == "0" && IsOperator(input) && input != "-")
            {
                DisplayText += input;
                return;
            }

            if (IsOperator(input) && DisplayText.Length > 0 && IsOperator(DisplayText[^1].ToString()))
            {
                DisplayText = DisplayText[..^1] + input;
                return;
            }

            DisplayText += input;
        }

        private static bool IsOperator(string input) =>
            input is "+" or "-" or "×" or "÷" or "−" or "*" or "/" or "^";

        private void ExecuteSign()
        {
            if (DisplayText == "Error")
            {
                DisplayText = "0";
                return;
            }

            var match = Regex.Match(DisplayText, @"(-?\d+\.?\d*)$");
            if (match.Success)
            {
                string number = match.Value;
                string toggled = number.StartsWith("-") ? number[1..] : "-" + number;
                DisplayText = DisplayText[..match.Index] + toggled;
                return;
            }

            if (DisplayText == "0" || DisplayText.EndsWith("("))
            {
                DisplayText += "-";
            }
        }

        private void ExecutePercent()
        {
            if (DisplayText == "Error")
            {
                DisplayText = "0";
                return;
            }

            if (!DisplayText.EndsWith("%"))
                DisplayText += "%";
        }

        private void CalculateResult()
        {
            DisplayText = CalculatorEngine.Evaluate(DisplayText);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}

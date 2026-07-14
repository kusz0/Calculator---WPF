using Calculator_Wpf.ViewModels;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Calculator_Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ResultScroll.ScrollToRightEnd();
        }


        private void ExpandBtn_Click(object sender, RoutedEventArgs e)
        {
            if (AdvancedModeSection.Visibility == Visibility.Collapsed)
            {
                AdvancedModeSection.Visibility = Visibility.Visible;
                expandBtn.Content = "-";
                expandBtn.Width = 100;
                ResultScroll.MaxWidth = 540;
                
            }
            else
            {
                AdvancedModeSection.Visibility = Visibility.Collapsed;
                expandBtn.Content = "+";
                expandBtn.Width = 30;
                ResultScroll.MaxWidth = 390;
            }
        }
    }
}
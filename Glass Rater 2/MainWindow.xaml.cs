using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Glass_Rater_2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        GlassPanelController gpc;
        public MainWindow()
        {
            InitializeComponent();
            gpc = new GlassPanelController();
        }

        private void calcRequiredThickness_Click(object sender, RoutedEventArgs e)
        {
            setThicknessBox(monoBox, "Annealed");
            setThicknessBox(lamBox, "Laminated");
            setThicknessBox(heatBox, "Heated");
            setThicknessBox(toughBox, "Toughened");
        }

        private void setThicknessBox(TextBox box, string glassType)
        {
            int width = stringToInt(widthBox.Text);
            int height = stringToInt(heightBox.Text);
            string pressureComboBoxString = ((ComboBoxItem)pressureComboBox.SelectedItem).Content as string;
            
            int thickness = gpc.getRequiredGlassThckness(width, height, glassType, pressureComboBoxString);
            if (thickness == 0) { box.Text = "Unknown."; }
            else { box.Text = thickness.ToString(); }
        }

        private int stringToInt(string text)
        {
            int val;
            int.TryParse(text, out val);
            return val;
        }
    }
}

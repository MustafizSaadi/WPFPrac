using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AutoLotDAL.Repos;

namespace WpfTabControls
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Be in Ink mode by default.
            this.MyInkCanvas.EditingMode = InkCanvasEditingMode.Ink;
            this.inkRadio.IsChecked = true;
            this.comboColors.SelectedIndex = 0;

            SetBindings();
            ConfigureGrid();
        }

        private void ConfigureGrid()
        {
            using (var repo = new InventoryRepo())
            {
                // Build a LINQ query that gets back some data from the Inventory table.
                gridInventory.ItemsSource = repo.GetAll().Select(x => new { x.Id, x.Make, x.Color, x.PetName });
            }
        }

        private void SetBindings()
        {
            // Create a Binding object.
            Binding b = new Binding();
            // Register the converter, source, and path.
            b.Converter = new MyDoubleConverter();
            b.Source = this.mySB;
            b.Path = new PropertyPath("Value");
            // Call the SetBinding method on the Label.
            this.labelSBThumb.SetBinding(Label.ContentProperty, b);
        }

        private void RadioButtonClicked(object sender, RoutedEventArgs e)
        {
            // Based on which button sent the event, place the InkCanvas in a unique
            // mode of operation.
            switch ((sender as RadioButton)?.Content.ToString())
            {
                // These strings must be the same as the Content values for each
                // RadioButton.
                case "Ink Mode!":
                    this.MyInkCanvas.EditingMode = InkCanvasEditingMode.Ink;
                    break;
                case "Erase Mode!":
                    this.MyInkCanvas.EditingMode = InkCanvasEditingMode.EraseByStroke;
                    break;
                case "Select Mode!":
                    this.MyInkCanvas.EditingMode = InkCanvasEditingMode.Select;
                    break;
            }
        }

        private void ColorChanged(object sender, SelectionChangedEventArgs e)
        {
            string colorToUse = (this.comboColors.SelectedItem as StackPanel)?.Tag.ToString();
            // Change the color used to render the strokes.
            this.MyInkCanvas.DefaultDrawingAttributes.Color = (Color)ColorConverter.ConvertFromString(colorToUse);
        }

        private void SaveData(object sender, RoutedEventArgs e)
        {
            // Save all data on the InkCanvas to a local file.
            using (FileStream fs = new FileStream("StrokeData.bin", FileMode.Create))
            {
                this.MyInkCanvas.Strokes.Save(fs);
                fs.Close();
            }
        }

        private void LoadData(object sender, RoutedEventArgs e)
        {
            // Fill StrokeCollection from file.
            using (FileStream fs = new FileStream("StrokeData.bin", FileMode.Open, FileAccess.Read))
            {
                StrokeCollection strokes = new StrokeCollection(fs);
                this.MyInkCanvas.Strokes = strokes;
            }
        }

        private void Clear(object sender, RoutedEventArgs e)
        {
            // Clear all strokes.
            this.MyInkCanvas.Strokes.Clear();
        }
    }
}

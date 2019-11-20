/*
 * Author: Samuel Bishop
 * Filename: MainWindow.xaml.cs
 * Date Created: 05/02/18
 * Modifications: 5/4 - added save/load functionality
 *      5/8 - added undo functionality
 *      5/9 - added keyboard shortcuts
 */
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
using System.IO;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Paint
{
    [Serializable]
    public partial class MainWindow : Window
    {
        /*
         * Class: MyShape
         * 
         * Purpose: This class allows the creation of a custom shape object, to be drawn on the canvas.
         *      custom shape is used so that a list of MyShapes can be serialized for save/load.
         *      
         * Manager Functions: 
         *      public MyShape() 
         *      public MyShape(DrawingInstrument d, Color borderC, Color fillC, bool fillS, Point firstP, Point secondP, double thick)
         */
        public class MyShape
        {
            public MyShape()
            {
                m_borderColor = Colors.Black;
                m_fillColor = Colors.Black;
                m_thickness = 1;
                m_fillSelected = true;
                m_instrument = DrawingInstrument.PENC;
                m_pc = new PointCollection();
                m_drawing = false;
            }
            public MyShape(DrawingInstrument d, Color borderC, Color fillC, bool fillS, Point firstP, Point secondP, double thick)
            {
                m_instrument = d;
                m_borderColor = borderC;
                m_fillColor = fillC;
                m_fillSelected = fillS;
                m_firstPoint = firstP;
                m_secondPoint = secondP;
                m_thickness = thick;
                m_pc = new PointCollection();
                m_drawing = false;
            }
            public enum DrawingInstrument { RECT, ELLI, LINE, PENC };
            public DrawingInstrument m_instrument { get; set; }
            public Color m_borderColor { get; set; }
            public Color m_fillColor { get; set; }
            public bool m_fillSelected { get; set; }
            public Point m_firstPoint { get; set; }
            public Point m_secondPoint { get; set; }
            public double m_thickness { get; set; }
            public PointCollection m_pc { get; set; }
            public bool m_drawing { get; set; }
        }

        public List<MyShape> shapes = new List<MyShape>();
        public MyShape m_shape = new MyShape();
        public PointCollection pc;

        public MainWindow()
        {
            InitializeComponent();
        }
        /*
         * Purpose: This function allows the user to choose an xml file and open
         *      it onto the canvas.
         *      
         * Precondition: xml file exists and has a list of MyShape objects within it.
         * 
         * Postcondition: those objects are deserialized and drawn on the canvas. Canvas
         *      is cleared before drawing.
         */
        private void OpenClick(object sender, RoutedEventArgs e)
        {
            string filename = "";

            Microsoft.Win32.OpenFileDialog openFileDialog = new
                Microsoft.Win32.OpenFileDialog();
            openFileDialog.InitialDirectory = "";

            if (openFileDialog.ShowDialog() == true)
            {
                filename = openFileDialog.FileName;
            }

            var serializer = new XmlSerializer(shapes.GetType());
            FileStream fs = new FileStream(filename, FileMode.Open);
            shapes = new List<MyShape>();
            shapes = (List<MyShape>)serializer.Deserialize(fs);
            fs.Close();

            myCanvas.Children.Clear();

            foreach (MyShape shape in shapes)
            {
                drawShape(shape);
            }
        }

        /*
         * Purpose: This function allows the user to save their current canvas into an xml file.
         * 
         * Precondition: None
         * 
         * Postcondition: list of MyShapes are serialized into an xml file.
         */
        private void SaveClick(object sender, RoutedEventArgs e)
        {
            string filename = "";

            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            saveFileDialog.InitialDirectory = "";
            saveFileDialog.ShowDialog();
            if (saveFileDialog.FileName != "")
            {
                filename = saveFileDialog.FileName;

                var serializer = new XmlSerializer(shapes.GetType());
                TextWriter writer = new StreamWriter(filename);
                serializer.Serialize(writer, shapes);
                writer.Close();
            }
        }

        /*
         * Purpose: This function opens a new about box.
         * 
         * Precondition: None
         * 
         * Postcondition: The About box is displayed. Main Window cannot gain focus until
         *      About box is closed.
         */
        private void AboutClick(object sender, RoutedEventArgs e)
        {
            AboutBox about = new AboutBox
            {
                Owner = this
            };
            about.ShowDialog();
        }

        /*
         * Purpose: This function closes the current window.
         * 
         * Precondition: None
         * 
         * Postcondition: The current window is closed.
         */ 
        private void ExitClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /*
         * Purpose: This function sets the enum to RECT
         * 
         * Precondition: None
         * 
         * Postcondition: The MyShape enum is set to RECT
         */
        private void Rectangle_Button_Click(object sender, RoutedEventArgs e)
        {
            m_shape.m_instrument = MyShape.DrawingInstrument.RECT;
        }
        /*
         * Purpose: This function sets the enum to ELLI
         * 
         * Precondition: None
         * 
         * Postcondition: The MyShape enum is set to ELLI
         */
        private void Ellipse_Button_Click(object sender, RoutedEventArgs e)
        {
            m_shape.m_instrument = MyShape.DrawingInstrument.ELLI;
        }

        /*
        * Purpose: This function sets the enum to LINE
        * 
        * Precondition: None
        * 
        * Postcondition: The MyShape enum is set to LINE
        */
        private void Line_Button_Click(object sender, RoutedEventArgs e)
        {
            m_shape.m_instrument = MyShape.DrawingInstrument.LINE;
        }

        /*
         * Purpose: This function sets the enum to PENC
         * 
         * Precondition: None
         * 
         * Postcondition: The MyShape enum is set to PENC
         */
        private void Pencil_Button_Click(object sender, RoutedEventArgs e)
        {
            m_shape.m_instrument = MyShape.DrawingInstrument.PENC;
        }

        private void Brown_Button_Click(object sender, RoutedEventArgs e)
        {
            if (m_shape.m_fillSelected)
            {
                FillColorButton.Background = new SolidColorBrush(m_shape.m_fillColor = Colors.Brown);
            }
            else
            {
                BackgroundColorButton.Background = new SolidColorBrush(m_shape.m_borderColor = Colors.Brown);
            }
        }

        private void DarkRed_Button_Click(object sender, RoutedEventArgs e)
        {
            if (m_shape.m_fillSelected)
            {
                FillColorButton.Background = new SolidColorBrush(m_shape.m_fillColor = Colors.DarkRed);
            }
            else
            {
                BackgroundColorButton.Background = new SolidColorBrush(m_shape.m_borderColor = Colors.DarkRed);
            }
        }

        private void Red_Button_Click(object sender, RoutedEventArgs e)
        {
            if (m_shape.m_fillSelected)
            {
                FillColorButton.Background = new SolidColorBrush(m_shape.m_fillColor = Colors.Red);
            }
            else
            {
                BackgroundColorButton.Background = new SolidColorBrush(m_shape.m_borderColor = Colors.Red);
            }
        }

        private void DarkOrange_Button_Click(object sender, RoutedEventArgs e)
        {
            if (m_shape.m_fillSelected)
            {
                FillColorButton.Background = new SolidColorBrush(m_shape.m_fillColor = Colors.DarkOrange);
            }
            else
            {
                BackgroundColorButton.Background = new SolidColorBrush(m_shape.m_borderColor = Colors.DarkOrange);
            }
        }

        private void Orange_Button_Click(object sender, RoutedEventArgs e)
        {
            if (m_shape.m_fillSelected)
            {
                FillColorButton.Background = new SolidColorBrush(m_shape.m_fillColor = Colors.Orange);
            }
            else
            {
                BackgroundColorButton.Background = new SolidColorBrush(m_shape.m_borderColor = Colors.Orange);
            }
        }

        private void Yellow_Button_Click(object sender, RoutedEventArgs e)
        {
            if (m_shape.m_fillSelected)
            {
                FillColorButton.Background = new SolidColorBrush(m_shape.m_fillColor = Colors.Yellow);
            }
            else
            {
                BackgroundColorButton.Background = new SolidColorBrush(m_shape.m_borderColor = Colors.Yellow);
            }
        }

        private void LightGreen_Button_Click(object sender, RoutedEventArgs e)
        {
            if (m_shape.m_fillSelected)
            {
                FillColorButton.Background = new SolidColorBrush(m_shape.m_fillColor = Colors.LightGreen);
            }
            else
            {
                BackgroundColorButton.Background = new SolidColorBrush(m_shape.m_borderColor = Colors.LightGreen);
            }
        }

        private void Green_Button_Click(object sender, RoutedEventArgs e)
        {
            if (m_shape.m_fillSelected)
            {
                FillColorButton.Background = new SolidColorBrush(m_shape.m_fillColor = Colors.Green);
            }
            else
            {
                BackgroundColorButton.Background = new SolidColorBrush(m_shape.m_borderColor = Colors.Green);
            }
        }

        private void LightBlue_Button_Click(object sender, RoutedEventArgs e)
        {
            if (m_shape.m_fillSelected)
            {
                FillColorButton.Background = new SolidColorBrush(m_shape.m_fillColor = Colors.LightBlue);
            }
            else
            {
                BackgroundColorButton.Background = new SolidColorBrush(m_shape.m_borderColor = Colors.LightBlue);
            }
        }

        private void Blue_Button_Click(object sender, RoutedEventArgs e)
        {
            if (m_shape.m_fillSelected)
            {
                FillColorButton.Background = new SolidColorBrush(m_shape.m_fillColor = Colors.Blue);
            }
            else
            {
                BackgroundColorButton.Background = new SolidColorBrush(m_shape.m_borderColor = Colors.Blue);
            }
        }

        private void Indigo_Button_Click(object sender, RoutedEventArgs e)
        {
            if (m_shape.m_fillSelected)
            {
                FillColorButton.Background = new SolidColorBrush(m_shape.m_fillColor = Colors.Indigo);
            }
            else
            {
                BackgroundColorButton.Background = new SolidColorBrush(m_shape.m_borderColor = Colors.Indigo);
            }
        }

        private void Violet_Button_Click(object sender, RoutedEventArgs e)
        {
            if (m_shape.m_fillSelected)
            {
                FillColorButton.Background = new SolidColorBrush(m_shape.m_fillColor = Colors.Violet);
            }
            else
            {
                BackgroundColorButton.Background = new SolidColorBrush(m_shape.m_borderColor = Colors.Violet);
            }
        }

        private void Pink_Button_Click(object sender, RoutedEventArgs e)
        {
            if (m_shape.m_fillSelected)
            {
                FillColorButton.Background = new SolidColorBrush(m_shape.m_fillColor = Colors.Pink);
            }
            else
            {
                BackgroundColorButton.Background = new SolidColorBrush(m_shape.m_borderColor = Colors.Pink);
            }
        }

        private void Black_Button_Click(object sender, RoutedEventArgs e)
        {
            if (m_shape.m_fillSelected)
            {
                FillColorButton.Background = new SolidColorBrush(m_shape.m_fillColor = Colors.Black);
            }
            else
            {
                BackgroundColorButton.Background = new SolidColorBrush(m_shape.m_borderColor = Colors.Black);
            }
        }

        private void Gray_Button_Click(object sender, RoutedEventArgs e)
        {
            if (m_shape.m_fillSelected)
            {
                FillColorButton.Background = new SolidColorBrush(m_shape.m_fillColor = Colors.Gray);
            }
            else
            {
                BackgroundColorButton.Background = new SolidColorBrush(m_shape.m_borderColor = Colors.Gray);
            }
        }

        private void White_Button_Click(object sender, RoutedEventArgs e)
        {
            if (m_shape.m_fillSelected)
            {
                FillColorButton.Background = new SolidColorBrush(m_shape.m_fillColor = Colors.White);
            }
            else
            {
                BackgroundColorButton.Background = new SolidColorBrush(m_shape.m_borderColor = Colors.White);
            }
        }
        /*
         *  Purpose: This function checks to see if the MouseButtonState is Pressed; if it is,
         *      it sets the Points of MyShape and draws the shape.
         *      
         *  Precondition: None
         *  
         *  Postcondition: If mouse button is pressed, will draw MyShape.
         */
        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                m_shape.m_firstPoint = e.GetPosition(myCanvas);
                m_shape.m_secondPoint = e.GetPosition(myCanvas);
                m_shape.m_pc = new PointCollection();
                m_shape.m_pc.Add(e.GetPosition(myCanvas));             
                drawShape(m_shape);                
            }
        }

        /*
         *  Purpose: This function triggers when the mouse is moved. It allows real-time feedback on shape creation
         *           for rectangle, ellipse and line. It also allows free-draw to occur during mouse move.
         *           
         *  Precondition: an object registers a mouse movement and calls this function.
         *  
         *  Postcondition: The current shape is drawn on the canvas as the mouse moves.
         */ 
        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (m_shape.m_instrument != MyShape.DrawingInstrument.PENC && e.LeftButton == MouseButtonState.Pressed)
            {
                if (m_shape.m_drawing == true)
                {
                    m_shape.m_secondPoint = e.GetPosition(myCanvas);
                    myCanvas.Children.RemoveAt(myCanvas.Children.Count - 1);
                    drawShape(m_shape);
                }
                else
                {
                    m_shape.m_firstPoint = e.GetPosition(myCanvas);
                    m_shape.m_drawing = true;
                }
            }
            if (m_shape.m_instrument == MyShape.DrawingInstrument.PENC && e.LeftButton == MouseButtonState.Pressed)
            {
                if (myCanvas.Children.Count != 0)
                {
                    myCanvas.Children.RemoveAt(myCanvas.Children.Count - 1);
                }
                m_shape.m_pc.Add(e.GetPosition(myCanvas));
                drawShape(m_shape);
            }            
        }

        /*
         *  Purpose: This function handle the actions that occur when the user releases the mouse button.
         *      It removes the last temporary shape drawn from Canvas_MouseDown, and adds the final shape.
         *      
         *  Precondition: The mouse button has been released.
         *  
         *  Postcondition: The shape is drawn and added to the canvas.
         */
        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Released)
            {
                m_shape.m_secondPoint = e.GetPosition(myCanvas);
                if (m_shape.m_instrument != MyShape.DrawingInstrument.PENC)
                {
                    if (myCanvas.Children.Count != 0)
                    {
                        myCanvas.Children.RemoveAt(myCanvas.Children.Count - 1);
                    }
                    drawShape(m_shape);
                    shapes.Add(m_shape);
                    m_shape = new MyShape(m_shape.m_instrument, m_shape.m_borderColor, m_shape.m_fillColor, m_shape.m_fillSelected, m_shape.m_firstPoint, m_shape.m_secondPoint, m_shape.m_thickness);
                }
                else
                {
                    if (myCanvas.Children.Count != 0)
                    {
                        myCanvas.Children.RemoveAt(myCanvas.Children.Count - 1);
                    }
                    drawShape(m_shape);
                    m_shape.m_pc.Add(e.GetPosition(myCanvas));
                    shapes.Add(m_shape);
                    m_shape = new MyShape(m_shape.m_instrument, m_shape.m_borderColor, m_shape.m_fillColor, m_shape.m_fillSelected, m_shape.m_firstPoint, m_shape.m_secondPoint, m_shape.m_thickness);
                }
            }
        }
        /*
         *  Purpose: This function draws a shape. It takes the information from the MyShape class and
         *      adds it to the canvas.
         *      
         *  Precondition: A MyShape object must be passed in.
         *  
         *  Postcondition: The shape is added to the canvas. For Pencil shapes, the point collection is converted
         *      into a polyline and drawn onto the canvas.
         */ 
        private void drawShape(MyShape shape)
        {
            switch (shape.m_instrument)
            {
                case MyShape.DrawingInstrument.RECT:
                    Rectangle r = new Rectangle();
                    double rectLeft = (shape.m_firstPoint.X < shape.m_secondPoint.X ? shape.m_firstPoint.X : shape.m_secondPoint.X);
                    double rectTop = (shape.m_firstPoint.Y < shape.m_secondPoint.Y ? shape.m_firstPoint.Y : shape.m_secondPoint.Y);

                    r.Width = Math.Abs(shape.m_firstPoint.X - shape.m_secondPoint.X);
                    r.Height = Math.Abs(shape.m_firstPoint.Y - shape.m_secondPoint.Y);
                    r.Fill = new SolidColorBrush(shape.m_fillColor);
                    r.Stroke = new SolidColorBrush(shape.m_borderColor);
                    r.StrokeThickness = shape.m_thickness;

                    Canvas.SetLeft(r, rectLeft);
                    Canvas.SetTop(r, rectTop);

                    myCanvas.Children.Add(r);
                    break;
                case MyShape.DrawingInstrument.ELLI:
                    Ellipse ellipse = new Ellipse();
                    double elliLeft = (shape.m_firstPoint.X < shape.m_secondPoint.X ? shape.m_firstPoint.X : shape.m_secondPoint.X);
                    double elliTop = (shape.m_firstPoint.Y < shape.m_secondPoint.Y ? shape.m_firstPoint.Y : shape.m_secondPoint.Y);

                    ellipse.Width = Math.Abs(shape.m_firstPoint.X - shape.m_secondPoint.X);
                    ellipse.Height = Math.Abs(shape.m_firstPoint.Y - shape.m_secondPoint.Y);
                    ellipse.Fill = new SolidColorBrush(shape.m_fillColor);
                    ellipse.Stroke = new SolidColorBrush(shape.m_borderColor);
                    ellipse.StrokeThickness = shape.m_thickness;

                    Canvas.SetLeft(ellipse, elliLeft);
                    Canvas.SetTop(ellipse, elliTop);

                    myCanvas.Children.Add(ellipse);
                    break;
                case MyShape.DrawingInstrument.LINE:
                    Line line = new Line();

                    line.X1 = shape.m_firstPoint.X;
                    line.X2 = shape.m_secondPoint.X;
                    line.Y1 = shape.m_firstPoint.Y;
                    line.Y2 = shape.m_secondPoint.Y;

                    line.Stroke = new SolidColorBrush(shape.m_fillColor);
                    line.StrokeThickness = shape.m_thickness;

                    myCanvas.Children.Add(line);              
                    break;
                case MyShape.DrawingInstrument.PENC:
                    Polyline poly = new Polyline();
                    poly.Points = shape.m_pc;
                    poly.Stroke = new SolidColorBrush(shape.m_fillColor);
                    poly.StrokeThickness = shape.m_thickness;
                    myCanvas.Children.Add(poly);
                    Console.WriteLine(myCanvas.Children.Count);
                    break;
                default:
                    break;
            }
        }

        private void Fill_Button_Click(object sender, RoutedEventArgs e)
        {
            m_shape.m_fillSelected = true;
            Border_RadioButton.IsChecked = false;
        }

        private void Border_Button_Click(object sender, RoutedEventArgs e)
        {
            m_shape.m_fillSelected = false;
            Fill_RadioButton.IsChecked = false;
        }
 
        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            m_shape.m_thickness = e.NewValue;
        }
        /*
         *  Purpose: This function provides 'Undo' functionality for the user.
         *  
         *  Precondition: None
         *  
         *  Postcondition: The last item is removed from the canvas and from the list of MyShapes
         *      that are used for saving to an xml file.
         */ 
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (myCanvas.Children.Count != 0)
            {            
                myCanvas.Children.RemoveAt(myCanvas.Children.Count - 1);
                if (shapes.Count != 0)
                {
                    shapes.RemoveAt(shapes.Count - 1);
                }
            }
        }
        /*
         *  Purpose: This function checks to see which keys the user is pressing down,
         *      and calls other functions based on key combinations pressed. Supported
         *      operations are Ctrl+Z (undo), Ctrl+O (open), Ctrl+S (save), Alt_F4 (exit),
         *      and Ctrl+H (help/about).
         *      
         *  Precondition: None
         *  
         *  Postcondition: If a supported key shortcut is identified, the appropriate function is called.
         */
        private void MenuItem_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;

            Key key = (e.Key == Key.System ? e.SystemKey : e.Key);

            if (key == Key.LeftShift || key == Key.RightShift
                || key == Key.LeftCtrl || key == Key.RightCtrl
                || key == Key.LeftAlt || key == Key.RightAlt
                || key == Key.LWin || key == Key.RWin)
            {
                return;
            }

            StringBuilder shortcutText = new StringBuilder();
            if ((Keyboard.Modifiers & ModifierKeys.Control) != 0)
            {
                shortcutText.Append("Ctrl+");
            }
            if ((Keyboard.Modifiers & ModifierKeys.Alt) != 0)
            {
                shortcutText.Append("Alt+");
            }
            shortcutText.Append(key.ToString());
            if (shortcutText.ToString() == "Ctrl+Z") 
            {
                MenuItem_Click(sender, e);
            }
            else if (shortcutText.ToString() == "Ctrl+O")
            {
                OpenClick(sender, e);
            }
            else if (shortcutText.ToString() == "Ctrl+S")
            {
                SaveClick(sender, e);
            }
            else if (shortcutText.ToString() == "Alt+F4")
            {
                ExitClick(sender, e);
            }
            else if (shortcutText.ToString() == "Ctrl+H")
            {
                AboutClick(sender, e);
            }
        }

        private void Triangle_Button_Selected(object sender, RoutedEventArgs e)
        {

        }
    }
}
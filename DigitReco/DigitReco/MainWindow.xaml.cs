using System;
using System.Drawing;
using System.Collections.Generic;
using System.IO;
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
using MNIST.IO;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Diagnostics;

namespace DigitReco
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Dll imports and stuff
        // show/hide console
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();
        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        const int SW_HIDE = 0;
        const int SW_SHOW = 5;
        IntPtr handle = GetConsoleWindow();
        static Random random = new Random();
        #endregion

        NeuralNetwork network = new NeuralNetwork(10, 784);
        List<pictureBits> pictures;

        double total = 0;
        double total2 = 0;
        double total3 = 0;
        double correctCount = 0;

        public MainWindow()
        {
            // Show console
            ShowWindow(handle, SW_SHOW);
            InitializeComponent();
        }
        static Random r = new Random();
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Hide();



            #region manual test matrix class
            //double[,] arr = new double[1, 4];

            //for (int i = 0; i < 1; i++)
            //{
            //    for (int k = 0; k < 4; k++)
            //    {
            //        arr[i, k] = r.Next(10);
            //    }
            //}
            //Matrix matrix = new Matrix(1, 4, arr);
            //matrix.print();
            //Console.WriteLine();

            //double[,] arr2 = new double[4, 1];

            //for (int i = 0; i < 4; i++)
            //{
            //    for (int k = 0; k < 1; k++)
            //    {
            //        arr2[i, k] = r.Next(10);
            //    }
            //}
            //Matrix matrix2 = new Matrix(4, 1, arr2);
            //matrix2.print();
            //Console.WriteLine();

            //var mno = matrix2 * matrix;
            //mno.print();
            #endregion
        }

        void start()
        {
            string dir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            network.ReadWeights();
            Hide();
            //start python exe 
            Process.Start(dir + @"\mnist.exe");
            using (WaitCursor wk = new WaitCursor())
            {
                TrainOnMyDataset(false);
                //  TrainOnMyDataset(false);
                TrainOnMyDataset(true);
            }
            // Hide console and show window
            ShowWindow(handle, SW_HIDE);
            clear();
            Show();
        }

        void Train()
        {
            Console.Clear();
            correctCount = 0;
            total = 0;
            string TrainingSetPath = "train-images-idx3-ubyte.gz";
            string TrainingLabelsPath = "train-labels-idx1-ubyte.gz";
            Console.WriteLine("Reading training data...");
            pictures = pictureBits.readmnist(TrainingSetPath, TrainingLabelsPath);
            Console.WriteLine("database loaded, learning in progress...");
            foreach (pictureBits pb in pictures)
            {
                foreach (Perceptron pc in network.perceptrones)
                {
                    pc.train(pb.oneDimArr, pb.label);
                }
                if (network.GiveAnswer(pb.oneDimArr) == pb.label)
                {
                    correctCount++;
                }
                total++;
                //  Console.Clear();
                progress.Content = "Total: " + total;
                double rate = ((correctCount / total) * 100.0);
                correct.Content = "Correct: " + correctCount;
                percent.Content = "Percent: " + (int)rate + " %";
                Console.Write("\r" + progress.Content + "\t||\t" + correct.Content + "\t||\t" + percent.Content);
            }
        }

        void TrainOnMyDataset(bool OnlyTest)
        {
            Console.Clear();
            correctCount = 0;
            total3 = 0;
            string dir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            var files = Directory.GetFiles(dir + @"\savedData", "*.*", SearchOption.AllDirectories);
            byte label;
            Random r = new Random();

            files = files.OrderBy(x => r.Next()).ToArray();
            foreach (var picture in files)
            {
                label = byte.Parse((System.IO.Path.GetFileName(picture)[0]).ToString());

                // Console.WriteLine(picture + "  "+ label);
                var twoD = pictureBits.ImageTo2dIntArray(picture);
                var oneD = pictureBits.image2dTo1d(twoD);
                if (!OnlyTest)
                {
                    foreach (Perceptron pc in network.perceptrones)
                    {
                        pc.train(oneD, label);
                    }
                }
                if (network.GiveAnswer(oneD) == label)
                {
                    correctCount++;
                }
                total3++;
                //  Console.Clear();
                progress.Content = "Total: " + (total3 + total2 + total);
                double rate = ((correctCount / total3) * 100.0);
                correct.Content = "Correct: " + correctCount;
                percent.Content = "Percent: " + (int)rate + " %";
                Console.Write("\r" + progress.Content + "\t||\t" + correct.Content + "\t||\t" + percent.Content);
            }
            Console.WriteLine("\nEnd of training.");
        }

        void Test()
        {
            Console.Clear();

            total2 = 0;
            correctCount = 0;
            string TestingSetPath = "t10k-images-idx3-ubyte.gz";
            string TestingLabelsPath = "t10k-labels-idx1-ubyte.gz";
            pictures = pictureBits.readmnist(TestingSetPath, TestingLabelsPath);
            Console.WriteLine("database loaded, starting...");
            foreach (pictureBits pb in pictures)
            {
                foreach (Perceptron pc in network.perceptrones)
                {
                    pc.train(pb.oneDimArr, pb.label);
                }
                if (network.GiveAnswer(pb.oneDimArr) == pb.label)
                {
                    correctCount++;
                }
                total2++;
                progress.Content = "Total: " + (total + total2);
                double rate = ((correctCount / total2) * 100.0);
                correct.Content = "Correct: " + correctCount;
                percent.Content = "Percent: " + (int)rate + " %";
                Console.Write("\r" + progress.Content + "\t||\t" + correct.Content + "\t||\t" + percent.Content);
            }
        }

        #region buttons
        void corr()
        {
            correctCount++;
            total++;
            double rate = ((correctCount / total) * 100.0);
            progress.Content = "Total: " + total;

            correct.Content = "Correct: " + correctCount;
            percent.Content = "Percent: " + (int)rate + " %";
            string dir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string fullDir = dir + @"\savedData\";
            if (Guesslbl.Content.ToString() == "-")
            {
                MessageBox.Show("Pusty obrazek.");
                return;
            }
            if (!Directory.Exists(fullDir))
            {
                Directory.CreateDirectory(fullDir);
                for (int i = 0; i < 10; i++)
                {
                    Directory.CreateDirectory(fullDir + i);
                }
            }
            string name;
            do
            {
                name = fullDir + Guesslbl.Content + @"\" + Guesslbl.Content + RandomName() + ".bmp";
            } while (File.Exists(name));
            try
            {
                File.Copy(dir + @"\readyTemp.bmp", name);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            Guesslbl.Foreground = System.Windows.Media.Brushes.ForestGreen;
            clear();
        }
        private void Button_Correct_Click(object sender, RoutedEventArgs e)
        {
            corr();
        }

        void incorrect()
        {
            total++;
            double rate = ((correctCount / total) * 100.0);
            correct.Content = "Correct: " + correctCount;
            percent.Content = "Percent: " + (int)rate + " %";
            progress.Content = "Total: " + total;

            if (Guesslbl.Content.ToString() == "-")
            {
                MessageBox.Show("Pusty obrazek.");
                return;
            }
            if (ValidChar())
            {
                string dir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                string fullDir = dir + @"\savedData\";
                if (!Directory.Exists(fullDir))
                {
                    Directory.CreateDirectory(fullDir);
                    for (int i = 0; i < 10; i++)
                    {
                        Directory.CreateDirectory(fullDir + i);
                    }
                }
                string name;
                do
                {
                    name = fullDir + tbCorrectAns.Text + @"\" + tbCorrectAns.Text + RandomName() + ".bmp";
                } while (File.Exists(name));
                try
                {
                    File.Copy(dir + @"\readyTemp.bmp", name);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
                Guesslbl.Foreground = System.Windows.Media.Brushes.ForestGreen;
                clear();
            }
            else
            {
                MessageBox.Show("Nieprawidłowy znak!");
            }
        }
        private void Button_Incorrect_Click(object sender, RoutedEventArgs e)
        {
            incorrect();
        }

        bool justOnce = false;
        void reco()
        {
            if (!justOnce)
            {
                correctCount = 0;
                total = total2 = total3 = 0;
                justOnce = true;
            }
            using (WaitCursor wk = new WaitCursor())
            {
                string dir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                string readyFileName = dir + @"\readyTemp.bmp";
                string rawFileName = dir + @"\rawTemp.bmp";
                string oldFileName = dir + @"\oldTemp.bmp";
                var map = pictureBits.ImageToByte(img.Source as BitmapImage);
                byte[] resp;
                //obrobka przez pythona
                try
                {
                    resp = PythonTcpTunnel.tcp(map);
                }
                catch
                {
                    MessageBox.Show("cannot connect with python script");
                    return;
                }
                if (resp[0] == 49)
                {
                    if (File.Exists(readyFileName))
                    {
                        var twoD = pictureBits.ImageTo2dIntArray(readyFileName);
                        var oneD = pictureBits.image2dTo1d(twoD);
                        Guesslbl.Content = network.GiveAnswer(oneD);
                    }
                    else
                    {
                        MessageBox.Show("Problem z obrabianiem pliku przez pythona.");
                    }
                }
                else
                {
                    MessageBox.Show("Problem z obrabianiem pliku przez pythona.");
                }
            }
        }
        private void Button_Recognize_Click(object sender, RoutedEventArgs e)
        {
            reco();
        }

        void clear()
        {
            Bitmap bmp = new Bitmap(200, 200);
            for (int i = 0; i < 200; i++)
            {
                for (int k = 0; k < 200; k++)
                {
                    bmp.SetPixel(i, k, System.Drawing.Color.WhiteSmoke);
                }
            }
            ImageSource imgSource = pictureBits.BitmapToImageSource(bmp);
            img.Source = imgSource;
            //tbCorrectAns.Text = "";
            Guesslbl.Content = "-";
            Guesslbl.Foreground = System.Windows.Media.Brushes.Silver;
        }

        private void Button_Clear_Click(object sender, RoutedEventArgs e)
        {
            clear();
        }
        #endregion

        #region Drawing Box
        bool mousedown = false;
        private void Img_MouseDown(object sender, MouseButtonEventArgs e)
        {
            mousedown = true;
            Utility.SetMouseSpeed(3);
        }

        private void Img_MouseMove(object sender, MouseEventArgs e)
        {
            int penThick = 6;
            if (mousedown)
            {
                var map = pictureBits.ConvertToBitmap((BitmapSource)img.Source);
                System.Windows.Point p = e.GetPosition(img);
                if (p.X > penThick && p.Y > penThick && p.X < map.Width - penThick && p.Y < map.Height - penThick)
                {
                    for (int j = 0; j < penThick; j++)
                    {
                        for (int i = 0; i < penThick; i++)
                        {
                            map.SetPixel((int)p.X - i, (int)p.Y + j, System.Drawing.Color.Black);
                        }
                        for (int i = 0; i < penThick; i++)
                        {
                            map.SetPixel((int)p.X + j, (int)p.Y + i, System.Drawing.Color.Black);
                        }
                        for (int i = 0; i < penThick; i++)
                        {
                            map.SetPixel((int)p.X + j, (int)p.Y - i, System.Drawing.Color.Black);
                        }
                        for (int i = 0; i < penThick; i++)
                        {
                            map.SetPixel((int)p.X - i, (int)p.Y - j, System.Drawing.Color.Black);
                        }
                    }
                    img.Source = pictureBits.BitmapToImageSource(map);
                }
            }
        }

        private void Img_MouseUp(object sender, MouseButtonEventArgs e)
        {
            mousedown = false;
            Utility.SetMouseSpeed(6);
        }
        #endregion

        #region Textbox and slider Correct Answer 
        bool ValidChar()
        {
            if (tbCorrectAns.Text.Length != 1) return false;
            char[] charset = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            foreach (char c in charset)
            {
                if (c == tbCorrectAns.Text[0])
                {
                    return true;
                }
            }
            return false;
        }
        bool flag = false;
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (!flag)
            {
                flag = true;
                tbCorrectAns.Text = "";
            }
        }

        int once = 0;
        private void TbCorrectAns_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (once > 2)
            {
                if (tbCorrectAns.Text.Length > 1) tbCorrectAns.Text = tbCorrectAns.Text.Remove(tbCorrectAns.Text.Length - 1);
                if (!ValidChar()) tbCorrectAns.Text = "";
                if (tbCorrectAns.Text.Length > 0) slider.Value = double.Parse(tbCorrectAns.Text);
            }
            once++;
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            tbCorrectAns.Text = ((int)((Slider)sender).Value).ToString();
        }

        private void Slider_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta < 0) slider.Value--;
            else slider.Value++;
        }
        #endregion

        #region Change mouse speed
        public static class Utility
        {
            public const UInt32 SPI_SETMOUSESPEED = 0x0071;

            [DllImport("User32.dll")]
            static extern Boolean SystemParametersInfo(
                UInt32 uiAction,
                UInt32 uiParam,
                UInt32 pvParam,
                UInt32 fWinIni);

            public static void SetMouseSpeed(uint speed)
            {
                SystemParametersInfo(SPI_SETMOUSESPEED, 0, speed, 0);
            }
        }
        #endregion

        #region window
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            int b = 1;
            for (int i = 0; i < b; i++)
            {
                try
                {
                    foreach (var process in Process.GetProcessesByName("mnist"))
                    {
                        process.Kill();
                    }
                }
                catch
                {
                    System.Threading.Thread.Sleep(100);
                    if (b > 4) return;
                    b++;
                }
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            network.SaveWeights();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.W)
            {
                reco();
            }
            else if (e.Key == Key.S)
            {
                clear();
            }
            else if (e.Key == Key.A)
            {
                incorrect();
            }
            else if (e.Key == Key.D)
            {
                corr();
            }
            else if (e.Key == Key.Q)
            {
                slider.Value--;
            }
            else if (e.Key == Key.E)
            {
                slider.Value++;
            }
        }
        #endregion

        public static string RandomName()
        {
            var name = new List<char>();
            for (int i = 0; i < 4; i++)
            {
                name.Add((char)random.Next(97, 123));
            }
            for (int i = 0; i < 4; i++)
            {
                name.Add((char)random.Next(48, 58));
            }
            return new string(name.ToArray());
        }
    }
}

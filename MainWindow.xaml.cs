using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
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

namespace WpfApp2
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();

            const int MaxX = 50;

            var r = new Random();
            ChartSample.Items = new [] {
                new LineSeries()
                {
                    HeaderY = "TESTあいうえお",
                    MaxY = 12,
                    MinY = 2,
                    LineBrush = Brushes.YellowGreen,
                    LineThickness = 1,
                    ScaleSplitCountY = 5,
                    Lines = Enumerable.Range(0, MaxX+1).Select(i => new Line(){ X = i, Y = r.Next(2, 13) })
                },
                new LineSeries()
                {
                    HeaderY = "TESTあいうえお",
                    MaxY = 12,
                    MinY = 2,
                    LineBrush = Brushes.CornflowerBlue,
                    LineThickness = 1,
                    ScaleSplitCountY = 5,
                    Lines = Enumerable.Range(0, MaxX+1).Select(i => new Line(){ X = i, Y = r.Next(2, 13) })
                },
                new LineSeries()
                {
                    HeaderY = "TESTあいうえお",
                    MaxY = 12,
                    MinY = 2,
                    LineBrush = Brushes.DarkOrange,
                    LineThickness = 1,
                    ScaleSplitCountY = 5,
                    Lines = Enumerable.Range(0, MaxX+1).Select(i => new Line(){ X = i, Y = r.Next(2, 13) })
                },
                new LineSeries()
                {
                    HeaderY = "TESTあいうえお",
                    MaxY = 12,
                    MinY = 2,
                    LineBrush = Brushes.DodgerBlue,
                    LineThickness = 1,
                    ScaleSplitCountY = 5,
                    Lines = Enumerable.Range(0, MaxX+1).Select(i => new Line(){ X = i, Y = r.Next(2, 13) })
                },
                new LineSeries()
                {
                    HeaderY = "TESTあいうえお",
                    MaxY = 12,
                    MinY = 2,
                    LineBrush = Brushes.Goldenrod,
                    LineThickness = 1,
                    ScaleSplitCountY = 5,
                    Lines = Enumerable.Range(0, MaxX+1).Select(i => new Line(){ X = i, Y = r.Next(2, 13) })
                },
                new LineSeries()
                {
                    HeaderY = "TESTあいうえお",
                    MaxY = 12,
                    MinY = 2,
                    LineBrush = Brushes.LawnGreen,
                    LineThickness = 1,
                    ScaleSplitCountY = 5,
                    Lines = Enumerable.Range(0, MaxX+1).Select(i => new Line(){ X = i, Y = r.Next(2, 13) })
                },
                new LineSeries()
                {
                    HeaderY = "TESTあいうえお",
                    MaxY = 12,
                    MinY = 2,
                    LineBrush = Brushes.LightPink,
                    LineThickness = 1,
                    ScaleSplitCountY = 5,
                    Lines = Enumerable.Range(0, MaxX+1).Select(i => new Line(){ X = i, Y = r.Next(2, 13) })
                },
                new LineSeries()
                {
                    HeaderY = "TESTあいうえお",
                    MaxY = 12,
                    MinY = 2,
                    LineBrush = Brushes.MediumBlue,
                    LineThickness = 1,
                    ScaleSplitCountY = 5,
                    Lines = Enumerable.Range(0, MaxX+1).Select(i => new Line(){ X = i, Y = r.Next(2, 13) })
                },
                new LineSeries()
                {
                    HeaderY = "TESTあいうえお",
                    MaxY = 12,
                    MinY = 2,
                    LineBrush = Brushes.MintCream,
                    LineThickness = 1,
                    ScaleSplitCountY = 5,
                    Lines = Enumerable.Range(0, MaxX+1).Select(i => new Line(){ X = i, Y = r.Next(2, 13) })
                },
                new LineSeries()
                {
                    HeaderY = "TESTあいうえお",
                    MaxY = 12,
                    MinY = 2,
                    LineBrush = Brushes.PaleGoldenrod,
                    LineThickness = 1,
                    ScaleSplitCountY = 5,
                    Lines = Enumerable.Range(0, MaxX+1).Select(i => new Line(){ X = i, Y = r.Next(2, 13) })
                },
                new LineSeries()
                {
                    HeaderY = "TESTかきくけこ",
                    MaxY = 50,
                    MinY = 0,
                    LineBrush = Brushes.Red,
                    LineThickness = 1,
                    ScaleSplitCountY = 5,
                    Lines = Enumerable.Range(0, MaxX+1).Select(i => new Line(){ X = i, Y = 12 })//r.Next(0, 51) })
                },
                new LineSeries()
                {
                    HeaderY = "TESTさしすせそ",
                    MaxY = 500,
                    MinY = -500,
                    LineBrush = Brushes.Tan,
                    LineThickness = 1,
                    ScaleSplitCountY = 5,
                    Lines = Enumerable.Range(0, MaxX+1).Select(i => new Line(){ X = i, Y = 150 })
                }
            };
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var sp = new Stopwatch();
            sp.Start();
            ChartSample.Redraw();
            GC.Collect();
            long mem = GC.GetTotalMemory(true);
            Console.WriteLine("処理時間={0:#,0}ﾐﾘ秒,ﾒﾓﾘ消費量={1:#,0}byte", sp.Elapsed.TotalMilliseconds.ToString(), mem);
        }

    }
}

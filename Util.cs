using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace WpfApp2
{
    class Util
    {
        /// <summary>
        /// 目盛りの凸部分の長さ
        /// </summary>
        internal const int ScaleLineLength = 5;

        /// <summary>
        /// 目盛りに表示する値を取得する
        /// </summary>
        /// <param name="min">最小値</param>
        /// <param name="max">最大値</param>
        /// <param name="splitCount">分割数</param>
        /// <returns></returns>
        internal static IEnumerable<string> GetScaleStrings(double min, double max, int splitCount)
        {
            return Enumerable.Range(0, splitCount + 1).Select(
                i => (min + ((max - min) / splitCount * i)).ToString()
            );
        }

        /// <summary>
        /// 表題文字列を描写用書式ｵﾌﾞｼﾞｪｸﾄで取得する
        /// </summary>
        /// <param name="text">表題文字列</param>
        /// <param name="color">表題の色</param>
        /// <returns>描写用書式ｵﾌﾞｼﾞｪｸﾄ</returns>
        internal static FormattedText GetFormattedText(string text, Brush brush)
        {
            return new FormattedText(text,
                new CultureInfo("ja-jp"),
                FlowDirection.LeftToRight,
                new Typeface(new FontFamily("Meiryo UI"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal),
                12,
                brush
            );
        }

        /// <summary>
        /// 表題文字列を描写用書式ｵﾌﾞｼﾞｪｸﾄで取得する
        /// </summary>
        /// <param name="text">表題文字列</param>
        /// <returns>描写用書式ｵﾌﾞｼﾞｪｸﾄ</returns>
        internal static FormattedText GetFormattedText(string text)
        {
            return GetFormattedText(text, Brushes.Black);
        }

        /// <summary>
        /// 線を作成する
        /// </summary>
        /// <param name="beginX">X軸の目盛り開始位置</param>
        /// <param name="endX">X軸の目盛り終了位置</param>
        /// <param name="beginY">Y軸の目盛り開始位置</param>
        /// <param name="endY">Y軸の目盛り終了位置</param>
        /// <returns>線を表す<code>PathFigure</code></returns>
        internal static PathFigure CreateLine(double beginX, double beginY, double endX, double endY)
        {
            return new PathFigure(
                new Point(beginX, beginY),
                new[] { new LineSegment(new Point(endX, endY), true) },
                false
            );
        }

        /// <summary>
        /// <code>DrawingVisual</code>に設定されている<code>Drawing</code>ｲﾝｽﾀﾝｽ全てのｱﾝﾁｴｲﾘｱｽを解除する
        /// </summary>
        /// <param name="dv"><code>DwawingVisual</code></param>
        /// <returns>ｱﾝﾁｴｲﾘｱｽを解除した<code>DrawingVisual</code></returns>
        internal static DrawingVisual SetRenderOptions(DrawingVisual dv)
        {
            var drawingGroup = dv.Drawing;

            foreach (var d in GetDrawings(drawingGroup))
            {
                if (!d.IsSealed)
                {
                    RenderOptions.SetBitmapScalingMode(d, BitmapScalingMode.Fant);
                    RenderOptions.SetEdgeMode(d, EdgeMode.Aliased);
                }
            }

            dv = new DrawingVisual();
            using (DrawingContext ctx = dv.RenderOpen())
            {
                ctx.DrawDrawing(drawingGroup);
            }

            return dv;
        }

        /// <summary>
        /// <code>DrawingGroup</code>に格納された全ての子ｲﾝｽﾀﾝｽを階層構造を考慮して取得します
        /// </summary>
        /// <param name="g"><code>DrawingGroup</code></param>
        /// <returns>階層を考慮して取得した<code>Drawing配列</code></returns>
        private static IEnumerable<Drawing> GetDrawings(DrawingGroup g)
        {
            if (g == null)
            {
                yield break;
            }

            foreach (var d in g.Children)
            {
                yield return d;

                foreach (var c in GetDrawings(d as DrawingGroup))
                {
                    yield return c;
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace WpfApp2
{
    public abstract class Series
    {
        /// <summary>
        /// 目盛りの描写位置の余白
        /// </summary>
        private const int ScalePadding = 5;

        /// <summary>
        /// 線の色
        /// </summary>
        public Brush LineBrush { get; set; }

        /// <summary>
        /// 線の太さ
        /// </summary>
        public int LineThickness { get; set; }

        /// <summary>
        /// 縦軸のﾍｯﾀﾞ
        /// </summary>
        public string HeaderY { get; set; }

        /// <summary>
        /// Y 軸の最小値
        /// </summary>
        public int MinY { get; set; }

        /// <summary>
        /// Y 軸の最大値
        /// </summary>
        public int MaxY { get; set; }

        /// <summary>
        /// Y軸の目盛りの分割数
        /// </summary>
        public int ScaleSplitCountY { get; set; }

        /// <summary>
        /// ｼﾘｰｽﾞを描写します。
        /// </summary>
        /// <param name="chart">描写するｼﾘｰｽﾞ</param>
        /// <returns>描写結果を格納する図形ｵﾌﾞｼﾞｪｸﾄ</returns>
        public abstract Geometry CreateGeometry(Chart chart);

        /// <summary>
        /// Y軸のﾍｯﾀﾞを描写する
        /// </summary>
        /// <param name="dc">ｺﾝﾃｷｽﾄ</param>
        /// <param name="chart">ﾁｬｰﾄ</param>
        /// <param name="beginX">X座標の開始位置</param>
        internal void DrawYHeader(DrawingContext dc, Chart chart, double beginX)
        {
            if (!string.IsNullOrWhiteSpace(HeaderY))
            {
                // Y軸目盛り値表示
                var nicks = Util.GetScaleStrings(MinY, MaxY, ScaleSplitCountY);
                for (int i = 0; i <= ScaleSplitCountY; i++)
                {
                    // 目盛り文字と目盛りを表示する高さ、目盛り文字の幅を取得
                    var headerText = Util.GetFormattedText(nicks.ElementAt(ScaleSplitCountY - i), LineBrush);
                    var headerY = (chart.GraphHeight / ScaleSplitCountY * i);
                    var headerWidth = headerText.Width;

                    // 目盛りを描写
                    dc.DrawText(headerText, new Point(beginX - headerWidth - ScalePadding - Util.ScaleLineLength, headerY - (headerText.Height / 2)));

                    // 目盛り値に紐付く凸型の線
                    dc.DrawLine(new Pen(LineBrush, 1),
                        new Point(beginX - ScalePadding, headerY),
                        new Point(beginX - ScalePadding + Util.ScaleLineLength, headerY)
                    );
                }

                // 目盛り値に紐付く凸型の線
                dc.DrawLine(new Pen(LineBrush, 1),
                    new Point(beginX, 0),
                    new Point(beginX, chart.GraphHeight)
                );

                // Y軸標題
                var text = Util.GetFormattedText(HeaderY, LineBrush);
                var textHeight = text.Height;
                var point = new Point(beginX, chart.GraphHeight / 2 + text.Width / 2);

                // Y軸標題の中心で反時計回りに90度回転させる
                dc.PushTransform(new RotateTransform(-90, point.X, point.Y));

                // 表題位置補正
                point.Y -= GetHeaderYWidth();
                dc.DrawText(text, point);
                dc.Pop();
            }
        }

        /// <summary>
        /// Y軸のﾍｯﾀﾞの幅を取得する
        /// </summary>
        /// <returns>Y軸のﾍｯﾀﾞの幅</returns>
        internal protected double GetHeaderYWidth()
        {
            // 目盛り文字の幅と高さの合計の最大値を取得
            var wh = Util.GetScaleStrings(MinY, MaxY, ScaleSplitCountY)
                .Select(n =>
                {
                    // 目盛り文字の幅と高さの合計を返却
                    var t = Util.GetFormattedText(n);
                    return t.Width + t.Height;
                })
                .Max();
            // 目盛り文字の幅と高さの合計の最大値＋余白をﾍｯﾀﾞの幅として返却
            return wh + ScalePadding + Util.ScaleLineLength;
        }

        /// <summary>
        /// ｼﾘｰｽﾞのｸﾞﾗﾌを描写する
        /// </summary>
        /// <param name="dc">ｺﾝﾃｷｽﾄ</param>
        /// <param name="chart">描写する親ﾁｬｰﾄのｲﾝｽﾀﾝｽ</param>
        public void Redraw(DrawingContext dc, Chart chart)
        {
            // 引数＝塗りつぶし色、枠線、ｸﾞﾗﾌｲﾝｽﾀﾝｽ
            dc.DrawGeometry(null, new Pen(LineBrush, LineThickness), CreateGeometry(chart));
        }
    }
}

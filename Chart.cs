using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfApp2
{
    public class Chart : Image
    {
        public Chart() : base()
        {
            // ｷｬﾝﾊﾞｽｻｲｽﾞ変更ｲﾍﾞﾝﾄを感知
            LayoutUpdated += This_LayoutUpdated;

            // ｸﾞﾗﾌ内の余白(目盛り上限値が切れないような初期値)
            Padding = new Thickness(0, 8, 8, 0);
            
            // ｸﾞﾗﾌ枠線
            BorderBrush = Brushes.Gray;
        }

        // **************************************************
        // * 内部変数
        // **************************************************

        /// <summary>
        /// ｷｬﾝﾊﾞｽ
        /// </summary>
        private RenderTargetBitmap Canvas { get; set; }

        /// <summary>
        /// ｸﾞﾗﾌの余白
        /// </summary>
        internal Thickness Padding;

        /// <summary>
        /// ｸﾞﾗﾌ表示場所の移動量(X 座標)
        /// </summary>
        private int TranslateX { get; set; }

        /// <summary>
        /// ｸﾞﾗﾌ表示場所の移動量(Y 座標)
        /// </summary>
        private int TranslateY { get; set; }

        /// <summary>
        /// ｸﾞﾗﾌのX軸拡大縮小の比率
        /// </summary>
        internal double ZoomRatioX { get; set; }

        /// <summary>
        /// ｷｬﾝﾊﾞｽの幅
        /// </summary>
        internal double CanvasWidth { get; set; }

        /// <summary>
        /// ｷｬﾝﾊﾞｽの高さ
        /// </summary>
        internal double CanvasHeight { get; set; }

        /// <summary>
        /// ｸﾞﾗﾌ描写領域の幅
        /// </summary>
        internal double GraphWidth { get; set; }

        /// <summary>
        /// ｸﾞﾗﾌ描写領域の高さ
        /// </summary>
        internal double GraphHeight { get; set; }

        // **************************************************
        // * 公開変数
        // **************************************************

        /// <summary>
        /// ﾁｬｰﾄに描写するﾃﾞｰﾀ
        /// </summary>
        public IEnumerable<Series> Items { get; set; }

        /// <summary>
        /// 横軸のﾍｯﾀﾞ
        /// </summary>
        public string HeaderX { get; set; }

        /// <summary>
        /// X軸の最小値
        /// </summary>
        public double MinX { get; set; }

        /// <summary>
        /// X軸の最大値
        /// </summary>
        public double MaxX { get; set; }

        /// <summary>
        /// X軸の目盛りの分割数
        /// </summary>
        public int ScaleSplitCountX { get; set; }

        /// <summary>
        /// ｸﾞﾗﾌ枠線の色ｽﾀｲﾙ
        /// </summary>
        public Brush BorderBrush { get; set; }

        /// <summary>
        /// ｸﾞﾗﾌを再描写する。
        /// </summary>
        public void Redraw()
        {
            Canvas?.Clear();

            var dv = new DrawingVisual();
            
            using (var dc = dv.RenderOpen())
            {
                // ｸﾞﾗﾌ描写位置の移動量を演算
                CalcInnerVariables();

                // ｸﾞﾗﾌ描画位置の移動量をセット
                var transTrans = new TranslateTransform(TranslateX, TranslateY);
                dc.PushTransform(transTrans);
                
                // Y 軸の中心で反転をセット(左上原点補正)
                var scaleTrans = new ScaleTransform();
                scaleTrans.CenterY = GraphHeight / 2;
                scaleTrans.ScaleY = -1;
                dc.PushTransform(scaleTrans);

                // ﾌﾚｰﾑからはみ出た描写を切り捨てる設定を追加
                dc.PushClip(new RectangleGeometry(new Rect(0, 0, GraphWidth, GraphHeight+1)));

                // ﾌﾚｰﾑを描画
                dc.DrawGeometry(null, new Pen(BorderBrush, 1), DrawFrame());

                // ｸﾞﾗﾌを描写
                var tmp = Items.Select(i =>
                {
                    i.Redraw(dc, this);
                    return i;
                }).ToArray();

                // 反転設定とはみ出し除去設定を解除
                dc.Pop();
                dc.Pop();

                // X軸の表題
                DrawXHeader(dc);

                // Y軸の表題
                var y = 0.0D;
                for (int i = 0; i < Items.Count(); i++)
                {
                    var item = Items.ElementAt(i);
                    item.DrawYHeader(dc, this, y);
                    y -= item.GetHeaderYWidth();
                }
                dc.Pop();
            }

            // ｷｬﾝﾊﾞｽへ描画し、ｿｰｽへ反映
            //Canvas.Render(dv);
            Canvas.Render(Util.SetRenderOptions(dv));
            Source = Canvas;
        }

        /// <summary>
        /// ｸﾞﾗﾌ描写位置の移動量を演算する
        /// </summary>
        private void CalcInnerVariables()
        {
            Padding.Right = Util.GetFormattedText(MaxX.ToString()).Width / 2 + 2;

            // X軸のﾍｯﾀﾞの高さとY軸のﾍｯﾀﾞの幅を求める
            var t = Util.GetFormattedText(HeaderX);
            var HeaderXHeight = (int)(t.Height * 2);
            var HeaderYWidth = (int)Items.Select(i => i.GetHeaderYWidth()).Sum();

            // 移動量を求める
            TranslateX = (int)(HeaderYWidth + Padding.Left);    // X軸への移動量は「Y軸のﾍｯﾀﾞの幅＋余白」
            TranslateY = (int)(Padding.Top);                    // Y軸への移動量は「余白」

            // ｷｬﾝﾊﾞｽの大きさ
            var dpi = (Double)DpiGetter.GetDpi(Orientation.Horizontal);
            Canvas = new RenderTargetBitmap(Convert.ToInt32(CanvasWidth), Convert.ToInt32(CanvasHeight), dpi, dpi, PixelFormats.Default);

            // ｸﾞﾗﾌ領域の大きさ
            GraphHeight = CanvasHeight - HeaderXHeight - TranslateY - Padding.Bottom;
            GraphWidth = CanvasWidth - TranslateX - Padding.Right;

            // ｸﾞﾗﾌ描写時の倍率
            ZoomRatioX = GraphWidth / (MaxX - MinX);
        }

        /// <summary>
        /// ﾌﾚｰﾑと目盛りを描写する
        /// </summary>
        /// <returns></returns>
        private Geometry DrawFrame()
        {
            // 描写する図のﾘｽﾄ
            var pathFigures = new List<PathFigure>();

            var minX = 0;
            var maxX = GraphWidth;
            var minY = 0;
            var maxY = GraphHeight;

            // ﾌﾚｰﾑの4隅を描写
            pathFigures.Add(new PathFigure(
                new Point(minX, minY),
                new[]
                {
                    new LineSegment(new Point(maxX, minY), true),
                    new LineSegment(new Point(maxX, maxY), true),
                    new LineSegment(new Point(minX, maxY), true)
                },
                true
            ));

            // Y軸の目盛り線を表示 (区切り位置は最初のｽｹｰﾙを使用する)
            var item = Items.FirstOrDefault();
            if (item != null)
            {
                pathFigures.AddRange(
                    Enumerable.Range(0, item.ScaleSplitCountY).Select(i =>
                    {
                        var memoriY = (maxY - minY) / (item.ScaleSplitCountY);
                        return Util.CreateLine(minX, memoriY * i, maxX, memoriY * i);
                    })
                );
            }

            // X軸の目盛り線を表示
            pathFigures.AddRange(
                Enumerable.Range(0, ScaleSplitCountX).Select(i =>
                {
                    var memoriX = (maxX - minX) / (ScaleSplitCountX);
                    return Util.CreateLine(memoriX * i, minY, memoriX * i, maxY);
                })
            );

            // 結果を返却
            return new PathGeometry(pathFigures);
        }

        /// <summary>
        /// X軸のﾍｯﾀﾞを描写する
        /// </summary>
        /// <param name="dc">ｺﾝﾃｷｽﾄ</param>
        private void DrawXHeader(DrawingContext dc)
        {
            // X軸目盛値表示
            if (!string.IsNullOrWhiteSpace(HeaderX))
            {
                var nicks = Util.GetScaleStrings(MinX, MaxX, ScaleSplitCountX);
                for (int i = 0; i <= ScaleSplitCountX; i++)
                {
                    var headerText = Util.GetFormattedText(nicks.ElementAt(i));
                    var headerX = (GraphWidth / ScaleSplitCountX * i);
                    dc.DrawText(headerText, new Point(headerX - headerText.Width / 2, GraphHeight + Util.ScaleLineLength));

                    // 目盛り値に紐付く凸型の線
                    dc.DrawLine(new Pen(BorderBrush, 1), 
                        new Point(headerX, GraphHeight),
                        new Point(headerX, GraphHeight + Util.ScaleLineLength)
                    );
                }

                // X軸標題
                var text = Util.GetFormattedText(HeaderX);
                dc.DrawText(text, new Point(GraphWidth / 2 - text.Width / 2, GraphHeight + text.Height));

                // X軸の区切り線を表示
                dc.DrawGeometry(null, new Pen(BorderBrush, 1), new PathGeometry(new[] { Util.CreateLine(0, GraphHeight, GraphWidth, GraphHeight) }));
            }
        }

        private void This_LayoutUpdated(object sender, EventArgs e)
        {
            var p = this.Parent as FrameworkElement;
            
            if (p != null && 0 < p.ActualWidth && 0 < p.ActualHeight && (CanvasWidth != p.ActualWidth || CanvasHeight != p.ActualHeight))
            {
                CanvasHeight = p.ActualHeight;
                CanvasWidth = p.ActualWidth;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace WpfApp2
{
    public class LineSeries : Series
    {
        /// <summary>
        /// 線を繋ぐ頂点
        /// </summary>
        public IEnumerable<Line> Lines { get; set; }

        /// <summary>
        /// 折れ線ｸﾞﾗﾌを描写します
        /// </summary>
        /// <param name="chart">折れ線ｸﾞﾗﾌを描写するﾁｬｰﾄｵﾌﾞｼﾞｪｸﾄ</param>
        /// <returns><code>Geometry</code></returns>
        public override Geometry CreateGeometry(Chart chart)
        {
            // 始点を取得する
            var line = Lines.FirstOrDefault();

            if (line != null)
            {
                // Y座標の倍率
                var ZoomRatioY = chart.GraphHeight / (MaxY - MinY);

                // 第一引数：StartPoint
                // 第二引数：Segments
                // 第三引数：始点と終点を接続するか
                var figure = new PathFigure(
                    ToRenderingPoint(chart, line, ZoomRatioY),
                    Lines.Skip(1).Select(l => new LineSegment()
                    {
                        IsSmoothJoin = true,
                        IsStroked = true,
                        Point = ToRenderingPoint(chart, l, ZoomRatioY)
                    }),
                    false
                );
                return new PathGeometry(new[] { figure });
            }
            else
            {
                return new PathGeometry();
            }
        }

        /// <summary>
        /// 指定したﾗｲﾝのﾃﾞｰﾀを実際にﾚﾝﾀﾞﾘﾝｸﾞする位置に変換します
        /// </summary>
        /// <param name="chart">親ﾁｬｰﾄのｲﾝｽﾀﾝｽ</param>
        /// <param name="line">ﾗｲﾝﾃﾞｰﾀ</param>
        /// <param name="zoomRatioY">Y座標の倍率</param>
        /// <returns>ﾚﾝﾀﾞﾘﾝｸﾞする位置を示す<code>Point</code></returns>
        private Point ToRenderingPoint(Chart chart, Line line, double zoomRatioY)
        {
            return new Point(
                (line.X - chart.MinX) * chart.ZoomRatioX, 
                (line.Y - MinY) * zoomRatioY
            );
        }
    }
}

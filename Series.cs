using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WpfApp2
{
    public class Series
    {
        public double MinX { get; set; }

        public double MaxX { get; set; }

        public double MinY { get; set; }

        public double MaxY { get; set; }

        public Color LineColor { get; set; }

        public IEnumerable<Line> Lines { get; set; }
    }
}

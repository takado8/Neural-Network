using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
namespace Snake2
{
    class snake
    {
        public List<segment> segments = new List<segment>();
        public segment.dir nextDir = segment.dir.down;
        public snake()
        {
            //make head
            segment sg = new segment();
            segment sg2 = new segment(50.0, 40.0, false);
            segments.Add(sg);
            segments.Add(sg2);
        }
        public segment this[int i]
        {
            get
            {
                return segments[i];
            }
            set
            {
                segments[i] = value;
            }
        }
        public class segment
        {
            public Rectangle rec = new Rectangle();
            public int size = 10;
            public enum dir
            {
                up,
                down,
                right,
                left
            }
            public dir Direction;

            public segment(double start_left = 50.0, double start_point_top = 50.0, bool black = true)
            {
                rec.Height = size;
                rec.Width = size;
                if (black) rec.Fill = Brushes.Black;
                else rec.Fill = (Brush)(new BrushConverter().ConvertFrom("#222222"));
                rec.SetValue(Canvas.TopProperty, start_point_top);
                rec.SetValue(Canvas.LeftProperty, start_left);
                Direction = dir.down;
            }
            public segment(double top, double left, dir _direction)
            {
                rec.Height = size;
                rec.Width = size;
                rec.Fill = (Brush)(new BrushConverter().ConvertFrom("#222222"));
                rec.SetValue(Canvas.TopProperty, top);
                rec.SetValue(Canvas.LeftProperty, left);
                Direction = _direction;
            }
        }
    }
}

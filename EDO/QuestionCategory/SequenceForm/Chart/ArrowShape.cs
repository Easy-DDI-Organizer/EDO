using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;

namespace EDO.QuestionCategory.SequenceForm.Chart
{
    public class ArrowShape :ChartShape
    {
        public ArrowShape()
        {
            polyline = new Polyline();
            polyline.Stroke = Brushes.SlateGray;
            polyline.StrokeThickness = 3;

            arrow = new Polyline();
            arrow.Stroke = Brushes.SlateGray;
            arrow.StrokeThickness = 3;
            arrow.StrokeEndLineCap = PenLineCap.Round;
        }

        private Polyline polyline;
        private Polyline arrow;

        public double X1 { get; set; }
        public double Y1 { get; set; }

        public double X2 { get; set; }
        public double Y2 { get; set; }


        public override void AppendTo(Canvas canvas)
        {
            polyline.Points.Add(new Point(X1, Y1));
            polyline.Points.Add(new Point(X2, Y2));

            double arrowWidth = 10;
            double arrowHeight = 10;
            arrow.Points.Add(new Point(X2 - arrowWidth / 2.0, Y2 - arrowHeight));
            arrow.Points.Add(new Point(X2, Y2));
            arrow.Points.Add(new Point(X2 + arrowWidth / 2.0, Y2 - arrowHeight));

            canvas.Children.Add(polyline);
            canvas.Children.Add(arrow);
        }

    }
}

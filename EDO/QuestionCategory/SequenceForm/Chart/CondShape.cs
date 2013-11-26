using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows;
using EDO.Core.View;

namespace EDO.QuestionCategory.SequenceForm.Chart
{
    public class CondShape :ChartShape
    {
        public CondShape()
        {
            points = new PointCollection();

            polyline1 = new Polyline();
            polyline1.Stroke = Brushes.SlateGray;
            polyline1.StrokeThickness = 3;
            polyline1.StrokeEndLineCap = PenLineCap.Square;

            polyline2 = new RoundedCornersPolygon();
            polyline1.StrokeStartLineCap = PenLineCap.Square;
            polyline2.Stroke = polyline1.Stroke;
            polyline2.StrokeThickness = polyline1.StrokeThickness;
            polyline2.ArcRoundness = 15;
            polyline2.IsClosed = false;

            arrow = new Polyline();
            arrow.Stroke = Brushes.SlateGray;
            arrow.StrokeThickness = 3;
            arrow.StrokeEndLineCap = PenLineCap.Round;

            textBlock = new TextBlock();
            NormalOrder = true;

            
        }

        private PointCollection points;
        private Polyline polyline1;
        private RoundedCornersPolygon polyline2;
        private Polyline arrow;
        private TextBlock textBlock;

        public bool NormalOrder { get; set; }

        public PointCollection Points { get { return points; } }

        public string Text { get; set; }

        public override void AppendTo(Canvas canvas)
        {
            if (Points.Count < 5)
            {
                return;
            }
            polyline1.Points.Add(Points[0]);
            polyline1.Points.Add(Points[1]);
            canvas.Children.Add(polyline1);

            polyline2.Points.Add(Points[1]);
            polyline2.Points.Add(Points[2]);
            polyline2.Points.Add(Points[3]);
            polyline2.Points.Add(Points[4]);
            canvas.Children.Add(polyline2);

            //すべての点がセットされているときだけ最後の矢印を描画する
            Point lastPoint = Points.Last();
            double arrowWidth = 10;
            double arrowHeight = 10;
            arrow.Points.Add(new Point(lastPoint.X + arrowWidth, lastPoint.Y - arrowHeight / 2.0));
            arrow.Points.Add(new Point(lastPoint.X, lastPoint.Y));
            arrow.Points.Add(new Point(lastPoint.X + arrowWidth, lastPoint.Y + arrowHeight / 2.0));
            canvas.Children.Add(arrow);

            //3点以上揃っている時描画(最初のL字は書ける)
            Point point1 = Points[0];
            Point point2 = Points[1];
            Point point3 = Points[2];
            textBlock.Margin = new Thickness(4, 4, 0, 0);
            textBlock.TextAlignment = TextAlignment.Left;
            textBlock.Width = point3.X - point2.X;
            textBlock.Height = point2.Y - point1.Y;
            textBlock.Text = Text;
            Canvas.SetLeft(textBlock, point1.X);
            Canvas.SetTop(textBlock, point1.Y - 2);
            canvas.Children.Add(textBlock);
        }
    }
}

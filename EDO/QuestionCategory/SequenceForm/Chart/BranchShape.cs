using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using System.Diagnostics;

namespace EDO.QuestionCategory.SequenceForm.Chart
{
    public class BranchShape :ChartShape
    {
        public BranchShape(IfThenElseVM ifThenElse, Action<IfThenElseVM> clickAction)
        {
            this.ifThenElse = ifThenElse;
            this.clickAction = clickAction;

            polygon = new Polygon();
            polygon.StrokeThickness = 2;
            polygon.Stroke = Brushes.IndianRed;
            polygon.Fill = Brushes.Pink;

            polygon.MouseLeftButtonDown += polygon_MouseLeftButtonDown;


            CondShapes = new List<CondShape>();
        }
        private IfThenElseVM ifThenElse;
        private Action<IfThenElseVM> clickAction;
        private Polygon polygon;

        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }

        public double CenterX { get { return X + Width / 2.0; } }
        public double CenterY { get { return Y + Height / 2.0; } }

        public string ToolTip { get; set; }
        public List<CondShape> CondShapes { get; set; }

        public override void AppendTo(Canvas canvas)
        {
            double centerX = CenterX;
            double centerY = CenterY;
            polygon.Points.Add(new Point(centerX, Y));
            polygon.Points.Add(new Point(X + Width, centerY));
            polygon.Points.Add(new Point(centerX, Y + Height));
            polygon.Points.Add(new Point(X, centerY));
            polygon.ToolTip = ToolTip;

            canvas.Children.Add(polygon);

            foreach (CondShape condShape in CondShapes)
            {
                condShape.AppendTo(canvas);
            }
        }

          void polygon_MouseLeftButtonDown(object sender, MouseEventArgs e)
          {
              clickAction(ifThenElse);
          }
    }
}

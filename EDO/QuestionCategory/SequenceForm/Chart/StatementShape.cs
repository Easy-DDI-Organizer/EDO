using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;

namespace EDO.QuestionCategory.SequenceForm.Chart
{
    public class StatementShape :ChartShape
    {
        public StatementShape()
        {
            border = new Border();
            border.BorderThickness = new Thickness(2);
            border.Background = new SolidColorBrush(Colors.LightGray);
            border.BorderBrush = new SolidColorBrush(Colors.DarkGray);
            textBlock = new TextBlock();
            textBlock.HorizontalAlignment = HorizontalAlignment.Center;
            textBlock.VerticalAlignment = VerticalAlignment.Center;
            border.Child = textBlock;

        }

        private Border border;
        private TextBlock textBlock;

        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }

        public string Text { get; set; }
        public string ToolTip { get; set; }

        public override void AppendTo(Canvas canvas)
        {
            border.Width = Width;
            border.Height = Height;
            border.ToolTip = ToolTip;
            textBlock.Text = Text;
            Canvas.SetLeft(border, X);
            Canvas.SetTop(border, Y);
            canvas.Children.Add(border);            
        }
    }
}

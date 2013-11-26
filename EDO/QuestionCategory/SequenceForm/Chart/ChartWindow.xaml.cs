using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using EDO.Core.Model;
using System.Diagnostics;

namespace EDO.QuestionCategory.SequenceForm.Chart
{
    /// <summary>
    /// ChartWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class ChartWindow : Window
    {
        private const double NODE_WIDTH = 100;
        private const double NODE_HEIGHT = 40;
        private const double MARGIN_X = 10;
        private const double MARGIN_Y = 10;
        private const double NODE_GAP = 40;
        private const double LINE_GAP_WIDTH = 50;
        private const double LINE_GAP_HEIGHT = 20;

        private ChartWindowVM viewModel;

        public ChartWindow(ChartWindowVM viewModel)
        {
            InitializeComponent();
            this.viewModel = viewModel;
            DataContext = viewModel;
            CreateShapes();
        }

        private void CreateShapes()
        {
            canvas.Children.Clear();
            List<NodeInfo> nodeInfos = new List<NodeInfo>();
            nodeInfos = CreateNodeInfos();
            ResizeNodeInfos(nodeInfos);
            CreateShapes(nodeInfos);
            ResizeCanvas(nodeInfos);
        }

        private List<NodeInfo> CreateNodeInfos()
        {
            // 第一段階として各ノードの位置とサイズを決定する
            List<NodeInfo> nodeInfos = new List<NodeInfo>();
            ObservableCollection<ConstructVM> constructs = viewModel.Constructs;
            double y = MARGIN_Y;
            double gap = NODE_GAP;
            for (int i = 0; i < constructs.Count; i++)
            {
                ConstructVM construct = constructs[i];
                NodeInfo nodeInfo = null;
                if (construct as QuestionConstructVM != null)
                {
                    //質問ノード
                    QuestionConstructVM questionConstruct = (QuestionConstructVM)construct;
                    nodeInfo = new NodeInfo(NodeType.NodeTypeQuestion, questionConstruct);
                    nodeInfo.X = MARGIN_X;
                    nodeInfo.Y = y;
                    nodeInfo.Width = NODE_WIDTH;
                    nodeInfo.Height = NODE_HEIGHT;
                    gap = NODE_GAP;
                }
                else if (construct as QuestionGroupConstructVM != null)
                {
                    QuestionGroupConstructVM questionGroupConstruct = (QuestionGroupConstructVM)construct;
                    nodeInfo = new NodeInfo(NodeType.NodeTypeQuestionGroup, questionGroupConstruct);
                    nodeInfo.X = MARGIN_X;
                    nodeInfo.Y = y;
                    nodeInfo.Width = NODE_WIDTH;
                    nodeInfo.Height = NODE_HEIGHT;
                    gap = NODE_GAP;
                }
                else if (construct as StatementVM != null)
                {
                    //説明文ノード
                    StatementVM statement = (StatementVM)construct;
                    nodeInfo = new NodeInfo(NodeType.NodeTypeStatement, statement);
                    nodeInfo.X = MARGIN_X;
                    nodeInfo.Y = y;
                    nodeInfo.Width = NODE_WIDTH;
                    nodeInfo.Height = NODE_HEIGHT;
                    gap = NODE_GAP;
                }
                else if (construct as IfThenElseVM != null)
                {
                    //分岐ノード(分岐ノードの高さは、矢印部分の最後の水平線の位置を含める)
                    IfThenElseVM ifThenElse = (IfThenElseVM)construct;
                    nodeInfo = new NodeInfo(NodeType.NodeTypeBranch, ifThenElse);
                    nodeInfo.X = MARGIN_X;
                    nodeInfo.Y = y;
                    nodeInfo.Width = NODE_WIDTH;
                    nodeInfo.ContainerHeight = NODE_HEIGHT; //ContainerHeight=菱形の高さ
                    //分岐の考慮
                    List<CondInfo> condInfos = CondInfo.Create(ifThenElse);
                    nodeInfo.CondInfos.AddRange(condInfos);
                    //分岐シェイプの高さは矢印の水平線の高さを含める(条件の数分水平線がでるのでその分拡張する)
                    nodeInfo.Height = nodeInfo.ContainerHeight + LINE_GAP_HEIGHT * condInfos.Count;
                    gap = LINE_GAP_HEIGHT;
                }
                nodeInfos.Add(nodeInfo);
                y += nodeInfo.Height + gap;
            }
            return nodeInfos;
        }

        private bool ShouldResizeNodeInfo(NodeInfo nodeInfo)
        {
            if (nodeInfo.IsNodeTypeQuestion || nodeInfo.IsNodeTypeStatement)
            {
                if (nodeInfo.RightIncomingCount > 1)
                {
                    return true;
                }
            }
            return false;
        }

        private void ResizeNodeInfo(List<NodeInfo> nodeInfos, int targetIndex)
        {
            NodeInfo nodeInfo = nodeInfos[targetIndex];
            int extendCount = nodeInfo.RightIncomingCount - 1;
            double extendHeight = (NODE_HEIGHT / 2.0) * extendCount;
            nodeInfo.Height = nodeInfo.Height + extendHeight;
            for (int i = targetIndex + 1; i < nodeInfos.Count; i++)
            {
                NodeInfo moveNodeInfo = nodeInfos[i];
                moveNodeInfo.Y = moveNodeInfo.Y + extendHeight;
            }
        }

        private void ResizeNodeInfos(List<NodeInfo> nodeInfos)
        {
            // 入る矢印の数に応じて、質問・説明文ノードの高さを増やす
            foreach (NodeInfo nodeInfo in nodeInfos)
            {
                if (nodeInfo.IsNodeTypeBranch)
                {
                    List<CondInfo> condInfos = nodeInfo.CondInfos;
                    foreach (CondInfo condInfo in condInfos)
                    {
                        NodeInfo thenNodeInfo = NodeInfo.Find(nodeInfos, condInfo.TargetConstructId);
                        thenNodeInfo.RightIncomingCount += 1; //ノードに入ってくる矢印の数を最初にカウントしておく
                    }
                }
            }

            // 矢印の数を使ってリサイズ(リサイズ時、ノード以下にあるものを全部移動する必要がある)
            for (int i = 0; i < nodeInfos.Count; i++)
            {
                NodeInfo nodeInfo = nodeInfos[i];
                if (ShouldResizeNodeInfo(nodeInfo))
                {
                    ResizeNodeInfo(nodeInfos, i);
                }
            }
        }

        private void CreateShapes(List<NodeInfo> nodeInfos)
        {
            for (int i = 0; i < nodeInfos.Count; i++)
            {
                NodeInfo nodeInfo = nodeInfos[i];
                ChartShape shape = null;
                if (nodeInfo.IsNodeTypeQuestion)
                {
                    QuestionConstructVM questionConstruct = (QuestionConstructVM)nodeInfo.Construct;
                    QuestionShape questionShape = new QuestionShape();
                    questionShape.X = nodeInfo.X;
                    questionShape.Y = nodeInfo.Y;
                    questionShape.Width = nodeInfo.Width;
                    questionShape.Height = nodeInfo.Height;
                    questionShape.ToolTip = questionConstruct.Title;
                    questionShape.Text = questionConstruct.No;
                    shape = questionShape;
                }
                else if (nodeInfo.IsNodeTypeQuestionGroup)
                {
                    QuestionGroupConstructVM questionGroupConstruct = (QuestionGroupConstructVM)nodeInfo.Construct;
                    QuestionGroupShape questionGroupShape = new QuestionGroupShape();
                    questionGroupShape.X = nodeInfo.X;
                    questionGroupShape.Y = nodeInfo.Y;
                    questionGroupShape.Width = nodeInfo.Width;
                    questionGroupShape.Height = nodeInfo.Height;
                    questionGroupShape.ToolTip = questionGroupConstruct.Title;
                    questionGroupShape.Text = questionGroupConstruct.No;
                    shape = questionGroupShape;
                }
                else if (nodeInfo.IsNodeTypeStatement)
                {
                    StatementVM statement = (StatementVM)nodeInfo.Construct;
                    StatementShape statementShape = new StatementShape();
                    statementShape.X = nodeInfo.X;
                    statementShape.Y = nodeInfo.Y;
                    statementShape.Width = nodeInfo.Width;
                    statementShape.Height = nodeInfo.Height;
                    statementShape.ToolTip = statement.Text;
                    statementShape.Text = statement.No;
                    shape = statementShape;
                }
                else if (nodeInfo.IsNodeTypeBranch)
                {
                    IfThenElseVM ifThenElse = (IfThenElseVM)nodeInfo.Construct;
                    BranchShape branchShape = new BranchShape(ifThenElse, BranchClicked);
                    branchShape.X = nodeInfo.X;
                    branchShape.Y = nodeInfo.Y;
                    branchShape.Width = nodeInfo.Width;
                    branchShape.Height = nodeInfo.ContainerHeight;
                    branchShape.ToolTip = ifThenElse.Title;
                    shape = branchShape;

                    //条件
                    List<CondInfo> condInfos = nodeInfo.CondInfos;

                    double offsetY = 0;
                    foreach (CondInfo condInfo in condInfos)
                    {
                        NodeInfo thenNodeInfo = NodeInfo.Find(nodeInfos, condInfo.TargetConstructId);
                        int thenNodeIndex = nodeInfos.IndexOf(thenNodeInfo);
                        int slot = NodeInfo.PreserveFreeSlot(nodeInfos, i, thenNodeIndex);

                        double lineWidth = NODE_WIDTH + slot * LINE_GAP_WIDTH;
                        thenNodeInfo.RightIncomingCount = thenNodeInfo.RightIncomingCount + 1;

                        CondShape condShape = new CondShape();
                        Point point1 = new Point(branchShape.CenterX, branchShape.Y + branchShape.Height + offsetY);
                        Point point2 = new Point(point1.X, point1.Y + LINE_GAP_HEIGHT);
                        Point point3 = new Point(point2.X + lineWidth, point2.Y); //縦線の始まり
                        thenNodeInfo.RightIncomingIndex += 1;
                        double targetY = thenNodeInfo.Y + thenNodeInfo.RightIncomingIndex * (NODE_HEIGHT / 2.0);
                        Point point4 = new Point(point3.X, targetY); //縦線の終了
                        Point point5 = new Point(thenNodeInfo.X + thenNodeInfo.Width, point4.Y);
                        condShape.Points.Add(point1);
                        condShape.Points.Add(point2);
                        condShape.Points.Add(point3);
                        condShape.Points.Add(point4);
                        condShape.Points.Add(point5);
                        condShape.Text = condInfo.Code;

                        branchShape.CondShapes.Add(condShape);

                        offsetY += LINE_GAP_HEIGHT;
                    }
                }
                shape.AppendTo(canvas);
                if (!nodeInfo.IsNodeTypeBranch && i != nodeInfos.Count - 1)
                {
                    //ノード間の矢印。ただし一つ前が分岐の場合は分岐自体で描画するので不要。
                    NodeInfo nextNodeInfo = nodeInfos[i + 1];

                    ArrowShape arrowShape = new ArrowShape();
                    arrowShape.X1 = nodeInfo.CenterX;
                    arrowShape.Y1 = nodeInfo.Y + nodeInfo.Height;
                    arrowShape.X2 = nodeInfo.CenterX;
                    arrowShape.Y2 = nextNodeInfo.Y;
                    arrowShape.AppendTo(canvas);
                }
            }
        }

        private void ResizeCanvas(List<NodeInfo> nodeInfos)
        {
            if (nodeInfos.Count > 0)
            {
                NodeInfo finishNode = nodeInfos.Last();
                double bottom = finishNode.Y + finishNode.Height + MARGIN_Y;
                canvas.Height = bottom;
            }
        }

        private void okButton_Clicked(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void BranchClicked(IfThenElseVM ifThenElse)
        {
            if (viewModel.EditBranch(ifThenElse, this))
            {
                CreateShapes();
            }
        }
    }
}

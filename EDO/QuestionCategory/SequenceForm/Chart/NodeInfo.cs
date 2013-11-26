using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EDO.QuestionCategory.SequenceForm.Chart
{
    public enum NodeType
    {
        NodeTypeQuestion,
        NodeTypeQuestionGroup,
        NodeTypeStatement,
        NodeTypeBranch
    };

    public class NodeInfo
    {
        public static NodeInfo Find(List<NodeInfo> nodeInfos, string constructId)
        {
            foreach (NodeInfo nodeInfo in nodeInfos)
            {
                if (nodeInfo.Construct.Id == constructId)
                {
                    return nodeInfo;
                }
            }
            return null;
        }

        private static bool IsFreeSlot(List<NodeInfo> nodeInfos, int startIndex, int finishIndex, int freeSlot)
        {
            for (int i = startIndex; i <= finishIndex; i++)
            {
                NodeInfo nodeInfo = nodeInfos[i];

                bool slotUsed =  nodeInfo.IsUsedRightSlot(freeSlot);
                bool existIncomingArrow = false;
                if (slotUsed || existIncomingArrow)
                {
                    return false;
                }
            }
            return true;
        }

        private static void UseFreeSlot(List<NodeInfo> nodeInfos, int startIndex, int finishIndex, int freeSlot)
        {
            for (int i = startIndex; i <= finishIndex; i++)
            {
                NodeInfo nodeInfo = nodeInfos[i];
                nodeInfo.RightSlot.Add(freeSlot);
            }

            NodeInfo finishNode = nodeInfos[finishIndex];
            for (int i = 0; i <= freeSlot; i++)
            {
                finishNode.RightSlot.Add(i);
            }
        }

        public static int PreserveFreeSlot(List<NodeInfo> nodeInfos, int startIndex, int finishIndex)
        {
            int freeSlot = 0;


            int tmp = 0;
            if (startIndex > finishIndex)
            {
                tmp = startIndex;
                startIndex = finishIndex;
                finishIndex = tmp;
            }
            while (true)
            {
                if (IsFreeSlot(nodeInfos, startIndex, finishIndex, freeSlot))
                {
                    UseFreeSlot(nodeInfos, startIndex, finishIndex, freeSlot);
                    return freeSlot;
                }
                freeSlot++;
            }
        }

        public NodeInfo(NodeType nodeType, ConstructVM construct)
        {
            this.nodeType = nodeType;
            this.construct = construct;
            CondInfos = new List<CondInfo>();
            RightSlot = new HashSet<int>();
            LeftSlot = new HashSet<int>();
        }

        private NodeType nodeType;

        public bool IsNodeTypeQuestion { get { return NodeType.NodeTypeQuestion == nodeType; } }
        public bool IsNodeTypeQuestionGroup { get { return NodeType.NodeTypeQuestionGroup == nodeType; } }
        public bool IsNodeTypeStatement { get { return NodeType.NodeTypeStatement == nodeType; } }
        public bool IsNodeTypeBranch { get { return NodeType.NodeTypeBranch == nodeType; } }

        private ConstructVM construct;
        public ConstructVM Construct { get { return construct; } }

        public int RightIncomingCount { get; set; } //リサイズ時に使う
        public int RightIncomingIndex { get; set; } //シェイプ生成時に使う
        public HashSet<int> RightSlot { get; set; }
        public bool IsUsedRightSlot(int slot)
        {
            return RightSlot.Contains(slot);
        }
        public int MaxSlot
        {
            get
            {
                int max = 0;
                foreach (int slot in RightSlot)
                {
                    if (slot > max)
                    {
                        max = slot;
                    }
                }
                return max;
            }
        }

        public int LeftIncomingCount { get; set; }
        public HashSet<int> LeftSlot { get; set; }

        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }

        public double CenterX { get { return X + Width / 2.0; } }
        public double CenterY { get { return Y + Height / 2.0; } }

        public double ContainerHeight { get; set; } //分岐の時の菱形の高さ
        public List<CondInfo> CondInfos { get; set; }
    }
}

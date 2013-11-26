using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core.Model;
using EDO.Core.IO;
using System.Diagnostics;
using EDO.Core.ViewModel;

namespace EDO.Core.Util
{
    public class UndoManager
    {
        private List<UndoInfo> infos;
        private int curIndex;
        private EDOModel curModel;
        private IStatefullVM stateProvider;
        public event EventHandler ModelChangeHandler;
        public UndoManager()
        {
            infos = new List<UndoInfo>();
            curIndex = -1;
            curModel = null;
            stateProvider = null;
            IsEnabled = true;
            transactions = new List<UndoTransaction>();
        }

        public bool IsEnabled { get; set; }

        public void Init(EDOModel edoModel, IStatefullVM stateProvider)
        {
            this.curModel = edoModel;
            this.stateProvider = stateProvider;
            curIndex = -1;
            IsEnabled = true;
            infos.Clear();
            transactions.Clear();
            Memorize();
        }

        public int UndoBufferCount { get; set; }

        private bool ShouldMemorize
        {
            get
            {
                if (!IsEnabled)
                {
                    //無効状態の場合は記憶しない
                    return false;
                }
                if (Transactions.Count > 0)
                {
                    return false;
                }
                if (curModel == null)
                {
                    //カレントモデルがnullの場合は記憶できない
                    return false;
                }
                if (infos.Count == 0)
                {
                    //記憶済みモデル数が0の場合は無条件で記憶すべき
                    return true;
                }
                ////それ以外は前回のモデルと現在のモデルが異なる場合だけ記憶
                //データグリッドで編集モードに移行→値を変えずに行を変えた場合、
                //EndEditが実行されMemorizeが実行されるのでこれをさける。
                bool result = EDOSerializer.IsNotSame(infos[curIndex].Model, curModel);
                if (result)
                {
//                    EDOSerializer.SaveDebug("d:/temp/edo1.xml", infos.Last().Model);
//                    EDOSerializer.SaveDebug("d:/temp/edo2.xml", curModel);
                }
                return result;
            }
        }

        private void OnModelChanged()
        {
            if (ModelChangeHandler != null)
            {
                ModelChangeHandler(this, new EventArgs());
            }
        }

        public bool Memorize()
        {
            if (!ShouldMemorize)
            {
                return false;
            }
            //現在のインデックスから最後までを削除
            //3個あって現在のインデックスが2の場合、インデックス3から0個(3 - 1 - 2 = 0) →　削除しない
            //3個あって現在のインデックスが1の場合、インデックス2から1個(3 - 1 - 1 = 1) →  1削除
            int startIndex = curIndex + 1;
            int removeCount = infos.Count - 1 - curIndex;
//            Debug.WriteLine("RemoveRange({0}, {1})", startIndex, removeCount);
            infos.RemoveRange(startIndex, removeCount);
            if (UndoBufferCount > 0 && infos.Count > UndoBufferCount)
            {
                removeCount = infos.Count - UndoBufferCount;
                infos.RemoveRange(0, removeCount);
            }
            //プロパティ変更後のモデルの複製が保存される。

            EDOModel newModel = EDOSerializer.Clone(curModel);
            infos.Add(new UndoInfo(newModel, stateProvider.SaveState()));
            curIndex = infos.Count - 1;
//            Debug.WriteLine("model's count=" + models.Count + " curIndex=" + curIndex);
//            Debug.Assert(curIndex == models.Count - 1); //末尾をさす
            OnModelChanged();
            return true;
        }

        public bool ReplaceMemorize()
        {
            if (!ShouldMemorize)
            {
                return false;
            }
            //現在のモデルを最後のモデルと入れ替える。
            EDOModel newModel = EDOSerializer.Clone(curModel);
            UndoInfo info = new UndoInfo(newModel, stateProvider.SaveState());
            infos[curIndex] = info;
            OnModelChanged();
            return true;
        }

        public bool CanUndo
        {
            get
            {
                //2個以上あるときのみ有効
                return curIndex > 0;
            }
        }

        public UndoInfo Undo()
        {
            if (!CanUndo)
            {
                return null;
            }
            Debug.Assert(curIndex > 0);
            curIndex--;

            UndoInfo info = infos[curIndex].Copy();
            curModel = info.Model;
            return info;
        }

        public bool CanRedo
        {
            get
            {
                return curIndex <  infos.Count - 1;
            }
        }

        public UndoInfo Redo()
        {
            if (!CanRedo)
            {
                return null;
            }
            Debug.Assert(curIndex < infos.Count - 1);
            curIndex++;
            UndoInfo info = infos[curIndex].Copy();
            curModel = info.Model;
            return info;
        }

        private List<UndoTransaction> transactions;
        public List<UndoTransaction> Transactions 
        {
            get
            {
                return transactions;
            }
        }
    }
}

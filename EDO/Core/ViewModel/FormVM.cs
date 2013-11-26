using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core.Util;
using EDO.Core.Model;
using EDO.Main;
using EDO.Core.View;

namespace EDO.Core.ViewModel
{
    public class FormVM :BaseVM, IStatefullVM
    {
        protected VMState state;

        // メイン画面の書く入力フォームに対応したViewModel。
        // 親としてEDOUnitVMを継承した、StudyUnitVMか、GroupVMを想定。
        public FormVM(EDOUnitVM edoUnit)
            : base(edoUnit)
        {
            state = null;
        }

        public IValidatable View { get; set; }

        public void Reload()
        {
            //DataContextが変更されたタイミングで呼び出される。
            //ここで状態を復元
            Reload(state);
            state = null;
        }

        protected virtual void Reload(VMState state)
        {
        }


        public void LoadState(VMState state)
        {
            //FormVMの場合Reloadで状態を戻すのでここで一旦保存しておく。
            this.state = state; 
        }


        protected virtual Action GetCompleteAction(VMState state)
        {
            return null;
        }

        public virtual void Complete(VMState state)
        {
            Action action = GetCompleteAction(state);
            if (action == null)
            {
                return;
            }
            using (UndoTransaction tx = new UndoTransaction(UndoManager, true))
            {
                action();
            }
        }

        public virtual VMState SaveState()
        {
            return null;
        }

        public virtual bool Validate()
        {
            if (this.View == null)
            {
                return true;
            }
            if (!this.View.Validate())
            {
                return false;
            }
            Complete(null); //GetCompleteActionして実行
            return true;
        }

        public override StudyUnitVM StudyUnit { get { return (StudyUnitVM)Parent; } }
        public GroupVM Group { get { return (GroupVM)Parent; } }
        public virtual void InitRow(object newItem) { }

    }
}

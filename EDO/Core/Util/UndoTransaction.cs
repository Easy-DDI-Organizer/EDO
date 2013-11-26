using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace EDO.Core.Util
{
    public class UndoTransaction :IDisposable
    {
        private UndoManager undoManager;

        public UndoTransaction(UndoManager undoManager) :this(undoManager, false)
        {
        }

        public UndoTransaction(UndoManager undoManager, bool newTransaction)
        {
            if (newTransaction)
            {
                undoManager.Transactions.Clear();
            }
            undoManager.Transactions.Add(this);

            this.undoManager = undoManager;
            Debug.Assert(undoManager != null);
            this.undoManager.IsEnabled = false;
        }


        public void Commit()
        {
            //トランザクションの数が0じゃないと記憶されないので先に削除しておく
            undoManager.Transactions.Remove(this);
            undoManager.IsEnabled = true;
            undoManager.Memorize();
        }


        public void ReplaceCommit()
        {
            //トランザクションの数が0じゃないと記憶されないので先に削除しておく
            undoManager.Transactions.Remove(this);
            undoManager.IsEnabled = true;
            undoManager.ReplaceMemorize();
        }

        public void Dispose()
        {
            // Commit()されないときもあるのでここで削除することも必要。
            undoManager.Transactions.Remove(this);
            this.undoManager.IsEnabled = true;
        }
    }
}

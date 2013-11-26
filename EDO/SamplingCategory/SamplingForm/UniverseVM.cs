using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core.ViewModel;
using EDO.Core.Model;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EDO.SamplingCategory.SamplingForm
{
    public class UniverseVM :BaseVM, IEditableObject
    {
        private Universe universe;
        private Universe bakUniverse;

        public UniverseVM()
            : this(new Universe())
        {
        }

        public UniverseVM(Universe universe)
        {
            this.universe = universe;
            this.bakUniverse = null;
        }

        #region プロパティ

        public Universe Universe { get { return universe; } }
        public override object Model {get {return universe; }}
        public string Id { get { return universe.Id; } }

        public string Title
        {
            get
            {
                return universe.Title;
            }
            set
            {
                if (universe.Title != value)
                {
                    universe.Title = value;
                    NotifyPropertyChanged("Title");
                }
            }
        }

        public string Memo
        {
            get
            {
                return universe.Memo;
            }
            set
            {
                if (universe.Memo != value)
                {
                    universe.Memo = value;
                    NotifyPropertyChanged("Memo");
                }
            }
        }

        public string Method
        {
            get
            {
                return universe.Method;
            }
            set
            {
                if (universe.Method != value)
                {
                    universe.Method = value;
                    NotifyPropertyChanged("Method");
                }
            }
        }

        public bool IsMain
        {
            get
            {
                return universe.IsMain;
            }
            set
            {
                if (universe.IsMain != value)
                {
                    universe.IsMain = value;
                    NotifyPropertyChanged("IsMain");
                }
            }
        }

        public string IsMainString
        {
            get
            {
                return this.IsMain ? "x" : "";
            }
        }

        #endregion

        #region IEditableObject メンバー

        public bool InEdit { get { return inEdit; } }

        private bool inEdit;

        public void BeginEdit()
        {
            if (inEdit)
            {
                return;
            }
            inEdit = true;
            bakUniverse = universe.Clone() as Universe;
        }

        public void CancelEdit()
        {
            if (!inEdit)
            {
                return;
            }
            inEdit = false;
            this.Title = bakUniverse.Title;
            this.Memo = bakUniverse.Memo;
            this.Method = bakUniverse.Method;
            this.IsMain = bakUniverse.IsMain;
        }

        public void EndEdit()
        {
            if (!inEdit)
            {
                return;
            }
            inEdit = false;
            bakUniverse = null;
            Memorize();
        }

        protected override void PrepareValidation()
        {
            if (string.IsNullOrEmpty(Title))
            {
                Title = EMPTY_VALUE;
            }
        }

        #endregion
    }
}

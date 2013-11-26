using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core.Model;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using EDO.Core.ViewModel;
using System.ComponentModel.DataAnnotations;
using EDO.Core.Util;
using EDO.Main;
using EDO.QuestionCategory.QuestionForm;
using EDO.Properties;

namespace EDO.QuestionCategory.CodeForm
{
    public class CodeSchemeVM :BaseVM, ISelectableObject, IStringIDProvider
    {
        public CodeSchemeVM()
            : this(new CodeScheme())
        {
        }
        
        public CodeSchemeVM(CodeScheme codeScheme) :this(codeScheme, new ObservableCollection<CodeVM>())
        {
        }

        public CodeSchemeVM(CodeScheme codeScheme, ObservableCollection<CodeVM> codes)
        {
            this.codeScheme = codeScheme;
            this.codes = codes;
            foreach (CodeVM code in codes)
            {
                code.Parent = this;
            }
            modelSyncher = new ModelSyncher<CodeVM, Code>(this, codes, codeScheme.Codes);
            modelSyncher.AddActionHandler = (param) => { 
                param.CodeSchemeId = codeScheme.Id;
            };
        }

        private CodeScheme codeScheme;

        private ModelSyncher<CodeVM, Code> modelSyncher;

        public CodeScheme CodeScheme { get { return codeScheme; } }

        public override object Model { get { return codeScheme; } }

        public string Id { get { return codeScheme.Id; } }
        private ObservableCollection<CodeVM> codes;
        public ObservableCollection<CodeVM> Codes { get { return codes; } }

        public string Title
        {
            get
            {
                return codeScheme.Title;
            }
            set
            {
                if (codeScheme.Title != value)
                {
                    if (string.IsNullOrEmpty(value))
                    {
                        value = Resources.UntitledCodeScheme; //無題のコード群;
                    }
                    codeScheme.Title = value;
                    NotifyPropertyChanged("Title");
                    Memorize();
                }
            }
        }

        public string Memo
        {
            get
            {
                return codeScheme.Memo;
            }
            set
            {
                if (codeScheme.Memo != value)
                {
                    codeScheme.Memo = value;
                    NotifyPropertyChanged("Memo");
                    Memorize();
                }
            }
        }

        private object selectedCodeItem;
        public object SelectedCodeItem
        {
            get
            {
                return selectedCodeItem;
            }
            set
            {
                if (selectedCodeItem != value)
                {
                    selectedCodeItem = value;
                    NotifyPropertyChanged("SelectedCodeItem");
                }
            }
        }

        public CodeVM SelectedCode
        {
            get
            {
                return SelectedCodeItem as CodeVM;
            }
        }


        public ResponseVM OwnerResponse { get; set; }

        public override StudyUnitVM StudyUnit
        {
            get
            {
                StudyUnitVM studyUnit = base.StudyUnit;
                if (studyUnit == null)
                {
                    if (OwnerResponse != null)
                    {
                        studyUnit = OwnerResponse.StudyUnit;
                    }
                }
                return studyUnit;
            }
        }
    }

}

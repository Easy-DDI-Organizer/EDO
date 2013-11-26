using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using EDO.Core.ViewModel;
using EDO.Main;
using EDO.Core.Model;
using System.Collections.Specialized;
using System.Windows.Input;
using EDO.Core.Util;
using EDO.Core.View;
using System.Windows;
using EDO.QuestionCategory.CategoryForm;
using EDO.QuestionCategory.QuestionForm;
using EDO.Properties;

namespace EDO.QuestionCategory.CodeForm
{
    public class CodeFormVM :FormVM
    {
        public CodeFormVM(StudyUnitVM studyUnit) :base(studyUnit)
        {
            codeSchemes = new ObservableCollection<CodeSchemeVM>();
            foreach (CodeScheme codeSchemeModel in studyUnit.CodeSchemeModels)
            {
                CodeSchemeVM codeScheme = CreateCodeScheme(codeSchemeModel);
                codeSchemes.Add(codeScheme);
            }
            modelSyncher = new ModelSyncher<CodeSchemeVM, CodeScheme>(this, codeSchemes, studyUnit.CodeSchemeModels);
        }

        private CodeFormView Window { get { return (CodeFormView)View; } }

        private ModelSyncher<CodeSchemeVM, CodeScheme> modelSyncher;

        protected override void Reload(VMState state)
        {
            if (state != null)
            {
                SelectedCodeScheme = EDOUtils.Find(codeSchemes, state.State1);
            }
            if (SelectedCodeScheme == null)
            {
                SelectedCodeScheme = EDOUtils.GetFirst(codeSchemes);
            }
        }

        public override VMState SaveState()
        {
            if (SelectedCodeScheme == null)
            {
                return null;
            }
            return new VMState(SelectedCodeScheme.Id);
        }

        public CodeSchemeVM FindCodeScheme(string codeSchemeId)
        {
            foreach (CodeSchemeVM codeScheme in codeSchemes)
            {
                if (codeScheme.CodeScheme.Id == codeSchemeId)
                {
                    return codeScheme;
                }
            }
            return null;
        }

        public CodeVM FindCodeByCategoryId(string categoryId)
        {
            foreach (CodeSchemeVM codeScheme in codeSchemes)
            {
                foreach (CodeVM code in codeScheme.Codes)
                {
                    if (code.CategoryId == categoryId)
                    {
                        return code;
                    }
                }
            }
            return null;
        }

        private CodeSchemeVM CreateCodeScheme(CodeScheme codeSchemeModel)
        {
            ObservableCollection<CodeVM> codes = new ObservableCollection<CodeVM>();
            foreach (Code codeModel in codeSchemeModel.Codes)
            {
                CategoryVM category = StudyUnit.FindCategory(codeModel.CategoryId);
                CodeVM code = new CodeVM(codeModel, category);
                codes.Add(code);
            }
            CodeSchemeVM codeScheme = new CodeSchemeVM(codeSchemeModel, codes)
            {
                Parent = this
            };
            return codeScheme;
        }

        private ObservableCollection<CodeSchemeVM> codeSchemes;
        public ObservableCollection<CodeSchemeVM> CodeSchemes {
            get
            {
                return codeSchemes;
            }
        }

        private CodeSchemeVM selectedCodeScheme;
        public CodeSchemeVM SelectedCodeScheme
        {
            get
            {
                return selectedCodeScheme;
            }
            set
            {
                if (selectedCodeScheme != value)
                {
                    Window.FinalizeDataGrid();
                    selectedCodeScheme = value;
                    NotifyPropertyChanged("SelectedCodeScheme");
                }
            }
        }

        public void AddCodeScheme()
        {
            InputDialog dlg = new InputDialog();
            dlg.Title = Resources.InputCodeSchemeName; // コード群の名前を入力してください;
            dlg.Info = Resources.CodeFormHelpMessage; //※コード群作成後は、「選択肢群から追加」、「選択肢を追加」ボタンを押してコードを割り当ててください。;
            dlg.Owner = Application.Current.MainWindow;
            dlg.ShowDialog();
            if (dlg.DialogResult == true)
            {
                CodeScheme codeSchemeModel = new CodeScheme() {Title = dlg.textBox.Text};
                CodeSchemeVM newCodeScheme = new CodeSchemeVM(codeSchemeModel);
                codeSchemes.Add(newCodeScheme);
                if (SelectedCodeScheme == null)
                {
                    SelectedCodeScheme = newCodeScheme;
                }
                Memorize();
            }
        }


        private ICommand removeCodeSchemeCommand;
        public ICommand RemoveCodeSchemeCommand
        {
            get
            {
                if (removeCodeSchemeCommand == null)
                {
                    removeCodeSchemeCommand = new RelayCommand(param => this.RemoveCodeScheme(), param => this.CanRemoveCodeScheme);
                }
                return removeCodeSchemeCommand;
            }
        }

        public bool CanRemoveCodeScheme
        {
            get
            {
                if (SelectedCodeScheme == null)
                {
                    return false;
                }
                return true;
            }
        }

        public void RemoveCodeScheme()
        {
            if (SelectedCodeScheme == null)
            {
                return;
            }
            using (UndoTransaction tx = new UndoTransaction(UndoManager))
            {
                StudyUnit.RemoveCodeSchemeFromResponse(SelectedCodeScheme);
                SelectedCodeScheme.Codes.Clear();
                codeSchemes.Remove(SelectedCodeScheme);
                tx.Commit();
            }
            SelectedCodeScheme = null;
        }

        private ICommand removeCodeCommand;
        public ICommand RemoveCodeCommand
        {
            get
            {
                if (removeCodeCommand == null)
                {
                    removeCodeCommand = new RelayCommand(param => this.RemoveCode(), param => this.CanRemoveCode);
                }
                return removeCodeCommand;

            }
        }

        public bool CanRemoveCode
        {
            get
            {
                if (SelectedCodeScheme == null)
                {
                    return false;
                }
                if (SelectedCodeScheme.SelectedCode == null)
                {
                    return false;
                }
                return true;
            }
        }

        public void RemoveCode()
        {
            SelectedCodeScheme.Codes.Remove(SelectedCodeScheme.SelectedCode);
            SelectedCodeScheme.SelectedCodeItem = null;
        }

        private void AddCodes(ICollection<CategoryVM> categories)
        {
            foreach (CategoryVM category in categories)
            {
                Code codeModel = new Code();
                SelectedCodeScheme.Codes.Add(new CodeVM(codeModel, category));
            }
            Memorize();
        }

        public void AddFromCategoryScheme()
        {
            if (SelectedCodeScheme == null)
            {
                return;
            }
            SelectCategorySchemeWindow dlg = new SelectCategorySchemeWindow(StudyUnit);
            dlg.Owner = Application.Current.MainWindow;
            dlg.ShowDialog();
            if (dlg.DialogResult != true) 
            {
                return;
            }
            AddCodes(dlg.SelectedCategories);           
        }


        public void AddFromCategory()
        {
            if (SelectedCodeScheme == null)
            {
                return;
            }
            SelectCategoryWindow dlg = new SelectCategoryWindow(StudyUnit);
            dlg.Owner = Application.Current.MainWindow;
            dlg.ShowDialog();
            if (dlg.DialogResult != true)
            {
                return;
            }
            AddCodes(dlg.SelectedCategories);
        }


        public bool Contains(CodeSchemeVM codeScheme)
        {
            return CodeSchemes.Contains(codeScheme);
        }


    
    }
    

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using EDO.Core.ViewModel;
using EDO.Core.Model;
using EDO.Main;
using System.Collections.Specialized;
using System.Windows.Input;
using EDO.Core.Util;

namespace EDO.StudyCategory.CoverageForm
{
    public class CoverageFormVM :FormVM
    {
        public CoverageFormVM(StudyUnitVM studyUnit) :base(studyUnit)
        {
            coverage = studyUnit.CoverageModel;
            topics = new ObservableCollection<CheckOptionVM>();
            foreach (CheckOption topic in coverage.Topics) {
                topics.Add(new CheckOptionVM(this, topic));
            }
            areas = new ObservableCollection<CheckOptionVM>();
            foreach (CheckOption area in coverage.Areas)
            {
                areas.Add(new CheckOptionVM(this, area));
            }
            keywords = new ObservableCollection<KeywordVM>();
            foreach (Keyword keywordModel in coverage.Keywords)
            {
                KeywordVM keyword = new KeywordVM(keywordModel);
                InitKeyword(keyword);
                keywords.Add(keyword);
            }

            modelSyncher = new ModelSyncher<KeywordVM, Keyword>(this, keywords, coverage.Keywords);
        }

        private Coverage coverage;

        private ModelSyncher<KeywordVM, Keyword> modelSyncher;

        public override void InitRow(object newItem)
        {
            if (newItem is KeywordVM)
            {
                InitKeyword((KeywordVM)newItem);
            }
        }

        public void InitKeyword(KeywordVM keyword)
        {
            keyword.Parent = this;
        }

        private ObservableCollection<CheckOptionVM> topics;
        public ObservableCollection<CheckOptionVM> Topics { get { return topics; } }

        private ObservableCollection<KeywordVM> keywords;
        public ObservableCollection<KeywordVM> Keywords { get {return keywords; }}

        private ObservableCollection<CheckOptionVM> areas;
        public ObservableCollection<CheckOptionVM> Areas { get { return areas; } }

        public DateRange DateRange
        {
            get
            {
                return coverage.DateRange;
            }
            set
            {
                if (!DateRange.EqualsDateRange(coverage.DateRange, value))
                {
                    coverage.DateRange = value;
                    NotifyPropertyChanged("DateRange");
                    Memorize();
                }
            }
        }

        public string Memo
        {
            get
            {
                return coverage.Memo;
            }
            set
            {
                if (coverage.Memo != value) 
                {
                    coverage.Memo = value;
                    NotifyPropertyChanged("Memo");
                    Memorize();
                }
            }
        }


        private object selectedKeywordItem;
        public object SelectedKeywordItem
        {
            get
            {
                return selectedKeywordItem;
            }
            set
            {
                if (selectedKeywordItem != value)
                {
                    selectedKeywordItem = value;
                    NotifyPropertyChanged("SelectedKeywordItem");
                }
            }
        }

        public KeywordVM SelectedKeyword
        {
            get
            {
                return selectedKeywordItem as KeywordVM;
            }
        }

        private ICommand removeKeywordCommand;

        public ICommand RemoveKeywordCommand
        {
            get
            {
                if (removeKeywordCommand == null)
                {
                    removeKeywordCommand = new RelayCommand(param => this.RemoveKeyword(), param => this.CanRemoveKeyword);
                }
                return removeKeywordCommand;
            }
        }

        public bool CanRemoveKeyword
        {
            get
            {
                KeywordVM keyword = SelectedKeyword;
                if (keyword == null)
                {
                    return false;
                }
                if (keyword.InEdit)
                {
                    return false;
                }
                return true;
            }
        }

        public void RemoveKeyword()
        {
            keywords.Remove(SelectedKeyword);
            SelectedKeywordItem = null;
        }

    }
}

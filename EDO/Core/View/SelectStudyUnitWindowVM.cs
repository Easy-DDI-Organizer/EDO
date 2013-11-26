using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core.Model;
using System.Collections.ObjectModel;
using EDO.Core.ViewModel;
using System.Windows.Input;
using EDO.Core.Util;
using System.Windows;
using EDO.Core.IO;

namespace EDO.Core.View
{
    public class SelectStudyUnitWindowVM :BaseVM
    {
        public SelectStudyUnitWindowVM(EDOModel fromModel, EDOModel toModel, StudyUnit curStudyUnit, DDIImportOption importOption)
        {
            this.fromModel = fromModel;
            this.toModel = toModel;
            fromStudyUnits = new ObservableCollection<StudyUnit>(fromModel.StudyUnits);
            this.FromStudyUnit = fromModel.StudyUnits.FirstOrDefault();
            toStudyUnits = new ObservableCollection<StudyUnit>(toModel.StudyUnits);
            this.ToStudyUnit = curStudyUnit;

            this.importOption = importOption;
        }

        public Window Window { get; set;  }

        private DDIImportOption importOption;
        public DDIImportOption ImportOption { get { return importOption; } }

        private EDOModel fromModel;
        private EDOModel toModel;

        private ObservableCollection<StudyUnit> fromStudyUnits;
        public ObservableCollection<StudyUnit> FromStudyUnits { get { return fromStudyUnits; } }
        private StudyUnit fromStudyUnit;
        public StudyUnit FromStudyUnit
        {
            get
            {
                return fromStudyUnit;
            }
            set
            {
                if (fromStudyUnit != value)
                {
                    fromStudyUnit = value;
                    NotifyPropertyChanged("FromStudyUnit");
                }
            }
        }
        

        private ObservableCollection<StudyUnit> toStudyUnits;
        public ObservableCollection<StudyUnit> ToStudyUnits { get { return toStudyUnits;  } }
        private StudyUnit toStudyUnit;
        public StudyUnit ToStudyUnit
        {
            get
            {
                return toStudyUnit;
            }
            set
            {
                if (toStudyUnit != value)
                {
                    toStudyUnit = value;
                    NotifyPropertyChanged("ToStudyUnit");
                }
            }
        }

        private ICommand changeImportOptionCommand;
        public ICommand ChangeImportOptionCommand
        {
            get
            {
                if (changeImportOptionCommand == null)
                {
                    changeImportOptionCommand = new RelayCommand(param => ChangeImportOption(), param => CanChangeImportOption);
                }
                return changeImportOptionCommand;
            }
        }

        public bool CanChangeImportOption
        {
            get
            {
                return true;
            }
        }

        public void ChangeImportOption()
        {
            ImportOptionWindowVM vm = new ImportOptionWindowVM(importOption);
            ImportOptionWindow window = new ImportOptionWindow(vm);
            window.Owner = Window;
            if (window.ShowDialog() == true)
            {
                importOption.UpdateValidMenuElems(vm.CheckMenuElems);
            }
        }
    }
}

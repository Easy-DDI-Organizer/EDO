using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows.Input;
using EDO.Core.ViewModel;
using EDO.Main;
using EDO.Core.Model;
using System.Collections.Specialized;
using EDO.Core.Util;
using EDO.Core.View;
using EDO.Properties;

namespace EDO.QuestionCategory.ConceptForm
{
    public class ConceptFormVM :FormVM
    {
        private static readonly string PREFIX = Resources.MajorDivision;

        public ConceptFormVM(StudyUnitVM studyUnit): base(studyUnit)
        {
            this.conceptSchemes = new ObservableCollection<ConceptSchemeVM>();
            int i = 1;
            foreach (ConceptScheme conceptSchemeModel in studyUnit.ConceptSchemeModels) 
            {
                ConceptSchemeVM conceptScheme = new ConceptSchemeVM(conceptSchemeModel) {
                    Parent = this,
                    OrderNo = i++,
                    OrderPrefix = PREFIX
                };
                conceptScheme.InitTitle();
                conceptSchemes.Add(conceptScheme);
            }
            modelSyncher = new ModelSyncher<ConceptSchemeVM, ConceptScheme>(this, conceptSchemes, studyUnit.ConceptSchemeModels);
            allConcepts = new ObservableCollection<ConceptVM>();

        }

        protected override void Reload(VMState state)
        {
            if (state != null)
            {
                SelectedConceptScheme = EDOUtils.Find(conceptSchemes, state.State1);
            }
            if (SelectedConceptScheme == null)
            {
                SelectedConceptScheme = EDOUtils.GetFirst(conceptSchemes);
            }
        }

        public override VMState SaveState()
        {
            if (SelectedConceptScheme == null)
            {
                return null;
            }
            return new VMState(SelectedConceptScheme.Id);
        }

        private ModelSyncher<ConceptSchemeVM, ConceptScheme> modelSyncher;
        private ObservableCollection<ConceptVM> allConcepts;
        private ObservableCollection<ConceptSchemeVM> conceptSchemes;
        public ObservableCollection<ConceptSchemeVM> ConceptSchemes { get { return conceptSchemes; } }

        public ConceptSchemeVM selectedConceptScheme;
        public ConceptSchemeVM SelectedConceptScheme
        {
            get
            {
                return selectedConceptScheme;
            }
            set
            {
                if (selectedConceptScheme != value)
                {
                    selectedConceptScheme = value;
                    NotifyPropertyChanged("SelectedConceptScheme");
                }
            }
        }

        private ICommand addCommand;
        public ICommand AddCommand
        {
            get
            {
                if (addCommand == null)
                {
                    addCommand = new RelayCommand(param => AddConceptScheme(), param => CanAddConceptScheme);
                }
                return addCommand;
            }
        }

        public bool CanAddConceptScheme
        {
            get
            {
                return true;
            }
        }

        public void AddConceptScheme()
        {
            ConceptSchemeVM conceptScheme = new ConceptSchemeVM();
            conceptScheme.OrderNo = EDOUtils.GetMaxOrderNo<ConceptSchemeVM>(conceptSchemes) + 1;
            conceptScheme.OrderPrefix = PREFIX;
            conceptScheme.InitTitle();
            conceptSchemes.Add(conceptScheme);
            SelectedConceptScheme = conceptScheme;
            Memorize();
        }


        private ICommand removeCommand;

        public ICommand RemoveCommand
        {
            get
            {
                if (removeCommand == null)
                {
                    removeCommand = new RelayCommand(param => RemoveConceptScheme(), param => CanRemoveConceptScheme);
                }
                return removeCommand;
            }
        }

        public bool CanRemoveConceptScheme
        {
            get
            {
                if (SelectedConceptScheme == null)
                {
                    return false;
                }
                if (conceptSchemes.Count <= 1)
                {
                    return false;
                }
                return StudyUnit.CanRemoveConceptScheme(SelectedConceptScheme);
            }
        }

        public void RemoveConceptScheme()
        {
            List<ConceptVM> concepts = new List<ConceptVM>();
            concepts.AddRange(SelectedConceptScheme.Concepts);
            StudyUnit.OnRemoveConcepts(concepts);
            conceptSchemes.Remove(SelectedConceptScheme);
            SelectedConceptScheme = conceptSchemes.Last();
        }

        public void UpdateAllConcepts()
        {
            foreach (ConceptSchemeVM conceptScheme in ConceptSchemes)
            {
                foreach (ConceptVM concept in conceptScheme.Concepts)
                {
                    allConcepts.Add(concept);
                }
            }
        }

        public ObservableCollection<ConceptVM> AllConcepts {
            get
            {
                allConcepts.Clear();
                UpdateAllConcepts();
                return allConcepts;
            }
        }

        public ConceptVM FindConcept(string conceptId) {
            return ConceptVM.Find(AllConcepts, conceptId);
        }

        public ConceptSchemeVM FindConceptScheme(ConceptVM concept)
        {
            foreach (ConceptSchemeVM conceptScheme in conceptSchemes)
            {
                if (conceptScheme.Concepts.Contains(concept))
                {
                    return conceptScheme;
                }
            }
            return null;
        }

        private ICommand removeConceptDelegateCommand;
        public ICommand RemoveConceptDelegateCommand
        {
            get
            {
                if (removeConceptDelegateCommand == null)
                {
                    removeConceptDelegateCommand = new RelayCommand(param => RemoveConceptDelegate(), param => CanRemoveConceptDelegate);
                }
                return removeConceptDelegateCommand;
            }
        }

        public bool CanRemoveConceptDelegate
        {
            get
            {
                if (SelectedConceptScheme == null)
                {
                    return false;
                }
                return SelectedConceptScheme.CanRemoveConcept;
            }
        }

        public void RemoveConceptDelegate()
        {
            SelectedConceptScheme.RemoveConcept();
        }
    }


}

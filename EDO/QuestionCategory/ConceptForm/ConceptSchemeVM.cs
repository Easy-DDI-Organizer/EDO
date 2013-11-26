using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;
using EDO.Core.ViewModel;
using EDO.Core.Model;
using EDO.Core.Util;
using System.ComponentModel.DataAnnotations;
using System.Collections.Specialized;
using System.Windows.Input;

namespace EDO.QuestionCategory.ConceptForm
{
    public class ConceptSchemeVM : BaseVM, IOrderedObject, IStringIDProvider
    {
        public ConceptSchemeVM() :this(new ConceptScheme())
        {
        }

        public ConceptSchemeVM(ConceptScheme conceptScheme)
        {
            this.conceptScheme = conceptScheme;
            concepts = new ObservableCollection<ConceptVM>();
            foreach (Concept conceptModel in conceptScheme.Concepts) {
                ConceptVM conceptVM = new ConceptVM(conceptModel);
                conceptVM.Parent = this;
                concepts.Add(conceptVM);
            }
            modelSyncher = new ModelSyncher<ConceptVM, Concept>(this, concepts, conceptScheme.Concepts);
            string message = Properties.Resources.ConceptFormHelpMessage1;
            TestMessage = message;
        }
        public string TestMessage { get; set; }


        private ConceptScheme conceptScheme;
        private ModelSyncher<ConceptVM, Concept> modelSyncher;

        public ConceptScheme ConceptScheme { get { return conceptScheme; } }

        public override object Model {get {return conceptScheme; }}

        public string Id
        {
            get { return conceptScheme.Id; }
        }

        public void InitTitle()
        {
            if (string.IsNullOrEmpty(conceptScheme.Title))
            {
                conceptScheme.Title = EDOUtils.OrderTitle(this);
            }
        }


        public string Title 
        {
            get
            {
                return conceptScheme.Title;
            }
            set
            {
                if (conceptScheme.Title != value)
                {
                    conceptScheme.Title = value;
                    InitTitle();
                    NotifyPropertyChanged("Title");
                    Memorize();
                }
            }
        }

        public string Memo {
            get
            {
                return conceptScheme.Memo;
            }
            set
            {
                if (conceptScheme.Memo != value)
                {
                    conceptScheme.Memo = value;
                    NotifyPropertyChanged("Memo");
                    Memorize();
                }
            }
        }

        private ObservableCollection<ConceptVM> concepts;
        public ObservableCollection<ConceptVM> Concepts {
            get
            {
                return concepts;
            }
        }


        public object selectedConcetItem;
        public object SelectedConceptItem
        {
            get
            {
                return selectedConcetItem;
            }
            set
            {
                if (selectedConcetItem != value)
                {
                    selectedConcetItem = value;
                    NotifyPropertyChanged("SelectedConceptItem");
                }
            }
        }

        public ConceptVM SelectedConcept
        {
            get
            {
                return selectedConcetItem as ConceptVM;
            }
        }

        private ICommand removeConceptCommand;

        public ICommand RemoveConceptCommand
        {
            get
            {
                if (removeConceptCommand == null)
                {
                    removeConceptCommand = new RelayCommand(param => RemoveConcept(), param => CanRemoveConcept);
                }
                return removeConceptCommand;
            }
        }

        public bool CanRemoveConcept
        {
            get
            {
                return CanRemoveConceptExternal(SelectedConcept);
            }
        }

        public void RemoveConcept()
        {
            RemoveConceptExternal(SelectedConcept);
        }

        public bool CanRemoveConceptExternal(ConceptVM concept)
        {
            if (concept == null)
            {
                return false;
            }
            if (concept.InEdit)
            {
                return false;
            }
            return StudyUnit.CanRemoveConcept(concept);
        }

        public void RemoveConceptExternal(ConceptVM concept)
        {
            StudyUnit.OnRemoveConcept(concept);
            concepts.Remove(concept);
            if (SelectedConceptItem == concept)
            {
                SelectedConceptItem = null;
            }
        }

        #region IOrderedObject メンバー

        public int OrderNo { get; set; }
        public string OrderPrefix { get; set; }

        #endregion

        private bool isExpanded;
        public bool IsExpanded
        {
            get
            {
                return isExpanded;
            }
            set
            {
                if (isExpanded != value)
                {
                    isExpanded = value;
                    NotifyPropertyChanged("IsExpanded");
                }
            }
        }
    }
}

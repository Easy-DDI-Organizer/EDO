using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core.ViewModel;
using EDO.Core.Model;
using System.Collections.ObjectModel;
using EDO.Core.Util;
using EDO.StudyCategory.MemberForm;
using EDO.Main;
using System.Windows.Input;
using System.Windows;
using System.Diagnostics;

namespace EDO.SamplingCategory.SamplingForm
{
    public class SamplingVM :BaseVM, IOrderedObject, IStringIDProvider
    {
        public SamplingVM() :this(new Sampling())
        {
        }

        public SamplingVM(Sampling sampling)
        {
            this.sampling = sampling;
            this.universes = new ObservableCollection<UniverseVM>();
            foreach (Universe universeModel in sampling.Universes)
            {
                UniverseVM universe = new UniverseVM(universeModel);
                universe.Parent = this;
                universes.Add(universe);

            }
            samplingMethods = Options.SamplingMethods;
            modelSyncher = new ModelSyncher<UniverseVM, Universe>(this, universes, sampling.Universes);
        }

        public void Init()
        {
            InitTitle();
            MemberVM member = StudyUnit.FindMember(sampling.MemberId);
            updateMember(member);
        }

        private Sampling sampling;
        private ObservableCollection<UniverseVM> universes;
        private ModelSyncher<UniverseVM, Universe> modelSyncher;
        private ObservableCollection<Option> samplingMethods;

        public Sampling Sampling { get { return sampling; } }
        public override object Model { get { return sampling; } }
        public string Id
        {
            get { return sampling.Id; }
        }
        public ObservableCollection<UniverseVM> Universes { get { return universes; } }

        public ObservableCollection<Option> SamplingMethods { get { return samplingMethods; } }

        public void InitTitle()
        {
            if (string.IsNullOrEmpty(sampling.Title))
            {
                sampling.Title = EDOUtils.OrderTitle(this);
            }
        }

        public string Title
        {
            get
            {
                return sampling.Title;
            }
            set
            {
                if (sampling.Title != value)
                {
                    sampling.Title = value;
                    InitTitle();
                    NotifyPropertyChanged("Title");
                    Memorize();
                }
            }
        }


        public string LastName
        {
            get
            {
                return sampling.LastName;
            }
            set
            {
                if (sampling.LastName != value)
                {
                    sampling.LastName = value;
                    NotifyPropertyChanged("LastName");
                    Memorize();
                }
            }
        }

        public string FirstName
        {
            get
            {
                return sampling.FirstName;
            }
            set
            {
                if (sampling.FirstName != value)
                {
                    sampling.FirstName = value;
                    NotifyPropertyChanged("FirstName");
                    Memorize();
                }
            }
        }

        public string OrganizationName
        {
            get
            {
                return sampling.OrganizationName;
            }
            set
            {
                if (sampling.OrganizationName != value)
                {
                    sampling.OrganizationName = value;
                    NotifyPropertyChanged("OrganizationName");
                    Memorize();
                }
            }
        }

        public string Position
        {
            get
            {
                return sampling.Position;
            }
            set
            {
                if (sampling.Position != value)
                {
                    sampling.Position = value;
                    NotifyPropertyChanged("Position");
                    Memorize();
                }
            }
        }


        public DateRange DateRange
        {
            get
            {
                return sampling.DateRange;
            }
            set
            {
                if (!DateRange.EqualsDateRange(sampling.DateRange, value))
                {
//                    Debug.WriteLine("From = " + sampling.DateRange.ToSafeString());
//                    Debug.WriteLine("To = " + value.ToSafeString());
                    sampling.DateRange = value;
                    NotifyPropertyChanged("DateRange");
                    Memorize();
                }
            }
        }

        public string Situation
        {
            get
            {
                return sampling.Situation;
            }
            set
            {
                if (sampling.Situation != value)
                {
                    sampling.Situation = value;
                    NotifyPropertyChanged("Situation");
                    Memorize();
                }
            }
        }

        public string MethodCode
        {
            get
            {
                return sampling.MethodCode;
            }
            set
            {
                if (sampling.MethodCode != value)
                {
                    sampling.MethodCode = value;
                    NotifyPropertyChanged("SelectedSamplingMethod");
                    Memorize();
                }
            }
        }

        public string Method
        {
            //コードブックの書き出しに利用
            get
            {
                return Option.FindLabel(samplingMethods, MethodCode);
            }
        }

        public void GenerateMember()
        {
            StudyUnitVM studyUnit = StudyUnit;
            MemberFormVM memberForm = studyUnit.MemberForm;
            MemberVM newMember = memberForm.AppendMember(sampling.MemberId, this.LastName, this.FirstName, this.OrganizationName, this.Position);
            updateMember(newMember);
        }

        private ICommand selectMemberCommand;

        public ICommand SelectMemberCommand
        {
            get
            {
                if (selectMemberCommand == null)
                {
                    selectMemberCommand = new RelayCommand(param => this.SelectMember(), param => this.CanSelectMember);
                }
                return selectMemberCommand;
            }
        }

        public bool CanSelectMember
        {
            get
            {
                return true;
            }
        }

        private void updateMember(MemberVM member)
        {
            if (member == null)
            {
                return;
            }

            string memberId = null;
            string lastName = null;
            string firstName = null;
            string organizationName = null;
            string position = null;
            if (member != null)
            {
                memberId = member.Member.Id;
                lastName = member.LastName;
                firstName = member.FirstName;
                organizationName = member.OrganizationName;
                position = member.Position;
            }
            sampling.MemberId = memberId;
            this.LastName = lastName;
            this.FirstName = firstName;
            this.OrganizationName = organizationName;
            this.Position = position;
        }

        public void SelectMember()
        {
            SelectMemberWindow dlg = new SelectMemberWindow(StudyUnit);
            dlg.Owner = Application.Current.MainWindow;
            dlg.ShowDialog();
            if (dlg.DialogResult == true)
            {
                MemberVM member = dlg.SelectedMember;
                updateMember(member);
            }
        }

        public string DefaultUniverseGuid 
        {
            get
            {
                if (universes.Count == 0)
                {
                    return null;
                }
                foreach (UniverseVM universe in universes)
                {
                    if (universe.IsMain)
                    {
                        return universe.Id;
                    }
                }
                return universes[0].Id;
            }
        }

        private object selectedUniverseItem;
        public object SelectedUniverseItem
        {
            get
            {
                return selectedUniverseItem;
            }
            set
            {
                if (selectedUniverseItem != value)
                {
                    selectedUniverseItem = value;
                    NotifyPropertyChanged("SelectedUniverseItem");
                }
            }
        }

        public UniverseVM SelectedUniverse
        {
            get
            {
                return SelectedUniverseItem as UniverseVM;
            }
        }

        private ICommand removeUniverseCommand;
        public ICommand RemoveUniverseCommand
        {
            get
            {
                if (removeUniverseCommand == null)
                {
                    removeUniverseCommand = new RelayCommand(param => RemoveUniverse(), param => CanRemoveUniverse);
                }
                return removeUniverseCommand;
            }
        }

        public bool CanRemoveUniverse
        {
            get
            {
                if (SelectedUniverse == null)
                {
                    return false;
                }
                return true;
            }
        }

        public void RemoveUniverse()
        {
            StudyUnit.RemoveUniverseFromVariable(SelectedUniverse);
            Universes.Remove(SelectedUniverse);
            SelectedUniverseItem = null;
        }



        #region IOrderedObject メンバー

        public int OrderNo { get; set; }
        public string OrderPrefix { get; set; }
        
        #endregion
    }
}

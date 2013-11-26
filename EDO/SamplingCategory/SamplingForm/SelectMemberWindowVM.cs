using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core.ViewModel;
using EDO.Main;
using EDO.StudyCategory.MemberForm;
using System.Collections.ObjectModel;

namespace EDO.SamplingCategory.SamplingForm
{
    public class SelectMemberWindowVM :FormVM
    {
        private ObservableCollection<MemberVM> members;

        public SelectMemberWindowVM(StudyUnitVM studyUnit) :base(studyUnit)
        {
            this.members = new ObservableCollection<MemberVM>();
            this.selectedMember = null;
            this.Filter("");
        }

        public ObservableCollection<MemberVM> Members { get { return members; } }

        private MemberVM selectedMember;
        public MemberVM SelectedMember {
            get
            {
                return selectedMember;
            }
            set
            {
                if (selectedMember != value)
                {
                    selectedMember = value;
                    NotifyPropertyChanged("selectedMember");
                }
            }
        }

        public void Filter(string text)
        {
            StudyUnitVM studyUnit = (StudyUnitVM)this.Parent;

            string lowerText = text.ToLower();

            members.Clear();
            foreach (MemberVM member in studyUnit.MemberForm.Members)
            {
                if (string.IsNullOrEmpty(lowerText) || member.FullName.ToLower().Contains(lowerText))
                {
                    members.Add(member);
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using EDO.Core.ViewModel;
using System.ComponentModel.DataAnnotations;
using EDO.Core.Model;
using System.ComponentModel;
using System.Diagnostics;
using EDO.Core.Util;

namespace EDO.StudyCategory.MemberForm
{
    public class MemberVM :BaseVM, IEditableObject
    {
        public static List<MemberVM> FindByOrganizationName(ObservableCollection<MemberVM> members, string organizationName)
        {
            List<MemberVM> foundMembers = new List<MemberVM>();
            foreach (MemberVM member in members)
            {
                if (member.OrganizationName == organizationName)
                {
                    foundMembers.Add(member);
                }
            }
            return foundMembers;
        }

        public static MemberVM FindById(ObservableCollection<MemberVM> members, string memberId)
        {
            if (memberId == null)
            {
                return null;
            }
            foreach (MemberVM member in members)
            {
                if (member.Member.Id == memberId)
                {
                    return member;
                }
            }
            return null;
        }

        public static int GetMaxNo(Collection<MemberVM> items)
        {
            int maxNo = 0;
            foreach (MemberVM member in items)
            {
                if (member.No > maxNo)
                {
                    maxNo = member.No;
                }
            }
            return maxNo;
        }

        public MemberVM()
            : this(new Member(), null)
        {
        }

        public MemberVM(Member member, string organizationName)
        {
            this.member = member;
            this.bakMember = null;
            this.organizationName = organizationName;
            this.bakOrganizationName = null;
        }

        #region フィールド・プロパティ
        private Member member;
        private Member bakMember;
        private string organizationName;
        private string bakOrganizationName;

//        private Option role;
//        private Option bakRole;

        public Member Member
        {
            get
            {
                return member;
            }
        }

        public override object Model
        {
            get
            {
                return member;
            }
        }

        public string OrganizationId
        {
            get
            {
                return member.OrganizationId;
            }
            set
            {
                member.OrganizationId = value;
            }
        }

        private int no;
        public int No { 
            get 
            {
                return no;
            }
            set
            {
                if (no != value)
                {
                    no = value;
                    NotifyPropertyChanged("No");
                }
            }
        }

        public string RoleCode
        { 
            get
            {
                return member.RoleCode;
            }
            set{
                if (member.RoleCode != value)
                {
                    member.RoleCode = value;
                    NotifyPropertyChanged("RoleCode");
                }
            }
        }

        public string LastName
        {
            get
            {
                return member.LastName;
            }
            set
            {
                if (member.LastName != value)
                {
                    member.LastName = value;
                    NotifyPropertyChanged("LastName");
                }
            }
        }

        public string FirstName { 
            get
            {
                return member.FirstName;
            }
            set
            {
                if (member.FirstName != value)
                {
                    member.FirstName = value;
                    NotifyPropertyChanged("FirstName");
                }
            }
        }

        public string FullName
        {
            get
            {
                return this.LastName + " " + this.FirstName;
            }
        }

        public string Position
        {
            get
            {
                return member.Position;
            }
            set
            {
                if (member.Position != value)
                {
                    member.Position = value;
                    NotifyPropertyChanged("Position");
                }
            }
        }

        public string OrganizationName
        {
            get
            {
                return organizationName;
            }
            set
            {
                if (organizationName != value)
                {
                    organizationName = value;
                    NotifyPropertyChanged("OrganizationName");
                }
            }
        }

        public ObservableCollection<Option> Roles { get; set; }

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

            bakMember = member.Clone() as Member;
//            bakRole = role == null ? null : role.Clone() as Option;
            bakOrganizationName = organizationName;
        }

        public void CancelEdit()
        {
            if (!inEdit)
            {
                return;
            }
            inEdit = false;
            this.LastName = bakMember.LastName;
            this.FirstName = bakMember.FirstName;
            this.Position = bakMember.Position;
            this.RoleCode = bakMember.RoleCode;
            this.OrganizationName = bakOrganizationName;
            bakMember = null;
        }

        public Action<IEditableObject> EndEditAction { get; set; }

        public void EndEdit()
        {
            if (!inEdit)
            {
                return;
            }
            inEdit = false;
            bakMember = null;
            bakOrganizationName = null;
            if (this.EndEditAction != null)
            {
                this.EndEditAction(this);
            }
            Memorize();
        }

        #endregion

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            //builder.Append("MemberVM No=").Append(No).AppendLine();
            //builder.Append("    IsEditing=").Append(IsEditing).AppendLine();
            //builder.Append("    curMember=").AppendLine(curMember.ToDebugString());
            //builder.Append("    orgMember=").AppendLine(orgMember.ToDebugString());
            //builder.Append("    bakMember=").AppendLine(bakMember.ToDebugString());
            //builder.Append("    curOrganization=").AppendLine(curOrganization.ToDebugString());
            //builder.Append("    orgOrganization=").AppendLine(orgOrganization.ToDebugString());
            //builder.Append("    bakOrganization=").AppendLine(bakOrganization.ToDebugString());
            return builder.ToString();
        }

        public bool IsLeader
        {
            get
            {
                return member.IsLeader;
            }
        }


        protected override void PrepareValidation()
        {
            if (string.IsNullOrEmpty(RoleCode))
            {
                RoleCode = Roles.First().Code;
            }
            if (string.IsNullOrEmpty(LastName))
            {
                LastName = EMPTY_VALUE;
            }
            if (string.IsNullOrEmpty(FirstName))
            {
                FirstName = EMPTY_VALUE;
            }
            if (string.IsNullOrEmpty(OrganizationName))
            {
                OrganizationName = EMPTY_VALUE;
            }
        }
    }

}

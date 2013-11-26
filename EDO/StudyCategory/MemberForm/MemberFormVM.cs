using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using EDO.Core.ViewModel;
using EDO.Main;
using EDO.Core.Model;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;
using EDO.Core.Util;
using System.Windows;
using EDO.Properties;

namespace EDO.StudyCategory.MemberForm
{
    public class MemberFormVM :FormVM
    {
        private static void InitRole(MemberVM member, bool isLeader)
        {
            if (isLeader)
            {
                member.Roles = Options.FirstRoles;
            }
            else
            {
                member.Roles = Options.OtherRoles;
            }
        }

        private static void InitRoleCode(MemberVM member, bool isLeader)
        {
            if (isLeader)
            {
                member.RoleCode = Options.ROLE_DAIHYOSHA_CODE;
            }
            else
            {
                member.RoleCode = Options.ROLE_OTHER_CODE;
            }
        }

        public MemberFormVM(StudyUnitVM studyUnit) :base(studyUnit)
        {
            members = new ObservableCollection<MemberVM>();
            organizations = new ObservableCollection<OrganizationVM>();

            //OrganizationVMのリストを生成する(画面下部に表示される他、メンバーの組織選択コンボで使われる)
            int i = 1;
            foreach (Organization organizationModel in studyUnit.OrganizationModels)
            {
                OrganizationVM organization = new OrganizationVM(organizationModel);
                InitExistOrganization(organization, i++);
                organizations.Add(organization);
            }

            //MemberVMのリストを生成する
            i = 1;
            foreach (Member memberModel in studyUnit.MemberModels)
            {
                OrganizationVM organization = OrganizationVM.Find(organizations, memberModel.OrganizationId);
                MemberVM member = new MemberVM(memberModel, organization.OrganizationName);
                InitExistMember(member, i++);
                members.Add(member);
            }
            memberSyncher = new ModelSyncher<MemberVM, Member>(this, members, studyUnit.MemberModels);
            organizationSyncher = new ModelSyncher<OrganizationVM, Organization>(this, organizations, studyUnit.OrganizationModels);
        }

        #region フィールド・プロパティ

        private ModelSyncher<MemberVM, Member> memberSyncher;
        private ModelSyncher<OrganizationVM, Organization> organizationSyncher;

        private ObservableCollection<MemberVM> members;
        public ObservableCollection<MemberVM> Members { get { return members; } }

        private ObservableCollection<OrganizationVM> organizations;
        public ObservableCollection<OrganizationVM> Organizations { get { return organizations; } }

        private object selectedMemberItem;
        public object SelectedMemberItem {
            get
            {
                return selectedMemberItem;
            }
            set
            {
                if (selectedMemberItem != value)
                {
                    selectedMemberItem = value;
                    NotifyPropertyChanged("SelectedMemberItem");
                }
            }
        }

        private object selectedOrganizationItem;
        public object SelectedOrganizationItem
        {
            get
            {
                return selectedOrganizationItem;
            }
            set
            {
                if (selectedOrganizationItem != value)
                {
                    selectedOrganizationItem = value;
                    NotifyPropertyChanged("SelectedOrganizationItem");
                }
            }
        }

        public MemberVM SelectedMember
        {
            get
            {
                return SelectedMemberItem as MemberVM;
            }
        }

        public OrganizationVM SelectedOrganization
        {
            get
            {
                return SelectedOrganizationItem as OrganizationVM;
            }
        }

        private int NextMemberNo
        {
            get
            {
                return MemberVM.GetMaxNo(members) + 1;
            }
        }

        private int NextOrganizationNo
        {
            get
            {
                return OrganizationVM.GetMaxNo(organizations) + 1;
            }
        }

        #endregion

        #region 組織に関する処理

        private void InitOrganization(OrganizationVM organization, int no)
        {
            organization.Parent = this;
            organization.No = no;
        }

        private void InitExistOrganization(OrganizationVM organization, int no)
        {
            InitOrganization(organization, no);
        }

        private void InitNewOrganization(OrganizationVM organization)
        {
            InitOrganization(organization, NextOrganizationNo);
        }

        private OrganizationVM FindOrganizationByName(string name)
        {
            return OrganizationVM.FindByName(organizations, name);
        }

        #endregion

        #region メンバーに関する処理

        private void InitMember(MemberVM member, bool isLeader, int no)
        {
            InitRole(member, isLeader);
            member.Parent = this;
            member.No = no;
            member.EndEditAction = new Action<IEditableObject>(member_ItemEndEdit);
        }

        private void InitExistMember(MemberVM member, int no)
        {
            InitMember(member, member.IsLeader, no);
        }

        public override void InitRow(object newItem)
        {
            if (newItem is MemberVM)
            {
                InitNewMember((MemberVM)newItem);
            }
        }

        public void InitNewMember(MemberVM member)
        {
            InitMember(member, members.Count == 1, NextMemberNo);
        }

        public void member_ItemEndEdit(IEditableObject x)
        {
            ///// MemberVMの編集終了後の処理
            MemberVM editedMember = (MemberVM)x;
            if (string.IsNullOrEmpty(editedMember.OrganizationName))
            {
                return;
            }
            OrganizationVM existOrganization = FindOrganizationByName(editedMember.OrganizationName);
            if (existOrganization == null)
            {
                ////既存の組織に一致するものがない場合は新しく作りVMに追加しておく。
                existOrganization = new OrganizationVM(editedMember.OrganizationName);
                InitNewOrganization(existOrganization);
                organizations.Add(existOrganization);
            }
            editedMember.OrganizationId = existOrganization.Id;

            //同じ組織を参照しているメンバーがあるかもしれないのでその画面表示も追随させる
            //List<MemberVM> relatedMembers = this.GetSameOrganizationMemberVMs(editedMember);
            //foreach (MemberVM member in relatedMembers)
            //{
            //    member.NotifyPropertyChanged("OrganizationName");
            //}
        }

        #endregion

        #region メンバーの生成(データの収集方法の画面から呼ばれる)
        public MemberVM AppendMember(string memberId, string lastName, string firstName, string organizationName, string position)
        {
            //一応この三つは必須扱い?
            if (string.IsNullOrEmpty(lastName) || string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(organizationName)) {
                return null;
            }

            //メンバーを探す
            MemberVM existMember = MemberVM.FindById(this.Members, memberId);
            if (existMember != null && existMember.LastName == lastName && existMember.FirstName == firstName)
            {
                //メンバーが存在しかつ名前がかわっていない場合はそのメンバーを返す
                //(この場合データの収集方法の画面で指定された組織、ポジションは変更されない)。
                return existMember;
            }

            //上記意外は新しいメンバーとみなし生成する。
            MemberVM newMember = new MemberVM();
            members.Add(newMember);
            InitNewMember(newMember);
            newMember.LastName = lastName;
            newMember.FirstName = firstName;
            newMember.OrganizationName = organizationName;
            newMember.Position = position;
            InitRoleCode(newMember, members.Count == 1);
            member_ItemEndEdit(newMember);
            return newMember;
        }

        #endregion


        #region メンバーの削除コマンド

        private ICommand removeMemberCommand;
        public ICommand RemoveMemberCommand { 
            get 
            {
                if (removeMemberCommand == null) {
                    removeMemberCommand = new RelayCommand(param => RemoveMember(), param => CanRemoveMember);
                }
                return removeMemberCommand;
            }
        }

        public bool CanRemoveMember
        {
            get
            {
                MemberVM member = SelectedMember;
                if (member == null)
                {
                    return false;
                }
                if (member.IsLeader)
                {
                    return false;
                }
                return !member.InEdit;
            }
        }

        public void RemoveMember()
        {
            members.Remove(SelectedMember);
            SelectedMemberItem = null;
        }

        #endregion


        #region 組織の削除コマンド
        private ICommand removeOrganizationCommand;
        public ICommand RemoveOrganizationCommand
        {
            get
            {
                if (removeOrganizationCommand == null)
                {
                    removeOrganizationCommand = new RelayCommand(param => this.RemoveOrganization(), param => this.CanRemoveOrganization);
                }
                return removeOrganizationCommand;
            }
        }

        public bool CanRemoveOrganization
        {
            get
            {
                OrganizationVM organization = SelectedOrganization;
                if (organization == null)
                {
                    return false;
                }
                return true;
            }
        }

        public void RemoveOrganization()
        {
            List<MemberVM> usedMembers = MemberVM.FindByOrganizationName(members, SelectedOrganization.OrganizationName);
            if (usedMembers.Count > 0)
            {
                string msg = EDOUtils.CannotDeleteError(Resources.Member, usedMembers, param => param.FullName);
                MessageBox.Show(msg);
                return;
            }
            organizations.Remove(SelectedOrganization);
            SelectedOrganizationItem = null;
        }

        #endregion 
    }
}

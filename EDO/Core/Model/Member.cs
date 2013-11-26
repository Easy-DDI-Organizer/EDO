using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core.Util;

namespace EDO.Core.Model
{
    public class Member :ICloneable, IIDPropertiesProvider
    {
        public static void ChangeOrganizationId(List<Member> members, string oldId, string newId)
        {
            //組織のIDが変わったのでメンバーで参照しているIDも変更する
            foreach (Member member in members)
            {
                if (member.OrganizationId == oldId)
                {
                    member.OrganizationId = newId;
                }
            }
        }

        public static Member FindByName(List<Member> members, string lastName, string firstName)
        {
            foreach (Member member in members)
            {
                if (member.LastName == lastName && member.FirstName == firstName)
                {
                    return member;
                }
            }
            return null;
        }

        public string[] IdProperties
        {
            get
            {
                return new string[] { "Id" };
            }
        }

        public Member()
        {
            Id = IDUtils.NewGuid();
        }
        public string Id { get; set; }
        public string RoleCode { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Position {get; set; }
        public string OrganizationId { get; set; }

        public override string ToString()
        {
            return string.Format("Id={0}, RoleCode={1} LastName={2} FirstName={3} Position={4} OrganizationId={5}",
                Id,
                RoleCode, LastName, FirstName, Position, OrganizationId);
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public bool IsLeader
        {
            get
            {
                return RoleCode == Options.ROLE_DAIHYOSHA_CODE;
            }
        }
    }
}

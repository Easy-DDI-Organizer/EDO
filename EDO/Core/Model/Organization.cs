using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core.Util;

namespace EDO.Core.Model
{
    public class Organization :ICloneable, IIDPropertiesProvider
    {
        public static Organization GetOrganization(List<Organization> organizations, string id)
        {
            foreach (Organization organization in organizations)
            {
                if (organization.Id == id)
                {
                    return organization;
                }
            }
            return null;
        }

        public static Organization FindOrganizationByName(List<Organization> organizations, string organizationName)
        {
            foreach (Organization organization in organizations)
            {
                if (organization.OrganizationName == organizationName)
                {
                    return organization;
                }
            }
            return null;
        }
        public static List<string> GetOrganizationNames(List<Organization> organizations)
        {
            List<string> names = new List<string>();
            foreach (Organization organization in organizations)
            {
                names.Add(organization.OrganizationName);
            }
            return names;
        }


        public string[] IdProperties
        {
            get
            {
                return new string[] { "Id" };
            }
        }

        public Organization() :this(null)
        {
        }

        public Organization(string organizationName)
        {
            Id = IDUtils.NewGuid();
            OrganizationName = organizationName;
        }

        public string Id { get; set; }
        public string OrganizationName { get; set; }

        public override string ToString()
        {
            return string.Format("OrganizationId={0} OrganizationName={1}", Id, OrganizationName);
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core.Util;

namespace EDO.Core.Model
{
    public class EDOModel
    {
        public static EDOModel createDefault()
        {
            EDOModel data = new EDOModel();
            StudyUnit study1 = StudyUnit.CreateDefault();
            data.StudyUnits.Add(study1);
            return data;
        }

        public EDOModel()
        {
            Id = IDUtils.NewGuid();
            this.StudyUnits = new List<StudyUnit>();
        }
        public string Id { get; set; }
        public Group Group { get; set; }
        public List<StudyUnit> StudyUnits { get; set; }
        public bool IsExistGroup
        {
            get
            {
                return this.Group != null;
            }
        }
    }
}

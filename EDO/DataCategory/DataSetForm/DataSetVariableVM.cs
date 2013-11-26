using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core.ViewModel;
using EDO.Core.Model;
using System.Collections.ObjectModel;
using EDO.Core.Util;

namespace EDO.DataCategory.DataSetForm
{
    public class DataSetVariableVM :BaseVM
    {
        public static DataSetVariableVM Find(ICollection<DataSetVariableVM> variables, string variableId)
        {
            foreach (DataSetVariableVM variable in variables)
            {
                if (variable.Id == variableId)
                {
                    return variable;
                }
            }
            return null;
        }

        public static int GetMaxPosition(ObservableCollection<DataSetVariableVM> variables)
        {
            int max = 0;
            foreach (DataSetVariableVM variable in variables)
            {
                if (variable.Position > max)
                {
                    max = variable.Position;
                }
            }
            return max;
        }


        public DataSetVariableVM()
        {
        }

        public string TitleNo
        {
            get
            {
                return EDOUtils.ToTitleNo(Title);
            }
        }

        public string Id { get; set; }
        public override object Model { get { return Id; } }
        public string Title { get; set; }
        public string Label { get; set; }
        public string ConceptTitle { get; set; }
        public int Position { get; set; }
    }
}

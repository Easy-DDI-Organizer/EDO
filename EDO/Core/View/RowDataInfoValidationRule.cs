using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;
using System.Globalization;
using System.ComponentModel;
using EDO.Core.ViewModel;

namespace EDO.Core.View
{
    public class RowDataInfoValidationRule :ValidationRule
    {
        private BaseVM GetBaseVM(BindingGroup group)
        {
            if (group.Items.Count == 0)
            {
                return null;
            }
            return (BaseVM)group.Items[0];
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            BindingGroup group = (BindingGroup)value;
            StringBuilder error = null;
            foreach (var item in group.Items)
            {
                // aggregate errors
                IDataErrorInfo info = item as IDataErrorInfo;
                if (info != null)
                {
                    if (!string.IsNullOrEmpty(info.Error))
                    {
                        if (error == null)
                            error = new StringBuilder();
                        error.Append((error.Length != 0 ? ", " : "") + info.Error);
                    }
                }
            }
            BaseVM baseVM = GetBaseVM(group);
            if (error != null)
            {
                baseVM.SetStatusMessage(error.ToString(), true);
                return new ValidationResult(false, error.ToString());
            }
            baseVM.SetStatusMessage("", false);
            return ValidationResult.ValidResult;
        }
    }
}

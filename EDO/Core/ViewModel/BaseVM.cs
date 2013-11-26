using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core.View;
using System.ComponentModel;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using EDO.Core.Model;
using EDO.Core.Util;
using EDO.Main;
using EDO.Properties;

namespace EDO.Core.ViewModel
{
    public class BaseVM :INotifyPropertyChanged, IDataErrorInfo
    {
        public static readonly string EMPTY_VALUE = Resources.UndefinedValue; //<未入力>;

        public static bool IsNotEmpty(string value)
        {
            return !string.IsNullOrEmpty(value) && value != EMPTY_VALUE;
        }

        private static ValidationAttribute[] GetValidationAttributes(PropertyInfo property)
        {
            return (ValidationAttribute[])property.GetCustomAttributes(typeof(ValidationAttribute), true);
        }

        public class PropertyValidator
        {
            public PropertyValidator(string propertyName, MethodInfo getMethod, ValidationAttribute[] validationAttributes)
            {
                this.PropertyName = propertyName;
                this.GetMethod = getMethod;
                this.ValidationAttributes = validationAttributes;
            }
            public string PropertyName { get; set; }
            public MethodInfo GetMethod { get; set; }
            public ValidationAttribute[] ValidationAttributes { get; set; }

            public string validate(Object viewModel)
            {
                var value = this.GetMethod.Invoke(viewModel, null);
                var errors = new List<string>();
                foreach (ValidationAttribute va in this.ValidationAttributes)
                {
                    if (!va.IsValid(value))
                    {
                        errors.Add(va.ErrorMessage);
                    }
                }
                return string.Join(Environment.NewLine, errors);
            }
        }

        public delegate void ItemEndEditEventHandler(IEditableObject sender);

        private Dictionary<string, PropertyValidator> propertyValidators;

        public BaseVM() :this(null)
        {
        }

        public BaseVM(BaseVM parent)
        {
            this.Parent = parent;
            propertyValidators = new Dictionary<string, PropertyValidator>();
            foreach (PropertyInfo propertyInfo in this.GetType().GetProperties())
            {
                ValidationAttribute[] vas = GetValidationAttributes(propertyInfo);
                if (vas.Length > 0 && propertyInfo.GetGetMethod() != null)
                {
                    PropertyValidator validator = new PropertyValidator(propertyInfo.Name, propertyInfo.GetGetMethod(), vas);
                    propertyValidators[propertyInfo.Name] = validator;
                }
            }
        }

        public BaseVM Parent { get; set; }

        public virtual object Model { get { return null; } }

        public virtual MainWindowVM Main { get {return EDOUtils.GetAncestorViewModel<MainWindowVM>(this); } }

        public virtual UndoManager UndoManager
        {
            get
            {
                MainWindowVM  main = Main;
                if (main == null)
                {
                    return null;
                }
                return main.UndoManager;
            }
        }

        protected virtual bool CanMemorize
        {
            get
            {
                return UndoManager != null;
            }
        }

        protected void Memorize()
        {
            if (CanMemorize)
            {
                UndoManager.Memorize();
            }
        }

        public virtual StudyUnitVM StudyUnit { get { return EDOUtils.GetAncestorViewModel<StudyUnitVM>(this); } }

        public virtual void SetStatusMessage(string message, bool isError)
        {
            if (Main == null)
            {
                Debug.WriteLine("Main is null");
                return;
            }
            Main.SetStatusMessage(message, isError);
        }


        #region INotifyPropertyChanged メンバー

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }

            //if (!propertyName.StartsWith("Selected"))
            //{
            //    UndoManager undoManager = UndoManager.Instance;
            //    undoManager.Memorize();
            //}
        }

        #endregion

        #region IDataErrorInfo メンバー

        protected virtual void PrepareValidation()
        {
        }

        public virtual string Error
        {
            get
            {
                //var errors = from i in validators
                //             from v in i.Value
                //             where !v.IsValid(propertyGetters[i.Key](this))
                //             select v.ErrorMessage;
                //return string.Join(Environment.NewLine, errors.ToArray());
                PrepareValidation();

                List<string> errors = new List<string>();
                foreach (string columnName in propertyValidators.Keys) {
                    string columnError = this[columnName];
                    if (!string.IsNullOrEmpty(columnError))
                    {
                        errors.Add(columnError);
                    }

                }
                if (errors.Count == 0)
                {
                    return null;
                }
                return string.Join(" ", errors.ToArray());
            }
        }

        public bool IsIgnoreValidation { get; set; }

        public virtual string this[string columnName]
        {
            get
            {
                if (IsIgnoreValidation)
                {
                    return string.Empty;
                }

                if (propertyValidators.ContainsKey(columnName))
                {
                    PropertyValidator validator = propertyValidators[columnName];
                    return validator.validate(this);
                }
                return string.Empty;
            }
        }

        #endregion
    }
}

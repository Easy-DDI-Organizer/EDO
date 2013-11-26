using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Windows.Controls;
using System.ComponentModel;

namespace EDO
{
    public class MenuItem :INotifyPropertyChanged
    {
        private string title;
        public string Title
        {
            set
            {
                this.title = value;
            }
            get
            {
                return this.title;
            }
        }

        private string window;
        public string Window
        {
            set { this.window = value; }
            get { return this.window; }
        }

        private ArrayList menus;
        public ArrayList Menus
        {
            set
            {
                this.menus = value;
            }
            get
            {
                return this.menus;
            }
        }

        public MenuItem(string title)
            : this(title, null)
        {
        }

        public MenuItem(string title, string window)
        {
            this.title = title;
            this.window = window;
            this.menus = new ArrayList();
            this.IsCategory = false;
            this.IsCode = false;
            this.IsSelected = false;
        }


        public void AddMenu(MenuItem childMenu)
        {
            this.menus.Add(childMenu);
        }


        public override string ToString()
        {
            return "abc";
        }

        public bool IsCategory { get; set; }
        public bool IsCode { get; set; }

        private bool isSelected;
        public bool IsSelected {
            get
            {
                return isSelected;
            }
            set
            {
                if (isSelected != value)
                {
                    isSelected = value;
                    NotifyPropertyChanged("IsSelected");
                }
            }
        }

        public UserControl UserControl { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}

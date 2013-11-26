using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Main;
using EDO.Core.ViewModel;
using System.Collections.ObjectModel;
using EDO.QuestionCategory.QuestionForm;

namespace EDO.QuestionCategory.QuestionForm
{
    public class SelectResponseWindowVM :FormVM
    {
        public SelectResponseWindowVM(StudyUnitVM studyUnit, ResponseVM excludeResponse)
            : base(studyUnit)
        {
            this.excludeResponse = excludeResponse;
            this.responses = new ObservableCollection<ResponseVM>();
            Filter("");
        }

        private ResponseVM excludeResponse;

        private ObservableCollection<ResponseVM> responses;
        public ObservableCollection<ResponseVM> Responses { get { return responses; } }

        private ResponseVM selectedResponse;
        public ResponseVM SelectedResponse
        {
            get
            {
                return selectedResponse;
            }
            set
            {
                if (selectedResponse != value)
                {
                    selectedResponse = value;
                    NotifyPropertyChanged("SelectedResponse");
                }
            }
        }

        public void Filter(string text)
        {
            StudyUnitVM studyUnit = (StudyUnitVM)Parent;

            string lowerText = text.ToLower();
            responses.Clear();
            foreach (ResponseVM response in studyUnit.QuestionForm.AllResponses)
            {
                if (response == excludeResponse)
                {
                    continue;
                }
                if (string.IsNullOrEmpty(lowerText) || response.Title.ToLower().Contains(lowerText))
                {
                    responses.Add(response);
                }
            }
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;
using EDO.Core.ViewModel;
using EDO.Core.Model;
using System.ComponentModel.DataAnnotations;
using EDO.QuestionCategory.CodeForm;

namespace EDO.QuestionCategory.QuestionForm
{
    public class QuestionVM :BaseVM, IEditableObject, IStringIDProvider, ITitleProvider
    {
        public static QuestionVM QuestionFor(ICollection<QuestionVM> questions, ResponseVM response)
        {
            foreach (QuestionVM question in questions)
            {
                if (question.Response == response)
                {
                    return question;
                }
            }
            return null;
        }

        // 読み込み用のコンストラクタとデフォルトコンストラクタで処理が異なるので
        // コンストラクタのオーバーロードは利用していない.
        public QuestionVM()
        {
            //新規データ追加用のコンストラクタ
            this.question = new Question();
            this.Response = new ResponseVM(question.Response);
        }

        public QuestionVM(Question question)
        {
            //データ読み込み用のコンストラクタ
            //Responseは後でセットするので引数としては渡さない
            this.question = question;
        }

        public ObservableCollection<Option> ResponseTypes { get; set; }

        private Question question;
        private Question bakQuestion;
        private string bakResponseTypeCode;

        public Question Question { get { return question; } }
        public override object Model { get { return question; } }
        public string Id { get { return question.Id; } }

        public bool IsCreatedVariable
        {
            get
            {
                return question.IsCreatedVariable;
            }
            set
            {
                question.IsCreatedVariable = value;
            }
        }

        public bool IsCreatedConstruct
        {
            get
            {
                return question.IsCreatedConstruct;
            }
            set
            {
                question.IsCreatedConstruct = value;
            }
        }

        void ResponsePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //回答方法編集ペインでTitleを変更したときGridの回答方法を連動して変更するための処理
            if (e.PropertyName == "Title")
            {
                NotifyPropertyChanged("ResponseTitle");
            }
        }

        private ResponseVM response;
        public ResponseVM Response { 
            get 
            { 
                return response;
            }
            set
            {
                if (response != value)
                {
                    response = value;
                    if (response != null)
                    {                       
                        response.Parent = this;
                        response.ParentId = Id;
                        if (question.Response != response.Response)
                        {
                            question.Response = response.Response;
                        }
                        response.PropertyChanged += ResponsePropertyChanged;
                    }
                    NotifyPropertyChanged("Response");
                    Memorize();
                }
            }
        }

        public string ResponseTypeCode { get { return response.TypeCode; } }
        public string ResponseTypeName { get { return response.TypeName; } }

        public bool IsResponseTypeUnknown { get { return response.IsTypeUnknown; } }
        public bool IsResponseTypeChoices { get { return response.IsTypeChoices; } }
        public bool IsResponseTypeNumber { get { return response.IsTypeNumber; } }
        public bool IsResponseTypeFree { get { return response.IsTypeFree; } }
        public bool IsResponseTypeDateTime { get { return response.IsTypeDateTime; } }

        public string Title
        {
            get
            {
                return question.Title;
            }
            set
            {
                if (question.Title != value)
                {
                    question.Title = value;
                    NotifyPropertyChanged("Title");
                }
            }
        }

        public string Text
        {
            get
            {
                return question.Text;
            }
            set
            {
                if (question.Text != value)
                {
                    question.Text = value;
                    NotifyPropertyChanged("Text");
                }
            }
        }

        public string Content
        {
            get
            {
                StringBuilder buf = new StringBuilder();
                if (Title != null) 
                {
                    buf.Append(Title);
                }
                buf.Append(": ");
                if (Text != null)
                {
                    buf.Append(Text);
                }
                return buf.ToString();
            }
        }

        public string ResponseTitle
        {
            get
            {
                return response.Title;
            }
            set
            {
                response.SetTitle(value);
            }
        }

        public Response DupResponseModel()
        {
            return question.Response.Dup();
        }

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

            bakQuestion = question.Clone() as Question;
            bakResponseTypeCode = response.TypeCode;
        }

        public void CancelEdit()
        {
            if (!inEdit)
            {
                return;
            }
            inEdit = false;

            this.Title = bakQuestion.Title;
            this.Text = bakQuestion.Title;
            response.TypeCode = bakResponseTypeCode;
        }

        public void EndEdit()
        {
            if (!inEdit)
            {
                return;
            }
            inEdit = false;

            bakQuestion = null;
            bakResponseTypeCode = null;
            Memorize();
        }


        public string ToDebugString()
        {
            StringBuilder buf = new StringBuilder();
            buf.AppendFormat("QuestionVM Id={0} Title={1} InEdit={2}", Id, Title, InEdit);
            buf.AppendFormat("[{0}]", response.ToString());
            return buf.ToString();
        }

        protected override void PrepareValidation()
        {
            if (string.IsNullOrEmpty(Title))
            {
                Title = EMPTY_VALUE;
            }
            if (string.IsNullOrEmpty(Text))
            {
                Text = EMPTY_VALUE;
            }
        }

        #endregion
    }
}

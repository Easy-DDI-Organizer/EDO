using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using EDO.Core.Util;
using EDO.Core.Model.Layout;

//QuestionのプロパティとしてResponseを持ち、それを継承したクラスにすると
//シリアル化できないのでとりあえず汎用のファットベースクラスを作る。
namespace EDO.Core.Model
{
    public class Response :ICloneable, IIDPropertiesProvider
    {
        public string[] IdProperties
        {
            get
            {
                //必要ないかもしれないけど念のため実装
                return new string[] { "Id" };
            }
        }

        public Response()
            : this(Options.RESPONSE_TYPE_UNKNOWN)
        {
        }

        public Response(Option type)
        {
            Id = IDUtils.NewGuid();
            TypeCode = type.Code;
            MissingValues = new List<MissingValue>();
            Layout = null;
        }

        public string Id { get; set; }

        public string TypeCode {get; set; }
        public bool IsTypeChoices { get { return Options.RESPONSE_TYPE_CHOICES_CODE == TypeCode; } }
        public bool IsTypeUnknown { get { return Options.RESPONSE_TYPE_UNKNOWN_CODE == TypeCode; } }
        public bool IsTypeNumber { get { return Options.RESPONSE_TYPE_NUMBER_CODE == TypeCode; } }
        public bool IsTypeFree { get { return Options.RESPONSE_TYPE_FREE_CODE == TypeCode; } }
        public bool IsTypeDateTime { get { return Options.RESPONSE_TYPE_DATETIME_CODE == TypeCode; } }

        //タイトル
        public string Title { get; set; }

        // for choices
        public string CodeSchemeId { get; set;  }

        // for DateTime and Number (それぞれの型保管用)
        public string DetailTypeCode { get; set; }
        public string DetailTypeLabel { get; set; }

        // for Free
        public string Text { get; set; }

        // for Free and Number
        public decimal? Max { get; set; }
        public decimal? Min { get; set; }

        public List<MissingValue> MissingValues { get; set; }
        public ResponseLayout Layout { get; set; }

        public Response Dup()
        {
            Response newResponseModel = Clone() as Response;
            //回答自体は新しく複製する
            newResponseModel.Id = IDUtils.NewGuid();
            return newResponseModel;
        }

        #region ICloneable メンバー

        public object Clone()
        {
            Response newResponse =  (Response)this.MemberwiseClone();
            newResponse.MissingValues = new List<MissingValue>();
            foreach (MissingValue mv in MissingValues)
            {
                newResponse.MissingValues.Add((MissingValue)mv.Clone());
            }
            if (Layout != null)
            {
                newResponse.Layout = Layout.Clone() as ResponseLayout;
            }
            return newResponse;
        }

        public string JoinMissingValuesContent()
        {
            return MissingValue.JoinContent(MissingValues, " ");
        }

        #endregion
    }
}

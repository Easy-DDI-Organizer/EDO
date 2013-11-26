using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Properties;

namespace EDO.Core.Model
{
    //メイン画面のメニュー画面とDDIインポート時のメニュー画面で使い回すために定義
    public class MenuElem
    {
        //頭にCがつくのはカテゴリ、Mはメニュー本体
        public static MenuElem C_EVENT;
        public static MenuElem M_EVENT;

        public static MenuElem C_STUDY;
        public static MenuElem M_MEMBER;
        public static MenuElem M_ABSTRACT;
        public static MenuElem M_COVERAGE;
        public static MenuElem M_FUNDING_INFO;

        public static MenuElem C_SAMPLING;
        public static MenuElem M_SAMPLING;

        public static MenuElem C_QUESTION;
        public static MenuElem M_CONCEPT;
        public static MenuElem M_QUESTION;
        public static MenuElem M_CATEGORY;
        public static MenuElem M_CODE;
        public static MenuElem M_SEQUENCE;
        public static MenuElem M_QUESTION_GROUP;

        public static MenuElem C_VARIABLE;
        public static MenuElem M_VARIABLE;

        public static MenuElem C_DATA;
        public static MenuElem M_DATA_SET;
        public static MenuElem M_DATA_FILE;
        public static MenuElem M_BOOKS;

        public static MenuElem C_GROUP;
        public static MenuElem M_DETAIL;
        public static MenuElem M_COMPARE_DAI;
        public static MenuElem M_COMPARE_SHO;
        public static MenuElem M_COMPARE_VARIABLE;

        static MenuElem()
        {
            int id = 1;
            C_EVENT = new MenuElem(id++, Resources.EventManagement, true); //イベント管理
            M_EVENT = new MenuElem(id++, Resources.EventManagement); //イベント管理

            C_STUDY = new MenuElem(id++, Resources.StudyPlan, true); //調査の企画
            M_MEMBER = new MenuElem(id++, Resources.StudyMember); //調査メンバー
            M_ABSTRACT = new MenuElem(id++, Resources.StudyAbstract); //調査の概
            M_COVERAGE = new MenuElem(id++, Resources.StudyRange); //調査の範囲
            M_FUNDING_INFO = new MenuElem(id++, Resources.StudyFund); //研究資金

            C_SAMPLING = new MenuElem(id++, Resources.DataCollectionMethod, true); //データの収集方法
            M_SAMPLING = new MenuElem(id++, Resources.DataCollectionMethod); //データの収集方法

            C_QUESTION = new MenuElem(id++, Resources.QuestionDesign, true); //設問設計
            M_CONCEPT = new MenuElem(id++, Resources.VariableConcept); //扱いたい変数のイメージ
            M_QUESTION = new MenuElem(id++, Resources.QuestionItemDesign); //質問項目の設計
            M_CATEGORY = new MenuElem(id++, Resources.Category); //選択肢
            M_CODE = new MenuElem(id++, Resources.Code); //コード
            M_SEQUENCE = new MenuElem(id++, Resources.Sequence); //順序
            M_QUESTION_GROUP = new MenuElem(id++, Resources.QuestionGroup);

            C_VARIABLE = new MenuElem(id++, Resources.VariableManagement, true); //変数の管理
            M_VARIABLE = new MenuElem(id++, Resources.VariableInfo); //変数情報

            C_DATA = new MenuElem(id++, Resources.DataManagement, true); //データの管理
            M_DATA_SET = new MenuElem(id++, Resources.DataSetDefinition); //データセットの定義
            M_DATA_FILE = new MenuElem(id++, Resources.DataFileDefinition); //データファイルの定義
            M_BOOKS = new MenuElem(id++, Resources.BookList); //データファイルの定義

            C_GROUP = new MenuElem(id++, Resources.GroupManagement, true); //グループ管理
            M_DETAIL = new MenuElem(id++, Resources.DetailItem); //詳細項目
            M_COMPARE_DAI = new MenuElem(id++, Resources.CompareMajorDivision); //大分類で比較
            M_COMPARE_SHO = new MenuElem(id++, Resources.CompareMinorDivision); //小分類で比
            M_COMPARE_VARIABLE = new MenuElem(id++, Resources.CompareVariable); //変数で比較
        }

        public MenuElem(int id, string title)
            : this(id, title, false)
        {
        }

        public MenuElem(int id, string title, bool isCategory)
        {
            this.id = id;
            this.title = title;
            this.isCategory = isCategory;
        }

        private int id;
        public int Id { get { return id; } }

        private string title;
        public string Title { get { return title; } }

        private bool isCategory;
        public bool IsCategory { get { return isCategory; } }
    }
}

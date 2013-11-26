using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core.Model;

namespace EDO.Core.IO
{
    public abstract class DDIImportOption
    {
        public static void AppendUniq(List<MenuElem> menuElems, List<MenuElem> appendMenuElems)
        {
            foreach (MenuElem appendMenuElem in appendMenuElems)
            {
                AppendUniq(menuElems, appendMenuElem);
            }
        }

        public static void AppendUniq(List<MenuElem> menuElems, MenuElem appendMenuElem)
        {
            if (menuElems.Contains(appendMenuElem))
            {
                return;
            }
            menuElems.Add(appendMenuElem);
        }


        public DDIImportOption()
        {
            menuElems = new List<MenuElem>(GetDefaultMenuElems());
            validMenuElems = new List<MenuElem>(menuElems);
        }

        public bool CanSelectFromStudyUnit { get; set; }

        public abstract List<MenuElem> GetDefaultMenuElems();

        private List<MenuElem> menuElems;
        public List<MenuElem> MenuElems { get { return menuElems; } }

        private List<MenuElem> validMenuElems;

        public bool IsValid(MenuElem menuElem)
        {
            return validMenuElems.Contains(menuElem);
        }

        public List<MenuElem> GetRelatedMenuElems(MenuElem menuElem)
        {
            //最初の項目(配列の0番目)がチェックされたとき、残りの項目がチェックされる)
            MenuElem[][] menuMatrix = new MenuElem[][] {
                new MenuElem[] {MenuElem.M_QUESTION,  MenuElem.M_CONCEPT, MenuElem.M_CATEGORY, MenuElem.M_CODE, MenuElem.M_VARIABLE, MenuElem.M_DATA_SET, MenuElem.M_DATA_FILE},
                new MenuElem[] {MenuElem.M_CODE, MenuElem.M_CATEGORY},
                new MenuElem[] {MenuElem.M_SEQUENCE, MenuElem.M_CONCEPT, MenuElem.M_QUESTION, MenuElem.M_CATEGORY, MenuElem.M_CODE, MenuElem.M_VARIABLE, MenuElem.M_DATA_SET, MenuElem.M_DATA_FILE},
                new MenuElem[] {MenuElem.M_VARIABLE, MenuElem.M_CONCEPT, MenuElem.M_QUESTION, MenuElem.M_CATEGORY, MenuElem.M_CODE, MenuElem.M_DATA_SET, MenuElem.M_DATA_FILE},
                new MenuElem[] {MenuElem.M_DATA_SET, MenuElem.M_CONCEPT, MenuElem.M_QUESTION, MenuElem.M_CATEGORY, MenuElem.M_CODE, MenuElem.M_VARIABLE, MenuElem.M_DATA_FILE},
                new MenuElem[] {MenuElem.M_DATA_FILE, MenuElem.M_CONCEPT, MenuElem.M_QUESTION, MenuElem.M_CATEGORY, MenuElem.M_CODE, MenuElem.M_VARIABLE, MenuElem.M_DATA_SET},
            };
            List<MenuElem> relatedMenuElems = new List<MenuElem>();
            foreach (MenuElem[] menuList in menuMatrix)
            {
                MenuElem fromMenu = menuList[0];
                if (fromMenu == menuElem)
                {
                    for (int i = 1; i < menuList.Length; i++)
                    {
                        relatedMenuElems.Add(menuList[i]);
                    }
                    if (relatedMenuElems.Contains(fromMenu))
                    {
                        throw new ApplicationException();
                    }
                    return relatedMenuElems;
                }
            }
            return relatedMenuElems;
        }

        public void UpdateValidMenuElems(List<MenuElem> checkMenuElems)
        {
            validMenuElems.Clear();
            foreach (MenuElem menuElem in checkMenuElems)
            {
                validMenuElems.Add(menuElem);
                List<MenuElem> relatedMenuElems = GetRelatedMenuElems(menuElem);
                //質問をチェックした場合、選択肢、コードなどはまとめてインポートできないといけない。
                //画面上で質問チェック→選択肢、コードのチェックは制御しているが、後から選択肢のチェックを外すことはできる。
                //この場合、選択肢もインポートできるようここで関連するメニューを全部有効扱いとする。
                AppendUniq(validMenuElems, relatedMenuElems);
            }
        }

        public bool ImportEvent { get { return IsValid(MenuElem.M_EVENT); } }
        public bool ImportMember { get { return IsValid(MenuElem.M_MEMBER); } }
        public bool ImportAbstract { get { return IsValid(MenuElem.M_ABSTRACT); } }
        public bool ImportCoverage { get { return IsValid(MenuElem.M_COVERAGE); } }
        public bool ImportFundingInfo { get { return IsValid(MenuElem.M_FUNDING_INFO); } }
        public bool ImportSampling { get { return IsValid(MenuElem.M_SAMPLING); } }
        public bool ImportConcept { get { return IsValid(MenuElem.M_CONCEPT); } }
        public bool ImportQuestion { get { return IsValid(MenuElem.M_QUESTION); } }
        public bool ImportCategory { get { return IsValid(MenuElem.M_CATEGORY); } }
        public bool ImportCode { get { return IsValid(MenuElem.M_CODE); } }
        public bool ImportSequence { get { return IsValid(MenuElem.M_SEQUENCE); } }
        public bool ImportBook { get { return IsValid(MenuElem.M_BOOKS); } }
        public bool ImportQuestionGroup { get { return IsValid(MenuElem.M_QUESTION_GROUP); } }
        public bool ImportVariable { get { return IsValid(MenuElem.M_VARIABLE); } }
        public bool ImportDataSet { get { return IsValid(MenuElem.M_DATA_SET); } }
        public bool ImportDataFile { get { return IsValid(MenuElem.M_DATA_FILE); } }
    }
}

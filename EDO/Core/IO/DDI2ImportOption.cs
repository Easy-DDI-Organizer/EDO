﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core.Model;

namespace EDO.Core.IO
{
    public class DDI2ImportOption: DDIImportOption
    {
        public DDI2ImportOption()
        {
            CanSelectFromStudyUnit = false;
        }

        public override List<MenuElem> GetDefaultMenuElems()
        {
            List<MenuElem> menuElems = new List<MenuElem>();
            menuElems.Add(MenuElem.C_STUDY);
            menuElems.Add(MenuElem.M_MEMBER);
            menuElems.Add(MenuElem.M_ABSTRACT);
            menuElems.Add(MenuElem.M_COVERAGE);
            menuElems.Add(MenuElem.M_FUNDING_INFO);
            menuElems.Add(MenuElem.C_SAMPLING);
            menuElems.Add(MenuElem.M_SAMPLING);
            menuElems.Add(MenuElem.C_QUESTION);
            menuElems.Add(MenuElem.M_CONCEPT);
            menuElems.Add(MenuElem.M_QUESTION);
            menuElems.Add(MenuElem.M_CATEGORY);
            menuElems.Add(MenuElem.M_CODE);
            menuElems.Add(MenuElem.C_VARIABLE);
            menuElems.Add(MenuElem.M_VARIABLE);
            menuElems.Add(MenuElem.C_DATA);
            menuElems.Add(MenuElem.M_DATA_SET);
            menuElems.Add(MenuElem.M_DATA_FILE);
            menuElems.Add(MenuElem.M_BOOKS);
            return menuElems;
        }


    }
}

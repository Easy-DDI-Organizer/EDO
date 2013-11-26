using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EDO.Core.Model
{

    public enum QuestionGroupOrientation
    {
        Row,
        Column
    };

    public enum QuestionGroupSentence
    {
        Top,
        EachCell
    };

    public enum NumericLayoutMeasurementMethod
    {
        Number,
        VisualAnalogScale
    };

    public enum ChoicesLayoutMesurementMethod
    {
        Single,
        Multiple,
        SD
    };

    public enum ChoicesLayoutDirection
    {
        Vertical,
        Horizontal
    };

    public enum DateTimeLayoutCalendarEra
    {
        Japanese,
        Western,
        None
    };

    public enum LayoutStyle
    {
        Underline,
        Textbox,
        Box
    };

    public enum BookType
    {
        Book,
        BookChapter,
        TreatiseWithPeerReview,
        TreatiseWithoutPeerReview,
        SocietyAbstract,
        Report,
        Thesis,
        Wegpage,
        Other
    };

    public enum BookRelationType
    {
        Abstract,
        Concept,
        Question,
        Variable
    };
}


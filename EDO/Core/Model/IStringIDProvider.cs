using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EDO.Core.Model
{
    //Formの状態を戻すときにIDを保存するためVMに実装するインターフェイス
    //ReloadのなかのEDOUtils.Findで使用。
    public interface IStringIDProvider
    {
        string Id { get; }
    }
}

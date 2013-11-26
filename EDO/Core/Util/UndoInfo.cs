using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core.Model;
using EDO.Core.View;
using EDO.Core.IO;

namespace EDO.Core.Util
{
    public class UndoInfo
    {
        private EDOModel edoModel;
        private VMState state;

        public UndoInfo(EDOModel edoModel, VMState state)
        {
            this.edoModel = edoModel;
            this.state = state;
        }

        public EDOModel Model { get { return edoModel; } }
        public VMState State { get { return state; } }

        public UndoInfo Copy()
        {
            return new UndoInfo(EDOSerializer.Clone(edoModel), (VMState)state.Clone());
        }
    }
}

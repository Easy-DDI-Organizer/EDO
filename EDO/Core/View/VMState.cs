using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EDO.Core.View
{
    public class VMState :ICloneable
    {
//        public int EdoUnitId { get; set; }
//        public int MenuItemId { get; set; }
        //public void CopyFrom(VMState other)
        //{
        //    State1 = other.State1;
        //    State2 = other.State2;
        //    State3 = other.State3;
        //}

        //public void Clear()
        //{
        //    State1 = null;
        //    State2 = null;
        //    State3 = null;
        //}

        public VMState()
            : this(null)
        {
        }

        public VMState(object state1)
            : this(state1, null)
        {
        }

        public VMState(object state1, object state2) :this(state1, state2, null)
        {
        }

        public VMState(object state1, object state2, object state3)
        {
            State1 = state1;
            State2 = state2;
            State3 = state3;
        }

        public object State1 { get; set; }
        public object State2 { get; set; }
        public object State3 { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}

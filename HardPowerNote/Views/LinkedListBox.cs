using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CJT;

namespace HardPowerNote {
    class LinkedListBox : ListBox {

        //MAYBE dont even need this. MAYBE,
        //just need to set its datatemplate to LinkedTextBlock.
        //BUT then how does LTB get instantiated,
        //so that its dataContext is set to that of item, NOT the parentEntryVM???
        //YES tricky... SO maybe have to override a method here,
        //that MAKES the LTBlocks, and in it, it SETS the DataContext,
        //TO the a EntryVM that WRAPS property in the actual LIST!

    }
}

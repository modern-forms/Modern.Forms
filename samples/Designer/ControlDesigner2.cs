using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modern.Forms;

namespace Designer
{
    public class ControlDesigner2 : ComponentDesigner
    {
        public Control Control => (Control)Component;
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Linq;
using System.Text;
using Designer;
using Modern.Forms;
using SkiaSharp;

namespace Modern.Forms.Design
{
    public class FormDocumentDesigner : DocumentDesigner
    {
        //public Form Form { get; private set; }


        //IComponent IDesigner.Component => Form;

        //DesignerVerbCollection? IDesigner.Verbs => null;

        //void IDesigner.DoDefaultAction ()
        //{
        //    throw new NotImplementedException ();
        //}

        //void IDesigner.Initialize (IComponent component)
        //{
        //    base.Initialize (component);

        //    Form = (Form)component;

        //    //foreach (var control in form.Controls.ToArray ()) {
        //    //    if (control is not ControlDesigner) {
        //    //        form.Controls.Remove (control);
        //    //        var new_control = new ControlDesigner (control);
        //    //        //new_control.Top -= form.TitleBar.Height;
        //    //        Controls.Add (new_control);
        //    //    } else {
        //    //        form.Controls.Remove (control);
        //    //        Controls.Add (control);
        //    //    }
        //    //}
        //}
    }
}

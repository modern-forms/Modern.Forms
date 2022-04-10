using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modern.Forms.Layout;

namespace Modern.Forms
{
    public class ContainerControl : Control, IContainerControl
    {
        private Control _activeControl;

        /// <summary>
        ///  Activates the specified control.
        /// </summary>
        bool IContainerControl.ActivateControl (Control control)
        {
            return ActivateControl (control, originator: true);
        }

        internal bool ActivateControl (Control control)
        {
            return ActivateControl (control, originator: true);
        }

        internal bool ActivateControl (Control control, bool originator)
        {
            //Debug.WriteLineIf (s_focusTracing.TraceVerbose, "ContainerControl::ActivateControl(" + (control is null ? "null" : control.Name) + "," + originator.ToString () + ") - " + Name);

            // Recursive function that makes sure that the chain of active controls is coherent.
            bool ret = true;
            bool updateContainerActiveControl = false;
            ContainerControl cc = null;
            Control parent = Parent;
            if (parent is not null) {
                cc = (parent.GetContainerControl ()) as ContainerControl;
                if (cc is not null) {
                    updateContainerActiveControl = (cc.ActiveControl != this);
                }
            }

            if (control != _activeControl || updateContainerActiveControl) {
                if (updateContainerActiveControl) {
                    if (!cc.ActivateControl (this, false)) {
                        return false;
                    }
                }

                //ret = AssignActiveControlInternal ((control == this) ? null : control);
            }

            if (originator) {
               // ScrollActiveControlIntoView ();
            }

            return ret;
        }

        /// <summary>
        ///  Indicates the current active control on the container control.
        /// </summary>
        //[SRCategory (nameof (SR.CatBehavior))]
        [Browsable (false)]
        [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
        //[SRDescription (nameof (SR.ContainerControlActiveControlDescr))]
        public Control ActiveControl {
            get => _activeControl;
            set => SetActiveControl (value);
        }

        /// <summary>
        ///  Cleans up form state after a control has been removed.
        /// </summary>
        internal virtual void AfterControlRemoved (Control control, Control oldParent)
        {
            //ContainerControl cc;
            //Debug.Assert (control is not null);
            //Debug.WriteLineIf (s_focusTracing.TraceVerbose, "ContainerControl::AfterControlRemoved(" + control.Name + ") - " + Name);
            //if (control == _activeControl || control.Contains (_activeControl)) {
            //    bool selected = SelectNextControl (control, true, true, true, true);
            //    if (selected && _activeControl != control) {
            //        // Add the check. If it is set to true, do not call into FocusActiveControlInternal().
            //        // The TOP MDI window could be gone and CreateHandle method will fail
            //        // because it try to create a parking window Parent for the MDI children
            //        if (!_activeControl.Parent.IsTopMdiWindowClosing) {
            //            FocusActiveControlInternal ();
            //        }
            //    } else {
            //        SetActiveControl (null);
            //    }
            //} else if (_activeControl is null && ParentInternal is not null) {
            //    // The last control of an active container was removed. Focus needs to be given to the next
            //    // control in the Form.
            //    cc = ParentInternal.GetContainerControl () as ContainerControl;
            //    if (cc is not null && cc.ActiveControl == this) {
            //        Form f = FindForm ();
            //        if (f is not null) {
            //            f.SelectNextControl (this, true, true, true, true);
            //        }
            //    }
            //}

            //// Two controls in UserControls that don't take focus via UI can have bad behavior if ...
            //// When a control is removed from a container, not only do we need to clear the unvalidatedControl of that
            //// container potentially, but the unvalidatedControl of all its container parents, up the chain, needs to
            //// now point to the old parent of the disappearing control.
            //cc = this;
            //while (cc is not null) {
            //    Control parent = cc.ParentInternal;
            //    if (parent is null) {
            //        break;
            //    } else {
            //        cc = parent.GetContainerControl () as ContainerControl;
            //    }

            //    if (cc is not null &&
            //        cc._unvalidatedControl is not null &&
            //        (cc._unvalidatedControl == control || control.Contains (cc._unvalidatedControl))) {
            //        cc._unvalidatedControl = oldParent;
            //    }
            //}

            //if (control == _unvalidatedControl || control.Contains (_unvalidatedControl)) {
            //    _unvalidatedControl = null;
            //}
        }

        /// <summary>
        ///  Implements ActiveControl property setter.
        /// </summary>
        internal void SetActiveControl (Control value)
        {
            //Debug.WriteLineIf (s_focusTracing.TraceVerbose, $"ContainerControl::SetActiveControl({(value is null ? "null" : value.Name)}) - {Name}");

            if (_activeControl == value && (value is null || value.Focused)) {
                return;
            }

            if (value is not null && !Contains (value)) {
                throw new ArgumentException (SR.CannotActivateControl, nameof (value));
            }

            bool result;
            ContainerControl containerControl = this;

            if (value is not null) {
                containerControl = value.Parent.GetContainerControl () as ContainerControl;
            }

            //if (containerControl is not null) {
            //    // Call to the recursive function that corrects the chain of active controls
            //    result = containerControl.ActivateControl (value, false);
            //} else {
            //    result = AssignActiveControlInternal (value);
            //}

            //if (containerControl is not null && result) {
            //    ContainerControl ancestor = this;
            //    while (ancestor.ParentInternal?.GetContainerControl () is ContainerControl parentContainer) {
            //        ancestor = parentContainer;
            //    }

            //    if (ancestor.ContainsFocus
            //        && (value is null || value is not UserControl userControl || !userControl.HasFocusableChild ())) {
            //        containerControl.FocusActiveControlInternal ();
            //    }
            //}
        }
    }
}

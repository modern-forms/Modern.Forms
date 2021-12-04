using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modern.Forms.Design
{
    internal sealed class SelectionService : ISelectionService, IDisposable
    {
        // Member variables
        private IServiceProvider _provider; // The service provider
        private BitVector32 _state; // state of the selection service
        private readonly EventHandlerList _events; // the events we raise
        private ArrayList _selection; // list of selected objects
        private string[] _contextAttributes; // help context information we have pushed to the help service.
        private short _contextKeyword; // the offset into the selection keywords for the current selection.
        //private StatusCommandUI _statusCommandUI; // UI for setting the StatusBar Information..

        // ISelectionService events
        private static readonly object s_eventSelectionChanging = new object ();
        private static readonly object s_eventSelectionChanged = new object ();

        /// <summary>
        ///  Creates a new selection manager object.  The selection manager manages all selection of all designers under the current form file.
        /// </summary>
        internal SelectionService (IServiceProvider provider) : base ()
        {
            _provider = provider;
            _state = new BitVector32 ();
            _events = new EventHandlerList ();
            //_statusCommandUI = new StatusCommandUI (provider);
        }

        /// <summary>
        ///  Adds the given selection to our selection list.
        /// </summary>
        internal void AddSelection (object sel)
        {
            if (_selection is null) {
                _selection = new ArrayList ();
                // Now is the opportune time to hook up all of our events
                //if (GetService (typeof (IComponentChangeService)) is IComponentChangeService cs) {
                //    cs.ComponentRemoved += new ComponentEventHandler (OnComponentRemove);
                //}

                //if (GetService (typeof (IDesignerHost)) is IDesignerHost host) {
                //    host.TransactionOpened += new EventHandler (OnTransactionOpened);
                //    host.TransactionClosed += new DesignerTransactionCloseEventHandler (OnTransactionClosed);
                //    if (host.InTransaction) {
                //        OnTransactionOpened (host, EventArgs.Empty);
                //    }
                //}
            }

            if (!_selection.Contains (sel)) {
                _selection.Add (sel);
            }
        }

        /// <summary>
        ///  Helper function to retrieve services.
        /// </summary>
        private object GetService (Type serviceType)
        {
            if (_provider is not null) {
                return _provider.GetService (serviceType);
            }

            return null;
        }

        /// <summary>
        ///  Retrieves the object that is currently the primary selection.  The primary selection has a slightly different UI look and is used as a "key" when an operation is to be done on multiple components.
        /// </summary>
        object ISelectionService.PrimarySelection => _selection is not null && _selection.Count > 0 ? _selection[0] : null;

        /// <summary>
        ///  Retrieves the count of selected objects.
        /// </summary>
        int ISelectionService.SelectionCount => _selection is not null ? _selection.Count : 0;

        /// <summary>
        ///  Adds a <see cref='ISelectionService.SelectionChanged'/> event handler to the selection service.
        /// </summary>
        event EventHandler ISelectionService.SelectionChanged {
            add => _events.AddHandler (s_eventSelectionChanged, value);
            remove => _events.RemoveHandler (s_eventSelectionChanged, value);
        }

        /// <summary>
        ///  Occurs whenever the user changes the current list of selected components in the designer.  This event is raised before the actual selection changes.
        /// </summary>
        event EventHandler ISelectionService.SelectionChanging {
            add => _events.AddHandler (s_eventSelectionChanging, value);
            remove => _events.RemoveHandler (s_eventSelectionChanging, value);
        }

        /// <summary>
        ///  Determines if the component is currently selected.  This is faster than getting the entire list of selected components.
        /// </summary>
        bool ISelectionService.GetComponentSelected (object component)
        {
            ArgumentNullException.ThrowIfNull (component);

            return (_selection is not null && _selection.Contains (component));
        }

        /// <summary>
        ///  Retrieves an array of components that are currently part of the user's selection.
        /// </summary>
        ICollection ISelectionService.GetSelectedComponents ()
        {
            if (_selection is not null) {
                // Must clone here.  Otherwise the values collection is a live collection and will change when the selection changes.  GetSelectedComponents should be a snapshot.
                object[] selectedValues = new object[_selection.Count];
                _selection.CopyTo (selectedValues, 0);
                return selectedValues;
            }

            return Array.Empty<object> ();
        }

        /// <summary>
        ///  Changes the user's current set of selected components to the components in the given array.  If the array is null or doesn't contain any components, this will select the top level component in the designer.
        /// </summary>
        void ISelectionService.SetSelectedComponents (ICollection components)
        {
            ((ISelectionService)this).SetSelectedComponents (components, SelectionTypes.Auto);
        }

        /// <summary>
        ///  Changes the user's current set of selected components to the components in the given array.  If the array is null or doesn't contain any components, this will select the top level component in the designer.
        /// </summary>
        void ISelectionService.SetSelectedComponents (ICollection components, SelectionTypes selectionType)
        {
            bool fToggle = (selectionType & SelectionTypes.Toggle) == SelectionTypes.Toggle;
            bool fPrimary = (selectionType & SelectionTypes.Primary) == SelectionTypes.Primary;
            bool fAdd = (selectionType & SelectionTypes.Add) == SelectionTypes.Add;
            bool fRemove = (selectionType & SelectionTypes.Remove) == SelectionTypes.Remove;
            bool fReplace = (selectionType & SelectionTypes.Replace) == SelectionTypes.Replace;
            bool fAuto = !(fToggle | fAdd | fRemove | fReplace);

            // We always want to allow NULL arrays coming in.
            if (components is null) {
                components = Array.Empty<object> ();
            }

            // If toggle, replace, remove or add are not specifically specified, infer them from  the state of the modifier keys.  This creates the "Auto" selection type for us by default.
            //if (fAuto) {
            //    fToggle = (Control.ModifierKeys & (Keys.Control | Keys.Shift)) > 0;
            //    fAdd |= Control.ModifierKeys == Keys.Shift;
            //    // If we are in auto mode, and if we are toggling or adding new controls, then cancel out the primary flag.
            //    if (fToggle || fAdd) {
            //        fPrimary = false;
            //    }
            //}

            // This flag is true if we changed selection and should therefore raise a selection change event.
            bool fChanged = false;
            // Handle the click case
            object requestedPrimary = null;
            int primaryIndex;

            if (fPrimary && 1 == components.Count) {
                foreach (object o in components) {
                    requestedPrimary = o;
                    ArgumentNullException.ThrowIfNull (o, nameof (components));

                    break;
                }
            }

            if (requestedPrimary is not null && _selection is not null && (primaryIndex = _selection.IndexOf (requestedPrimary)) != -1) {
                if (primaryIndex != 0) {
                    object tmp = _selection[0];
                    _selection[0] = _selection[primaryIndex];
                    _selection[primaryIndex] = tmp;
                    fChanged = true;
                }
            } else {
                // If we are replacing the selection, only remove the ones that are not in our new list. We also handle the special case here of having a singular component selected that's already selected.  In this case we just move it to the primary selection.
                if (!fToggle && !fAdd && !fRemove) {
                    if (_selection is not null) {
                        object[] selections = new object[_selection.Count];
                        _selection.CopyTo (selections, 0);
                        // Yucky and N^2, but even with several hundred components this should be fairly fast
                        foreach (object item in selections) {
                            bool remove = true;
                            foreach (object comp in components) {
                                ArgumentNullException.ThrowIfNull (comp, nameof (components));

                                if (ReferenceEquals (comp, item)) {
                                    remove = false;
                                    break;
                                }
                            }

                            if (remove) {
                                RemoveSelection (item);
                                fChanged = true;
                            }
                        }
                    }
                }

                // Now select / toggle the components.
                foreach (object comp in components) {
                    ArgumentNullException.ThrowIfNull (comp, nameof (components));

                    if (_selection is not null && _selection.Contains (comp)) {
                        if (fToggle || fRemove) {
                            RemoveSelection (comp);
                            fChanged = true;
                        }
                    } else if (!fRemove) {
                        AddSelection (comp);
                        fChanged = true;
                    }
                }
            }

            // Notify that our selection has changed
            if (fChanged) {
                //Set the SelectionInformation
                //if (_selection.Count > 0) {
                //    _statusCommandUI.SetStatusInformation (_selection[0] as Component);
                //} else {
                //    _statusCommandUI.SetStatusInformation (Rectangle.Empty);
                //}

                OnSelectionChanged ();
            }
        }

        /// <summary>
        ///  called anytime the selection has changed.  We update our UI for the selection, and then we fire a juicy change event.
        /// </summary>
        private void OnSelectionChanged ()
        {
            //if (_state[s_stateTransaction]) {
            //    _state[s_stateTransactionChange] = true;
            //} else {
                EventHandler eh = _events[s_eventSelectionChanging] as EventHandler;
                eh?.Invoke (this, EventArgs.Empty);

                //UpdateHelpKeyword (true);

                eh = _events[s_eventSelectionChanged] as EventHandler;
                if (eh is not null) {
                    try {
                        eh (this, EventArgs.Empty);
                    } catch {
                        // eat exceptions - required for compatibility with Everett.
                    }
                }
            //}
        }

        /// <summary>
        ///  Removes the given selection from the selection list.
        /// </summary>
        internal void RemoveSelection (object sel)
        {
            if (_selection is not null) {
                _selection.Remove (sel);
            }
        }

        public void Dispose ()
        {
            throw new NotImplementedException ();
        }
    }
}

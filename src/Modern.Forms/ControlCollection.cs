// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Modern.Forms.Layout;

namespace Modern.Forms;

public partial class Control
{
    /// <summary>
    ///  Represents a collection of Controls.
    /// </summary>
    [ListBindable (false)]
    public class ControlCollection : IList<Control>
    {
        // Implicit controls are built-in controls that the user did not add
        // and should not see. Things like implicit scrollbars.
        private readonly List<Control> control_list = new List<Control> ();
        private readonly List<Control> implicit_control_list = new List<Control> ();

        ///  A caching mechanism for key accessor
        ///  We use an index here rather than control so that we don't have lifetime
        ///  issues by holding on to extra references.
        ///  Note this is not Thread Safe - but WinForms has to be run in a STA anyways.
        private int _lastAccessedIndex = -1;

        /// <summary>
        /// Initializes a new instance of the ControlCollection class.
        /// </summary>
        public ControlCollection (Control owner)
        {
            Owner = owner.OrThrowIfNull ();
        }

        /// <summary>
        ///  Adds a child control to this control. The control becomes the last control in
        ///  the child control list. If the control is already a child of another control it
        ///  is first removed from that control.
        /// </summary>
        public virtual T Add<T> (T value) where T : Control
        {
            Insert (control_list.Count, value);
            return value;
        }

        void ICollection<Control>.Add (Control item)
        {
            Add<Control> (item);
        }

        internal T AddImplicitControl<T> (T item) where T : Control
        {
            // We do a lot less here for an implicit control because we control them
            // - They are added in constructors so we don't need to do layouts yet
            // - They won't already be parented to other controls
            item.ImplicitControl = true;
            implicit_control_list.Add (item);
            item.SetParentInternal (Owner);

            return item;
        }

        /// <summary>
        /// Adds multiple child controls to this control. This suspends layouts until all
        /// controls are adding, which is more efficient than adding controls individually.
        /// </summary>
        public virtual void AddRange (params Control[] controls)
        {
            ArgumentNullException.ThrowIfNull (controls);

            if (controls.Length > 0) {
                Owner.SuspendLayout ();

                try {
                    for (var i = 0; i < controls.Length; ++i)
                        Add (controls[i]);
                } finally {
                    Owner.ResumeLayout (true);
                }
            }
        }

        /// <summary>
        /// Removes all controls from the collection.
        /// </summary>
        public virtual void Clear ()
        {
            Owner.SuspendLayout ();
            // clear all preferred size caches in the tree -
            // inherited fonts could go away, etc.
            CommonProperties.xClearAllPreferredSizeCaches (Owner);

            try {
                while (Count > 0)
                    RemoveAt (Count - 1);
            } finally {
                Owner.ResumeLayout ();
            }
        }

        /// <summary>
        /// Determines if the collection contains the specified control.
        /// </summary>
        public bool Contains (Control item) => control_list.Contains (item);

        /// <summary>
        ///  Returns true if the collection contains an item with the specified key, false otherwise.
        /// </summary>
        public virtual bool ContainsKey (string key)
        {
            return IsValidIndex (IndexOfKey (key));
        }

        private static void Copy (ControlCollection sourceList, int sourceIndex, ControlCollection destinationList, int destinationIndex, int length)
        {
            if (sourceIndex < destinationIndex) {
                // We need to copy from the back forward to prevent overwrite if source and
                // destination lists are the same, so we need to flip the source/dest indices
                // to point at the end of the spans to be copied.
                sourceIndex += length;
                destinationIndex += length;

                for (; length > 0; length--)
                    destinationList[--destinationIndex] = sourceList[--sourceIndex];
            } else {
                for (; length > 0; length--)
                    destinationList[destinationIndex++] = sourceList[sourceIndex++];
            }
        }

        /// <summary>
        /// Copies the collection of controls to the specified array.
        /// </summary>
        public void CopyTo (Control[] array, int arrayIndex) => control_list.CopyTo (array, arrayIndex);

        /// <summary>
        /// Return the number of controls in the collection.
        /// </summary>
        public int Count => control_list.Count;

        /// <summary>
        ///  Searches for Controls by their Name property, builds up an array
        ///  of all the controls that match.
        /// </summary>
        public Control[] Find (string key, bool searchAllChildren)
        {
            key.ThrowIfNullOrEmptyWithMessage (SR.FindKeyMayNotBeEmptyOrNull);

            List<Control> foundControls = new ();
            FindInternal (key, searchAllChildren, this, foundControls);
            return foundControls.ToArray ();
        }

        /// <summary>
        ///  Searches for Controls by their Name property, builds up a list
        ///  of all the controls that match.
        /// </summary>
        private void FindInternal (string key, bool searchAllChildren, ControlCollection controlsToLookIn, List<Control> foundControls)
        {
            try {
                // Perform breadth first search - as it's likely people will want controls belonging
                // to the same parent close to each other.
                for (var i = 0; i < controlsToLookIn.Count; i++) {
                    if (controlsToLookIn[i] is null)
                        continue;

                    if (WindowsFormsUtils.SafeCompareStrings (controlsToLookIn[i].Name, key, ignoreCase: true))
                        foundControls.Add (controlsToLookIn[i]);
                }

                // Optional recursive search for controls in child collections.
                if (searchAllChildren) {
                    for (var i = 0; i < controlsToLookIn.Count; i++) {
                        if (controlsToLookIn[i] is null)
                            continue;

                        if (controlsToLookIn[i].Controls.Count > 0)
                            // If it has a valid child collection, append those results to our collection.
                            FindInternal (key, true, controlsToLookIn[i].Controls, foundControls);
                    }
                }
            } catch (Exception e) when (!WindowsFormsUtils.IsCriticalException (e)) {
            }
        }

        internal IEnumerable<Control> GetAllControls (bool includeImplicit = true)
        {
            if (includeImplicit)
                return control_list.Concat (implicit_control_list);

            return control_list;
        }

        /// <summary>
        ///  Retrieves the index of the specified
        ///  child control in this array.  An ArgumentException
        ///  is thrown if child is not parented to this
        ///  Control.
        /// </summary>
        public virtual int GetChildIndex (Control child, bool throwException = true)
        {
            var index = IndexOf (child);

            if (index == -1 && throwException)
                throw new ArgumentException (SR.ControlNotChild);

            return index;
        }

        /// <summary>
        /// Returns an enumerator for the collection.
        /// </summary>
        public IEnumerator<Control> GetEnumerator () => control_list.GetEnumerator ();

        IEnumerator IEnumerable.GetEnumerator () => control_list.GetEnumerator ();

        /// <summary>
        ///  The zero-based index of the first occurrence of value within the entire CollectionBase, if found; otherwise, -1.
        /// </summary>
        public virtual int IndexOfKey (string key)
        {
            // Step 0 - Arg validation
            if (string.IsNullOrEmpty (key))
                return -1; // we don't support empty or null keys.

            // step 1 - check the last cached item
            if (IsValidIndex (_lastAccessedIndex))
                if (WindowsFormsUtils.SafeCompareStrings (this[_lastAccessedIndex].Name, key, /* ignoreCase = */ true))
                    return _lastAccessedIndex;

            // step 2 - search for the item
            for (var i = 0; i < Count; i++)
                if (WindowsFormsUtils.SafeCompareStrings (this[i].Name, key, /* ignoreCase = */ true)) {
                    _lastAccessedIndex = i;
                    return i;
                }

            // step 3 - we didn't find it.  Invalidate the last accessed index and return -1.
            _lastAccessedIndex = -1;
            return -1;
        }

        /// <summary>
        /// Returns the index in the collection of the specified control.
        /// </summary>
        public int IndexOf (Control item) => control_list.IndexOf (item);

        /// <summary>
        /// Adds the specified control at the specified index in the collection.
        /// </summary>
        public virtual void Insert (int index, Control value)
        {
            if (value is null)
                return;

            CheckParentingCycle (Owner, value);

            if (value.parent == Owner) {
                value.SendToBack ();
                return;
            }

            // Remove the new control from its old parent (if any)
            value.parent?.Controls.Remove (value);

            // Find the next highest tab index
            if (value.tab_index == -1)
                value.tab_index = Count == 0 ? 0 : control_list.Max (c => c.TabIndex) + 1;

            // Add the control
            control_list.Insert (index, value);

            // if we don't suspend layout, AssignParent will indirectly trigger a layout event
            // before we're ready (AssignParent will fire a PropertyChangedEvent("Visible"), which calls PerformLayout)
            Owner.SuspendLayout ();

            try {
                var oldParent = value.parent;

                try {
                    // AssignParent calls into user code - this could throw, which
                    // would make us short-circuit the rest of the reparenting logic.
                    // you could end up with a control half reparented.
                    value.AssignParent (Owner);
                } finally {
                    if (value.Visible) {
                        value.CreateControl ();
                        value.OnVisibleChanged (EventArgs.Empty);
                    }
                }

                value.InitLayout ();
            } finally {
                Owner.ResumeLayout (false);
            }

            // Not putting in the finally block, as it would eat the original
            // exception thrown from AssignParent if the following throws an exception.
            LayoutTransaction.DoLayout (Owner, value, PropertyNames.Parent);
            Owner.OnControlAdded (new EventArgs<Control> (value));

            return;
        }

        /// <summary>
        /// Returns a value indicating if controls can be added to the collection.
        /// </summary>
        public bool IsReadOnly => false;

        /// <summary>
        ///  Determines if the index is valid for the collection.
        /// </summary>
        private bool IsValidIndex (int index)
        {
            return ((index >= 0) && (index < Count));
        }

        /// <summary>
        ///  Repositions a element in this list.
        /// </summary>
        private protected void MoveElement (Control element, int fromIndex, int toIndex)
        {
            var delta = toIndex - fromIndex;

            switch (delta) {
                case -1:
                case 1:
                    // Simple swap
                    control_list[fromIndex] = control_list[toIndex];
                    break;

                default:
                    int start;
                    int dest;

                    // Which direction are we moving?
                    if (delta > 0) {
                        // Shift down by the delta to open the new spot
                        start = fromIndex + 1;
                        dest = fromIndex;
                    } else {
                        // Shift up by the delta to open the new spot
                        start = toIndex;
                        dest = toIndex + 1;

                        // Make it positive
                        delta = -delta;
                    }

                    Copy (this, start, this, dest, delta);
                    break;
            }

            control_list[toIndex] = element;
        }

        /// <summary>
        ///  Who owns this control collection.
        /// </summary>
        public Control Owner { get; }

        /// <summary>
        ///  Removes control from this control. Inheriting controls should call
        ///  base.remove to ensure that the control is removed.
        /// </summary>
        public virtual bool Remove (Control value)
        {
            // Sanity check parameter
            if (value is null)
                return false;     // Don't do anything

            if (value.Parent == Owner) {

                // Remove the control from the internal control array
                control_list.Remove (value);
                value.AssignParent (null);

                LayoutTransaction.DoLayout (Owner, value, PropertyNames.Parent);
                Owner.OnControlRemoved (new EventArgs<Control> (value));

                // ContainerControl needs to see it needs to find a new ActiveControl. TODO
                //if (Owner.GetContainerControl () is ContainerControl cc)
                //    cc.AfterControlRemoved (value, Owner);
            }

            return true;
        }

        /// <summary>
        /// Removes the control at the specified collection index.
        /// </summary>
        public void RemoveAt (int index)
        {
            Remove (this[index]);
        }

        /// <summary>
        ///  Removes the child control with the specified key.
        /// </summary>
        public virtual void RemoveByKey (string key)
        {
            var index = IndexOfKey (key);

            if (IsValidIndex (index))
                RemoveAt (index);
        }

        /// <summary>
        ///  Sets the index of the specified
        ///  child control in this array.  An ArgumentException
        ///  is thrown if child is not parented to this
        ///  Control.
        /// </summary>
        public virtual void SetChildIndex (Control child, int newIndex) => SetChildIndexInternal (child, newIndex);

        /// <summary>
        ///  This is internal virtual method so that "Readonly Collections" can override this and throw as they should not allow changing
        ///  the child control indices.
        /// </summary>
        internal virtual void SetChildIndexInternal (Control child, int newIndex)
        {
            // Sanity check parameters
            ArgumentNullException.ThrowIfNull (child);

            var currentIndex = GetChildIndex (child);

            if (currentIndex == newIndex)
                return;

            if (newIndex >= Count || newIndex == -1)
                newIndex = Count - 1;

            MoveElement (child, currentIndex, newIndex);

            LayoutTransaction.DoLayout (Owner, child, PropertyNames.ChildIndex);
        }

        /// <summary>
        ///  Retrieves the child control with the specified index.
        /// </summary>
        public virtual Control this[int index] {
            get {
                //do some bounds checking here...
                if (index < 0 || index >= Count) {
                    throw new ArgumentOutOfRangeException (
                        nameof (index),
                        string.Format (SR.IndexOutOfRange, index.ToString (CultureInfo.CurrentCulture)));
                }

                var control = control_list[index];
                Debug.Assert (control is not null, "Why are we returning null controls from a valid index?");
                return control;
            }
            set => control_list[index] = value;
        }

        /// <summary>
        ///  Retrieves the child control with the specified key.
        /// </summary>
        public virtual Control? this[string key] {
            get {
                // We do not support null and empty string as valid keys.
                if (string.IsNullOrEmpty (key))
                    return null;

                // Search for the key in our collection
                var index = IndexOfKey (key);

                return IsValidIndex (index) ? this[index] : null;
            }
        }
    }
}

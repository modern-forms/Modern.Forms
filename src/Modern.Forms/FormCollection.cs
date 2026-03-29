// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;

namespace Modern.Forms;

/// <summary>
///  This is a read only collection of Forms exposed as a static property of the
///  Application class. This is used to store all the currently loaded forms in an app.
/// </summary>
public class FormCollection : IReadOnlyCollection<Form>
{
    private readonly List<Form> inner_list = [];
    internal static object CollectionSyncRoot = new object ();

    /// <summary>
    ///  Gets a form specified by name, if present, else returns null. If there are multiple
    ///  forms with matching names, the first form found is returned.
    /// </summary>
    public virtual Form? this[string? name] {
        get {
            if (name is not null) {
                lock (CollectionSyncRoot) {
                    foreach (var form in inner_list) {
                        if (string.Equals (form.Name, name, StringComparison.OrdinalIgnoreCase))
                            return form;
                    }
                }
            }

            return null;
        }
    }

    /// <summary>
    ///  Gets a form specified by index.
    /// </summary>
    public virtual Form? this[int index] {
        get {
            Form? f = null;

            lock (CollectionSyncRoot)
                f = inner_list[index];

            return f;
        }
    }

    /// <summary>
    /// Gets the number of elements contained in the collection.
    /// </summary>
    public int Count {
        get {
            lock (CollectionSyncRoot)
                return inner_list.Count;
        }
    }

    /// <summary>
    ///  Used internally to add a Form to the FormCollection
    /// </summary>
    internal void Add (Form form)
    {
        lock (CollectionSyncRoot) {
            if (!inner_list.Contains (form))
                inner_list.Add (form);
        }
    }

    /// <summary>
    ///  Used internally to check if a Form is in the FormCollection
    /// </summary>
    internal bool Contains (Form form)
    {
        var inCollection = false;

        lock (CollectionSyncRoot)
            inCollection = inner_list.Contains (form);

        return inCollection;
    }

    /// <summary>
    /// Returns an enumerator that iterates through the collection of forms.
    /// </summary>
    public IEnumerator<Form> GetEnumerator ()
    {
        Form[] snapshot;

        lock (CollectionSyncRoot)
            snapshot = inner_list.ToArray ();

        return ((IEnumerable<Form>)snapshot).GetEnumerator ();
    }

    /// <summary>
    ///  Used internally to add a Form to the FormCollection
    /// </summary>
    internal void Remove (Form form)
    {
        lock (CollectionSyncRoot)
            inner_list.Remove (form);
    }

    IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
}

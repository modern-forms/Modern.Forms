using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Modern.Forms.Design
{
    public class DesignerHost : Container, IDesignerLoaderHost2
    {
        private readonly DesignSurface _surface; // the owning designer surface.
        private readonly EventHandlerList _events; // event list
        private readonly Hashtable _designers;  // designer -> component mapping
        private BitVector32 _state; // state for this host

        private string _newComponentName; // transient value indicating the name of a component that is being created
        private IComponent _rootComponent; // the root of our design
        private string _rootComponentClassName; // class name of the root of our design
        private DesignerLoader _loader; // the loader that loads our designers
        private IDesignerEventService _designerEventService;
        private bool _ignoreErrorsDuringReload;
        private bool _canReloadWithErrors;

        private static readonly Type[] s_defaultServices = new Type[] { typeof (IDesignerHost), typeof (IContainer), typeof (IDesignerLoaderHost2) /*, typeof (IComponentChangeService)*/ };

        // IDesignerHost events
        private static readonly object s_eventActivated = new object (); // Designer has been activated
        private static readonly object s_eventDeactivated = new object (); // Designer has been deactivated
        private static readonly object s_eventLoadComplete = new object (); // Loading has been completed
        private static readonly object s_eventTransactionClosed = new object (); // The last transaction has been closed
        private static readonly object s_eventTransactionClosing = new object (); // The last transaction is about to be closed
        private static readonly object s_eventTransactionOpened = new object (); // The first transaction has been opened
        private static readonly object s_eventTransactionOpening = new object (); // The first transaction is about to be opened

        // IComponentChangeService events
        private static readonly object s_eventComponentAdding = new object (); // A component is about to be added to the container
        private static readonly object s_eventComponentAdded = new object (); // A component was just added to the container
        private static readonly object s_eventComponentChanging = new object (); // A component is about to be changed
        private static readonly object s_eventComponentChanged = new object (); // A component has changed
        private static readonly object s_eventComponentRemoving = new object (); // A component is about to be removed from the container
        private static readonly object s_eventComponentRemoved = new object (); // A component has been removed from the container
        private static readonly object s_eventComponentRename = new object (); // A component has been renamed

        // State flags for the state of the designer host
        private static readonly int s_stateLoading = BitVector32.CreateMask (); // Designer is currently loading from the loader host.
        private static readonly int s_stateUnloading = BitVector32.CreateMask (s_stateLoading); // Designer is currently unloading.
        private static readonly int s_stateIsClosingTransaction = BitVector32.CreateMask (s_stateUnloading); // A transaction is in the process of being Canceled or Commited.

        public DesignerHost (DesignSurface surface)
        {
            _surface = surface;
            _state = new BitVector32 ();
            _designers = new Hashtable ();
            _events = new EventHandlerList ();

            // Add the relevant services.  We try to add these as "fixed" services.  A fixed service cannot be removed by the user.  The reason for this is that each of these services depends on each other, so you can't really remove and replace just one of them. If we can't get our own service container that supports fixed services, we add these as regular services.
            //if (GetService (typeof (DesignSurfaceServiceContainer)) is DesignSurfaceServiceContainer dsc) {
            //    foreach (Type t in s_defaultServices) {
            //        dsc.AddFixedService (t, this);
            //    }
            //} else {
                IServiceContainer sc = GetService (typeof (IServiceContainer)) as IServiceContainer;
                Debug.Assert (sc is not null, "DesignerHost: Ctor needs a service provider that provides IServiceContainer");
                if (sc is not null) {
                    foreach (Type t in s_defaultServices) {
                        sc.AddService (t, this);
                    }
                }
            //}
        }

        /// <summary>
        ///  This is called by the designer loader to indicate that the load has  terminated.  If there were errors, they should be passed in the errorCollection as a collection of exceptions (if they are not exceptions the designer loader host may just call ToString on them).  If the load was successful then errorCollection should either be null or contain an empty collection.
        /// </summary>
        void IDesignerLoaderHost.EndLoad (string rootClassName, bool successful, ICollection errorCollection)
        {
            bool wasLoading = _state[s_stateLoading];
            _state[s_stateLoading] = false;

            if (rootClassName is not null) {
                _rootComponentClassName = rootClassName;
            } else if (_rootComponent is not null && _rootComponent.Site is not null) {
                _rootComponentClassName = _rootComponent.Site.Name;
            }

            // If the loader indicated success, but it never created a component, that is an error.
            if (successful && _rootComponent is null) {
                ArrayList errorList = new ArrayList ();
                InvalidOperationException ex = new InvalidOperationException (SR.DesignerHostNoBaseClass) {
                    HelpLink = SR.DesignerHostNoBaseClass
                };
                errorList.Add (ex);
                errorCollection = errorList;
                successful = false;
            }

            // TODO
            //// If we failed, unload the doc so that the OnLoaded event can't get to anything that actually did work.
            //if (!successful) {
            //    Unload ();
            //}

            //if (wasLoading && _surface is not null) {
            //    _surface.OnLoaded (successful, errorCollection);
            //}

            //if (successful) {
            //    // We may be invoked to do an EndLoad when we are already loaded.  This can happen if the user called AddLoadDependency, essentially putting us in a loading state while we are already loaded.  This is OK, and is used as a hint that the user is going to monkey with settings but doesn't want the code engine to report it.
            //    if (wasLoading) {
            //        IRootDesigner rootDesigner = ((IDesignerHost)this).GetDesigner (_rootComponent) as IRootDesigner;
            //        // Offer up our base help attribute
            //        if (GetService (typeof (IHelpService)) is IHelpService helpService) {
            //            helpService.AddContextAttribute ("Keyword", "Designer_" + rootDesigner.GetType ().FullName, HelpKeywordType.F1Keyword);
            //        }

            //        // and let everyone know that we're loaded
            //        try {
            //            OnLoadComplete (EventArgs.Empty);
            //        } catch (Exception ex) {
            //            Debug.Fail ("Exception thrown on LoadComplete event handler.  You should not throw here : " + ex.ToString ());
            //            // The load complete failed.  Put us back in the loading state and unload.
            //            _state[s_stateLoading] = true;
            //            Unload ();

            //            ArrayList errorList = new ArrayList
            //            {
            //                ex
            //            };
            //            if (errorCollection is not null) {
            //                errorList.AddRange (errorCollection);
            //            }

            //            errorCollection = errorList;
            //            successful = false;

            //            if (_surface is not null) {
            //                _surface.OnLoaded (successful, errorCollection);
            //            }

            //            // We re-throw.  If this was a synchronous load this will error back to BeginLoad (and, as a side effect, may call us again).  For asynchronous loads we need to throw so the caller knows what happened.
            //            throw;
            //        }

            //        // If we saved a selection as a result of a reload, try to replace it.
            //        if (successful && _savedSelection is not null) {
            //            if (GetService (typeof (ISelectionService)) is ISelectionService ss) {
            //                ArrayList selectedComponents = new ArrayList (_savedSelection.Count);
            //                foreach (string name in _savedSelection) {
            //                    IComponent comp = Components[name];
            //                    if (comp is not null) {
            //                        selectedComponents.Add (comp);
            //                    }
            //                }

            //                _savedSelection = null;
            //                ss.SetSelectedComponents (selectedComponents, SelectionTypes.Replace);
            //            }
            //        }
            //    }
            //}
        }

        /// <summary>
        ///  Override of Container's GetService method.  This just delegates to the  parent service provider.
        /// </summary>
        /// <param name="service"> The type of service to retrieve </param>
        /// <returns> An instance of the service. </returns>
        protected override object GetService (Type service)
        {
            object serviceInstance = null;
            ArgumentNullException.ThrowIfNull (service);

            if (service == typeof (IMultitargetHelperService)) {
                if (_loader is IServiceProvider provider) {
                    serviceInstance = provider.GetService (typeof (IMultitargetHelperService));
                }
            } else {
                serviceInstance = base.GetService (service);
                if (serviceInstance is null && _surface is not null) {
                    serviceInstance = _surface.GetService (service);
                }
            }

            return serviceInstance;
        }

        IContainer IDesignerHost.Container => this;

        bool IDesignerHost.InTransaction => false;

        bool IDesignerHost.Loading => false;

        IComponent IDesignerHost.RootComponent => _rootComponent;

        string IDesignerHost.RootComponentClassName => _rootComponentClassName;

        string IDesignerHost.TransactionDescription => throw new NotImplementedException ();

        /// <summary>
        ///  Adds an event handler for the <see cref='IDesignerHost.Activated'/> event.
        /// </summary>
        event EventHandler IDesignerHost.Activated {
            add => _events.AddHandler (s_eventActivated, value);
            remove => _events.RemoveHandler (s_eventActivated, value);
        }

        /// <summary>
        ///  Adds an event handler for the <see cref='IDesignerHost.Deactivated'/> event.
        /// </summary>
        event EventHandler IDesignerHost.Deactivated {
            add => _events.AddHandler (s_eventDeactivated, value);
            remove => _events.RemoveHandler (s_eventDeactivated, value);
        }

        /// <summary>
        ///  Adds an event handler for the <see cref='IDesignerHost.LoadComplete'/> event.
        /// </summary>
        event EventHandler IDesignerHost.LoadComplete {
            add => _events.AddHandler (s_eventLoadComplete, value);
            remove => _events.RemoveHandler (s_eventLoadComplete, value);
        }

        /// <summary>
        ///  Adds an event handler for the <see cref='IDesignerHost.TransactionClosed'/> event.
        /// </summary>
        event DesignerTransactionCloseEventHandler IDesignerHost.TransactionClosed {
            add => _events.AddHandler (s_eventTransactionClosed, value);
            remove => _events.RemoveHandler (s_eventTransactionClosed, value);
        }

        /// <summary>
        ///  Adds an event handler for the <see cref='IDesignerHost.TransactionClosing'/> event.
        /// </summary>
        event DesignerTransactionCloseEventHandler IDesignerHost.TransactionClosing {
            add => _events.AddHandler (s_eventTransactionClosing, value);
            remove => _events.RemoveHandler (s_eventTransactionClosing, value);
        }

        /// <summary>
        ///  Adds an event handler for the <see cref='IDesignerHost.TransactionOpened'/> event.
        /// </summary>
        event EventHandler IDesignerHost.TransactionOpened {
            add => _events.AddHandler (s_eventTransactionOpened, value);
            remove => _events.RemoveHandler (s_eventTransactionOpened, value);
        }

        /// <summary>
        ///  Adds an event handler for the <see cref='IDesignerHost.TransactionOpening'/> event.
        /// </summary>
        event EventHandler IDesignerHost.TransactionOpening {
            add => _events.AddHandler (s_eventTransactionOpening, value);
            remove => _events.RemoveHandler (s_eventTransactionOpening, value);
        }

        /// <summary>
        ///  Called by DesignerSurface to begin loading the designer.
        /// </summary>
        internal void BeginLoad (DesignerLoader loader)
        {
            if (_loader is not null && _loader != loader) {
                Exception ex = new InvalidOperationException (SR.DesignerHostLoaderSpecified) {
                    HelpLink = SR.DesignerHostLoaderSpecified
                };
                throw ex;
            }

            IDesignerEventService des = null;
            bool reloading = (_loader is not null);
            _loader = loader;
            if (!reloading) {
                if (loader is IExtenderProvider) {
                    if (GetService (typeof (IExtenderProviderService)) is IExtenderProviderService eps) {
                        eps.AddExtenderProvider ((IExtenderProvider)loader);
                    }
                }

                des = GetService (typeof (IDesignerEventService)) as IDesignerEventService;
                if (des is not null) {
                    des.ActiveDesignerChanged += new ActiveDesignerEventHandler (OnActiveDesignerChanged);
                    _designerEventService = des;
                }
            }

            _state[s_stateLoading] = true;
            _surface.OnLoading ();

            try {
                _loader?.BeginLoad (this);
            } catch (Exception e) {
                if (e is TargetInvocationException) {
                    e = e.InnerException;
                }

                string message = e.Message;
                // We must handle the case of an exception with no message.
                if (message is null || message.Length == 0) {
                    e = new Exception (string.Format (SR.DesignSurfaceFatalError, e.ToString ()), e);
                }

                  // Loader blew up.  Add this exception to our error list.
                  ((IDesignerLoaderHost)this).EndLoad (null, false, new object[] { e });
            }

            if (_designerEventService is null) {
                // If there is no designer event service, make this designer the currently active designer.  It will remain active.
                OnActiveDesignerChanged (null, new ActiveDesignerEventArgs (null, this));
            }
        }

        void IDesignerHost.Activate ()
        {
            //throw new NotImplementedException ();
        }

        /// <summary>
        ///  Adds the given service to the service container.
        /// </summary>
        void IServiceContainer.AddService (Type serviceType, ServiceCreatorCallback callback)
        {
            // Our service container is implemented on the parenting DesignSurface object, so we just ask for its service container and run with it.
            if (GetService (typeof (IServiceContainer)) is not IServiceContainer sc)
                throw new ObjectDisposedException ("IServiceContainer");

            sc.AddService (serviceType, callback);
        }

        /// <summary>
        ///  Adds the given service to the service container.
        /// </summary>
        void IServiceContainer.AddService (Type serviceType, ServiceCreatorCallback callback, bool promote)
        {
            // Our service container is implemented on the parenting DesignSurface object, so we just ask for its service container and run with it.
            if (GetService (typeof (IServiceContainer)) is not IServiceContainer sc)
                throw new ObjectDisposedException ("IServiceContainer");

            sc.AddService (serviceType, callback, promote);
        }

        /// <summary>
        ///  Adds the given service to the service container.
        /// </summary>
        void IServiceContainer.AddService (Type serviceType, object serviceInstance)
        {
            // Our service container is implemented on the parenting DesignSurface object, so we just ask for its service container and run with it.
            if (GetService (typeof (IServiceContainer)) is not IServiceContainer sc)
                throw new ObjectDisposedException ("IServiceContainer");

            sc.AddService (serviceType, serviceInstance);
        }

        /// <summary>
        ///  Adds the given service to the service container.
        /// </summary>
        void IServiceContainer.AddService (Type serviceType, object serviceInstance, bool promote)
        {
            // Our service container is implemented on the parenting DesignSurface object, so we just ask for its service container and run with it.
            if (GetService (typeof (IServiceContainer)) is not IServiceContainer sc)
                throw new ObjectDisposedException ("IServiceContainer");

            sc.AddService (serviceType, serviceInstance, promote);
        }

        /// <summary>
        ///  Creates a component of the specified class type.
        /// </summary>
        IComponent IDesignerHost.CreateComponent (Type componentType)
        {
            return ((IDesignerHost)this).CreateComponent (componentType, null);
        }

        /// <summary>
        ///  Creates a component of the given class type and name and places it into the designer container.
        /// </summary>
        IComponent IDesignerHost.CreateComponent (Type componentType, string name)
        {
            ArgumentNullException.ThrowIfNull (componentType);

            IComponent component;
            //LicenseContext oldContext = LicenseManager.CurrentContext;
            //bool changingContext = false; // we don't want if there is a recursivity (creating a component create another one) to change the context again. we already have the one we want and that would create a locking problem.
            //if (oldContext != LicenseContext) {
            //    LicenseManager.CurrentContext = LicenseContext;
            //    LicenseManager.LockContext (s_selfLock);
            //    changingContext = true;
            //}

            try {
                try {
                    _newComponentName = name;
                    component = _surface.CreateInstance (componentType) as IComponent;
                } finally {
                    _newComponentName = null;
                }

                if (component is null) {
                    InvalidOperationException ex = new InvalidOperationException (string.Format (SR.DesignerHostFailedComponentCreate, componentType.Name)) {
                        HelpLink = SR.DesignerHostFailedComponentCreate
                    };
                    throw ex;
                }

                // Add this component to our container
                //
                if (component.Site is null || component.Site.Container != this) {
                    PerformAdd (component, name);
                }
            } finally {
                //if (changingContext) {
                //    LicenseManager.UnlockContext (s_selfLock);
                //    LicenseManager.CurrentContext = oldContext;
                //}
            }

            return component;
        }

        DesignerTransaction IDesignerHost.CreateTransaction ()
        {
            throw new NotImplementedException ();
        }

        DesignerTransaction IDesignerHost.CreateTransaction (string description)
        {
            throw new NotImplementedException ();
        }

        void IDesignerHost.DestroyComponent (IComponent component)
        {
            throw new NotImplementedException ();
        }

        /// <summary>
        ///  Gets the designer instance for the specified component.
        /// </summary>
        IDesigner IDesignerHost.GetDesigner (IComponent component)
        {
            ArgumentNullException.ThrowIfNull (component);

            return _designers[component] as IDesigner;
        }

        bool IDesignerLoaderHost2.IgnoreErrorsDuringReload {
            get => _ignoreErrorsDuringReload;
            set {
                // Only allow to set to true if we CanReloadWithErrors
                if (!value || ((IDesignerLoaderHost2)this).CanReloadWithErrors) {
                    _ignoreErrorsDuringReload = value;
                }
            }
        }

        bool IDesignerLoaderHost2.CanReloadWithErrors {
            get => _canReloadWithErrors;
            set => _canReloadWithErrors = value;
        }

        /// <summary>
        ///  IServiceProvider implementation.  We just delegate to the  protected GetService method we are inheriting from our container.
        /// </summary>
        object? IServiceProvider.GetService (Type serviceType)
        {
            return GetService (serviceType);
        }

        Type? IDesignerHost.GetType (string typeName)
        {
            throw new NotImplementedException ();
        }

        /// <summary>
        ///  Removes the given service type from the service container.
        /// </summary>
        void IServiceContainer.RemoveService (Type serviceType)
        {
            // Our service container is implemented on the parenting DesignSurface object, so we just ask for its service container and run with it.
            if (GetService (typeof (IServiceContainer)) is not IServiceContainer sc)
                throw new ObjectDisposedException ("IServiceContainer");

            sc.RemoveService (serviceType);
        }

        /// <summary>
        ///  Removes the given service type from the service container.
        /// </summary>
        void IServiceContainer.RemoveService (Type serviceType, bool promote)
        {
            // Our service container is implemented on the parenting DesignSurface object, so we just ask for its service container and run with it.
            if (GetService (typeof (IServiceContainer)) is not IServiceContainer sc)
                throw new ObjectDisposedException ("IServiceContainer");

            sc.RemoveService (serviceType, promote);
        }

        private void PerformAdd (IComponent component, string name)
        {
            if (AddToContainerPreProcess (component, name, this)) {
                // Site creation fabricates a name for this component.
                base.Add (component, name);
                try {
                    AddToContainerPostProcess (component, name, this);
                } catch (Exception t) {
                    if (t != CheckoutException.Canceled) {
                        Remove (component);
                    }

                    throw;
                }
            }
        }

        /// <summary>
        ///  We support adding to either our main IDesignerHost container or to a private per-site container for nested objects.  This code is the stock add code that creates a designer, etc.  See Add (above) for an example of how to call this correctly.
        ///  This method is called before the component is actually added.  It returns true if the component can be added to this container or false if the add should not occur (because the component may already be in this container, for example.) It may also throw if adding this component is illegal.
        /// </summary>
        internal bool AddToContainerPreProcess (IComponent component, string name, IContainer containerToAddTo)
        {
            ArgumentNullException.ThrowIfNull (component);

            // We should never add anything while we're unloading.
            if (_state[s_stateUnloading]) {
                Exception ex = new Exception (SR.DesignerHostUnloading) {
                    HelpLink = SR.DesignerHostUnloading
                };
                throw ex;
            }

            // Make sure we're not adding an instance of the root component to itself.
            if (_rootComponent is not null) {
                if (string.Equals (component.GetType ().FullName, _rootComponentClassName, StringComparison.OrdinalIgnoreCase)) {
                    Exception ex = new Exception (string.Format (SR.DesignerHostCyclicAdd, component.GetType ().FullName, _rootComponentClassName)) {
                        HelpLink = SR.DesignerHostCyclicAdd
                    };
                    throw ex;
                }
            }

            ISite existingSite = component.Site;
            // If the component is already in our container, we just rename.
            if (existingSite is not null && existingSite.Container == this) {
                if (name is not null) {
                    existingSite.Name = name;
                }

                return false;
            }

            // Raise an adding event for our container if the container is us.
            ComponentEventArgs ce = new ComponentEventArgs (component);
            (_events[s_eventComponentAdding] as ComponentEventHandler)?.Invoke (containerToAddTo, ce);
            return true;
        }

        /// <summary>
        ///  We support adding to either our main IDesignerHost container or to a private     per-site container for nested objects.  This code is the stock add code     that creates a designer, etc.  See Add (above) for an example of how to call     this correctly.
        /// </summary>
        internal void AddToContainerPostProcess (IComponent component, string name, IContainer containerToAddTo)
        {
            // Now that we've added, check to see if this is an extender provider.  If it is, add it to our extender provider service so it is available.
            if (component is IExtenderProvider &&
                // UNDONE.  Try to get Inheritance knowledge out of this basic code.
                !TypeDescriptor.GetAttributes (component).Contains (InheritanceAttribute.InheritedReadOnly)) {
                if (GetService (typeof (IExtenderProviderService)) is IExtenderProviderService eps) {
                    eps.AddExtenderProvider ((IExtenderProvider)component);
                }
            }

            IDesigner designer;
            // Is this the first component the loader has created?  If so, then it must be the root component (by definition) so we will expect there to be a root designer associated with the component.  Otherwise, we search for a normal designer, which can be optionally provided.
            if (_rootComponent is null) {
                designer = _surface.CreateDesigner (component, true) as IRootDesigner;
                if (designer is null) {
                    Exception ex = new Exception (string.Format (SR.DesignerHostNoTopLevelDesigner, component.GetType ().FullName)) {
                        HelpLink = SR.DesignerHostNoTopLevelDesigner
                    };
                    throw ex;
                }

                _rootComponent = component;
                // Check and see if anyone has set the class name of the root component. we default to the component name.
                if (_rootComponentClassName is null) {
                    _rootComponentClassName = component.Site.Name;
                }
            } else {
                designer = _surface.CreateDesigner (component, false);
            }

            if (designer is not null) {
                // The presence of a designer in this table allows the designer to filter the component's properties, which is often needed during designer initialization.  So, we stuff it in the table first, initialize, and if it throws we remove it from the table.
                _designers[component] = designer;
                try {
                    designer.Initialize (component);
                    if (designer.Component is null) {
                        throw new InvalidOperationException (SR.DesignerHostDesignerNeedsComponent);
                    }
                } catch {
                    _designers.Remove (component);
                    throw;
                }

                // Designers can also implement IExtenderProvider.
                if (designer is IExtenderProvider) {
                    if (GetService (typeof (IExtenderProviderService)) is IExtenderProviderService eps) {
                        eps.AddExtenderProvider ((IExtenderProvider)designer);
                    }
                }
            }

            // The component has been added.  Note that it is tempting to move this above the designer because the designer will never need to know that its own component just got added, but this would be bad because the designer is needed to extract shadowed properties from the component.
            ComponentEventArgs ce = new ComponentEventArgs (component);
            (_events[s_eventComponentAdded] as ComponentEventHandler)?.Invoke (containerToAddTo, ce);
        }

        /// <summary>
        ///  Called in response to a designer becoming active or inactive.
        /// </summary>
        private void OnActiveDesignerChanged (object sender, ActiveDesignerEventArgs e)
        {
            // NOTE: sender can be null (we call this directly in BeginLoad)
            if (e is null) {
                return;
            }

            object eventobj = null;

            if (e.OldDesigner == this) {
                eventobj = s_eventDeactivated;
            } else if (e.NewDesigner == this) {
                eventobj = s_eventActivated;
            }

            // Not our document, so we don't fire.
            if (eventobj is null) {
                return;
            }

            // If we are deactivating, flush any code changes. We always route through the design surface so it can correctly raise its Flushed event.
            if (e.OldDesigner == this && _surface is not null) {
                //_surface.Flush ();  // TODO
            }

            // Fire the appropriate event.
            (_events[eventobj] as EventHandler)?.Invoke (this, EventArgs.Empty);
        }

        /// <summary>
        ///  This is called by the designer loader when it wishes to reload the design document.  The reload will happen immediately so the caller should ensure that it is in a state where BeginLoad may be called again.
        /// </summary>
        void IDesignerLoaderHost.Reload ()
        {
            //if (_loader is not null) {
            //    // Flush the loader to make sure there aren't any pending  changes.  We always route through the design surface so it can correctly raise its Flushed event.
            //    _surface.Flush ();
            //    // Next, stash off the set of selected objects by name.  After the reload we will attempt to re-select them.
            //    if (GetService (typeof (ISelectionService)) is ISelectionService ss) {
            //        ArrayList list = new ArrayList (ss.SelectionCount);
            //        foreach (object o in ss.GetSelectedComponents ()) {
            //            if (o is IComponent comp && comp.Site is not null && comp.Site.Name is not null) {
            //                list.Add (comp.Site.Name);
            //            }
            //        }

            //        _savedSelection = list;
            //    }

            //    Unload ();
            //    BeginLoad (_loader);
            //}
        }
    }
}

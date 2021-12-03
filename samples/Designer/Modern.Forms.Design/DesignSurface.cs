using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Modern.Forms.Design
{
    public class DesignSurface : IDisposable, IServiceProvider
    {
        private readonly IServiceProvider? _parentProvider;
        private readonly ServiceContainer _serviceContainer;
        private readonly DesignerHost _host;
        private ICollection _loadErrors;

        /// <summary>
        ///  Creates a new DesignSurface.
        /// </summary>
        public DesignSurface () : this ((IServiceProvider?)null)
        {
        }

        /// <summary>
        ///  Creates a new DesignSurface given a parent service provider.
        /// </summary>
        /// <param name="parentProvider"> The parent service provider. If there is no parent used to resolve services this can be null. </param>
        public DesignSurface (IServiceProvider? parentProvider)
        {
            _parentProvider = parentProvider;
            _serviceContainer = new ServiceContainer (_parentProvider);

            // Configure our default services
            //ServiceCreatorCallback callback = new ServiceCreatorCallback (OnCreateService);
            //ServiceContainer.AddService (typeof (ISelectionService), callback);
            //ServiceContainer.AddService (typeof (IExtenderProviderService), callback);
            //ServiceContainer.AddService (typeof (IExtenderListService), callback);
            //ServiceContainer.AddService (typeof (ITypeDescriptorFilterService), callback);
            //ServiceContainer.AddService (typeof (IReferenceService), callback);

            ServiceContainer.AddService (typeof (DesignSurface), this);
            _host = new DesignerHost (this);
        }

        /// <summary>
        ///  Creates a new DesignSurface.
        /// </summary>
        public DesignSurface (Type rootComponentType) : this (null, rootComponentType)
        {
        }

        /// <summary>
        ///  Creates a new DesignSurface given a parent service provider.
        /// </summary>
        /// <param name="parentProvider"> The parent service provider.  If there is no parent used to resolve services this can be null. </param>
        public DesignSurface (IServiceProvider? parentProvider, Type rootComponentType) : this (parentProvider)
        {
            ArgumentNullException.ThrowIfNull (rootComponentType);

            BeginLoad (rootComponentType);
        }

        /// <summary>
        ///  This method begins the loading process for a component of the given type.  This will create an instance of the component type and initialize a designer for that instance.  Loaded is raised before this method returns.
        /// </summary>
        public void BeginLoad (Type rootComponentType)
        {
            ArgumentNullException.ThrowIfNull (rootComponentType);

            if (_host is null) {
                throw new ObjectDisposedException (GetType ().FullName);
            }

            BeginLoad (new DefaultDesignerLoader (rootComponentType));
        }

        /// <summary>
        ///  This method begins the loading process with the given designer loader.  Designer loading can be asynchronous, so the loading may continue to  progress after this call has returned.  Listen to the Loaded event to know when the design surface has completed loading.
        /// </summary>
        public void BeginLoad (DesignerLoader loader)
        {
            ArgumentNullException.ThrowIfNull (loader);

            if (_host is null) {
                throw new ObjectDisposedException (GetType ().FullName);
            }

            // Create the designer host.  We need the host so we can begin the loading process.
            //_loadErrors = null;
            _host.BeginLoad (loader);
        }

        /// <summary>
        ///  Provides access to the design surface's container, which contains all components currently being designed.
        /// </summary>
        public IContainer ComponentContainer {
            get {
                if (_host is null) {
                    throw new ObjectDisposedException (GetType ().FullName);
                }

                return ((IDesignerHost)_host).Container;
            }
        }

        /// <summary>
        ///  This method is called to create a designer for a component.
        /// </summary>
        protected internal virtual IDesigner CreateDesigner (IComponent component, bool rootDesigner)
        {
            ArgumentNullException.ThrowIfNull (component);

            if (_host is null) {
                throw new ObjectDisposedException (GetType ().FullName);
            }

            IDesigner designer;
            if (rootDesigner) {
                designer = TypeDescriptor.CreateDesigner (component, typeof (IRootDesigner)) as IRootDesigner;
            } else {
                designer = TypeDescriptor.CreateDesigner (component, typeof (IDesigner));
            }

            return designer;
        }

        /// <summary>
        ///  This method is called to create an instance of the given type.  If the type is a component
        ///  this will search for a constructor of type IContainer first, and then an empty constructor.
        /// </summary>
        protected internal virtual object CreateInstance (Type type)
        {
            ArgumentNullException.ThrowIfNull (type);

            // Locate an appropriate constructor for IComponents.
            object instance = null;
            ConstructorInfo ctor = TypeDescriptor.GetReflectionType (type).GetConstructor (Array.Empty<Type> ());
            if (ctor is not null) {
                instance = TypeDescriptor.CreateInstance (this, type, Array.Empty<Type> (), Array.Empty<object> ());
            } else {
                if (typeof (IComponent).IsAssignableFrom (type)) {
                    ctor = TypeDescriptor.GetReflectionType (type).GetConstructor (BindingFlags.Public | BindingFlags.Instance | BindingFlags.ExactBinding, null, new Type[] { typeof (IContainer) }, null);
                }

                if (ctor is not null) {
                    instance = TypeDescriptor.CreateInstance (this, type, new Type[] { typeof (IContainer) }, new object[] { ComponentContainer });
                }
            }

            if (instance is null) {
                instance = Activator.CreateInstance (type, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.CreateInstance, null, null, null);
            }

            return instance;
        }

        public void Dispose ()
        {
        }

        /// <summary>
        ///  Retrieves a service in this design surface's service container.
        /// </summary>
        /// <param name="serviceType"> The type of service to retrieve. </param>
        /// <returns> An instance of the requested service or null if the service could not be found. </returns>
        public object? GetService (Type serviceType)
        {
            if (_serviceContainer is not null) {
                return _serviceContainer.GetService (serviceType);
            }

            return null;
        }

        /// <summary>
        ///  Called when the designer load is about to begin the loading process.
        /// </summary>
        public event EventHandler Loading;

        /// <summary>
        ///  Called when the loading process is about to begin.
        /// </summary>
        internal void OnLoading ()
        {
            OnLoading (EventArgs.Empty);
        }

        /// <summary>
        ///  Called when the loading process is about to begin.
        /// </summary>
        protected virtual void OnLoading (EventArgs e)
        {
            Loading?.Invoke (this, e);
        }

        /// <summary>
        ///  Provides access to the design surface's ServiceContainer. This property allows inheritors to add their own services.
        /// </summary>
        protected ServiceContainer ServiceContainer {
            get {
                if (_serviceContainer is null) {
                    throw new ObjectDisposedException (GetType ().FullName);
                }

                return _serviceContainer;
            }
        }

        /// <summary>
        ///  This property will return the view for the root designer. BeginLoad must have been called beforehand to start the loading process. It is possible to return a view before the designer loader finishes loading because the root designer, which supplies the view, is the first object created by the designer loader. If a view is unavailable this method will throw an exception.
        ///  Possible exceptions:
        ///  The design surface is not loading or the designer loader has not yet created a root designer: InvalidOperationException
        ///  The design surface finished the load, but failed. (Various. This will throw the first exception the designer loader added to the error collection).
        /// </summary>
        public object View {
            get {
                if (_host is null) {
                    throw new ObjectDisposedException (ToString ());
                }

                IComponent rootComponent = ((IDesignerHost)_host).RootComponent;
                if (rootComponent is null) {
                    // Check to see if we have any load errors.  If so, use them.
                    if (_loadErrors is not null) {
                        foreach (object o in _loadErrors) {
                            if (o is Exception ex) {
                                throw new InvalidOperationException (ex.Message, ex);
                            } else if (o is not null) {
                                throw new InvalidOperationException (o.ToString ());
                            }
                        }
                    }

                    // loader didn't provide any help.  Just generally fail.
                    throw new InvalidOperationException (SR.DesignSurfaceNoRootComponent) {
                        HelpLink = SR.DesignSurfaceNoRootComponent
                    };
                }

                if (!(((IDesignerHost)_host).GetDesigner (rootComponent) is IRootDesigner rootDesigner)) {
                    throw new InvalidOperationException (SR.DesignSurfaceDesignerNotLoaded) {
                        HelpLink = SR.DesignSurfaceDesignerNotLoaded
                    };
                }

                ViewTechnology[] designerViews = rootDesigner.SupportedTechnologies;
                if (designerViews is null || designerViews.Length == 0) {
                    throw new NotSupportedException (SR.DesignSurfaceNoSupportedTechnology) {
                        HelpLink = SR.DesignSurfaceNoSupportedTechnology
                    };
                }

                // We just feed the available technologies back into the root designer. ViewTechnology itself is outdated.
                return rootDesigner.GetView (designerViews[0]);
            }
        }

        /// <summary>
        ///  This is a simple designer loader that creates an instance of the given type and then calls EndLoad.  If a collection of objects was passed, this will simply add those objects to the container.
        /// </summary>
        private class DefaultDesignerLoader : DesignerLoader
        {
            private readonly Type _type;

            public DefaultDesignerLoader (Type type)
            {
                _type = type;
            }

            public override void BeginLoad (IDesignerLoaderHost loaderHost)
            {
                loaderHost.CreateComponent (_type);
                loaderHost.EndLoad (_type.FullName, true, null);
            }

            public override void Dispose ()
            {
            }
        }
    }
}

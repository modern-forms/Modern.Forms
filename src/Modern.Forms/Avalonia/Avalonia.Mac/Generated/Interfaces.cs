namespace Avalonia.Native.Interop
{
    [System.Runtime.InteropServices.GuidAttribute("2e2cda0a-9ae5-4f1b-8e20-081a04279f01")]
    public partial class IAvaloniaNativeFactory : SharpGen.Runtime.ComObject
    {
        public IAvaloniaNativeFactory(System.IntPtr nativePtr): base (nativePtr)
        {
        }

        public static explicit operator IAvaloniaNativeFactory(System.IntPtr nativePtr) => nativePtr == System.IntPtr.Zero ? null : new IAvaloniaNativeFactory(nativePtr);
        /// <summary>
        /// No documentation.
        /// </summary>
        /// <unmanaged>GetMacOptions</unmanaged>
        /// <unmanaged-short>GetMacOptions</unmanaged-short>
        public Avalonia.Native.Interop.IAvnMacOptions MacOptions
        {
            get => GetMacOptions();
        }

        /// <summary>
        /// No documentation.
        /// </summary>
        /// <returns>No documentation.</returns>
        /// <unmanaged>HRESULT IAvaloniaNativeFactory::Initialize()</unmanaged>
        /// <unmanaged-short>IAvaloniaNativeFactory::Initialize</unmanaged-short>
        public unsafe void Initialize()
        {
            SharpGen.Runtime.Result __result__;
            __result__ = Avalonia.Native.LocalInterop.CalliThisCallint(this._nativePointer, (*(void ***)this._nativePointer)[3]);
            __result__.CheckError();
        }

        /// <summary>
        /// No documentation.
        /// </summary>
        /// <returns>No documentation.</returns>
        /// <unmanaged>IAvnMacOptions* IAvaloniaNativeFactory::GetMacOptions()</unmanaged>
        /// <unmanaged-short>IAvaloniaNativeFactory::GetMacOptions</unmanaged-short>
        internal unsafe Avalonia.Native.Interop.IAvnMacOptions GetMacOptions()
        {
            Avalonia.Native.Interop.IAvnMacOptions __result__;
            System.IntPtr __result__native = System.IntPtr.Zero;
            __result__native = Avalonia.Native.LocalInterop.CalliThisCallSystemIntPtr(this._nativePointer, (*(void ***)this._nativePointer)[4]);
            if (__result__native != System.IntPtr.Zero)
                __result__ = new Avalonia.Native.Interop.IAvnMacOptions(__result__native);
            else
                __result__ = null;
            return __result__;
        }

        /// <summary>
        /// No documentation.
        /// </summary>
        /// <param name = "cb">No documentation.</param>
        /// <returns>No documentation.</returns>
        /// <unmanaged>HRESULT IAvaloniaNativeFactory::CreateWindow([In] IAvnWindowEvents* cb,[In] IAvnWindow** ppv)</unmanaged>
        /// <unmanaged-short>IAvaloniaNativeFactory::CreateWindow</unmanaged-short>
        public unsafe Avalonia.Native.Interop.IAvnWindow CreateWindow(Avalonia.Native.Interop.IAvnWindowEvents cb)
        {
            System.IntPtr cb_ = System.IntPtr.Zero;
            Avalonia.Native.Interop.IAvnWindow vOut;
            System.IntPtr vOut_ = System.IntPtr.Zero;
            SharpGen.Runtime.Result __result__;
            cb_ = SharpGen.Runtime.CppObject.ToCallbackPtr<Avalonia.Native.Interop.IAvnWindowEvents>(cb);
            __result__ = Avalonia.Native.LocalInterop.CalliThisCallint(this._nativePointer, (void *)cb_, &vOut_, (*(void ***)this._nativePointer)[5]);
            if (vOut_ != System.IntPtr.Zero)
                vOut = new Avalonia.Native.Interop.IAvnWindow(vOut_);
            else
                vOut = null;
            __result__.CheckError();
            return vOut;
        }

        /// <summary>
        /// No documentation.
        /// </summary>
        /// <param name = "cb">No documentation.</param>
        /// <returns>No documentation.</returns>
        /// <unmanaged>HRESULT IAvaloniaNativeFactory::CreatePopup([In] IAvnWindowEvents* cb,[In] IAvnPopup** ppv)</unmanaged>
        /// <unmanaged-short>IAvaloniaNativeFactory::CreatePopup</unmanaged-short>
        public unsafe Avalonia.Native.Interop.IAvnPopup CreatePopup(Avalonia.Native.Interop.IAvnWindowEvents cb)
        {
            System.IntPtr cb_ = System.IntPtr.Zero;
            Avalonia.Native.Interop.IAvnPopup vOut;
            System.IntPtr vOut_ = System.IntPtr.Zero;
            SharpGen.Runtime.Result __result__;
            cb_ = SharpGen.Runtime.CppObject.ToCallbackPtr<Avalonia.Native.Interop.IAvnWindowEvents>(cb);
            __result__ = Avalonia.Native.LocalInterop.CalliThisCallint(this._nativePointer, (void *)cb_, &vOut_, (*(void ***)this._nativePointer)[6]);
            if (vOut_ != System.IntPtr.Zero)
                vOut = new Avalonia.Native.Interop.IAvnPopup(vOut_);
            else
                vOut = null;
            __result__.CheckError();
            return vOut;
        }

        /// <summary>
        /// No documentation.
        /// </summary>
        /// <returns>No documentation.</returns>
        /// <unmanaged>HRESULT IAvaloniaNativeFactory::CreatePlatformThreadingInterface([In] IAvnPlatformThreadingInterface** ppv)</unmanaged>
        /// <unmanaged-short>IAvaloniaNativeFactory::CreatePlatformThreadingInterface</unmanaged-short>
        public unsafe Avalonia.Native.Interop.IAvnPlatformThreadingInterface CreatePlatformThreadingInterface()
        {
            Avalonia.Native.Interop.IAvnPlatformThreadingInterface vOut;
            System.IntPtr vOut_ = System.IntPtr.Zero;
            SharpGen.Runtime.Result __result__;
            __result__ = Avalonia.Native.LocalInterop.CalliThisCallint(this._nativePointer, &vOut_, (*(void ***)this._nativePointer)[7]);
            if (vOut_ != System.IntPtr.Zero)
                vOut = new Avalonia.Native.Interop.IAvnPlatformThreadingInterface(vOut_);
            else
                vOut = null;
            __result__.CheckError();
            return vOut;
        }

        /// <summary>
        /// No documentation.
        /// </summary>
        /// <returns>No documentation.</returns>
        /// <unmanaged>HRESULT IAvaloniaNativeFactory::CreateSystemDialogs([In] IAvnSystemDialogs** ppv)</unmanaged>
        /// <unmanaged-short>IAvaloniaNativeFactory::CreateSystemDialogs</unmanaged-short>
        public unsafe Avalonia.Native.Interop.IAvnSystemDialogs CreateSystemDialogs()
        {
            Avalonia.Native.Interop.IAvnSystemDialogs vOut;
            System.IntPtr vOut_ = System.IntPtr.Zero;
            SharpGen.Runtime.Result __result__;
            __result__ = Avalonia.Native.LocalInterop.CalliThisCallint(this._nativePointer, &vOut_, (*(void ***)this._nativePointer)[8]);
            if (vOut_ != System.IntPtr.Zero)
                vOut = new Avalonia.Native.Interop.IAvnSystemDialogs(vOut_);
            else
                vOut = null;
            __result__.CheckError();
            return vOut;
        }

        /// <summary>
        /// No documentation.
        /// </summary>
        /// <returns>No documentation.</returns>
        /// <unmanaged>HRESULT IAvaloniaNativeFactory::CreateScreens([In] IAvnScreens** ppv)</unmanaged>
        /// <unmanaged-short>IAvaloniaNativeFactory::CreateScreens</unmanaged-short>
        public unsafe Avalonia.Native.Interop.IAvnScreens CreateScreens()
        {
            Avalonia.Native.Interop.IAvnScreens vOut;
            System.IntPtr vOut_ = System.IntPtr.Zero;
            SharpGen.Runtime.Result __result__;
            __result__ = Avalonia.Native.LocalInterop.CalliThisCallint(this._nativePointer, &vOut_, (*(void ***)this._nativePointer)[9]);
            if (vOut_ != System.IntPtr.Zero)
                vOut = new Avalonia.Native.Interop.IAvnScreens(vOut_);
            else
                vOut = null;
            __result__.CheckError();
            return vOut;
        }

        /// <summary>
        /// No documentation.
        /// </summary>
        /// <returns>No documentation.</returns>
        /// <unmanaged>HRESULT IAvaloniaNativeFactory::CreateClipboard([In] IAvnClipboard** ppv)</unmanaged>
        /// <unmanaged-short>IAvaloniaNativeFactory::CreateClipboard</unmanaged-short>
        public unsafe Avalonia.Native.Interop.IAvnClipboard CreateClipboard()
        {
            Avalonia.Native.Interop.IAvnClipboard vOut;
            System.IntPtr vOut_ = System.IntPtr.Zero;
            SharpGen.Runtime.Result __result__;
            __result__ = Avalonia.Native.LocalInterop.CalliThisCallint(this._nativePointer, &vOut_, (*(void ***)this._nativePointer)[10]);
            if (vOut_ != System.IntPtr.Zero)
                vOut = new Avalonia.Native.Interop.IAvnClipboard(vOut_);
            else
                vOut = null;
            __result__.CheckError();
            return vOut;
        }

        /// <summary>
        /// No documentation.
        /// </summary>
        /// <returns>No documentation.</returns>
        /// <unmanaged>HRESULT IAvaloniaNativeFactory::CreateCursorFactory([In] IAvnCursorFactory** ppv)</unmanaged>
        /// <unmanaged-short>IAvaloniaNativeFactory::CreateCursorFactory</unmanaged-short>
        public unsafe Avalonia.Native.Interop.IAvnCursorFactory CreateCursorFactory()
        {
            Avalonia.Native.Interop.IAvnCursorFactory vOut;
            System.IntPtr vOut_ = System.IntPtr.Zero;
            SharpGen.Runtime.Result __result__;
            __result__ = Avalonia.Native.LocalInterop.CalliThisCallint(this._nativePointer, &vOut_, (*(void ***)this._nativePointer)[11]);
            if (vOut_ != System.IntPtr.Zero)
                vOut = new Avalonia.Native.Interop.IAvnCursorFactory(vOut_);
            else
                vOut = null;
            __result__.CheckError();
            return vOut;
        }

        /// <summary>
        /// No documentation.
        /// </summary>
        /// <returns>No documentation.</returns>
        /// <unmanaged>HRESULT IAvaloniaNativeFactory::ObtainGlFeature([In] IAvnGlFeature** ppv)</unmanaged>
        /// <unmanaged-short>IAvaloniaNativeFactory::ObtainGlFeature</unmanaged-short>
        public unsafe Avalonia.Native.Interop.IAvnGlFeature ObtainGlFeature()
        {
            Avalonia.Native.Interop.IAvnGlFeature vOut;
            System.IntPtr vOut_ = System.IntPtr.Zero;
            SharpGen.Runtime.Result __result__;
            __result__ = Avalonia.Native.LocalInterop.CalliThisCallint(this._nativePointer, &vOut_, (*(void ***)this._nativePointer)[12]);
            if (vOut_ != System.IntPtr.Zero)
                vOut = new Avalonia.Native.Interop.IAvnGlFeature(vOut_);
            else
                vOut = null;
            __result__.CheckError();
            return vOut;
        }
    }

    class IAvnActionCallbackShadow : SharpGen.Runtime.ComObjectShadow
    {
        protected unsafe class IAvnActionCallbackVtbl : SharpGen.Runtime.ComObjectShadow.ComObjectVtbl
        {
            public IAvnActionCallbackVtbl(int numberOfCallbackMethods): base (numberOfCallbackMethods + 1)
            {
                AddMethod(new RunDelegate(Run));
            }

            [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.ThisCall)]
            private delegate void RunDelegate(System.IntPtr thisObject);
            private static unsafe void Run(System.IntPtr thisObject)
            {
                try
                {
                    IAvnActionCallback @this = (IAvnActionCallback)ToShadow<Avalonia.Native.Interop.IAvnActionCallbackShadow>(thisObject).Callback;
                    @this.Run();
                }
                catch (System.Exception)
                {
                }
            }
        }

        protected override SharpGen.Runtime.CppObjectVtbl Vtbl
        {
            get;
        }

        = new Avalonia.Native.Interop.IAvnActionCallbackShadow.IAvnActionCallbackVtbl(0);
    }

    [System.Runtime.InteropServices.GuidAttribute("2e2cda0a-9ae5-4f1b-8e20-081a04279f08"), SharpGen.Runtime.ShadowAttribute(typeof (Avalonia.Native.Interop.IAvnActionCallbackShadow))]
    public partial interface IAvnActionCallback : SharpGen.Runtime.IUnknown
    {
        void Run();
    }

    [System.Runtime.InteropServices.GuidAttribute("2e2cda0a-9ae5-4f1b-8e20-081a04279f0f")]
    public partial class IAvnClipboard : SharpGen.Runtime.ComObject
    {
        public IAvnClipboard(System.IntPtr nativePtr): base (nativePtr)
        {
        }

        public static explicit operator IAvnClipboard(System.IntPtr nativePtr) => nativePtr == System.IntPtr.Zero ? null : new IAvnClipboard(nativePtr);
        /// <summary>
        /// No documentation.
        /// </summary>
        /// <returns>No documentation.</returns>
        /// <unmanaged>HRESULT IAvnClipboard::GetText([In] IAvnString** ppv)</unmanaged>
        /// <unmanaged-short>IAvnClipboard::GetText</unmanaged-short>
        public unsafe Avalonia.Native.Interop.IAvnString GetText()
        {
            Avalonia.Native.Interop.IAvnString vOut;
            System.IntPtr vOut_ = System.IntPtr.Zero;
            SharpGen.Runtime.Result __result__;
            __result__ = Avalonia.Native.LocalInterop.CalliThisCallint(this._nativePointer, &vOut_, (*(void ***)this._nativePointer)[3]);
            if (vOut_ != System.IntPtr.Zero)
                vOut = new Avalonia.Native.Interop.IAvnString(vOut_);
            else
                vOut = null;
            __result__.CheckError();
            return vOut;
        }

        /// <summary>
        /// No documentation.
        /// </summary>
        /// <param name = "utf8Text">No documentation.</param>
        /// <returns>No documentation.</returns>
        /// <unmanaged>HRESULT IAvnClipboard::SetText([In] void* utf8Text)</unmanaged>
        /// <unmanaged-short>IAvnClipboard::SetText</unmanaged-short>
        public unsafe void SetText(System.IntPtr utf8Text)
        {
            SharpGen.Runtime.Result __result__;
            __result__ = Avalonia.Native.LocalInterop.CalliThisCallint(this._nativePointer, (void *)utf8Text, (*(void ***)this._nativePointer)[4]);
            __result__.CheckError();
        }

        /// <summary>
        /// No documentation.
        /// </summary>
        /// <returns>No documentation.</returns>
        /// <unmanaged>HRESULT IAvnClipboard::Clear()</unmanaged>
        /// <unmanaged-short>IAvnClipboard::Clear</unmanaged-short>
        public unsafe void Clear()
        {
            SharpGen.Runtime.Result __result__;
            __result__ = Avalonia.Native.LocalInterop.CalliThisCallint(this._nativePointer, (*(void ***)this._nativePointer)[5]);
            __result__.CheckError();
        }
    }

    [System.Runtime.InteropServices.GuidAttribute("2e2cda0a-9ae5-4f1b-8e20-081a04279f10")]
    public partial class IAvnCursor : SharpGen.Runtime.ComObject
    {
        public IAvnCursor(System.IntPtr nativePtr): base (nativePtr)
        {
        }

        public static explicit operator IAvnCursor(System.IntPtr nativePtr) => nativePtr == System.IntPtr.Zero ? null : new IAvnCursor(nativePtr);
    }

    [System.Runtime.InteropServices.GuidAttribute("2e2cda0a-9ae5-4f1b-8e20-081a04279f11")]
    public partial class IAvnCursorFactory : SharpGen.Runtime.ComObject
    {
        public IAvnCursorFactory(System.IntPtr nativePtr): base (nativePtr)
        {
        }

        public static explicit operator IAvnCursorFactory(System.IntPtr nativePtr) => nativePtr == System.IntPtr.Zero ? null : new IAvnCursorFactory(nativePtr);
        /// <summary>
        /// No documentation.
        /// </summary>
        /// <param name = "cursorType">No documentation.</param>
        /// <returns>No documentation.</returns>
        /// <unmanaged>HRESULT IAvnCursorFactory::GetCursor([In] AvnStandardCursorType cursorType,[Out] IAvnCursor** retOut)</unmanaged>
        /// <unmanaged-short>IAvnCursorFactory::GetCursor</unmanaged-short>
        public unsafe Avalonia.Native.Interop.IAvnCursor GetCursor(Avalonia.Native.Interop.AvnStandardCursorType cursorType)
        {
            Avalonia.Native.Interop.IAvnCursor retOut;
            System.IntPtr retOut_ = System.IntPtr.Zero;
            SharpGen.Runtime.Result __result__;
            __result__ = Avalonia.Native.LocalInterop.CalliThisCallint(this._nativePointer, unchecked ((System.Int32)cursorType), &retOut_, (*(void ***)this._nativePointer)[3]);
            if (retOut_ != System.IntPtr.Zero)
                retOut = new Avalonia.Native.Interop.IAvnCursor(retOut_);
            else
                retOut = null;
            __result__.CheckError();
            return retOut;
        }
    }

    [System.Runtime.InteropServices.GuidAttribute("2e2cda0a-9ae5-4f1b-8e20-081a04279f14")]
    public partial class IAvnGlContext : SharpGen.Runtime.ComObject
    {
        public IAvnGlContext(System.IntPtr nativePtr): base (nativePtr)
        {
        }

        public static explicit operator IAvnGlContext(System.IntPtr nativePtr) => nativePtr == System.IntPtr.Zero ? null : new IAvnGlContext(nativePtr);
        /// <summary>
        /// No documentation.
        /// </summary>
        /// <returns>No documentation.</returns>
        /// <unmanaged>HRESULT IAvnGlContext::MakeCurrent()</unmanaged>
        /// <unmanaged-short>IAvnGlContext::MakeCurrent</unmanaged-short>
        public unsafe void MakeCurrent()
        {
            SharpGen.Runtime.Result __result__;
            __result__ = Avalonia.Native.LocalInterop.CalliThisCallint(this._nativePointer, (*(void ***)this._nativePointer)[3]);
            __result__.CheckError();
        }
    }

    [System.Runtime.InteropServices.GuidAttribute("2e2cda0a-9ae5-4f1b-8e20-081a04279f13")]
    public partial class IAvnGlDisplay : SharpGen.Runtime.ComObject
    {
        public IAvnGlDisplay(System.IntPtr nativePtr): base (nativePtr)
        {
        }

        public static explicit operator IAvnGlDisplay(System.IntPtr nativePtr) => nativePtr == System.IntPtr.Zero ? null : new IAvnGlDisplay(nativePtr);
        /// <summary>
        /// No documentation.
        /// </summary>
        /// <returns>No documentation.</returns>
        /// <unmanaged>HRESULT IAvnGlDisplay::GetSampleCount([In] int* ret)</unmanaged>
        /// <unmanaged-short>IAvnGlDisplay::GetSampleCount</unmanaged-short>
        public unsafe System.Int32 GetSampleCount()
        {
            System.Int32 ret;
            SharpGen.Runtime.Result __result__;
            __result__ = Avalonia.Native.LocalInterop.CalliThisCallint(this._nativePointer, &ret, (*(void ***)this._nativePointer)[3]);
            __result__.CheckError();
            return ret;
        }

        /// <summary>
        /// No documentation.
        /// </summary>
        /// <returns>No documentation.</returns>
        /// <unmanaged>HRESULT IAvnGlDisplay::GetStencilSize([In] int* ret)</unmanaged>
        /// <unmanaged-short>IAvnGlDisplay::GetStencilSize</unmanaged-short>
        public unsafe System.Int32 GetStencilSize()
        {
            System.Int32 ret;
            SharpGen.Runtime.Result __result__;
            __result__ = Avalonia.Native.LocalInterop.CalliThisCallint(this._nativePointer, &ret, (*(void ***)this._nativePointer)[4]);
            __result__.CheckError();
            return ret;
        }

        /// <summary>
        /// No documentation.
        /// </summary>
        /// <returns>No documentation.</returns>
        /// <unmanaged>HRESULT IAvnGlDisplay::ClearContext()</unmanaged>
        /// <unmanaged-short>IAvnGlDisplay::ClearContext</unmanaged-short>
        public unsafe void ClearContext()
        {
            SharpGen.Runtime.Result __result__;
            __result__ = Avalonia.Native.LocalInterop.CalliThisCallint(this._nativePointer, (*(void ***)this._nativePointer)[5]);
            __result__.CheckError();
        }

        /// <summary>
        /// No documentation.
        /// </summary>
        /// <param name = "rocRef">No documentation.</param>
        /// <returns>No documentation.</returns>
        /// <unmanaged>void* IAvnGlDisplay::GetProcAddress([In] char* proc)</unmanaged>
        /// <unmanaged-short>IAvnGlDisplay::GetProcAddress</unmanaged-short>
        public unsafe System.IntPtr GetProcAddress(System.String rocRef)
        {
            System.IntPtr rocRef_;
            System.IntPtr __result__;
            rocRef_ = System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi(rocRef);
            __result__ = Avalonia.Native.LocalInterop.CalliThisCallSystemIntPtr(this._nativePointer, (void *)rocRef_, (*(void ***)this._nativePointer)[6]);
            System.Runtime.InteropServices.Marshal.FreeHGlobal(rocRef_);
            return __result__;
        }
    }

    [System.Runtime.InteropServices.GuidAttribute("2e2cda0a-9ae5-4f1b-8e20-081a04279f12")]
    public partial class IAvnGlFeature : SharpGen.Runtime.ComObject
    {
        public IAvnGlFeature(System.IntPtr nativePtr): base (nativePtr)
        {
        }

        public static explicit operator IAvnGlFeature(System.IntPtr nativePtr) => nativePtr == System.IntPtr.Zero ? null : new IAvnGlFeature(nativePtr);
        /// <summary>
        /// No documentation.
        /// </summary>
        /// <returns>No documentation.</returns>
        /// <unmanaged>HRESULT IAvnGlFeature::ObtainDisplay([Out] IAvnGlDisplay** retOut)</unmanaged>
        /// <unmanaged-short>IAvnGlFeature::ObtainDisplay</unmanaged-short>
        public unsafe Avalonia.Native.Interop.IAvnGlDisplay ObtainDisplay()
        {
            Avalonia.Native.Interop.IAvnGlDisplay retOut;
            System.IntPtr retOut_ = System.IntPtr.Zero;
            SharpGen.Runtime.Result __result__;
            __result__ = Avalonia.Native.LocalInterop.CalliThisCallint(this._nativePointer, &retOut_, (*(void ***)this._nativePointer)[3]);
            if (retOut_ != System.IntPtr.Zero)
                retOut = new Avalonia.Native.Interop.IAvnGlDisplay(retOut_);
            else
                retOut = null;
            __result__.CheckError();
            return retOut;
        }

        /// <summary>
        /// No documentation.
        /// </summary>
        /// <returns>No documentation.</returns>
        /// <unmanaged>HRESULT IAvnGlFeature::ObtainImmediateContext([Out] IAvnGlContext** retOut)</unmanaged>
        /// <unmanaged-short>IAvnGlFeature::ObtainImmediateContext</unmanaged-short>
        public unsafe Avalonia.Native.Interop.IAvnGlContext ObtainImmediateContext()
        {
            Avalonia.Native.Interop.IAvnGlContext retOut;
            System.IntPtr retOut_ = System.IntPtr.Zero;
            SharpGen.Runtime.Result __result__;
            __result__ = Avalonia.Native.LocalInterop.CalliThisCallint(this._nativePointer, &retOut_, (*(void ***)this._nativePointer)[4]);
            if (retOut_ != System.IntPtr.Zero)
                retOut = new Avalonia.Native.Interop.IAvnGlContext(retOut_);
            else
                retOut = null;
            __result__.CheckError();
            return retOut;
        }
    }

    [System.Runtime.InteropServices.GuidAttribute("2e2cda0a-9ae5-4f1b-8e20-081a04279f16")]
    public partial class IAvnGlSurfaceRenderingSession : SharpGen.Runtime.ComObject
    {
        public IAvnGlSurfaceRenderingSession(System.IntPtr nativePtr): base (nativePtr)
        {
        }

        public static explicit operator IAvnGlSurfaceRenderingSession(System.IntPtr nativePtr) => nativePtr == System.IntPtr.Zero ? null : new IAvnGlSurfaceRenderingSession(nativePtr);
        /// <summary>
        /// No documentation.
        /// </summary>
        /// <returns>No documentation.</returns>
        /// <unmanaged>HRESULT IAvnGlSurfaceRenderingSession::GetPixelSize([In] AvnPixelSize* ret)</unmanaged>
        /// <unmanaged-short>IAvnGlSurfaceRenderingSession::GetPixelSize</unmanaged-short>
        public unsafe Avalonia.Native.Interop.AvnPixelSize GetPixelSize()
        {
            Avalonia.Native.Interop.AvnPixelSize ret;
            SharpGen.Runtime.Result __result__;
            __result__ = Avalonia.Native.LocalInterop.CalliThisCallint(this._nativePointer, &ret, (*(void ***)this._nativePointer)[3]);
            __result__.CheckError();
            return ret;
        }

        /// <summary>
        /// No documentation.
        /// </summary>
        /// <returns>No documentation.</returns>
        /// <unmanaged>HRESULT IAvnGlSurfaceRenderingSession::GetScaling([In] double* ret)</unmanaged>
        /// <unmanaged-short>IAvnGlSurfaceRenderingSession::GetScaling</unmanaged-short>
        public unsafe System.Double GetScaling()
        {
            System.Double ret;
            SharpGen.Runtime.Result __result__;
            __result__ = Avalonia.Native.LocalInterop.CalliThisCallint(this._nativePointer, &ret, (*(void ***)this._nativePointer)[4]);
            __result__.CheckError();
            return ret;
        }
    }

    [System.Runtime.InteropServices.GuidAttribute("2e2cda0a-9ae5-4f1b-8e20-081a04279f15")]
    public partial class IAvnGlSurfaceRenderTarget : SharpGen.Runtime.ComObject
    {
        public IAvnGlSurfaceRenderTarget(System.IntPtr nativePtr): base (nativePtr)
        {
        }

        public static explicit operator IAvnGlSurfaceRenderTarget(System.IntPtr nativePtr) => nativePtr == System.IntPtr.Zero ? null : new IAvnGlSurfaceRenderTarget(nativePtr);
        /// <summary>
        /// No documentation.
        /// </summary>
        /// <returns>No documentation.</returns>
        /// <unmanaged>HRESULT IAvnGlSurfaceRenderTarget::BeginDrawing([In] IAvnGlSurfaceRenderingSession** ret)</unmanaged>
        /// <unmanaged-short>IAvnGlSurfaceRenderTarget::BeginDrawing</unmanaged-short>
        public unsafe Avalonia.Native.Interop.IAvnGlSurfaceRenderingSession BeginDrawing()
        {
            Avalonia.Native.Interop.IAvnGlSurfaceRenderingSession ret;
            System.IntPtr ret_ = System.IntPtr.Zero;
            SharpGen.Runtime.Result __result__;
            __result__ = Avalonia.Native.LocalInterop.CalliThisCallint(this._nativePointer, &ret_, (*(void ***)this._nativePointer)[3]);
            if (ret_ != System.IntPtr.Zero)
                ret = new Avalonia.Native.Interop.IAvnGlSurfaceRenderingSession(ret_);
            else
                ret = null;
            __result__.CheckError();
            return ret;
        }
    }

    [System.Runtime.InteropServices.GuidAttribute("2e2cda0a-9ae5-4f1b-8e20-081a04279f0a")]
    public partial class IAvnLoopCancellation : SharpGen.Runtime.ComObject
    {
        public IAvnLoopCancellation(System.IntPtr nativePtr): base (nativePtr)
        {
        }

        public static explicit operator IAvnLoopCancellation(System.IntPtr nativePtr) => nativePtr == System.IntPtr.Zero ? null : new IAvnLoopCancellation(nativePtr);
        /// <summary>
        /// No documentation.
        /// </summary>
        /// <unmanaged>void IAvnLoopCancellation::Cancel()</unmanaged>
        /// <unmanaged-short>IAvnLoopCancellation::Cancel</unmanaged-short>
        public unsafe void Cancel()
        {
            Avalonia.Native.LocalInterop.CalliThisCallvoid(this._nativePointer, (*(void ***)this._nativePointer)[3]);
        }
    }

    [System.Runtime.InteropServices.GuidAttribute("2e2cda0a-9ae5-4f1b-8e20-081a04279f07")]
    public partial class IAvnMacOptions : SharpGen.Runtime.ComObject
    {
        public IAvnMacOptions(System.IntPtr nativePtr): base (nativePtr)
        {
        }

        public static explicit operator IAvnMacOptions(System.IntPtr nativePtr) => nativePtr == System.IntPtr.Zero ? null : new IAvnMacOptions(nativePtr);
        /// <summary>
        /// No documentation.
        /// </summary>
        /// <unmanaged>SetShowInDock</unmanaged>
        /// <unmanaged-short>SetShowInDock</unmanaged-short>
        public System.Int32 ShowInDock
        {
            set => SetShowInDock(value);
        }

        /// <summary>
        /// No documentation.
        /// </summary>
        /// <param name = "show">No documentation.</param>
        /// <returns>No documentation.</returns>
        /// <unmanaged>HRESULT IAvnMacOptions::SetShowInDock([In] int show)</unmanaged>
        /// <unmanaged-short>IAvnMacOptions::SetShowInDock</unmanaged-short>
        internal unsafe void SetShowInDock(System.Int32 show)
        {
            SharpGen.Runtime.Result __result__;
            __result__ = Avalonia.Native.LocalInterop.CalliThisCallint(this._nativePointer, show, (*(void ***)this._nativePointer)[3]);
            __result__.CheckError();
        }
    }

    [System.Runtime.InteropServices.GuidAttribute("2e2cda0a-9ae5-4f1b-8e20-081a04279f0b")]
    public partial class IAvnPlatformThreadingInterface : SharpGen.Runtime.ComObject
    {
        public IAvnPlatformThreadingInterface(System.IntPtr nativePtr): base (nativePtr)
        {
        }

        public static explicit operator IAvnPlatformThreadingInterface(System.IntPtr nativePtr) => nativePtr == System.IntPtr.Zero ? null : new IAvnPlatformThreadingInterface(nativePtr);
        /// <summary>
        /// No documentation.
        /// </summary>
        /// <unmanaged>GetCurrentThreadIsLoopThread</unmanaged>
        /// <unmanaged-short>GetCurrentThreadIsLoopThread</unmanaged-short>
        public System.Boolean CurrentThreadIsLoopThread
        {
            get => GetCurrentThreadIsLoopThread();
        }

        /// <summary>
        /// No documentation.
        /// </summary>
        /// <unmanaged>SetSignaledCallback</unmanaged>
        /// <unmanaged-short>SetSignaledCallback</unmanaged-short>
        public Avalonia.Native.Interop.IAvnSignaledCallback SignaledCallback
        {
            set => SetSignaledCallback(value);
        }

        /// <summary>
        /// No documentation.
        /// </summary>
        /// <returns>No documentation.</returns>
        /// <unmanaged>bool IAvnPlatformThreadingInterface::GetCurrentThreadIsLoopThread()</unmanaged>
        /// <unmanaged-short>IAvnPlatformThreadingInterface::GetCurrentThreadIsLoopThread</unmanaged-short>
        internal unsafe System.Boolean GetCurrentThreadIsLoopThread()
        {
            System.Boolean __result__;
            System.Byte __result__native;
            __result__native = Avalonia.Native.LocalInterop.CalliThisCallSystemByte(this._nativePointer, (*(void ***)this._nativePointer)[3]);
            __result__ = __result__native != 0;
            return __result__;
        }

        /// <summary>
        /// No documentation.
        /// </summary>
        /// <param name = "cb">No documentation.</param>
        /// <unmanaged>void IAvnPlatformThreadingInterface::SetSignaledCallback([In] IAvnSignaledCallback* cb)</unmanaged>
        /// <unmanaged-short>IAvnPlatformThreadingInterface::SetSignaledCallback</unmanaged-short>
        internal unsafe void SetSignaledCallback(Avalonia.Native.Interop.IAvnSignaledCallback cb)
        {
            System.IntPtr cb_ = System.IntPtr.Zero;
            cb_ = SharpGen.Runtime.CppObject.ToCallbackPtr<Avalonia.Native.Interop.IAvnSignaledCallback>(cb);
            Avalonia.Native.LocalInterop.CalliThisCallvoid(this._nativePointer, (void *)cb_, (*(void ***)this._nativePointer)[4]);
        }

        /// <summary>
        /// No documentation.
        /// </summary>
        /// <returns>No documentation.</returns>
        /// <unmanaged>IAvnLoopCancellation* IAvnPlatformThreadingInterface::CreateLoopCancellation()</unmanaged>
        /// <unmanaged-short>IAvnPlatformThreadingInterface::CreateLoopCancellation</unmanaged-short>
        public unsafe Avalonia.Native.Interop.IAvnLoopCancellation CreateLoopCancellation()
        {
            Avalonia.Native.Interop.IAvnLoopCancellation __result__;
            System.IntPtr __result__native = System.IntPtr.Zero;
            __result__native = Avalonia.Native.LocalInterop.CalliThisCallSystemIntPtr(this._nativePointer, (*(void ***)this._nativePointer)[5]);
            if (__result__native != System.IntPtr.Zero)
                __result__ = new Avalonia.Native.Interop.IAvnLoopCancellation(__result__native);
            else
                __result__ = null;
            return __result__;
        }

        /// <summary>
        /// No documentation.
        /// </summary>
        /// <param name = "cancel">No documentation.</param>
        /// <unmanaged>void IAvnPlatformThreadingInterface::RunLoop([In] IAvnLoopCancellation* cancel)</unmanaged>
        /// <unmanaged-short>IAvnPlatformThreadingInterface::RunLoop</unmanaged-short>
        public unsafe void RunLoop(Avalonia.Native.Interop.IAvnLoopCancellation cancel)
        {
            System.IntPtr cancel_ = System.IntPtr.Zero;
            cancel_ = SharpGen.Runtime.CppObject.ToCallbackPtr<Avalonia.Native.Interop.IAvnLoopCancellation>(cancel);
            Avalonia.Native.LocalInterop.CalliThisCallvoid(this._nativePointer, (void *)cancel_, (*(void ***)this._nativePointer)[6]);
        }

        /// <summary>
        /// No documentation.
        /// </summary>
        /// <param name = "priority">No documentation.</param>
        /// <unmanaged>void IAvnPlatformThreadingInterface::Signal([In] int priority)</unmanaged>
        /// <unmanaged-short>IAvnPlatformThreadingInterface::Signal</unmanaged-short>
        public unsafe void Signal(System.Int32 priority)
        {
            Avalonia.Native.LocalInterop.CalliThisCallvoid(this._nativePointer, priority, (*(void ***)this._nativePointer)[7]);
        }

        /// <summary>
        /// No documentation.
        /// </summary>
        /// <param name = "priority">No documentation.</param>
        /// <param name = "ms">No documentation.</param>
        /// <param name = "callback">No documentation.</param>
        /// <returns>No documentation.</returns>
        /// <unmanaged>IUnknown* IAvnPlatformThreadingInterface::StartTimer([In] int priority,[In] int ms,[In] IAvnActionCallback* callback)</unmanaged>
        /// <unmanaged-short>IAvnPlatformThreadingInterface::StartTimer</unmanaged-short>
        public unsafe SharpGen.Runtime.ComObject StartTimer(System.Int32 priority, System.Int32 ms, Avalonia.Native.Interop.IAvnActionCallback callback)
        {
            System.IntPtr callback_ = System.IntPtr.Zero;
            SharpGen.Runtime.ComObject __result__;
            System.IntPtr __result__native = System.IntPtr.Zero;
            callback_ = SharpGen.Runtime.CppObject.ToCallbackPtr<Avalonia.Native.Interop.IAvnActionCallback>(callback);
            __result__native = Avalonia.Native.LocalInterop.CalliThisCallSystemIntPtr(this._nativePointer, priority, ms, (void *)callback_, (*(void ***)this._nativePointer)[8]);
            if (__result__native != System.IntPtr.Zero)
                __result__ = new SharpGen.Runtime.ComObject(__result__native);
            else
                __result__ = null;
            return __result__;
        }
    }

    [System.Runtime.InteropServices.GuidAttribute("2e2cda0a-9ae5-4f1b-8e20-081a04279f03")]
    public partial class IAvnPopup : Avalonia.Native.Interop.IAvnWindowBase
    {
        public IAvnPopup(System.IntPtr nativePtr): base (nativePtr)
        {
        }

        public static explicit operator IAvnPopup(System.IntPtr nativePtr) => nativePtr == System.IntPtr.Zero ? null : new IAvnPopup(nativePtr);
    }

    [System.Runtime.InteropServices.GuidAttribute("2e2cda0a-9ae5-4f1b-8e20-081a04279f0e")]
    public partial class IAvnScreens : SharpGen.Runtime.ComObject
    {
        public IAvnScreens(System.IntPtr nativePtr): base (nativePtr)
        {
        }

        public static explicit operator IAvnScreens(System.IntPtr nativePtr) => nativePtr == System.IntPtr.Zero ? null : new IAvnScreens(nativePtr);
        /// <summary>
        /// No documentation.
        /// </summary>
        /// <returns>No documentation.</returns>
        /// <unmanaged>HRESULT IAvnScreens::GetScreenCount([In] int* ret)</unmanaged>
        /// <unmanaged-short>IAvnScreens::GetScreenCount</unmanaged-short>
        public unsafe System.Int32 GetScreenCount()
        {
            System.Int32 ret;
            SharpGen.Runtime.Result __result__;
            __result__ = Avalonia.Native.LocalInterop.CalliThisCallint(this._nativePointer, &ret, (*(void ***)this._nativePointer)[3]);
            __result__.CheckError();
            return ret;
        }

        /// <summary>
        /// No documentation.
        /// </summary>
        /// <param name = "index">No documentation.</param>
        /// <returns>No documentation.</returns>
        /// <unmanaged>HRESULT IAvnScreens::GetScreen([In] int index,[In] AvnScreen* ret)</unmanaged>
        /// <unmanaged-short>IAvnScreens::GetScreen</unmanaged-short>
        public unsafe Avalonia.Native.Interop.AvnScreen GetScreen(System.Int32 index)
        {
            Avalonia.Native.Interop.AvnScreen ret;
            SharpGen.Runtime.Result __result__;
            __result__ = Avalonia.Native.LocalInterop.CalliThisCallint(this._nativePointer, index, &ret, (*(void ***)this._nativePointer)[4]);
            __result__.CheckError();
            return ret;
        }
    }

    class IAvnSignaledCallbackShadow : SharpGen.Runtime.ComObjectShadow
    {
        protected unsafe class IAvnSignaledCallbackVtbl : SharpGen.Runtime.ComObjectShadow.ComObjectVtbl
        {
            public IAvnSignaledCallbackVtbl(int numberOfCallbackMethods): base (numberOfCallbackMethods + 1)
            {
                AddMethod(new SignaledDelegate(Signaled));
            }

            [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.ThisCall)]
            private delegate void SignaledDelegate(System.IntPtr thisObject, int arg0, System.Byte arg1);
            private static unsafe void Signaled(System.IntPtr thisObject, int param0, System.Byte param1)
            {
                try
                {
                    System.Int32 priority = default (System.Int32);
                    priority = (System.Int32)param0;
                    System.Boolean priorityContainsMeaningfulValue = default (System.Boolean);
                    System.Byte priorityContainsMeaningfulValue_ = (System.Byte)param1;
                    IAvnSignaledCallback @this = (IAvnSignaledCallback)ToShadow<Avalonia.Native.Interop.IAvnSignaledCallbackShadow>(thisObject).Callback;
                    priorityContainsMeaningfulValue = priorityContainsMeaningfulValue_ != 0;
                    @this.Signaled(priority, priorityContainsMeaningfulValue);
                }
                catch (System.Exception)
                {
                }
            }
        }

        protected override SharpGen.Runtime.CppObjectVtbl Vtbl
        {
            get;
        }

        = new Avalonia.Native.Interop.IAvnSignaledCallbackShadow.IAvnSignaledCallbackVtbl(0);
    }

    [System.Runtime.InteropServices.GuidAttribute("2e2cda0a-9ae5-4f1b-8e20-081a04279f09"), SharpGen.Runtime.ShadowAttribute(typeof (Avalonia.Native.Interop.IAvnSignaledCallbackShadow))]
    public partial interface IAvnSignaledCallback : SharpGen.Runtime.IUnknown
    {
        void Signaled(System.Int32 priority, System.Boolean priorityContainsMeaningfulValue);
    }

    [System.Runtime.InteropServices.GuidAttribute("2e2cda0a-9ae5-4f1b-8e20-081a04279f17")]
    public partial class IAvnString : SharpGen.Runtime.ComObject
    {
        public IAvnString(System.IntPtr nativePtr): base (nativePtr)
        {
        }

        public static explicit operator IAvnString(System.IntPtr nativePtr) => nativePtr == System.IntPtr.Zero ? null : new IAvnString(nativePtr);
        /// <summary>
        /// No documentation.
        /// </summary>
        /// <returns>No documentation.</returns>
        /// <unmanaged>HRESULT IAvnString::Pointer([Out] void** retOut)</unmanaged>
        /// <unmanaged-short>IAvnString::Pointer</unmanaged-short>
        public unsafe System.IntPtr Pointer()
        {
            System.IntPtr retOut;
            SharpGen.Runtime.Result __result__;
            __result__ = Avalonia.Native.LocalInterop.CalliThisCallint(this._nativePointer, &retOut, (*(void ***)this._nativePointer)[3]);
            __result__.CheckError();
            return retOut;
        }

        /// <summary>
        /// No documentation.
        /// </summary>
        /// <returns>No documentation.</returns>
        /// <unmanaged>HRESULT IAvnString::Length([In] int* ret)</unmanaged>
        /// <unmanaged-short>IAvnString::Length</unmanaged-short>
        public unsafe System.Int32 Length()
        {
            System.Int32 ret;
            SharpGen.Runtime.Result __result__;
            __result__ = Avalonia.Native.LocalInterop.CalliThisCallint(this._nativePointer, &ret, (*(void ***)this._nativePointer)[4]);
            __result__.CheckError();
            return ret;
        }
    }

    class IAvnSystemDialogEventsShadow : SharpGen.Runtime.ComObjectShadow
    {
        protected unsafe class IAvnSystemDialogEventsVtbl : SharpGen.Runtime.ComObjectShadow.ComObjectVtbl
        {
            public IAvnSystemDialogEventsVtbl(int numberOfCallbackMethods): base (numberOfCallbackMethods + 1)
            {
                AddMethod(new OnCompletedDelegate(OnCompleted));
            }

            [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.ThisCall)]
            private delegate void OnCompletedDelegate(System.IntPtr thisObject, int arg0, void *arg1);
            private static unsafe void OnCompleted(System.IntPtr thisObject, int param0, void *param1)
            {
                try
                {
                    System.Int32 numResults = default (System.Int32);
                    numResults = (System.Int32)param0;
                    System.IntPtr trFirstResultRef = default (System.IntPtr);
                    trFirstResultRef = (System.IntPtr)param1;
                    IAvnSystemDialogEvents @this = (IAvnSystemDialogEvents)ToShadow<Avalonia.Native.Interop.IAvnSystemDialogEventsShadow>(thisObject).Callback;
                    @this.OnCompleted(numResults, trFirstResultRef);
                }
                catch (System.Exception)
                {
                }
            }
        }

        protected override SharpGen.Runtime.CppObjectVtbl Vtbl
        {
            get;
        }

        = new Avalonia.Native.Interop.IAvnSystemDialogEventsShadow.IAvnSystemDialogEventsVtbl(0);
    }

    [System.Runtime.InteropServices.GuidAttribute("2e2cda0a-9ae5-4f1b-8e20-081a04279f0c"), SharpGen.Runtime.ShadowAttribute(typeof (Avalonia.Native.Interop.IAvnSystemDialogEventsShadow))]
    public partial interface IAvnSystemDialogEvents : SharpGen.Runtime.IUnknown
    {
        void OnCompleted(System.Int32 numResults, System.IntPtr trFirstResultRef);
    }

    [System.Runtime.InteropServices.GuidAttribute("2e2cda0a-9ae5-4f1b-8e20-081a04279f0d")]
    public partial class IAvnSystemDialogs : SharpGen.Runtime.ComObject
    {
        public IAvnSystemDialogs(System.IntPtr nativePtr): base (nativePtr)
        {
        }

        public static explicit operator IAvnSystemDialogs(System.IntPtr nativePtr) => nativePtr == System.IntPtr.Zero ? null : new IAvnSystemDialogs(nativePtr);
        /// <summary>
        /// No documentation.
        /// </summary>
        /// <param name = "arentWindowHandleRef">No documentation.</param>
        /// <param name = "events">No documentation.</param>
        /// <param name = "title">No documentation.</param>
        /// <param name = "initialPath">No documentation.</param>
        /// <unmanaged>void IAvnSystemDialogs::SelectFolderDialog([In] IAvnWindow* parentWindowHandle,[In] IAvnSystemDialogEvents* events,[In] const char* title,[In] const char* initialPath)</unmanaged>
        /// <unmanaged-short>IAvnSystemDialogs::SelectFolderDialog</unmanaged-short>
        public unsafe void SelectFolderDialog(Avalonia.Native.Interop.IAvnWindow arentWindowHandleRef, Avalonia.Native.Interop.IAvnSystemDialogEvents events, System.String title, System.String initialPath)
        {
            System.IntPtr arentWindowHandleRef_ = System.IntPtr.Zero;
            System.IntPtr events_ = System.IntPtr.Zero;
            System.IntPtr title_;
            System.IntPtr initialPath_;
            arentWindowHandleRef_ = SharpGen.Runtime.CppObject.ToCallbackPtr<Avalonia.Native.Interop.IAvnWindow>(arentWindowHandleRef);
            events_ = SharpGen.Runtime.CppObject.ToCallbackPtr<Avalonia.Native.Interop.IAvnSystemDialogEvents>(events);
            title_ = System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi(title);
            initialPath_ = System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi(initialPath);
            Avalonia.Native.LocalInterop.CalliThisCallvoid(this._nativePointer, (void *)arentWindowHandleRef_, (void *)events_, (void *)title_, (void *)initialPath_, (*(void ***)this._nativePointer)[3]);
            System.Runtime.InteropServices.Marshal.FreeHGlobal(title_);
            System.Runtime.InteropServices.Marshal.FreeHGlobal(initialPath_);
        }

        /// <summary>
        /// No documentation.
        /// </summary>
        /// <param name = "arentWindowHandleRef">No documentation.</param>
        /// <param name = "events">No documentation.</param>
        /// <param name = "allowMultiple">No documentation.</param>
        /// <param name = "title">No documentation.</param>
        /// <param name = "initialDirectory">No documentation.</param>
        /// <param name = "initialFile">No documentation.</param>
        /// <param name = "filters">No documentation.</param>
        /// <unmanaged>void IAvnSystemDialogs::OpenFileDialog([In] IAvnWindow* parentWindowHandle,[In] IAvnSystemDialogEvents* events,[In] bool allowMultiple,[In] const char* title,[In] const char* initialDirectory,[In] const char* initialFile,[In] const char* filters)</unmanaged>
        /// <unmanaged-short>IAvnSystemDialogs::OpenFileDialog</unmanaged-short>
        public unsafe void OpenFileDialog(Avalonia.Native.Interop.IAvnWindow arentWindowHandleRef, Avalonia.Native.Interop.IAvnSystemDialogEvents events, System.Boolean allowMultiple, System.String title, System.String initialDirectory, System.String initialFile, System.String filters)
        {
            System.IntPtr arentWindowHandleRef_ = System.IntPtr.Zero;
            System.IntPtr events_ = System.IntPtr.Zero;
            System.Byte allowMultiple_;
            System.IntPtr title_;
            System.IntPtr initialDirectory_;
            System.IntPtr initialFile_;
            System.IntPtr filters_;
            arentWindowHandleRef_ = SharpGen.Runtime.CppObject.ToCallbackPtr<Avalonia.Native.Interop.IAvnWindow>(arentWindowHandleRef);
            events_ = SharpGen.Runtime.CppObject.ToCallbackPtr<Avalonia.Native.Interop.IAvnSystemDialogEvents>(events);
            allowMultiple_ = (System.Byte)(allowMultiple ? 1 : 0);
            title_ = System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi(title);
            initialDirectory_ = System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi(initialDirectory);
            initialFile_ = System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi(initialFile);
            filters_ = System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi(filters);
            Avalonia.Native.LocalInterop.CalliThisCallvoid(this._nativePointer, (void *)arentWindowHandleRef_, (void *)events_, allowMultiple_, (void *)title_, (void *)initialDirectory_, (void *)initialFile_, (void *)filters_, (*(void ***)this._nativePointer)[4]);
            System.Runtime.InteropServices.Marshal.FreeHGlobal(title_);
            System.Runtime.InteropServices.Marshal.FreeHGlobal(initialDirectory_);
            System.Runtime.InteropServices.Marshal.FreeHGlobal(initialFile_);
            System.Runtime.InteropServices.Marshal.FreeHGlobal(filters_);
        }

        /// <summary>
        /// No documentation.
        /// </summary>
        /// <param name = "arentWindowHandleRef">No documentation.</param>
        /// <param name = "events">No documentation.</param>
        /// <param name = "title">No documentation.</param>
        /// <param name = "initialDirectory">No documentation.</param>
        /// <param name = "initialFile">No documentation.</param>
        /// <param name = "filters">No documentation.</param>
        /// <unmanaged>void IAvnSystemDialogs::SaveFileDialog([In] IAvnWindow* parentWindowHandle,[In] IAvnSystemDialogEvents* events,[In] const char* title,[In] const char* initialDirectory,[In] const char* initialFile,[In] const char* filters)</unmanaged>
        /// <unmanaged-short>IAvnSystemDialogs::SaveFileDialog</unmanaged-short>
        public unsafe void SaveFileDialog(Avalonia.Native.Interop.IAvnWindow arentWindowHandleRef, Avalonia.Native.Interop.IAvnSystemDialogEvents events, System.String title, System.String initialDirectory, System.String initialFile, System.String filters)
        {
            System.IntPtr arentWindowHandleRef_ = System.IntPtr.Zero;
            System.IntPtr events_ = System.IntPtr.Zero;
            System.IntPtr title_;
            System.IntPtr initialDirectory_;
            System.IntPtr initialFile_;
            System.IntPtr filters_;
            arentWindowHandleRef_ = SharpGen.Runtime.CppObject.ToCallbackPtr<Avalonia.Native.Interop.IAvnWindow>(arentWindowHandleRef);
            events_ = SharpGen.Runtime.CppObject.ToCallbackPtr<Avalonia.Native.Interop.IAvnSystemDialogEvents>(events);
            title_ = System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi(title);
            initialDirectory_ = System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi(initialDirectory);
            initialFile_ = System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi(initialFile);
            filters_ = System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi(filters);
            Avalonia.Native.LocalInterop.CalliThisCallvoid(this._nativePointer, (void *)arentWindowHandleRef_, (void *)events_, (void *)title_, (void *)initialDirectory_, (void *)initialFile_, (void *)filters_, (*(void ***)this._nativePointer)[5]);
            System.Runtime.InteropServices.Marshal.FreeHGlobal(title_);
            System.Runtime.InteropServices.Marshal.FreeHGlobal(initialDirectory_);
            System.Runtime.InteropServices.Marshal.FreeHGlobal(initialFile_);
            System.Runtime.InteropServices.Marshal.FreeHGlobal(filters_);
        }
    }

    [System.Runtime.InteropServices.GuidAttribute("2e2cda0a-9ae5-4f1b-8e20-081a04279f04")]
    public partial class IAvnWindow : Avalonia.Native.Interop.IAvnWindowBase
    {
        public IAvnWindow(System.IntPtr nativePtr): base (nativePtr)
        {
        }

        public static explicit operator IAvnWindow(System.IntPtr nativePtr) => nativePtr == System.IntPtr.Zero ? null : new IAvnWindow(nativePtr);
        /// <summary>
        /// No documentation.
        /// </summary>
        /// <unmanaged>SetCanResize</unmanaged>
        /// <unmanaged-short>SetCanResize</unmanaged-short>
        public System.Boolean CanResize
        {
            set => SetCanResize(value);
        }

        /// <summary>
        /// No documentation.
        /// </summary>
        /// <unmanaged>SetHasDecorations</unmanaged>
        /// <unmanaged-short>SetHasDecorations</unmanaged-short>
        public System.Boolean HasDecorations
        {
            set => SetHasDecorations(value);
        }

        /// <summary>
        /// No documentation.
        /// </summary>
        /// <unmanaged>SetTitle</unmanaged>
        /// <unmanaged-short>SetTitle</unmanaged-short>
        public System.IntPtr Title
        {
            set => SetTitle(value);
        }

        /// <summary>
        /// No documentation.
        /// </summary>
        /// <unmanaged>SetTitleBarColor</unmanaged>
        /// <unmanaged-short>SetTitleBarColor</unmanaged-short>
        public Avalonia.Native.Interop.AvnColor TitleBarColor
        {
            set => SetTitleBarColor(value);
        }

        /// <summary>
        /// No documentation.
        /// </summary>
        /// <param name = "arentRef">No documentation.</param>
        /// <returns>No documentation.</returns>
        /// <unmanaged>HRESULT IAvnWindow::ShowDialog([In] IAvnWindow* parent)</unmanaged>
        /// <unmanaged-short>IAvnWindow::ShowDialog</unmanaged-short>
        public unsafe void ShowDialog(Avalonia.Native.Interop.IAvnWindow arentRef)
        {
            System.IntPtr arentRef_ = System.IntPtr.Zero;
            SharpGen.Runtime.Result __result__;
            arentRef_ = SharpGen.Runtime.CppObject.ToCallbackPtr<Avalonia.Native.Interop.IAvnWindow>(arentRef);
            __result__ = Avalonia.Native.LocalInterop.CalliThisCallint(this._nativePointer, (void *)arentRef_, (*(void ***)this._nativePointer)[26]);
            __result__.CheckError();
        }

        /// <summary>
        /// No documentation.
        /// </summary>
        /// <param name = "value">No documentation.</param>
        /// <returns>No documentation.</returns>
        /// <unmanaged>HRESULT IAvnWindow::SetCanResize([In] bool value)</unmanaged>
        /// <unmanaged-short>IAvnWindow::SetCanResize</unmanaged-short>
        internal unsafe void SetCanResize(System.Boolean value)
        {
            System.Byte value_;
            SharpGen.Runtime.Result __result__;
            value_ = (System.Byte)(value ? 1 : 0);
            __result__ = Avalonia.Native.LocalInterop.CalliThisCallint(this._nativePointer, value_, (*(void ***)this._nativePointer)[27]);
            __result__.CheckError();
        }

        /// <summary>
        /// No documentation.
        /// </summary>
        /// <param name = "value">No documentation.</param>
        /// <returns>No documentation.</returns>
        /// <unmanaged>HRESULT IAvnWindow::SetHasDecorations([In] bool value)</unmanaged>
        /// <unmanaged-short>IAvnWindow::SetHasDecorations</unmanaged-short>
        internal unsafe void SetHasDecorations(System.Boolean value)
        {
            System.Byte value_;
            SharpGen.Runtime.Result __result__;
            value_ = (System.Byte)(value ? 1 : 0);
            __result__ = Avalonia.Native.LocalInterop.CalliThisCallint(this._nativePointer, value_, (*(void ***)this._nativePointer)[28]);
            __result__.CheckError();
        }

        /// <summary>
        /// No documentation.
        /// </summary>
        /// <param name = "utf8Title">No documentation.</param>
        /// <returns>No documentation.</returns>
        /// <unmanaged>HRESULT IAvnWindow::SetTitle([In] void* utf8Title)</unmanaged>
        /// <unmanaged-short>IAvnWindow::SetTitle</unmanaged-short>
        internal unsafe void SetTitle(System.IntPtr utf8Title)
        {
            SharpGen.Runtime.Result __result__;
            __result__ = Avalonia.Native.LocalInterop.CalliThisCallint(this._nativePointer, (void *)utf8Title, (*(void ***)this._nativePointer)[29]);
            __result__.CheckError();
        }

        /// <summary>
        /// No documentation.
        /// </summary>
        /// <param name = "color">No documentation.</param>
        /// <returns>No documentation.</returns>
        /// <unmanaged>HRESULT IAvnWindow::SetTitleBarColor([In] AvnColor color)</unmanaged>
        /// <unmanaged-short>IAvnWindow::SetTitleBarColor</unmanaged-short>
        internal unsafe void SetTitleBarColor(Avalonia.Native.Interop.AvnColor color)
        {
            SharpGen.Runtime.Result __result__;
            __result__ = Avalonia.Native.LocalInterop.CalliThisCallint0(this._nativePointer, color, (*(void ***)this._nativePointer)[30]);
            __result__.CheckError();
        }

        /// <summary>
        /// No documentation.
        /// </summary>
        /// <param name = "state">No documentation.</param>
        /// <returns>No documentation.</returns>
        /// <unmanaged>HRESULT IAvnWindow::SetWindowState([In] AvnWindowState state)</unmanaged>
        /// <unmanaged-short>IAvnWindow::SetWindowState</unmanaged-short>
        public unsafe void SetWindowState(Avalonia.Native.Interop.AvnWindowState state)
        {
            SharpGen.Runtime.Result __result__;
            __result__ = Avalonia.Native.LocalInterop.CalliThisCallint(this._nativePointer, unchecked ((System.Int32)state), (*(void ***)this._nativePointer)[31]);
            __result__.CheckError();
        }

        /// <summary>
        /// No documentation.
        /// </summary>
        /// <returns>No documentation.</returns>
        /// <unmanaged>HRESULT IAvnWindow::GetWindowState([In] AvnWindowState* ret)</unmanaged>
        /// <unmanaged-short>IAvnWindow::GetWindowState</unmanaged-short>
        public unsafe Avalonia.Native.Interop.AvnWindowState GetWindowState()
        {
            Avalonia.Native.Interop.AvnWindowState ret;
            SharpGen.Runtime.Result __result__;
            __result__ = Avalonia.Native.LocalInterop.CalliThisCallint(this._nativePointer, &ret, (*(void ***)this._nativePointer)[32]);
            __result__.CheckError();
            return ret;
        }
    }

    [System.Runtime.InteropServices.GuidAttribute("2e2cda0a-9ae5-4f1b-8e20-081a04279f02")]
    public partial class IAvnWindowBase : SharpGen.Runtime.ComObject
    {
        public IAvnWindowBase(System.IntPtr nativePtr): base (nativePtr)
        {
        }

        public static explicit operator IAvnWindowBase(System.IntPtr nativePtr) => nativePtr == System.IntPtr.Zero ? null : new IAvnWindowBase(nativePtr);
        /// <summary>
        /// No documentation.
        /// </summary>
        /// <unmanaged>SetTopMost</unmanaged>
        /// <unmanaged-short>SetTopMost</unmanaged-short>
        public System.Boolean TopMost
        {
            set => SetTopMost(value);
        }

        /// <summary>
        /// No documentation.
        /// </summary>
        /// <unmanaged>SetCursor</unmanaged>
        /// <unmanaged-short>SetCursor</unmanaged-short>
        public Avalonia.Native.Interop.IAvnCursor Cursor
        {
            set => SetCursor(value);
        }

        /// <summary>
        /// No documentation.
        /// </summary>
        /// <returns>No documentation.</returns>
        /// <unmanaged>HRESULT IAvnWindowBase::Show()</unmanaged>
        /// <unmanaged-short>IAvnWindowBase::Show</unmanaged-short>
        public unsafe void Show()
        {
            SharpGen.Runtime.Result __result__;
            __result__ = Avalonia.Native.LocalInterop.CalliThisCallint(this._nativePointer, (*(void ***)this._nativePointer)[3]);
            __result__.CheckError();
        }

        /// <summary>
        /// No documentation.
        /// </summary>
        /// <returns>No documentation.</returns>
        /// <unmanaged>HRESULT IAvnWindowBase::Hide()</unmanaged>
        /// <unmanaged-short>IAvnWindowBase::Hide</unmanaged-short>
        public unsafe void Hide()
        {
            SharpGen.Runtime.Result __result__;
            __result__ = Avalonia.Native.LocalInterop.CalliThisCallint(this._nativePointer, (*(void ***)this._nativePointer)[4]);
            __result__.CheckError();
        }

        /// <summary>
        /// No documentation.
        /// </summary>
        /// <returns>No documentation.</returns>
        /// <unmanaged>HRESULT IAvnWindowBase::Close()</unmanaged>
        /// <unmanaged-short>IAvnWindowBase::Close</unmanaged-short>
        public unsafe void Close()
        {
            SharpGen.Runtime.Result __result__;
            __result__ = Avalonia.Native.LocalInterop.CalliThisCallint(this._nativePointer, (*(void ***)this._nativePointer)[5]);
            __result__.CheckError();
        }

        /// <summary>
        /// No documentation.
        /// </summary>
        /// <returns>No documentation.</returns>
        /// <unmanaged>HRESULT IAvnWindowBase::Activate()</unmanaged>
        /// <unmanaged-short>IAvnWindowBase::Activate</unmanaged-short>
        public unsafe void Activate()
        {
            SharpGen.Runtime.Result __result__;
            __result__ = Avalonia.Native.LocalInterop.CalliThisCallint(this._nativePointer, (*(void ***)this._nativePointer)[6]);
            __result__.CheckError();
        }

        /// <summary>
        /// No documentation.
        /// </summary>
        /// <returns>No documentation.</returns>
        /// <unmanaged>HRESULT IAvnWindowBase::GetClientSize([In] AvnSize* ret)</unmanaged>
        /// <unmanaged-short>IAvnWindowBase::GetClientSize</unmanaged-short>
        public unsafe Avalonia.Native.Interop.AvnSize GetClientSize()
        {
            Avalonia.Native.Interop.AvnSize ret;
            SharpGen.Runtime.Result __result__;
            __result__ = Avalonia.Native.LocalInterop.CalliThisCallint(this._nativePointer, &ret, (*(void ***)this._nativePointer)[7]);
            __result__.CheckError();
            return ret;
        }

        /// <summary>
        /// No documentation.
        /// </summary>
        /// <returns>No documentation.</returns>
        /// <unmanaged>HRESULT IAvnWindowBase::GetMaxClientSize([In] AvnSize* ret)</unmanaged>
        /// <unmanaged-short>IAvnWindowBase::GetMaxClientSize</unmanaged-short>
        public unsafe Avalonia.Native.Interop.AvnSize GetMaxClientSize()
        {
            Avalonia.Native.Interop.AvnSize ret;
            SharpGen.Runtime.Result __result__;
            __result__ = Avalonia.Native.LocalInterop.CalliThisCallint(this._nativePointer, &ret, (*(void ***)this._nativePointer)[8]);
            __result__.CheckError();
            return ret;
        }

        /// <summary>
        /// No documentation.
        /// </summary>
        /// <returns>No documentation.</returns>
        /// <unmanaged>HRESULT IAvnWindowBase::GetScaling([In] double* ret)</unmanaged>
        /// <unmanaged-short>IAvnWindowBase::GetScaling</unmanaged-short>
        public unsafe System.Double GetScaling()
        {
            System.Double ret;
            SharpGen.Runtime.Result __result__;
            __result__ = Avalonia.Native.LocalInterop.CalliThisCallint(this._nativePointer, &ret, (*(void ***)this._nativePointer)[9]);
            __result__.CheckError();
            return ret;
        }

        /// <summary>
        /// No documentation.
        /// </summary>
        /// <param name = "minSize">No documentation.</param>
        /// <param name = "maxSize">No documentation.</param>
        /// <returns>No documentation.</returns>
        /// <unmanaged>HRESULT IAvnWindowBase::SetMinMaxSize([In] AvnSize minSize,[In] AvnSize maxSize)</unmanaged>
        /// <unmanaged-short>IAvnWindowBase::SetMinMaxSize</unmanaged-short>
        public unsafe void SetMinMaxSize(Avalonia.Native.Interop.AvnSize minSize, Avalonia.Native.Interop.AvnSize maxSize)
        {
            SharpGen.Runtime.Result __result__;
            __result__ = Avalonia.Native.LocalInterop.CalliThisCallint0(this._nativePointer, minSize, maxSize, (*(void ***)this._nativePointer)[10]);
            __result__.CheckError();
        }

        /// <summary>
        /// No documentation.
        /// </summary>
        /// <param name = "width">No documentation.</param>
        /// <param name = "height">No documentation.</param>
        /// <returns>No documentation.</returns>
        /// <unmanaged>HRESULT IAvnWindowBase::Resize([In] double width,[In] double height)</unmanaged>
        /// <unmanaged-short>IAvnWindowBase::Resize</unmanaged-short>
        public unsafe void Resize(System.Double width, System.Double height)
        {
            SharpGen.Runtime.Result __result__;
            __result__ = Avalonia.Native.LocalInterop.CalliThisCallint(this._nativePointer, width, height, (*(void ***)this._nativePointer)[11]);
            __result__.CheckError();
        }

        /// <summary>
        /// No documentation.
        /// </summary>
        /// <param name = "rect">No documentation.</param>
        /// <returns>No documentation.</returns>
        /// <unmanaged>HRESULT IAvnWindowBase::Invalidate([In] AvnRect rect)</unmanaged>
        /// <unmanaged-short>IAvnWindowBase::Invalidate</unmanaged-short>
        public unsafe void Invalidate(Avalonia.Native.Interop.AvnRect rect)
        {
            SharpGen.Runtime.Result __result__;
            __result__ = Avalonia.Native.LocalInterop.CalliThisCallint0(this._nativePointer, rect, (*(void ***)this._nativePointer)[12]);
            __result__.CheckError();
        }

        /// <summary>
        /// No documentation.
        /// </summary>
        /// <returns>No documentation.</returns>
        /// <unmanaged>HRESULT IAvnWindowBase::BeginMoveDrag()</unmanaged>
        /// <unmanaged-short>IAvnWindowBase::BeginMoveDrag</unmanaged-short>
        public unsafe void BeginMoveDrag()
        {
            SharpGen.Runtime.Result __result__;
            __result__ = Avalonia.Native.LocalInterop.CalliThisCallint(this._nativePointer, (*(void ***)this._nativePointer)[13]);
            __result__.CheckError();
        }

        /// <summary>
        /// No documentation.
        /// </summary>
        /// <param name = "edge">No documentation.</param>
        /// <returns>No documentation.</returns>
        /// <unmanaged>HRESULT IAvnWindowBase::BeginResizeDrag([In] AvnWindowEdge edge)</unmanaged>
        /// <unmanaged-short>IAvnWindowBase::BeginResizeDrag</unmanaged-short>
        public unsafe void BeginResizeDrag(Avalonia.Native.Interop.AvnWindowEdge edge)
        {
            SharpGen.Runtime.Result __result__;
            __result__ = Avalonia.Native.LocalInterop.CalliThisCallint(this._nativePointer, unchecked ((System.Int32)edge), (*(void ***)this._nativePointer)[14]);
            __result__.CheckError();
        }

        /// <summary>
        /// No documentation.
        /// </summary>
        /// <returns>No documentation.</returns>
        /// <unmanaged>HRESULT IAvnWindowBase::GetPosition([In] AvnPoint* ret)</unmanaged>
        /// <unmanaged-short>IAvnWindowBase::GetPosition</unmanaged-short>
        public unsafe Avalonia.Native.Interop.AvnPoint GetPosition()
        {
            Avalonia.Native.Interop.AvnPoint ret;
            SharpGen.Runtime.Result __result__;
            __result__ = Avalonia.Native.LocalInterop.CalliThisCallint(this._nativePointer, &ret, (*(void ***)this._nativePointer)[15]);
            __result__.CheckError();
            return ret;
        }

        /// <summary>
        /// No documentation.
        /// </summary>
        /// <param name = "point">No documentation.</param>
        /// <returns>No documentation.</returns>
        /// <unmanaged>HRESULT IAvnWindowBase::SetPosition([In] AvnPoint point)</unmanaged>
        /// <unmanaged-short>IAvnWindowBase::SetPosition</unmanaged-short>
        public unsafe void SetPosition(Avalonia.Native.Interop.AvnPoint point)
        {
            SharpGen.Runtime.Result __result__;
            __result__ = Avalonia.Native.LocalInterop.CalliThisCallint0(this._nativePointer, point, (*(void ***)this._nativePointer)[16]);
            __result__.CheckError();
        }

        /// <summary>
        /// No documentation.
        /// </summary>
        /// <param name = "point">No documentation.</param>
        /// <returns>No documentation.</returns>
        /// <unmanaged>HRESULT IAvnWindowBase::PointToClient([In] AvnPoint point,[In] AvnPoint* ret)</unmanaged>
        /// <unmanaged-short>IAvnWindowBase::PointToClient</unmanaged-short>
        public unsafe Avalonia.Native.Interop.AvnPoint PointToClient(Avalonia.Native.Interop.AvnPoint point)
        {
            Avalonia.Native.Interop.AvnPoint ret;
            SharpGen.Runtime.Result __result__;
            __result__ = Avalonia.Native.LocalInterop.CalliThisCallint0(this._nativePointer, point, &ret, (*(void ***)this._nativePointer)[17]);
            __result__.CheckError();
            return ret;
        }

        /// <summary>
        /// No documentation.
        /// </summary>
        /// <param name = "point">No documentation.</param>
        /// <returns>No documentation.</returns>
        /// <unmanaged>HRESULT IAvnWindowBase::PointToScreen([In] AvnPoint point,[In] AvnPoint* ret)</unmanaged>
        /// <unmanaged-short>IAvnWindowBase::PointToScreen</unmanaged-short>
        public unsafe Avalonia.Native.Interop.AvnPoint PointToScreen(Avalonia.Native.Interop.AvnPoint point)
        {
            Avalonia.Native.Interop.AvnPoint ret;
            SharpGen.Runtime.Result __result__;
            __result__ = Avalonia.Native.LocalInterop.CalliThisCallint0(this._nativePointer, point, &ret, (*(void ***)this._nativePointer)[18]);
            __result__.CheckError();
            return ret;
        }

        /// <summary>
        /// No documentation.
        /// </summary>
        /// <param name = "fb">No documentation.</param>
        /// <param name = "dispose">No documentation.</param>
        /// <returns>No documentation.</returns>
        /// <unmanaged>HRESULT IAvnWindowBase::ThreadSafeSetSwRenderedFrame([In] AvnFramebuffer* fb,[In] IUnknown* dispose)</unmanaged>
        /// <unmanaged-short>IAvnWindowBase::ThreadSafeSetSwRenderedFrame</unmanaged-short>
        public unsafe void ThreadSafeSetSwRenderedFrame(ref Avalonia.Native.Interop.AvnFramebuffer fb, SharpGen.Runtime.IUnknown dispose)
        {
            System.IntPtr dispose_ = System.IntPtr.Zero;
            SharpGen.Runtime.Result __result__;
            dispose_ = SharpGen.Runtime.CppObject.ToCallbackPtr<SharpGen.Runtime.IUnknown>(dispose);
            fixed (void *fb_ = &fb)
                __result__ = Avalonia.Native.LocalInterop.CalliThisCallint(this._nativePointer, fb_, (void *)dispose_, (*(void ***)this._nativePointer)[19]);
            __result__.CheckError();
        }

        /// <summary>
        /// No documentation.
        /// </summary>
        /// <param name = "value">No documentation.</param>
        /// <returns>No documentation.</returns>
        /// <unmanaged>HRESULT IAvnWindowBase::SetTopMost([In] bool value)</unmanaged>
        /// <unmanaged-short>IAvnWindowBase::SetTopMost</unmanaged-short>
        internal unsafe void SetTopMost(System.Boolean value)
        {
            System.Byte value_;
            SharpGen.Runtime.Result __result__;
            value_ = (System.Byte)(value ? 1 : 0);
            __result__ = Avalonia.Native.LocalInterop.CalliThisCallint(this._nativePointer, value_, (*(void ***)this._nativePointer)[20]);
            __result__.CheckError();
        }

        /// <summary>
        /// No documentation.
        /// </summary>
        /// <param name = "cursor">No documentation.</param>
        /// <returns>No documentation.</returns>
        /// <unmanaged>HRESULT IAvnWindowBase::SetCursor([In] IAvnCursor* cursor)</unmanaged>
        /// <unmanaged-short>IAvnWindowBase::SetCursor</unmanaged-short>
        internal unsafe void SetCursor(Avalonia.Native.Interop.IAvnCursor cursor)
        {
            System.IntPtr cursor_ = System.IntPtr.Zero;
            SharpGen.Runtime.Result __result__;
            cursor_ = SharpGen.Runtime.CppObject.ToCallbackPtr<Avalonia.Native.Interop.IAvnCursor>(cursor);
            __result__ = Avalonia.Native.LocalInterop.CalliThisCallint(this._nativePointer, (void *)cursor_, (*(void ***)this._nativePointer)[21]);
            __result__.CheckError();
        }

        /// <summary>
        /// No documentation.
        /// </summary>
        /// <returns>No documentation.</returns>
        /// <unmanaged>HRESULT IAvnWindowBase::CreateGlRenderTarget([In] IAvnGlSurfaceRenderTarget** ret)</unmanaged>
        /// <unmanaged-short>IAvnWindowBase::CreateGlRenderTarget</unmanaged-short>
        public unsafe Avalonia.Native.Interop.IAvnGlSurfaceRenderTarget CreateGlRenderTarget()
        {
            Avalonia.Native.Interop.IAvnGlSurfaceRenderTarget ret;
            System.IntPtr ret_ = System.IntPtr.Zero;
            SharpGen.Runtime.Result __result__;
            __result__ = Avalonia.Native.LocalInterop.CalliThisCallint(this._nativePointer, &ret_, (*(void ***)this._nativePointer)[22]);
            if (ret_ != System.IntPtr.Zero)
                ret = new Avalonia.Native.Interop.IAvnGlSurfaceRenderTarget(ret_);
            else
                ret = null;
            __result__.CheckError();
            return ret;
        }

        /// <summary>
        /// No documentation.
        /// </summary>
        /// <returns>No documentation.</returns>
        /// <unmanaged>HRESULT IAvnWindowBase::GetSoftwareFramebuffer([In] AvnFramebuffer* ret)</unmanaged>
        /// <unmanaged-short>IAvnWindowBase::GetSoftwareFramebuffer</unmanaged-short>
        public unsafe Avalonia.Native.Interop.AvnFramebuffer GetSoftwareFramebuffer()
        {
            Avalonia.Native.Interop.AvnFramebuffer ret;
            SharpGen.Runtime.Result __result__;
            __result__ = Avalonia.Native.LocalInterop.CalliThisCallint(this._nativePointer, &ret, (*(void ***)this._nativePointer)[23]);
            __result__.CheckError();
            return ret;
        }

        /// <summary>
        /// No documentation.
        /// </summary>
        /// <returns>No documentation.</returns>
        /// <unmanaged>bool IAvnWindowBase::TryLock()</unmanaged>
        /// <unmanaged-short>IAvnWindowBase::TryLock</unmanaged-short>
        public unsafe System.Boolean TryLock()
        {
            System.Boolean __result__;
            System.Byte __result__native;
            __result__native = Avalonia.Native.LocalInterop.CalliThisCallSystemByte(this._nativePointer, (*(void ***)this._nativePointer)[24]);
            __result__ = __result__native != 0;
            return __result__;
        }

        /// <summary>
        /// No documentation.
        /// </summary>
        /// <unmanaged>void IAvnWindowBase::Unlock()</unmanaged>
        /// <unmanaged-short>IAvnWindowBase::Unlock</unmanaged-short>
        public unsafe void Unlock()
        {
            Avalonia.Native.LocalInterop.CalliThisCallvoid(this._nativePointer, (*(void ***)this._nativePointer)[25]);
        }
    }

    class IAvnWindowBaseEventsShadow : SharpGen.Runtime.ComObjectShadow
    {
        protected unsafe class IAvnWindowBaseEventsVtbl : SharpGen.Runtime.ComObjectShadow.ComObjectVtbl
        {
            public IAvnWindowBaseEventsVtbl(int numberOfCallbackMethods): base (numberOfCallbackMethods + 11)
            {
                AddMethod(new PaintDelegate(Paint));
                AddMethod(new ClosedDelegate(Closed));
                AddMethod(new ActivatedDelegate(Activated));
                AddMethod(new DeactivatedDelegate(Deactivated));
                AddMethod(new ResizedDelegate(Resized));
                AddMethod(new PositionChangedDelegate(PositionChanged));
                AddMethod(new RawMouseEventDelegate(RawMouseEvent));
                AddMethod(new RawKeyEventDelegate(RawKeyEvent));
                AddMethod(new RawTextInputEventDelegate(RawTextInputEvent));
                AddMethod(new ScalingChangedDelegate(ScalingChanged));
                AddMethod(new RunRenderPriorityJobsDelegate(RunRenderPriorityJobs));
            }

            [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.ThisCall)]
            private delegate int PaintDelegate(System.IntPtr thisObject);
            private static unsafe int Paint(System.IntPtr thisObject)
            {
                try
                {
                    IAvnWindowBaseEvents @this = (IAvnWindowBaseEvents)ToShadow<Avalonia.Native.Interop.IAvnWindowBaseEventsShadow>(thisObject).Callback;
                    @this.Paint();
                    return SharpGen.Runtime.Result.Ok.Code;
                }
                catch (System.Exception __exception__)
                {
                    return SharpGen.Runtime.Result.GetResultFromException(__exception__).Code;
                }
            }

            [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.ThisCall)]
            private delegate void ClosedDelegate(System.IntPtr thisObject);
            private static unsafe void Closed(System.IntPtr thisObject)
            {
                try
                {
                    IAvnWindowBaseEvents @this = (IAvnWindowBaseEvents)ToShadow<Avalonia.Native.Interop.IAvnWindowBaseEventsShadow>(thisObject).Callback;
                    @this.Closed();
                }
                catch (System.Exception)
                {
                }
            }

            [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.ThisCall)]
            private delegate void ActivatedDelegate(System.IntPtr thisObject);
            private static unsafe void Activated(System.IntPtr thisObject)
            {
                try
                {
                    IAvnWindowBaseEvents @this = (IAvnWindowBaseEvents)ToShadow<Avalonia.Native.Interop.IAvnWindowBaseEventsShadow>(thisObject).Callback;
                    @this.Activated();
                }
                catch (System.Exception)
                {
                }
            }

            [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.ThisCall)]
            private delegate void DeactivatedDelegate(System.IntPtr thisObject);
            private static unsafe void Deactivated(System.IntPtr thisObject)
            {
                try
                {
                    IAvnWindowBaseEvents @this = (IAvnWindowBaseEvents)ToShadow<Avalonia.Native.Interop.IAvnWindowBaseEventsShadow>(thisObject).Callback;
                    @this.Deactivated();
                }
                catch (System.Exception)
                {
                }
            }

            [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.ThisCall)]
            private delegate void ResizedDelegate(System.IntPtr thisObject, void *arg0);
            private static unsafe void Resized(System.IntPtr thisObject, void *param0)
            {
                try
                {
                    Avalonia.Native.Interop.AvnSize size = System.Runtime.CompilerServices.Unsafe.AsRef<Avalonia.Native.Interop.AvnSize>(param0);
                    IAvnWindowBaseEvents @this = (IAvnWindowBaseEvents)ToShadow<Avalonia.Native.Interop.IAvnWindowBaseEventsShadow>(thisObject).Callback;
                    @this.Resized(size);
                }
                catch (System.Exception)
                {
                }
            }

            [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.ThisCall)]
            private delegate void PositionChangedDelegate(System.IntPtr thisObject, Avalonia.Native.Interop.AvnPoint arg0);
            private static unsafe void PositionChanged(System.IntPtr thisObject, Avalonia.Native.Interop.AvnPoint param0)
            {
                try
                {
                    Avalonia.Native.Interop.AvnPoint position = default (Avalonia.Native.Interop.AvnPoint);
                    position = (Avalonia.Native.Interop.AvnPoint)param0;
                    IAvnWindowBaseEvents @this = (IAvnWindowBaseEvents)ToShadow<Avalonia.Native.Interop.IAvnWindowBaseEventsShadow>(thisObject).Callback;
                    @this.PositionChanged(position);
                }
                catch (System.Exception)
                {
                }
            }

            [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.ThisCall)]
            private delegate void RawMouseEventDelegate(System.IntPtr thisObject, int arg0, System.UInt32 arg1, int arg2, Avalonia.Native.Interop.AvnPoint arg3, Avalonia.Native.Interop.AvnVector arg4);
            private static unsafe void RawMouseEvent(System.IntPtr thisObject, int param0, System.UInt32 param1, int param2, Avalonia.Native.Interop.AvnPoint param3, Avalonia.Native.Interop.AvnVector param4)
            {
                try
                {
                    Avalonia.Native.Interop.AvnRawMouseEventType type = default (Avalonia.Native.Interop.AvnRawMouseEventType);
                    type = (Avalonia.Native.Interop.AvnRawMouseEventType)param0;
                    System.UInt32 timeStamp = default (System.UInt32);
                    timeStamp = (System.UInt32)param1;
                    Avalonia.Native.Interop.AvnInputModifiers modifiers = default (Avalonia.Native.Interop.AvnInputModifiers);
                    modifiers = (Avalonia.Native.Interop.AvnInputModifiers)param2;
                    Avalonia.Native.Interop.AvnPoint point = default (Avalonia.Native.Interop.AvnPoint);
                    point = (Avalonia.Native.Interop.AvnPoint)param3;
                    Avalonia.Native.Interop.AvnVector delta = default (Avalonia.Native.Interop.AvnVector);
                    delta = (Avalonia.Native.Interop.AvnVector)param4;
                    IAvnWindowBaseEvents @this = (IAvnWindowBaseEvents)ToShadow<Avalonia.Native.Interop.IAvnWindowBaseEventsShadow>(thisObject).Callback;
                    @this.RawMouseEvent(type, timeStamp, modifiers, point, delta);
                }
                catch (System.Exception)
                {
                }
            }

            [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.ThisCall)]
            private delegate System.Byte RawKeyEventDelegate(System.IntPtr thisObject, int arg0, System.UInt32 arg1, int arg2, System.UInt32 arg3);
            private static unsafe System.Byte RawKeyEvent(System.IntPtr thisObject, int param0, System.UInt32 param1, int param2, System.UInt32 param3)
            {
                try
                {
                    System.Boolean __result__ = default (System.Boolean);
                    System.Byte __result__native;
                    Avalonia.Native.Interop.AvnRawKeyEventType type = default (Avalonia.Native.Interop.AvnRawKeyEventType);
                    type = (Avalonia.Native.Interop.AvnRawKeyEventType)param0;
                    System.UInt32 timeStamp = default (System.UInt32);
                    timeStamp = (System.UInt32)param1;
                    Avalonia.Native.Interop.AvnInputModifiers modifiers = default (Avalonia.Native.Interop.AvnInputModifiers);
                    modifiers = (Avalonia.Native.Interop.AvnInputModifiers)param2;
                    System.UInt32 key = default (System.UInt32);
                    key = (System.UInt32)param3;
                    IAvnWindowBaseEvents @this = (IAvnWindowBaseEvents)ToShadow<Avalonia.Native.Interop.IAvnWindowBaseEventsShadow>(thisObject).Callback;
                    __result__ = @this.RawKeyEvent(type, timeStamp, modifiers, key);
                    __result__native = (System.Byte)(__result__ ? 1 : 0);
                    return __result__native;
                }
                catch (System.Exception)
                {
                    return default (System.Byte);
                }
            }

            [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.ThisCall)]
            private delegate System.Byte RawTextInputEventDelegate(System.IntPtr thisObject, System.UInt32 arg0, void *arg1);
            private static unsafe System.Byte RawTextInputEvent(System.IntPtr thisObject, System.UInt32 param0, void *param1)
            {
                try
                {
                    System.Boolean __result__ = default (System.Boolean);
                    System.Byte __result__native;
                    System.UInt32 timeStamp = default (System.UInt32);
                    timeStamp = (System.UInt32)param0;
                    System.String text = default (System.String);
                    System.IntPtr text_ = (System.IntPtr)param1;
                    IAvnWindowBaseEvents @this = (IAvnWindowBaseEvents)ToShadow<Avalonia.Native.Interop.IAvnWindowBaseEventsShadow>(thisObject).Callback;
                    text = System.Runtime.InteropServices.Marshal.PtrToStringAnsi(text_);
                    __result__ = @this.RawTextInputEvent(timeStamp, text);
                    __result__native = (System.Byte)(__result__ ? 1 : 0);
                    return __result__native;
                }
                catch (System.Exception)
                {
                    return default (System.Byte);
                }
            }

            [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.ThisCall)]
            private delegate void ScalingChangedDelegate(System.IntPtr thisObject, double arg0);
            private static unsafe void ScalingChanged(System.IntPtr thisObject, double param0)
            {
                try
                {
                    System.Double scaling = default (System.Double);
                    scaling = (System.Double)param0;
                    IAvnWindowBaseEvents @this = (IAvnWindowBaseEvents)ToShadow<Avalonia.Native.Interop.IAvnWindowBaseEventsShadow>(thisObject).Callback;
                    @this.ScalingChanged(scaling);
                }
                catch (System.Exception)
                {
                }
            }

            [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.ThisCall)]
            private delegate void RunRenderPriorityJobsDelegate(System.IntPtr thisObject);
            private static unsafe void RunRenderPriorityJobs(System.IntPtr thisObject)
            {
                try
                {
                    IAvnWindowBaseEvents @this = (IAvnWindowBaseEvents)ToShadow<Avalonia.Native.Interop.IAvnWindowBaseEventsShadow>(thisObject).Callback;
                    @this.RunRenderPriorityJobs();
                }
                catch (System.Exception)
                {
                }
            }
        }

        protected override SharpGen.Runtime.CppObjectVtbl Vtbl
        {
            get;
        }

        = new Avalonia.Native.Interop.IAvnWindowBaseEventsShadow.IAvnWindowBaseEventsVtbl(0);
    }

    [System.Runtime.InteropServices.GuidAttribute("2e2cda0a-9ae5-4f1b-8e20-081a04279f05"), SharpGen.Runtime.ShadowAttribute(typeof (Avalonia.Native.Interop.IAvnWindowBaseEventsShadow))]
    public partial interface IAvnWindowBaseEvents : SharpGen.Runtime.IUnknown
    {
        void Paint();
        void Closed();
        void Activated();
        void Deactivated();
        void Resized(Avalonia.Native.Interop.AvnSize size);
        void PositionChanged(Avalonia.Native.Interop.AvnPoint position);
        void RawMouseEvent(Avalonia.Native.Interop.AvnRawMouseEventType type, System.UInt32 timeStamp, Avalonia.Native.Interop.AvnInputModifiers modifiers, Avalonia.Native.Interop.AvnPoint point, Avalonia.Native.Interop.AvnVector delta);
        System.Boolean RawKeyEvent(Avalonia.Native.Interop.AvnRawKeyEventType type, System.UInt32 timeStamp, Avalonia.Native.Interop.AvnInputModifiers modifiers, System.UInt32 key);
        System.Boolean RawTextInputEvent(System.UInt32 timeStamp, System.String text);
        void ScalingChanged(System.Double scaling);
        void RunRenderPriorityJobs();
    }

    class IAvnWindowEventsShadow : Avalonia.Native.Interop.IAvnWindowBaseEventsShadow
    {
        protected unsafe class IAvnWindowEventsVtbl : Avalonia.Native.Interop.IAvnWindowBaseEventsShadow.IAvnWindowBaseEventsVtbl
        {
            public IAvnWindowEventsVtbl(int numberOfCallbackMethods): base (numberOfCallbackMethods + 2)
            {
                AddMethod(new ClosingDelegate(Closing));
                AddMethod(new WindowStateChangedDelegate(WindowStateChanged));
            }

            [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.ThisCall)]
            private delegate System.Byte ClosingDelegate(System.IntPtr thisObject);
            private static unsafe System.Byte Closing(System.IntPtr thisObject)
            {
                try
                {
                    System.Boolean __result__ = default (System.Boolean);
                    System.Byte __result__native;
                    IAvnWindowEvents @this = (IAvnWindowEvents)ToShadow<Avalonia.Native.Interop.IAvnWindowEventsShadow>(thisObject).Callback;
                    __result__ = @this.Closing();
                    __result__native = (System.Byte)(__result__ ? 1 : 0);
                    return __result__native;
                }
                catch (System.Exception)
                {
                    return default (System.Byte);
                }
            }

            [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.ThisCall)]
            private delegate void WindowStateChangedDelegate(System.IntPtr thisObject, int arg0);
            private static unsafe void WindowStateChanged(System.IntPtr thisObject, int param0)
            {
                try
                {
                    Avalonia.Native.Interop.AvnWindowState state = default (Avalonia.Native.Interop.AvnWindowState);
                    state = (Avalonia.Native.Interop.AvnWindowState)param0;
                    IAvnWindowEvents @this = (IAvnWindowEvents)ToShadow<Avalonia.Native.Interop.IAvnWindowEventsShadow>(thisObject).Callback;
                    @this.WindowStateChanged(state);
                }
                catch (System.Exception)
                {
                }
            }
        }

        protected override SharpGen.Runtime.CppObjectVtbl Vtbl
        {
            get;
        }

        = new Avalonia.Native.Interop.IAvnWindowEventsShadow.IAvnWindowEventsVtbl(0);
    }

    [System.Runtime.InteropServices.GuidAttribute("2e2cda0a-9ae5-4f1b-8e20-081a04279f06"), SharpGen.Runtime.ShadowAttribute(typeof (Avalonia.Native.Interop.IAvnWindowEventsShadow))]
    public partial interface IAvnWindowEvents : Avalonia.Native.Interop.IAvnWindowBaseEvents
    {
        System.Boolean Closing();
        void WindowStateChanged(Avalonia.Native.Interop.AvnWindowState state);
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkiaSharp;

namespace Modern.Forms.Design
{
    internal sealed class DropSourceBehavior : Behavior
    {
        private readonly IServiceProvider serviceProviderSource;
        private readonly IDesignerHost srcHost;
        private readonly BehaviorDataObject data;//drag data that represents the controls we're dragging & the effect/action
        private readonly DragDropEffects allowedEffects;//initial allowed effects for the drag operation
        private readonly DragComponent[] dragComponents;

        // These 2 could be different (e.g. if dropping between forms)
        private readonly BehaviorService behaviorServiceSource;//ptr back to the BehaviorService in the drop source
        private BehaviorService behaviorServiceTarget;//ptr back to the BehaviorService in the drop target
        private DragDropEffects lastEffect;//the last effect we saw (used for determining a valid drop)
        private Point lastSnapOffset;//the last snapoffset we used.
        private ArrayList dragObjects; // used to initialize the DragAssistanceManager
        private Point initialMouseLoc;//original mouse location in screen coordinates

        private Image dragImage;//A single image of the controls we are actually dragging around
        private Rectangle dragImageRect;//Rectangle of the dragImage -- in SOURCE AdornerWindow coordinates
        private Rectangle clearDragImageRect; //Rectangle used to remember the last dragimage rect we cleared
        private Point originalDragImageLocation; //original location of the drag image
        private Region dragImageRegion;

        private Point lastFeedbackLocation; // the last position we got feedback at
        private Control suspendedParent;//pointer to the parent that we suspended @ the beginning of the drag
        private Size parentGridSize; //used to snap around to grid dots if layoutmode == SnapToGrid
        private Point parentLocation;//location of parent on AdornerWindow - used for grid snap calculations
        private bool shareParent = true;//do dragged components share the parent
        private bool cleanedUpDrag;
        //private StatusCommandUI statusCommandUITarget;// UI for setting the StatusBar Information in the drop target
        private int primaryComponentIndex = -1; // Index of the primary component (control) in dragComponents

        /// <summary>
        ///  Constuctor that caches all needed vars for perf reasons.
        /// </summary>
        internal DropSourceBehavior (ICollection dragComponents, Control source, Point initialMouseLocation)
        {
            serviceProviderSource = source.Site;
            if (serviceProviderSource is null) {
                Debug.Fail ("DragBehavior could not be created because the source ServiceProvider was not found");
                return;
            }

            behaviorServiceSource = (BehaviorService)serviceProviderSource.GetService (typeof (BehaviorService));
            if (behaviorServiceSource is null) {
                Debug.Fail ("DragBehavior could not be created because the BehaviorService was not found");
                return;
            }

            if (dragComponents is null || dragComponents.Count <= 0) {
                Debug.Fail ("There are no component to drag!");
                return;
            }

            srcHost = (IDesignerHost)serviceProviderSource.GetService (typeof (IDesignerHost));
            if (srcHost is null) {
                Debug.Fail ("DragBehavior could not be created because the srcHost could not be found");
                return;
            }

            data = new BehaviorDataObject (dragComponents, source, this);
            allowedEffects = DragDropEffects.Copy | DragDropEffects.None | DragDropEffects.Move;
            this.dragComponents = new DragComponent[dragComponents.Count];
            parentGridSize = Size.Empty;

            lastEffect = DragDropEffects.None;
            lastFeedbackLocation = new Point (-1, -1);
            lastSnapOffset = Point.Empty;
            dragImageRect = Rectangle.Empty;
            clearDragImageRect = Rectangle.Empty;
            InitiateDrag (initialMouseLocation, dragComponents);
        }

        private void DisableAdorners (IServiceProvider serviceProvider, BehaviorService behaviorService, bool hostChange)
        {
            // find our body glyph adorner offered by the behavior service we don't want to disable the transparent body glyphs
            Adorner bodyGlyphAdorner = null;
            SelectionManager selMgr = (SelectionManager)serviceProvider.GetService (typeof (SelectionManager));
            if (selMgr != null) {
                bodyGlyphAdorner = selMgr.BodyGlyphAdorner;
            }

            //disable all adorners except for body glyph adorner
            foreach (Adorner a in behaviorService.Adorners) {
                if (bodyGlyphAdorner != null && a.Equals (bodyGlyphAdorner)) {
                    continue;
                }

                a.EnabledInternal = false;
            }

            behaviorService.Invalidate ();

            if (hostChange) { // TODO
                //selMgr.OnBeginDrag (new BehaviorDragDropEventArgs (dragObjects));
            }
        }

        /// <summary>
        ///  Called when the ControlDesigner starts a drag operation. Here, all adorners are disabled, screen shots of all related controls are taken, and the DragAssistanceManager  (for SnapLines) is created.
        /// </summary>
        private void InitiateDrag (Point initialMouseLocation, ICollection dragComps)
        {
            dragObjects = new ArrayList (dragComps);
            DisableAdorners (serviceProviderSource, behaviorServiceSource, false);
            Control primaryControl = dragObjects[0] as Control;
            Control primaryParent = primaryControl?.Parent;
            SKColor backColor = primaryParent?.Style.BackgroundColor ?? SKColor.Empty;// primaryParent != null ? primaryParent.BackColor : Color.Empty;
            dragImageRect = Rectangle.Empty;
            clearDragImageRect = Rectangle.Empty;
            initialMouseLoc = initialMouseLocation;

            //loop through every control we need to drag, calculate the offsets and get a snapshot
            for (int i = 0; i < dragObjects.Count; i++) {
                Control dragControl = (Control)dragObjects[i];

                dragComponents[i].dragComponent = dragObjects[i];
                dragComponents[i].positionOffset = new Point (dragControl.Location.X - primaryControl.Location.X,
                                                dragControl.Location.Y - primaryControl.Location.Y);
                Rectangle controlRect = behaviorServiceSource.ControlRectInAdornerWindow (dragControl);
                if (dragImageRect.IsEmpty) {
                    dragImageRect = controlRect;
                    dragImageRegion = new Region (controlRect);
                } else {
                    dragImageRect = Rectangle.Union (dragImageRect, controlRect);
                    dragImageRegion.Union (controlRect);
                }

                //Initialize the dragged location to be the current position of the control
                dragComponents[i].draggedLocation = controlRect.Location;
                dragComponents[i].originalControlLocation = dragComponents[i].draggedLocation;
                //take snapshot of each control
                DesignerUtils.GenerateSnapShot (dragControl, ref dragComponents[i].dragImage, i == 0 ? 2 : 1, 1, backColor);

                // The dragged components are not in any specific order. If they all share the same parent, we will sort them by their index in that parent's control's collection to preserve correct Z-order
                if (primaryParent != null && shareParent) {
                    dragComponents[i].zorderIndex = primaryParent.Controls.GetChildIndex (dragControl, false /*throwException*/);
                    if (dragComponents[i].zorderIndex == -1) {
                        shareParent = false;
                    }
                }
            }

            if (shareParent) {
                Array.Sort (dragComponents, this);
            }

            // Now that we are sorted, set the primaryComponentIndex...
            for (int i = 0; i < dragComponents.Length; i++) {
                if (primaryControl.Equals (dragComponents[i].dragComponent as Control)) {
                    primaryComponentIndex = i;
                    break;
                }
            }

            Debug.Assert (primaryComponentIndex != -1, "primaryComponentIndex was not set!");
            //suspend layout of the parent
            if (primaryParent != null) {
                suspendedParent = primaryParent;
                suspendedParent.SuspendLayout ();
                // Get the parent's grid settings here
                GetParentSnapInfo (suspendedParent, behaviorServiceSource);
            }

            // If the thing that's being dragged is of 0 size, make the image a little  bigger so that the user can see where they're dragging it.
            int imageWidth = dragImageRect.Width;
            if (imageWidth == 0) {
                imageWidth = 1;
            }

            int imageHeight = dragImageRect.Height;
            if (imageHeight == 0) {
                imageHeight = 1;
            }

            dragImage = new Bitmap (imageWidth, imageHeight, Drawing.Imaging.PixelFormat.Format32bppPArgb);
            using (Graphics g = Graphics.FromImage (dragImage)) {
                g.Clear (Color.Chartreuse);
            }

            ((Bitmap)dragImage).MakeTransparent (Color.Chartreuse);
            // Gotta use 2 using's here... Too bad.
            // Draw each control into the dragimage
            using (Graphics g = Graphics.FromImage (dragImage)) {
                using (SolidBrush brush = new SolidBrush (primaryControl.BackColor)) {
                    for (int i = 0; i < dragComponents.Length; i++) {
                        Rectangle controlRect = new Rectangle (dragComponents[i].draggedLocation.X - dragImageRect.X,
                                                  dragComponents[i].draggedLocation.Y - dragImageRect.Y,
                                                  dragComponents[i].dragImage.Width, dragComponents[i].dragImage.Height);
                        // The background
                        g.FillRectangle (brush, controlRect);
                        // The foreground
                        g.DrawImage (dragComponents[i].dragImage, controlRect,
                                    new Rectangle (0, 0, dragComponents[i].dragImage.Width, dragComponents[i].dragImage.Height),
                                    GraphicsUnit.Pixel);
                    }
                }
            }

            originalDragImageLocation = new Point (dragImageRect.X, dragImageRect.Y);
            //hide actual controls - this might cause a brief flicker, we are okay with that.
            ShowHideDragControls (false);
            cleanedUpDrag = false;
        }

        /// <summary>
        ///  This class extends from DataObject and carries additional  information such as: the list of Controls currently being dragged and the drag 'Source'.
        /// </summary>
        internal class BehaviorDataObject // : DataObject
        {
            private readonly ICollection _dragComponents;
            private readonly Control _source;
            private IComponent _target;
            private readonly DropSourceBehavior _sourceBehavior;

            public BehaviorDataObject (ICollection dragComponents, Control source, DropSourceBehavior sourceBehavior)// : base ()
            {
                _dragComponents = dragComponents;
                _source = source;
                _sourceBehavior = sourceBehavior;
                _target = null;
            }

            public Control Source {
                get => _source;
            }

            public ICollection DragComponents {
                get => _dragComponents;
            }

            public IComponent Target {
                get => _target;
                set => _target = value;
            }

            internal void EndDragDrop (bool allowSetChildIndexOnDrop) => _sourceBehavior.EndDragDrop (allowSetChildIndexOnDrop);

            internal void CleanupDrag () => _sourceBehavior.CleanupDrag ();

            internal ArrayList GetSortedDragControls (ref int primaryControlIndex) => _sourceBehavior.GetSortedDragControls (ref primaryControlIndex);
        }

        private struct DragComponent
        {
            public object dragComponent; //the dragComponent
            public int zorderIndex; //the dragComponent's z-order index
            public Point originalControlLocation; //the original control of the control in AdornerWindow coordinates
            public Point draggedLocation; //the location of the component after each drag - in AdornerWindow coordinates
            public Image dragImage; //bitblt'd image of control
            public Point positionOffset; //control position offset from primary selection
        }
    }
}

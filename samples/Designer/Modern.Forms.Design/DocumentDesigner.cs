using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modern.Forms.Design
{
    public class DocumentDesigner : ControlDesigner, IRootDesigner
    {
        private DesignerFrame? frame;
        private BehaviorService behaviorService;
        private SelectionManager selectionManager;

        public override void Initialize (IComponent component)
        {
            base.Initialize (component);

            frame = new DesignerFrame (this);

            var host = (IDesignerHost)GetService (typeof (IDesignerHost));

            behaviorService = new BehaviorService (Component.Site, frame);
            host.AddService (typeof (BehaviorService), behaviorService);

            selectionManager = new SelectionManager (host, behaviorService);
            host.AddService (typeof (SelectionManager), selectionManager);
        }

        ViewTechnology[] IRootDesigner.SupportedTechnologies => new[] { ViewTechnology.Default };
 
        object IRootDesigner.GetView (ViewTechnology technology)
        {
            return frame;
        }
   }
}

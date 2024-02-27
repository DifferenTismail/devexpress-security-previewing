using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using DevExpress.Persistent.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SecurityPreviewing.Module.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppWindowControllertopic.aspx.
    public partial class SecurityPreviewController : WindowController
    {

        // Use CodeRush to create Controllers and Actions with a few keystrokes.
        // https://docs.devexpress.com/CodeRushForRoslyn/403133/
        public SecurityPreviewController()
        {
            InitializeComponent();
            // Target required Windows (via the TargetXXX properties) and create their Actions.
            TargetWindowType = WindowType.Main;
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target Window.
            LoadRoles();
        }
        protected override void OnDeactivated()
        {
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }

        private void LoadRoles()
        {
            actionSecurityPreview.Items.Clear();
            using (IObjectSpace space = Application.CreateObjectSpace())
            {
                ISecurityPreview user = (ISecurityPreview)SecuritySystem.CurrentUser;

                if (user.SecurityPreview)
                    {
                        foreach(IPermissionPolicyRole role in space.GetObjects<PermissionPolicyRole>().OrderBy(x => x.Name))
                        {
                              ChoiceActionItem item = new ChoiceActionItem(role.Name, role.Oid);
                              actionSecurityPreview.Items.Add(item);
                        }
                    {
                    
                }
                actionSecurityPreview.Items.Add(new ChoiceActionItem("Refresh...", Guid.Empty));
                }
            }
        }
        private void actionSecurityPreview_Execute(object sender, SingleChoiceActionExecuteEventArgs e)
        {
            Guid roleid = (Guid)e.SelectedChoiceActionItem.Data;

            if (roleid.Equals(Guid.Empty))
            {
                LoadRoles();
                return;
            }
            using (IObjectSpace space = Application.CreateObjectSpace())
            {
                PermissionPolicyRole role = space.GetObjectByKey<PermissionPolicyRole>(roleid);
                PermissionPolicyUser user = space.GetObjectByKey<PermissionPolicyUser>(SecuritySystem.CurrentUserId);

                if(user == null || role == null)
                {
                    throw new UserFriendlyException("Unable to find user or role");
                }

                while(user.Roles.Count > 0) {
                    user.Roles.Remove(user.Roles[0]);        
                }
                user.Roles.Add(role);

                space.CommitChanges();

                Application.LogOff();
            }
              
        }

    public interface ISecurityPreview
        {  
            bool SecurityPreview { get; }
        }
    }
}

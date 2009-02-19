﻿using System;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Sitecore.Data.Items;
using Sitecore.Data;
using Sitecore.Diagnostics;
using Sitecore.Web;
using Sitecore.Configuration;
using Sitecore.Modules.Eviblog.Items;
using Sitecore.Data.Managers;
using Sitecore.Modules.Eviblog.Utilities;

namespace Sitecore.Modules.Eviblog.Commands.UI
{
    public class NewBlog : System.Web.UI.Page
    {
        #region Fields
        protected Label lblTitle;
        protected TextBox tbTitle;
        protected Label lbEmail;
        protected TextBox tbEmail;
        protected Label lbItemCount;
        protected TextBox tbItemCount;
        protected Label lbCommentsSidebarCount;
        protected TextBox tbCommentsSidebarCount;
        protected Label lbEnableRss;
        protected CheckBox chkEnableRSS;
        protected Label lbEnableComments;
        protected CheckBox chkEnableComments;
        protected Label lbShowEmailInComments;
        protected CheckBox chkShowEmailInComments;
        protected Label lbEnableLiveWriter;
        protected CheckBox chkEnableLiveWriter;
        protected Button btnCreate;
        protected Button btnCancel;
        protected Button btnDone;
        protected HtmlInputControl hidCreatedId;
         protected Database db = Factory.GetDatabase(WebUtil.GetQueryString("database"));
        #endregion

        #region Page Methods
         protected void Page_Load(object sender, EventArgs e)
        {
          
        }

        protected void btnCreate_Click(object sender, EventArgs e)
        {
            // Get the properties
            string currentItemID = WebUtil.GetQueryString("id");
            Item parent = db.GetItem(currentItemID);
            string BlogTitle = tbTitle.Text;

            // Create the item based on the branch template
            BranchItem newBlog = db.Branches.GetMaster(new ID("{6FC4278C-E043-458B-9D5D-BBA775A9C386}"));
            Item item = parent.Add(BlogTitle, newBlog);

            // Save ID to hidden textbox to return to created item
            hidCreatedId.Value = item.ID.ToString();

            // Fill in the fields
            SetBlogProperties(item.ID);

            // Register close event
            ClientScript.RegisterStartupScript(this.GetType(), "close","<script type=\"text/javascript\">onClose();</script>");
        }

        private void SetBlogProperties(ID BlogID)
        {
            try
            {
                // Get current blog as item
                Item blogItem = db.GetItem(BlogID);

                // Convert item to Blog
                Blog createdBlog = new Blog(blogItem);

                // Fill the blog item with the entered values
                createdBlog.BeginEdit();
                createdBlog.Email = tbEmail.Text;
                createdBlog.EnableComments = chkEnableComments.Checked;
                createdBlog.EnableRSS = chkEnableRSS.Checked;
                createdBlog.ShowEmailWithinComments = chkShowEmailInComments.Checked;
                createdBlog.EnableLiveWriter = chkEnableLiveWriter.Checked;
                if (tbItemCount.Text != null)
                {
                    createdBlog.DisplayItemCount = System.Convert.ToInt32(tbItemCount.Text);
                }
                if (tbCommentsSidebarCount.Text != null)
                {
                    createdBlog.DisplayCommentSidebarCount = System.Convert.ToInt32(tbCommentsSidebarCount.Text);
                }

                //Create media library folder
                Item mediaLibrayBlog = db.GetItem("/sitecore/media library/Modules/EviBlog");
                Item newMediaFolder = ItemManager.AddFromTemplate(tbTitle.Text, TemplateIDs.Folder, mediaLibrayBlog);
                Publish.PublishItem(newMediaFolder);

                createdBlog.EndEdit();
            }
            catch
            {
                Log.Error("Error during creating blog: " + BlogID, this);
            }
        }
        #endregion
    }
}

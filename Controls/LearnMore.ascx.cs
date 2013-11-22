using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;

namespace ClearCostWeb.Controls
{
    public partial class LearnMore : UserControl
    {
        #region Private Variables
        private ITemplate titleTemplate = null;
        private ITemplate textTemplate = null;
        #endregion

        #region Public Properties
        [TemplateContainer(typeof(LearnMoreContainer)),
        PersistenceMode(PersistenceMode.InnerProperty)]
        public ITemplate TitleTemplate
        {
            get { return titleTemplate; }
            set { titleTemplate = value; }
        }
        [TemplateContainer(typeof(LearnMoreContainer)),
        PersistenceMode(PersistenceMode.InnerProperty)]
        public ITemplate TextTemplate
        {
            get { return textTemplate; }
            set { textTemplate = value; }
        }
        #endregion

        #region Methods
        public void Page_Init()
        {
            imgLearnMore.ImageUrl = ResolveUrl("~/Images/icon_question_mark.png");
            imgMoreInfo.ImageUrl = ResolveUrl("~/Images/icon_x_sm.png");
        }
        protected override void CreateChildControls()
        {
            phTitle.Controls.Clear();
            LearnMoreContainer lmcTitle = new LearnMoreContainer();
            TitleTemplate.InstantiateIn(lmcTitle);
            phTitle.Controls.Add(lmcTitle);

            phText.Controls.Clear();
            LearnMoreContainer lmcText = new LearnMoreContainer();
            TextTemplate.InstantiateIn(lmcText);
            phText.Controls.Add(lmcText);

            base.CreateChildControls();
        }
        public override void DataBind()
        {
            CreateChildControls();
            this.ChildControlsCreated = true;
            base.DataBind();
        }
        #endregion

        public class LearnMoreContainer : Control, INamingContainer
        {
            internal LearnMoreContainer()
            { }
        }
    }
}

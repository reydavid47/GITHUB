using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;

namespace ClearCostWeb.Controls
{
    public partial class AlertBar : System.Web.UI.UserControl
    {
        public enum AlertType { Normal, Small, Mini };
        private ITemplate messageTemplate = null;
        private String saveTotal;
        private String navigateTo;
        private AlertType myAlertType = AlertType.Normal;
        private String staticText;

        #region Results_Rx only
        //Used only for the results_rx page
        private String pharmacyName = "";
        public String PharmacyName { get { return String.Format("({0})&nbsp;", pharmacyName); } set { pharmacyName = value; } }
        private Double mySavings = 0.0;

        [Description("The total savings with the current pharmacy")]
        public Double TotalSavings
        {
            get { return mySavings; }
            set { mySavings = value; }
        }
        protected Boolean CouldVisible
        {
            get { return (mySavings != 0.0); }
        }
        protected Boolean MaxVisible
        {
            get { return (mySavings == 0.0); }
        }
        #endregion

        [Browsable(false)]
        [TemplateContainer(typeof(MessageContainer), System.ComponentModel.BindingDirection.TwoWay)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public ITemplate MessageTemplate
        {
            get { return messageTemplate; }
            set { messageTemplate = value; }
        }

        [Description("The amount to display in the \"You Chould Have Saved\" text")]
        public String SaveTotal
        {
            get { return saveTotal; }
            set { saveTotal = value; }
        }

        [Description("The page to navigate to upon clicking the internal link")]
        public String NavigateTo
        {
            get { return navigateTo; }
            set { navigateTo = value; }
        }

        [Description("Size of the alert display")]
        public AlertType TypeOfAlert
        {
            get { return myAlertType; }
            set { myAlertType = value; }
        }

        [Description("The general text to display")]
        public String StaticText
        {
            get { return staticText; }
            set { staticText = value; }
        }

        protected String pClass
        {
            get
            {
                String ret = "";
                switch (myAlertType)
                {
                    case AlertType.Normal:
                        break;
                    case AlertType.Small:
                        ret = "smaller alertsmall";
                        break;
                    case AlertType.Mini:
                        ret = "smaller alertmini";
                        break;
                    default:
                        break;
                }
                return ret;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            switch (myAlertType)
            {
                case AlertType.Normal:
                    alertbar.ClientIDMode = ClientIDMode.Static;
                    alert.ClientIDMode = ClientIDMode.Static;
                    break;
                case AlertType.Small:
                    alertbar.ClientIDMode = ClientIDMode.Inherit;
                    alert.ClientIDMode = ClientIDMode.Inherit;
                    break;
                case AlertType.Mini:
                    alertbar.ClientIDMode = ClientIDMode.Inherit;
                    alert.ClientIDMode = ClientIDMode.Inherit;
                    break;
                default:
                    break;
            }
            if (messageTemplate != null)
            {
                MessageContainer container;
                if (!String.IsNullOrWhiteSpace(staticText))
                    container = new MessageContainer(staticText);
                else
                    //if (pharmacyName == "")
                    //{
                    //    container = new MessageContainer(saveTotal, navigateTo);
                    //}
                    //else
                    //{
                    container = new MessageContainer(saveTotal, navigateTo, pharmacyName, CouldVisible, MaxVisible);
                //}
                messageTemplate.InstantiateIn(container);
                AlertPlaceHolder.Controls.Add(container);
            }

            DataBind();
        }

        public class MessageContainer : Control, INamingContainer
        {
            private String _saveTotal;
            private String _navigateTo;
            private String _pharmacyName;
            private Boolean _couldVisible;
            private Boolean _maxVisible;
            private string _staticText;
            internal MessageContainer(String saveTotal, String navigateTo)
            {
                _saveTotal = saveTotal;
                _navigateTo = navigateTo;
            }
            internal MessageContainer(String saveTotal, String navigateTo, String pharmacyName, Boolean couldVisible, Boolean maxVisible)
            {
                _saveTotal = saveTotal;
                _navigateTo = navigateTo;
                _pharmacyName = pharmacyName;
                _couldVisible = couldVisible;
                _maxVisible = maxVisible;
            }
            internal MessageContainer(String staticText)
            {
                _staticText = staticText;
            }
            public String SaveTotal { get { return _saveTotal; } }
            public String NavigateTo { get { return _navigateTo; } }
            public String PharmacyName { get { return _pharmacyName; } }
            public Boolean CouldVisible { get { return _couldVisible; } }
            public Boolean MaxVisible { get { return _maxVisible; } }
            public String StaticText { get { return _staticText; } }
        }
    }
}
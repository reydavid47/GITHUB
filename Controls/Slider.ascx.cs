﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace ClearCostWeb.Controls
{
    public partial class Slider : System.Web.UI.UserControl
    {
        private ITemplate scriptTemplate = null;

        #region Properties
        protected String ControlPostBack
        {
            get
            {
                return Page.ClientScript.GetPostBackEventReference(this, "slideStop");
            }
        }
        protected string width;
        public virtual string Width
        {
            get { return this.width; }
            set { this.width = value; }
        }
        protected int min = 0;
        public virtual int Min
        {
            get { return this.min; }
            set { this.min = value; }
        }
        protected int max = 10;
        public virtual int Max
        {
            get { return this.max; }
            set { this.max = value; }
        }
        protected int value
        {
            get { return int.Parse((this.ViewState["SliderValue"] ?? 0).ToString()); }
            set { this.ViewState["SliderValue"] = value; }
        }
        public virtual int Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        [ TemplateContainer(typeof(ScriptContainer)) ]
        [ PersistenceMode(PersistenceMode.InnerProperty) ]
        public ITemplate ScriptTemplate
        {
            get
            {
                return scriptTemplate;
            }
            set
            {
                scriptTemplate = value;
            }
        }
        #endregion

        #region Event Related
        public event EventHandler SlideChanged;
        public class SliderEventArgs : EventArgs
        {
            public int NewValue { get; set; }
        }
        protected void OnSlideChanged(SliderEventArgs e)
        {
            if (SlideChanged != null && scriptTemplate == null)
            {
                SlideChanged(this, e);
            }
        }
        #endregion

        #region Control methods
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            JavaScriptHelper.IncludeJQuerySlider(Page.ClientScript);
            slider.Style.Add(HtmlTextWriterStyle.Width, this.width);
            slider.Style.Add(HtmlTextWriterStyle.Display, "inline-block");
            slider.Style.Add(HtmlTextWriterStyle.VerticalAlign, "middle");
        }
        protected void Page_Init()
        {
            if (scriptTemplate != null)
            {
                ScriptWrapperStart.Visible = ScriptWrapperEnd.Visible = true;
                ScriptContainer container = new ScriptContainer(_SLIDER.ClientID);
                scriptTemplate.InstantiateIn(container);
                ScriptPlaceHolder.Controls.Add(container);
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            Page.AddCSSToHeader("jquery.ui.slider.css", true);
            if (!IsPostBack)
            {
                _SLIDER.Value = this.value.ToString();
                this.DataBind();
            }
            else
            {
                if (scriptTemplate == null)
                {
                    int v = 0;
                    if (int.TryParse(_SLIDER.Value, out v))
                    {
                        this.value = v;
                        OnSlideChanged(new SliderEventArgs() { NewValue = v });
                    }
                }
            }
        }
        #endregion

        public class ScriptContainer : Control, INamingContainer
        {
            private string sliderID;
            internal ScriptContainer(string sliderId)
            {
                sliderID = sliderId;
            }
            public string SliderID { get { return sliderID; } }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.UI;
using System.Data;
using QSEncryption.QSEncryption;
using System.Dynamic;
using System.Web.Script.Serialization;
using System.Text;
using System.Collections.Specialized;
using System.Diagnostics;

namespace ClearCostWeb
{
    /// <summary>
    /// Used to centralize methods that do nothing more than put data into the session
    /// </summary>
    public static class BaseCCHDataExtensionMethods
    {
        /// <summary>
        /// Puts the relevant data into session from a DataSet object rather than storing the whole object in session
        /// </summary>
        /// <param name="info">The GetKeyEmployeeInfo Object filled with data</param>
        /// <param name="UserName">The UserName of the employee</param>
        /// <returns>True if successfull, False if there are errors or no data</returns>
        public static Boolean PutInSession(this GetKeyEmployeeInfo info, String UserName)
        {
            if (info.HasErrors) return false;
            if (info.Tables.Count == 0) return false;
            if (info.Tables[0].Rows.Count == 0) return false;

            ThisSession.CCHID = info.CCHID;
            ThisSession.EmployeeID = info.EmployeeID;
            ThisSession.SubscriberMedicalID = info.SubscriberMedicalID;
            ThisSession.SubscriberRXID = info.SubscriberRXID;
            ThisSession.LastName = info.LastName;
            ThisSession.FirstName = info.FirstName;
            ThisSession.PatientAddress1 = info.Address1;
            ThisSession.PatientAddress2 = info.Address2;
            ThisSession.PatientCity = info.City;
            ThisSession.PatientState = info.State;
            ThisSession.PatientZipCode = info.ZipCode;
            ThisSession.PatientLatitude = info.Latitude;
            ThisSession.PatientLongitude = info.Longitude;
            ThisSession.DefaultPatientAddress = ThisSession.PatientAddressSingleLine;
            ThisSession.PatientDateOfBirth = info.DateOfBirth;
            ThisSession.PatientPhone = info.Phone;
            ThisSession.HealthPlanType = info.HealthPlanType;
            ThisSession.MedicalPlanType = info.MedicalPlanType;
            ThisSession.RxPlanType = info.RxPlanType;
            ThisSession.PatientGender = info.Gender;
            ThisSession.Parent = info.Parent;
            ThisSession.Adult = info.Adult;
            ThisSession.PatientEmail = UserName.Trim();
            ThisSession.OptInIncentiveProgram = info.OptInIncentiveProgram;
            ThisSession.OptInEmailAlerts = info.OptInEmailAlerts;
            ThisSession.OptInTextMsgAlerts = info.OptInTextMsgAlerts;
            ThisSession.MobilePhone = info.MobilePhone;
            ThisSession.OptInPriceConcierge = info.OptInPriceConcierge;
            if (info.Insurer != String.Empty)
                ThisSession.Insurer = info.Insurer;
            if (info.RXProvider != String.Empty)
                ThisSession.RXProvider = info.RXProvider;
            if (info.DependentTable.TableName != "EmptyTable")
            {
                Dependents deps = new Dependents();
                Dependent dep = null;

                info.ForEachDependent(delegate(DataRow dr)
                {
                    dep = new Dependent();
                    dep.CCHID = int.Parse(dr["CCHID"].ToString());
                    dep.FirstName = dr["FirstName"].ToString();
                    dep.LastName = dr["LastName"].ToString();
                    dep.DateOfBirth = DateTime.Parse(dr["DateOfBirth"].ToString());
                    dep.Age = int.Parse(dr["Age"].ToString());
                    dep.IsAdult = int.Parse(dr["Adult"].ToString()) == 1 ? true : false;
                    dep.ShowAccessQuestions = int.Parse(dr["ShowAccessQuestions"].ToString()) == 1 ? true : false;
                    dep.RelationshipText = dr["RelationshipText"].ToString();
                    dep.DepToUserGranted = int.Parse(dr["DepToUserGranted"].ToString()) == 1 ? true : false;
                    dep.UserToDepGranted = int.Parse(dr["UserToDepGranted"].ToString()) == 1 ? true : false;
                    dep.Email = dr["Email"].ToString();

                    deps.Add(dep);
                });

                ThisSession.Dependents = deps;
            }

            if (info.YouCouldHaveSavedTable.TableName != "EmptyTable")
                ThisSession.YouCouldHaveSaved = (int)info.YouCouldHaveSaved;

            return true;
        }
        /// <summary>
        /// Puts the relevant data into session from a DataSet object rather than storing the whole object in session
        /// </summary>
        /// <param name="info">The GetKeyUserInfo Object filled with data</param>
        /// <returns>True if successfull, False if there are errors or no data</returns>
        public static Boolean PutInSession(this GetKeyUserInfo info)
        {
            if (info.HasErrors) return false;
            if (info.Tables.Count == 0) return false;
            if (info.Tables[0].Rows.Count == 0) return false;

            ThisSession.CnxString = info.ConnectionString;
            ThisSession.EmployerID = info.EmployerID;
            ThisSession.EmployerName = info.EmployerName;
            ThisSession.Insurer = info.Insurer;
            ThisSession.RXProvider = info.RXProvider;
            ThisSession.ShowYourCostColumn = info.ShowYourCostColumn;

            return true;
        }
        /// <summary>
        /// Puts the relevant data into session from a DataSet object rather than storing the whole object in session
        /// </summary>
        /// <param name="info">The GetEmployeeEnrollment Object filled with data</param>
        /// <returns>True if successfull, False if there are errors or no data</returns>
        public static Boolean PutInSession(this GetEmployeeEnrollment info)
        {
            //General exit statuses
            if (info.HasErrors) return false;
            if (info.Tables.Count == 0) return false;
            if (info.Tables[0].Rows.Count == 0) return false;

            //Employee info
            if (info.EmployeeTable.TableName == "Empty") return false;
            if (info.EmployeeTable.Rows.Count == 0) return false;

            ThisSession.CCHID = info.CCHID;
            ThisSession.EmployeeID = info.EmployeeID;
            ThisSession.SubscriberMedicalID = info.SubscriberMedicalID;
            ThisSession.SubscriberRXID = info.SubscriberRXID;
            ThisSession.LastName = info.LastName;
            ThisSession.FirstName = info.FirstName;
            ThisSession.PatientAddress1 = info.Address1;
            ThisSession.PatientAddress2 = info.Address2;
            ThisSession.PatientCity = info.City;
            ThisSession.PatientState = info.State;
            ThisSession.PatientZipCode = info.ZipCode;
            ThisSession.PatientLatitude = info.Latitude;
            ThisSession.PatientLongitude = info.Longitude;
            ThisSession.PatientDateOfBirth = info.DOB;
            ThisSession.PatientPhone = info.Phone;
            ThisSession.HealthPlanType = info.HealthPlanType;
            ThisSession.MedicalPlanType = info.MedicalPlanType;
            ThisSession.RxPlanType = info.RxPlanType;
            ThisSession.PatientGender = info.Gender;
            ThisSession.Parent = info.Parent;
            ThisSession.Adult = info.Adult;
            ThisSession.PatientEmail = info.Email;

            if (info.Insurer != string.Empty) ThisSession.Insurer = info.Insurer;
            if (info.RXProvider != string.Empty) ThisSession.RXProvider = info.RXProvider;

            //Dependent Info
            if (info.DependentTable.TableName != "EmptyTable")
            {
                Dependents deps = new Dependents();
                info.ForEachDependent(delegate(DataRow dr)
                {
                    deps.Add(dr.DependentFromRow());
                });
                ThisSession.Dependents = deps;
            }

            //You Could Have Saved
            if (info.YouCouldHaveSavedTable.TableName != "EmptyTable")
                ThisSession.YouCouldHaveSaved = (int)info.YouCouldHaveSaved;

            //Alternate plans
            if (info.AlternateTable.TableName != "EmptyTable" && info.AlternateTable.Rows.Count > 0)
            {
                //JM 9/16/13 - At this time we only support one split
                ThisSession.EmployerID = info.AlternateEmployerID.ToString();
                ThisSession.CnxString = info.AlternateConnectionString;
            }

            return true;
        }
        /// <summary>
        /// Puts the relevant data into session from a DataSet object rather than storing the whole object in session
        /// </summary>
        /// <param name="info">The GetEmployerContent Object filled with data</param>
        /// <returns>True if successfull, False if there are errors or no data</returns>
        public static Boolean PutInSession(this GetEmployerContent info)
        {
            if (info.HasErrors) return false;
            if (info.Tables.Count == 0) return false;
            if (info.Tables[0].Rows.Count == 0) return false;

            if (info.Tables.Count > 0 && info.Tables[0].Rows.Count > 0)
            {
                ThisSession.InsurerName = info.InsurerName;
                ThisSession.LogoImageName = info.LogoImageName;
                ThisSession.EmployerPhone = info.PhoneNumber;
                ThisSession.InternalLogo = info.InternalLogo;
                ThisSession.ContactText = info.ContactText;
                ThisSession.SpecialtyNetworkText = info.SpecialtyNetworkText;
                ThisSession.PastCareDisclaimerText = info.PastCareDisclaimerText;  //  lam, 20130418, MSF-299
                ThisSession.RxResultDisclaimerText = info.RxResultDisclaimerText;  //  lam, 20130418, MSF-294
                ThisSession.AllResult1DisclaimerText = info.AllResult1DisclaimerText;  //  lam, 20130425, MSF-295 
                ThisSession.AllResult2DisclaimerText = info.AllResult2DisclaimerText;  //  lam, 20130425, MSF-295
                ThisSession.SpecialtyDrugDisclaimerText = info.SpecialtyDrugDisclaimerText;  //  lam, 20130429, CI-59
                ThisSession.MentalHealthDisclaimerText = info.MentalHealthDisclaimerText;  //  lam, 20130508, CI-144
                ThisSession.ServiceNotFoundDisclaimerText = info.ServiceNotFoundDisclaimerText;  //  lam, 20130604, MSF-377
                ThisSession.DefaultYourCostOn = info.DefaultYourCostOn;
                ThisSession.DefaultSort = info.DefaultSort;
                ThisSession.AllowFairPriceSort = info.AllowFairPriceSort;  //  lam, 20130618, MSB-324
                ThisSession.SavingsChoiceEnabled = info.SavingsChoiceEnabled;
                ThisSession.ShowSCIQTab = info.ShowSCIQTab;  //  lam, 20130816, SCIQ-77
            }
            
            return true;
        }
        /// <summary>
        /// Puts the relevant data into session from a DataSet object rather than storing the whole object in session
        /// </summary>
        /// <param name="info">The GetPasswordQuestions Object filled with data</param>
        /// <returns>True if successfull, False if there are errors or no data</returns>
        public static Boolean PutInSession(this GetPasswordQuestions info)
        {
            if (info.HasErrors) return false;
            if (info.Tables.Count == 0) return false;
            if (info.Tables[0].Rows.Count == 0) return false;

            ThisSession.CurrentAvailableSecurityQuestions =
                (from i in info.Tables[0].AsEnumerable()
                 select i[0].ToString()).ToArray<String>();

            return true;
        }

        public static String GetFASRowHTML(this DataRow dr, Boolean IsPreferred = false)
        {
            StringWriter sw = new StringWriter();
            using (HtmlTextWriter htw = new HtmlTextWriter(sw))
            {
                htw.AddAttribute(HtmlTextWriterAttribute.Class, "resultrow graydiv");
                htw.RenderBeginTag(HtmlTextWriterTag.Tr); //Table Row definition

                htw.AddAttribute(HtmlTextWriterAttribute.Class, "tdfirst graydiv PRAC");
                htw.AddStyleAttribute(HtmlTextWriterStyle.Width, "323px");
                htw.RenderBeginTag(HtmlTextWriterTag.Td); //Facility Name TD
                htw.AddStyleAttribute(HtmlTextWriterStyle.Cursor, "pointer");
                //htw.AddAttribute("nav",this.Nav);

                htw.RenderBeginTag(HtmlTextWriterTag.A); //Link Facility Link button
                htw.AddAttribute(HtmlTextWriterAttribute.Class, "readmore result");
                htw.RenderBeginTag(HtmlTextWriterTag.Div); // Facility link DIV                    
                htw.Write(dr.Field<dynamic>("PracticeName"));
                htw.RenderEndTag(); // End Facility link DIV
                htw.RenderEndTag(); //End Facility Name Link
                htw.AddStyleAttribute(HtmlTextWriterStyle.MarginLeft, "19px");
                htw.RenderBeginTag(HtmlTextWriterTag.Div); //City Div                    
                htw.Write(dr.Field<dynamic>("LocationCity"));
                htw.RenderEndTag(); //End City Div
                htw.RenderEndTag(); // End Facility Name TD

                htw.AddAttribute(HtmlTextWriterAttribute.Class, "graydiv DIST");
                htw.AddStyleAttribute(HtmlTextWriterStyle.Width, "97px");
                htw.AddStyleAttribute(HtmlTextWriterStyle.WhiteSpace, "nowrap");
                htw.AddStyleAttribute(HtmlTextWriterStyle.VerticalAlign, "middle");
                htw.RenderBeginTag(HtmlTextWriterTag.Td); //Distance TD
                htw.AddStyleAttribute(HtmlTextWriterStyle.TextAlign, "center");
                htw.RenderBeginTag(HtmlTextWriterTag.Div); // Distance DIV
                htw.RenderBeginTag(HtmlTextWriterTag.Span); //Distance SPAN
                htw.RenderEndTag(); // End Distance SPAN
                htw.RenderEndTag(); // End Distance DIV
                htw.RenderEndTag(); // End Distance TD

                htw.AddAttribute(HtmlTextWriterAttribute.Class, "graydiv EC");
                htw.AddStyleAttribute(HtmlTextWriterStyle.Width, "162px");
                htw.AddStyleAttribute(HtmlTextWriterStyle.VerticalAlign, "middle");
                htw.RenderBeginTag(HtmlTextWriterTag.Td); // Total Estimated Cost TD
                htw.AddStyleAttribute(HtmlTextWriterStyle.TextAlign, "center");
                htw.RenderBeginTag(HtmlTextWriterTag.Div); // Total Estimated Cost DIV
                if (dr.Field<dynamic>("AntiTransparency"))
                {
                    using (LiteralControl lc = new LiteralControl("<b>Undisclosed</b>&nbsp;"))
                        lc.RenderControl(htw);

                    using (Panel learnMore = new Panel())
                    {
                        learnMore.CssClass = "learnmore";
                        learnMore.RenderBeginTag(htw);

                        htw.AddAttribute(HtmlTextWriterAttribute.Title, "Learn More");
                        htw.RenderBeginTag(HtmlTextWriterTag.A); //Render learnmore Title A

                        using (Image i = new Image())
                        {
                            i.AlternateText = "Learn More";
                            i.Width = new Unit(12, UnitType.Pixel);
                            i.Height = new Unit(13, UnitType.Pixel);
                            i.BorderWidth = new Unit(0, UnitType.Pixel);
                            //i.Style[HtmlTextWriterStyle.Width] = "13px";
                            //i.Style[HtmlTextWriterStyle.Height] = "13px";
                            //i.Style[HtmlTextWriterStyle.ZIndex] = "1030";
                            i.ImageUrl = "~/Images/icon_question_mark.png";
                            i.RenderControl(htw); //Render Learnmore Question Mark Image
                        }

                        htw.RenderEndTag(); //Close Title A

                        using (Panel moreInfo = new Panel())
                        {
                            moreInfo.CssClass = "moreinfo";
                            moreInfo.Style[HtmlTextWriterStyle.ZIndex] = "1031";
                            moreInfo.RenderBeginTag(htw);

                            using (Image i = new Image())
                            {
                                i.AlternateText = "Close";
                                i.Width = new Unit(14, UnitType.Pixel);
                                i.Height = new Unit(14, UnitType.Pixel);
                                i.BorderWidth = new Unit(0, UnitType.Pixel);
                                i.ImageAlign = ImageAlign.Right;
                                i.Style[HtmlTextWriterStyle.Cursor] = "pointer";
                                //i.Style[HtmlTextWriterStyle.Width] = "14px";
                                //i.Style[HtmlTextWriterStyle.Height] = "14px";
                                i.ImageUrl = "~/Images/icon_x_sm.png";
                                i.RenderControl(htw); //Render MoreInfo close image
                            }

                            htw.RenderBeginTag(HtmlTextWriterTag.P); //Render MoreInfo Title Paragraph
                            htw.AddAttribute(HtmlTextWriterAttribute.Class, "upper");
                            htw.RenderBeginTag(HtmlTextWriterTag.B); //Render MoreInfo Title Bold
                            htw.Write("Anti-Transparency Providers"); //Add MoreInfo Title text
                            htw.RenderEndTag(); //Close MoreInfo Title Bold
                            htw.RenderEndTag(); //Close MoreInfo Title Paragraph
                            htw.RenderBeginTag(HtmlTextWriterTag.P); //Render MoreInfo Text Paragraph
                            htw.Write("This provider has chosen not to show prices to patients.  Please note that in some instances, refusal to show this information can be an indication of high prices."); //Write Text
                            htw.RenderEndTag(); //Close MoreInfo Text Paragraph

                            moreInfo.RenderEndTag(htw);
                        }

                        learnMore.RenderEndTag(htw);
                    }
                }
                else
                {
                    htw.AddAttribute(HtmlTextWriterAttribute.Class, "alignright");
                    htw.RenderBeginTag(HtmlTextWriterTag.B); //Range Min B
                    htw.Write(String.Format("{0:c0}", dr.Field<dynamic>("RangeMin")));
                    htw.RenderEndTag(); // End RangeMin B
                    //  ------------------------------------------------------------------
                    //  lam, 20130402, MSF-293
                    if (dr.Field<dynamic>("RangeMin") != dr.Field<dynamic>("RangeMax"))
                    {
                    //  ------------------------------------------------------------------
                        htw.AddAttribute(HtmlTextWriterAttribute.Class, "dashcol");
                        htw.RenderBeginTag(HtmlTextWriterTag.B); //DashCol B
                        htw.Write("-");
                        htw.RenderEndTag(); // End DashCol B
                        htw.AddAttribute(HtmlTextWriterAttribute.Class, "alignleft");
                        htw.RenderBeginTag(HtmlTextWriterTag.B); //Range Max B
                        htw.Write(String.Format("{0:c0}", dr.Field<dynamic>("RangeMax")));
                        htw.RenderEndTag(); // End RangeMax B
                    }
                }
                htw.RenderEndTag(); // End RangeMax DIV
                htw.RenderEndTag(); // End RangeMax TD

                htw.AddAttribute(HtmlTextWriterAttribute.Class, "graydiv YC");
                htw.AddStyleAttribute(HtmlTextWriterStyle.Width, "0px");
                htw.AddStyleAttribute(HtmlTextWriterStyle.VerticalAlign, "middle");
                htw.AddStyleAttribute(HtmlTextWriterStyle.Display, "none");
                htw.RenderBeginTag(HtmlTextWriterTag.Td); // Your Estimated Cost TD
                htw.AddStyleAttribute(HtmlTextWriterStyle.TextAlign, "center");
                htw.RenderBeginTag(HtmlTextWriterTag.Div); // Your Estimated Cost DIV
                if (dr.Field<dynamic>("AntiTransparency"))
                {
                    using (LiteralControl lc = new LiteralControl("<b>Undisclosed</b>&nbsp;"))
                        lc.RenderControl(htw);

                    using (Panel learnMore = new Panel())
                    {
                        learnMore.CssClass = "learnmore";
                        learnMore.RenderBeginTag(htw);

                        htw.AddAttribute(HtmlTextWriterAttribute.Title, "Learn More");
                        htw.RenderBeginTag(HtmlTextWriterTag.A); //Render learnmore Title A

                        using (Image i = new Image())
                        {
                            i.AlternateText = "Learn More";
                            i.Width = new Unit(12, UnitType.Pixel);
                            i.Height = new Unit(13, UnitType.Pixel);
                            i.BorderWidth = new Unit(0, UnitType.Pixel);
                            //i.Style[HtmlTextWriterStyle.Width] = "13px";
                            //i.Style[HtmlTextWriterStyle.Height] = "13px";
                            //i.Style[HtmlTextWriterStyle.ZIndex] = "1030";
                            i.ImageUrl = "~/Images/icon_question_mark.png";
                            i.RenderControl(htw); //Render Learnmore Question Mark Image
                        }

                        htw.RenderEndTag(); //Close Title A

                        using (Panel moreInfo = new Panel())
                        {
                            moreInfo.CssClass = "moreinfo";
                            moreInfo.Style[HtmlTextWriterStyle.ZIndex] = "1031";
                            moreInfo.RenderBeginTag(htw);

                            using (Image i = new Image())
                            {
                                i.AlternateText = "Close";
                                i.Width = new Unit(14, UnitType.Pixel);
                                i.Height = new Unit(14, UnitType.Pixel);
                                i.BorderWidth = new Unit(0, UnitType.Pixel);
                                i.ImageAlign = ImageAlign.Right;
                                i.Style[HtmlTextWriterStyle.Cursor] = "pointer";
                                //i.Style[HtmlTextWriterStyle.Width] = "14px";
                                //i.Style[HtmlTextWriterStyle.Height] = "14px";
                                i.ImageUrl = "~/Images/icon_x_sm.png";
                                i.RenderControl(htw); //Render MoreInfo close image
                            }

                            htw.RenderBeginTag(HtmlTextWriterTag.P); //Render MoreInfo Title Paragraph
                            htw.AddAttribute(HtmlTextWriterAttribute.Class, "upper");
                            htw.RenderBeginTag(HtmlTextWriterTag.B); //Render MoreInfo Title Bold
                            htw.Write("Anti-Transparency Providers"); //Add MoreInfo Title text
                            htw.RenderEndTag(); //Close MoreInfo Title Bold
                            htw.RenderEndTag(); //Close MoreInfo Title Paragraph
                            htw.RenderBeginTag(HtmlTextWriterTag.P); //Render MoreInfo Text Paragraph
                            htw.Write("This provider has chosen not to show prices to patients.  Please note that in some instances, refusal to show this information can be an indication of high prices."); //Write Text
                            htw.RenderEndTag(); //Close MoreInfo Text Paragraph

                            moreInfo.RenderEndTag(htw);
                        }

                        learnMore.RenderEndTag(htw);
                    }
                }
                else
                {
                    htw.AddAttribute(HtmlTextWriterAttribute.Class, "alignright");
                    htw.RenderBeginTag(HtmlTextWriterTag.B); //Range Min B
                    htw.Write(String.Format("{0:c0}", dr.Field<dynamic>("YourCostMin")));
                    htw.RenderEndTag(); // End RangeMin B
                    //  ------------------------------------------------------------------
                    //  lam, 20130402, MSF-293
                    if (dr.Field<dynamic>("YourCostMin") != dr.Field<dynamic>("YourCostMax"))
                    {
                    //  ------------------------------------------------------------------
                        htw.AddAttribute(HtmlTextWriterAttribute.Class, "dashcol");
                        htw.RenderBeginTag(HtmlTextWriterTag.B); //DashCol B
                        htw.Write("-");
                        htw.RenderEndTag(); // End DashCol B
                        htw.AddAttribute(HtmlTextWriterAttribute.Class, "alignleft");
                        htw.RenderBeginTag(HtmlTextWriterTag.B); //Range Max B
                        htw.Write(String.Format("{0:c0}", dr.Field<dynamic>("YourCostMax")));
                        htw.RenderEndTag(); // End RangeMax B
                    }
                }
                htw.RenderEndTag(); // End RangeMax DIV
                htw.RenderEndTag(); // End RangeMax TD

                htw.AddAttribute(HtmlTextWriterAttribute.Class, "tdcheck graydiv FP");
                htw.AddStyleAttribute(HtmlTextWriterStyle.Width, "88px");
                htw.AddStyleAttribute(HtmlTextWriterStyle.VerticalAlign, "middle");
                htw.AddAttribute(HtmlTextWriterAttribute.Align, "center");
                htw.RenderBeginTag(HtmlTextWriterTag.Td); // Fair Price TD
                htw.AddStyleAttribute(HtmlTextWriterStyle.Width, "23px");
                htw.AddStyleAttribute(HtmlTextWriterStyle.Height, "23px");
                htw.AddStyleAttribute(HtmlTextWriterStyle.BorderWidth, "0px");
                htw.AddAttribute(HtmlTextWriterAttribute.Alt, "FairPrice?");
                if (dr.GetData<bool>("FairPrice"))  //(dr.Field<dynamic>("FairPrice"))  lam, 20130308, MSF-278, 279
                    htw.AddAttribute(HtmlTextWriterAttribute.Src, "../Images/check_green.png");
                else
                    htw.AddAttribute(HtmlTextWriterAttribute.Src, "../Images/s.gif");
                htw.RenderBeginTag(HtmlTextWriterTag.Img); //Fair Price Images
                htw.RenderEndTag(); // End Fair Price TD

                htw.AddAttribute(HtmlTextWriterAttribute.Class, "tdcheck graydiv HG");
                htw.AddStyleAttribute(HtmlTextWriterStyle.Width, "137px");
                htw.AddStyleAttribute(HtmlTextWriterStyle.VerticalAlign, "middle");
                htw.AddAttribute(HtmlTextWriterAttribute.Align, "center");
                htw.RenderBeginTag(HtmlTextWriterTag.Td); // HGRecognized TD
                if (dr.Field<dynamic>("HGDocCount") == 0)
                    htw.Write("N/A");
                else
                    if (IsPreferred && dr.Field<dynamic>("HGRecognizedDocCount") == 0)
                        htw.Write("N/A");
                    else
                        htw.Write(String.Format("{0}/{1} physicians",
                            dr.Field<dynamic>("HGRecognizedDocCount"),
                            dr.Field<dynamic>("HGDocCount")));

                htw.RenderEndTag(); // End HGRecognized TD

                htw.RenderEndTag(); //End Table Row
            }
            return sw.ToString()
                .Replace("\r", String.Empty)
                .Replace("\n", String.Empty)
                .Replace("\t", String.Empty);
        }
        public static String GetInfoHTML(this DataRow dr,Boolean isPreferred = false)
        {
            StringWriter sw = new StringWriter();
            using (HtmlTextWriter htw = new HtmlTextWriter(sw))
            {
                htw.AddAttribute(HtmlTextWriterAttribute.Class, "smaller infoWin");
                htw.AddStyleAttribute(HtmlTextWriterStyle.Width, "220px");
                htw.RenderBeginTag(HtmlTextWriterTag.P); //Info Window Paragraph definition

                //htw.AddAttribute("nav", this.Nav);
                htw.AddStyleAttribute(HtmlTextWriterStyle.Cursor, "pointer");
                htw.AddAttribute(HtmlTextWriterAttribute.Class, "readmore");
                htw.RenderBeginTag(HtmlTextWriterTag.A); //Link Facility Link button                
                htw.Write(dr.Field<dynamic>("PracticeName"));                
                htw.RenderEndTag(); //End Facility Name Link
                htw.WriteBreak();
                if (isPreferred) { htw.Write("<b>(Caesars Preferred Provider)</b><br />"); }

                htw.WriteEncodedText("Total Estimated Cost: ");
                htw.AddAttribute(HtmlTextWriterAttribute.Class, "alignright");
                htw.RenderBeginTag(HtmlTextWriterTag.B); //Range Min B
                htw.Write(String.Format("{0:c0}", dr.Field<dynamic>("RangeMin")));
                htw.RenderEndTag(); // End RangeMin B
                htw.AddAttribute(HtmlTextWriterAttribute.Class, "dashcol");
                htw.RenderBeginTag(HtmlTextWriterTag.B); //DashCol B
                htw.Write("-");
                htw.RenderEndTag(); // End DashCol B
                htw.AddAttribute(HtmlTextWriterAttribute.Class, "alignleft");
                htw.RenderBeginTag(HtmlTextWriterTag.B); //Range Max B
                htw.Write(String.Format("{0:c0}", dr.Field<dynamic>("RangeMax")));
                htw.RenderEndTag(); // End RangeMax B
                htw.WriteBreak();

                if (dr.GetData<bool>("FairPrice"))  //  lam, 20130308, MSF-278, 279
                //if (dr.Field<dynamic>("FairPrice"))
                {
                    htw.AddStyleAttribute(HtmlTextWriterStyle.Width, "23px");
                    htw.AddStyleAttribute(HtmlTextWriterStyle.Height, "23px");
                    htw.AddStyleAttribute(HtmlTextWriterStyle.BorderWidth, "0px");
                    htw.AddAttribute(HtmlTextWriterAttribute.Alt, "FairPrice?");
                    htw.AddAttribute(HtmlTextWriterAttribute.Src, "../Images/check_green.png");
                    htw.RenderBeginTag(HtmlTextWriterTag.Img); //Fair Price Images
                    htw.RenderEndTag(); // End Fair Price TD
                    htw.WriteEncodedText(" Fair Price");
                }
                htw.RenderEndTag(); //End Table Row
            }
            return sw.ToString();
        }
        public static String GetNavInfo(this DataRow dr)
        {
            QueryStringEncryption qs = new QueryStringEncryption();
            qs.UserKey = new Guid(ThisSession.UserLogginID);
            qs["PracticeName"] = dr.Field<dynamic>("PracticeName");
            qs["PracticeNPI"] = dr.Field<dynamic>("NPI");
            qs["OrganizationLocationID"] = dr.Field<dynamic>("OrganizationLocationID").ToString();
            return qs.ToString();
        }
        public static String GetDocNavInfo(this DataRow dr)
        {
            QueryStringEncryption qs = new QueryStringEncryption();
            qs.UserKey = new Guid(ThisSession.UserLogginID);
            qs["PracticeName"] = dr.Field<dynamic>("PracticeName");
            qs["ProviderName"] = dr.Field<dynamic>("ProviderName");
            qs["PracticeNPI"] = dr.Field<dynamic>("NPI");
            qs["OrganizationLocationID"] = dr.Field<dynamic>("OrganizationLocationID").ToString();
            qs["TaxID"] = dr.Field<dynamic>("TaxID");
            return qs.ToString();
        }
    }

    public static class DataExtensionMethods
    {
        public static void DataBind(this Repeater repeater, object dataSource)
        {
            repeater.DataSource = dataSource;
            repeater.DataBind();
        }
        public static void DataBind(this DropDownList ddl, object dataSource)
        {
            ddl.DataSource = dataSource;
            ddl.DataBind();
        }

        public static ClearCostWeb.Dependent DependentFromRow(this DataRow dr)
        {
            return new Dependent()
            {
                CCHID = dr.Field<int>("CCHID"),
                FirstName = dr.Field<string>("FirstName"),
                LastName = dr.Field<string>("LastName"),
                DateOfBirth = dr.Field<DateTime>("DateOfBirth"),
                Age = dr.Field<int>("Age"),
                IsAdult = dr.Field<int>("Adult") == 1 ? true : false,
                ShowAccessQuestions = dr.Field<int>("ShowAccessQuestions") == 1 ? true : false,
                RelationshipText = dr.Field<string>("RelationShipText"),
                DepToUserGranted = dr.Field<int>("DepToUserGranted") == 1 ? true : false,
                UserToDepGranted = dr.Field<int>("UserToDepGranted") == 1 ? true : false,
                Email = dr.Field<string>("Email")
            };
        }

        public static Boolean CheckForRows(this DataTable dt) { return dt.Rows.Count >= 1; }
        public static dynamic ToDyn(this DataRow dr, Dictionary<String, object> template)
        {
            var eo = new System.Dynamic.ExpandoObject();
            var eoColl = (ICollection<KeyValuePair<String, object>>)eo;

            foreach (var kvp in template)
            {
                KeyValuePair<String, object> newKvp = new KeyValuePair<string, object>(kvp.Key, dr.Field<object>(kvp.Key));
                eoColl.Add(newKvp);
            }

            dynamic eoDynamic = eo;

            return eoDynamic;
        }

        public static T GetData<T>(this DataRow dr, String Column) where T : struct
        {
            if (!dr.Table.Columns.Contains(Column)) return default(T);
            if (dr.IsNull(Column)) return default(T);
            return (T)Convert.ChangeType(dr[Column],typeof(T));
        }
    }
    
    public static class DynamicExtensionMethods
    {
        public static String ToJson(this ExpandoObject expando)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            StringBuilder json = new StringBuilder();
            List<String> keyPairs = new List<string>();
            IDictionary<string, object> dictionary = expando as IDictionary<string, object>;
            json.Append("{");
            foreach (KeyValuePair<string, object> pair in dictionary)
            {
                if (pair.Value is ExpandoObject)
                    keyPairs.Add(String.Format(@"""{0}"":{1}", pair.Key, (pair.Value as ExpandoObject).ToJson()));
                else
                    keyPairs.Add(String.Format(@"""{0}"":{1}", pair.Key, serializer.Serialize(pair.Value)));
            }

            json.Append(String.Join(",", keyPairs.ToArray()));
            json.Append("}");

            return json.ToString();
        }
    }

    public static class HttpContextExtensionMethods
    {
        public static T GetLatitude<T>(this HttpContext c) { return c.Session["PatientLatitude"].To<T>(); }
        public static T GetLongitude<T>(this HttpContext c) { return c.Session["PatientLongitude"].To<T>(); }
        public static T GetServiceID<T>(this HttpContext c) { return c.Session["ServiceID"].To<T>(); }
        public static T GetSpecialtyID<T>(this HttpContext c) { return c.Session["SpecialtyID"].To<T>(); }
        public static T GetCCHID<T>(this HttpContext c) { return c.Session["CCHID"].To<T>(); }
        public static T GetUserID<T>(this HttpContext c) { return c.Session["UserID"].To<T>(); }
        public static T GetUserLogginID<T>(this HttpContext c) { return c.Session["UserLogginID"].To<T>(); }
        public static T GetSessionID<T>(this HttpContext c) { return c.Session.SessionID.To<T>(); }
        public static T GetDomain<T>(this HttpContext c) { return c.Request.Url.Host.To<T>(); }
        public static T GetServiceName<T>(this HttpContext c) { return c.Session["ServiceName"].To<T>(); }
        public static T GetChosenDrugs<T>(this HttpContext c) { return c.Session["ChosenDrugs"].To<T>(); }
        public static T GetDrugID<T>(this HttpContext c) { return c.Session["DrugID"].To<T>(); }
        public static T GetDrugID<T>(this HttpContext c, Int32 rowIndex) { return c.GetChosenDrugs<DataTable>().Rows[rowIndex]["DrugID"].To<T>(); }
        public static T GetDrugGPI<T>(this HttpContext c) { return c.Session["DrugGPI"].To<T>(); }
        public static T GetDrugGPI<T>(this HttpContext c, Int32 rowIndex) { return c.GetChosenDrugs<DataTable>().Rows[rowIndex]["GPI"].To<T>(); }
        public static T GetDrugQuantity<T>(this HttpContext c) { return c.Session["DrugQuantity"].To<T>(); }
        public static T GetDrugQuantity<T>(this HttpContext c, Int32 rowIndex) { return c.GetChosenDrugs<DataTable>().Rows[rowIndex]["Quantity"].To<T>(); }
        public static T GetPastCareID<T>(this HttpContext c) { return c.Session["PastCareID"].To<T>(); }
        public static T GetPastCareID<T>(this HttpContext c, Int32 rowIndex) { return c.GetChosenDrugs<DataTable>().Rows[rowIndex]["PastCareID"].To<T>(); }
        public static T GetPastCareProcedureCode<T>(this HttpContext c) { return c.Session["PastCareProcedureCode"].To<T>(); }
    }

    public static class ObjectExtensionMethods
    {
        [DebuggerStepThrough]
        public static T To<T>(this object value)
        {
            try
            {
                Type t = typeof(T);
                if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>))
                    if (value == null)
                        return default(T);
                    else
                    {
                        Type valueType = t.GetGenericArguments()[0];
                        object result = Convert.ChangeType(value, valueType);
                        return (T)result;
                    }
                else
                    return (T)Convert.ChangeType(value, typeof(T));
            }
            catch (FormatException)
            {
                return default(T);
            }
            catch (InvalidCastException)
            {
                return default(T);
            }
        }
    }

    public static class RequestExtensionMethods
    {
        [DebuggerStepThrough]
        public static Boolean IsMobileBrowser(this HttpRequest req)
        {
            if (req.Browser.IsMobileDevice)
                return true;

            NameValueCollection nvc = req.ServerVariables;

            if (nvc["HTTP_X_WAP_PROFILE"] != null)
                return true;
            if (nvc["HTTP_ACCEPT"] != null && nvc["HTTP_ACCEPT"].ToLower().Contains("wap"))
                return true;
            if (nvc["HTTP_USER_AGENT"] != null)
            {
                // rld, 2013/11/12, MSB-451
                // removed 'jb' from the list, since 'tnjb' is a valid non-mobile user agent that contains the 'jb' substring
                String[] mobiles =
                    new[] {
                        "midp", "j2me", "avant", "docomo", 
                    "novarra", "palmos", "palmsource", 
                    "240x320", "opwv", "chtml",
                    "pda", "windows ce", "mmp/", 
                    "blackberry", "mib/", "symbian", 
                    "wireless", "nokia", "hand", "mobi",
                    "phone", "cdm", "up.b", "audio", 
                    "SIE-", "SEC-", "samsung", "HTC", 
                    "mot-", "mitsu", "sagem", "sony"
                    , "alcatel", "lg", "eric", "vx", 
                    "NEC", "philips", "mmm", "xx", 
                    "panasonic", "sharp", "wap", "sch",
                    "rover", "pocket", "benq", "java", 
                    "pt", "pg", "vox", "amoi", 
                    "bird", "compal", "kg", "voda",
                    "sany", "kdd", "dbt", "sendo", 
                    "sgh", "gradi", "dddi", 
                    "moto", "iphone"
                    };
                foreach (String s in mobiles)
                    if (nvc["HTTP_USER_AGENT"].ToLower().Contains(s.ToLower()))
                        return true;
            }

            return false;
        }
        public static Boolean IsTimeout(this HttpRequest req)
        {
            return req.Url.Query.Contains("timeout");
        }
        [DebuggerStepThrough]
        public static String CompatabilityWarning(this HttpRequest req)
        {
            HttpBrowserCapabilities b = req.Browser;
            String brand = b.Browser.ToLower();
            Int32 ver = b.MajorVersion;
            if (brand == "ie" && (ver == 6 || ver == 7))
                return String.Concat(
                    "<center><div class=\"compatWarn\">It appears you are using Internet Explorer ",
                    ver,
                    ".  We do not actively support this older browser.<br />We suggest you upgrade your browser for the best experience using ClearCost Health.</div></center>");
            else
                return String.Empty;
        }
        public static T EmployerID<T>(this HttpRequest req)
        {
            if (req.QueryString.AllKeys.Contains("e")
                && req.QueryString["e"] != null)
                return req.QueryString["e"].To<T>();
            else
                return default(T);
        }
    }

    public static class StringExtensions
    {
        public static String Append(this String s, String appString)
        {
            if (s == String.Empty)
                s += appString;
            else
                s += String.Concat(" and ", appString);

            return s;
        }
    }

    public static class PageExtensionMethods
    {
        public static void AddScriptToHeader(this Page page, String script)
        {
            page.Header.Controls.Add(
                new LiteralControl(
                    String.Format(
                        "<script src=\"{0}\" type=\"text/javascript\"></script>",
                        page.ResolveClientUrl("~/Scripts/" + script)
                    )
                )
            );
        }
        public static void AddCSSToHeader(this Page page, String css, Boolean isPublic = false)
        {
            String baseUrl = "~/";
            if (!isPublic) baseUrl += "SearchInfo/";
            baseUrl += "Styles/";

            page.Header.Controls.Add(
                new LiteralControl(
                    String.Format(
                        "<link href=\"{0}\" rel=\"stylesheet\" type=\"text/css\" />",
                        page.ResolveClientUrl(baseUrl + css)
                    )
                )
            );
        }
    }
}
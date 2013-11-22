using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using System.Text.RegularExpressions;
using Microsoft.Security.Application;

namespace ClearCostWeb
{
    /// <summary>
    /// Created "ThisSession" to avoid issues with spelling of some key session variables
    /// </summary>
    [Serializable]
    public class ThisSession
    {
        public ThisSession()
        { }

        #region Public Properties
        //Tabbing Related
        public enum AvailableTab
        { SavingsChoiceDashboard = 0, FindAService, FindRx, FindADoc, FindPastCare }
        public static AvailableTab SelectedTab
        {
            get
            {
                object st = HttpContext.Current.Session["SelectedTab"];
                if (st == null)
                    return AvailableTab.FindAService;
                else
                    return (AvailableTab)st;
            }
            set { HttpContext.Current.Session["SelectedTab"] = value; }
        }

        //Key Employer / Database info:
        public static string EmployerID
        {
            get { return HttpContext.Current.Session["EmployerID"].ToString(); }
            set { HttpContext.Current.Session["EmployerID"] = value; }
        }
        public static string CnxString
        {
            get { return HttpContext.Current.Session["CnxString"].ToString(); }
            set { HttpContext.Current.Session["CnxString"] = value; }
        }
        public static string EmployerName
        {
            get { return (HttpContext.Current.Session["EmployerName"] == null ? String.Empty : HttpContext.Current.Session["EmployerName"].ToString()); }
            set { HttpContext.Current.Session["EmployerName"] = value; }
        }
        public static string Insurer
        {
            get { return HttpContext.Current.Session["Insurer"].ToString(); }
            set { HttpContext.Current.Session["Insurer"] = value; }
        }
        public static string RXProvider
        {
            get { return HttpContext.Current.Session["RXProvider"].ToString(); }
            set { HttpContext.Current.Session["RXProvider"] = value; }
        }

        public static bool ShowYourCostColumn
        {
            get { return bool.Parse(HttpContext.Current.Session["ShowYourCostColumn"].ToString()); }
            set { HttpContext.Current.Session["ShowYourCostColumn"] = value; }
        }

        //Key Employee info:
        public static Boolean LoggedIn
        {
            get { return (HttpContext.Current.Session["LoggedIn"] != null ? Boolean.Parse(HttpContext.Current.Session["LoggedIn"].ToString()) : false); }
            set { HttpContext.Current.Session["LoggedIn"] = value; }
        }
        public static int CCHID
        {
            get
            {
                return
                    int.Parse(HttpContext.Current.Session["CCHID"].ToString());
            }
            set { HttpContext.Current.Session["CCHID"] = value; }
        }

        public static int ReffralCCHID
        {
            get
            {
                return
                    int.Parse(HttpContext.Current.Session["ReffralCCHID"].ToString());
            }
            set { HttpContext.Current.Session["ReffralCCHID"] = value; }
        }

        public static string EmployeeID
        {
            get { return HttpContext.Current.Session["EmployeeID"].ToString(); }
            set { HttpContext.Current.Session["EmployeeID"] = value; }
        }
        public static string SubscriberMedicalID
        {
            get { return HttpContext.Current.Session["SubscriberMedicalID"].ToString(); }
            set { HttpContext.Current.Session["SubscriberMedicalID"] = value; }
        }
        public static string SubscriberRXID
        {
            get { return HttpContext.Current.Session["SubscriberRXID"].ToString(); }
            set { HttpContext.Current.Session["SubscriberRXID"] = value; }
        }
        public static string FirstName
        {
            get { return HttpContext.Current.Session["FirstName"].ToString(); }
            set { HttpContext.Current.Session["FirstName"] = value; }
        }
        public static string LastName
        {
            get { return HttpContext.Current.Session["LastName"].ToString(); }
            set { HttpContext.Current.Session["LastName"] = value; }
        }
        public static string PatientAddress1
        {
            get { return HttpContext.Current.Session["PatientAddress1"].ToString(); }
            set { HttpContext.Current.Session["PatientAddress1"] = value; }
        }
        public static string PatientAddress2
        {
            get { return HttpContext.Current.Session["PatientAddress2"].ToString(); }
            set { HttpContext.Current.Session["PatientAddress2"] = value; }
        }
        public static string PatientCity
        {
            get { return HttpContext.Current.Session["PatientCity"].ToString(); }
            set { HttpContext.Current.Session["PatientCity"] = value; }
        }
        public static string PatientState
        {
            get { return HttpContext.Current.Session["PatientState"].ToString(); }
            set { HttpContext.Current.Session["PatientState"] = value; }
        }
        public static string PatientZipCode
        {
            get { return HttpContext.Current.Session["PatientZipCode"].ToString(); }
            set { HttpContext.Current.Session["PatientZipCode"] = value; }
        }
        public static string PatientEmail
        {
            get { return (HttpContext.Current.Session["PatientEmail"] != null ? HttpContext.Current.Session["PatientEmail"].ToString() : ""); }
            set { HttpContext.Current.Session["PatientEmail"] = value; }
        }
        public static string PatientDateOfBirth
        {
            get { return HttpContext.Current.Session["PatientDateOfBirth"].ToString(); }
            set { HttpContext.Current.Session["PatientDateOfBirth"] = value; }
        }
        public static string DefaultPatientAddress
        {
            get { return HttpContext.Current.Session["DefaultPatientAddress"].ToString(); }
            set { HttpContext.Current.Session["DefaultPatientAddress"] = value; }
        }
        public static string PatientAddressSingleLine
        {
            get
            {
                string AddressLine = string.Empty;
                //Deal with existance of second address line.
                if (PatientAddress2 == string.Empty)
                {
                    AddressLine = PatientAddress1 + ", " + PatientCity + ", " + PatientState;
                }
                else
                {
                    AddressLine = PatientAddress1 + " " + PatientAddress2 + ", " + PatientCity + ", " + PatientState;
                }
                //Deal with format of 9 digit zip.
                if (PatientZipCode.Length > 5)
                { AddressLine = AddressLine + " " + PatientZipCode.Substring(0, 5) + "-" + PatientZipCode.Substring(5); }
                else
                { AddressLine = AddressLine + " " + PatientZipCode; }

                return AddressLine;
            }
        }
        public static string PatientAddressTwoLines
        {
            get
            {
                string AddressLine = string.Empty;
                //Deal with existance of second address line.
                if (PatientAddress2 == string.Empty)
                {
                    AddressLine = PatientAddress1 + "<br />" + PatientCity + ", " + PatientState;
                }
                else
                {
                    AddressLine = PatientAddress1 + " " + PatientAddress2 + "<br />" + PatientCity + ", " + PatientState;
                }
                //Deal with format of 9 digit zip.
                if (PatientZipCode.Length > 5)
                { AddressLine = AddressLine + " " + PatientZipCode.Substring(0, 5) + "-" + PatientZipCode.Substring(5); }
                else
                { AddressLine = AddressLine + " " + PatientZipCode; }

                return AddressLine;
            }
        }
        public static string PatientLongitude
        {
            get { return HttpContext.Current.Session["PatientLongitude"].ToString(); }
            set { HttpContext.Current.Session["PatientLongitude"] = value; }
        }
        public static string PatientLatitude
        {
            get { return HttpContext.Current.Session["PatientLatitude"].ToString(); }
            set { HttpContext.Current.Session["PatientLatitude"] = value; }
        }
        public static string PatientAddressForGoogle
        {
            get
            {
                string AddressLine = string.Empty;
                //Deal with existance of second address line.
                if (PatientAddress2 == string.Empty)
                {
                    AddressLine = PatientAddress1.Replace(" ", "+") + "+" + PatientCity.Replace(" ", "+") + "+" + PatientState;
                }
                else
                {
                    AddressLine = PatientAddress1.Replace(" ", "+") + "+" + PatientAddress2.Replace(" ", "+") + "+" + PatientCity.Replace(" ", "+") + "+" + PatientState;
                }
                //Ignore 9 digit zip - not sure how Google will deal.
                AddressLine = AddressLine + "+" + PatientZipCode.Substring(0, 5);

                return AddressLine;
            }
        }
        public static string PatientPhone
        {
            get { return HttpContext.Current.Session["PatientPhone"].ToString(); }
            set { HttpContext.Current.Session["PatientPhone"] = value; }
        }
        public static string HealthPlanType
        {
            get { return HttpContext.Current.Session["HealthPlanType"].ToString(); }
            set { HttpContext.Current.Session["HealthPlanType"] = value; }
        }
        public static string MedicalPlanType
        {
            get { return HttpContext.Current.Session["MedicalPlanType"].ToString(); }
            set { HttpContext.Current.Session["MedicalPlanType"] = value; }
        }
        public static string RxPlanType
        {
            get { return HttpContext.Current.Session["RxPlanType"].ToString(); }
            set { HttpContext.Current.Session["RxPlanType"] = value; }
        }
        public static string UserLogginID
        {
            get { return HttpContext.Current.Session["UserLogginID"].ToString(); }
            set { HttpContext.Current.Session["UserLogginID"] = value; }
        }
        public static string PatientGender
        {
            get { return HttpContext.Current.Session["Gender"].ToString(); }
            set { HttpContext.Current.Session["Gender"] = value; }
        }
        public static bool Parent
        {
            get { return Convert.ToBoolean(HttpContext.Current.Session["Parent"].ToString()); }
            set { HttpContext.Current.Session["Parent"] = value; }
        }
        public static bool Adult
        {
            get { return Convert.ToBoolean(HttpContext.Current.Session["Adult"].ToString()); }
            set { HttpContext.Current.Session["Adult"] = value; }
        }
        public static bool OptInIncentiveProgram
        {
            get
            {
                Boolean? b = (Boolean?)HttpContext.Current.Session["OptInIncentiveProgram"];
                return (b.HasValue ? b.Value : false);
            }
            set { HttpContext.Current.Session["OptInIncentiveProgram"] = value; }
        }
        public static bool OptInEmailAlerts
        {
            get
            {
                Boolean? b = (Boolean?)HttpContext.Current.Session["OptInEmailAlerts"];
                return (b.HasValue ? b.Value : false);
            }
            set { HttpContext.Current.Session["OptInEmailAlerts"] = value; }
        }
        public static bool OptInTextMsgAlerts
        {
            get
            {
                Boolean? b = (Boolean?)HttpContext.Current.Session["OptInTextMsgAlerts"];
                return (b.HasValue ? b.Value : false);
            }
            set { HttpContext.Current.Session["OptInTextMsgAlerts"] = value; }
        }
        public static string MobilePhone
        {
            get { return HttpContext.Current.Session["MobilePhone"].ToString(); }
            set { HttpContext.Current.Session["MobilePhone"] = value; }
        }
        public static bool OptInPriceConcierge
        {
            get
            {
                Boolean? b = (Boolean?)HttpContext.Current.Session["OptInPriceConcierge"];
                return (b.HasValue ? b.Value : false);
            }
            set { HttpContext.Current.Session["OptInPriceConcierge"] = value; }
        }
        public static String CurrentSecurityQuestion
        {
            get { return (HttpContext.Current.Session["CurrentSecurityQuestion"] == null ? String.Empty : HttpContext.Current.Session["CurrentSecurityQuestion"].ToString()); }
            set { HttpContext.Current.Session["CurrentSecurityQuestion"] = value; }
        }
        public static String[] CurrentAvailableSecurityQuestions
        {
            get { return (String[])HttpContext.Current.Session["CurrentAvailableSecurityQuestions"]; }
            set { HttpContext.Current.Session["CurrentAvailableSecurityQuestions"] = value; }
        }


        //"You could have saved" info
        public static int YouCouldHaveSaved
        {
            get
            {
                return
                    int.Parse(HttpContext.Current.Session["YouCouldHaveSaved"].ToString());
            }
            set { HttpContext.Current.Session["YouCouldHaveSaved"] = value; }
        }
        public static string YouCouldHaveSavedDisplay
        {
            get
            {
                return
                    String.Format("{0:c0}", HttpContext.Current.Session["YouCouldHaveSaved"]);
            }
        }

        //Dependents of the employee
        public static Dependents Dependents
        {
            get { return (Dependents)HttpContext.Current.Session["Dependents"]; }
            set { HttpContext.Current.Session["Dependents"] = value; }
        }

        // Service Information
        public static string ServiceName
        {
            get { return HttpContext.Current.Session["ServiceName"].ToString(); }
            set { HttpContext.Current.Session["ServiceName"] = value; }
        }
        public static int ServiceID
        {
            get { return int.Parse(HttpContext.Current.Session["ServiceID"].ToString()); }
            set { HttpContext.Current.Session["ServiceID"] = value; }
        }
        public static string PastCareProcedureCode // used for PastCare lookups.
        {
            get { return HttpContext.Current.Session["PastCareProcedureCode"].ToString(); }
            set { HttpContext.Current.Session["PastCareProcedureCode"] = value; }
        }
        public static string ServiceEnteredFrom // Did the user type an entry? Select from list?
        {
            get { return HttpContext.Current.Session["ServiceEnteredFrom"].ToString(); }
            set { HttpContext.Current.Session["ServiceEnteredFrom"] = value; }
        }
        public static string ServiceEntered // Actual entered or selected Service Name (if from place where it could be different from saved name)
        {
            get { return HttpContext.Current.Session["ServiceEntered"].ToString(); }
            set { HttpContext.Current.Session["ServiceEntered"] = value; }
        }
        public static Boolean IsShotVaccine
        {
            get { return (HttpContext.Current.Session["ShotVaccine"] == null ? false : Convert.ToBoolean(HttpContext.Current.Session["ShotVaccine"].ToString())); }
            set { HttpContext.Current.Session["ShotVaccine"] = value; }
        }
        public static DataTable ChosenLabs
        {
            get { return (DataTable)HttpContext.Current.Session["ChosenLabs"]; }
            set { HttpContext.Current.Session["ChosenLabs"] = (DataTable)value; }
        }
        public static Int32 OrganizationLocationID
        {
            get { return Int32.Parse(HttpContext.Current.Session["OrganizationLocationID"].ToString()); }
            set { HttpContext.Current.Session["OrganizationLocationID"] = value; }
        }
        // Drug Information
        public static string DrugName
        {
            get { return HttpContext.Current.Session["DrugName"].ToString().ToUpper(); }
            set { HttpContext.Current.Session["DrugName"] = value; }
        }
        public static string DrugEnteredFrom
        {
            get { return HttpContext.Current.Session["DrugEnteredFrom"].ToString(); }
            set { HttpContext.Current.Session["DrugEnteredFrom"] = value; }
        }
        public static string DrugEntered
        {
            get { return HttpContext.Current.Session["DrugEntered"].ToString(); }
            set { HttpContext.Current.Session["DrugEntered"] = value; }
        }
        public static string DrugID
        {
            get { return HttpContext.Current.Session["DrugID"].ToString(); }
            set { HttpContext.Current.Session["DrugID"] = value; }
        }
        public static string DrugGPI
        {
            get { return HttpContext.Current.Session["DrugGPI"].ToString(); }
            set { HttpContext.Current.Session["DrugGPI"] = value; }
        }
        public static string DrugStrength
        {
            get { return HttpContext.Current.Session["DrugStrength"].ToString(); }
            set { HttpContext.Current.Session["DrugStrength"] = value; }
        }
        public static string DrugQuantity
        {
            get { return HttpContext.Current.Session["DrugQuantity"].ToString(); }
            set { HttpContext.Current.Session["DrugQuantity"] = value; }
        }
        public static string DrugQuantityUOM
        {
            get { return HttpContext.Current.Session["DrugQuantityUOM"].ToString(); }
            set { HttpContext.Current.Session["DrugQuantityUOM"] = value; }
        }
        public static String PastCareID
        {
            get { return HttpContext.Current.Session["PastCareID"].ToString(); }
            set { HttpContext.Current.Session["PastCareID"] = value; }
        }
        public static DataTable ChosenDrugs
        {
            get { return (DataTable)HttpContext.Current.Session["ChosenDrugs"]; }
            set { HttpContext.Current.Session["ChosenDrugs"] = (DataTable)value; }
        }
        public static String ChosenLabLocationID
        {
            get { return HttpContext.Current.Session["ChosenLabLocationID"].ToString(); }
            set { HttpContext.Current.Session["ChosenLabLocationID"] = value; }
        }
        public static String ChosenLabID
        {
            get { return HttpContext.Current.Session["ChosenLabID"].ToString(); }
            set { HttpContext.Current.Session["ChosenLabID"] = value; }
        }
        public static String TaxID
        {
            get { return HttpContext.Current.Session["TaxID"].ToString(); }
            set { HttpContext.Current.Session["TaxID"] = value; }
        }

        //Pharmacy Info
        public static String PharmacyID
        {
            get { return HttpContext.Current.Session["PharmacyID"].ToString(); }
            set { HttpContext.Current.Session["PharmacyID"] = value; }
        }
        public static String PharmacyLocationID
        {
            get { return HttpContext.Current.Session["PharmacyLocationID"].ToString(); }
            set { HttpContext.Current.Session["PharmacyLocationID"] = value; }
        }
        public static String CurrentPharmacyID
        {
            get { return HttpContext.Current.Session["CurrentPharmacyID"].ToString(); }
            set { HttpContext.Current.Session["CurrentPharmacyID"] = value; }
        }
        public static String CurrentPharmacyLocationID
        {
            get { return HttpContext.Current.Session["CurrentPharmacyLocationID"].ToString(); }
            set { HttpContext.Current.Session["CurrentPharmacyLocationID"] = value; }
        }
        public static String CurrentPharmacyPrice
        {
            get { return (HttpContext.Current.Session["CurrentPharmacyPrice"].ToString() == "" ? "0" : HttpContext.Current.Session["CurrentPharmacyPrice"].ToString()); }
            set { HttpContext.Current.Session["CurrentPharmacyPrice"] = value; }
        }

        //Specialty Info
        public static int SpecialtyID
        {
            get { return int.Parse(HttpContext.Current.Session["SpecialtyID"].ToString()); }
            set { HttpContext.Current.Session["SpecialtyID"] = value; }
        }
        public static string Specialty
        {
            get { return HttpContext.Current.Session["Specialty"].ToString(); }
            set { HttpContext.Current.Session["Specialty"] = value; }
        }

        //Facility Information
        public static string PastCareFacilityName // used for comparison display for PastCare lookups.
        {
            get { return HttpContext.Current.Session["PastCareFacilityName"].ToString(); }
            set { HttpContext.Current.Session["PastCareFacilityName"] = value; }
        }
        public static string PracticeName
        {
            get { return HttpContext.Current.Session["PracticeName"].ToString(); }
            set { HttpContext.Current.Session["PracticeName"] = value; }
        }
        public static string ProviderName //Need both practice and provider name for Find a Doc
        {
            get { return HttpContext.Current.Session["ProviderName"].ToString(); }
            set { HttpContext.Current.Session["ProviderName"] = value; }
        }
        public static string PracticeNPI
        {
            get { return HttpContext.Current.Session["PracticeNPI"].ToString(); }
            set { HttpContext.Current.Session["PracticeNPI"] = value; }
        }
        public static string FacilityAddress1
        {
            get { return HttpContext.Current.Session["FacilityAddress1"].ToString(); }
            set { HttpContext.Current.Session["FacilityAddress1"] = value; }
        }
        public static string FacilityAddress2
        {
            get { return HttpContext.Current.Session["FacilityAddress2"].ToString(); }
            set { HttpContext.Current.Session["FacilityAddress2"] = value; }
        }
        public static string FacilityCity
        {
            get { return HttpContext.Current.Session["FacilityCity"].ToString(); }
            set { HttpContext.Current.Session["FacilityCity"] = value; }
        }
        public static string FacilityState
        {
            get { return HttpContext.Current.Session["FacilityState"].ToString(); }
            set { HttpContext.Current.Session["FacilityState"] = value; }
        }
        public static string FacilityZipCode
        {
            get { return HttpContext.Current.Session["FacilityZipCode"].ToString(); }
            set { HttpContext.Current.Session["FacilityZipCode"] = value; }
        }
        public static string FacilityZipCodeFormatted
        {
            get
            {
                //Deal with format of 9 digit zip.
                string newZip = FacilityZipCode.Substring(0, 5);
                if (FacilityZipCode.Length > 5) newZip += "-" + FacilityZipCode.Substring(5);

                return newZip;
            }
        }
        public static string FacilityLongitude
        {
            get { return HttpContext.Current.Session["FacilityLongitude"].ToString(); }
            set { HttpContext.Current.Session["FacilityLongitude"] = value; }
        }
        public static string FacilityLatitude
        {
            get { return HttpContext.Current.Session["FacilityLatitude"].ToString(); }
            set { HttpContext.Current.Session["FacilityLatitude"] = value; }
        }
        /// <summary>
        /// We should have Latitutude/Longitude, but just in case.
        /// </summary>
        public static string FacilityAddressForGoogle
        {
            get
            {
                string AddressLine = string.Empty;
                //Deal with existance of second address line.
                if (FacilityAddress2 == string.Empty)
                {
                    AddressLine = FacilityAddress1.Replace(" ", "+") + "+" + FacilityCity.Replace(" ", "+") + "+" + FacilityState;
                }
                else
                {
                    AddressLine = FacilityAddress1.Replace(" ", "+") + "+" + FacilityAddress2.Replace(" ", "+") + "+" + FacilityCity.Replace(" ", "+") + "+" + FacilityState;
                }
                //Ignore 9 digit zip - not sure how Google will deal.
                AddressLine = AddressLine + "+" + FacilityZipCode.Substring(0, 5);

                return AddressLine;
            }
        }
        public static string FacilityTelephone
        {
            get { return HttpContext.Current.Session["FacilityTelephone"].ToString(); }
            set { HttpContext.Current.Session["FacilityTelephone"] = value; }
        }
        public static string FacilityTelephoneFormatted
        {
            get
            {
                //  lam, 20130403, MSF-271 telephone number format to filter out non-digit before formatting
                //  also, don't show any telephone numbers that is less than 10 characters
                Regex nonNumeric = new Regex(@"[^0-9]");
                string newPhone = nonNumeric.Replace(FacilityTelephone, String.Empty);

                if (newPhone.Trim() != String.Empty && newPhone.Trim().Length >= 10)
                {
                    newPhone = "(" + newPhone.Substring(0, 3) + ") " + newPhone.Substring(3, 3) + "-" + newPhone.Substring(6);
                    return newPhone;
                }
                else
                {
                    return String.Empty;
                }
            }
        }

        //Facility/Patient/Service specific
        public static string FacilityDistance
        {
            get { return HttpContext.Current.Session["FacilityDistance"].ToString(); }
            set { HttpContext.Current.Session["FacilityDistance"] = value; }
        }
        public static string FacilityFairPrice
        {
            get { return HttpContext.Current.Session["FacilityFairPrice"].ToString(); }
            set { HttpContext.Current.Session["FacilityFairPrice"] = value; }
        }
        public static string FacilityHGRecognized
        {
            get { return HttpContext.Current.Session["FacilityHGRecognized"].ToString(); }
            set { HttpContext.Current.Session["FacilityHGRecognized"] = value; }
        }

        //This may be replaced with better solution - created to work with google distances.
        public static DataTable FacilityList
        {
            get { return (DataTable)HttpContext.Current.Session["FacilityList"]; }
            set { HttpContext.Current.Session["FacilityList"] = value; }
        }

        public static String PrevPage
        {
            get { return HttpContext.Current.Session["PrevPage"].ToString(); }
            set { HttpContext.Current.Session["PrevPage"] = value; }
        }

        //Error Handling
        public static Exception AppException
        {
            get { return (Exception)HttpContext.Current.Session["AppException"]; }
            set { HttpContext.Current.Session["AppException"] = value; }
        }

        //Employer Content Information
        public static String InsurerName
        {
            get { return HttpContext.Current.Session["InsurerName"].ToString(); }
            set { HttpContext.Current.Session["InsurerName"] = value; }
        }
        public static String LogoImageName
        {
            get { return HttpContext.Current.Session["LogoImageName"].ToString(); }
            set { HttpContext.Current.Session["LogoImageName"] = value; }
        }
        public static String EmployerPhone
        {
            get { return HttpContext.Current.Session["EmployerPhone"].ToString(); }
            set { HttpContext.Current.Session["EmployerPhone"] = value; }
        }
        public static Boolean InternalLogo
        {
            get { return Boolean.Parse(HttpContext.Current.Session["InternalLogo"].ToString()); }
            set { HttpContext.Current.Session["InternalLogo"] = value; }
        }
        public static String ContactText
        {
            get { return HttpContext.Current.Session["ContactText"].ToString(); }
            set { HttpContext.Current.Session["ContactText"] = value; }
        }
        public static String SpecialtyNetworkText
        {
            get { return HttpContext.Current.Session["SpecialtyNetworkText"].ToString(); }
            set { HttpContext.Current.Session["SpecialtyNetworkText"] = value; }
        }
        public static String PastCareDisclaimerText  //  lam, 20130418, MSF-290
        {
            get { return HttpContext.Current.Session["PastCareDisclaimerText"].ToString(); }
            set { HttpContext.Current.Session["PastCareDisclaimerText"] = value; }
        }
        public static String RxResultDisclaimerText  //  lam, 20130418, MSF-294
        {
            get { return HttpContext.Current.Session["RxResultDisclaimerText"].ToString(); }
            set { HttpContext.Current.Session["RxResultDisclaimerText"] = value; }
        }
        public static String AllResult1DisclaimerText  //  lam, 20130425, MSF-295
        {
            get { return HttpContext.Current.Session["AllResult1DisclaimerText"].ToString(); }
            set { HttpContext.Current.Session["AllResult1DisclaimerText"] = value; }
        }
        public static String AllResult2DisclaimerText  //  lam, 20130425, MSF-295
        {
            get { return HttpContext.Current.Session["AllResult2DisclaimerText"].ToString(); }
            set { HttpContext.Current.Session["AllResult2DisclaimerText"] = value; }
        }
        public static String SpecialtyDrugDisclaimerText  //  lam, 20130418, CI-59
        {
            get { return HttpContext.Current.Session["SpecialtyDrugDisclaimerText"].ToString(); }
            set { HttpContext.Current.Session["SpecialtyDrugDisclaimerText"] = value; }
        }
        public static String MentalHealthDisclaimerText  //  lam, 20130508, CI-144
        {
            get { return HttpContext.Current.Session["MentalHealthDisclaimerText"].ToString(); }
            set { HttpContext.Current.Session["MentalHealthDisclaimerText"] = value; }
        }
        public static String ServiceNotFoundDisclaimerText  //  lam, 20130604, MSF-377
        {
            get { return HttpContext.Current.Session["ServiceNotFoundDisclaimerText"].ToString(); }
            set { HttpContext.Current.Session["ServiceNotFoundDisclaimerText"] = value; }
        }
        public static Boolean DefaultYourCostOn
        {
            get { return Boolean.Parse(HttpContext.Current.Session["DefaultYourCostOn"].ToString()); }
            set { HttpContext.Current.Session["DefaultYourCostOn"] = value; }
        }
        public static String DefaultSort
        {
            get { return HttpContext.Current.Session["DefaultSort"].ToString(); }
            set { HttpContext.Current.Session["DefaultSort"] = value; }
        }
        public static Boolean AllowFairPriceSort  //  lam, 20130618, MSB-324
        {
            get { return Boolean.Parse(HttpContext.Current.Session["AllowFairPriceSort"].ToString()); }
            set { HttpContext.Current.Session["AllowFairPriceSort"] = value; }
        }
        public static Boolean SavingsChoiceEnabled
        {
            get { return Boolean.Parse(HttpContext.Current.Session["SavingsChoiceEnabled"].ToString()); }
            set { HttpContext.Current.Session["SavingsChoiceEnabled"] = value; }
        }
        public static Boolean ShowSCIQTab  //  lam, 20130816, SCIQ-77
        {
            get { return Boolean.Parse(HttpContext.Current.Session["ShowSCIQTab"].ToString()); }
            set { HttpContext.Current.Session["ShowSCIQTab"] = value; }
        }
        #endregion

        #region Public Methods

        public static void ClearSessionVariables()
        {
            HttpContext.Current.Session["FromCallCenter"] = false;

            HttpContext.Current.Session["LoggedIn"] = false;
            HttpContext.Current.Session["EmployerID"] = string.Empty;
            HttpContext.Current.Session["CnxString"] = string.Empty;
            HttpContext.Current.Session["EmployerName"] = string.Empty;
            HttpContext.Current.Session["Insurer"] = string.Empty;
            HttpContext.Current.Session["RXProvider"] = string.Empty;
            HttpContext.Current.Session["ShowYourCostColumn"] = false;
            HttpContext.Current.Session["SavingsChoiceEnabled"] = false;

            HttpContext.Current.Session["CCHID"] = 0;
            HttpContext.Current.Session["ReffralCCHID"] = 0;
            HttpContext.Current.Session["EmployeeID"] = string.Empty;
            HttpContext.Current.Session["SubscriberMedicalID"] = string.Empty;
            HttpContext.Current.Session["SubscriberRXID"] = string.Empty;
            HttpContext.Current.Session["FirstName"] = string.Empty;
            HttpContext.Current.Session["LastName"] = string.Empty;
            HttpContext.Current.Session["PatientAddress1"] = string.Empty;
            HttpContext.Current.Session["PatientAddress2"] = string.Empty;
            HttpContext.Current.Session["PatientCity"] = string.Empty;
            HttpContext.Current.Session["PatientState"] = string.Empty;
            HttpContext.Current.Session["PatientZipCode"] = string.Empty;
            HttpContext.Current.Session["PatientLongitude"] = string.Empty;
            HttpContext.Current.Session["PatientLatitude"] = string.Empty;
            HttpContext.Current.Session["PatientEmail"] = string.Empty;
            HttpContext.Current.Session["PatientDateOfBirth"] = string.Empty;
            HttpContext.Current.Session["YouCouldHaveSaved"] = 0;
            HttpContext.Current.Session["Dependents"] = new Dependents();
            HttpContext.Current.Session["ServiceName"] = string.Empty;
            HttpContext.Current.Session["ServiceID"] = 0;
            HttpContext.Current.Session["PastCareProcedureCode"] = string.Empty;
            HttpContext.Current.Session["ServiceEnteredFrom"] = string.Empty;
            HttpContext.Current.Session["ServiceEntered"] = string.Empty;
            HttpContext.Current.Session["PatientPhone"] = string.Empty;
            HttpContext.Current.Session["HealthPlanType"] = string.Empty;
            HttpContext.Current.Session["MedicalPlanType"] = string.Empty;
            HttpContext.Current.Session["RxPlanType"] = string.Empty;
            HttpContext.Current.Session["ChosenLabs"] = null;
            HttpContext.Current.Session["UserLogginID"] = string.Empty;
            HttpContext.Current.Session["Gender"] = string.Empty;
            HttpContext.Current.Session["Parent"] = false;
            HttpContext.Current.Session["Adult"] = false;
            HttpContext.Current.Session["OptInIncentiveProgram"] = false;
            HttpContext.Current.Session["OptInEmailAlerts"] = false;
            HttpContext.Current.Session["OptInTextMsgAlerts"] = false;
            HttpContext.Current.Session["MobilePhone"] = string.Empty;
            HttpContext.Current.Session["OptInPriceConcierge"] = false;
            HttpContext.Current.Session["CurrentSecurityQuestion"] = string.Empty;
            HttpContext.Current.Session["CurrentAvailableSecurityQuestions"] = null;

            HttpContext.Current.Session["DrugName"] = string.Empty;
            HttpContext.Current.Session["DrugEnteredFrom"] = string.Empty;
            HttpContext.Current.Session["DrugEntered"] = string.Empty;
            HttpContext.Current.Session["DrugID"] = string.Empty;
            HttpContext.Current.Session["DrugGPI"] = string.Empty;
            HttpContext.Current.Session["DrugStrength"] = string.Empty;
            HttpContext.Current.Session["DrugQuantity"] = string.Empty;
            HttpContext.Current.Session["DrugQuantityUOM"] = string.Empty;
            HttpContext.Current.Session["PastCareID"] = string.Empty;
            HttpContext.Current.Session["ChosenDrugs"] = null;
            HttpContext.Current.Session["ChosenLabLocationID"] = string.Empty;
            HttpContext.Current.Session["ChosenLabID"] = string.Empty;

            HttpContext.Current.Session["PharmacyID"] = string.Empty;
            HttpContext.Current.Session["PharmacyLocationID"] = string.Empty;
            HttpContext.Current.Session["CurrentPharmacyID"] = string.Empty;
            HttpContext.Current.Session["CurrentPharmacyLocationID"] = string.Empty;
            HttpContext.Current.Session["CurrentPharmacyPrice"] = string.Empty;

            HttpContext.Current.Session["SpecialtyID"] = -1;
            HttpContext.Current.Session["Specialty"] = string.Empty;
            HttpContext.Current.Session["PracticeName"] = string.Empty;
            HttpContext.Current.Session["ProviderName"] = string.Empty;
            HttpContext.Current.Session["PracticeNPI"] = string.Empty;
            HttpContext.Current.Session["FacilityAddress1"] = string.Empty;
            HttpContext.Current.Session["FacilityAddress2"] = string.Empty;
            HttpContext.Current.Session["FacilityCity"] = string.Empty;
            HttpContext.Current.Session["FacilityState"] = string.Empty;
            HttpContext.Current.Session["FacilityZipCode"] = string.Empty;
            HttpContext.Current.Session["FacilityLongitude"] = string.Empty;
            HttpContext.Current.Session["FacilityLatitude"] = string.Empty;
            HttpContext.Current.Session["FacilityTelephone"] = string.Empty;
            HttpContext.Current.Session["PastCareFacilityName"] = string.Empty;
            HttpContext.Current.Session["FacilityDistance"] = string.Empty;
            HttpContext.Current.Session["FacilityFairPrice"] = string.Empty;
            HttpContext.Current.Session["FacilityHGRecognized"] = string.Empty;

            HttpContext.Current.Session["FacilityList"] = new DataTable();

            HttpContext.Current.Session["PrevPage"] = String.Empty;
        }


        public static void ClearSearchSessionVariables()
        {

            HttpContext.Current.Session["ServiceName"] = string.Empty;
            HttpContext.Current.Session["ServiceID"] = 0;
            HttpContext.Current.Session["PastCareProcedureCode"] = string.Empty;
            HttpContext.Current.Session["ServiceEnteredFrom"] = string.Empty;
            HttpContext.Current.Session["ServiceEntered"] = string.Empty;

            HttpContext.Current.Session["DrugName"] = string.Empty;
            HttpContext.Current.Session["DrugEnteredFrom"] = string.Empty;
            HttpContext.Current.Session["DrugEntered"] = string.Empty;
            HttpContext.Current.Session["DrugID"] = string.Empty;
            HttpContext.Current.Session["DrugGPI"] = string.Empty;
            HttpContext.Current.Session["DrugStrength"] = string.Empty;
            HttpContext.Current.Session["DrugQuantity"] = string.Empty;
            HttpContext.Current.Session["DrugQuantityUOM"] = string.Empty;
            HttpContext.Current.Session["PastCareID"] = string.Empty;
            HttpContext.Current.Session["ChosenDrugs"] = null;
            HttpContext.Current.Session["ChosenLabs"] = null;
            HttpContext.Current.Session["ChosenLabLocationID"] = string.Empty;
            HttpContext.Current.Session["ChosenLabID"] = string.Empty;

            HttpContext.Current.Session["PharmacyID"] = string.Empty;
            HttpContext.Current.Session["PharmacyLocationID"] = string.Empty;
            HttpContext.Current.Session["CurrentPharmacyID"] = string.Empty;
            HttpContext.Current.Session["CurrentPharmacyLocationID"] = string.Empty;
            HttpContext.Current.Session["CurrentPharmacyPrice"] = string.Empty;

            HttpContext.Current.Session["SpecialtyID"] = -1;
            HttpContext.Current.Session["Specialty"] = string.Empty;
            HttpContext.Current.Session["PracticeName"] = string.Empty;
            HttpContext.Current.Session["ProviderName"] = string.Empty;
            HttpContext.Current.Session["PracticeNPI"] = string.Empty;
            HttpContext.Current.Session["FacilityAddress1"] = string.Empty;
            HttpContext.Current.Session["FacilityAddress2"] = string.Empty;
            HttpContext.Current.Session["FacilityCity"] = string.Empty;
            HttpContext.Current.Session["FacilityState"] = string.Empty;
            HttpContext.Current.Session["FacilityZipCode"] = string.Empty;
            HttpContext.Current.Session["FacilityLongitude"] = string.Empty;
            HttpContext.Current.Session["FacilityLatitude"] = string.Empty;
            HttpContext.Current.Session["FacilityTelephone"] = string.Empty;
            HttpContext.Current.Session["PastCareFacilityName"] = string.Empty;
            HttpContext.Current.Session["FacilityDistance"] = string.Empty;
            HttpContext.Current.Session["FacilityFairPrice"] = string.Empty;
            HttpContext.Current.Session["FacilityHGRecognized"] = string.Empty;

            HttpContext.Current.Session["FacilityList"] = new DataTable();
        }

        #endregion
    }
}
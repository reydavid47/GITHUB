<%@ WebHandler Language="C#" Class="ClearCostWeb.Handlers.SCRatings" %>

using System;
using System.Web;
using System.Collections.Specialized;
using System.Web.SessionState;
using System.Data;
using System.IO;

namespace ClearCostWeb.Handlers
{
    public class SCRatings : IHttpHandler, IReadOnlySessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            NameValueCollection qs = context.Request.QueryString;
            if (qs.Count == 0)
                qs = context.Request.Form;
            int cchid, providerId, score, employerId, organizationID;
            string category, review, type, cchids;

            if (int.TryParse(qs["cchid"], out cchid) &&
                int.TryParse(qs["pid"], out providerId) &&
                int.TryParse(qs["oid"], out organizationID) &&
                int.TryParse(qs["rating"], out score) &&
                int.TryParse(qs["eid"], out employerId) &&
                (qs["category"] ?? "") != string.Empty)// && (qs["review"] ?? "") != string.Empty)
            {
                cchids = qs["cchids"];
                category = qs["category"];
                review = qs["review"];
                type = qs["type"];
                switch (type) {
                    case "existing":
                        using (sc_InsertUpdateProviderRating scIUPR = new sc_InsertUpdateProviderRating())
                        {
                            scIUPR.CCHID = cchid;
                            scIUPR.ProviderID = providerId;
                            scIUPR.Score = score;
                            scIUPR.EmployerID = employerId;
                            scIUPR.Category = category;
                            scIUPR.Review = review;
                            scIUPR.PostData();
                        }
                        break;                                              
                    default:                    
                        using (SC_AddNewProviderRating scIUPR = new SC_AddNewProviderRating())
                        {
                            scIUPR.CCHID = cchid;
                            Array CCHIDsArray = cchids.Split('|');
                            DataTable CCHIDsTable = new DataTable("ArrayInt");
                            CCHIDsTable.Columns.Add("item", typeof(Int32));
                            if (CCHIDsArray.Length == 0)
                            {
                                foreach (string element in CCHIDsArray)
                                {
                                    CCHIDsTable.Rows.Add(Convert.ToInt32(element));
                                }
                            }
                            else {
                                CCHIDsTable.Rows.Add(Convert.ToInt32(cchid));                            
                            }                        
                            scIUPR.CCHIDs = CCHIDsTable;
                            scIUPR.ProviderID = providerId;
                            scIUPR.OrganizationID = organizationID;
                            scIUPR.Rating = score;
                            scIUPR.EmployerID = employerId;
                            scIUPR.Category = category;
                            scIUPR.Review = review;
                            scIUPR.PostData();
                        }              
                        break;
                };                
            }
            context.Response.StatusCode = 200;
        }       

        public bool IsReusable
        {
            get
            {
                return true;
            }
        }

    }
}
<%@ WebHandler Language="C#" Class="Enrollments" %>

using System;
using System.Data;
using System.Web;
using System.Web.Script.Serialization;
using System.Text;
using System.Collections.Generic;


class Person
{
    public string FirstName;
    public string LastName;
    public int Number;
    public bool FullTime;
    
}

public struct ResultData
{
    public int LoginCounts;
}

public class Enrollments : IHttpHandler {
    
    public void ProcessRequest (HttpContext context) {
        context.Response.ContentType = "application/json";
        context.Response.ContentEncoding = Encoding.UTF8;
        var people = new List<Person>();
                
        // TESTING OUT DATA FOR GOOGLE TABLE API
        /*
        using (GetLoginCounts glc = new GetLoginCounts())
        {
            glc.GetData();
            if (glc.Counts.TableName != "EmptyTable")
            {
                foreach (DataRow dr in glc.Counts.Rows)
                {
                    ResultData rd = CCHSerializer.DeserializeDataRow<ResultData>(dr);
                    people.Add(new Person() { FirstName = "Joe", Number = rd.LoginCounts, FullTime = true });
                }
            }
        }
        */
        
        people.Add(new Person() { FirstName = "Joe", Number = 666, FullTime = true });
        people.Add(new Person() { FirstName = "Jason", Number = 10000, FullTime = false });
        people.Add(new Person() { FirstName = "Johnson", Number = 50, FullTime = true });
        people.Add(new Person() { FirstName = "Steve", Number = 25, FullTime = false });
        
        JavaScriptSerializer jss = new JavaScriptSerializer();
        String jsonBack = jss.Serialize(people);

        context.Response.Write(jsonBack);
                                
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}
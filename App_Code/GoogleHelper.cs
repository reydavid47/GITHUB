using System;
using System.Text;
using System.Net;
using System.Xml;
using System.Xml.XPath;
using System.Collections.Generic;

namespace ClearCostWeb
{
    /// <summary>
    /// Summary description for GoogleHelper
    /// </summary>
    public class GoogleHelper
    {
        public GoogleHelper()
        { }
        public static string GetLatLng(string Address)
        {
            string results = "";

            StringBuilder queryStringBuilder = new StringBuilder();
            queryStringBuilder.Append("address=").Append(Uri.EscapeUriString(Address));
            queryStringBuilder.Append("&");
            queryStringBuilder.Append("sensor=").Append(Uri.EscapeUriString("false"));

            UriBuilder uriBuild = new UriBuilder("http://maps.googleapis.com");
            uriBuild.Path = "/maps/api/geocode/xml";
            uriBuild.Query = queryStringBuilder.ToString();

            HttpWebRequest request = WebRequest.Create(uriBuild.Uri) as HttpWebRequest;

            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                XmlDocument xml = new XmlDocument();
                xml.Load(response.GetResponseStream());

                XmlNodeList xnStatusList = xml.SelectNodes("/GeocodeResponse/status");
                XmlNode statusNode = xnStatusList.Item(0);
                if (statusNode.InnerText == "OK")
                {
                    XmlNodeList xnList = xml.SelectNodes("/GeocodeResponse/result/geometry");
                    foreach (XmlNode xn in xnList)
                    {
                        XmlNodeList xnChildren;
                        XmlNode xnChildLat, xnChildLng;

                        XmlNode xnStart = xn.SelectSingleNode("/GeocodeResponse/result/geometry/location");
                        xnChildren = xnStart.ChildNodes;
                        xnChildLat = xnChildren.Item(0);
                        xnChildLng = xnChildren.Item(1);

                        results = string.Format("{0},{1}", xnChildLat.InnerText, xnChildLng.InnerText);
                    }
                }
                return results;
            }
        }
        public static String[] GetDistances(String StartingAddress, String[] Destinations)
        {
            try
            {
                if (Destinations.Length > 0)
                {
                    List<string> results = new List<string>();

                    StringBuilder queryStringBuilder = new StringBuilder();
                    queryStringBuilder.Append("origins=").Append(Uri.EscapeUriString(StartingAddress));
                    queryStringBuilder.Append("&");
                    queryStringBuilder.Append("destinations=");
                    for (int i = 0; i < Destinations.Length - 1; i++)
                    {
                        queryStringBuilder.Append(Uri.EscapeUriString(Destinations[i])).Append("|");
                    }
                    queryStringBuilder.Append(Uri.EscapeUriString(Destinations[Destinations.Length - 1]));
                    queryStringBuilder.Append("&");
                    queryStringBuilder.Append("units=imperial");
                    queryStringBuilder.Append("&");
                    queryStringBuilder.Append("sensor=").Append(Uri.EscapeUriString("false"));

                    UriBuilder uriBuild = new UriBuilder("http://maps.googleapis.com");
                    uriBuild.Path = "/maps/api/distancematrix/xml";
                    uriBuild.Query = queryStringBuilder.ToString();

                    HttpWebRequest request = WebRequest.Create(uriBuild.Uri) as HttpWebRequest;

                    using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                    {
                        XmlDocument xml = new XmlDocument();
                        xml.Load(response.GetResponseStream());

                        XmlNodeList xnStatusList = xml.SelectNodes("/DistanceMatrixResponse/status");
                        XmlNode statusNode = xnStatusList.Item(0);
                        if (statusNode.InnerText == "OK")
                        {
                            XmlNodeList xnList = xml.SelectNodes("/DistanceMatrixResponse/row/element");
                            foreach (XmlNode xn in xnList)
                            {
                                XmlNodeList elementChildren = xn.ChildNodes;
                                if (elementChildren.Item(0).InnerText == "OK")
                                {
                                    XmlNodeList distanceChildren = elementChildren.Item(2).ChildNodes;
                                    XmlNode distance = distanceChildren.Item(1);
                                    if (distance.InnerText.ToLower().Contains("ft")) { distance.InnerText = "0.0 mi"; } //We began receiving feet, change to 0 miles for our concerns
                                    results.Add(distance.InnerText);
                                }
                                else
                                {
                                    results.Add(elementChildren.Item(0).InnerText.ToString());
                                }
                            }
                        }
                        return results.ToArray();
                    }
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return new String[] { "" };
            }
        }
    }
}
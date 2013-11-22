using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClearCostWeb
{
    /// <summary>
    /// Holds data for the registration of Client Side Java at runtime
    /// </summary>
    [Serializable]
    public class JavaRegistrar
    {
        public JavaRegistrar()
        { }

        public static class jQuery
        {
            #region The Main Dictionary for Java Resources
            private static Dictionary<string, string> Scripts = new Dictionary<string, string>
        {
            {"BaseName","jquery-1.5.1"},{"BaseURL","~/Scripts/jquery-1.5.1.js"},
            {"CoreName", "jquery.ui.core"},{"CoreURL","~/Scripts/jquery.ui.core.js"},
            {"PositionName","jquery.ui.position"},{"PositionURL","~/Scripts/jquery.ui.position.js"},
            {"WidgetName","jquery.ui.widget"},{"WidgetURL","~/Scripts/jquery.ui.widget.js"},
            {"AutoCompleteName", "jquery.ui.autocomplete"},{"AutoCompleteURL","~/Scripts/jquery.ui.widget.js"}
        };
            #endregion

            #region The Public properties for use with accessing the dictionary
            public static String BaseName { get { return Scripts["BaseName"]; } }
            public static String BaseURL { get { return Scripts["BaseURL"]; } }
            public static String CoreName { get { return Scripts["CoreName"]; } }
            public static String CoreURL { get { return Scripts["CoreURL"]; } }
            public static String PositionName { get { return Scripts["PositionName"]; } }
            public static String PositionURL { get { return Scripts["PositionURL"]; } }
            public static String WidgetName { get { return Scripts["WidgetName"]; } }
            public static String WidgetURL { get { return Scripts["WidgetURL"]; } }
            public static String AutoCompleteName { get { return Scripts["AutoCompleteName"]; } }
            public static String AutoCompleteURL { get { return Scripts["AutoCompleteURL"]; } }
            #endregion
        }

        public static class Google
        {
            #region The Main Dictionaries for Java Resources
            private static Dictionary<string, string> Includes = new Dictionary<string, string>
        {
            {"APIName","GoogleAPI"},{"APIUrl","http://maps.googleapis.com/maps/api/js?sensor=false"}
        };
            private static Dictionary<string, string> Scripts = new Dictionary<string, string>
        {
        {"GeoCodeName", "GetLatLong"},
        {"GeoCodeScript",
                "var geocoder = new google.maps.Geocoder(); var map; var address = '{Addr}'; geocoder.geocode( { 'address': address}, function(results, status) { if (status == google.maps.GeocoderStatus.OK) { __doPostBack('{ClientID}',results[0].geometry.location.lat() + '|' + results[0].geometry.location.lng()); } else { alert('Geocode was not successful for the following reason: ' + status); } });"}

        };
            #endregion

            #region The Public properties for use with accessing the dictionary
            public static String APIName { get { return Includes["APIName"]; } }
            public static String APIUrl { get { return Includes["APIUrl"]; } }
            public static String GeoCodeName { get { return Scripts["GeoCodeName"]; } }
            #endregion

            #region The Public Methods for use with accessing the dictionary
            public static String GetGeoCodeScript(String FullAddress, String ClientID) { String retStr = Scripts["GeoCodeScript"].Replace("{Addr}", FullAddress).Replace("{ClientID}", ClientID); return retStr; }
            #endregion
        }
    }
}
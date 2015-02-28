using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using Microsoft.SharePoint.Client;

namespace Verona.Lib.Common.Utility
{
    public static class CnvUtility
    {
        /// <summary>
        /// Returns the object value as integer. If it fails, it returns int.MinValue. Does not work in Sandbox Solutions in Office 365
        /// </summary>
        /// <param name="oObject">The o object.</param>
        /// <returns></returns>
        public static int ToInt(object oObject)
        {
            if (oObject == null)
                return int.MinValue;

            int number;
            return int.TryParse(oObject.ToString(), out number) ? number : int.MinValue;
        }

        /// <summary>
        /// Returns the object value as string. If it fails, it returns string.empty. 
        /// </summary>
        /// <param name="oObject">The o object.</param>
        /// <returns></returns>
        public static String ToStr(object oObject)
        {
            return oObject == null ? string.Empty : oObject.ToString();
        }

        /// <summary>
        /// Returns the object value as datetime. If it fails, it returns the epoch date 01.01.1970 00:00:01
        /// </summary>
        /// <param name="oObject">The o object.</param>
        /// <returns></returns>
        public static DateTime ToDatetime(object oObject)
        {
            return oObject is DateTime ? (DateTime)oObject : DateTime.Parse("01.01.1970 00:00:01");
        }

        /// <summary>
        /// Returns the object value as Guid. If it fails, it returns default Guid (zero-based Guid)
        /// </summary>
        /// <param name="oObject">The o object.</param>
        /// <returns></returns>
        public static Guid ToGuid(object oObject)
        {
            if (oObject == null) return default(Guid);

            var strGuid = oObject.ToString();
            if (!string.IsNullOrEmpty(strGuid) && GuidRegEx.IsMatch(strGuid))
                return new Guid(strGuid);

            return default(Guid);
        }

        private static readonly Regex GuidRegEx = new Regex("^[A-Fa-f0-9]{32}$|^({|\\()?[A-Fa-f0-9]{8}-([A-Fa-f0-9]{4}-){3}[A-Fa-f0-9]{12}(}|\\))?$|" +
                          "^({)?[0xA-Fa-f0-9]{3,10}(, {0,1}[0xA-Fa-f0-9]{3,6}){2}, {0,1}({)([0xA-Fa-f0-9]{3,4}, {0,1}){7}[0xA-Fa-f0-9]{3,4}(}})$", RegexOptions.Compiled);

        /// <summary>
        /// Returns the object value as an array. If it fails, it returns null.
        /// </summary>
        /// <param name="oObject">The o object.</param>
        /// <param name="delimiter"></param>
        /// <returns></returns>
        public static String[] ToArray(object oObject, char delimiter)
        {
            var objAsString = ToStr(oObject);
            if (string.IsNullOrEmpty(objAsString) || string.IsNullOrWhiteSpace(objAsString)) return null;
            return objAsString.Split(delimiter);
        }

        /// <summary>
        /// Returns the object value as bool. If it fails, it returns false
        /// </summary>
        /// <param name="oObject">The o object.</param>
        /// <returns></returns>
        public static bool ToBool(object oObject)
        {
            if (oObject == null)
                return false;

            if (!string.IsNullOrEmpty(ToStr(oObject)))
                return oObject.ToString().ToLower().Equals("true");

            return ToInt(oObject) > int.MinValue && ToInt(oObject).Equals(1);
        }

        /// <summary>
        /// Converts object value to absolute URI. Returns NULL if it fails.
        /// </summary>
        /// <param name="oObject">The o object.</param>
        /// <returns></returns>
        public static Uri ToAbsoluteUri(object oObject)
        {
            if (oObject == null)
                return null;

            try
            {
                return new Uri(ToStr(oObject), UriKind.Absolute);
            }
            catch
            {
                return null;
            }

        }

        /// <summary>
        /// Converts Degrees to Radians.
        /// </summary>
        /// <param name="deg">The deg.</param>
        /// <returns></returns>
        public static double Deg2Rad(double deg)
        {
            return (deg * Math.PI / 180.0);
        }

        /// <summary>
        /// Converts Radians to Degrees
        /// </summary>
        /// <param name="rad">The RAD.</param>
        /// <returns></returns>
        public static double Rad2Deg(double rad)
        {
            return (rad / Math.PI * 180.0);
        }

        /// <summary>
        /// Converts a querystring To the request query string.
        /// </summary>
        /// <param name="fullUrl">The query string.</param>
        /// <param name="delimiter">The delimiter.</param>
        /// <returns></returns>
        public static NameValueCollection ToRequestQueryString(string fullUrl, char delimiter)
        {
            if (string.IsNullOrEmpty(fullUrl) || fullUrl.IndexOf("?", StringComparison.Ordinal) <= 0)
                return null;

            delimiter = (string.IsNullOrEmpty(ToStr(delimiter))) ? '&' : delimiter;

            var nameValueCollection = new NameValueCollection();

            var rawQuery = fullUrl.Split('?')[1];

            if (rawQuery == null)
                return null;

            var queryElements = rawQuery.Split(delimiter);

            if (queryElements.Length <= 0)
                return null;

            foreach (var queryElement in queryElements)
            {
                var keyValuePair = queryElement.Split('=');
                if (keyValuePair.Length == 2)
                {
                    var key = keyValuePair[0];
                    var value = keyValuePair[1];

                    if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
                        continue;

                    nameValueCollection.Add(key, value);
                }
            }

            return nameValueCollection;

        }

        public static NameValueCollection ToRequestQueryString(string fullUrl)
        {
            return ToRequestQueryString(fullUrl, '&');
        }

        /// <summary>
        /// Converts a SharePoint client taxonomyfield listitem and returns hte taxonomy field value (the name)
        /// </summary>
        /// <param name="listItem">The list item.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns></returns>
        public static string ToTaxFieldValue(ListItem listItem, string fieldName)
        {
            if (listItem == null || string.IsNullOrEmpty(fieldName))
                return null;

            // taxFieldValue kan bli satt til null her pga en feil i o365 (feilen "kommer og går" og handler om tax-felt er inititialisert i tide)
            var taxFieldValue = listItem[fieldName] as Microsoft.SharePoint.Client.Taxonomy.TaxonomyFieldValue;

            // Håndterer feil i o365 her (hvis taxFieldValue == null henter vi verdien fra fieldvalues på en annen måte)
            return taxFieldValue == null ? ToStr(((Dictionary<string, object>)(listItem.FieldValues[fieldName]))["Label"]) : taxFieldValue.Label;
        }

        /// <summary>
        /// Converts the fieldvalue in listitem to a fieldUrlValue
        /// </summary>
        /// <param name="listItem">The list item.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns></returns>
        public static FieldUrlValue ToFieldUrlValue(ListItem listItem, string fieldName)
        {
            if (listItem == null || string.IsNullOrEmpty(fieldName))
                return null;

            return listItem[fieldName] as FieldUrlValue;
        }
    }
}

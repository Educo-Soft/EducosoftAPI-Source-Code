using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using Educo.ELS.SystemSettings;


namespace EducosoftAPI.Models
{
    /// <summary>
    /// This class defined to handle the timezone depending on the user timezone ID
    /// </summary>

    [Serializable]
    public class TimeZoneDateTime
    {
        private TimeZoneInformation[] m_zones;

        private class TimeZoneComparer : IComparer
        {
            #region Compare TimezoneID Objects
            /// <summary>
            /// Compare to objects
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <returns></returns>
            public int Compare(object x, object y)
            {
                TimeZoneInformation tzx, tzy;

                tzx = x as TimeZoneInformation;
                tzy = y as TimeZoneInformation;

                if (tzx == null || tzy == null)
                {
                    throw new ArgumentException("Parameter null or wrong type");
                }

                int biasDifference = tzx.Bias - tzy.Bias;

                if (biasDifference == 0)
                {
                    return tzx.DisplayName.CompareTo(tzy.DisplayName);
                }
                else
                {
                    return biasDifference;
                }
            }
            #endregion
        }

        #region GetUserTimeZoneDateTime
        /// <summary>
        /// Get the Datetime from the Input time using Timezone
        /// </summary>
        /// <param name="UserTimeZone"></param>
        /// <param name="LocalDateTime"></param>
        /// <returns></returns>
        public DateTime GetUserTimeZoneDateTime(string UserTimeZone, DateTime LocalDateTime)
        {

            DateTime UserZoneDtTm = DateTime.Now;
            DateTime UniversalDtTm;
            m_zones = TimeZoneInformation.EnumZones();
            Array.Sort(m_zones, new TimeZoneComparer());

            UniversalDtTm = LocalDateTime.ToUniversalTime();      // Convert time fomate to universal datetime formate
            for (int j = 0; j < m_zones.Length; j++)
            {
                if (UserTimeZone == m_zones[j].DisplayName)
                {
                    TimeZoneInformation TimeZoneInfo = m_zones[j] as TimeZoneInformation;
                    UserZoneDtTm = TimeZoneInfo.FromUniversalTime(UniversalDtTm);       //conver datetime from uinversal formate into user zone formate
                }
            }
            return UserZoneDtTm;
        }
        #endregion

        #region GetUserTimeZoneDateTime

        #endregion

        #region getUserTimeZoneDateTimeIndex
        /// <summary>
        /// Input Timezone,from timezone with specfied the input datatime
        /// </summary>
        /// <param name="UserTimeZone"></param>
        /// <param name="LocalDateTime"></param>
        /// <param name="inputTimeZone"></param>
        /// <returns></returns>
        public DateTime getUserTimeZoneDateTimeIndex(string UserTimeZone, DateTime LocalDateTime, string inputTimeZone)
        {
            DateTime UserZoneDtTm = DateTime.Now;
            DateTime UniversalDtTm;
            m_zones = TimeZoneInformation.EnumZones();
            Array.Sort(m_zones, new TimeZoneComparer());

            try
            {
                if (System.Web.HttpContext.Current.Request.Url.ToString().ToLower().IndexOf("studenttestpaperlist.aspx") <= -1)
                {
                    if (UserTimeZone == inputTimeZone)
                    {
                        inputTimeZone = TimeZoneInformation.CurrentTimeZone.Index.ToString();
                    }
                    else
                    {
                        LocalDateTime = GetUserTimeZoneDateTime(TimeZoneInformation.FromIndex(Convert.ToInt32(inputTimeZone)).DisplayName, LocalDateTime);

                    }
                }
                UniversalDtTm = TimeZoneInformation.ToUniversalTime(Convert.ToInt32(inputTimeZone), LocalDateTime);
            }
            catch
            {

                UniversalDtTm = LocalDateTime;      // Convert time fomate to universal datetime formate
            }

            for (int j = 0; j < m_zones.Length; j++)
            {
                if (UserTimeZone == m_zones[j].Index.ToString())
                {
                    TimeZoneInformation TimeZoneInfo = m_zones[j] as TimeZoneInformation;
                    UserZoneDtTm = TimeZoneInfo.FromUniversalTime(UniversalDtTm);       //conver datetime from uinversal formate into user zone formate
                }
            }
            return UserZoneDtTm;
        }
        #endregion
        public bool Checkpenaltyassdate(DateTime dttstenddate, DateTime dtatmptkndt, String SesTzname)
        {

            DateTime dttstdt;
            DateTime dtatmdt;

            dttstdt = GetUserTimeZoneDateTime(SesTzname, dttstenddate);
            dtatmdt = GetUserTimeZoneDateTime(SesTzname, dtatmptkndt);
            if (dttstdt <= dtatmdt)
                return true;
            else
                return false;
        }

        public bool Checkpenaltyassdate(DateTime dttstenddate, DateTime dtatmptkndt, String SesTzname, bool IsApplyTZ)
        {

            DateTime dttstdt;
            DateTime dtatmdt;

            if (IsApplyTZ)
                dttstdt = GetUserTimeZoneDateTime(SesTzname, dttstenddate);
            else
                dttstdt = dttstenddate;

            dtatmdt = GetUserTimeZoneDateTime(SesTzname, dtatmptkndt);
            if (dttstdt <= dtatmdt)
                return true;
            else
                return false;
        }
    }


}
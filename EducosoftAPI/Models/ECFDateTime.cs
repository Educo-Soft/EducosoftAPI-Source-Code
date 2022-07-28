using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EducosoftAPI.Models
{
    public class ECFDateTime
    {
        public ECFDateTime()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public string getECFDateFormat(string strDate, string CountryId)
        {
            try
            {
                string strOutputDate = string.Empty;
                strOutputDate = DisplayDate(strDate, CountryId);
                return strOutputDate;
            }
            catch
            {
                return strDate;
            }
        }

        public string getECFDateFormat(string strDate, bool blnTimeRequired, string CountryId)
        {
            try
            {
                string strOutputDate = string.Empty;
                strOutputDate = DisplayDate(strDate, CountryId);
                DateTime dt = new DateTime();
                System.IFormatProvider fp;
                if (CountryId == "1")
                    fp = new System.Globalization.CultureInfo("en-IN");
                else
                    fp = new System.Globalization.CultureInfo("en-US");

                dt = DateTime.Parse(strDate, fp);
                if (blnTimeRequired == true)
                {
                    strOutputDate += ' ' + String.Format("{0:t}", Convert.ToDateTime(strDate));
                }
                return strOutputDate;
            }
            catch
            {
                return strDate;
            }
        }

        public string DisplayDate(string strDate, string CountryId)
        {
            string strOutputDate = string.Empty;
            DateTime dt = new DateTime();
            System.IFormatProvider fp;
            Int32 intFormat = 1;
            if (CountryId == "1")
            {
                fp = new System.Globalization.CultureInfo("en-IN");
                intFormat = 2;
            }
            else
            {
                fp = new System.Globalization.CultureInfo("en-US");
                intFormat = 1;
            }
            dt = DateTime.Parse(strDate, fp);

            switch (intFormat)
            {
                case 1: // mm/dd/yyyy format
                    strOutputDate = dt.Month.ToString() + '/' + dt.Day.ToString() + '/' + dt.Year.ToString();
                    break;
                case 2:// dd/mm/yyyy format
                    strOutputDate = dt.Day.ToString() + '/' + dt.Month.ToString() + '/' + dt.Year.ToString();
                    break;
                case 3:// yyyy/mm/dd format
                    strOutputDate = dt.Year.ToString() + '/' + dt.Month.ToString() + '/' + dt.Day.ToString();
                    break;
                default:// mm/dd/yyyy format
                    strOutputDate = dt.Month.ToString() + '/' + dt.Day.ToString() + '/' + dt.Year.ToString();
                    break;
            }

            return strOutputDate;
        }
    }
}
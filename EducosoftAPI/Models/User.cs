using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Data;
using Educosoft.Api.User;

namespace EducosoftAPI.Models
{
    public class User : PageBase
    {
        SurveyAppUser objSurveyAppUser = null;
        StringBuilder spParam = null;
        DataSet dst = null;

        public User()
        {
            objSurveyAppUser = new SurveyAppUser();
        }

        public DataSet SaveSurveyUser (string et_name, string et_email, string et_mobile)
        {
            spParam = new StringBuilder();

            spParam.Append("1").Append(colSeperator).Append(et_name).Append(colSeperator);
            spParam.Append("2").Append(colSeperator).Append(et_email).Append(colSeperator);
            spParam.Append("3").Append(colSeperator).Append(et_mobile);

            dst = objSurveyAppUser.SaveSurveyUser(spParam.ToString());

            return dst;
        }

        public DataSet SaveSurveyFeedback(string et_email, string et_feedbackstring)
        {
            spParam = new StringBuilder();

            spParam.Append("1").Append(colSeperator).Append(et_email).Append(colSeperator);
            spParam.Append("2").Append(colSeperator).Append(et_feedbackstring);

            dst = objSurveyAppUser.SaveSurveyFeedback(spParam.ToString());

            return dst;
        }

        //UserInfo
        public DataSet GetUserInfo(string UserID)//"Strings-En"
        {
            spParam = new StringBuilder();
            spParam.Append("1").Append(colSeperator).Append(UserID);
            dst = objSurveyAppUser.USRGetUserInfo(spParam.ToString());
            return dst;
        }
    }
}
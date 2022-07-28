using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using Educosoft.Api.Course;
using System.Text;
using EducosoftAPI.Models;

namespace EducosoftAPI.Models
{
    public class UserCourse : PageBase
    {
        Api_Course objApi_UserCourse = null;
        StringBuilder spParam = null;
        DataSet dst = null;

        public UserCourse()
        {
            objApi_UserCourse = new Api_Course();
        }

        public DataSet GetUserCourseInfo(string userId,string UserType)
        {
            spParam = new StringBuilder();

            spParam.Append("1").Append(colSeperator).Append(userId).Append(colSeperator);
            spParam.Append("2").Append(colSeperator).Append(UserType);

            dst = objApi_UserCourse.GetUserCourseInfo(spParam.ToString());

            return dst;
        }

        public DataSet GetCourseLevelInfo(string CoursId)
        {
            spParam = new StringBuilder();

            spParam.Append("1").Append(colSeperator).Append(CoursId);
            dst = objApi_UserCourse.GetUserCourseFile(spParam.ToString());

            return dst;
        }

        public DataSet GetUserTimeSpentAllLevels(string UserId, string CourseId, string SectionId)
        {
            spParam = new StringBuilder();

            spParam.Append("1").Append(colSeperator).Append(UserId).Append(colSeperator);
            spParam.Append("2").Append(colSeperator).Append(CourseId).Append(colSeperator);
            spParam.Append("3").Append(colSeperator).Append(SectionId);
            dst = objApi_UserCourse.GetStudentTutorialTimeSpentAllLevels(spParam.ToString());

            return dst;
        }

        public DataSet GetBlockedLevels(string CourseID)
        {
            spParam = new StringBuilder();
            spParam.Append("1").Append(colSeperator).Append(CourseID);
            dst = objApi_UserCourse.GetBlockedLevels(spParam.ToString());
            return dst;
        }

        public DataSet GetScheduledTopics(string sectionID)
        {
            spParam = new StringBuilder();
            spParam.Append("1").Append(colSeperator).Append(sectionID);
            dst = objApi_UserCourse.GetScheduledTopics(spParam.ToString());
            return dst;
        }

        //public DataSet GetcourselevelFiles(string LevelID,string SectionID)
        //{
        //    spParam = new StringBuilder();
        //    spParam.Append("1").Append(colSeperator).Append(LevelID).Append(colSeperator);
        //    spParam.Append("2").Append(colSeperator).Append(SectionID);
        //    //spParam.Append("3").Append(colSeperator).Append(BBCourseID);
        //    dst = objApi_UserCourse.Getcourselevelfiles(spParam.ToString());
        //    return dst;
        //}
       
    }
}
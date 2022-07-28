using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Text;
using Educosoft.Api.Course;
using EducosoftAPI.Models;

namespace EducosoftAPI.Models.User
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

        public DataSet GetUserCourseInfo(string userId, string UserType)
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

        public DataSet GetcourselevelFiles(string LevelID, string SectionID)
        {
            spParam = new StringBuilder();
            spParam.Append("1").Append(colSeperator).Append(LevelID).Append(colSeperator);
            spParam.Append("2").Append(colSeperator).Append(SectionID);
            //spParam.Append("3").Append(colSeperator).Append(BBCourseID);
            dst = objApi_UserCourse.Getcourselevelfiles(spParam.ToString());
            return dst;
        }

        public string GetLevelHierarchy(string CourseID, string LevelID)
        {
            spParam = new StringBuilder();
            string strCourseTitle = "", levelHierarchy = "";
            spParam = new StringBuilder();
            spParam.Append("1").Append(colSeperator).Append(CourseID);
            dst = objApi_UserCourse.GetCourseDetails(spParam.ToString());

            if (dst.Tables[0].Rows[0][0].ToString() == "0")
            {
                if (dst.Tables[0].Rows[0][23].ToString() != null && dst.Tables[0].Rows[0][23].ToString() != "")
                {
                    strCourseTitle = dst.Tables[0].Rows[0][4].ToString();
                }
            }
            spParam.Length = 0;
            spParam.Append("1").Append(colSeperator).Append(LevelID);

            DataSet dstCourse = objApi_UserCourse.GetLevelHierarchy(spParam.ToString());
            if (dstCourse.Tables[0].Rows[0][0].ToString() == "0")
            {
                foreach (DataRow dtRow in dstCourse.Tables[0].Rows)
                {
                    if (strCourseTitle != "")
                    {
                        strCourseTitle = strCourseTitle.ToString();
                    }
                    else
                    {
                        strCourseTitle = dtRow[4].ToString();
                    }
                    if (levelHierarchy == "")
                    {
                        if (strCourseTitle != "")
                        {
                            levelHierarchy = strCourseTitle.ToString();
                        }
                        else
                        {
                            levelHierarchy = dtRow[4].ToString();
                            strCourseTitle = dtRow[4].ToString();
                        }
                    }
                    else
                    {
                        if (dtRow[6].ToString() == "0")
                        {
                            levelHierarchy += " > ";
                        }
                        levelHierarchy += dtRow[4].ToString();
                    }
                }
            }
            return levelHierarchy;
        }

        public string GetFilePath(string LoID)
        {
            char colSeperator = (char)195;
            Api_Course objCourse = new Api_Course();
            string paramList = "", filePathName = "", str = "", splitVal = Convert.ToString((char)209); ;
            int moreVal;
            string[] splitPath = null;

            paramList = "2" + colSeperator + LoID;
            var dstAsset = objCourse.GetCourseComponents(paramList);
            foreach (DataRow dtRow1 in dstAsset.Tables[0].Rows)
            {
                if (dtRow1[10].ToString() == "0")
                {
                    str = dtRow1[6].ToString();
                    moreVal = str.IndexOf(splitVal);
                    if (moreVal != -1)
                    {
                        splitPath = getFileName(dtRow1[6].ToString());
                        filePathName = splitPath[0].ToString() + @"/" + splitPath[1].ToString();
                    }
                }
            }
            return filePathName;
        }

        private string[] getFileName(string path)
        {
            string paramList = "";
            string[] splitPath = null;
            string delim = Convert.ToString((char)209);
            Api_Course objCourse;
            char chrFolderDelim = (char)201;
            char colSeperator = (char)195;

            DataSet dstCs;
            try
            {
                objCourse = new Api_Course();
                dstCs = new DataSet();
                splitPath = path.Split(delim.ToCharArray());

                if (splitPath.Length > 1)
                {
                    if (splitPath[0].ToString().Contains(chrFolderDelim.ToString())) //Imported Level
                    {
                        splitPath[0] = splitPath[0].ToString().Split(chrFolderDelim)[0];
                    }
                    else
                    {
                        paramList = "1" + colSeperator + splitPath[0].ToString();
                        dstCs = objCourse.GetCourseDetails(paramList);
                        if (dstCs.Tables[0].Rows[0][0].ToString() == "0")
                        {
                            splitPath[0] = dstCs.Tables[0].Rows[0][21].ToString();
                        }
                    }
                }
            }
            catch (Exception error)
            {

            }
            finally
            {
                dstCs = null;
            }

            return splitPath;
        }

    }
}

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using WasherDAL.Models;
using System.Linq;

namespace WasherDAL
{
    public class WasherRepository
    {
        washerContext context;
        SqlConnection conObj = new SqlConnection();
        SqlCommand cmdObj;
        public WasherRepository()
        {
            context = new washerContext();
            conObj.ConnectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=washer; Integrated Security=SSPI";
            cmdObj = new SqlCommand();
        }

        public string UserSignUp(string userName, string userEmail, string userPassword,string userMobile, string lat, string lon,bool washing)
        {
            
            string result = "-1";
            try
            {
                SqlParameter prmUserName = new SqlParameter("@UserName", userName);
                SqlParameter prmUserEmail = new SqlParameter("@UserEmail", userEmail);
                SqlParameter prmUserPass = new SqlParameter("@UserPassword", userPassword);
                SqlParameter prmUserMob = new SqlParameter("@UserMobile", userMobile);
                SqlParameter prmLat = new SqlParameter("@lat", lat);
                SqlParameter prmLon = new SqlParameter("@lon", lon);
                SqlParameter prmWash = new SqlParameter("@Washing", washing);

                SqlParameter prmUserId = new SqlParameter("@UserId", System.Data.SqlDbType.VarChar,20);
                prmUserId.Direction = System.Data.ParameterDirection.Output;

                context.Database.ExecuteSqlCommand("EXEC dbo.usp_SignUp @UserName,@UserEmail,@UserMobile,@lat,@lon,@UserPassword,@Washing, @UserId OUT", new[] { prmUserName, prmUserEmail,prmUserMob,prmLat,prmLon,prmUserPass,prmWash, prmUserId });

                result = Convert.ToString(prmUserId.Value);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                result = "-99";
            }
            return result;
        }

        public string UserLogin(string userEmail, string userPassword)
        {

            string returnValue;
            
            cmdObj = new SqlCommand(@"SELECT [dbo].ufn_Login(@UserEmail,@Password)", conObj);
            cmdObj.Parameters.AddWithValue("@UserEmail", userEmail);
            cmdObj.Parameters.AddWithValue("@Password", userPassword);
            try
            {
                conObj.Open();
                returnValue = Convert.ToString(cmdObj.ExecuteScalar());
            }
            catch (SqlException ex)
            {
                returnValue = "-1";
            }
            finally
            {
                conObj.Close();
            }
            return returnValue;
        }

        public Users GetUserInfo(string userId)
        {
            userId = userId.Replace(" ", string.Empty);
            try
            {
                var user = context.Users.Where(u => u.Userid == userId).FirstOrDefault();
                return user;
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        public string UpdateUserInfo(Users user)
        {

        }


    }
}

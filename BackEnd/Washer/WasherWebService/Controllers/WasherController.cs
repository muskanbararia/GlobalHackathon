using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WasherDAL;
using WasherDAL.Models;

namespace WasherWebService.Controllers
{
    [Route("api/[controller]/[Action]")]
    public class WasherController : Controller
    {
        WasherRepository rep;
        public WasherController()
        {
            rep = new WasherRepository();
        }

        [HttpPost]

        public JsonResult SignUp([FromBody] Models.User user)
        {
            string status = "-99";
            try
            {

                status = rep.UserSignUp(user.Username, user.Useremail, user.Userpassword, user.Usermobile, user.Latitude, user.Longitude,user.Washing);
            }
            catch (Exception ex)
            {
                status = "-99";
            }
            return Json(status);
        }


        [HttpGet]
        public JsonResult Login(string email, string password)
        {
            string status = "-99";
            try
            {

                status = rep.UserLogin(email,password);
            }
            catch (Exception ex)
            {
                status = "-99";
            }
            return Json(status);
        }

        [HttpGet]
        public JsonResult UserInfo(string userId)
        {
            Users u = new Users();
            try
            {

                u = rep.GetUserInfo(userId);
            }
            catch (Exception ex)
            {
                u = null;
            }
            return Json(u);
        }

    }
}
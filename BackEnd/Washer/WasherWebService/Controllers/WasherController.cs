using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WasherDAL;
using WasherDAL.Models;

namespace WasherWebService.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class WasherController : Controller
    {
        WasherRepository rep;
        private readonly IMapper _mapper;
        public WasherController(IMapper mapper)
        {
            rep = new WasherRepository();
            _mapper = mapper;
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

        //Send request
        [HttpPut]
        public JsonResult SendRequest(Models.MatchedRequest matchedRequest)
        {
            bool status = false;
            try
            {
                status = rep.SendRequest(matchedRequest.OwnerId, matchedRequest.WasherId,
                    matchedRequest.OwnerRequestId, matchedRequest.WasherRequestId);
            }
            catch (Exception ex)
            {
                status = false;
            }
            return Json(status);
        }

        //View pending requests
        [HttpGet]
        public JsonResult ViewPendingRequests(string userId)
        {
            List<Models.MatchedRequest> matchedRequests = new List<Models.MatchedRequest>();
            try
            {
                var requests = rep.ViewPendingRequests(userId);
                if(requests.Any())
                {
                    foreach (var r in requests)
                    {
                        matchedRequests.Add(_mapper.Map<Models.MatchedRequest>(r));
                    }
                }
            }
            catch (Exception ex)
            {
                matchedRequests = null;
            }
            return Json(matchedRequests);
        }

        //Update wash status
        [HttpPost]
        public JsonResult UpdateWashStatus(int acceptRequestId)
        {
            bool status = false;
            try
            {
                status = rep.UpdateWashStatus(acceptRequestId);
            }
            catch (Exception ex)
            {
                status = false;
            }
            return Json(status);
        }

        //Buy coins
        [HttpPost]
        public JsonResult BuyCoins(string userId,long accountNumber, int numberOfCoins)
        {
            bool status = false;
            try
            {
                status = rep.BuyCoins(userId, accountNumber, numberOfCoins);
            }
            catch (Exception ex)
            {
                status = false;
            }
            return Json(status);
        }

        //Fetch laundrocash for user
        [HttpGet]
        public JsonResult FetchLaundrocashForUser(string userId)
        {
            int result = 0;
            try
            {
                result = rep.FetchLaundrocashForUser(userId);
            }
            catch (Exception ex)
            {
                result = -99;
            }
            return Json(result);
        }

        [HttpPost]
        //Raising a new request
        public JsonResult RaiseRequest(LaundryRequest laundryRequest)
        {
            int status = 0;
            try
            {
                status = rep.RaiseRequest(_mapper.Map<LaundryRequest>(laundryRequest));
            }
            catch (Exception ex)
            {
                status = 0;
            }
            return Json(status);
        }

        [HttpGet]
        //Lists all the matched request for given user id
        public JsonResult ViewMatchedRequests(string userId)
        {
            List<Models.MatchedRequest> matchedRequests = new List<Models.MatchedRequest>();
            try
            {
                List<MatchedRequest> matched = rep.ViewMatchedRequests(userId);
                if (matched != null)
                {
                    foreach (var match in matched)
                    {
                        Models.MatchedRequest matchObj = _mapper.Map<Models.MatchedRequest>(match);
                        matchedRequests.Add(matchObj);
                    }
                }
            }
            catch (Exception e)
            {
                matchedRequests = null;
            }
            return Json(matchedRequests);
        }

        [HttpPut]
        //Accepting request
        public JsonResult AcceptOrRejectRequest(int matchedRequestId, string newStatus)
        {
            bool status = false;
            try
            {
                status = rep.AcceptOrRejectRequest(matchedRequestId, newStatus);
            }
            catch (Exception e)
            {
                status = false;
            }
            return Json(status);
        }
    }
}
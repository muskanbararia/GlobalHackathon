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
        private readonly WasherDBContext _context;
        SqlConnection conObj = new SqlConnection();
        SqlCommand cmdObj;
        public WasherRepository()
        {
            context = new washerContext();
            conObj.ConnectionString = "Data Source=.\\SQLExpress;Initial Catalog=WasherDB; Integrated Security=SSPI";
            cmdObj = new SqlCommand();
            _context = new WasherDBContext();
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
            return null;
        }

        //Raising a new request
        public int RaiseRequest(LaundryRequest laundryRequest)
        {
            int status = 0;
            try
            {
                _context.LaundryRequest.Add(laundryRequest);
                _context.SaveChanges();
                MatchRequests(laundryRequest);
                status = 1;
            }
            catch (Exception ex)
            {
                status = 0;
            }
            return status;
        }

        //Match requests
        public void MatchRequests(LaundryRequest laundryRequest)
        {        
            List<LaundryRequest> laundryRequests = new List<LaundryRequest>();
            MatchedRequest matchedRequest;
            try
            {
                laundryRequests = (from lr in _context.LaundryRequest
                                   where lr.Status.ToLower() == "active"
                                   && lr.WashingMachine == false
                                   select lr).ToList();
                if(laundryRequests.Any())
                {
                    foreach (var request in laundryRequests)
                    {
                        if(laundryRequest.WhitesOnly == request.WhitesOnly && 
                            laundryRequest.UnderGarmentsOnly == request.UnderGarmentsOnly &&
                            laundryRequest.GarmentsOnly == request.GarmentsOnly &&
                            laundryRequest.DenimsOrTrousersOnly == request.DenimsOrTrousersOnly)
                        {
                            matchedRequest = new MatchedRequest();
                            matchedRequest.OwnerId = laundryRequest.UserId;
                            matchedRequest.WasherId = request.UserId;
                            matchedRequest.OwnerRequestId = laundryRequest.RequestId;
                            matchedRequest.WasherRequestId = request.RequestId;
                            matchedRequest.Status = "Inactive";
                            _context.MatchedRequest.Add(matchedRequest);
                            _context.SaveChanges();                            
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                
            }            
        }

        //Lists all the matched request for given user id
        public List<MatchedRequest> ViewMatchedRequests(string userId)
        {
            List<MatchedRequest> matchedRequests = new List<MatchedRequest>();
            try
            {
                matchedRequests = _context.MatchedRequest.FromSql("Select * from ufn_ViewMatchedRequests(@UserId)").ToList();
                SqlParameter user = new SqlParameter("@UserId", userId);
            }
            catch (Exception e)
            {
                matchedRequests = null;
            }
            return matchedRequests;
        }

        //Send request
        public bool SendRequest(string senderUserId, string receiverUserId, 
            int senderRequestId, int receiverRequestId)
        {
            bool status = false;
            try
            {
                MatchedRequest matchedRequest = new MatchedRequest();
                matchedRequest = (from mr in _context.MatchedRequest
                                  where (mr.OwnerId == senderUserId &&
                                  mr.WasherId == receiverUserId &&
                                  mr.OwnerRequestId == senderRequestId &&
                                  mr.WasherRequestId == receiverRequestId) ||
                                  (mr.WasherId == senderUserId &&
                                      mr.OwnerId == receiverUserId &&
                                      mr.WasherRequestId == senderRequestId &&
                                      mr.OwnerRequestId == receiverRequestId)
                                  select mr).FirstOrDefault();
                if(matchedRequest != null)
                {
                    matchedRequest.Status = "Pending";
                    _context.SaveChanges();
                    status = true;
                }
            }
            catch (Exception ex)
            {
                status = false;
            }
            return status;
        }

        

        //View pending requests
        public List<MatchedRequest> ViewPendingRequests(string userId)
        {
            List<MatchedRequest> matchedRequests = new List<MatchedRequest>();
            try
            {
                matchedRequests = (from mr in _context.MatchedRequest
                                   where (mr.OwnerId == userId
                                   || mr.WasherId == userId) && 
                                   mr.Status.ToLower()=="pending"
                                   select mr).ToList();
            }
            catch (Exception ex)
            {
                matchedRequests = null;
            }
            return matchedRequests;
        }

        //Accepting request
        public bool AcceptOrRejectRequest(int matchedRequestId, string newStatus)
        {
            bool status = false;
            MatchedRequest matchedRequests = new MatchedRequest();
            try
            {
                matchedRequests = (from mr in _context.MatchedRequest
                                   where mr.MatchedRequestId == matchedRequestId
                                   select mr).FirstOrDefault();
                if (matchedRequests != null)
                {
                    matchedRequests.Status = newStatus;
                    //Add to Accepted Request

                    _context.SaveChanges();
                    status = true;
                }
            }
            catch (Exception e)
            {
                status = false;
            }
            return status;
        }

        //Start wash cycle -> will call UpdateWashStatus automatically after 2 hours

        //Update wash status
        public bool UpdateWashStatus(int acceptedRequestId)
        {
            bool status = false;
            try
            {
                AcceptedRequest acceptedRequest = new AcceptedRequest();
                acceptedRequest = (from ar in _context.AcceptedRequest
                                   where ar.AcceptedRequestId == acceptedRequestId
                                   select ar).FirstOrDefault();
                if(acceptedRequest != null)
                {
                    //Update status as completed
                    acceptedRequest.Status = "Complete";

                    //Debit transaction
                    Transaction debitTransaction = new Transaction();
                    debitTransaction.UserId = acceptedRequest.WasherId;
                    int? washerReqId = acceptedRequest.WasherRequestId;
                    LaundryRequest washerRequest = (from lr in _context.LaundryRequest
                                     where lr.RequestId == washerReqId
                                     select lr).FirstOrDefault();
                    debitTransaction.Laundrocash = washerRequest.Weight;
                    debitTransaction.TransactionType = "D";
                    debitTransaction.Message = "Debited successfully";
                    debitTransaction.TransactionDateTime = DateTime.Now;

                    //Credit transaction
                    Transaction creditTransaction = new Transaction();
                    creditTransaction.UserId = acceptedRequest.OwnerId;
                    int? ownerReqId = acceptedRequest.OwnerRequestId;
                    LaundryRequest ownerRequest = (from lr in _context.LaundryRequest
                                                    where lr.RequestId == ownerReqId
                                                   select lr).FirstOrDefault();
                    creditTransaction.Laundrocash = ownerRequest.Weight;
                    creditTransaction.TransactionType = "C";
                    creditTransaction.Message = "Credited successfully";
                    creditTransaction.TransactionDateTime = DateTime.Now;

                    //Add transactions
                    _context.Transaction.Add(debitTransaction);
                    _context.Transaction.Add(creditTransaction);

                    //Update request status as Inactive
                    washerRequest.Status = "Inactive";
                    ownerRequest.Status = "Inactive";

                    //Save changes
                    _context.SaveChanges();
                    status = true;
                }
            }
            catch (Exception ex)
            {
                status = false;
            }
            return status;
        }

        //Buy coins
        public bool BuyCoins(string userId, long accountNumber, int numberOfCoins)
        {
            bool status = false;
            try
            {
                //logic to validate account number
                Transaction transaction = new Transaction();
                transaction.UserId = userId;
                transaction.Laundrocash = numberOfCoins;
                transaction.TransactionType = "C";
                transaction.TransactionDateTime = DateTime.Now;
                transaction.Message = "Laundrocash added";
                _context.Transaction.Add(transaction);
                _context.SaveChanges();
                status = true;
            }
            catch (Exception ex)
            {
                status = false;
            }
            return status;
        }

        //Fetch laundrocash for a user
        public int FetchLaundrocashForUser(string userId)
        {
            int laundrocash = 0;
            try
            {
                List<int> totalDebit = (from t in _context.Transaction
                                        where t.TransactionType == "D"
                                        && t.UserId == userId
                                        select t.Laundrocash).ToList();
                List<int> totalCredit = (from t in _context.Transaction
                                        where t.TransactionType == "C"
                                        && t.UserId == userId
                                        select t.Laundrocash).ToList();
                laundrocash = totalCredit.Sum() - totalDebit.Sum();

            }
            catch (Exception ex)
            {
                laundrocash = -99;
            }
            return laundrocash;
        }
    }
}

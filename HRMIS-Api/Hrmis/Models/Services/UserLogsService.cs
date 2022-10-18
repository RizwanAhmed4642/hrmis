using Hrmis.Models.Common;
using Hrmis.Models.DbModel;
using Hrmis.Models.ViewModels;
using Hrmis.Models.ViewModels.Application;
using Hrmis.Models.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Hrmis.Models.Services
{

    public class UserLogsSErvice
    {
        public void SaveProfileLoggedInInfo(string userId, string remarks, LogClass obj)
        {

            try
            {
                using (var db = new HR_System())
                {
                    string ip = HttpContext.Current.Request.UserHostAddress;
                    string httpXForwarded = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                    string remote = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    string ip5 = HttpContext.Current.Request.Params["HTTP_CLIENT_IP"];
                    db.LoggedProfileInLogs.Add(new LoggedProfileInLog()
                    {
                        IPAddress = ip,
                        LoggedInDate = DateTime.Now,
                        UserId = userId,
                        ForwardedIPAddress = getIPAddress(HttpContext.Current.Request),
                        Browser = HttpContext.Current.Request.UserAgent,
                        //Remarks = remarks,
                        HttpXForwardedFor = httpXForwarded,
                        RemoteAddress = remote,
                        IsList = obj.IsList,
                        IsSearched = obj.IsSearched,
                        SearchedCNIC = obj.searchedCNIC

                    }); 
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
            }
        }
        public void SaveLoggedInInfo(string userId, string remarks, string remoteIp = null, string remoteDetail = null)
        {

            try
            {
                using (var db = new HR_System())
                {
                    string ip = HttpContext.Current.Request.UserHostAddress;
                    string httpXForwarded = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                    string remote = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    string ip5 = HttpContext.Current.Request.Params["HTTP_CLIENT_IP"];
                    db.LoggedInLogs.Add(new LoggedInLog()
                    {
                        IPAddress = ip,
                        LoggedInDate = DateTime.Now,
                        UserId = userId,
                        ForwardedIPAddress = getIPAddress(HttpContext.Current.Request),
                        Browser = HttpContext.Current.Request.UserAgent,
                        //Remarks = remarks,
                        HttpXForwardedFor = httpXForwarded,
                        RemoteAddress = remote,
                        RemoteIP = remoteIp,
                        RemoteDetailJSON = remoteDetail,
                        
                    });
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {

            }
        }


        public static string getIPAddress(HttpRequest request)
        {
            string szIP = null;
            string szRemoteAddr = request.UserHostAddress;
            try
            {
                string szXForwardedFor = request.ServerVariables["X_FORWARDED_FOR"];

                if (szXForwardedFor == null)
                {
                    szIP = szRemoteAddr;
                }
                else
                {
                    szIP = szXForwardedFor;
                    if (szIP.IndexOf(",") > 0)
                    {
                        string[] arIPs = szIP.Split(',');

                        //foreach (string item in arIPs)
                        //{
                        //    if (!IsPrivateIpAddress(item))
                        //    {
                        //        return item;
                        //    }
                        //}
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return szIP;
        }

    }

    
  
}
   
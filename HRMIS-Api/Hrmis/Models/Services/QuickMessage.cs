using Hrmis.Models.Common;
using Hrmis.Models.DbModel;
using System;
using System.Data.Entity;
using System.Linq;
using System.Xml;
using System.Net;


namespace Hrmis.Models.Services
{
    public class QuickMessage
    {
        private string MSISDN;
        private string PASSWORD;
        private string abc;

        public QuickMessage(string msisdn, string password)
        {
            MSISDN = msisdn;
            this.PASSWORD = password;
        }
        public SMS_Session getSessionId(string userId)
        {
            try
            {
                using (var db = new HR_System())
                {
                    //var session = db.SMS_Session.OrderByDescending(x => x.RequestDateTime).FirstOrDefault();
                    //if (session != null)
                    //{
                    //    double minutes = (DateTime.UtcNow.AddHours(5) - session.LastActivityTime).Value.TotalMinutes;
                    //    if (minutes < 30)
                    //    {
                    //        session.LastActivityTime = DateTime.UtcNow.AddHours(5);
                    //        db.Entry(session).State = EntityState.Modified;
                    //        db.SaveChanges();
                    //        return session;
                    //    }
                    //}
                    string url = "https://telenorcsms.com.pk:27677/corporate_sms2/api/auth.jsp?msisdn=" + MSISDN + "&password=" + PASSWORD;
                    //string sessionId = sendRequest(url);
                    SMS_Session sMS_Session = new SMS_Session();
                    //sMS_Session.SessionId = sessionId;
                    sMS_Session.RequestDateTime = DateTime.UtcNow.AddHours(5);
                    sMS_Session.LastActivityTime = sMS_Session.RequestDateTime;
                    sMS_Session.RequestedBy = string.IsNullOrEmpty(userId) ? "System" : userId;
                    //db.SMS_Session.Add(sMS_Session);
                    //db.SaveChanges();
                    return sMS_Session;
                }
            }
            catch (Exception ex)
            {

                throw;
            }

        }
        public SMS_Log sendQuickMessage(SMS_Session session, SMS sms)
        {

            try
            {
                using (var db = new HR_System())
                {
                    //string url = "https://telenorcsms.com.pk:27677/corporate_sms2/api/sendsms.jsp?session_id=" + session.SessionId + "&text=" + sms.Message + "&to=" + sms.MobileNumber;
                    //if (sms.Mask != null)
                    //{
                    //    url = url += "&mask=" + sms.Mask;
                    //}

                    //string url = @"https://bsms.ufone.com/bsms_v8_api/sendapi-0.3.jsp?id=03315519747&message=" + sms.Message + "&shortcode=HISDU&lang=English&mobilenum=" + sms.MobileNumber + "&password=Hi$DU@112345&messagetype=Nontransactional";
                    ////string url = @"https://bsms.ufone.com/bsms_v8_api/sendapi0.3.jsp?id=03315519747&message=" + sms.Message + "&shortcode=HISDU&lang=English&mobilenum=" + sms.MobileNumber + "&password=admin@Block123&&messagetype=Nontransactional";
                    ///for Jazz
                    ///
                    string url = @"https://connect.jazzcmt.com/sendsms_url.html?Username=03018482714&Password=Jazz@123&From=" + sms.Mask + "&To=" + sms.MobileNumber + "&Message=" + sms.Message + "&Identifier=123456&UniqueId=123456789&ProductId=123456789&Channel=123456789&TransactionId=123456789";

                    SMS_Log sMS_Log = new SMS_Log();
                    sMS_Log.SMS_Session_Id = session.Id;
                    sMS_Log.Message = sms.Message;
                    sMS_Log.Number = sms.MobileNumber;
                    sMS_Log.Mask = sms.Mask;
                    sMS_Log.UserId = sms.UserId;
                    sMS_Log.DateTime = DateTime.UtcNow.AddHours(5);
                    sMS_Log.FKId = sms.FKId;
                    sMS_Log.MessageId = sendRequest(url);
                    db.SMS_Log.Add(sMS_Log);
                    db.SaveChanges();
                    return sMS_Log;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public string sendRequest(string url)
        {
            string response = null;
            try
            {
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                var client = new WebClient();
                Uri smsUri = new Uri(url);
                response = client.DownloadString(smsUri);
                XmlDocument xmldoc = new XmlDocument();
                //xmldoc.LoadXml(response);
                //XmlNodeList responseType = xmldoc.GetElementsByTagName("response_to_browser");
                //XmlNodeList data = xmldoc.GetElementsByTagName("response_id");
                //XmlNodeList text = xmldoc.GetElementsByTagName("response_text");
                //if (responseType.Equals("Error"))
                //{
                //    return null;
                //}
                //string responseId = data[0].InnerText;
                string responseText = response;
                return responseText.Replace("Successful:", "");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return null;
        }

        public SMS_Log checkStatus(SMS_Session session, int smsLogId)
        {
            try
            {
                using (var db = new HR_System())
                {
                    SMS_Log sMS_Log = db.SMS_Log.FirstOrDefault(x => x.Id == smsLogId);
                    if (sMS_Log != null)
                    {
                        string url = "https://telenorcsms.com.pk:27677/corporate_sms2/api/querymsg.jsp?session_id=" + session.SessionId + "&msg_id=" + sMS_Log.MessageId;
                        sMS_Log.Status = sendRequest(url);
                        db.Entry(sMS_Log).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    return sMS_Log;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public string ping(string sessionId)
        {
            string url =
           "https://telenorcsms.com.pk:27677/corporate_sms2/api/ping.jsp?session_id=" + sessionId;
            return sendRequest(url);
        }
        public string status(string messageId, string sessionId)
        {
            string url =
           "https://telenorcsms.com.pk:27677/corporate_sms2/api/ping.jsp?session_id=" + sessionId;
            return sendRequest(url);
        }
     
    }
}
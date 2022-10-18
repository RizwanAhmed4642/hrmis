//using DPUruNet;
using DPUruNet;
using Hrmis.Models.DbModel;
using Hrmis.Models.Services;
using log4net;
using Microsoft.Exchange.WebServices.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Hrmis.Models.Common
{
    public static class Common
    {
        private static readonly HttpClient client = new HttpClient();

        

        public static List<string> southTehsils = new List<string>()
        {
            "031001001",
            "031001002",
            "031001003",
            "031001004",
            "031001005",
            "031002001",
            "031002002",
            "031002003",
            "031002004",
            "031002005",
            "031002007",
            "031003001",
            "031003002",
            "031003003",
            "031003004",
            "032001001",
            "032001002",
            "032001003",
            "032001004",
            "032002001",
            "032002002",
            "032002003",
            "032003001",
            "032003002",
            "032003003",
            "032003004",
            "032004001",
            "032004002",
            "032004003",
            "036001001",
            "036001004",
            "036001006",
            "036001007",
            "036003001",
            "036003002",
            "036003003",
            "036004001",
            "036004002",
            "036004003",
            "036004005",
            "036006001",
            "036006002",
            "036006003"
        };

        public static List<string> southDistricts = new List<string>()
        {
            "031001",
            "031002",
            "031003",
            "032001",
            "032002",
            "032003",
            "032004",
            "036001",
            "036003",
            "036004",
            "036006"
        };

        public static List<string> southDistrictNames = new List<string>()
        {
            "Bahawalnagar",
            "Bahawalpur",
            "Rahim Yar Khan",
            "Dera Ghazi Khan",
            "Layyah",
            "Muzaffargarh",
            "Rajanpur",
            "Multan",
            "Vehari",
            "Khanewal",
            "Lodhran"
        };

        public static List<string> HFTypeCodes = new List<string>()
        {
            "011",
            "012",
            "013",
            "014",
            "015"
        };


        public static List<string> phfmcDistrictCodes = new List<string>()
        {
            "031003",
            "032001",
            "032004",
            "033001",
            "033003",
            "034005",
            "035001",
            "035002",
            "036003",
            "036006",
            "037004",
            "038003",
            "039007",
            "039008"
        };


        public static List<string> phfmcDistrictNames = new List<string>()
        {
            "Chakwal		 ",
            "Dera Ghazi Khan ",
            "Rajanpur		 ",
            "Faisalabad		 ",
            "T.T Singh		 ",
            "Hafizabad		 ",
            "Lahore			 ",
            "Kasur			 ",
            "Lodhran		 ",
            "Vehari			 ",
            "Pakpattan		 ",
            "Sahiwal		 ",
            "Rahim Yar Khan	 ",
            "Mianwali        "


        };



        public static IList<T> Clone<T>(this IList<T> listToClone) where T : ICloneable
        {
            return listToClone.Select(item => (T)item.Clone()).ToList();
        }

        public static Boolean SMS_Send(List<SMS> SmsList)
        {
            try
            {
                HttpClient client = new HttpClient();
                foreach (SMS sms in SmsList)
                {
                    if (sms.MobileNumber.StartsWith("030") || sms.MobileNumber.StartsWith("032"))
                    {
                        client.PostAsync("http://119.160.92.2:7700/sendsms_url.html?Username=03028504711&Password=123.123&From=HISDU&To=" + sms.MobileNumber.Replace("-", "") + "&Message=" + sms.Message, null);
                    }
                    else
                    {
                        client.PostAsync("http://119.160.92.2:7700/sendsms_url.html?Username=03028504710&Password=123.123&From=HISDU&To=" + sms.MobileNumber.Replace("-", "") + "&Message=" + sms.Message, null);
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }


        public static async Task<HttpResponseMessage> SMS_Send(SMS meessage)
        {
            try
            {
                HttpClient client = new HttpClient();

                if (meessage.MobileNumber.StartsWith("030") || meessage.MobileNumber.StartsWith("032"))
                {
                    meessage.From = "03028504711";
                    return await client.PostAsync("http://119.160.92.2:7700/sendsms_url.html?Username=03028504711&Password=123.123&ur=1&From=HISDU&To=" + meessage.MobileNumber.Replace("-", "") + "&Message=" + meessage.Message, null);
                }
                else
                {
                    meessage.From = "03028504710";
                    return await client.PostAsync("http://119.160.92.2:7700/sendsms_url.html?Username=03028504710&Password=123.123&ur=1&From=HISDU&To=" + meessage.MobileNumber.Replace("-", "") + "&Message=" + meessage.Message, null);
                }
            }
            catch
            {
                return null;
            }
        }
        public async static Task<string> SendSMSUfone(SMS sms)
        {
            sms.UserId = "92fa5fb6-f532-410b-9ccc-d529277b3c30";
            var res = await HttpPost("https://notifications.pshealthpunjab.gov.pk/api/Sms/Send", sms);
            return res;
        }

        private async static Task<string> HttpPost(string urlToPost, SMS sms)
        {
            //Converting the object to a json string. NOTE: Make sure the object doesn't contain circular references.
            string json = JsonConvert.SerializeObject(sms);

            //Needed to setup the body of the request
            StringContent data = new StringContent(json, Encoding.UTF8, "application/json");

            //The url to post to.
            var url = urlToPost;
            var client = new HttpClient();

            //Pass in the full URL and the json string content
            var response = await client.PostAsync(url, data);

            //It would be better to make sure this request actually made it through
            string result = await response.Content.ReadAsStringAsync();

            //close out the client
            client.Dispose();
            return result;
        }
        public static SMS_Log SendSMSTelenor(SMS sms)
        {
            string userName = "923464950571";
            string password = "Hisdu%40012_p%26shd-Hisdu%2Fp%26shd";

            QuickMessage obj = new QuickMessage(userName, password);
            SMS_Session session = obj.getSessionId(sms.UserId);
            if (session != null)
            {
                sms.Mask = sms.Mask == null ? "HISDU" : sms.Mask;
                SMS_Log message = obj.sendQuickMessage(session, sms);
                return message;
            }
            return null;
        }
        public static SMS_Log CheckMessageStatus(int smsLogId)
        {
            string userName = "923464950571";
            string password = "Hisdu%40012_p%26shd-Hisdu%2Fp%26shd";

            QuickMessage obj = new QuickMessage(userName, password);
            SMS_Session session = obj.getSessionId("");
            if (session != null)
            {
                SMS_Log message = obj.checkStatus(session, smsLogId);
                return message;
            }
            return null;
        }
        public static string DashifyCNIC(string cnic)
        {
            if (cnic.Length != 13)
            {
                return null;
            }
            return cnic[0].ToString() +
                   cnic[1].ToString() +
                   cnic[2].ToString() +
                   cnic[3].ToString() +
                   cnic[4].ToString() +
                   "-" +
                   cnic[5].ToString() +
                   cnic[6].ToString() +
                   cnic[7].ToString() +
                   cnic[8].ToString() +
                   cnic[9].ToString() +
                   cnic[10].ToString() +
                   cnic[11].ToString() +
                   "-" +
                   cnic[12].ToString();
        }
        public static Image barCodeZ(long Id)
        {
            Zen.Barcode.Code128BarcodeDraw barcode = Zen.Barcode.BarcodeDrawFactory.Code128WithChecksum;
            return barcode.Draw(Convert.ToString(Id), 90, 2);
            //Code128BarcodeDraw barcode = BarcodeDrawFactory.Code128WithChecksum;
            //BarcodeSymbology s = BarcodeSymbology.Code39NC;
            //BarcodeDraw drawObject = BarcodeDrawFactory.GetSymbology(s);
            //var metrics = drawObject.GetDefaultMetrics(40);
            //metrics.Scale = 1;
            //return barcode.Draw(Convert.ToString(Id), metrics);
        }
        //public static void SendEmail(string to, string subject, string body)
        //{
        //    try
        //    {
        //        ////string FromEmailAddress = "hisduhr@gmail.com";
        //        ////string SMTP = "smtp.gmail.com";
        //        ////string Password = "hisdu@123";

        //        //string FromEmailAddress = "no-reply@pshealthpunjab.gov.pk";
        //        //string SMTP = "mail.pshealthpunjab.gov.pk";
        //        //string Password = "pshe@lth123";

        //        ////string FromEmailAddress = "pshealth@codenax.com";
        //        ////string SMTP = "serverfun.funchit.com";
        //        ////string Password = "xBNpeU0LvH";
        //        //MailMessage mail = new MailMessage();
        //        //mail.To.Add(to);
        //        //mail.From = new MailAddress(FromEmailAddress);
        //        //mail.Subject = subject;
        //        //mail.Body = body;
        //        //mail.IsBodyHtml = true;
        //        //SmtpClient smtp = new SmtpClient(SMTP);
        //        //smtp.Host = "mail.pshealthpunjab.gov.pk";
        //        //smtp.Port = 25;
        //        //smtp.UseDefaultCredentials = false;
        //        //smtp.Credentials = new System.Net.NetworkCredential
        //        //(FromEmailAddress, Password,"pshealthpunjab");// Enter seders User name and password
        //        //smtp.EnableSsl = false;
        //        //smtp.Send(mail);

        //        //==========================================================================================
        //        //SmtpClient smtpClient = new SmtpClient();
        //        //NetworkCredential basicCredential = new NetworkCredential("no-reply@pshealthpunjab.gov.pk", "pshe@lth123");
        //        //MailMessage message = new MailMessage();
        //        //MailAddress fromAddress = new MailAddress("no-reply@pshealthpunjab.gov.pk");

        //        //// setup up the host, increase the timeout to 5 minutes
        //        //smtpClient.Host = "mail.pshealthpunjab.gov.pk";
        //        //smtpClient.UseDefaultCredentials = false;
        //        //smtpClient.Credentials = basicCredential;
        //        //smtpClient.Port = 587;
        //        //smtpClient.EnableSsl = true;
        //        //smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
        //        //message.From = fromAddress;
        //        //message.Subject = subject;
        //        //message.IsBodyHtml = true;
        //        //message.Body = body;
        //        //message.To.Add(to);
        //        //smtpClient.Send(message);
        //    }
        //    catch (Exception ex)
        //    {
        //        File.AppendAllText(HttpContext.Current.Server.MapPath("~/Logs/Email.txt"), (ex.InnerException?.Message ?? ex.Message) + "  " + DateTime.Now.ToString("D"));
        //    }
        //}

        //public static void SendEmail(string to, string subject, string body)
        //{
        //try
        //{
        //    string FromEmailAddress = "hisduhr@gmail.com";
        //    string SMTP = "smtp.gmail.com";
        //    string Password = "hisdu@123";
        //    //string FromEmailAddress = "pshealth@codenax.com";
        //    //string SMTP = "serverfun.funchit.com";
        //    //string Password = "xBNpeU0LvH";
        //    MailMessage mail = new MailMessage();
        //    mail.To.Add(to);
        //    mail.From = new MailAddress(FromEmailAddress);
        //    mail.Subject = subject;
        //    mail.Body = body;
        //    mail.IsBodyHtml = true;
        //    SmtpClient smtp = new SmtpClient(SMTP);
        //    //  smtp.Host = "smtp.gmail.com";
        //    smtp.Port = 587;
        //    smtp.UseDefaultCredentials = false;
        //    smtp.Credentials = new System.Net.NetworkCredential
        //    (FromEmailAddress, Password) as ICredentialsByHost; ;// Enter seders User name and password
        //    smtp.EnableSsl = false;
        //    smtp.Send(mail);
        //}
        //catch (Exception ex)
        //{
        //    File.AppendAllText(HttpContext.Current.Server.MapPath("~/Logs/Email.txt"), (ex.InnerException?.Message ?? ex.Message) + "  " + DateTime.Now.ToString("D"));
        //}
        //}


        public static void SendEmail(string to, string subject, string body)
        {
            //try
            //{
            //    string FromEmailAddress = "hisduhr@gmail.com";
            //    string SMTP = "smtp.gmail.com";
            //    string Password = "hisdu@123";
            //    //string FromEmailAddress = "pshealth@codenax.com";
            //    //string SMTP = "serverfun.funchit.com";
            //    //string Password = "xBNpeU0LvH";
            //    MailMessage mail = new MailMessage();
            //    mail.To.Add(to);
            //    mail.From = new MailAddress(FromEmailAddress);
            //    mail.Subject = subject;
            //    mail.Body = body;
            //    mail.IsBodyHtml = true;
            //    SmtpClient smtp = new SmtpClient(SMTP);
            //    //  smtp.Host = "smtp.gmail.com";
            //    smtp.Port = 587;
            //    smtp.UseDefaultCredentials = false;
            //    smtp.Credentials = new System.Net.NetworkCredential
            //    (FromEmailAddress, Password) as ICredentialsByHost;// Enter senders User name and password
            //    smtp.EnableSsl = true;
            //    smtp.Send(mail);
            //}
            //catch (Exception ex)
            //{
            //    File.AppendAllText(HttpContext.Current.Server.MapPath("~/Logs/Email.txt"), (ex.InnerException?.Message ?? ex.Message) + "  " + DateTime.Now.ToString("D"));
            //}
        }


        public static void SendEmailWithExchange(string to, string subject, string body)
        {

            try
            {
                ExchangeService service = new ExchangeService(ExchangeVersion.Exchange2013_SP1);
                service.Credentials = new WebCredentials("no-reply", "pshe@lth123", "pshealthpunjab");
                //service.AutodiscoverUrl("no-reply@pshealthpunjab.gov.pk");
                service.Url = new Uri("https://mail.pshealthpunjab.gov.pk/EWS/Exchange.asmx");
                EmailMessage message = new EmailMessage(service);
                message.Subject = subject;
                message.Body = body;
                message.ToRecipients.Add(to);
                message.Save();
                message.SendAndSaveCopy();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static bool SendEmailOfficial(string userEmail, string subject, string body)
        {
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress("admin@pshealthpunjab.gov.pk");
            mailMessage.To.Add(new MailAddress(userEmail));
            mailMessage.BodyEncoding = System.Text.Encoding.UTF8;
            mailMessage.Subject = subject;
            mailMessage.IsBodyHtml = true;
            mailMessage.Body = body;

            SmtpClient client = new SmtpClient();
            client.UseDefaultCredentials = true;
            //client.Credentials = new System.Net.NetworkCredential("khizarjaved29@gmail.com", "Kh@n10140");
            client.Credentials = new System.Net.NetworkCredential("admin@pshealthpunjab.gov.pk", "P&SHD@hisdu@786");
            client.Host = "smtp.gmail.com";
            client.Port = 587;
            client.EnableSsl = true;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            try
            {
                client.Send(mailMessage);
                return true;
            }
            catch (Exception ex)
            {
                // log exception
            }
            return false;
        }

        public static void EmailToMe(string userame, string userId, string exMessage, Exception ex)
        {
            List<string> data = new List<string>();
            data.Add("belalmughal@gmail.com");

            int count = 1;
            string exceptionsMessage = count++ + ": " + ex.Message + "\n";
            while (ex.InnerException != null)
            {
                ex = ex.InnerException;
                exceptionsMessage = count++ + ": " + ex.Message + "\n";
            }
            var body =
                "<p>" + DateTime.Now.ToString("dd.MM.yyyy hh:mm a") + "</p>" +
                "<p>" + userId + "</p>" +
                "<p>" + userame + "</p>" +
                "<p>" + exceptionsMessage + "</p>" +
                "<p>" + ex.StackTrace?.ToString() + "</p>";

            foreach (var item in data)
            {
                SendEmail(item, "Vivant, SE", body);
            }
        }
        public static void EmailToMe(string userame, string userId, string exMessage)
        {
            List<string> data = new List<string>();

            data.Add("belalmughal@gmail.com");

            var body =
                "<p>" + DateTime.Now.ToLongDateString() + "</p>" +
                "<p>" + userId + "</p>" +
                "<p>" + userame + "</p>" +
                "<p>" + exMessage + "</p>";

            foreach (var item in data)
            {
                SendEmail(item, "Vacancy Anamoly, SE", body);
            }
        }
        private static Random random = new Random();

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHJKMPRSTUVWYZ23456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public static string RandomCodeString(int length)
        {
            const string chars = "234569";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static int CalculateDays(string fromDate, string toDate)
        {
            string[] startDateSplit = fromDate.Split('-');
            string[] endDateSplit = toDate.Split('-');
            DateTime st = DateTime.Parse(fromDate);
            DateTime tt = DateTime.Parse(toDate);
            return ((tt - st).Days + 1);
        }
        public static int CalculateDays(DateTime fromDate, DateTime toDate)
        {
            return ((fromDate - toDate).Days + 1);
        }
        public static DateTime ToDate(string fromDate, int totalDays)
        {
            return DateTime.Parse(fromDate).AddDays(totalDays - 1);
        }

    }

    public class MobileResponse
    {
        public bool isException { get; set; }
        public int typeInOut { get; set; }
        public string message { get; set; }
    }

    public class FingerprintSdk
    {

        private const int PROBABILITY_ONE = 0x7fffffff;

        private readonly ILog log = LogManager.GetLogger(typeof(FingerprintSdk));


        public P_SOfficers SearchOfficer(String base64)
        {
            using (HR_System db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                List<P_SOfficers> officers = db.P_SOfficers.Where(x => x.FingerPrint_Id != null).ToList();
                Fmd fmd = GetFmd(base64);
                foreach (var officer in officers)
                {
                    FingerPrint currentFps = db.FingerPrints.FirstOrDefault(x => x.Id == officer.FingerPrint_Id);
                    if (currentFps != null)
                    {
                        if (currentFps.FP1 != null)
                        {
                            try
                            {
                                if (compare(fmd, Fmd.DeserializeXml(currentFps.FP1)))
                                {
                                    return officer;
                                }
                            }
                            catch (Exception ex)
                            {

                            }

                        }
                        if (currentFps.FP2 != null)
                        {
                            try
                            {
                                if (compare(fmd, Fmd.DeserializeXml(currentFps.FP2)))
                                {
                                    return officer;
                                }
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                        if (currentFps.FP3 != null)
                        {
                            try
                            {
                                if (compare(fmd, Fmd.DeserializeXml(currentFps.FP3)))
                                {
                                    return officer;
                                }
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                        if (currentFps.FP4 != null)
                        {
                            try
                            {
                                if (compare(fmd, Fmd.DeserializeXml(currentFps.FP4)))
                                {
                                    return officer;
                                }
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                        if (currentFps.FP5 != null)
                        {
                            try
                            {
                                if (compare(fmd, Fmd.DeserializeXml(currentFps.FP5)))
                                {
                                    return officer;
                                }
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }
                }
                return null;
            }

        }


        public MobileResponse SearchProfile(string base64, int profile_Id, int typeInOut)
        {
            using (HR_System db = new HR_System())
            {

                db.Configuration.ProxyCreationEnabled = false;
                int c = 0;
                bool matched = false;
                Fmd fmd = GetFmd(base64);
                HrFp currentFps = db.HrFps.FirstOrDefault(x => x.Profile_Id == profile_Id);
                log.Info($"Line No. 312.");
                if (currentFps != null)
                {
                    c = 1;
                    log.Info($"Line No. 316.");
                    if (currentFps.FP1 != null)
                    {
                        try
                        {
                            log.Info($"Line No. 320.");
                            if (compare(fmd, Fmd.DeserializeXml(currentFps.FP1)))
                            {
                                matched = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            log.Info($"Line No. 329.");
                            while (ex.InnerException != null) { ex = ex.InnerException; }
                            return new MobileResponse() { isException = false, message = ex.Message, typeInOut = typeInOut };
                        }

                    }
                    if (currentFps.FP2 != null)
                    {
                        try
                        {
                            if (compare(fmd, Fmd.DeserializeXml(currentFps.FP2)))
                            {
                                matched = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            while (ex.InnerException != null) { ex = ex.InnerException; }
                            return new MobileResponse() { isException = false, message = ex.Message, typeInOut = typeInOut };
                        }
                    }
                    if (currentFps.FP3 != null)
                    {
                        try
                        {
                            if (compare(fmd, Fmd.DeserializeXml(currentFps.FP3)))
                            {
                                matched = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            while (ex.InnerException != null) { ex = ex.InnerException; }
                            return new MobileResponse() { isException = false, message = ex.Message, typeInOut = typeInOut };
                        }
                    }
                    if (currentFps.FP4 != null)
                    {
                        try
                        {
                            if (compare(fmd, Fmd.DeserializeXml(currentFps.FP4)))
                            {
                                matched = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            while (ex.InnerException != null) { ex = ex.InnerException; }
                            return new MobileResponse() { isException = false, message = ex.Message, typeInOut = typeInOut };
                        }
                    }
                    if (currentFps.FP5 != null)
                    {
                        try
                        {
                            if (compare(fmd, Fmd.DeserializeXml(currentFps.FP5)))
                            {
                                matched = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            while (ex.InnerException != null) { ex = ex.InnerException; }
                            return new MobileResponse() { isException = false, message = ex.Message, typeInOut = typeInOut };
                        }
                    }
                }
                else
                {
                    c = 2;
                    log.Info($"Line No. 399.");
                    return new MobileResponse() { isException = false, message = "No Record Found!", typeInOut = typeInOut };
                }
                log.Info($"Line No. 402.");
                if (matched)
                {
                    HrAttendance hrAttendance = new HrAttendance();
                    DateTime today = DateTime.Today;

                    var dbHRAttendance = db.HrAttendances.FirstOrDefault(x => x.Profile_Id == profile_Id && x.DateTimeIN == today);
                    if (dbHRAttendance == null)
                    {
                        typeInOut = 1;
                        hrAttendance.DateTimeIN = DateTime.UtcNow.AddHours(5);
                        hrAttendance.Profile_Id = profile_Id;
                        db.HrAttendances.Add(hrAttendance);
                        db.SaveChanges();
                        c = 3;
                        return new MobileResponse() { isException = false, message = "Successfull!", typeInOut = typeInOut };
                    }
                    else
                    {
                        typeInOut = 2;
                        dbHRAttendance.DateTimeOut = DateTime.UtcNow.AddHours(5);
                        db.Entry(dbHRAttendance).State = EntityState.Modified;
                        db.SaveChanges();
                        c = 4;
                        return new MobileResponse() { isException = false, message = "Successfull!", typeInOut = typeInOut };
                    }
                }
                else
                {
                    c = 5;
                    return new MobileResponse() { isException = false, message = "Failed!", typeInOut = typeInOut };
                }
            }
        }

        public bool compare(Fmd Fmd1, Fmd Fmd2)
        {
            var compareResult = Comparison.Compare(Fmd1, 0, Fmd2, 0);
            if ((compareResult.Score < (PROBABILITY_ONE / 100000)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        //public object SearchPerson(Fmd fmd)
        //{
        //    List<object> persons = new List<object>();
        //    foreach (var item in persons)
        //    {
        //        var personFmd = Fmd.DeserializeXml(item.finger);
        //        if(compare(personFmd, fmd))
        //        {
        //            return item;
        //        }
        //    }
        //}

        public Fmd GetFmd(string base64string)
        {
            var finger = base64string;
            byte[] data = Convert.FromBase64String(finger);
            var stream = new MemoryStream(data, 0, data.Length);
            Image image = Image.FromStream(stream);
            byte[] arr = Encoding.ASCII.GetBytes(finger);




            Reader reader = new Reader();
            var fmd =
                DPUruNet.FeatureExtraction.CreateFmdFromRaw(
                        ExtractByteArray(new Bitmap(image)),
                        0,
                        0,
                        image.Width,
                        image.Height,
                        500, 
                        Constants.Formats.Fmd.ANSI)
                    .Data;
            return fmd;
        }
        public string Getbase64FromFMD(string dbObj)
        {
            if (dbObj == null) return null;
            var fmd = Fmd.DeserializeXml(dbObj);
            var fmdBytes = Fmd.DeserializeXml(dbObj).Bytes;
            return Convert.ToBase64String(fmdBytes);
        }
        public Image GetImage(string base64string)
        {
            var finger = base64string;
            byte[] data = Convert.FromBase64String(finger);
            var stream = new MemoryStream(data, 0, data.Length);
            return Image.FromStream(stream);
        }


        public Bitmap CreateBitmap(byte[] bytes, int width, int height)
        {
            byte[] rgbBytes = new byte[bytes.Length * 3];

            for (int i = 0; i <= bytes.Length - 1; i++)
            {
                rgbBytes[(i * 3)] = bytes[i];
                rgbBytes[(i * 3) + 1] = bytes[i];
                rgbBytes[(i * 3) + 2] = bytes[i];
            }
            Bitmap bmp = new Bitmap(width, height, PixelFormat.Format24bppRgb);

            BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

            for (int i = 0; i <= bmp.Height - 1; i++)
            {
                IntPtr p = new IntPtr(data.Scan0.ToInt64() + data.Stride * i);
                System.Runtime.InteropServices.Marshal.Copy(rgbBytes, i * bmp.Width * 3, p, bmp.Width * 3);
            }

            bmp.UnlockBits(data);

            return bmp;
        }


        private static byte[] ExtractByteArray(Bitmap img)
        {
            byte[] rawData = null;
            byte[] bitData = null;
            //ToDo: CreateFmdFromRaw only works on 8bpp bytearrays. As such if we have an image with 24bpp then average every 3 values in Bitmapdata and assign it to bitdata
            if (img.PixelFormat == PixelFormat.Format8bppIndexed)
            {

                //Lock the bitmap's bits
                BitmapData bitmapdata = img.LockBits(new System.Drawing.Rectangle(0, 0, img.Width, img.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, img.PixelFormat);
                //Declare an array to hold the bytes of bitmap
                byte[] imgData = new byte[bitmapdata.Stride * bitmapdata.Height]; //stride=360, height 392

                //Copy bitmapdata into array
                Marshal.Copy(bitmapdata.Scan0, imgData, 0, imgData.Length);//imgData.length =141120

                bitData = new byte[bitmapdata.Width * bitmapdata.Height];//ditmapdata.width =357, height = 392

                for (int y = 0; y < bitmapdata.Height; y++)
                {
                    for (int x = 0; x < bitmapdata.Width; x++)
                    {
                        bitData[bitmapdata.Width * y + x] = imgData[y * bitmapdata.Stride + x];
                    }
                }

                rawData = new byte[bitData.Length];

                for (int i = 0; i < bitData.Length; i++)
                {
                    int avg = (img.Palette.Entries[bitData[i]].R + img.Palette.Entries[bitData[i]].G + img.Palette.Entries[bitData[i]].B) / 3;
                    rawData[i] = (byte)avg;
                }
            }

            else
            {
                bitData = new byte[img.Width * img.Height];//ditmapdata.width =357, height = 392, bitdata.length=139944
                for (int y = 0; y < img.Height; y++)
                {
                    for (int x = 0; x < img.Width; x++)
                    {
                        Color pixel = img.GetPixel(x, y);
                        bitData[img.Width * y + x] = (byte)((Convert.ToInt32(pixel.R) + Convert.ToInt32(pixel.G) + Convert.ToInt32(pixel.B)) / 3);
                    }
                }

            }

            return bitData;
        }
    }

    public class SMS
    {
        public string MobileNumber { get; set; }
        public string Message { get; set; }
        public string From { get; set; }
        public string Status { get; set; }
        public string Mask { get; set; }
        public int FKId { get; set; }
        public string UserId { get; set; }
    }

    public class Result<T> where T : class
    {
        public string Type { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public Exception exception { get; set; }
        public int TotalRecords { get; set; } // Total Records for pagination
    }



}
using Hrmis.Models.Common;
using Hrmis.Models.DbModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Hrmis.Models.Services
{
    public class UserService
    {
        public C_User GetUser(string userId)
        {
            using (var _db = new HR_System())
            {
                var user = _db.C_User.FirstOrDefault(x => x.Id.Equals(userId));
                user.hfmiscode = (user.hfmiscode != null ?
                user.hfmiscode : user.TehsilID != null ?
                    user.TehsilID : user.DistrictID != null ?
                        user.DistrictID : user.DivisionID != null ?
                            user.DivisionID : "0");
                return user;
            }
        }
        public bool AlertRegisteredUser(HrProfile profile, ApplicationUser user, string currentuserName)
        {
            string MessageBody = @"Dear " + profile.EmployeeName + ",\nCNIC: " + profile.CNIC + "\n\nYour account has been REGISTERED on HRMIS (Human Resource Management Information System).\n\nAccount Credentials:-\n" +
                "Username: " + user.UserName + "\nPassword: " + user.hashynoty + "\nYou can change your PASSWORD from MY ACCOUNT section in HRMIS.\n\nRegards,\nHealth Information and Services Delivery Unit.\nPrimary and Secondary Healthcare Department.";

            //List<SMS> smsy = new List<SMS>();
            //SMS sms = new SMS()
            //{
            //    MobileNumber = profile.MobileNo,
            //    Message = MessageBody

            //};
            //smsy.Add(sms);

            //Common.Common.SMS_Send(smsy);
            SMS sms = new SMS()
            {
                UserId = user.Id,
                FKId = 0,
                MobileNumber = user.PhoneNumber,
                Message = MessageBody
            };
            Thread t = new Thread(() => Common.Common.SendSMSTelenor(sms));
            t.Start();
            string emailBody = @"
                               <p><strong>Dear</strong> " + profile.EmployeeName + @",</p>
                                <p><strong>CNIC</strong>: " + profile.CNIC + @" </p>
                                <p>Your account has been registered on <a href='https://hrmis.pshealthpunjab.gov.pk' target='_blank'>HRMIS</a> - (Human Resource Management Information System)</p>
                                <p><strong>Account Credentials </strong >:-</p>
        
                                        <p><strong>Username </strong>: " + user.UserName + @" </p>
                
                                                <p><strong>Password </strong>:&nbsp; " + user.hashynoty + @" </p>
                         
                                                         <p></p>
                         
                                                         <p>You can change your password from<u> MY ACCOUNT </u> section in HRMIS.</p>
                               
                                                               <p><a href =""https://hrmis.pshealthpunjab.gov.pk"" target=""_blank"">Click Here To Login </a></p>
                                        
                                                                        <p> &nbsp;</p>
                                           
                                                                           <p><strong> Regards,</strong ></p>
                                                
                                                                                <p><span style ='text-decoration: underline;'><em>Health Information and Services Delivery Unit </em></span></p>
                                                         
                                                                                            <p><span style ='text-decoration: underline;'><em>Primary & Secondary Healthcare Department </em></span></p>
                                                                             ";
            if (!string.IsNullOrEmpty(profile.EMaiL) && !profile.EMaiL.Equals("abc@gmail.com"))
            {
                Common.Common.SendEmail(profile.EMaiL, "HRMIS - Account Registration", emailBody);
            }

            return true;
        }
        public async Task<bool> AlertPublicUser(CreateUserViewModel user, string currentuserName)
        {
            string MessageBody = "";
            if (user.roles[0].Equals("JobApplicantAdhoc"))
            {
                MessageBody = @"CNIC: " + user.Cnic + "\n\nYour account has been REGISTERED on Adhoc Portal.\n\nPlease login through given username and password:-\n" +
               "Username: " + user.UserName + "\nPassword: " + user.Password + "\n\nRegards,\nHealth Information and Services Delivery Unit.\nPrimary and Secondary Healthcare Department.";
            }
            else
            {
                MessageBody = @"CNIC: " + user.Cnic + "\n\nYour account has been REGISTERED on HRMIS (Human Resource Management Information System).\n\nAccount Credentials:-\n" +
                "Username: " + user.UserName + "\nPassword: " + user.Password + "\n\nRegards,\nHealth Information and Services Delivery Unit.\nPrimary and Secondary Healthcare Department.";
            }

            //List<SMS> smsy = new List<SMS>();
            //SMS sms = new SMS()
            //{
            //    MobileNumber = user.PhoneNumber,
            //    Message = MessageBody

            //};
            //smsy.Add(sms);
            user.PhoneNumber = !user.PhoneNumber.ToString().StartsWith("0") ? "0" + user.PhoneNumber : user.PhoneNumber;
            SMS sms = new SMS()
            {
                UserId = user.Id,
                FKId = 0,
                //MobileNumber = "03214677763",
                MobileNumber = user.PhoneNumber,
                Message = MessageBody
            };

            if (!string.IsNullOrEmpty(user.PhoneNumberSecondary) && !user.PhoneNumber.Equals(user.PhoneNumberSecondary))
            {
                SMS sms2 = new SMS()
                {
                    UserId = user.Id,
                    FKId = 0,
                    //MobileNumber = "03214677763",
                    MobileNumber = user.PhoneNumberSecondary,
                    Message = MessageBody
                };
                Thread t2 = new Thread(() => Common.Common.SendSMSTelenor(sms2));
                t2.Start();
                //await Common.Common.SendSMSTelenor(sms2);
            }
            Thread t = new Thread(() => Common.Common.SendSMSTelenor(sms));
            t.Start();
            //await Common.Common.SendSMSTelenor(sms);

            //Common.Common.SMS_Send(smsy);

            string emailBody = @"
                                <p>Registration Successfull</p>
                                <p><strong>Account Credentials </strong >:-</p>
                                        <p><strong>CNIC </strong>: " + user.UserName + @" </p>
                                                <p><strong>Password </strong>:&nbsp; " + user.Password + @" </p>
                                                         <p></p>
                                                               <p><a href =""https://hrmis.pshealthpunjab.gov.pk/ppsc"" target=""_blank"">Click Here To Login </a></p>
                                        
                                                                        <p> &nbsp;</p>
                                           
                                                                           <p><strong> Regards,</strong ></p>
                                                
                                                                                <p><span style ='text-decoration: underline;'><em>Health Information and Service Delivery Unit </em></span></p>
                                                         
                                                                                            <p><span style ='text-decoration: underline;'><em>Primary & Secondary Healthcare Department </em></span></p>
                                                                             ";

            if (!string.IsNullOrEmpty(user.Email) && !user.Email.Equals("abc@gmail.com"))
            {
                Common.Common.SendEmailOfficial(user.Email, "P&S Healthcare Department - Account Registration", emailBody);
                //Common.Common.SendEmail("belalmughal@gmail.com", "P&S Healthcare Department - Account Registration", emailBody);
            }
            if (!string.IsNullOrEmpty(user.EmailSecondary) && !user.Email.Equals(user.EmailSecondary) && !user.EmailSecondary.Equals("abc@gmail.com"))
            {
                //Common.Common.SendEmail("belalmughal@gmail.com", "P&S Health Department - Account Registration", emailBody);
                Common.Common.SendEmailOfficial(user.Email, "P&S Health Department - Account Registration", emailBody);
            }
            return true;
        }
        public int SendAuthenticationCode(string username, string phoneNumber, string email)
        {
            int code = new Random().Next(1000, 9999);
            SMS sms = new SMS()
            {
                UserId = "Login Auth",
                FKId = 0,
                MobileNumber = phoneNumber,
                Message = "Your HRMIS authentication code is " + code + "\nUsername: " + username
            };
            Common.Common.SendSMSTelenor(sms);

            string emailBody = @"<p>HRMIS Authentication </p>
                                 <p>Your HRMIS authentication code is <strong> " + code + @"</strong></p>
                                 <p>Username: <strong>" + username + @"</strong></p><p></p> <p>&nbsp;</p> <p><strong> Regards,</strong></p>
                                 <p><span style ='text-decoration: underline;'><em>Health Information and Service Delivery Unit </em></span></p>
                                 <p><span style ='text-decoration: underline;'><em>Primary & Secondary Healthcare Department </em></span></p>
                                                                             ";
            if (!string.IsNullOrEmpty(email) && !email.Equals("abc@gmail.com"))
            {
                Common.Common.SendEmail(email, "HRMIS, P&S Healthcare Department - Authentication Code", emailBody);
            }
            return code;
        }

        public List<C_ErpModule> GetNav(string userName, IList<string> Roles)
        {
            using (var _db = new HR_System())
            {
                List<C_ErpModule> nav;
                userName = userName.ToLower();
                string roleName = Roles.FirstOrDefault();
                var allowedRoutes = new List<int>();
                if (roleName.Equals("HR Admin"))
                {
                    allowedRoutes.Add((int)AngularRoutesNav.DASHBAORD);
                    allowedRoutes.Add((int)AngularRoutesNav.HEALTHFACILITY);
                    //allowedRoutes.Add((int)AngularRoutesNav.PROFILE);        // commented by Adnan on 06/10/2022 on order of Ahmer sb/Khalid sb
                    allowedRoutes.Add((int)AngularRoutesNav.VACANCY);
                    allowedRoutes.Add((int)AngularRoutesNav.APPLICATION);
                    allowedRoutes.Add((int)AngularRoutesNav.SEARCHTRACKING);
                    allowedRoutes.Add((int)AngularRoutesNav.ORDER);
                    allowedRoutes.Add((int)AngularRoutesNav.SCANNEDFILES);
                    allowedRoutes.Add((int)AngularRoutesNav.PPSCCANDIDATES);
                    allowedRoutes.Add((int)AngularRoutesNav.PROMOTIONAPPLICATION);
                }
                else if (userName.Equals("dpd"))
                {
                    allowedRoutes.Add((int)AngularRoutesNav.DASHBAORD);
                    allowedRoutes.Add((int)AngularRoutesNav.HEALTHFACILITY);
                    //allowedRoutes.Add((int)AngularRoutesNav.PROFILE);
                    allowedRoutes.Add((int)AngularRoutesNav.VACANCY);
                    allowedRoutes.Add((int)AngularRoutesNav.APPLICATION);
                    allowedRoutes.Add((int)AngularRoutesNav.ORDER);
                    //allowedRoutes.Add((int)AngularRoutesNav.RETIREMENTEMPLOYEE);
                    allowedRoutes.Add((int)AngularRoutesNav.INBOX);
                    allowedRoutes.Add((int)AngularRoutesNav.MYAPPLICATIONS);
                    allowedRoutes.Add((int)AngularRoutesNav.SCANNEDFILES);
                    allowedRoutes.Add((int)AngularRoutesNav.SENIORITYLIST);
                    //allowedRoutes.Add((int)AngularRoutesNav.MONTHLYLEAVE);
                    //allowedRoutes.Add((int)AngularRoutesNav.LEAVESDETAIL);
                    //allowedRoutes.Add((int)AngularRoutesNav.DAILYATTENDANCE);
                    //allowedRoutes.Add((int)AngularRoutesNav.ATTENDANCEREPORT);
                    //allowedRoutes.Add((int)AngularRoutesNav.FILEREQUISITION);
                    //allowedRoutes.Add((int)AngularRoutesNav.RIBRANCH);
                    //allowedRoutes.Add((int)AngularRoutesNav.USER);
                    allowedRoutes.Add((int)AngularRoutesNav.PPSCCANDIDATES);
                    // allowedRoutes.Add((int)AngularRoutesNav.PROMOTIONAPPLICATION);
                    //allowedRoutes.Add((int)AngularRoutesNav.DAILYCALLSLIST);
                    //allowedRoutes.Add((int)AngularRoutesNav.INQUIRYFILES);
                    allowedRoutes.Add((int)AngularRoutesNav.ADHOCAPPLICATIONS);
                    allowedRoutes.Add((int)AngularRoutesNav.MERITLIST);
                    allowedRoutes.Add((int)AngularRoutesNav.NEWDOCUMENT);
                    allowedRoutes.Add((int)AngularRoutesNav.DAILYWAGERPROFILE);

                    //allowedRoutes.Add((int)AngularRoutesNav.PROFILEREVIEW);
                }
                else if (userName.Equals("Saadat"))
                {
                    allowedRoutes.Add((int)AngularRoutesNav.DASHBAORD);
                    allowedRoutes.Add((int)AngularRoutesNav.HEALTHFACILITY);
                    //allowedRoutes.Add((int)AngularRoutesNav.PROFILE);            // commented by Adnan on 06/10/2022 on order of Ahmer sb/Khalid sb
                    allowedRoutes.Add((int)AngularRoutesNav.VACANCY);
                    allowedRoutes.Add((int)AngularRoutesNav.APPLICATION);
                    allowedRoutes.Add((int)AngularRoutesNav.ORDER);
                    allowedRoutes.Add((int)AngularRoutesNav.SEARCHTRACKING);
                }
                else if (roleName.Equals("Data Entry"))
                {
                    allowedRoutes.Add((int)AngularRoutesNav.DASHBAORD);
                    allowedRoutes.Add((int)AngularRoutesNav.NEWPROFILE);
                    allowedRoutes.Add((int)AngularRoutesNav.PPSCCANDIDATES);
                    allowedRoutes.Add((int)AngularRoutesNav.PPSCPROFILE);
                }
                else if (roleName.Equals("Administrative Office"))
                {
                    allowedRoutes.Add((int)AngularRoutesNav.DASHBAORD);
                    allowedRoutes.Add((int)AngularRoutesNav.NEWDOCUMENT);
                    allowedRoutes.Add((int)AngularRoutesNav.SEARCHTRACKING);
                }
                else if (roleName.Equals("Admin"))
                {
                    allowedRoutes.Add((int)AngularRoutesNav.DASHBAORD);
                    allowedRoutes.Add((int)AngularRoutesNav.HEALTHFACILITY);
                    //allowedRoutes.Add((int)AngularRoutesNav.PROFILE);         // commented by Adnan on 06/10/2022 on order of Ahmer sb/Khalid sb
                    allowedRoutes.Add((int)AngularRoutesNav.VACANCY);
                    allowedRoutes.Add((int)AngularRoutesNav.APPLICATION);
                    allowedRoutes.Add((int)AngularRoutesNav.ORDER);
                    allowedRoutes.Add((int)AngularRoutesNav.SCANNEDFILES);
                    allowedRoutes.Add((int)AngularRoutesNav.MYACCOUNT);
                    allowedRoutes.Add((int)AngularRoutesNav.SEARCHTRACKING);
                    allowedRoutes.Add((int)AngularRoutesNav.DAILYWAGERPROFILE);
                }
                else if (roleName.Equals("Helpline"))
                {
                    allowedRoutes.Add((int)AngularRoutesNav.SEARCHTRACKING);
                    allowedRoutes.Add((int)AngularRoutesNav.ADHOCUSER);
                }
                else if (roleName.Equals("PHFMC Admin") || roleName.Equals("PHFMC"))
                {
                    allowedRoutes.Add((int)AngularRoutesNav.DASHBAORD);
                    allowedRoutes.Add((int)AngularRoutesNav.HEALTHFACILITY);
                    //allowedRoutes.Add((int)AngularRoutesNav.PROFILE);         // commented by Adnan on 06/10/2022 on order of Ahmer sb/Khalid sb
                    allowedRoutes.Add((int)AngularRoutesNav.VACANCY);
                    allowedRoutes.Add((int)AngularRoutesNav.ORDER);
                    allowedRoutes.Add((int)AngularRoutesNav.MYACCOUNT);
                    allowedRoutes.Add((int)AngularRoutesNav.ORDERREQUEST);
                    if (userName.Equals("phfmcadmin"))
                    {
                        allowedRoutes.Add((int)AngularRoutesNav.JOBS);
                        allowedRoutes.Add((int)AngularRoutesNav.JOBAPPLICATION);
                        allowedRoutes.Add((int)AngularRoutesNav.USER);
                    }
                    if (userName.Equals("phfmc.hr"))
                    {
                        allowedRoutes.Add((int)AngularRoutesNav.JOBS);
                        allowedRoutes.Add((int)AngularRoutesNav.JOBAPPLICATION);
                    }
                }

                else if (roleName.Equals("ViewOnly"))
                {
                    allowedRoutes.Add((int)AngularRoutesNav.DASHBAORD);
                    allowedRoutes.Add((int)AngularRoutesNav.APPLICATION);
                    allowedRoutes.Add((int)AngularRoutesNav.SEARCHTRACKING);
                }
                else if (userName.Equals("so.toqeer"))
                {
                    allowedRoutes.Add((int)AngularRoutesNav.DASHBAORD);
                    allowedRoutes.Add((int)AngularRoutesNav.HEALTHFACILITY);
                    allowedRoutes.Add((int)AngularRoutesNav.PROFILE);       // commented by Adnan on 06/10/2022 on order of Ahmer sb/Khalid sb
                    allowedRoutes.Add((int)AngularRoutesNav.VACANCY);
                    allowedRoutes.Add((int)AngularRoutesNav.DATABASE);
                    allowedRoutes.Add((int)AngularRoutesNav.INBOX);
                    allowedRoutes.Add((int)AngularRoutesNav.MYAPPLICATIONS);
                    allowedRoutes.Add((int)AngularRoutesNav.SENT);
                    allowedRoutes.Add((int)AngularRoutesNav.MYACCOUNT);
                    allowedRoutes.Add((int)AngularRoutesNav.SEARCHTRACKING);
                    allowedRoutes.Add((int)AngularRoutesNav.PROMOTIONAPPLICATION);
                    //allowedRoutes.Add((int)AngularRoutesNav.PPSCCANDIDATES);
                }
                else if (roleName.Equals("South Punjab"))
                {
                    allowedRoutes.Add((int)AngularRoutesNav.DASHBAORD);
                    if (userName.Equals("rr.south"))
                    {
                        allowedRoutes.Add((int)AngularRoutesNav.DASHBAORD);
                        allowedRoutes.Add((int)AngularRoutesNav.SCANNEDFILES);
                    }
                    else
                    {
                        allowedRoutes.Add((int)AngularRoutesNav.HEALTHFACILITY);
                        allowedRoutes.Add((int)AngularRoutesNav.PROFILE);      // uncommented by Adnan on 06/10/2022 on order of Ahmer sb/Khalid sb
                        allowedRoutes.Add((int)AngularRoutesNav.VACANCY);
                        if (userName.Equals("south.ordercell"))
                        {
                            allowedRoutes.Add((int)AngularRoutesNav.ORDER);
                        }
                        allowedRoutes.Add((int)AngularRoutesNav.MYACCOUNT);

                    }

                }
                else if (userName.Equals("shcmed"))
                {
                    allowedRoutes.Add((int)AngularRoutesNav.PROMOTIONAPPLICATION);
                    //allowedRoutes.Add((int)AngularRoutesNav.PPSCCANDIDATES);
                }
                else if (userName.StartsWith("crr.") || roleName.Equals("ACR Room"))
                {
                    allowedRoutes.Add((int)AngularRoutesNav.DASHBAORD);
                    allowedRoutes.Add((int)AngularRoutesNav.FILE);
                    allowedRoutes.Add((int)AngularRoutesNav.SCANNEDFILES);
                    allowedRoutes.Add((int)AngularRoutesNav.INBOX);
                    allowedRoutes.Add((int)AngularRoutesNav.MYAPPLICATIONS);
                    allowedRoutes.Add((int)AngularRoutesNav.SENT);
                    allowedRoutes.Add((int)AngularRoutesNav.SEARCHTRACKING);
                    if (userName.StartsWith("acr"))
                    {
                        allowedRoutes.Add((int)AngularRoutesNav.SENIORITYLIST);
                    }
                }
                else if (roleName.Equals("Employee"))
                {
                    allowedRoutes.Add((int)AngularRoutesNav.DASHBAORD);
                    allowedRoutes.Add((int)AngularRoutesNav.PLACEOFPOSTING);
                    //allowedRoutes.Add((int)AngularRoutesNav.EPROFILE);               // commented by Adnan on 06/10/2022 on order of Ahmer sb/Khalid sb
                    allowedRoutes.Add((int)AngularRoutesNav.APPLICATIONS);
                    allowedRoutes.Add((int)AngularRoutesNav.ISSUEREPORT);
                }
                else if (roleName.Equals("Employee Applicant"))
                {
                    allowedRoutes.Add((int)AngularRoutesNav.DASHBAORD);
                    allowedRoutes.Add((int)AngularRoutesNav.PLACEOFPOSTING);
                    //allowedRoutes.Add((int)AngularRoutesNav.EPROFILE);            // commented by Adnan on 06/10/2022 on order of Ahmer sb/Khalid sb
                    allowedRoutes.Add((int)AngularRoutesNav.ISSUEREPORT);
                }
                else if ((userName.StartsWith("fdo") || userName.StartsWith("pl")) && !roleName.Equals("Facilitation Centre South"))
                {
                    allowedRoutes.Add((int)AngularRoutesNav.DASHBAORD);
                    allowedRoutes.Add((int)AngularRoutesNav.APPLICATION);
                    allowedRoutes.Add((int)AngularRoutesNav.INBOXFC);
                    allowedRoutes.Add((int)AngularRoutesNav.HEALTHFACILITY);
                    //allowedRoutes.Add((int)AngularRoutesNav.PROFILE);             // commented by Adnan on 06/10/2022 on order of Ahmer sb/Khalid sb
                    //allowedRoutes.Add((int)AngularRoutesNav.HEALTHFACILITY);
                    //allowedRoutes.Add((int)AngularRoutesNav.PROFILE);
                    allowedRoutes.Add((int)AngularRoutesNav.MYACCOUNT);
                    allowedRoutes.Add((int)AngularRoutesNav.SEARCHTRACKING);
                    if (userName.StartsWith("fdo"))
                    {
                        allowedRoutes.Add((int)AngularRoutesNav.SCANNEDFILES);
                        allowedRoutes.Add((int)AngularRoutesNav.SMOAPPLICATION);
                        allowedRoutes.Add((int)AngularRoutesNav.PPSCCANDIDATES);
                    }
                }
                else if (roleName.Equals("Facilitation Centre South"))
                {
                    allowedRoutes.Add((int)AngularRoutesNav.DASHBAORD);
                    allowedRoutes.Add((int)AngularRoutesNav.APPLICATION);
                    allowedRoutes.Add((int)AngularRoutesNav.MYACCOUNT);
                    allowedRoutes.Add((int)AngularRoutesNav.SEARCHTRACKING);
                }
                else if (roleName.Equals("Employee"))
                {
                    //allowedRoutes.Add((int)AngularRoutesNav.DASHBAORD);
                    //allowedRoutes.Add((int)AngularRoutesNav.PLACEOFPOSTING);
                    //allowedRoutes.Add((int)AngularRoutesNav.EPROFILE);
                    //allowedRoutes.Add((int)AngularRoutesNav.EFILE);
                    ////allowedRoutes.Add((int)AngularRoutesNav.ATTENDANCE);
                    //allowedRoutes.Add((int)AngularRoutesNav.APPLICATIONS);
                    //allowedRoutes.Add((int)AngularRoutesNav.LEAVERECORD);
                    //allowedRoutes.Add((int)AngularRoutesNav.SERVICERECORD);
                    //allowedRoutes.Add((int)AngularRoutesNav.INQUIRIES);
                    //allowedRoutes.Add((int)AngularRoutesNav.SEARCH);
                }
                else if (userName.Equals("managerfc"))
                {
                    allowedRoutes.Add((int)AngularRoutesNav.DASHBAORD);
                    allowedRoutes.Add((int)AngularRoutesNav.APPLICATION);
                    allowedRoutes.Add((int)AngularRoutesNav.INBOX);
                    allowedRoutes.Add((int)AngularRoutesNav.INBOXFC);
                    allowedRoutes.Add((int)AngularRoutesNav.MYAPPLICATIONS);
                    allowedRoutes.Add((int)AngularRoutesNav.SENT);
                    allowedRoutes.Add((int)AngularRoutesNav.HEALTHFACILITY);
                    //allowedRoutes.Add((int)AngularRoutesNav.PROFILE);          // commented by Adnan on 06/10/2022 on order of Ahmer sb/Khalid sb
                    allowedRoutes.Add((int)AngularRoutesNav.VACANCY);
                    allowedRoutes.Add((int)AngularRoutesNav.SCANNEDFILES);
                    allowedRoutes.Add((int)AngularRoutesNav.SEARCHTRACKING);
                }
                else if (userName.StartsWith("sdp") || roleName.Equals("Senior Data Processor"))
                {
                    allowedRoutes.Add((int)AngularRoutesNav.DASHBAORD);
                    allowedRoutes.Add((int)AngularRoutesNav.HEALTHFACILITY);
                    allowedRoutes.Add((int)AngularRoutesNav.PROFILE);          // commented by Adnan on 06/10/2022 on order of Ahmer sb/Khalid sb
                    allowedRoutes.Add((int)AngularRoutesNav.VACANCY);
                    allowedRoutes.Add((int)AngularRoutesNav.USER);
                    allowedRoutes.Add((int)AngularRoutesNav.MYACCOUNT);
                    allowedRoutes.Add((int)AngularRoutesNav.SEARCHTRACKING);
                    allowedRoutes.Add((int)AngularRoutesNav.ORDER);
                    allowedRoutes.Add((int)AngularRoutesNav.ADHOCAPPLICATIONS);
                    allowedRoutes.Add((int)AngularRoutesNav.SENIORITYLIST);
                    allowedRoutes.Add((int)AngularRoutesNav.DAILYWAGERPROFILE);
                }
                else if (userName.StartsWith("ri.") || roleName.Equals("Online Dairy Cell"))
                {
                    allowedRoutes.Add((int)AngularRoutesNav.DASHBAORD);
                    allowedRoutes.Add((int)AngularRoutesNav.RIBRANCH);
                    allowedRoutes.Add((int)AngularRoutesNav.MYACCOUNT);
                    allowedRoutes.Add((int)AngularRoutesNav.INBOX);
                    allowedRoutes.Add((int)AngularRoutesNav.MYAPPLICATIONS);
                    allowedRoutes.Add((int)AngularRoutesNav.SEARCHTRACKING);
                }
                else if (roleName.Equals("Citizen Portal"))
                {
                    allowedRoutes.Add((int)AngularRoutesNav.DASHBAORD);
                    allowedRoutes.Add((int)AngularRoutesNav.CITIZENPORTAL);
                    allowedRoutes.Add((int)AngularRoutesNav.MYACCOUNT);
                    allowedRoutes.Add((int)AngularRoutesNav.SEARCHTRACKING);
                }
                else if (roleName.Equals("Primary") || roleName.Equals("Secondary") || roleName.Equals("Office Institutes"))
                {
                    allowedRoutes.Add((int)AngularRoutesNav.DASHBAORD);
                    allowedRoutes.Add((int)AngularRoutesNav.HEALTHFACILITY);
                    //allowedRoutes.Add((int)AngularRoutesNav.PROFILE);         // commented by Adnan on 06/10/2022 on order of Ahmer sb/Khalid sb
                    allowedRoutes.Add((int)AngularRoutesNav.VACANCY);
                    allowedRoutes.Add((int)AngularRoutesNav.MYACCOUNT);
                }
                else if (roleName.Equals("District Computer Operator"))
                {
                    allowedRoutes.Add((int)AngularRoutesNav.DASHBAORD);
                    allowedRoutes.Add((int)AngularRoutesNav.HEALTHFACILITY);
                    allowedRoutes.Add((int)AngularRoutesNav.PROFILE);         // commented by Adnan on 06/10/2022 on order of Ahmer sb/Khalid sb
                    allowedRoutes.Add((int)AngularRoutesNav.VACANCY);
                    allowedRoutes.Add((int)AngularRoutesNav.MYACCOUNT);
                    allowedRoutes.Add((int)AngularRoutesNav.DAILYWAGERPROFILE);

                }
                else if (roleName.Equals("Health Facility"))
                {
                    
                    allowedRoutes.Add((int)AngularRoutesNav.HEALTHFACILITY);
                    allowedRoutes.Add((int)AngularRoutesNav.PROFILE);         // commented by Adnan on 06/10/2022 on order of Ahmer sb/Khalid sb
                    allowedRoutes.Add((int)AngularRoutesNav.VACANCY);
                    allowedRoutes.Add((int)AngularRoutesNav.DAILYWAGERPROFILE);
                    allowedRoutes.Add((int)AngularRoutesNav.MYACCOUNT);
                }
                else if (userName.StartsWith("ceo.") || roleName.Equals("Chief Executive Officer"  ) || roleName.Equals("Districts"))
                {
                    allowedRoutes.Add((int)AngularRoutesNav.DASHBAORD);
                    allowedRoutes.Add((int)AngularRoutesNav.HEALTHFACILITY);
                    allowedRoutes.Add((int)AngularRoutesNav.PROFILE);          // commented by Adnan on 06/10/2022 on order of Ahmer sb/Khalid sb
                    allowedRoutes.Add((int)AngularRoutesNav.VACANCY);
                    allowedRoutes.Add((int)AngularRoutesNav.ORDER);
                    //allowedRoutes.Add((int)AngularRoutesNav.CEOAPPLICATION);
                    allowedRoutes.Add((int)AngularRoutesNav.MYACCOUNT);
                    allowedRoutes.Add((int)AngularRoutesNav.DAILYWAGERPROFILE);

                }
                else if (userName.Equals("ordercell") || userName.Equals("og1") || roleName.StartsWith("Hisdu Order Team") || roleName.StartsWith("Order Generation"))
                {
                    allowedRoutes.Add((int)AngularRoutesNav.DASHBAORD);
                    allowedRoutes.Add((int)AngularRoutesNav.INBOX);
                    allowedRoutes.Add((int)AngularRoutesNav.MYAPPLICATIONS);
                    allowedRoutes.Add((int)AngularRoutesNav.SENT);
                    allowedRoutes.Add((int)AngularRoutesNav.ORDER);
                    allowedRoutes.Add((int)AngularRoutesNav.SEARCHTRACKING);
                    allowedRoutes.Add((int)AngularRoutesNav.HEALTHFACILITY);
                    allowedRoutes.Add((int)AngularRoutesNav.FILEREQUISITION);
                    allowedRoutes.Add((int)AngularRoutesNav.PROFILE);         // uncommented by Adnan on 06/10/2022 on order of Ahmer sb/Khalid sb
                    allowedRoutes.Add((int)AngularRoutesNav.MYACCOUNT);
                }
                else if (roleName.Equals("DG Health"))
                {
                    allowedRoutes.Add((int)AngularRoutesNav.DASHBAORD);
                    allowedRoutes.Add((int)AngularRoutesNav.ORDER);
                    allowedRoutes.Add((int)AngularRoutesNav.SEARCHTRACKING);
                    allowedRoutes.Add((int)AngularRoutesNav.HEALTHFACILITY);
                    //allowedRoutes.Add((int)AngularRoutesNav.PROFILE);           // commented by Adnan on 06/10/2022 on order of Ahmer sb/Khalid sb
                    allowedRoutes.Add((int)AngularRoutesNav.MYACCOUNT);
                }
                else if (roleName.Equals("HRView"))
                {
                    allowedRoutes.Add((int)AngularRoutesNav.HEALTHFACILITY);
                    //allowedRoutes.Add((int)AngularRoutesNav.PROFILE);            // commented by Adnan on 06/10/2022 on order of Ahmer sb/Khalid sb
                }
                else if (roleName.Equals("AdhocScrutiny"))
                {
                    allowedRoutes.Add((int)AngularRoutesNav.DASHBAORD);
                    allowedRoutes.Add((int)AngularRoutesNav.ADHOCAPPLICATIONS);
                    allowedRoutes.Add((int)AngularRoutesNav.ADHOCGRIEVANCE);
                    allowedRoutes.Add((int)AngularRoutesNav.INTERVIEWS);
                    //allowedRoutes.Add((int)AngularRoutesNav.SCRUTINY); 
                    //allowedRoutes.Add((int)AngularRoutesNav.MERITLIST);
                    //allowedRoutes.Add((int)AngularRoutesNav.ATTENDANCE);
                    //allowedRoutes.Add((int)AngularRoutesNav.INTERVIEWS);
                }
                else if (roleName.Equals("Attendance"))
                {
                    allowedRoutes.Add((int)AngularRoutesNav.DASHBAORD);
                    allowedRoutes.Add((int)AngularRoutesNav.HISDUEMPLOYEES);
                    allowedRoutes.Add((int)AngularRoutesNav.MONTHLYLEAVE);
                    allowedRoutes.Add((int)AngularRoutesNav.LEAVESDETAIL);
                    allowedRoutes.Add((int)AngularRoutesNav.DAILYATTENDANCE);
                    allowedRoutes.Add((int)AngularRoutesNav.ATTENDANCEREPORT);
                    allowedRoutes.Add((int)AngularRoutesNav.MYACCOUNT);
                }
                else if (roleName.Equals("PACP"))
                {
                    allowedRoutes.Add((int)AngularRoutesNav.DASHBAORD);
                    allowedRoutes.Add((int)AngularRoutesNav.HEALTHFACILITY);
                    //allowedRoutes.Add((int)AngularRoutesNav.PROFILE);      // commented by Adnan on 06/10/2022 on order of Ahmer sb/Khalid sb
                    allowedRoutes.Add((int)AngularRoutesNav.MYACCOUNT);
                }
                else if (roleName.Equals("Office Diary")  )
                {
                    allowedRoutes.Add((int)AngularRoutesNav.DASHBAORD);
                    allowedRoutes.Add((int)AngularRoutesNav.SEARCHTRACKING);

                    allowedRoutes.Add((int)AngularRoutesNav.INBOX);
                    allowedRoutes.Add((int)AngularRoutesNav.MYAPPLICATIONS);
                    allowedRoutes.Add((int)AngularRoutesNav.SENT);
                }
                else if (roleName.Equals("Administrative Office"))
                {
                    allowedRoutes.Add((int)AngularRoutesNav.DASHBAORD);
                    allowedRoutes.Add((int)AngularRoutesNav.SEARCHTRACKING);

                    allowedRoutes.Add((int)AngularRoutesNav.INBOX);
                    allowedRoutes.Add((int)AngularRoutesNav.MYAPPLICATIONS);
                    allowedRoutes.Add((int)AngularRoutesNav.SENT);
                    allowedRoutes.Add((int)AngularRoutesNav.DAILYWAGERPROFILE);

                }
                else if (roleName.StartsWith("Section Officer") || roleName.StartsWith("Deputy Secretary")
                    || roleName.Equals("Law wing"))
                {
                    allowedRoutes.Add((int)AngularRoutesNav.DASHBAORD);
                    //allowedRoutes.Add((int)AngularRoutesNav.DIARY);
                    allowedRoutes.Add((int)AngularRoutesNav.INBOX);
                    allowedRoutes.Add((int)AngularRoutesNav.MYAPPLICATIONS);
                    allowedRoutes.Add((int)AngularRoutesNav.SENT);
                    allowedRoutes.Add((int)AngularRoutesNav.SEARCHTRACKING);
                    //allowedRoutes.Add((int)AngularRoutesNav.RETIREMENTEMPLOYEE); 
                    //allowedRoutes.Add((int)AngularRoutesNav.RETIREMENTEMPLOYEE);
                    //allowedRoutes.Add((int)AngularRoutesNav.IMPORTANT);
                    allowedRoutes.Add((int)AngularRoutesNav.FILEREQUISITION);
                    //allowedRoutes.Add((int)AngularRoutesNav.SUMMARIES);
                    //allowedRoutes.Add((int)AngularRoutesNav.FILEREQUEST);
                    //allowedRoutes.Add((int)AngularRoutesNav.SENDRECIEVEFILES);
                    //allowedRoutes.Add((int)AngularRoutesNav.NEWFILEREQUEST);
                    if (userName.Equals("ds.admin") || userName.Equals("pshd") || userName.Equals("ds.general") || userName.Equals("dse") || userName.Equals("a.system") || userName.Equals("so.specialist") || userName.Equals("ds.staff"))
                    {
                        allowedRoutes.Add((int)AngularRoutesNav.HEALTHFACILITY);
                        //allowedRoutes.Add((int)AngularRoutesNav.PROFILE);          // commented by Adnan on 06/10/2022 on order of Ahmer sb/Khalid sb
                        allowedRoutes.Add((int)AngularRoutesNav.VACANCY);
                    }
                    if (userName.Equals("pshd"))
                    {
                        allowedRoutes.Add((int)AngularRoutesNav.ORDER);
                    }
                    if (userName.Equals("sodhas"))
                    {
                        allowedRoutes.Add((int)AngularRoutesNav.HEALTHFACILITY);
                        //allowedRoutes.Add((int)AngularRoutesNav.PROFILE);          // commented by Adnan on 06/10/2022 on order of Ahmer sb/Khalid sb
                    }
                    if (userName.Equals("so.inq"))
                    {
                        //allowedRoutes.Add((int)AngularRoutesNav.PROFILE);
                        allowedRoutes.Add((int)AngularRoutesNav.INQUIRYFILES);
                    }
                    if (userName.StartsWith("so.conf"))
                    {
                        allowedRoutes.Add((int)AngularRoutesNav.SCANNEDFILES);
                    }
                    if (userName.StartsWith("so.conf2"))
                    {
                        allowedRoutes.Add((int)AngularRoutesNav.PROMOTIONAPPLICATION);
                    }
                    if (userName.Equals("so.pension"))
                    {
                        allowedRoutes.Add((int)AngularRoutesNav.RETIREMENTEMPLOYEE);
                    }
                    if (userName.Equals("so.promotion"))
                    {
                        allowedRoutes.Add((int)AngularRoutesNav.SENIORITYLIST);
                        allowedRoutes.Add((int)AngularRoutesNav.SYNOPSIS);
                        allowedRoutes.Add((int)AngularRoutesNav.QUANTIFICATION);
                        allowedRoutes.Add((int)AngularRoutesNav.WORKINGPAPER);
                        allowedRoutes.Add((int)AngularRoutesNav.INQUIRY);
                    }
                    if (userName.Equals("a.system"))
                    {
                        allowedRoutes.Add((int)AngularRoutesNav.ORDERUPLOAD);
                    }
                    if (roleName.Equals("Law wing"))
                    {

                        if (userName.Equals("slo"))
                        {
                            allowedRoutes.Add((int)AngularRoutesNav.RIBRANCH);
                        }

                        allowedRoutes.Add((int)AngularRoutesNav.DAILYCALLSLIST);
                        allowedRoutes.Add((int)AngularRoutesNav.LAWFILES);
                    }
                    //if (userName.Contains("wmo") || userName.Equals("hrmanager"))
                    //{
                    //    allowedRoutes.Add((int)AngularRoutesNav.PPSCCANDIDATES);
                    //}
                }
                nav = _db.C_ErpModule.Where(x => allowedRoutes.Contains(x.Id)).ToList();
                return nav;
            }
        }
    }
    enum AngularRoutesNav
    {
        DASHBAORD = 1,
        DATABASE = 2,
        HEALTHFACILITY = 3,
        PROFILE = 4,
        VACANCY = 5,
        APPLICATION = 6,
        ORDER = 7,
        REPORTING = 8,
        INBOX = 9,
        USER = 10,
        MYAPPLICATIONS = 11,
        FILE = 12,
        IMPORTANT = 13,
        SENT = 14,
        SCANNEDFILES = 15,
        FILEREQUISITION = 16,
        MYACCOUNT = 18,
        FILEREQUEST = 20,
        SENDRECIEVEFILES = 20,
        NEWFILEREQUEST = 21,
        DIARY = 22,
        RIBRANCH = 23,
        SEARCHTRACKING = 24,
        INBOXFC = 25,
        VACANCYSTATUS = 26,
        ORDERUPLOAD = 27,
        PPSCCANDIDATES = 28,
        SMOAPPLICATION = 29,
        PPSCPROFILE = 31,
        //PLACEOFPOSTING = 30,
        //EPROFILE = 31,
        //APPLICATIONS = 32,
        //EFILE = 33,
        //ATTENDANCE = 34,
        //LEAVERECORD = 35,
        //SERVICERECORD = 36,
        //INQUIRIES = 37,
        //SEARCH = 38,
        NEWPROFILE = 30,
        PLACEOFPOSTING = 32,
        EPROFILE = 33,
        APPLICATIONS = 34,
        ISSUEREPORT = 35,
        //PROFILEREVIEW = 36,
        CITIZENPORTAL = 36,
        DAILYATTENDANCE = 39,
        MONTHLYLEAVE = 42,
        LEAVESDETAIL = 41,
        HISDUEMPLOYEES = 38,
        ATTENDANCEREPORT = 40,
        CEOAPPLICATION = 43,
        JOBS = 44,
        JOBAPPLICATION = 45,
        PROMOTIONAPPLICATION = 46,
        LAWFILES = 47,
        RETIREMENTEMPLOYEE = 48,
        DAILYCALLSLIST = 49,
        INQUIRYFILES = 50,
        ORDERREQUEST = 52,
        ADHOCUSER = 57,
        ADHOCAPPLICATIONS = 58,
        SCRUTINY = 59,
        MERITLIST = 61,
        ATTENDANCE = 62,
        INTERVIEWS = 63,
        ADHOCGRIEVANCE = 64,
        SUMMARIES = 66,
        SENIORITYLIST = 67,
        SYNOPSIS = 68,
        QUANTIFICATION = 69,
        WORKINGPAPER = 70,
        INQUIRY = 71,
        NEWDOCUMENT = 76,
        DAILYWAGERPROFILE = 76
    }
}
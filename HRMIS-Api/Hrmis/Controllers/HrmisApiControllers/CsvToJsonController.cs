using CsvHelper;
using Hrmis.Models.Common;
using Hrmis.Models.Dto;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Hrmis.Controllers.HrmisApiControllers
{
    [RoutePrefix("api/Parse")]
    public class CsvToJsonController : ApiController
    {

        [Route("HealthFacility/{tehsil}")]
        public IHttpActionResult HealthFacility(string tehsil)
        {
            try
            {
                string path = HttpContext.Current.Server.MapPath("~/Areas/Hrmis/Uploads/Csv/T-" + tehsil + ".csv");
                List<HealthFacilityDto> healthFacilityDtos;
                using (var csv = new CsvReader(new StreamReader(path)))
                {
                    csv.Configuration.RegisterClassMap<HealthFacilityMap>();
                    healthFacilityDtos = csv.GetRecords<HealthFacilityDto>().ToList();
                }
                return Ok(healthFacilityDtos);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        [Route("CDR")]
        [HttpGet]
        public IHttpActionResult CDR()
        {
            try
            {
                string path = HttpContext.Current.Server.MapPath(@"~\Uploads\Csv\th.csv");
                List<CDRDto> cdrs;
                using (var csv = new CsvReader(new StreamReader(path)))
                {
                    csv.Configuration.RegisterClassMap<CDRMap>();
                    cdrs = csv.GetRecords<CDRDto>().ToList();
                }
                return Ok(cdrs);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        [Route("Vaccine")]
        [HttpGet]
        public IHttpActionResult Vaccine()
        {
            try
            {
                string path = HttpContext.Current.Server.MapPath(@"~\Uploads\Csv\BahawalnagarTest.csv");
                List<VaccineDTO> vaccineData;
                string template = @"معزز شہری!آپ کی سہولت کے پیشِ نظر آپ کو  مطلع کیا جاتا ہے کہ آپ کا ویکسین لگوانے کا سنٹر تبدیل کر کے";
                string template2 = @" کر دیا گیا ہے۔لہذا آپ اپنے تبدیل شدہ سنٹر پر صبح 8بجے سے رات 8 بجے کے دوران ویکسین لگوا سکتے ہیں۔مزید رہنمائی کے لیے محکمہ صحت حکومتِ پنجاب کے سہو لت نمبر 1033 پر بھی رابطہ کیا جا سکتا ہے۔";
                string centre = @"گورنمنٹ رائس ریسرچ اینڈ ایگرکلچر سنٹر بہاولنگر";
                using (var csv = new CsvReader(new StreamReader(path)))
                {
                    csv.Configuration.RegisterClassMap<VaccineMap>();
                    vaccineData = csv.GetRecords<VaccineDTO>().ToList();
                    string text = template + centre + template2;

                    Debug.WriteLine(text);

                    foreach (var item in vaccineData)
                    {

                        if (!string.IsNullOrEmpty(item.MobileNo))
                        {
                            if (item.MobileNo.StartsWith("+92"))
                            {
                                item.MobileNo = item.MobileNo.Replace("+92", "0");
                            }
                        }

                        SMS sms = new SMS()
                        {
                            UserId = "123",
                            FKId = 0,
                            MobileNumber = item.MobileNo,
                            Message = text
                        };
                        //var jobId = BackgroundJob.Enqueue(() => Common.Common.SendSMSTelenor(sms));
                        //Thread t = new Thread(() => Common.Common.SendSMSTelenor(sms));
                        //t.Start();
                        //await Common.Common.SendSMSTelenor(sms);
                        Common.SendSMSTelenor(sms);
                    }

                }
                return Ok(vaccineData);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}

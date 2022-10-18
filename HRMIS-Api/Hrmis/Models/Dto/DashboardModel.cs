using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hrmis.Models.Dto
{
    public class DashboardModel
    {
        public string Name { get; set; }
        public int DHQ { get; set; }
        public int THQ { get; set; }
        public int RHC { get; set; }
        public int BHU { get; set; }
        public int Disp { get; set; }
        public int sph { get; set; }
        public int mch { get; set; }
        public int trauma { get; set; }
        public int mobDisp { get; set; }
        public int townHsp { get; set; }
        public int eyeHsp { get; set; }
        public int tbClinic { get; set; }
        public int tbinstitute { get; set; }
        public int tbFlysq { get; set; }
        public int tbPolhsp { get; set; }
        public int tbCivilhsp { get; set; }
        public int tbFilterC { get; set; }
        public int tbDHDC { get; set; }
        public int tbMCEU { get; set; }
        public int tbSchOP { get; set; }
        public int tbMH { get; set; }
        public int tbIDH { get; set; }
        public int tbDentalC { get; set; }
        public int tbCMC { get; set; }
        public int tbSHC { get; set; }
        public int PSHCD { get; set; }
        public int DGHS { get; set; }
        public int VP { get; set; }
        public int PU { get; set; }
        public int AdminOfT { get; set; }
        public int AdminOfD { get; set; }
        public int AdminOfDist { get; set; }

        public int ADceo { get; set; }
        public int ADdohp { get; set; }
        public int ADdohms { get; set; }
        public int ADdohhrmis { get; set; }
        public int ADdcirmnch { get; set; }
        public int ADdmphfmc { get; set; }


        //public int totalmch { get; set; }
        //public int totaltrauma { get; set; }
        //public int totalmobDisp { get; set; }
        //public int totaltownHsp { get; set; }
        //public int totaleyeHsp { get; set; }
        //public int totaltbClinic { get; set; }
        //public int totaltbinstitute { get; set; }
        //public int totaltbFlysq { get; set; }
        //public int totaltbPolhsp { get; set; }
        //public int totaltbCivilhsp { get; set; }
        //public int totaltbFilterC { get; set; }
        //public int totaltbDHDC { get; set; }
        //public int totaltbMCEU { get; set; }
        //public int totaltbSchOP { get; set; }
        //public int totaltbMH { get; set; }
        //public int totaltbIDH { get; set; }
        //public int totaltbDentalC { get; set; }
        //public int totaltbCMC { get; set; }
        //public int totaltbSHC { get; set; }
        //public int totalPSHCD { get; set; }
        //public int totalDGHS { get; set; }
        //public int totalVP { get; set; }
        //public int totalPU { get; set; }
        //public int totalAdminOfT = 0;
        //public int totalAdminOfD = 0;
        //public int totalAdminOfDist = 0;


    }
    public class DashboardStatffStatus
    {
        public string Name { get; set; }
        public int Profiles { get; set; }
        public int Santctioned { get; set; }
        public int Filled { get; set; }
        public int vacant { get; set; }
    }
}
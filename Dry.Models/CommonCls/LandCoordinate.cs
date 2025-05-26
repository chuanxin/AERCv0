using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using Dry.Models.Service;

namespace Dry.Models.CommonCls
{
    public class LandCoordinate
    {
        
        /// <summary>
        /// 取得地號座標(從webservice)
        /// </summary>
        /// <param name="SectionID">地段代碼</param>
        /// <param name="LandNo">地號</param>
        /// <returns></returns>
        public string GetLandNoLatlng(int SectionID, string LandNo)
        {
            //2015/12/3 by wei
            //this url is made from Mary
            //http://gis5.aerc.org.tw/aerclocationbackend17/getdata.ashx?method=getLandPoint2&section=AA0001&landno=1
            //paramter:section, landno
            //example: 
            //{
            //  landPointXY: [
            //      {
            //          landNo: "1",
            //          x: "121.5153419247",
            //          y: "25.0274665274323"
            //      }
            //  ]
            //}
            DryEntities DryDB = new DryEntities();
            string[] LandCodeSplit = LandNo.Split('-');
            if (LandCodeSplit.Count() == 2)
            {
                int param1 = Convert.ToInt32(LandCodeSplit[0]);
                int param2 = Convert.ToInt32(LandCodeSplit[1]);
                LandNo = param1.ToString() + "-" + param2.ToString();
                if (param2 == 0)
                {
                    LandNo = param1.ToString();
                }
            }
            else
            {
                LandNo = Convert.ToInt32(LandCodeSplit[0]).ToString();
            }
            string landCode = DryDB.LandData.Where(m => m.Section_Id == SectionID).FirstOrDefault().LandCode;

            string result = "";
            string param = "section=" + landCode + "&landno=" + LandNo;
            HttpWebRequest request = HttpWebRequest.Create("http://gis5.aerc.org.tw/aerclocationbackend17/getdata.ashx?method=getLandPoint2&" + param) as HttpWebRequest;
            request.Method = "GET";
            request.KeepAlive = true;
            request.ContentType = "application/x-www-form-urlencoded";

            using (WebResponse response = request.GetResponse())
            {
                StreamReader sr = new StreamReader(response.GetResponseStream());
                result = sr.ReadToEnd();
                sr.Close();
            }
            return result;
        }
        
        /// <summary>
        /// SOA取得地段地號中心座標位置
        /// </summary>
        /// <param name="SectionID"></param>
        /// <param name="LandNo"></param>
        /// <returns></returns>
        public string GetLandNoLatlngByMoiWeb(int SectionID, string LandNo)
        {
            DryEntities DryDB = new DryEntities();
            string[] LandCodeSplit = LandNo.Split('-');
            if (LandCodeSplit.Count() == 2)
            {
                int param1 = Convert.ToInt32(LandCodeSplit[0]);
                int param2 = Convert.ToInt32(LandCodeSplit[1]);
                LandNo = param1.ToString() + "-" + param2.ToString();
                if (param2 == 0)
                {
                    LandNo = param1.ToString();
                }
            }
            else
            {
                LandNo = Convert.ToInt32(LandCodeSplit[0]).ToString();
            }
            LandData landData = DryDB.LandData.Where(m => m.Section_Id == SectionID).FirstOrDefault();
            string sectionCode = landData.LandCode.ToString();
            string landOfficeCode = landData.Land_Office_Code;
            string result = "";

            string data = new dryapi.gisapi().getLand(sectionCode, LandNo);
            var point = JsonConvert.DeserializeObject<viewLandNo>(data, new JsonSerializerSettings() { Error = (sender, args) => args.ErrorContext.Handled = true });
            result = String.Format("\"X\":{0},\"Y\":{1}", point.longitude, point.latitude);
            result = "{" + result + "}";
            return result;
        }
        /// <summary>
        /// SOA地用座標取得GRP資訊
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="coortype"></param>
        /// <returns></returns>
        public GRPinfo GetGRPgByXYWeb(decimal x, decimal y, int coortype)
        {
            string data = new dryapi.gisapi().getGRP(x, y, coortype);
            var point = JsonConvert.DeserializeObject<GRPinfo>(data, new JsonSerializerSettings() { Error = (sender, args) => args.ErrorContext.Handled = true });
            
            return point;
        }
      
        public class viewLandNo
        {
            public string SectionCode { get; set; }
            public string LandNo { get; set; }
            public decimal X97 { get; set; }
            public decimal Y97 { get; set; }
            public decimal Xmin { get; set; }
            public decimal Ymin { get; set; }
            public decimal Xmax { get; set; }
            public decimal Ymax { get; set; }
            public decimal latitude { get; set; }
            public decimal longitude { get; set; }

        }
        public class GRPinfo
        {
            public string Ia { get; set; }
            public string IaCNS { get; set; }
            public string Mng { get; set; }
            public string MngCNS { get; set; }
            public string Stn { get; set; }
            public string StnCNS { get; set; }
            public string Grp { get; set; }
            public string GrpCNS { get; set; }
        }

        public class CoorPoint
        {
            public string X { get; set; }
            public string Y { get; set; }
        }
    }
}

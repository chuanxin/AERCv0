using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AERC.Models.CommonCls;

namespace Dry.Models.OldSystemData
{
    
    public class Old_ReapplyQuery 
    {
        DryEntities DryDB = new DryEntities();
        public List<OldSystemFarm> QueryOldFarm(int sectid, string Landno)
        {
            //FarmReapplyQuery landData = DryDB.FarmReapplyQuery.Find(sectid);
            FarmReapplyQuery landData = DryDB.FarmReapplyQuery.FirstOrDefault(m=>m.Section_Id ==sectid);

            List<string> LandNoAry = SpileLandNo(Landno);
            List<OldSystemFarm> FarmData = new List<OldSystemFarm>();
            //string Section_Code = landData.Section_Code.Value.ToString("0000");
            string LandNo1 = LandNoAry[0], LandNo2 = LandNoAry[1];
            List<string> yearsArray = new List<string>();
            for (int i = 0; i < 10; i++)
            {
                yearsArray.Add((DateTime.Now.Year - 1911 - i).ToString());
            }
            //FarmData =DryDB.FarmReapplyQuery.Where(m=>m.secno == landData.TownCode + Section_Code)
            FarmData = DryDB.OldSystemFarm.Where(m =>
                m.SECNO == landData.secno &&
                    //m.CITY == landData.TownCode &&
                    //m.SEC == Section_Code &&
                    //yearsArray.Contains(m.年度) && //查詢10年內土地
                m.地號1 == LandNo1 &&
                m.地號2 == LandNo2).OrderBy(m => m.年度).ThenBy(m => m.編號1) .ToList();
            //LandNo();
            return FarmData;
        }
        private List<string> SpileLandNo(string Landno)
        {
            List<string> SpiltedData = new List<string>();
            string[] Spiled = Landno.Split('-');
            SpiltedData.Add(Spiled[0].PadLeft(4, '0'));
            if (Spiled.Length > 1)
            {
                SpiltedData.Add(Spiled[1].PadLeft(4, '0'));
            }
            else
            {
                SpiltedData.Add("0000");
            }
            
            return SpiltedData;
        }
        private void LandNo()
        {
            List<Old_Farm> farm = DryDB.Old_Farm.Where(m => m.地號1.Length < 4 && m.地號2.Length < 4).Take(100).ToList();
            for (int s = 0; s < farm.Count; s++)
            {
                int aa = s;
                Old_Farm data = farm[s];
                List<string> land = SpileLandNo(data.地號1 + "-" + data.地號2);
                data.地號1 = land[0];
                data.地號2 = land[1];
                try
                {
                    DryDB.Old_Farm.Attach(data);
                    DryDB.Entry(data).State = System.Data.EntityState.Modified;
                    DryDB.SaveChanges();
                }
                catch (Exception e)
                {

                }
            }
        }
    }
}

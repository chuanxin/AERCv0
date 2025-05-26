/*
-- =============================================
-- Author: HaoHsuan
-- Create date: 2014-4-7
-- Description: 取得資料的資料庫操作類別
-- =============================================
*/
//
//                       _oo0oo_
//                      o8888888o
//                      88" . "88
//                      (| -_- |)
//                      0\  =  /0
//                    ___/`---'\___
//                  .' \\|     |// '.
//                 / \\|||  :  |||// \
//                / _||||| -:- |||||- \
//               |   | \\\  -  /// |   |
//               | \_|  ''\---/''  |_/ |
//               \  .-\__  '-'  ___/-. /
//             ___'. .'  /--.--\  `. .'___
//          ."" '<  `.___\_<|>_/___.' >' "".
//         | | :  `- \`.;`\ _ /`;.`/ - ` : | |
//         \  \ `_.   \_ __\ /__ _/   .-` /  /
//     =====`-.____`.___ \_____/___.-`___.-'=====
//                       `=---='
//
//
//     ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//
//               佛祖保佑         永無BUG
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dry.Models.Service;
using AERC.Models;

namespace Dry.Models.CommonCls
{
    public class GetData
    {
        private DryEntities DryDB = new DryEntities();
        private CommonEntities CommDB = new CommonEntities();

        #region 取得申請案件資料
        /// <summary>
        /// 取得申請案件資料
        /// </summary>
        /// <param name="MapNo">版本編號</param>
        /// <returns></returns>
        public Case GetCaseDataFromMapNo(int MapNo)
        {
            //VerMapping VM = GetVerMappingData(MapNo);
            //return VM == null ? null : DryDB.Case.Find(VM.EventNo);
            return GetVerMappingData(MapNo).Case;
        }
        #endregion

        #region 取得特定版本之農地列表
        /// <summary>
        /// 取得特定版本之農地列表
        /// </summary>
        /// <param name="mno">版本編號</param>
        /// <returns>特定版本之農地列表</returns>
        public List<NewFarm> GetFarmData(int mno)
        {
            List<NewFarm> NewFarmList = new List<NewFarm>();
            if (DryDB.Farm.Any(f => f.MapNo == mno))
            {
                List<Farm> FarmList = DryDB.Farm.Where(f => f.MapNo == mno).OrderBy(f => f.LandNo).ToList();
                foreach (var itm in FarmList)
                {
                    NewFarmList.Add(new NewFarm
                    {
                        BuildArea = itm.BuildArea,
                        FarmArea = itm.FarmArea,
                        FarmCrop = itm.FarmCrop,
                        FNo = itm.FNo,
                        LandEquity = itm.LandEquity,
                        LandNo = itm.LandNo,
                        MapNo = itm.MapNo,
                        Lat = itm.Lat,
                        Long = itm.Long,
                        LandType = itm.LandType,
                        Section = itm.Section,
                        Outside = itm.Outside,
                        VerMapping = itm.VerMapping,
                        sectName = GetSecName(itm.Section),
                        full_sectName = GetFullSecName(itm.Section)
                    });
                }
                
                //return DryDB.Farm.Where(f => f.MapNo == mno).OrderBy(f => f.LandNo).ToList();
            }
            return NewFarmList;
        }
        public List<Farm> GetFarmDataByMNo(int MNo)
        {
            return DryDB.Farm.Where(p => p.MapNo == MNo).OrderBy(p => p.LandNo).ToList();
        }
        #region New Farm
        public class NewFarm : Farm
        {
            public string sectName { get; set; }
            public string full_sectName { get; set; }
        }
        #endregion
        #endregion

        #region 取得完整地段字串(縣市鄉鎮-地段)
        public string GetFullSecName(int sect)
        {
            LandData data = DryDB.LandData.SingleOrDefault(m => m.Section_Id == sect);
            //Section section = CommDB.Section.Single(s => s.Section_Id == sect);
            string sectname = "空白";
            string townname = "空白";
            string cityname = "空白";
            if (data != null)
            {
                sectname = data.Section + "段";
                townname = data.Town;
                cityname = data.City;
                if (data.Subsection.Length > 0)
                {
                    sectname += data.Subsection + "小段";
                    //townname = data.Town;
                    //cityname = data.City;
                }
            }
            
            //Town town = CommDB.Town.Single(k => k.Town_Id == section.Town_Id);
            
            //City ct = CommDB.City.Single(k => k.City_Code == town.City_Code);
            

            return cityname + townname + "-" + sectname;
        }
        #endregion

        #region 取得鄉鎮id
        public short GetTownIdbyName(string citycode, string townname)
        {
            townname = townname.Substring(0, 2);
            return CommDB.Town.Where(t => t.City_Code == citycode && t.Town1.Contains(townname)).FirstOrDefault().Town_Id;
        }
        #endregion

        #region 取得地段字串
        public string GetSecName(int sect)
        {
            //CommDB = new CommonEntities();
            //Section section = CommDB.Section.Single(s => s.Section_Id == sect);
            //string sectname = section.Section1;
            //if (section.Subsection != null)
            //{
            //    sectname += section.Subsection;
            //}
            LandData data = DryDB.LandData.SingleOrDefault(m => m.Section_Id == sect);
            string sectname;
            if (data != null)
            {

                sectname = data.Section + "段" + (data.Subsection.Length > 0 ? (data.Subsection + "小段") : "");
            }
            else { sectname = "空白"; }
            return sectname;
        }
        #endregion

        #region 由版本編號取得案件編號
        /// <summary>
        /// 由版本編號取得案件編號
        /// </summary>
        /// <param name="mno">版本編號</param>
        /// <returns>案件編號</returns>
        public int GetEventNo(int mno)
        {
            return DryDB.VerMapping.Single(v => v.MapNo == mno).EventNo;
        }
        #endregion

        #region 由版本編號取得案件設計者
        /// <summary>
        /// 由版本編號取得案件設計者
        /// </summary>
        /// <param name="mno">版本編號</param>
        /// <returns>設計者代號</returns>
        public Guid GetDesignID(int mno)
        {
            return DryDB.FacDesign.Single(fd =>fd.MapNo ==  mno).DesId;
        }
        #endregion

        #region 由案件編號取得農戶編號
        /// <summary>
        /// 由案件編號取得農戶編號
        /// </summary>
        /// <param name="EventNo">案件編號</param>
        /// <returns></returns>
        public Guid GetFarmerNo(int EventNo)
        {
            return DryDB.Case.Single(c => c.EventNo == EventNo).FId;
        }
        #endregion

        #region 由版本編號取得農戶資料
        /// <summary>
        /// 由版本編號取得農戶資料
        /// </summary>
        /// <param name="mno">版本編號</param>
        /// <returns>農戶資料</returns>
        public Farmer GetFarmerData(int mno)
        {
            Guid FarmerNo = GetFarmerNo(GetEventNo(mno));
            return DryDB.Farmer.Find(FarmerNo);
        }
        #endregion

        #region 取得動力設備列表
        /// <summary>
        /// 取得動力設備列表
        /// </summary>
        /// <param name="MNo">版本編號</param>
        /// <returns>動力設備列表</returns>
        public List<Engine> GetEngineData(int MNo)
        {
            if (DryDB.Engine.Any(e => e.MapNo == MNo))
                return DryDB.Engine.Where(e => e.MapNo == MNo).ToList();
            else
                return null;
        }
        /// <summary>
        /// 取得動力設備數量(動力)
        /// </summary>
        /// <param name="MNo"></param>
        /// <returns></returns>
        public int GetEngineCountByPower(int MNo)
        {
            List<Engine> result = GetEngineData(MNo);
            if (result == null)
            {
                return 0;
            }
            else
            {
                var count = from eng in result where eng.EngCode == 3 || eng.EngCode == 4 select eng;
                return count.Count();
            }
        }
        /// <summary>
        /// 取得動力設備數量(抽水機)
        /// </summary>
        /// <param name="MNo"></param>
        /// <returns></returns>
        public int GetEngineCountByPump(int MNo)
        {
            List<Engine> result = GetEngineData(MNo);
            if (result == null)
            {
                return 0;
            }
            else
            {
                var count = from eng in result where eng.EngCode == 1 || eng.EngCode == 2 select eng;
                return count.Count();
            }
        }

        public EngApply GetEngApplyData(int MNo)
        {
            return DryDB.EngApply.Find(MNo);
        }

        #endregion

        #region 取得蓄水池資料
        /// <summary>
        /// 取得蓄水池列表
        /// </summary>
        /// <param name="MNo">版本編號</param>
        /// <returns>蓄水池列表</returns>
        public List<Pool> GetPoolData(int MNo)
        {
            if (DryDB.Pool.Any(p => p.MapNo == MNo))
                return DryDB.Pool.Where(p => p.MapNo == MNo).ToList();
            else
                return null;
        }

        public PoolApply GetPoolApplyData(int MNo)
        {
            if (DryDB.PoolApply.Any(p => p.MapNo == MNo))
                return DryDB.PoolApply.Single(ea => ea.MapNo == MNo);
            else
                return null;
        }
        /// <summary>
        /// 取得蓄水池噸數
        /// </summary>
        /// <param name="MNo"></param>
        /// <returns></returns>
        public int GetPoolTon(int MNo)
        {
            List<Pool> data = GetPoolData(MNo);
            if(data!=null)
            {
                return data.Sum(p => p.PoolWeight);
            }
            else
            {
                return 0;
            }
        }
        #endregion

        #region 取得案件版本資訊
        public VerMapping GetVerMappingData(int MNo)
        {
            return DryDB.VerMapping.Find(MNo);
        }
        #endregion

        #region 取得調控設施補助申請資料
        public CntrlApply GetCntrlApplyData(int MapNo)
        {
            return DryDB.CntrlApply.Find(MapNo);
        }
        #endregion

        #region 取得農地施設面積
        /// <summary>
        /// 取得農地施設面積
        /// </summary>
        /// <param name="MapNo">版本編號</param>
        /// <returns></returns>
        public int GetFarmBuildArea(int MapNo)
        {
            if (DryDB.Farm.Any(data => data.MapNo == MapNo))
            {
                return (int)DryDB.Farm.Where(data => data.MapNo == MapNo).Sum(data => data.FinalArea);
            }
            else
            {
                return 0;
            }
        }
        #endregion

        #region 取得田間管路設施補助費用
        /// <summary>
        /// 取得田間管路設施補助費用
        /// [所需經費, 輔助費, 自備款, 工作費]
        /// </summary>
        /// <param name="MapNo">版本編號</param>
        /// <returns>田間管路設施補助費用 [所需經費, 輔助費, 自備款, 工作費]</returns>
        public int[] GetPigingMoney(int MapNo)
        {
            Dry.Models.Service.FarmerSysPriceService Fsys = new Service.FarmerSysPriceService();
            int[] PigingMoney = { 0, 0, 0, 0 };
            if (DryDB.PigingConf.Any(pip => pip.MapNo == MapNo))
            {
                Dry.Models.ViewModel.FarmerSysView.ParaJsonData Data = new ViewModel.FarmerSysView.ParaJsonData();
                Data.EndTypeDataAry = new List<ViewModel.FarmerSysView.EndTypeStruct>();
                PigingConf Conf = DryDB.PigingConf.Single(pip => pip.MapNo == MapNo);
                List<EndType> endType = DryDB.EndType.Where(p => p.MapNo == Conf.MapNo).ToList();
                FarmerSys sys = DryDB.FarmerSys.Single(fsys => fsys.MapNo == MapNo);
                Data.Block = Conf.Cblock;
                Data.FacNo = sys.FacNo;

                foreach(var item in endType)
                {
                    Data.EndTypeDataAry.Add(new Dry.Models.ViewModel.FarmerSysView.EndTypeStruct
                    {
                        Endtype = item.EndTypeCode,
                        Fac = item.FacType,
                        //IrrWCode = item.IrrWCode,
                        SL = item.SL,
                        SS = item.SS,
                        StdpipeHei = item.StdpipeHei == null ? 0 : item.StdpipeHei.Value
                    }
                    );
                }

                Data.Unit = sys.ApplyUnit;

                //主管資料
                List<Dry.Models.ViewModel.FarmerSysView.MainJsonData> minData = new List<ViewModel.FarmerSysView.MainJsonData>();
                if (Conf.L1Price > 0)
                {
                    minData.Add(new Dry.Models.ViewModel.FarmerSysView.MainJsonData
                    {
                        Amount = Conf.L1Amount,
                        Mat = Conf.L1Mat == null ? "" : Conf.L1Mat,
                        LPrice = Conf.L1Price,
                        Length = (float)(Conf.L1)
                    });
                }
                if (Conf.L2Price > 0)
                {
                    minData.Add(new Dry.Models.ViewModel.FarmerSysView.MainJsonData
                    {
                        Amount = (short)Conf.L2Amount,
                        Mat = Conf.L2Mat == null ? "" : Conf.L2Mat,
                        LPrice = Conf.L2Price,
                        Length = (float)(Conf.L2)
                    });
                }
                Data.MainJsonDataAry = minData;

                //物料資料
                List<Dry.Models.ViewModel.FarmerSysView.PriceJsonData> priceData = new List<ViewModel.FarmerSysView.PriceJsonData>();
                foreach (var matitem in DryDB.FarmerSysM.Where(fsm => fsm.FarSysNo == sys.FarSysNo))
                {
                    priceData.Add(new Dry.Models.ViewModel.FarmerSysView.PriceJsonData { POMNo = matitem.POMNo, Amt = matitem.Amount, Price = matitem.SysPrice, TotalPrice = matitem.TotalPrice ?? 0 });
                }
                Data.PriceJsonDataAry = priceData;
                int[] tmpMoney = Fsys.GetHelpTotalPrice(Data, MapNo);
                for (int i = 0; i < tmpMoney.Length; i++)
                {
                    PigingMoney[i] = tmpMoney[i];
                }
                int area = GetFarmBuildArea(MapNo);
                //在所需經費中加入工作費

                Case cse = GetCaseDataFromMapNo(MapNo);
                if (cse.ApplyUnit == 17)//瑠公
                {
                    //PigingMoney[3] = Fsys.GetLiuWorkPrice(area, MapNo, Data.EndTypeDataAry.First().Endtype, Data.EndTypeDataAry.First().Fac);
                    //20240123 alex modify with regualar rule
                    PigingMoney[3] = Fsys.GetWorkPrice(Data, area, MapNo);

                }
                else //農委會
                {
                    PigingMoney[3] = Fsys.GetWorkPrice(Data, area, MapNo);
                }

                /*
                switch (Data.Unit)
                {
                    case 0: //農委會
                        int getpric = Fsys.GetWorkPrice(Data, area);
                        PigingMoney[3] = getpric;
                        break;
                    case 17: //瑠公
                        PigingMoney[3] = Fsys.GetLiuWorkPrice(area);
                        break;
                }*/
            }
            return PigingMoney;
        }
        //public int GetPingMoneyFromPay(int MapNo)
        //{
        //    if (DryDB.Pay.Any(m => m.MapNo == MapNo && m.ItemCode == 1))
        //    {
        //        return DryDB.Pay.Where(m => m.MapNo == MapNo && m.ItemCode == 1).FirstOrDefault().PayMoney;
        //    }
        //    else return 0;
        //}

        /// <summary>
        /// 取得田間管路設施補助費用(由資料庫取得)含工作費
        /// [所需經費, 輔助費, 自備款, 工作費]
        /// </summary>
        /// <param name="MapNo"></param>
        /// <returns>[所需經費, 輔助費, 自備款, 工作費]</returns>
        public int[] GetPigingMoneyByDB(int MapNo)
        {
            int[] result = new int[] { 0, 0, 0 ,0};
            IQueryable<Pay> pay = DryDB.Pay.Where(m => m.MapNo == MapNo && m.ItemCode == 1);
            if (pay.Count() > 0)
            {
                result[0] = pay.Sum(m => m.PayMoney + m.FarmerMoney);
                result[1] = pay.Sum(m => m.PayMoney);
                result[2] = pay.Sum(m => m.FarmerMoney);
                result[3] = DryDB.PigingConf.Find(MapNo).WorkPrice;
            }
            
            
            return result;
        }
        #endregion

        #region 取得田間管路系統資料
        /// <summary>
        /// 取得田間管路系統資料
        /// </summary>
        /// <param name="mno">版本編號</param>
        /// <returns></returns>
        public PigingConf GetPigingConfData(int mno)
        {
            if (DryDB.PigingConf.Any(p => p.MapNo == mno))
                return DryDB.PigingConf.Single(p => p.MapNo == mno);
            else
                return null;
        }
        #endregion
        /// <summary>
        /// 取得末端型式資料
        /// </summary>
        /// <param name="mno"></param>
        /// <returns></returns>
        public List<EndType> GetEndTypeData(int mno)
        {
            return DryDB.EndType.Where(p => p.MapNo == mno).ToList();
        }

        #region 取得農戶設施系統代碼
        /// <summary>
        /// 取得農戶設施系統代碼
        /// </summary>
        /// <param name="MapNo"></param>
        /// <returns></returns>
        public string GetFarmerSysNo(int MapNo)
        {
            //List<FarmerSys> farmerSysNo = DryDB.FarmerSys.Where(p => p.MapNo == MapNo).ToList();
            //if (farmerSysNo.Count > 0)
            //    return farmerSysNo[0].FarSysNo;
            //else
            //    return null;

            if (DryDB.FarmerSys.Any(fs => fs.MapNo == MapNo))
            {
                return DryDB.FarmerSys.Single(fs => fs.MapNo == MapNo).FarSysNo;
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region 取得農戶系統物料列表
        /// <summary>
        /// 取得農戶系統物料列表
        /// </summary>
        /// <param name="mno">版本編號</param>
        /// <returns>農戶系統物料列表</returns>
        public List<FarmerSysM> GetFarmerSysM(int mno)
        {
            if (DryDB.FarmerSys.Any(f => f.MapNo == mno))
            {
                string sysno = GetFarmerSysNo(mno); //DryDB.FarmerSys.Single(f => f.MapNo == mno).FarSysNo;
                if (DryDB.FarmerSysM.Any(fm => fm.FarSysNo == sysno))
                    return DryDB.FarmerSysM.Where(fm => fm.FarSysNo == sysno).ToList();
                else
                    return null;
            }
            else
                return null;
        }
        #endregion

        #region 取得調控物料列表
        /// <summary>
        /// 取得調控物料列表
        /// </summary>
        /// <param name="mno">版本編號</param>
        /// <returns>調控物料列表</returns>
        public List<CntrlMat> GetCntrlMatData(int mno)
        {
            if (DryDB.CntrlFac.Any(c => c.MapNo == mno))
            {
                List<CntrlFac> cfac = DryDB.CntrlFac.Where(c => c.MapNo == mno).ToList();

                List<CntrlMat> CntrMatData = new List<CntrlMat>();
                foreach (var facitem in cfac)
                {
                    CntrMatData.AddRange(DryDB.CntrlMat.Where(c => c.CFNo == facitem.CFNo).OrderBy( c => c.CntrlCode));
                }
                return CntrMatData;
            }
            else
                return null;
        }
        #endregion

        #region 取得調控設施補助費用
        /// <summary>
        /// 計算調控設施所需經費及輔助費用 return [所需經費, 輔助費, 自備款]
        /// </summary>
        /// <param name="_MNo">版本編號</param>
        /// <returns>[所需經費, 輔助費, 自備款]</returns>
        public int[] GetCntrlMoney(int _MNo)
        {
            int[] CntrlMoney = { 0, 0, 0 };
            CtrlPriceService CPSer = new CtrlPriceService();
            List<CntrlMat> MatData = GetCntrlMatData(_MNo);

            if (MatData != null)
            {
                List<Dry.Models.ViewModel.CtrlTableView> ctblview = new List<Dry.Models.ViewModel.CtrlTableView>();
                foreach (var matitem in MatData)
                {
                    ctblview.Add(new Dry.Models.ViewModel.CtrlTableView { CntrlCode = matitem.CntrlCode, MatAmt = matitem.MatAmt, MatNo = matitem.MatNo, MatPrice = matitem.MatPrice, MatPriceAply = matitem.MatPriceAply });
                }

                CntrlMoney = CPSer.GetCtrlHelpTotalPrice(ctblview.ToArray(), _MNo);
            }
            return CntrlMoney;
        }
        #endregion

        #region 取得農戶設施系統資料
        /// <summary>
        /// 取得農戶設施系統資料
        /// </summary>
        /// <param name="MapNo">版本編號</param>
        /// <returns>農戶設施系統資料</returns> 
        public Dry.Models.ViewModel.FarmerSysView.FarmerSysStruct GetFarmerSystemData(int mno)
        {
            if (DryDB.FarmerSys.Any(f => f.MapNo == mno))
            {
                var content = from endtype in DryDB.EndType
                              join endtypelist in DryDB.EndTypeList
                              on endtype.EndTypeCode equals endtypelist.EndType
                              join factype in DryDB.FacTypeList
                              on endtype.FacType equals factype.FacType
                              join farmerSys in DryDB.FarmerSys 
                              on endtype.MapNo equals farmerSys.MapNo
                              //join stdSys in DryDB.StdFacSys
                              //on farmerSys.FacNo equals stdSys.FacNo
                              where farmerSys.MapNo == mno
                              select new
                              {
                                  farmerSys.FarSysNo,
                                  farmerSys.MapNo,
                                  farmerSys.FacNo,
                                  //stdSys.FacTypeName,
                                  farmerSys.FacMoney,
                                  farmerSys.ApplyUnit,
                                  //stdSys.FacName,
                                  //stdSys.EndType,
                                  //stdSys.SubEndType,
                                  //stdSys.FacType,
                                  endtypelist.EndTypeCNS,
                                  endtype.EndTypeCode,
                                  endtype.FacType,
                                  factype.FTpeCNS
                              };
                Dry.Models.ViewModel.FarmerSysView.FarmerSysStruct data = new ViewModel.FarmerSysView.FarmerSysStruct();
                foreach(var item in content)
                {
                    data.FarSysNo = item.FarSysNo;
                    data.MapNo = item.MapNo;
                    data.FacTypeName = item.FTpeCNS;
                    data.FacNo = item.FacNo;
                    data.FacMoney = item.FacMoney;
                    data.ApplyUnit = item.ApplyUnit;
                    data.FacName = item.EndTypeCode == 1 ? item.EndTypeCNS : item.FTpeCNS + item.EndTypeCNS;
                    data.EndType = item.EndTypeCode;
                    //data.SubEndType = item.SubEndType == null ? (byte)0 : item.SubEndType.Value;
                    data.FacType = item.FacType;
                }
                return data;
            }
            else
                return null;
        }
        
        //public List<FarmerSys> GetFarmerSysList(int MapNo)
        //{
        //    return DryDB.FarmerSys.Where(p => p.MapNo == MapNo).ToList();
        //}
        #endregion

        #region 取得動力設施補助費用
        /// <summary>
        /// 取得動力設施補助費用
        /// </summary>
        /// <param name="MapNo">版本編號</param>
        /// <returns>動力設施補助費用</returns>
        public int GetEngineMoney(int MapNo)
        {
            int EngineMoney = 0;
            if (DryDB.Engine.Any(e => e.MapNo == MapNo))
            {
                Calculate_Funding CalculateCls = new Calculate_Funding();

                Case casedata = GetCaseDataFromMapNo(MapNo);

                byte[] EngList = DryDB.Engine.Where(e => e.MapNo == MapNo).Select(en => en.EngCode).ToArray();
                List<EngineDBService.EngineData> engData = new List<EngineDBService.EngineData>();
                foreach (var item in DryDB.Engine.Where(e => e.MapNo == MapNo).ToList())
                {
                    engData.Add(new EngineDBService.EngineData { EngineCode = item.EngCode, EngPrice = item.EngPrice, RegCode = item.EngRegCode });
                }
                EngineMoney = CalculateCls.GetEngMoneybyEngList(engData.ToArray(), casedata).GovPay;
            }
            return EngineMoney;
        }
        #endregion

        #region 取得蓄水設施補助費用
        /// <summary>
        /// 取得蓄水設施補助費用
        /// </summary>
        /// <param name="MapNo">版本編號</param>
        /// <returns>蓄水設施補助費用</returns>
        public int GetPoolMoney(int MapNo)
        {
            int PoolMoney = 0;
            if (DryDB.Pool.Any(p => p.MapNo == MapNo))
            {
                Calculate_Funding CalculateCls = new Calculate_Funding();
                Case casedata = GetCaseDataFromMapNo(MapNo);
                List<Dry.Models.Service.PoolDBService.PoolData> PoolList = new List<Service.PoolDBService.PoolData>();
                foreach (var poolitem in DryDB.Pool.Where(p => p.MapNo == MapNo).ToList())
                {
                    PoolList.Add(new Dry.Models.Service.PoolDBService.PoolData { poolSize = poolitem.PoolSize, poolType = poolitem.PtypeCode, poolW = poolitem.PoolWeight, poolPrice = poolitem.PoolPrice });
                }
                PoolMoney = CalculateCls.GetPoolMoneybyList(PoolList.ToArray(), casedata, MapNo).GovPay;
            }
            return PoolMoney;
        }
        #endregion

        #region 取得補助單位之補助款資料
        /// <summary>
        /// 取得補助單位之補助款資料
        /// </summary>
        /// <param name="Unit">補助單位代碼</param>
        /// <param name="Year">年度</param>
        /// <returns>補助單位之補助款資料</returns>
        public Subsidy GetApplyUnitData(short Unit, short Year)
        {
            if (Unit != 17)
                Unit = 0;
            return DryDB.Subsidy.Single(s => s.ApplyUnit == Unit && s.SYear == Year);
        }
        #endregion

        #region 依年度或回傳所有未結案之申請案件
        /// <summary>
        /// 依年度或回傳所有未結案之申請案件
        /// </summary>
        /// <param name="year">不填年度則回傳所有未結案之申請案件</param>
        /// <returns></returns>
        public List<Case> GetCase(int year = -1)
        {
            if (year == -1)
            { //取得所有
                return DryDB.Case.Where(c => c.Step != 13).ToList();
            }
            else
            {
                return DryDB.Case.Where(c => c.ApplyYear == year && c.Step != 13).ToList();
            }
        }
        #endregion

        #region 依版本編號取得各項目之支出金額列表
        /// <summary>
        /// 依版本編號取得各項目之支出金額
        /// </summary>
        /// <param name="mno">案件版本編號</param>
        /// <param name="Unit">補助單位(可不填，則回傳各項目之支出金額)</param>
        /// <returns></returns>
        public List<Pay> GetUnitMoney(int mno, int Unit = -1)
        {
            if (Unit == -1)
            {
                return DryDB.Pay.Where(p => p.MapNo == mno).ToList();
            }
            else
            {
                return DryDB.Pay.Where(p => p.MapNo == mno && p.ApplyUnit == Unit).ToList();
            }
        }
        #endregion

        #region 從CntrlApply中取得輔助單位代碼
        /// <summary>
        /// 從CntrlApply中取得輔助單位代碼
        /// </summary>
        /// <param name="MapNo">版本編號</param>
        /// <returns></returns>
        public short GetApplyUnitId(int MapNo)
        {
            /*try
            {
                return DryDB.CntrlApply.Find(MapNo).ApplyUnit;
            }
            catch
            {
                return -1;
            }*/
            if (DryDB.CntrlApply.Any(o => o.MapNo == MapNo))
            {
                return DryDB.CntrlApply.Find(MapNo).ApplyUnit;
            }
            else return -1;
        }
        #endregion
        
        #region 取得單位名稱
        /// <summary>
        /// 取得單位名稱
        /// </summary>
        /// <param name="UnitId"></param>
        /// <returns></returns>
        public string GetUnitName(short UnitId)
        {
            return CommDB.Unit.Find(UnitId).Unit1;
        }
        #endregion

        #region 取得單位代碼
        /// <summary>
        /// 取得單位代碼
        /// </summary>
        /// <param name="Admin_Id">使用者代碼</param>
        /// <returns></returns>
        public short GetUnitId(Guid AdminID)
        {
            short admin = 0;
            using (CommonEntities comm = new CommonEntities())
            {
                admin = comm.Admin.Single(ad =>ad.Admin_Id == AdminID).Unit_Id;
            }
            return admin;
        }
        /// <summary>
        /// 取得單位代碼
        /// </summary>
        /// <param name="MapNo">版本編號</param>
        /// <returns></returns>
        public short GetUnitId(int MapNo)
        {
            return DryDB.VerMapping.Find(MapNo).Case.ApplyUnit;
        }
        #endregion

        #region 取得現場勘查結果
        /// <summary>
        /// 取得現場勘查結果
        /// </summary>
        /// <param name="MapNo"></param>
        /// <returns></returns>
        public Examine GetExamineData(int MapNo)
        {
            return DryDB.Examine.Find(MapNo);
        }
        #endregion

        #region 取得勘查人員
        /// <summary>
        /// 取得勘查人員
        /// </summary>
        /// <param name="AdminID">登入者帳號</param>
        /// <returns></returns>
        public List<Admin> GetExamineAdmin(Guid AdminID)
        {
            short UnitID = CommDB.Admin.Find(AdminID).Unit_Id;
            return CommDB.Admin.Where(p => p.Unit_Id == UnitID).ToList();
        }
        #endregion
        #region 取得設計人員
        /// <summary>
        /// 取得設計人員
        /// </summary>
        /// <param name="UnitID"></param>
        /// <returns></returns>
        public List<DesignerList> GetDesignerList(short UnitID)
        {
            return DryDB.DesignerList.Where(p => p.Unit_Id == UnitID).ToList();
        }
        #endregion
        

        #region 取得作物類別
        /// <summary>
        /// 取得作物類別
        /// </summary>
        /// <returns></returns>
        public List<CropStruct> GetCrop()
        {
            List<CropStruct> CropList = new List<CropStruct>();
            foreach (var crop in CommDB.Crop.ToList())
            {
                CropList.Add(new CropStruct
                {
                    cropid = crop.Crop_Id,
                    cropname = crop.Crop1,
                    croptype = crop.Crop_Type.Crop_Type1
                });
            }
            return CropList;
        }
        #endregion
        /// <summary>
        /// 土地重複申請查詢V2
        /// </summary>
        /// <param name="sectid"></param>
        /// <param name="Landno"></param>
        /// <returns></returns>
        public MsgStruct ChkApplyV2(int sectid, string Landno)
        {
            string Name = GetFullSecName(sectid) + ", 地號 :" + Landno;
            MsgStruct msg = new MsgStruct();
            msg.status = Name + "\n\n尚未申請過補助!";
            msg.applied_area = 0;
            msg.IsApply = false;
            int num = 0;
            string status = Name + "\n\n補助申請記錄如下 :\n";
            double farm_area = 0f;
            double build_area = 0;
            double Barea1 = 0, Barea4 = 0, Barea5 = 0, Barea6 = 0;
            List<string> LandNoAry = SpileLandNo(Landno);
            string LandNoStr = LandNoAry[0] + "-" + LandNoAry[1];
            var farmdata = DryDB.FarmData.Where(f => f.LandNo == LandNoStr && f.Section == sectid).OrderByDescending(o => o.ApplyYear).FirstOrDefault();
            //if (DryDB.FarmData.Any(f => f.LandNo == LandNoStr && f.Section == sectid))
            if (farmdata != null)
                {
          
                
                msg.Farea = farmdata.FarmArea;
                msg.Ltype = farmdata.LandType;
                msg.IsApply = true;
                farm_area = farmdata.FarmArea;

                List<FarmData> farmlist = DryDB.FarmData.Where(f => f.LandNo == LandNoStr && f.Section == sectid).OrderByDescending(o => o.ApplyYear).ToList();
                int[] irrgitem = new int[] { 1, 4, 5, 6 };
                
                foreach (var farm in farmlist)
                {
                    //Case casedata = GetCaseDataFromMapNo(farm.MapNo);
                    CaseDetail casedetail = DryDB.CaseDetail.Where(m => m.MapNo == farm.MapNo).FirstOrDefault();
                    var payitems = from a in DryDB.Pay
                                       //join b in DryDB.SubsidyItemList on a.ItemCode equals b.ItemCode
                                   where a.MapNo == farm.MapNo && irrgitem.Contains(a.ItemCode) && a.PayMoney > 0
                                   select new { a.ItemCode };

                    string y = casedetail.ApplyYear.ToString();
                    string unitname = GetUnitName(casedetail.ApplyUnit);
                    string Barea = farm.BuildArea.ToString();
                    status +="\n" + y + "年於[" + unitname + "]申請如下\n";
                    status += "案號[" + casedetail.IANum.ToString() + "] " + "申請人:[" + casedetail.Name + "]\n";
                    //status += "申請人:" + casedetail.Name + "\n";
                    //status += "末端設施:" + casedetail.EndTypeCNS + "\n";
                    foreach (var item in payitems)
                    {
                        switch (item.ItemCode)
                        {
                            case 1:
                                Barea1 += farm.BuildArea;
                                status += $"田間管路({casedetail.CatalogCNS}):" + Barea + " m²\n";
                                break;
                            case 4:
                                Barea4 += farm.BuildArea;
                                status += "調控設施" + Barea + " m²\n";
                                break;
                            case 5:
                                Barea5 += farm.BuildArea;
                                status += "動力設備" + Barea + " m²\n";
                                break;
                            case 6:
                                Barea6 += farm.BuildArea;
                                status += "調蓄設施" + Barea + " m²\n";
                                break;
                        }
                    }
                    //status += Barea + " m²\n";
                    build_area += farm.FinalArea;
                    num++;
                    
                    //}
                }
                
            }
           
            if (num > 0)
            {
                status += "\n該筆農地面積: " + farm_area + " m²\n" +
                          "已經補助田間設施面積: " + Barea1.ToString() + " m²\n" +
                          "剩餘申請面積: " + (farm_area - Barea1 >= 0 ? (farm_area - Barea1).ToString() : "<span class='text-error'>" + (farm_area - Barea1).ToString() + "</span>") + " m²\n" +
                          "已經補助調控設施面積: " + Barea4.ToString() + " m²\n" +
                          "剩餘申請面積: " + (farm_area - Barea4 >= 0 ? (farm_area - Barea4).ToString() : "<span class='text-error'>" + (farm_area - Barea4).ToString() + "</span>") + " m²\n" +
                          //"已經補助動力設施面積: " + Barea5.ToString() + " m²\n" +
                          //"剩餘申請面積: " + (farm_area - Barea5).ToString() + " m²\n" +
                          "已經補助調蓄設施面積: " + Barea6.ToString() + " m²\n" +
                          "剩餘申請面積: " + (farm_area - Barea6 >= 0 ? (farm_area - Barea6).ToString() : "<span class='text-error'>" + (farm_area - Barea6).ToString() + "</span>") + " m²\n";

                msg.status = status;
                msg.applied_area = build_area;
                msg.area = farm_area - build_area;
            }
            
            string landnotmp = $"{int.Parse(LandNoAry[0])}";
            if (LandNoAry[1] != "0000")
            {
                landnotmp += $"-{int.Parse(LandNoAry[1])}";
            }

     
            var landdata = DryDB.LandData.Where(o => o.Section_Id == sectid).FirstOrDefault();
            if (landdata != null)
            {
                var leisurefarmlands = DryDB.LeisureFarm_LandInfo.Where(o => o.SectionCode == landdata.LandCode && o.LandNo == landnotmp);
                int recindex = 0;
                foreach (var item in leisurefarmlands)
                {
                    if (recindex == 0)
                    {
                        msg.status += $"\n\n休閒農場:{item.LeisureFarm.Farm_name}\n";
                    }
                    else msg.status += $"         {item.LeisureFarm.Farm_name}\n";
                    recindex++;                    
                }
                //msg.status = status;

                

                recindex = 0;
                var swbcdata = DryDB.SWCBPools.Where(o => o.SectionCode == landdata.LandCode && o.LandNo == LandNoStr);
                foreach (var item in swbcdata)
                {
                    if (recindex == 0)
                    {
                        msg.status += $"\n\n水保署調蓄設施申請案件:\n{item.ApplyYear}年申請{item.Pools}\n";
                    }
                    else msg.status += $"{item.ApplyYear}年申請{item.Pools}\n";
                    recindex++;
                }

            }
            
            

            return msg;
        }
        /// <summary>
        /// 土地重複申請查詢V3,年度資料加總
        /// </summary>
        /// <param name="sectid"></param>
        /// <param name="Landno"></param>
        /// <returns></returns>
        public MsgStruct ChkApplyV3(int sectid, string Landno)
        {
            string Name = GetFullSecName(sectid) + ", 地號 :" + Landno;
            MsgStruct msg = new MsgStruct();
            msg.status = Name + "\n\n尚未申請過補助!";
            msg.applied_area = 0;
            msg.IsApply = false;
            int num = 0;
            string status = Name + "\n\n補助申請記錄如下 :\n";
            double farm_area = 0f;
            double build_area = 0;
            double Barea1 = 0, Barea4 = 0, Barea5 = 0, Barea6 = 0;
            List<string> LandNoAry = SpileLandNo(Landno);
            string LandNoStr = LandNoAry[0] + "-" + LandNoAry[1];
            var farmdata = DryDB.FarmData.Where(f => f.LandNo == LandNoStr && f.Section == sectid).OrderByDescending(o => o.ApplyYear).FirstOrDefault();
            //if (DryDB.FarmData.Any(f => f.LandNo == LandNoStr && f.Section == sectid))
            if (farmdata != null)
            {
                //Farm Object
                FarmData farmObj = DryDB.FarmData.FirstOrDefault(f => f.LandNo == LandNoStr && f.Section == sectid);
                msg.Farea = farmObj.FarmArea;
                msg.Ltype = farmObj.LandType;
                msg.IsApply = true;
                farm_area = farmObj.FarmArea;
                List<FarmData> farmlist = DryDB.FarmData.Where(f => f.LandNo == LandNoStr && f.Section == sectid).OrderByDescending(o => o.ApplyYear).ToList();
                int[] irrgitem = new int[] { 1, 4, 5, 6 };//田間,調控,動力,蓄水

                string oldApplyyear = string.Empty;
                foreach (var farm in farmlist)
                {
                    //Case casedata = GetCaseDataFromMapNo(farm.MapNo);
                    CaseDetail casedetail = DryDB.CaseDetail.Where(m => m.MapNo == farm.MapNo).FirstOrDefault();
                    var payitems = from a in DryDB.Pay
                                       //join b in DryDB.SubsidyItemList on a.ItemCode equals b.ItemCode
                                   where a.MapNo == farm.MapNo && irrgitem.Contains(a.ItemCode)
                                   select new { a.ItemCode };

                    string y = casedetail.ApplyYear.ToString();
                    string unitname = GetUnitName(casedetail.ApplyUnit);
                    string Barea = farm.BuildArea.ToString();

                    if (y != oldApplyyear && oldApplyyear != string.Empty)
                    {

                        status += "\n" + oldApplyyear + "年該筆農地面積: " + farm_area + " m²\n" +
                          "已經補助田間設施面積: " + Barea1.ToString() + " m²\n" +
                          "剩餘申請面積: " + (farm_area - Barea1 >= 0 ? (farm_area - Barea1).ToString() : "<span class='text-error'>" + (farm_area - Barea1).ToString() + "</span>") + " m²\n" +
                          "已經補助調控設施面積: " + Barea4.ToString() + " m²\n" +
                          "剩餘申請面積: " + (farm_area - Barea4 >= 0 ? (farm_area - Barea4).ToString() : "<span class='text-error'>" + (farm_area - Barea4).ToString() + "</span>") + " m²\n" +
                          //"已經補助動力設施面積: " + Barea5.ToString() + " m²\n" +
                          //"剩餘申請面積: " + (farm_area - Barea5).ToString() + " m²\n" +
                          "已經補助調蓄設施面積: " + Barea6.ToString() + " m²\n" +
                          "剩餘申請面積: " + (farm_area - Barea6 >= 0 ? (farm_area - Barea6).ToString() : "<span class='text-error'>" + (farm_area - Barea6).ToString() + "</span>") + " m²\n";

                        oldApplyyear = casedetail.ApplyYear.ToString();
                        Barea1 = 0;
                        Barea4 = 0;
                        Barea5 = 0;
                        Barea6 = 0;


                    }


                    status += "\n" + y + "年於[" + unitname + "]申請如下\n";
                    status += "案號[" + casedetail.IANum.ToString() + "] " + "申請人:[" + casedetail.Name + "]\n";
                    //status += "申請人:" + casedetail.Name + "\n";
                    //status += "末端設施:" + casedetail.EndTypeCNS + "\n";
                    foreach (var item in payitems)
                    {
                        switch (item.ItemCode)
                        {
                            case 1:
                                Barea1 += farm.BuildArea;
                                status += $"田間管路({casedetail.CatalogCNS}):" + Barea + " m²\n";
                                break;
                            case 4:
                                Barea4 += farm.BuildArea;
                                status += "調控設施" + Barea + " m²\n";
                                break;
                            case 5:
                                Barea5 += farm.BuildArea;
                                status += "動力設備" + Barea + " m²\n";
                                break;
                            case 6:
                                Barea6 += farm.BuildArea;
                                status += "調蓄設施" + Barea + " m²\n";
                                break;
                        }
                    }
                    //status += Barea + " m²\n";
                    build_area += farm.FinalArea;
                    num++;
                    oldApplyyear = y;
                    //}
                }
                status += "\n" + oldApplyyear + "年該筆農地面積: " + farm_area + " m²\n" +
                          "已經補助田間設施面積: " + Barea1.ToString() + " m²\n" +
                          "剩餘申請面積: " + (farm_area - Barea1 >= 0 ? (farm_area - Barea1).ToString() : "<span class='text-error'>" + (farm_area - Barea1).ToString() + "</span>") + " m²\n" +
                          "已經補助調控設施面積: " + Barea4.ToString() + " m²\n" +
                          "剩餘申請面積: " + (farm_area - Barea4 >= 0 ? (farm_area - Barea4).ToString() : "<span class='text-error'>" + (farm_area - Barea4).ToString() + "</span>") + " m²\n" +
                          //"已經補助動力設施面積: " + Barea5.ToString() + " m²\n" +
                          //"剩餘申請面積: " + (farm_area - Barea5).ToString() + " m²\n" +
                          "已經補助調蓄設施面積: " + Barea6.ToString() + " m²\n" +
                          "剩餘申請面積: " + (farm_area - Barea6 >= 0 ? (farm_area - Barea6).ToString() : "<span class='text-error'>" + (farm_area - Barea6).ToString() + "</span>") + " m²\n";
            }
           
            string landnotmp = $"{int.Parse(LandNoAry[0])}";
            if (LandNoAry[1] != "0000")
            {
                landnotmp += $"-{int.Parse(LandNoAry[1])}";
            }

            //string landnotmp = $"{int.Parse(Landno.Split('-')[0])}";
            //if (Landno.Split('-')[1] != "0000")
            //{
            //    landnotmp += $"-{int.Parse(Landno.Split('-')[1])}";
            //}
            var landdata = DryDB.LandData.Where(o => o.Section_Id == sectid).FirstOrDefault();
            if (landdata != null)
            {
                var leisurefarmlands = DryDB.LeisureFarm_LandInfo.Where(o => o.SectionCode == landdata.LandCode && o.LandNo == landnotmp);
                int recindex = 0;
                foreach (var item in leisurefarmlands)
                {
                    if (recindex == 0)
                    {
                        msg.status += $"\n\n休閒農場:{item.LeisureFarm.Farm_name}\n";
                    }
                    else msg.status += $"         {item.LeisureFarm.Farm_name}\n";
                    recindex++;
                }
                //msg.status = status;

                

                recindex = 0;
                var swbcdata = DryDB.SWCBPools.Where(o => o.SectionCode == landdata.LandCode && o.LandNo == LandNoStr);
                foreach (var item in swbcdata)
                {
                    if (recindex == 0)
                    {
                        msg.status += $"\n\n水保署調蓄設施申請案件:\n{item.ApplyYear}年申請{item.Pools}\n";
                    }
                    else msg.status += $"{item.ApplyYear}年申請{item.Pools}\n";
                    recindex++;
                }

            }



            return msg;
        }

        /// <summary>
        /// 土地重複申請查詢扣除目前申報案件，舊系統案件不再查詢
        /// </summary>
        /// <param name="sectid"></param>
        /// <param name="Landno"></param>
        /// <param name="mno"></param>
        /// <returns></returns>
        public MsgStruct ChkApply4Case(int sectid, string Landno, int mno)
        {
            string Name = GetFullSecName(sectid) + ", 地號 :" + Landno;
            MsgStruct msg = new MsgStruct();
            msg.status = Name + "\n\n尚未申請過補助!";
            msg.applied_area = 0;
            msg.IsApply = false;
            int num = 0;
            string status = Name + "\n\n補助申請記錄如下 :\n";
            double farm_area = 0f;
            double build_area = 0;
            double Barea1 = 0, Barea4 = 0, Barea5 = 0, Barea6 = 0;
            List<string> LandNoAry = SpileLandNo(Landno);
            string LandNoStr = LandNoAry[0] + "-" + LandNoAry[1];
            var farmdata = DryDB.FarmData.Where(f => f.LandNo == LandNoStr && f.Section == sectid).OrderByDescending(o => o.ApplyYear).FirstOrDefault();
            //if (DryDB.FarmData.Any(f => f.LandNo == LandNoStr && f.Section == sectid))
            if (farmdata != null)
            {
                //Farm Object
                FarmData farmObj = DryDB.FarmData.OrderByDescending(o => o.ApplyYear).FirstOrDefault(f => f.LandNo == LandNoStr && f.Section == sectid );

                msg.Farea = farmObj.FarmArea;
                msg.Ltype = farmObj.LandType;
                msg.IsApply = true;
                farm_area = farmObj.FarmArea;
                List<FarmData> farmlist = DryDB.FarmData.Where(f => f.LandNo == LandNoStr && f.Section == sectid && f.MapNo != mno).OrderByDescending(o => o.ApplyYear).ToList();
                int[] irrgitem = new int[] { 1, 4, 5, 6 };


                foreach (var farm in farmlist)
                {
                    //Case casedata = GetCaseDataFromMapNo(farm.MapNo);
                    CaseDetail casedetail = DryDB.CaseDetail.Where(m => m.MapNo == farm.MapNo).FirstOrDefault();
                    var payitems = from a in DryDB.Pay
                                       //join b in DryDB.SubsidyItemList on a.ItemCode equals b.ItemCode
                                   where a.MapNo == farm.MapNo /*&& irrgitem.Contains(a.ItemCode)*/
                                   select new { a.ItemCode };
                    //string irrgCNS = string.Empty;




                    //if (casedata.Complete) //If the case has been completed
                    //{
                    //string y = casedata.ApplyYear.ToString();
                    string y = casedetail.ApplyYear.ToString();
                    //string unitname = GetUnitName(casedata.ApplyUnit);
                    string unitname = GetUnitName(casedetail.ApplyUnit);
                    string Barea = farm.BuildArea.ToString();
                    //status += "案號[" + casedata.IANum.ToString() + "]\n";
                    //status += "申請人:" + casedetail.Name + "\n";
                    //status += "末端設施:" + casedetail.EndTypeCNS + "\n";
                    //status += y + "年於[" + unitname + "]申請,施設: " + Barea + " m²\n";

                    status += y + "年於[" + unitname + "]申請如下\n";
                    //status += "案號[" + casedata.IANum.ToString() + "] " + "申請人:[" + casedetail.Name + "]\n";
                    status += "案號[" + casedetail.IANum.ToString() + "] " + "申請人:[" + casedetail.Name + "]\n";
                    //status += "申請人:" + casedetail.Name + "\n";
                    //status += "末端設施:" + casedetail.EndTypeCNS + "\n";
                    foreach (var item in payitems)
                    {
                        switch (item.ItemCode)
                        {
                            case 1:
                                Barea1 += farm.BuildArea;
                                status += $"田間管路({casedetail.CatalogCNS}):" + Barea + " m²\n";
                                break;
                            case 4:
                                Barea4 += farm.BuildArea;
                                status += "調控設施" + Barea + " m²\n";
                                break;
                            case 5:
                                Barea5 += farm.BuildArea;
                                status += "動力設備" + Barea + " m²\n";
                                break;
                            case 6:
                                Barea6 += farm.BuildArea;
                                status += "調蓄設施" + Barea + " m²\n";
                                break;
                        }
                    }
                    //status += Barea + " m²\n";
                    build_area += farm.FinalArea;
                    num++;
                    //}
                }
            }

            if (num > 0)
            {
                //status += "\n該筆農地面積: " + farm_area + " m²\n" +
                //          "已經補助面積: " + build_area + " m²\n" +
                //          "剩餘申請面積: " + (farm_area - build_area).ToString() + " m²\n";
                status += "\n該筆農地面積: " + farm_area + " m²\n" +
                    "已經補助田間設施面積: " + Barea1.ToString() + " m²\n" +
                    "剩餘申請面積: " + (farm_area - Barea1 >= 0 ? (farm_area - Barea1).ToString() : "<span class='text-error'>" + (farm_area - Barea1).ToString() + "</span>") + " m²\n" +
                    "已經補助調控設施面積: " + Barea4.ToString() + " m²\n" +
                    "剩餘申請面積: " + (farm_area - Barea4 >= 0 ? (farm_area - Barea4).ToString() : "<span class='text-error'>" + (farm_area - Barea4).ToString() + "</span>") + " m²\n" +
                    //"已經補助動力設施面積: " + Barea5.ToString() + " m²\n" +
                    //"剩餘申請面積: " + (farm_area - Barea5).ToString() + " m²\n" +
                    "已經補助調蓄設施面積: " + Barea6.ToString() + " m²\n" +
                    "剩餘申請面積: " + (farm_area - Barea6 >= 0 ? (farm_area - Barea6).ToString() : "<span class='text-error'>" + (farm_area - Barea6).ToString() + "</span>") + " m²\n";

                msg.status = status;
                msg.applied_area = build_area;
                msg.area = farm_area - build_area;
            }
    
            string landnotmp = $"{int.Parse(LandNoAry[0])}";
            if (LandNoAry[1] != "0000")
            {
                landnotmp += $"-{int.Parse(LandNoAry[1])}";
            }

            //string landnotmp = $"{int.Parse(Landno.Split('-')[0])}";
            //if (Landno.Split('-')[1] != "0000")
            //{
            //    landnotmp += $"-{int.Parse(Landno.Split('-')[1])}";
            //}
            var landdata = DryDB.LandData.Where(o => o.Section_Id == sectid).FirstOrDefault();
            if (landdata != null)
            {
                var leisurefarmlands = DryDB.LeisureFarm_LandInfo.Where(o => o.SectionCode == landdata.LandCode && o.LandNo == landnotmp);
                int recindex = 0;
                foreach (var item in leisurefarmlands)
                {
                    if (recindex == 0)
                    {
                        msg.status += $"\n\n休閒農場:{item.LeisureFarm.Farm_name}\n";
                    }
                    else msg.status += $"         {item.LeisureFarm.Farm_name}\n";
                    recindex++;
                }
                //msg.status = status;

                
                recindex = 0;
                var swbcdata = DryDB.SWCBPools.Where(o => o.SectionCode == landdata.LandCode && o.LandNo == LandNoStr);
                foreach (var item in swbcdata)
                {
                    if (recindex == 0)
                    {
                        msg.status += $"\n\n水保署調蓄設施申請案件:\n{item.ApplyYear}年申請{item.Pools}\n";
                    }
                    else msg.status += $"{item.ApplyYear}年申請{item.Pools}\n";
                    recindex++;
                }

            }



            return msg;
        }
        /// <summary>
        /// 土地施設項目重複申請
        /// </summary>
        /// <param name="sectid"></param>
        /// <param name="landno"></param>
        /// <param name="itemcode"></param>
        /// <param name="mapno"></param>
        /// <returns></returns>
        public MsgStruct ChkApplyByItem(int sectid, string landno, int itemcode, int mapno)
        {
            MsgStruct msg = new MsgStruct();
            msg.IsApply = false;
            int yearlimit = 10;
            int yearSatrt = DateTime.Now.Year - 1911 - yearlimit;
            var data = from a in DryDB.Farm
                       join b in DryDB.SummaryView on a.MapNo equals b.MapNo
                       join c in DryDB.Pay on a.MapNo equals c.MapNo
                       where a.Section == sectid && a.LandNo == landno && c.ItemCode == itemcode && a.MapNo != mapno
                       && b.ApplyYear >= yearSatrt
                       group a by a.LandNo into e
                       select new { e.Key, area = e.Sum(s => s.BuildArea), farmarea = e.Max(m => m.FarmArea) };
            if (data.Count() != 0)
            {
                msg.applied_area = data.First().area;
                msg.area = data.First().farmarea;
                msg.IsApply = true;
            }
            return msg;
        }

        /// <summary>
        /// 土地重複申請查詢
        /// </summary>
        /// <param name="sectid"></param>
        /// <param name="Landno"></param>
        /// <returns></returns>
        public MsgStruct ChkApply(int sectid, string Landno)
        {
            string Name = GetFullSecName(sectid) + ", 地號 :" + Landno;
            MsgStruct msg = new MsgStruct();
            msg.status = Name + "\n\n尚未申請過補助!";
            msg.applied_area = 0;
            msg.IsApply = false;
            int num = 0;
            string status = Name + "\n\n補助申請記錄如下 :\n";
            double farm_area = 0f;
            double build_area = 0;
            List<string> LandNoAry = SpileLandNo(Landno);
            string LandNoStr = LandNoAry[0] + "-" + LandNoAry[1];
            var farmdata = DryDB.FarmData.Where(f => f.LandNo == LandNoStr && f.Section == sectid).FirstOrDefault();
            //if (DryDB.FarmData.Any(f => f.LandNo == LandNoStr && f.Section == sectid))
            if (farmdata != null)
            {
                //Farm Object
                FarmData farmObj = DryDB.FarmData.OrderByDescending(o => o.ApplyYear).FirstOrDefault(f => f.LandNo == LandNoStr && f.Section == sectid);

                msg.Farea = farmObj.FarmArea;
                msg.Ltype = farmObj.LandType;
                msg.IsApply = true;
                farm_area = farmObj.FarmArea;
                List<FarmData> farmlist = DryDB.FarmData.Where(f => f.LandNo == LandNoStr && f.Section == sectid).OrderByDescending(o => o.ApplyYear).ToList();
                foreach (var farm in farmlist)
                {
                    Case casedata = GetCaseDataFromMapNo(farm.MapNo);
                    CaseDetail casedetail = DryDB.CaseDetail.Where(m => m.MapNo == farm.MapNo).FirstOrDefault();





                    //if (casedata.Complete) //If the case has been completed
                    //{
                    string y = casedata.ApplyYear.ToString();
                    string unitname = GetUnitName(casedata.ApplyUnit);
                    string Barea = farm.BuildArea.ToString();
                    status += "案號[" + casedata.IANum.ToString() + "]\n";
                    status += "申請人:" + casedetail.Name + "\n";
                    status += "末端設施:" + casedetail.EndTypeCNS + "\n";
                    status += y + "年於[" + unitname + "]申請,施設: " + Barea + " m²\n";
                    build_area += farm.FinalArea;
                    num++;
                    //}
                }
            }
            //List<OldSystemFarm> OldFarm = new Dry.Models.OldSystemData.Old_ReapplyQuery().QueryOldFarm(sectid, LandNoStr);
            var OldFarm = new Dry.Models.OldSystemData.Old_ReapplyQuery().QueryOldFarm(sectid, LandNoStr);
            foreach (var item in OldFarm)
            {
                var rec = (from tab1 in DryDB.Old_Farmer
                           where tab1.年度 == item.年度 && tab1.編號1 == item.編號1 && tab1.單位 == item.單位
                           select new { apname = tab1.姓名, tab1.灌溉器名稱 }).FirstOrDefault();
                string apname = (rec != null) ? rec.apname : "沒資料";

                string endtypecns = (rec != null) ? rec.灌溉器名稱 : "其它";


                string y = item.年度.ToString();
                string unitname = DryDB.Old_Unit.Where(m => m.LOCAL == item.單位).FirstOrDefault().LOCNS;
                string Barea = (item.面積 * 10000).ToString(); 
                //farm_area = item.面積 * 10000; 
                status += "案號[" + item.編號1 + "]\n";
                status += "申請人:" + apname + "\n";
                status += "末端設施:" + endtypecns + "\n";
                status += y + "年於[" + unitname + "]申請,施設: " + Barea + " m²\n";
                build_area += (item.面積 * 10000);
                num++;
            }
            if (num > 0)
            {
                status += "\n該筆農地面積: " + farm_area + " m²\n" +
                          "已經補助面積: " + build_area + " m²\n" +
                          "剩餘申請面積: " + (farm_area - build_area).ToString() + " m²\n";
                msg.status = status;
                msg.applied_area = build_area;
                msg.area = farm_area - build_area;
            }
            
            string landnotmp = $"{int.Parse(LandNoAry[0])}";
            if (LandNoAry[1] != "0000")
            {
                landnotmp += $"-{int.Parse(LandNoAry[1])}";
            }

            var landdata = DryDB.LandData.Where(o => o.Section_Id == sectid).FirstOrDefault();
            if (landdata != null)
            {
                var leisurefarmlands = DryDB.LeisureFarm_LandInfo.Where(o => o.SectionCode == landdata.LandCode && o.LandNo == landnotmp);
                int recindex = 0;
                foreach (var item in leisurefarmlands)
                {
                    if (recindex == 0)
                    {
                        msg.status += $"\n\n休閒農場:{item.LeisureFarm.Farm_name}\n";
                    }
                    else msg.status += $"         {item.LeisureFarm.Farm_name}\n";
                    recindex++;
                }
                //msg.status = status;

             
                recindex = 0;
                var swbcdata = DryDB.SWCBPools.Where(o => o.SectionCode == landdata.LandCode && o.LandNo == LandNoStr);
                foreach (var item in swbcdata)
                {
                    if (recindex == 0)
                    {
                        msg.status += $"\n\n水保署調蓄設施申請案件:\n{item.ApplyYear}年申請{item.Pools}\n";
                    }
                    else msg.status += $"{item.ApplyYear}年申請{item.Pools}\n";
                    recindex++;
                }

            }



            return msg;
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
        #region 取得案件是否為重複申請
        /// <summary>
        /// 取得案件是否為重複申請
        /// </summary>
        /// <param name="mno"></param>
        /// <returns></returns>
        public bool ChkAppliedByMaoNo(int mno)
        {
            return DryDB.Farm.Any(m => m.MapNo == mno && m.IsApplied == true);
        }
        #endregion

        #region 取得公版系統列表
        /// <summary>
        /// 取得公版系統列表
        /// </summary>
        /// <param name="FacNo">The Standard System No.</param>
        /// <returns></returns>
        public List<StdFacSys> GetStdSys(int FacNo = -1)
        {
            if (FacNo.Equals(""))
            {
                return DryDB.StdFacSys.ToList();
            }
            else
            {
                return DryDB.StdFacSys.Where(s => s.FacNo == FacNo).ToList();
            }
        }
        /// <summary>
        /// 取得公版系統-依單位別
        /// </summary>
        /// <param name="UnitID">單位代碼</param>
        /// <returns></returns>
        public List<StdFacSys> GetStdSysByUnit(short UnitID)
        {
            return DryDB.StdFacSys.Where(s => s.Unit == UnitID).ToList();
        }
        /// <summary>
        /// 取得公版系統-依末端型式
        /// </summary>
        /// <param name="EndType">末端型式代碼</param>
        /// <returns></returns>
        public List<StdFacSys> GetStdSysByEndType(byte EndType, short UnitID)
        {
            return DryDB.StdFacSys.Where(s => s.EndType == EndType && s.Unit == UnitID).ToList();
        }
        #endregion

        #region 取得最新版本編號
        /// <summary>
        /// 取得最新版本編號
        /// </summary>
        /// <param name="eventno">案件案號</param>
        /// <returns>Newest MapNo</returns>
        public int GetNewestMapNo(int eventno)
        {
            int MapNo = -1;
            if (DryDB.VerMapping.Any(v => v.EventNo == eventno))
            {
                MapNo = DryDB.VerMapping.Where(v => v.EventNo == eventno).Max(v => v.MapNo);
            }
            return MapNo;
        }
        #endregion

        #region 透過案件編號取得最新版本資料
        /// <summary>
        /// 透過案件編號取得最新版本資料
        /// </summary>
        /// <param name="eventno">案件案號</param>
        /// <returns>取得最新版本資料</returns>
        public VerMapping GetVerMapData(int eventno)
        {
            if (DryDB.VerMapping.Any(v => v.EventNo == eventno))
            {
                int MapNo = DryDB.VerMapping.Where(v => v.EventNo == eventno).Max(v => v.MapNo);
                return DryDB.VerMapping.Single(v => v.MapNo == MapNo);
            }
            else
            {
                return null;
            }
        }
        #endregion
        
        #region 取得前一版的版本編號
        /// <summary>
        /// 取得前一版的版本編號
        /// </summary>
        /// <param name="MapNo">版本編號</param>
        /// <returns></returns>
        public int GetBeforeNowMapNo(int MapNo)
        {
            int evenno = GetVerMappingData(MapNo).EventNo;
            int[] mnoary = DryDB.VerMapping.Where(v => v.EventNo == evenno).OrderBy(v => v.MapNo).Select(v => v.MapNo).ToArray();
            if (mnoary.Length == 1)///測試變更設計用，正式需刪除，以else那個取法為準
            {
                return mnoary[0];
            }
            else
            {
                return mnoary[mnoary.Length - 2];
            }
        }
        #endregion

        #region 取得物料價格
        /// <summary>
        /// 取得物料價格
        /// </summary>
        /// <param name="Mat">物料編號</param>
        /// <param name="ApplyY">申請年度</param>
        /// <param name="ApplyUnit">申請水利會</param>
        /// <returns>物料價格</returns>
        public double GetMatPrice(int Mat, short ApplyY, short ApplyUnit)
        {
            if (DryDB.PriceOfMat.Any(pom => pom.POMNo == Mat && pom.BYear == ApplyY))
            {
                List<PriceOfMat> PriceOfMatList = DryDB.PriceOfMat.Where(pom => pom.POMNo == Mat && pom.BYear == ApplyY).ToList();
                return PriceOfMatList[0].Price;
            }
            else
            {
                return 1;
            }
        }
        #endregion

        #region 取得 Module Mapping name
        public string GetModuleName(int moduleNo)
        {
            return DryDB.Mat_Module.Single(m => m.ModuleNo == moduleNo).ModuleCNS;
        }
        #endregion

        #region 由農戶代碼取得申請案件資料
        /// <summary>
        /// 取得申請案件資料
        /// </summary>
        /// <param name="Fid">農戶代碼</param>
        /// <returns></returns>
        public List<Case> GetCase(Guid Fid)
        {
            using (DryEntities dry = new DryEntities())
            {
                return dry.Case.Where(p => p.FId == Fid).ToList();
            }
        }
        #endregion

        #region 透過案件代碼取得農戶代碼
        /// <summary>
        /// 取得申請案件代碼
        /// </summary>
        /// <param name="EvntNo">案件代碼</param>
        /// <returns></returns>
        public Guid GetFIdByEvntNo(int EvntNo)
        {
            using (DryEntities dry = new DryEntities())
            {
                Case cases = dry.Case.Find(EvntNo);
                return cases == null ? Guid.Empty : cases.FId;
            }
        }
        #endregion

        #region 透過農戶代碼取得農戶資料
        public Farmer GetFarmer(Guid Fid)
        {
            using (DryEntities dry = new DryEntities())
            {
                return dry.Farmer.Find(Fid);
            }
        }
        #endregion

        #region 取得 完工之RadioList(完工 未完工)
        public List<System.Web.Mvc.SelectListItem> GetRadioList(bool check = false)
        {
            List<System.Web.Mvc.SelectListItem> item = new List<System.Web.Mvc.SelectListItem>();

            
            if (check)
            {
                item.Add(new System.Web.Mvc.SelectListItem()
                {
                    Text = "未完工",
                    Value = "false"
                });
                item.Add(new System.Web.Mvc.SelectListItem()
                {
                    Text = "完工",
                    Value = "true",
                    Selected = true
                });
            }
            else
            {
                item.Add(new System.Web.Mvc.SelectListItem()
                {
                    Text = "未完工",
                    Value = "false",
                    Selected = true
                });
                item.Add(new System.Web.Mvc.SelectListItem()
                {
                    Text = "完工",
                    Value = "true"
                });
            }
            return item;
        }
        #endregion

        #region 取得驗收狀況列表
        /// <summary>
        /// 取得驗收狀況列表
        /// </summary>        
        /// <returns></returns>
        public List<FacStatList> GetFacStatus()
        {
            return DryDB.FacStatList.ToList();
        }
        #endregion

        #region 取得案件所有申請項目之費用 依版本編號

        /// <summary>
        /// 取得案件所有申請項目之費用 依版本編號
        /// </summary>
        /// <param name="MapNo">版本編號</param>
        /// <returns></returns>
        public MoneyStruct GetAllApplyMoney(int _MNo) 
        {
            CtrlPriceService CPSer = new CtrlPriceService();
            Calculate_Funding calFund = new Calculate_Funding();

            MoneyStruct moneydata = new MoneyStruct();
            List<Pay> pays = DryDB.Pay.Where(m => m.MapNo == _MNo).ToList();

            #region Get Data
            Case CaseDta = GetCaseDataFromMapNo(_MNo);
            Farmer farmer = GetFarmerData(_MNo);
            List<Dry.Models.CommonCls.GetData.NewFarm> Farm = GetFarmData(_MNo).OrderBy(f => f.LandNo).ToList();
            //FarmerSys FSys = GetFarmerSystemData(_MNo);
            PigingConf piping = GetPigingConfData(_MNo);
            List<FarmerSysM> FarmerSysMat = GetFarmerSysM(_MNo);
            List<Pool> FarmerPool = GetPoolData(_MNo);
            MappingClass mapingCls = new MappingClass();
            List<Pay> payData = GetPay(_MNo);
            TotalFee totalFee = GetTotalFee(_MNo);
            #endregion


            #region Piping System(田間管路)
            int[] p_money = { 0, 0, 0, 0 };
            if (piping != null)
            {
                
                
                p_money = GetPigingMoneyByDB(_MNo);


                #region Irr System(灌溉系統)
 
                moneydata.IrrsM = payData.Where(m => m.ItemCode == 1).Sum(n => n.PayMoney + n.FarmerMoney);
                #endregion

                #region 材料費
                
                moneydata.MatM = moneydata.IrrsM;
                #endregion

                #region 工作費
                moneydata.WorkM = p_money[3];
                #endregion

                #region 田間管路設施費
                
                moneydata.PipingM = moneydata.MatM /*+ moneydata.WorkM*/;
                
                #endregion

                #region 規劃設計費
       
                moneydata.PlanM = payData.Where(o => o.ItemCode == 2).FirstOrDefault() == null ? 0 : payData.Where(o => o.ItemCode == 2).FirstOrDefault().PayMoney;
                
                #endregion
            }
            #endregion

            #region 調控設施
           
            moneydata.ControlM = payData.Where(m => m.ItemCode == 4).Sum(n => n.PayMoney + n.FarmerMoney);
            
            #endregion

            #region 動力設備
            //var englist = GetEngineData(_MNo);
            moneydata.EngineM = payData.Where(m => m.ItemCode == 5).Sum(n => n.PayMoney + n.FarmerMoney);

            #endregion

            #region 蓄水池
            moneydata.PoolM = payData.Where(m => m.ItemCode == 6).Sum(n => n.PayMoney + n.FarmerMoney);

            #endregion

            #region 合計
            if (totalFee != null)
            {
                moneydata.Sum = totalFee.FarmerFee + totalFee.GovSubsidy;
            } else
            {
                moneydata.Sum = 0;
            }
            #endregion

            #region 農戶配合款
            
            moneydata.FarmerPay += p_money[2];
            
            moneydata.FarmerPay += payData.Where(m => m.ItemCode == 4).Sum(n => n.FarmerMoney);
            #endregion

            #region 政府補助款
            govMoneyStruct govS = new govMoneyStruct();
            Boolean gold = GetGoldFromMapno(_MNo);

            if (!gold)
            {
                
                govS.PayFarmer = pays.Where(m => m.ItemCode != 2 && m.ItemCode != 8).Sum(n => n.PayMoney);
                
                govS.PlanM = moneydata.PlanM;
                
                govS.SubSum = govS.PayFarmer + govS.PlanM;

                moneydata.GovPay = govS;
            }
            else if (CaseDta.ApplyYear >= 109)
            {
                govS.PayFarmer = pays.Where(m => m.ItemCode != 2 && m.ItemCode != 8).Sum(n => n.PayMoney);
                
                govS.PlanM = moneydata.PlanM;
                
                govS.SubSum = govS.PayFarmer + govS.PlanM;

                moneydata.GovPay = govS;
            } else {
                foreach (var item in pays)
                {
                    int govm = 0;
                    int goldm = 0;
                    int farmm = 0;
                    switch (item.ItemCode){
                        case 1:
                            govm= item.PayMoney;
                            goldm= (int)Math.Round((double)item.Total * 0.7, 0, MidpointRounding.AwayFromZero) - govm;
                            farmm = (int)item.Total - govm - goldm;
                            govS.CoaM += govm;
                            govS.starM += goldm;
                            govS.PayFarmer += govm + goldm;
                            break;
                        case 2:
                            govS.PlanM = (int)item.Total;

                            break;
                        case 3:

                            break;
                        case 4:
                            govm= item.PayMoney;
                            goldm= (int)Math.Round((double)item.Total * 0.7, 0, MidpointRounding.AwayFromZero) - govm;
                            farmm = (int)item.Total - govm - goldm;
                            govS.CoaM += govm;
                            govS.starM += goldm;
                            govS.PayFarmer += govm + goldm;

                            break;
                        case 5:
                            if (item.ApplyUnit == 17)
                            {
                                govS.LiuM += (int)item.Total;
                                govS.PayFarmer += (int)item.Total;
                            }
                            else
                            {
                                govS.PayFarmer += (int)item.Total;
                            }
                            break;
                        case 6:
                            if (item.ApplyUnit == 17)
                            {
                                govS.LiuM += (int)item.Total;
                                govS.PayFarmer += (int)item.Total;
                            }
                            else
                            {
                                govS.PayFarmer += (int)item.Total;
                            }
                            break;
                        case 7:
                            break;
                        case 8:
                            govm= item.PayMoney;
                            goldm= (int)Math.Round((double)item.Total * 0.7, 0, MidpointRounding.AwayFromZero) - govm;
                            farmm = (int)item.Total - govm - goldm;
                            govS.CoaM += govm;
                            govS.starM += goldm;
                            govS.PayFarmer += govm + goldm;
                            break;

                    }
                }
                govS.SubSum = govS.PayFarmer + govS.PlanM;

                moneydata.GovPay = govS;
            }
            #endregion

            #region 本設施預算總計
            moneydata.Total = moneydata.Sum;
            #endregion

            return moneydata;
        }
        #endregion

        #region 取得特定水利會最新案件編號
        /// <summary>
        /// 取得特定水利會最新案件編號
        /// </summary>
        /// <param name="Unit">單位</param>
        /// <param name="Year">年度</param>
        /// <returns></returns>
        public int GetNewIANum(Int16 Unit, int Year)
        {
            int IANum = (Year * 100000) + 1;
            if (DryDB.Case.Any(c => c.ApplyUnit == Unit && c.ApplyYear == Year))
            {
                IANum = DryDB.Case.Where(c => c.ApplyUnit == Unit && c.ApplyYear == Year).Max(c => c.IANum);
                IANum++;
            }
            return IANum;
        }
        #endregion

       

        #region Money Data Struct
        public class MoneyStruct
        {
            /// <summary>
            /// 田間管路設施費
            /// </summary>
            public int PipingM { get; set; }
            /// <summary>
            /// 材料費
            /// </summary>
            public int MatM { get; set; }
            /// <summary>
            /// L1費用
            /// </summary>
            public int L1M { get; set; }
            /// <summary>
            /// L2費用
            /// </summary>
            public int L2M { get; set; }

            /// <summary>
            /// 灌溉系統費用
            /// </summary>
            public int IrrsM { get; set; }

            /// <summary>
            /// 工作費
            /// </summary>
            public int WorkM { get; set; }

            /// <summary>
            /// 規劃設計費
            /// </summary>
            public int PlanM { get; set; }

            /// <summary>
            /// 調控設施費用
            /// </summary>
            public int ControlM { get; set; }

            /// <summary>
            /// 動力設施費用
            /// </summary>
            public int EngineM { get; set; }

            /// <summary>
            /// 蓄水設施費用
            /// </summary>
            public int PoolM { get; set; }

            /// <summary>
            /// 合計:總工程費用
            /// </summary>
            public int Sum { get; set; }

            /// <summary>
            /// 農戶配合款
            /// </summary>
            public int FarmerPay { get; set; }

            /// <summary>
            /// 政府補助款
            /// </summary>
            public govMoneyStruct GovPay { get; set; }

            /// <summary>
            /// 本設施預算總計
            /// </summary>
            public int Total { get; set; }

        }
        #endregion

        #region govMoney Data Struct
        public class govMoneyStruct
        {
            /// <summary>
            /// 農戶請領款
            /// </summary>
            public int PayFarmer { get; set; }
            /// <summary>
            /// 規劃設計費
            /// </summary>
            public int PlanM { get; set; }

            /// <summary>
            /// 農委會補助款
            /// </summary>
            public int CoaM { get; set; }

            /// <summary>
            /// 瑠公水利會協助款
            /// </summary>
            public int LiuM { get; set; }

            /// <summary>
            /// 七星水利會協助款
            /// </summary>
            public int starM { get; set; }

            /// <summary>
            /// 小計
            /// </summary>
            public int SubSum { get; set; }
        }
        #endregion

        #region Crop Data Struct
        public class CropStruct
        {
            /// <summary>
            /// 作物代碼
            /// </summary>
            public short cropid { get; set; }
            /// <summary>
            /// 作物名稱
            /// </summary>
            public string cropname { get; set; }

            /// <summary>
            /// 作物類別
            /// </summary>
            public string croptype { get; set; }

        }
        #endregion

        #region Check Land Apply Msg Struct
        public class MsgStruct
        {
            /// <summary>
            /// 是否重覆申請
            /// </summary>
            public bool IsApply { get; set; }
            public int IANum { get; set; }
            /// <summary>
            /// 申請狀況
            /// </summary>
            public string status { get; set; }
            /// <summary>
            /// 已申請面積
            /// </summary>
            public double applied_area { get; set; }
            /// <summary>
            /// 可申請面積
            /// </summary>
            public double area { get; set; }

            /// <summary>
            /// 農地面積
            /// </summary>
            public double Farea { get; set; }

            /// <summary>
            /// 農地地目
            /// </summary>
            public byte Ltype { get; set; }
        }
        #endregion

        #region 取得 農戶設施系統範本
        public List<FSysDs> GetFarSys(byte ApplyUnit, short ApplyYear = 0)
        {
            if (ApplyYear == 0)
            {
                ApplyYear = (short)(DateTime.Now.Year - 1911);
            }

            List<FSysDs> res = new List<FSysDs>();
            //List<Case> list = DryDB.Case.Where(c => c.ApplyUnit == ApplyUnit && c.Complete == true).ToList();
            List<Case> list = DryDB.Case.Where(c => c.ApplyUnit == ApplyUnit && c.Complete == true).ToList();
            foreach (var cse in list)
            {
                VerMapping ver = GetVerMapData(cse.EventNo);
                Farmer far = GetFarmerData(ver.MapNo);
                PigingConf piping = GetPigingConfData(ver.MapNo);
                List<EndType> endType = DryDB.EndType.Where(p => p.MapNo == piping.MapNo).ToList();
                List<FarmerSysM> farMAT = GetFarmerSysM(ver.MapNo);

                int buildArea = GetFarmBuildArea(ver.MapNo);
                int ianum = cse.IANum;
                string farname = far.Name;
                string farsys = GetFarmerSystemData(ver.MapNo).FacTypeName; //DryDB.FacTypeList.Single(ftp => ftp.FacType == piping.FacType).FTpeCNS + DryDB.EndTypeList.Single(end => end.EndType == piping.FacType).EndTypeCNS;
                string sssl = endType.First().SS + " X " + endType.First().SL;// SS * SL

                List<SysMat> MatList = new List<SysMat>();
                foreach (var item in farMAT)
                {
                    FacSysMAT tmpmat = DryDB.FacSysMAT.Single(fsm => fsm.POMNo == item.POMNo);

                    SysMat tmpmatdata = new SysMat
                    {
                        description = tmpmat.Note,
                        itemunit = tmpmat.ItemUnit,
                        matname = tmpmat.MName,
                        module = GetModuleName(tmpmat.ModuleNo),
                        pomno = tmpmat.POMNo.ToString(),
                        spec = tmpmat.Spec,
                        matprice = GetMatPrice(tmpmat.POMNo, ApplyYear, ApplyUnit)
                    };
                    MatList.Add(tmpmatdata);
                }

                FSysDs tmpdata = new FSysDs
                {
                    facarea = buildArea,
                    farname = far.Name,
                    farsys = farsys,
                    ianum = ianum.ToString(),
                    sssl = sssl,
                    MatList = MatList
                };
                res.Add(tmpdata);
            }
            return res;
        }
        #endregion

        #region 取得 農戶設施系統 by MapNo
        /// <summary>
        /// 取得 農戶設施系統 by MapNo
        /// </summary>
        /// <param name="MapNo">版本編號</param>
        /// <returns>農戶設施系統物料</returns>
        public List<SysMat> GetFarSysviaMapNo(int MapNo)
        {
            Case cse = GetCaseDataFromMapNo(MapNo);
            if (!DryDB.FarmerSys.Any(Fs => Fs.MapNo == MapNo))
                return null;
            FarmerSys userSys = DryDB.FarmerSys.Single(Fs => Fs.MapNo == MapNo);
            List<FarmerSysM> fsmat = DryDB.FarmerSysM.Where(fsm => fsm.FarSysNo == userSys.FarSysNo)./*OrderBy(fsm => fsm.MatGroupID).ThenBy(fsm => fsm.MatOrder).*/ToList();
            List<MatGroup> groups = DryDB.MatGroup.ToList();
            List<SysMat> MatList = new List<SysMat>();
            
            foreach (var item in fsmat)
            {
                //FacSysMAT tmpmat = DryDB.FacSysMAT.Single(fsm => fsm.POMNo == item.POMNo);
                string spec = "";
                if (/*item.SpecName1.Length > 0 */item.Spec1 != null && item.Spec1 > 0)
                {
                    spec += "∮" + item.SpecName1;
                    if (/*item.SpecName2.Length > 0 */item.Spec2 != null && item.Spec2 > 0)
                    {
                        spec += "×" + item.SpecName2;

                    }
                    if (/*item.SpecName3.Length > 0 */item.Spec3 != null && item.Spec3 > 0)
                    {
                        spec += "×" + item.SpecName3;
                    }
                }
                else
                {
                    if (/*item.SpecName2.Length > 0 */item.Spec2 != null && item.Spec2 > 0)
                    {
                        spec +="∮"+ item.SpecName2;
                        if (/*item.SpecName3.Length > 0 */item.Spec3 != null && item.Spec3 > 0)
                        {
                            spec += "×" + item.SpecName3;
                        }
                    }
                    else
                    {
                        if (/*item.SpecName3.Length > 0 */item.Spec3 != null && item.Spec3 > 0)
                        {
                            spec +="∮"+ item.SpecName3;
                        }
                    }
                }
                int index = 1;
                SysMat tmpmatdata = new SysMat
                {
                    groupno = item.MatGroupID.HasValue ? item.MatGroupID.Value : 0,
                    group = item.MatGroupCNS,
                    order = item.MatOrder.HasValue ? item.MatOrder.Value : index,
                    description = item.Note,
                    itemunit = item.ItemUnit,
                    matname = item.MName,
                    moduleno = item.ModuleNo == null ? 0 : item.ModuleNo.Value,
                    module = item.ModuleCNS,
                    pomno = item.POMNo.ToString(),
                    spec = spec,
                    amount = item.Amount,
                    //matprice = GetMatPrice(item.POMNo, cse.ApplyYear, cse.ApplyUnit)
                    matprice = item.SysPrice
                };
                MatList.Add(tmpmatdata);
                index++;

            }
            return MatList;
        }
        #endregion

        #region 農戶設施系統 範本結構
        public class FSysDs
        {
            public string farname { get; set; }
            public string farsys { get; set; }
            public string ianum { get; set; }
            public int facarea { get; set; }
            public string sssl { get; set; }
            public List<SysMat> MatList { get; set; } 
        }

        public class SysMat
        {
            public int moduleno { get; set; }
            public int groupno { get; set; }
            public string group { get; set; }
            public int order { get; set; }
            public string module { get; set; }
            public string pomno { get; set; }
            public string matname { get; set; }
            public string spec { get; set; }
            public int amount { get; set; }
            public string itemunit { get; set; }
            public double matprice { get; set; }
            public string description { get; set; }
        }
        #endregion

        #region 取得地目名稱
        public LandTypeList GetLandTypeName(byte typecode)
        {
            return DryDB.LandTypeList.Single(lty => lty.LandType == typecode);
        }
        #endregion

        #region 判斷是否為變更設計
        /// <summary>
        /// 判斷是否為變更設計
        /// </summary>
        /// <param name="MapNo">版本編號</param>
        /// <returns></returns>
        public bool chgDesign(int MapNo)
        {
            bool isDesign = false;

            int evtno = DryDB.VerMapping.Single(ver => ver.MapNo == MapNo).EventNo;


            if (DryDB.VerMapping.Count(ver => ver.EventNo == evtno) > 1)
            {
                isDesign = true;
            }
            return isDesign;
        }
        #endregion

        /// <summary>
        /// 取得使用者代碼
        /// </summary>
        /// <param name="UserAccount"></param>
        /// <returns></returns>
        public Guid GetAdminID(string UserAccount)
        {
            return CommDB.Admin.Where(p => p.Account == UserAccount).ToList()[0].Admin_Id;
        }
        /// <summary>
        /// 取得使用者姓名
        /// </summary>
        /// <param name="AdminID"></param>
        /// <returns></returns>
        public string GetAdminName(Guid AdminID)
        {
            return CommDB.Admin.Find(AdminID).Name;
        }

        /// <summary>
        /// 透過版本編號取得農戶資料
        /// </summary>
        /// <param name="MapNo"></param>
        /// <returns></returns>
        public Farmer GetFarmerByMapNo(int MapNo)
        {
            int EventNO = GetEventNo(MapNo);
            Guid Fid = GetFIdByEvntNo(EventNO);
            return GetFarmer(Fid);
        }

        /// <summary>
        /// 透過版本編號及動力設備代碼取得動力設備價格
        /// </summary>
        /// <param name="MNO"></param>
        /// <param name="EngCode"></param>
        /// <returns></returns>
        public int GetEngPriceByMapNo(int MNO,byte EngCode)
        {
            Case cases = GetCaseDataFromMapNo(MNO);
            int price = 0;
            SubsidyLimit subsidylimit = DryDB.SubsidyLimit.Where(m => m.ApplyYear == cases.ApplyYear && m.ApplyUnit == cases.ApplyUnit).First();
            price = subsidylimit.EngLimit.Where(m => m.EngCode == EngCode).First().EngPrice;

            if (cases.Gold == true) {
       
                return (int)Math.Round((double)price * (double)subsidylimit.GoldPercent/100d, 0, MidpointRounding.AwayFromZero);
            }
            
            return price;
        }

        /// <summary>
        /// 透過黃金廊道動力設備價格轉為一標準價格
        /// </summary>
        /// <param name="MNO"></param>
        /// <param name="EngCode"></param>
        /// <returns></returns>
        public int GetEngPriceStdFromGold(int money)
        {
            int price = 0;
            switch (money)
            {
                case 5700:
                    price = 4000;
                    break;
                case 8500:
                    price = 6000;
                    break;
                case 15700:
                    price = 11000;
                    break;
                default :
                    price = money;
                    break;
            }

            return price;
        }


        /// <summary>
        /// 取得動力設備價格
        /// </summary>
        /// <param name="data"></param>
        /// <param name="EngCode"></param>
        /// <returns></returns>
        public int GetEngPriceByCase(Case data,byte EngCode)
        {
            return DryDB.SubsidyLimit.Where(m => m.ApplyYear == data.ApplyYear && m.ApplyUnit == data.ApplyUnit).First().EngLimit.Where(m => m.EngCode == EngCode).First().EngPrice;
        }
        
        /// <summary>
        /// 取得蓄水池價格
        /// </summary>
        /// <param name="data"></param>
        /// <param name="PtypeCode"></param>
        /// <param name="PoolTon"></param>
        /// <returns></returns>
        public int GetPoolPriceByCase(Case data,byte PtypeCode,int PoolTon)
        {
            SubsidyLimit subLimit = DryDB.SubsidyLimit.Where(m => m.ApplyYear == data.ApplyYear && m.ApplyUnit == data.ApplyUnit).First();
            if (PoolTon % 10 == 0)
            {
                if (!subLimit.PoolTonList.Where(m => m.PtypeCode == PtypeCode && m.PoolTon == PoolTon).Any())
                    return 0;
                else
                    return subLimit.PoolTonList.Where(m => m.PtypeCode == PtypeCode && m.PoolTon == PoolTon).First().PoolPrice;
            }else
            {
                int deltaTon = PoolTon % 10;
                int PoolTon1 = ((int)PoolTon / 10) * 10;
                int PoolTon2 = PoolTon1 + 10;
                var p1 = subLimit.PoolTonList.Where(m => m.PtypeCode == PtypeCode && m.PoolTon == PoolTon1).First();
                var p2 = subLimit.PoolTonList.Where(m => m.PtypeCode == PtypeCode && m.PoolTon == PoolTon2).First();
                if (p1 != null && p2 != null)
                {
                    return p1.PoolPrice + (((p2.PoolPrice - p1.PoolPrice) / 10) * deltaTon);
                }
                else return 0;
            }
            
            
            

        }
        /// <summary>
        /// 取得各水利會補助基準
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public SubsidyLimit GetSubsidyLimit(Case data)
        {
            return DryDB.SubsidyLimit.Where(m => m.ApplyYear == data.ApplyYear && m.ApplyUnit == data.ApplyUnit).First();
        }
        /// <summary>
        /// 取得田間管路補助基準
        /// </summary>
        /// <param name="data"></param>
        /// <param name="EndType"></param>
        /// <param name="FacType"></param>
        /// <returns></returns>
        public PigingLimit GetPigingLimit(Case data, byte EndType, byte FacType)
        {
            int LimitId = DryDB.SubsidyLimit.Where(n => n.ApplyUnit == data.ApplyUnit && n.ApplyYear == data.ApplyYear).First().LimitID;
            if (EndType == 1 || EndType == 7 || EndType == 8 || EndType == 6)
            {
                if (data.ApplyUnit == 17)
                {
                    return DryDB.PigingLimit.Where(m => m.LimitID == LimitId && m.EndType == EndType).FirstOrDefault();
                }
                else
                {
                    return DryDB.PigingLimit.Where(m => m.LimitID == LimitId && m.EndType == EndType).FirstOrDefault();
                }
            }
            else
            {
                if (data.ApplyUnit == 17)
                {
                    //return DryDB.PigingLimit.Where(m => m.LimitID == LimitId && m.EndType == EndType).FirstOrDefault();
                    return DryDB.PigingLimit.Where(m => m.LimitID == LimitId && m.EndType == EndType && m.FacType == FacType).FirstOrDefault();
                }
                else
                {
                    return DryDB.PigingLimit.Where(m => m.LimitID == LimitId && m.EndType == EndType && m.FacType == FacType).FirstOrDefault();
                }
            }

        }

        public ControlLimit GetControlLimit(Case data, int cntrlcode)
        {
            int LimitId = DryDB.SubsidyLimit.Where(n => n.ApplyUnit == data.ApplyUnit && n.ApplyYear == data.ApplyYear).First().LimitID;
            return DryDB.ControlLimit.Where(m => m.LimitID == LimitId && m.CntrlCode == cntrlcode).FirstOrDefault();
        }
        /// <summary>
        /// 取得補助款支出
        /// </summary>
        /// <param name="mno"></param>
        /// <returns></returns>
        public List<Pay> GetPay(int mno)
        {
            return DryDB.Pay.Where(m => m.MapNo == mno).ToList();
        }
        /// <summary>
        /// 從資料庫取得總經費
        /// </summary>
        /// <param name="mno"></param>
        /// <returns></returns>
        public TotalFee GetTotalFee(int mno)
        {
            return DryDB.TotalFee.Find(mno);
        }

        public double IdToSpec(int id)
        {
            MAT_Spec spec = DryDB.MAT_Spec.Find(id);
            if (spec != null)
                return spec.Spec;
            else
                return 0;
        }
        public int SpecToId(double spec)
        {
            MAT_Spec mat = DryDB.MAT_Spec.Where(m => m.Spec == spec).FirstOrDefault();
            if (mat != null)
                return mat.SpecNo;
            else
                return 29;
        }
        public int MatToId(string MatName)
        {
            if (MatName == null)
                return 1;
            var mat = DryDB.MAT_Types.Where(m => m.TypeName == MatName);
            if (mat != null)
                return mat.FirstOrDefault().TypeNo;
            else
                return 1;

        }
        #region 由案件MAPNO是否屬於山坡地或離島
        /// <summary>
        /// 取得案件是否屬於山坡地或離島
        /// </summary>
        /// <param name="mapno"></param>
        /// <returns></returns>
        public bool GetIs12FromMapNo(int mapno)
        {
            Case cases = GetCaseDataFromMapNo(mapno);
            return cases.Is12;
        }
        #endregion
        /// <summary>
        /// 取得案件是否屬於黃金廊道
        /// </summary>
        /// <param name="mapno"></param>
        /// <returns></returns>
        #region 由案件MAPNO是否屬於黃金廊道
        public bool GetGoldFromMapno(int mapno)
        {
            Case cases = GetCaseDataFromMapNo(mapno);
            return cases.Gold;
        }
        #endregion

        #region
        /// <summary>
        /// 取得案件原始mapno
        /// </summary>
        /// <param name="mapno"></param>
        /// <returns></returns>
        public int GetOrginMapno(int mapno)
        {
            int eventno = (from SummaryView in DryDB.SummaryView
                          where (SummaryView.MapNo == mapno)
                          select SummaryView.EventNo
                              ).ToList().First();
            return DryDB.VerMapping.Where(m => m.EventNo == eventno && m.ChgVer == 1).First().MapNo;
        } 

        #endregion
        /// <summary>
        /// 查詢案件編號是否重複
        /// </summary>
        /// <param name="Applyyear"></param>
        /// <param name="ApplyUnit"></param>
        /// <param name="IaNum"></param>
        /// <returns></returns>
        public bool IanumIsExist(int Applyyear,int ApplyUnit, int IaNum)
        {
            return DryDB.Case.Any(m => m.ApplyYear == Applyyear && m.ApplyUnit == ApplyUnit && m.IANum == IaNum && m.Enable == true) ;
        }
        /// <summary>
        /// 取得年度案件mapno集合
        /// </summary>
        /// <param name="Applyyear"></param>
        /// <param name="ApplyUnit"></param>
        /// <param name="unittype"></param>
        /// <returns></returns>
        public List<int?> getCaseNobyIa(int Applyyear, int ApplyUnit,string unittype)
        {
            
            var poolengList16 = (from a in DryDB.PoolApply
                               join b in DryDB.Pool on a.MapNo equals b.MapNo
                               where a.ApplyUnit == 16
                               select a.MapNo).Concat
                        (from a in DryDB.EngApply
                         join b in DryDB.Engine on a.MapNo equals b.MapNo
                         where a.ApplyUnit == 16 
                         select a.MapNo);
            var poolengList17 = (from a in DryDB.PoolApply
                                 join b in DryDB.Pool on a.MapNo equals b.MapNo
                                 where a.ApplyUnit == 17
                                 select a.MapNo).Concat
                        (from a in DryDB.EngApply
                         join b in DryDB.Engine on a.MapNo equals b.MapNo
                         where a.ApplyUnit == 17
                         select a.MapNo);
            var poolengListAll = (from a in poolengList16 select a).Concat(from a in poolengList17 select a);
            switch (unittype)
            {

                case "coa":
                    var caselist = from a in DryDB.SummaryView
                                   where a.ApplyYear == Applyyear && a.ApplyUnit == ApplyUnit && a.Gold == false
                                   && !poolengListAll.Contains(a.MapNo ?? 0)
                                   select a.MapNo;
                    return caselist.ToList();
                //break;
                case "16":
                    var caselist16 = from a in DryDB.SummaryView
                                   where a.ApplyYear == Applyyear && a.ApplyUnit == ApplyUnit && a.Gold == false
                                   && poolengList16.Contains(a.MapNo ?? 0)
                                   select a.MapNo;
                    return caselist16.ToList();
                case "17":
                    var caselist17 = from a in DryDB.SummaryView
                                     where a.ApplyYear == Applyyear && a.ApplyUnit == ApplyUnit && a.Gold == false
                                     && poolengList17.Contains(a.MapNo ?? 0)
                                     select a.MapNo;
                    return caselist17.ToList();
                case "gold":
                    var caselistgold = from a in DryDB.SummaryView
                                     where a.ApplyYear == Applyyear && a.ApplyUnit == ApplyUnit && a.Gold == true
                                     select a.MapNo;
                    return caselistgold.ToList();
                default:
                    return new List<int?>();
                    //break;
            }
        }
        /// <summary>
        /// 取得案件補助單位代碼
        /// </summary>
        /// <param name="mapno"></param>
        /// <returns></returns>
        public int getCaseAppliedUnit(int mapno)
        {
            if (DryDB.EngApply.Any(m => m.MapNo == mapno))
            {
                if (DryDB.EngApply.Where(m => m.MapNo == mapno).FirstOrDefault().ApplyUnit >=0)
                return DryDB.EngApply.Where(m => m.MapNo == mapno).FirstOrDefault().ApplyUnit;
            }
            else if (DryDB.PoolApply.Any(m => m.MapNo == mapno))
            {
                if (DryDB.PoolApply.Where(m => m.MapNo == mapno).FirstOrDefault().ApplyUnit >=0)
                return DryDB.PoolApply.Where(m => m.MapNo == mapno).FirstOrDefault().ApplyUnit;
            }
            else if (DryDB.FarmerSys.Any(m => m.MapNo == mapno))
            {
                if(DryDB.FarmerSys.Where(m => m.MapNo == mapno).FirstOrDefault().ApplyUnit >=0)
                return DryDB.FarmerSys.Where(m => m.MapNo == mapno).FirstOrDefault().ApplyUnit;
            }
            else if (DryDB.CntrlApply.Any(m => m.MapNo == mapno))
            {
                if (DryDB.CntrlApply.Where(m => m.MapNo == mapno).FirstOrDefault().ApplyUnit >= 0)
                return DryDB.CntrlApply.Where(m => m.MapNo == mapno).FirstOrDefault().ApplyUnit;
            }
            
            return -1;
        }
    }
}

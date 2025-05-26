using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Dry.Models.ViewModel
{
    public class SubsidyReportView
    {/// 補助清冊
        public int Pyears { get; set; }
        /// <summary>
        ///年度
        /// </summary>
        public int PNo { get; set; }
        /// <summary>
        /// 設施編號
        /// </summary>
        public string PName { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string PLocation { get; set; }
        /// <summary>
        /// 地點
        /// </summary>
        public string PArea { get; set; }
        /// <summary>
        /// 面積
        /// </summary>
        public string PPower { get; set; }
        /// <summary>
        /// 動力設備
        /// </summary>
        public string PPool { get; set; }
        /// <summary>
        /// 蓄水池
        /// </summary>
        public string PIrrigate1 { get; set; }
        /// <summary>
        /// 灌溉型式
        /// </summary>
        public string PIrrigate2 { get; set; }
        /// <summary>
        /// 灌溉型式
        /// </summary>
        public string PAddress { get; set; }
        /// <summary>
        /// 住址
        /// </summary>
        public string PPersonID { get; set; }
        /// <summary>
        /// 身分證字號
        /// </summary>
        public string PPhone { get; set; }
        /// <summary>
        /// 電話
        /// </summary>
        public string PMarks { get; set; }
        /// <summary>
        /// 備註
        /// </summary>
        public string PCount { get; set; }
        /// <summary>
        /// 總件數
        /// </summary>
        public string PFramCount { get; set; }
        /// <summary>
        /// 總農戶數
        /// </summary>
    }

    public class EngineeringReportView
    {/// 管路工程設施輔助
        public int EIA { get; set; }
        /// <summary>
        ///年度
        /// </summary>
        public int Eyears { get; set; }
        /// <summary>
        ///年度
        /// </summary>
        public int ENo { get; set; }
        /// <summary>
        /// 設施編號
        /// </summary>
        public string EName { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public float EArea { get; set; }
        /// <summary>
        /// 面積(公頃)
        /// </summary>
        public string ELocation { get; set; }
        /// <summary>
        /// 地點
        /// </summary>
        public string EIrrigation { get; set; }
        /// <summary>
        /// 灌溉型式
        /// </summary>
        public string EFarmer { get; set; }
        /// <summary>
        /// 農戶配合款
        /// </summary>
        public string Eendfacility { get; set; }
        /// <summary>
        /// 末端設施
        /// </summary>
        public string EWaterFacility { get; set; }
        /// <summary>
        /// 水源設施
        /// </summary>
        public string ERegulation { get; set; }
        /// <summary>
        /// 調控設施
        /// </summary>
        public string EWaterReservoir { get; set; }
        /// <summary>
        /// 蓄水池
        /// </summary>
        public string EPowerEquipment { get; set; }
        /// <summary>
        /// 動力設備
        /// </summary>
        public string EDesignCharges { get; set; }
        /// <summary>
        /// 設計費
        /// </summary>
        //public string ESubsidies { get; set; }
        ///// <summary>
        ///// 補助費
        ///// </summary>
        //public string EPercentage { get; set; }
        ///// <summary>
        ///// 百分比
        ///// </summary> 
        //public string ETotalCost { get; set; }
        ///// <summary>
        ///// 總工程費
        ///// </summary> 
        public string EFacilityA { get; set; }
        /// <summary>
        /// 穿孔管設施數量
        /// </summary> 
        public string EFacilityB { get; set; }
        /// <summary>
        /// 噴頭設施數量
        /// </summary> 
        public string EFacilityC { get; set; }
        /// <summary>
        /// 滴灌設施數量
        /// </summary> 
        public string EFacilityD { get; set; }
        /// <summary>
        /// 微噴設施數量
        /// </summary> 
        public string EFacilityE { get; set; }
        /// <summary>
        /// 軟管澆灌設施數量
        /// </summary> 
        public string EFacilityAreaA { get; set; }
        /// <summary>
        /// 穿孔管設施面積
        /// </summary> 
        public string EFacilityAreaB { get; set; }
        /// <summary>
        /// 噴頭設施面積
        /// </summary> 
        public string EFacilityAreaC { get; set; }
        /// <summary>
        /// 滴灌設施面積
        /// </summary> 
        public string EFacilityAreaD { get; set; }
        /// <summary>
        /// 微噴設施面積
        /// </summary> 
        public string EFacilityAreaE { get; set; }
        /// <summary>
        /// 軟管澆灌設施面積
        /// </summary> 
        public string EFacilityFarmerA { get; set; }
        /// <summary>
        /// 穿孔管設施農戶配合款
        /// </summary> 
        public string EFacilityFarmerB { get; set; }
        /// <summary>
        /// 噴頭設施農戶配合款
        /// </summary> 
        public string EFacilityFarmerC { get; set; }
        /// <summary>
        /// 滴灌設施農戶配合款
        /// </summary> 
        public string EFacilityFarmerD { get; set; }
        /// <summary>
        /// 微噴設施農戶配合款
        /// </summary> 
        public string EFacilityFarmerE { get; set; }
        /// <summary>
        /// 軟管澆灌設施農戶配合款
        /// </summary> 
        public string EFacilityendfacilityA { get; set; }
        /// <summary>
        /// 穿孔管末端設施款
        /// </summary> 
        public string EFacilityendfacilityB { get; set; }
        /// <summary>
        /// 噴頭末端設施款
        /// </summary> 
        public string EFacilityendfacilityC { get; set; }
        /// <summary>
        /// 滴灌末端設施款
        /// </summary> 
        public string EFacilityendfacilityD { get; set; }
        /// <summary>
        /// 微噴末端設施款
        /// </summary> 
        public string EFacilityendfacilityE { get; set; }
        /// <summary>
        /// 軟管澆灌末端設施款
        /// </summary> 
        public string EFacilityWaterFacilityA { get; set; }
        /// <summary>
        /// 穿孔管設施水源設施款
        /// </summary> 
        public string EFacilityWaterFacilityB { get; set; }
        /// <summary>
        /// 噴頭設施水源設施款
        /// </summary> 
        public string EFacilityWaterFacilityC { get; set; }
        /// <summary>
        /// 滴灌設施水源設施款
        /// </summary> 
        public string EFacilityWaterFacilityD { get; set; }
        /// <summary>
        /// 微噴設施水源設施款
        /// </summary> 
        public string EFacilityWaterFacilityE { get; set; }
        /// <summary>
        /// 軟管澆灌設施水源設施款
        /// </summary> 
        public string EFacilityRegulationA { get; set; }
        /// <summary>
        /// 穿孔管設施調控設施款
        /// </summary> 
        public string EFacilityRegulationB { get; set; }
        /// <summary>
        /// 噴頭設施調控設施款
        /// </summary> 
        public string EFacilityRegulationC { get; set; }
        /// <summary>
        /// 滴灌設施調控設施款
        /// </summary> 
        public string EFacilityRegulationD { get; set; }
        /// <summary>
        /// 微噴設施調控設施款
        /// </summary> 
        public string EFacilityRegulationE { get; set; }
        /// <summary>
        /// 軟管澆灌設施調控設施款
        /// </summary> 
        public string EFacilityWaterReservoirA { get; set; }
        /// <summary>
        /// 穿孔管設施蓄水池款
        /// </summary> 
        public string EFacilityWaterReservoirB { get; set; }
        /// <summary>
        /// 噴頭設施蓄水池款
        /// </summary> 
        public string EFacilityWaterReservoirC { get; set; }
        /// <summary>
        /// 滴灌設施蓄水池款
        /// </summary> 
        public string EFacilityWaterReservoirD { get; set; }
        /// <summary>
        /// 微噴設施蓄水池款
        /// </summary> 
        public string EFacilityWaterReservoirE { get; set; }
        /// <summary>
        /// 軟管澆灌設施蓄水池款
        /// </summary> 
        public string EFacilityPowerA { get; set; }
        /// <summary>
        /// 穿孔管設施動力設備款
        /// </summary> 
        public string EFacilityPowerB { get; set; }
        /// <summary>
        /// 噴頭設施動力設備款
        /// </summary> 
        public string EFacilityPowerC { get; set; }
        /// <summary>
        /// 滴灌設施動力設備款
        /// </summary> 
        public string EFacilityPowerD { get; set; }
        /// <summary>
        /// 微噴設施動力設備款
        /// </summary> 
        public string EFacilityPowerE { get; set; }
        /// <summary>
        /// 軟管澆灌設施動力設備款
        /// </summary> 
        public string EFacilityDesignA { get; set; }
        /// <summary>
        /// 穿孔管設施設計費
        /// </summary> 
        public string EFacilityDesignB { get; set; }
        /// <summary>
        /// 噴頭設施設計費
        /// </summary> 
        public string EFacilityDesignC { get; set; }
        /// <summary>
        /// 滴灌設施設計費
        /// </summary> 
        public string EFacilityDesignD { get; set; }
        /// <summary>
        /// 微噴設施設計費
        /// </summary> 
        public string EFacilityDesignE { get; set; }
        /// <summary>
        /// 軟管澆灌設施設計費
        /// </summary> 
        //public string EFacilitySubsidiesA { get; set; }
        ///// <summary>
        ///// 穿孔管設施補助費
        ///// </summary> 
        //public string EFacilitySubsidiesB { get; set; }
        ///// <summary>
        ///// 噴頭設施補助費
        ///// </summary> 
        //public string EFacilitySubsidiesC { get; set; }
        ///// <summary>
        ///// 滴灌設施補助費
        ///// </summary> 
        //public string EFacilitySubsidiesD { get; set; }
        ///// <summary>
        ///// 微噴設施補助費
        ///// </summary> 
        //public string EFacilitySubsidiesE { get; set; }
        ///// <summary>
        ///// 軟管澆灌設施補助費
        ///// </summary> 
        //public string EFacilityPercentageA { get; set; }
        ///// <summary>
        ///// 穿孔管設施百分比
        ///// </summary> 
        //public string EFacilityPercentageB { get; set; }
        ///// <summary>
        ///// 噴頭設施百分比
        ///// </summary> 
        //public string EFacilityPercentageC { get; set; }
        ///// <summary>
        ///// 滴灌設施百分比
        ///// </summary> 
        //public string EFacilityPercentageD { get; set; }
        ///// <summary>
        ///// 微噴設施百分比
        ///// </summary> 
        //public string EFacilityPercentageE { get; set; }
        ///// <summary>
        ///// 軟管澆灌設施百分比
        ///// </summary> 
        //public string EFacilityTotalA { get; set; }
        ///// <summary>
        ///// 穿孔管設施總工程費
        ///// </summary> 
        //public string EFacilityTotalB { get; set; }
        ///// <summary>
        ///// 噴頭設施總工程費
        ///// </summary> 
        //public string EFacilityTotalC { get; set; }
        ///// <summary>
        ///// 滴灌設施總工程費
        ///// </summary> 
        //public string EFacilityTotalD { get; set; }
        ///// <summary>
        ///// 微噴設施總工程費
        ///// </summary> 
        //public string EFacilityTotalE { get; set; }
        ///// <summary>
        ///// 軟管澆灌設施總工程費
        ///// </summary> 
    }

    public class EngineeringCostReportView
    {/// 管路工程設施面積及輔助金額統計表
        public int Rcountgroup { get; set; }
        /// <summary>
        ///群組筆數(多少個群組)
        /// </summary>
        public int Rdatacount { get; set; }
        /// <summary>
        ///群組資料筆數
        /// </summary>
        public int Ryears { get; set; }
        /// <summary>
        ///年度
        /// </summary>
        public int RNo { get; set; }
        /// <summary>
        /// 設施編號
        /// </summary>
        public string RIA { get; set; }
        /// <summary>
        /// 水利會
        /// </summary>
        public string RLocation { get; set; }
        /// <summary>
        /// 地點
        /// </summary>
         public string RIrrigation { get; set; }
        /// <summary>
        /// 灌溉型式
        /// </summary>
        public float RArea { get; set; }
        /// <summary>
        /// 面積(公頃)
        /// </summary>
        public float RUnit { get; set; }
        /// <summary>
        /// 戶數
        /// </summary>
        public string RFarmer { get; set; }
        /// <summary>
        /// 農戶配合款
        /// </summary>
        public string Rendfacility { get; set; }
        /// <summary>
        /// 末端設施
        /// </summary>
        public string RWaterReservoir { get; set; }
        /// <summary>
        /// 蓄水池
        /// </summary>
        public string RPowerEquipment { get; set; }
        /// <summary>
        /// 動力設備
        /// </summary>
        public string RDesignCharges { get; set; }
        /// <summary>
        /// 設計費
        /// </summary>
        public string RCaseNumber { get; set; }
        /// <summary>
        /// 合計數
        /// </summary>
        public string RLocationTotal { get; set; }
        /// <summary>
        /// 地點
        /// </summary>
        public string RIrrigationTotal { get; set; }
        /// <summary>
        /// 灌溉型式
        /// </summary>
        public float RAreaTotal { get; set; }
        /// <summary>
        /// 面積(公頃)
        /// </summary>
        public float RUnitTotal { get; set; }
        /// <summary>
        /// 戶數
        /// </summary>
        public string RFarmerTotal { get; set; }
        /// <summary>
        /// 農戶配合款
        /// </summary>
        public string RendfacilityTotal { get; set; }
        /// <summary>
        /// 末端設施
        /// </summary>
        public string RWaterReservoirTotal { get; set; }
        /// <summary>
        /// 蓄水池
        /// </summary>
        public string RPowerEquipmentTotal { get; set; }
        /// <summary>
        /// 動力設備
        /// </summary>
        public string RDesignChargesTotal { get; set; }
        /// <summary>
        /// 設計費
        /// </summary>
        public string RNumberTotal { get; set; }
        /// <summary>
        /// 設計費
        /// </summary>
        public string RCostTotal { get; set; }
        /// <summary>
        /// 設計費
        /// </summary>
    }

    public class StatementReportView
    {/// 管路灌溉設施工程決算表
        public int Syears { get; set; }
        /// <summary>
        ///年度
        /// </summary>
        public int SNo { get; set; }
        /// <summary>
        /// 申請案號
        /// </summary>
        public string SName { get; set; }
        /// <summary>
        /// 農戶姓名
        /// </summary>
        public string SAddress { get; set; }
        /// <summary>
        /// 住址
        /// </summary>
        public string SLocation { get; set; }
        /// <summary>
        /// 地段
        /// </summary>
        public string SNumber { get; set; }
        /// <summary>
        /// 地號
        /// </summary>
        public string SArea { get; set; }
        /// <summary>
        /// 設施面積
        /// </summary>
    }

    public class StatisticsReportView
    {/// 成果統計表
        public int Scountgroup { get; set; }
        /// <summary>
        ///群組筆數(多少個群組)
        /// </summary>
        public int Sdatacount { get; set; }
        /// <summary>
        ///群組資料筆數
        /// </summary>
        public int SIA { get; set; }
        /// <summary>
        ///水利會
        /// </summary>
        public int Syears { get; set; }
        /// <summary>
        ///年度
        /// </summary>
        public int SExtend { get; set; }
        /// <summary>
        /// 示範及推廣
        /// </summary>
        public int SCount { get; set; }
        /// <summary>
        /// 件數
        /// </summary>
        public int SIrrigation { get; set; }
        /// <summary>
        /// 灌溉型式
        /// </summary>
        public string SArea { get; set; }
        /// <summary>
        /// 設施面積
        /// </summary>
        public string SMoney { get; set; }
        /// <summary>
        /// 自備款
        /// </summary>
        public string SFacility { get; set; }
        /// <summary>
        /// 補助款
        /// </summary>
        public string SWaterReservoirT { get; set; }
        /// <summary>
        /// 蓄水槽(噸)
        /// </summary>
        public string SWaterReservoirS { get; set; }
        /// <summary>
        /// 蓄水槽(座)
        /// </summary>
        public string SHydro { get; set; }
        /// <summary>
        /// 抽水機
        /// </summary>
        public int SCountTotalA { get; set; }
        /// <summary>
        /// 穿孔管件數
        /// </summary>
        public int SCountTotalB { get; set; }
        /// <summary>
        /// 噴管件數
        /// </summary>
        public int SCountTotalC { get; set; }
        /// <summary>
        /// 微噴件數
        /// </summary>
        public int SCountTotalD { get; set; }
        /// <summary>
        /// 其他件數
        /// </summary>
        public int SCountTotalE { get; set; }
        /// <summary>
        /// 軟管澆灌件數
        /// </summary>
        public int SIrrigationTotal { get; set; }
        /// <summary>
        /// 灌溉型式
        /// </summary>
        public string SAreaTotalA { get; set; }
        /// <summary>
        /// 穿孔管設施面積
        /// </summary>
        public string SAreaTotalB { get; set; }
        /// <summary>
        /// 噴管設施面積
        /// </summary>
        public string SAreaTotalC { get; set; }
        /// <summary>
        /// 微噴設施面積
        /// </summary>
        public string SAreaTotalD { get; set; }
        /// <summary>
        /// 其他設施面積
        /// </summary>
        public string SAreaTotalE { get; set; }
        /// <summary>
        /// 軟管澆灌設施面積
        /// </summary>
        
        public string SMoneyTotalA { get; set; }
        /// <summary>
        /// 穿孔管自備款
        /// </summary>
        public string SMoneyTotalB { get; set; }
        /// <summary>
        /// 噴管自備款
        /// </summary>
        public string SMoneyTotalC { get; set; }
        /// <summary>
        /// 微噴自備款
        /// </summary>
        public string SMoneyTotalD { get; set; }
        /// <summary>
        /// 其他自備款
        /// </summary>
        public string SMoneyTotalE { get; set; }
        /// <summary>
        /// 軟管澆灌自備款
        /// </summary>
        
        public string SFacilityTotalA { get; set; }
        /// <summary>
        /// 穿孔管補助款
        /// </summary>
        public string SFacilityTotalB { get; set; }
        /// <summary>
        /// 噴管補助款
        /// </summary>
        public string SFacilityTotalC { get; set; }
        /// <summary>
        /// 微噴補助款
        /// </summary>
        public string SFacilityTotalD { get; set; }
        /// <summary>
        /// 其他補助款
        /// </summary>
        public string SFacilityTotalE { get; set; }
        /// <summary>
        /// 軟管澆灌補助款
        /// </summary>
        
        public string SWaterReservoirTTotalA { get; set; }
        /// <summary>
        /// 穿孔管蓄水槽(噸)
        /// </summary>
        public string SWaterReservoirTTotalB { get; set; }
        /// <summary>
        /// 噴管蓄水槽(噸)
        /// </summary>
        public string SWaterReservoirTTotalC { get; set; }
        /// <summary>
        /// 微噴蓄水槽(噸)
        /// </summary>
        public string SWaterReservoirTTotalD { get; set; }
        /// <summary>
        /// 其他蓄水槽(噸)
        /// </summary>
        public string SWaterReservoirTTotalE { get; set; }
        /// <summary>
        /// 軟管澆灌蓄水槽(噸)
        /// </summary>
        
        public string SWaterReservoirSTotalA { get; set; }
        /// <summary>
        /// 穿孔管蓄水槽(座)
        /// </summary>
        public string SWaterReservoirSTotalB { get; set; }
        /// <summary>
        /// 噴管蓄水槽(座)
        /// </summary>
        public string SWaterReservoirSTotalC { get; set; }
        /// <summary>
        /// 微噴蓄水槽(座)
        /// </summary>
        public string SWaterReservoirSTotalD { get; set; }
        /// <summary>
        /// 其他蓄水槽(座)
        /// </summary>
        public string SWaterReservoirSTotalE { get; set; }
        /// <summary>
        /// 軟管澆灌蓄水槽(座)
        /// </summary>
        
        public string SHydroTotalA { get; set; }
        /// <summary>
        /// 穿孔管抽水機
        /// </summary>
        public string SHydroTotalB { get; set; }
        /// <summary>
        /// 噴管抽水機
        /// </summary>
        public string SHydroTotalC { get; set; }
        /// <summary>
        /// 微噴抽水機
        /// </summary>
        public string SHydroTotalD { get; set; }
        /// <summary>
        /// 其他抽水機
        /// </summary>
        public string SHydroTotalE { get; set; }
        /// <summary>
        /// 軟管澆灌抽水機
        /// </summary>
        
    }

    public class FarmStatisticsReportView
    {/// 農作物統計表
        public int Fyears { get; set; }
        /// <summary>
        ///年度
        /// </summary>
        public int FIA { get; set; }
        /// <summary>
        ///水利會
        /// </summary>
        public int FTwon { get; set; }
        /// <summary>
        /// 鄉鎮
        /// </summary>
        public string FRoad { get; set; }
        /// <summary>
        /// 段別
        /// </summary>
        public string FCrop { get; set; }
        /// <summary>
        /// 作物名稱
        /// </summary>
        public string FIrrigation { get; set; }
        /// <summary>
        /// 灌溉型式
        /// </summary>
        public string FUnit { get; set; }
        /// <summary>
        /// 戶數
        /// </summary>
        public string FArea { get; set; }
        /// <summary>
        /// 設施面積
        /// </summary>
    }

    public class TownReportView
    {/// 歷年鄉鎮統計表
        public int TIA { get; set; }
        /// <summary>
        ///水利會
        /// </summary>
        public int TYears { get; set; }
        /// <summary>
        /// 年度
        /// </summary>
        public string TUnitA { get; set; }
        /// <summary>
        /// 台北市戶數
        /// </summary>
        public string TUnitB { get; set; }
        /// <summary>
        /// 新店市戶數
        /// </summary>
        public string TUnitC { get; set; }
        /// <summary>
        /// 石碇鄉戶數
        /// </summary>
        public string TUnitD { get; set; }
        /// <summary>
        /// 深坑鄉戶數
        /// </summary>
        public string TUnitE { get; set; }
        /// <summary>
        /// 坪林鄉戶數
        /// </summary>
        public string TAreaA { get; set; }
        /// <summary>
        /// 台北市面積
        /// </summary>
        public string TAreaB { get; set; }
        /// <summary>
        /// 新店市面積
        /// </summary>
        public string TAreaC { get; set; }
        /// <summary>
        /// 石碇鄉面積
        /// </summary>
        public string TAreaD { get; set; }
        /// <summary>
        /// 深坑鄉面積
        /// </summary>
        public string TAreaE { get; set; }
        /// <summary>
        /// 坪林鄉面積
        /// </summary>
    }

    public class FarmCountReportView
    {/// 農地筆數統計表
        public int Fyears { get; set; }
        /// <summary>
        ///年度
        /// </summary>
        public int FCount { get; set; }
        /// <summary>
        /// 申請案號
        /// </summary>
        public string FArea { get; set; }
        /// <summary>
        /// 設施面積
        /// </summary>
    }

    public class BenefitReportView
    {/// 歷年受益戶資料清查一覽表
        public int Bcountgroup { get; set; }
        /// <summary>
        ///群組筆數(多少個群組)
        /// </summary>
        public int Bdatacount { get; set; }
        /// <summary>
        ///群組資料筆數
        /// </summary>
        public int BIA { get; set; }
        /// <summary>
        ///水利會
        /// </summary>
        public int BYearS { get; set; }
        /// <summary>
        /// 年度起
        /// </summary>
        public string BYearE { get; set; }
        /// <summary>
        /// 年度迄
        /// </summary>
        public string BApplicationYear { get; set; }
        /// <summary>
        /// 申請年度
        /// </summary>
        public string BNo { get; set; }
        /// <summary>
        /// 編號
        /// </summary>
        public string BName { get; set; }
        /// <summary>
        /// 申請人
        /// </summary>
        public string BRoad { get; set; }
        /// <summary>
        /// 地段
        /// </summary>
        public string BRoadNumber { get; set; }
        /// <summary>
        /// 地號
        /// </summary>
        public string BPerson { get; set; }
        /// <summary>
        /// 土地所有權人
        /// </summary>
        public string BProportion { get; set; }
        /// <summary>
        /// 持分比例
        /// </summary>
        public string BLandArea { get; set; }
        /// <summary>
        /// 土地標示面積(平方公尺)
        /// </summary>
        public string BIAArea { get; set; }
        /// <summary>
        /// 水利會核定面積(公頃)
        /// </summary>
        public string BReason { get; set; }
        /// <summary>
        /// 登記原因
        /// </summary>
        public string BPersonArea { get; set; }
        /// <summary>
        /// 個人持有面積(平方公尺)
        /// </summary>
        public string BRange { get; set; }
        /// <summary>
        /// 權力範圍 
        /// </summary>
        public string BRegistration { get; set; }
        /// <summary>
        /// 登記日期
        /// </summary>
        public string BProvidedY { get; set; }
        /// <summary>
        /// 完整檢附(是)
        /// </summary>
        public string BProvidedN { get; set; }
        /// <summary>
        /// 完整檢視(否)
        /// </summary>
        public string BProvidedContent { get; set; }
        /// <summary>
        /// 檢附內容說明
        /// </summary>
        public string BDecision { get; set; }
        /// <summary>
        /// 加入意願
        /// </summary>
        public string BX { get; set; }
        /// <summary>
        /// X
        /// </summary>
        public string BY { get; set; }
        /// <summary>
        /// Y
        /// </summary>
        
    }

    public class SurveyReportView
    {/// 普查卡
        public int Scountgroup { get; set; }
        /// <summary>
        ///群組筆數(多少個群組)
        /// </summary>
        public int Sdatacount { get; set; }
        /// <summary>
        ///群組資料筆數
        /// </summary>
        public string SIA { get; set; }
        /// <summary>
        ///水利會
        /// </summary>
        public int SYearS { get; set; }
        /// <summary>
        /// 年度
        /// </summary>
        public string SNo { get; set; }
        /// <summary>
        /// 申請案號
        /// </summary>
        public string SApplicant { get; set; }
        /// <summary>
        /// 申請人
        /// </summary>
        public string SApplicantIDNumber { get; set; }
        /// <summary>
        /// 申請人身分證號碼
        /// </summary>
        public string SLandCount { get; set; }
        /// <summary>
        /// 申請施設土地總筆數
        /// </summary>
        public string SLandTotal { get; set; }
        /// <summary>
        /// 核定施設總面積(m2)
        /// </summary>
        public string SFinancialYears { get; set; }
        /// <summary>
        /// 補助年份
        /// </summary>
        public string SFarmer { get; set; }
        /// <summary>
        /// 農戶配合款
        /// </summary>
        public string SIAFinancial { get; set; }
        /// <summary>
        /// 水利會補助金額
        /// </summary>
        public string SConstruction { get; set; }
        /// <summary>
        /// 總工程費
        /// </summary>
        public string SApproval { get; set; }
        /// <summary>
        /// 核准字號
        /// </summary>
        public string SPpage { get; set; }
        /// <summary>
        /// 頁次
        /// </summary>
        public string SWokstation { get; set; }
        /// <summary>
        /// 工作站別 
        /// </summary>
        public string STeam { get; set; }
        /// <summary>
        /// 小組別
        /// </summary>
        public string SPower { get; set; }
        /// <summary>
        /// 動力設施
        /// </summary>
        public string SSave { get; set; }
        /// <summary>
        /// 調蓄設施
        /// </summary>
        public string SControl { get; set; }
        /// <summary>
        /// 調控設施
        /// </summary>
        public string SFinal { get; set; }
        /// <summary>
        /// 末端設施
        /// </summary>
        public string SLandNumber { get; set; }
        /// <summary>
        /// 地籍卡編號
        /// </summary>
        public string SLandLocation { get; set; }
        /// <summary>
        /// 地段
        /// </summary>
        public string SNumber { get; set; }
        /// <summary>
        /// 地號
        /// </summary>
        public string SLandArea { get; set; }
        /// <summary>
        /// 土地面積
        /// </summary>
        public string SApprovalArea { get; set; }
        /// <summary>
        /// 核定施設面積
        /// </summary>
        public string SName { get; set; }
        /// <summary>
        /// 姓名(底冊號碼)
        /// </summary>
        public string SIDNumber { get; set; }
        /// <summary>
        /// 身分證號碼
        /// </summary>
        public string SProportionA { get; set; }
        /// <summary>
        /// 持分比例
        /// </summary>
        public string SProportionB { get; set; }
        /// <summary>
        /// 持分比例
        /// </summary>
        public string SAddress { get; set; }
        /// <summary>
        /// 地政地址
        /// </summary>
        public string SPhone { get; set; }
        /// <summary>
        /// 聯絡電話
        /// </summary>
        public string SCertificate { get; set; }
        /// <summary>
        /// 其他所有權人同意施設証明書
        /// </summary> 
        public string STransactionReason { get; set; }
        /// <summary>
        /// 異動原因
        /// </summary> 
        public string STransactionData { get; set; }
        /// <summary>
        /// 異動日期
        /// </summary> 
    }

    public class ExamineReportView
    {/// 會員審核表
        public string EApplicant { get; set; }
        /// <summary>
        /// 申請人
        /// </summary> 
        public string EYears { get; set; }
        /// <summary>
        /// 設施完成年度
        /// </summary> 
    }
    public class LandReportView
    {/// 地籍卡
        public string LApplicant { get; set; }
        /// <summary>
        /// 申請人
        /// </summary> 
        public string LYears { get; set; }
        /// <summary>
        /// 設施完成年度
        /// </summary> 
    }
    public class PersonReportView
    {/// 基本資料卡
        public string LApplicant { get; set; }
        /// <summary>
        /// 申請人
        /// </summary> 
        public string LYears { get; set; }
        /// <summary>
        /// 設施完成年度
        /// </summary> 
    }
    public class AddressReportView
    {
        /// 住址標籤
        public string ZpiCode { get; set; }
        public string AAddress { get; set; }
        /// <summary>
        /// 地址
        /// </summary> 
        public string ACaseNumber { get; set; }
        /// <summary>
        /// 案號
        /// </summary> 
        public string AName { get; set; }
        /// <summary>
        /// 姓名
        /// </summary> 
    }

    public class DesignReceiptView
    {
        /// <summary>
        /// 年度
        /// </summary> 
        public string DYears { get; set; }
        /// <summary>
        /// 設計費
        /// </summary> 
        public string DesignPrice { get; set; }
        /// <summary>
        /// 中文money
        /// </summary> 
        public string ChineseMoney { get; set; }
        /// <summary>
        /// 設施編號
        /// </summary> 
        public string DNo { get; set; }
        public string IaName { get; set; }

    }
}

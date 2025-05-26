using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Dry.Models.ViewModel
{
    public class FarmerSysView
    {
        /// <summary>
        /// 申請單位
        /// </summary>
        public short ApplyUnit { get; set; }
        /// <summary>get and set FarmerSystem SS</summary>
        [Required(ErrorMessage = "必填")]
        public List<SelectListItem> pipDDL { get; set; }

        #region L1 data
        /// <summary>get and set FarmerSystem L1 Length</summary>
        [Required(ErrorMessage = "必填")]
        [Range(0, float.MaxValue, ErrorMessage = "超出範圍")]
        public double L1Len { get; set; }

        /// <summary>
        /// L1單價
        /// </summary>
        [Required(ErrorMessage = "必填")]
        public double L1Price { get; set; }
        public int L1Mat { get; set; }
        public int L1Spec { get; set; }

        /// <summary>get and set FarmerSystem L1 Mat Amount</summary>
        [Required(ErrorMessage = "必填")]
        //[Range(1, Int16.MaxValue, ErrorMessage = "數量超出範圍")]
        public short L1MatAmt { get; set; }
        #endregion

        #region L2 data
        /// <summary>get and set FarmerSystem L2 Length</summary>
        [Range(0, float.MaxValue, ErrorMessage = "超出範圍")]
        public double L2Len { get; set; }

        /// <summary>get and set FarmerSystem L2 Mat</summary>
        //[Range(1, Int32.MaxValue, ErrorMessage = "超出範圍")]
        public int L2Mat { get; set; }
        public double L2Spec { get; set; }

        /// <summary>
        /// L2單價
        /// </summary>
        [Required(ErrorMessage = "必填")]
        public double L2Price { get; set; }

        /// <summary>get and set FarmerSystem L2 Mat Amount</summary>
        [Required(ErrorMessage = "必填")]
        [Range(0, Int16.MaxValue, ErrorMessage = "超出範圍")]
        public short L2MatAmt { get; set; }
        #endregion
        /// <summary>
        /// 是否為修改狀態
        /// </summary>
        public bool IsModify { get; set; }
        /// <summary>
        /// 目前步驟
        /// </summary>
        public byte Step { get; set; }

        /// <summary>get and set FarmerSystem SS</summary>
        [Required(ErrorMessage = "必填")]
        [Range(0, float.MaxValue, ErrorMessage = "超出範圍")]
        public double SS { get; set; }

        /// <summary>
        /// 第二末端系統的SS
        /// </summary>
        public double SecondSS { get; set; }

        /// <summary>get and set FarmerSystem SL</summary>
        [Required(ErrorMessage = "必填")]
        [Range(0, float.MaxValue, ErrorMessage = "超出範圍")]
        public double SL { get; set; }

        /// <summary>
        /// 第二末端系統的SL
        /// </summary>
        public double SecondSL { get; set; }

        [Required(ErrorMessage = "必填")]
        [RegularExpression(@"^\d{1,4}x\d{1,4}$", ErrorMessage = "請輸入:長x寬")]
        public string Block { get; set; }

        /// <summary>
        /// 農地設施面積
        /// </summary>
        public int BuildArea { get; set; }

        /// <summary>
        /// 農地面積-長
        /// </summary>
        [Required(ErrorMessage = "必填")]
        [RegularExpression(@"^\d{1,4}$", ErrorMessage = "請輸入:長")]
        public int Length { get; set; }
        /// <summary>
        /// 農地面積-寬
        /// </summary>
        public int width { get; set; }
        #region DDL
        /// <summary>get and set Standard System DDL List</summary>
        public List<SelectListItem> StdSysDDL { get; set; }
        /// <summary>
        /// get and set Standard System by Amount DDL List
        /// </summary>
        public List<SelectListItem> StdSysByAmountDDL { get; set; }

        /// <summary>get and set System End Type DDL List</summary>
        public List<SelectListItem> EndTypeDDL { get; set; }

        /// <summary>
        /// L1材質DropDownList
        /// </summary>
        public List<SelectListItem> L1QualityDDL { get; set; }

        /// <summary>
        /// L1規格DropDownList
        /// </summary>
        public List<SelectListItem> L1SpecDDL { get; set; }



        /// <summary>
        /// L2材質DropDownList
        /// </summary>
        public List<SelectListItem> L2QualityDDL { get; set; }

        /// <summary>
        /// L2規格DropDownList
        /// </summary>
        public List<SelectListItem> L2SpecDDL { get; set; }



        /// <summary>
        /// 支管材質DropDownList
        /// </summary>
        public List<SelectListItem> BranchPipeMaterialDDL { get; set; }

        /// <summary>
        /// 支管規格DropDownList
        /// </summary>
        public List<SelectListItem> BranchPipeSpecDDL { get; set; }

        /// <summary>
        /// 噴頭材質DropDownList
        /// </summary>
        public List<SelectListItem> NozzleMaterialDDL { get; set; }

        /// <summary>
        /// 噴頭規格DropDownList
        /// </summary>
        public List<SelectListItem> NozzleSpecDDL { get; set; }

        /// <summary>
        /// 第二末端系統
        /// </summary>
        public List<SelectListItem> SecondEndTypeDDL { get; set; }

        /// <summary>get and set System Fac Type DDL List</summary>
        public List<SelectListItem> FacTypeDDL { get; set; }
        /// <summary>
        /// 第二末端系統-設施型式
        /// </summary>
        public List<SelectListItem> SecondFacTypeDDL { get; set; }

        /// <summary>get and set Unit List</summary>
        public List<SelectListItem> UnitDDL { get; set; }

        /// <summary>
        /// 滴灌系統類別DropDownList
        /// </summary>
        public List<SelectListItem> DropDDL { get; set; }

        /// <summary>
        /// 第二末端系統-滴灌系統類別DropDownList
        /// </summary>
        public List<SelectListItem> SecondDropDDL { get; set; }

        /// <summary>
        /// 噴頭系統類別DropDownList
        /// </summary>
        public List<SelectListItem> SprayDDL { get; set; }

        /// <summary>
        /// 第二末端系統-噴頭類別DropDownList
        /// </summary>
        public List<SelectListItem> SecondSprayDDL { get; set; }
        #endregion
        

        /// <summary>
        /// 第二末端系統是否顯示的CSS
        /// </summary>
        public string DisplayInfo { get; set; }
        /// <summary>
        /// 增加入二末端系統按鈕的顯示CSS
        /// </summary>
        public string DisplayInfoAdd { get; set; }
        public List<MatList> Mat { get; set; }
        public List<FarmerSysMat> FarmerSysMList { get; set; }

        [Range(0, 99, ErrorMessage = "請選擇")]
        public int ddl_FarmerSysUnit { get; set; }
        [RegularExpression(@"\b((?!-1)\w)+\b", ErrorMessage = "請選擇")]
        public int ddl_StdSys { get; set; }

        /// <summary>
        /// 公版系統DDL
        /// </summary>
        [RegularExpression(@"\b((?!-1)\w)+\b", ErrorMessage = "請選擇")]
        public int ddl_StdSysByAmount { get; set; }

        /// <summary>get and set System Mat List</summary>
        public List<FacSysMAT> SysMat { get; set; }

        public int PayMoney { get; set; }
        public int CaseMoney { get; set; }

        /// <summary>
        /// 灌溉水源DropDownList
        /// </summary>
       
        public List<SelectListItem> WaterSrcDDL { get; set; }

        /// <summary>豎管高度</summary>
        [Range(0, float.MaxValue, ErrorMessage = "超出範圍")]
        public float StdpipeHei { get; set; }
        public int StdpipeMat { get; set; }
        public int StdpipeSpec { get; set; }


        public int BranchPipeMaterial { get; set; }
        public int BranchPipeSpec { get; set; }
        //變徑
        public int Adjustable { get; set; }

        public int NozzleSpec { get; set; }
        public int NozzleType { get; set; }
        public int PerforatedPipe { get; set; }

        /// <summary>
        /// 第二末端系統-豎管高度
        /// </summary>
        public double SecondStdpipeHei { get; set; }


        /// <summary>
        /// 豎管規格DropDownList
        /// </summary>
        public List<SelectListItem> StdpipeSpecDDL { get; set; }

        /// <summary>
        /// 豎管材質DropDownList
        /// </summary>
        public List<SelectListItem> StdpipeMaterialDDL { get; set; }

        public List<SelectListItem> GroupDDL { get; set; }

        public class MatList
        {
            public string GroupName { get; set; }
            public int GroupNo { get; set; }
            public List<FarmerSysMat> List { get; set; }
        }

        /// <summary>
        /// 物料
        /// </summary>
        public class PriceJsonData
        {
            /// <summary>
            /// 系統物料代碼
            /// </summary>
            public int POMNo { get; set; }

            /// <summary>
            /// 數量
            /// </summary>
            public int Amt { get; set; }

            /// <summary>
            /// 單價
            /// </summary>
            public double Price { get; set; }

            public double TotalPrice { get; set; }
            public int Group { get; set; }
            public int Order { get; set; }
        }
        /// <summary>
        /// 田間主管
        /// </summary>
        public class MainJsonData
        {
            /// <summary>
            /// 長度
            /// </summary>
            public float Length { get; set; }

            /// <summary>
            /// 管別
            /// </summary>
            public string Mat { get; set; }

            public double Spec { get; set; }

            /// <summary>
            /// 單價
            /// </summary>
            public double LPrice { get; set; }

            /// <summary>
            /// 數量
            /// </summary>
            public short Amount { get; set; }
        }
        /// <summary>
        /// 田間管路系統資料
        /// </summary>
        public class ParaJsonData
        {
            /// <summary>
            /// 輔助單位
            /// </summary>
            public short Unit { get; set; }

            /// <summary>
            /// 坵塊型狀
            /// </summary>
            public string Block { get; set; }

            /// <summary>
            /// 設施總經費
            /// </summary>
            public int TotalPrice { get; set; }

            /// <summary>
            /// 公版設施系統代號
            /// </summary>
            public int FacNo { get; set; }

            /// <summary>
            /// 灌漑水源代碼
            /// </summary>
            public byte IrrWCode { get; set; }

            /// <summary>
            /// 物料
            /// </summary>
            public List<PriceJsonData> PriceJsonDataAry { get; set; }

            /// <summary>
            /// 田間主管
            /// </summary>
            public List<MainJsonData> MainJsonDataAry { get; set; }

            /// <summary>
            /// 末端型式資料
            /// </summary>
            public List<EndTypeStruct> EndTypeDataAry { get; set; }
        }


        public class StdMaterialStruct
        {
            /// <summary>
            /// 坵塊長度
            /// </summary>
            public int Length { get; set; }

            /// <summary>
            /// 坵塊寬度
            /// </summary>
            public int width { get; set; }

            /// <summary>
            /// 主管1-長度
            /// </summary>
            public float L1Len { get; set; }

            /// <summary>
            /// 主管1-價格
            /// </summary>
            public float L1Price { get; set; }

            /// <summary>
            /// 主管1-數量
            /// </summary>
            public short L1MatAmt { get; set; }

            /// <summary>
            /// 主管1-材質
            /// </summary>
            public int L1Material { get; set; }

            /// <summary>
            /// 田間主管1-規格
            /// </summary>
            public int L1Spec { get; set; }

            /// <summary>
            /// 田間主管2-長度
            /// </summary>
            public float L2Len { get; set; }

            /// <summary>
            /// 田間主管2-規格
            /// </summary>
            public short L2Price { get; set; }

            /// <summary>
            /// 田間主管2-數量
            /// </summary>
            public float L2MatAmt { get; set; }

            /// <summary>
            /// 主管2-材質
            /// </summary>
            public string L2Material { get; set; }

            /// <summary>
            /// 田間主管2-規格
            /// </summary>
            public int L2Spec { get; set; }

            /// <summary>
            /// 末端形式
            /// </summary>
            public int ddl_EndType { get; set; }

            /// <summary>
            /// 末端形式-噴頭
            /// </summary>
            public int ddl_Sprinkler { get; set; }

            /// <summary>
            /// 末端形式-滴灌
            /// </summary>
            public int ddl_Drop { get; set; }

            /// <summary>
            /// 設施形式
            /// </summary>
            public int ddl_FacType { get; set; }

            /// <summary>
            /// 灌溉水源
            /// </summary>
            public int ddl_WtaerSrc { get; set; }

            /// <summary>
            /// 支管-行距(SL)
            /// </summary>
            public double SL { get; set; }

            /// <summary>
            /// 支管-間距(SS)
            /// </summary>
            public double SS { get; set; }

            /// <summary>
            /// 支管-材質
            /// </summary>
            public int BranchMaterial { get; set; }

            /// <summary>
            /// 支管-規格
            /// </summary>
            public int BranchSpec { get; set; }

            /// <summary>
            /// 支管-變徑規格
            /// </summary>
            public int ChangeBranchSpec { get; set; }

            /// <summary>
            /// 噴頭-材質
            /// </summary>
            public int NozzleMaterial { get; set; }

            /// <summary>
            /// 噴頭-規格
            /// </summary>
            public int NozzleSpec { get; set; }

            /// <summary>
            /// 豎管高度
            /// </summary>
            public double StdpipeHei { get; set; }

            /// <summary>
            /// 豎管規格
            /// </summary>
            public int StdpipeSpec { get; set; }

            /// <summary>
            /// 豎管材質
            /// </summary>
            public int StdpipeMat { get; set; }
            public int PerforatedPipe { get; set; }
        }


        public class EndTypeStruct
        {
            /// <summary>
            /// 末端型式
            /// </summary>
            public byte Endtype { get; set; }


            /// <summary>
            /// 系統型式
            /// </summary>
            public byte Fac { get; set; }

            public string BranchPipeMaterial { get; set; }

            public double BranchPipeSpec { get; set; }

            /// <summary>
            /// 支管行距(SS)
            /// </summary>
            public double SS { get; set; }

            public double NozzleSpec { get; set; }
            public string NozzleType { get; set; }

            /// <summary>
            /// 噴頭間距(SL)
            /// </summary>
            public double SL { get; set; }

            /// <summary>
            /// 豎管高度
            /// </summary>
            public double StdpipeHei { get; set; }

            public string StdpipeMat { get; set; }

            public double StdpipeSpec { get; set; }
        }

        /// <summary>
        /// 田間管路系統物料表
        /// </summary>
        public class FarmerSysMat
        {
            /// <summary>
            /// 輔助單位
            /// </summary>
            public short ApplyUnit { get; set; }
            /// <summary>
            /// 公版設施系統流水號
            /// </summary>
            public int FacNo { get; set; }

            /// <summary>
            /// 農戶設施系統物料表代碼
            /// </summary>
            public int No { get; set; }
            /// <summary>
            /// 農設系統代號
            /// </summary>
            public string FarSysNo { get; set; }
            public int MatGroup { get; set; }
            public int MatOrder { get; set; }
            public string MatOrderCNS { get; set; }
            public string MatType { get; set; }
            /// <summary>
            /// 系統物料代碼
            /// </summary>
            public int POMNo { get; set; }
            /// <summary>
            /// 物料名稱
            /// </summary>
            public string MName { get; set; }
            /// <summary>
            /// 模組名稱
            /// </summary>
            public string ModuleCNS { get; set; }
            /// <summary>
            /// 規格1
            /// </summary>
            public string Spec1 { get; set; }
            /// <summary>
            /// 規格2
            /// </summary>
            public string Spec2 { get; set; }
            /// <summary>
            /// 規格3
            /// </summary>
            public string Spec3 { get; set; }
            /// <summary>
            /// 品項單位
            /// </summary>
            public string ItemUnit { get; set; }
            /// <summary>
            /// 備註
            /// </summary>
            public string Note { get; set; }
            /// <summary>
            /// 單價
            /// </summary>
            public float Price { get; set; }
            /// <summary>
            /// 數量
            /// </summary>
            public int Amount { get; set; }
            /// <summary>
            /// 總價
            /// </summary>
            public double TotalPrice { get; set; }
        }
        public class FarmerSysStruct
        {
            public string FarSysNo { get; set; }
            public int MapNo { get; set; }
            public int FacNo { get; set; }
            public string FacTypeName { get; set; }
            public int FacMoney { get; set; }
            public short ApplyUnit { get; set; }
            public string FacName { get; set; }
            public byte EndType { get; set; }
            public byte SubEndType { get; set; }
            public byte FacType { get; set; }
        }
        
    }
    /// <summary>
    /// 物料查詢資料結構
    /// </summary>
    public class SysMat
    {
        public string module { get; set; }
        public string pomno { get; set; }
        public string matname { get; set; }
        public string mattype { get; set; }
        public int matprice { get; set; }
        public string spec1 { get; set; }
        public string spec2 { get; set; }
        public string spec3 { get; set; }
        public string itemunit { get; set; }
        public string description { get; set; }
    }

    public class StdSysMat : FacSysMAT
    {
        public int Group { get; set; }
        public int Order { get; set; }
        /// <summary>
        /// 公版數量公式代碼
        /// </summary>
        public int AmountFormula { get; set; }

        /// <summary>
        /// 材料數量
        /// </summary>
        public int Amount { get; set; }
        /// <summary>
        /// 材料單價
        /// </summary>
        public double Price { get; set; }
        public string MatType { get; set; }
        public string Spec1 { get; set; }
        public string Spec2 { get; set; }
        public string Spec3 { get; set; }

    }
}

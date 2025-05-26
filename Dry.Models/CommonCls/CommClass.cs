/*
-- =============================================
-- Author:  HaoHsuan
-- Create date: 2014-2-11
-- Description: 常被引用的程式
-- =============================================
*/

using Dry.Models.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using AERC.Models.CommonCls;

namespace Dry.Models.CommonCls
{
    public class CommClass
    {
        #region 取得 補助方案 DropDownList
        public List<SelectListItem> GetUnitList(int MapNo)
        {
            List<SelectListItem> unit = new List<SelectListItem>();
            foreach (var item in GetApplyUnit())
            {                
                unit.Add(new SelectListItem() { Text = item.Text, Value = item.Idx.ToString() });
            }
            return unit;
        }
        #endregion

        #region 取得 指定索引的補助方案DropDownList
        public List<SelectListItem> GetUnitListByIdx(short idx, int MapNo)
        {
            List<DDL> ddl = GetApplyUnit();
            List<SelectListItem> unit = new List<SelectListItem>();
            
            foreach (var item in ddl)
            {
                if (item.Idx.Equals(idx))
                {
                    unit.Add(new SelectListItem() { Text = item.Text, Value = item.Idx.ToString(), Selected = true });
                }
                else
                {
                    unit.Add(new SelectListItem() { Text = item.Text, Value = item.Idx.ToString() });
                }
            }
            return unit;
        }
        #endregion

        #region 取得 動力設施 DropDownList
        public List<SelectListItem> GetEngList()
        {
            Dry.Models.Service.EngineDBService DBService = new Dry.Models.Service.EngineDBService();
            List<SelectListItem> item = new List<SelectListItem>();

            foreach (var eng in DBService.GetEngineList())
            {
                item.Add(new SelectListItem()
                {
                    Text = eng.EngCNS,
                    Value = eng.EngCode.ToString()
                });
            }
            return item;
        }
        #endregion

        #region 取得 蓄水池材質 DropDownList
        /// <summary>
        /// 取得 蓄水池材質 DropDownList
        /// </summary>
        /// <param name="unit">指定水利會</param>
        /// <returns></returns>
        public List<SelectListItem> GetPoolTyeList(short unit = -1)
        {
            PoolDBService DBService = new PoolDBService();
            List<SelectListItem> item = new List<SelectListItem>();

            if (unit == 170)
            {
                foreach (var pool in DBService.GetPoolTypeList())
                {
                    //if (pool.PtypeCode > 3)
                    //{
                        item.Add(new SelectListItem()
                        {
                            Text = pool.PtypeCNS,
                            Value = pool.PtypeCode.ToString()
                        });
                    //}
                }
            }
            else
            {
                foreach (var pool in DBService.GetPoolTypeList())
                {
                    if (pool.PtypeCode == 6)
                    {
                        break;
                    }
                    else
                    {
                        item.Add(new SelectListItem()
                        {
                            Text = pool.PtypeCNS,
                            Value = pool.PtypeCode.ToString()
                        });
                    }
                }
            }
            return item;
        }
        #endregion

        #region 取得 蓄水池噸數 DropDownList
        /// <summary>
        /// 取得 蓄水池噸數 DropDownList
        /// </summary>
        /// <param name="unit">指定水利會</param>
        /// <returns></returns>
        public List<SelectListItem> GetPoolWeiDDL(short unit = -1)
        {
            List<SelectListItem> item = new List<SelectListItem>();

            if (unit > 0 && unit == 170)
            {
                for (int wei = 1; wei <= 6; wei++)
                {
                    if(wei == 10 )
                    {
                        item.Add(new SelectListItem()
                        {
                            Text = wei.ToString() + " 噸",
                            Value = wei.ToString(),
                            Selected = true

                        });
                    }
                    else
                    {
                        item.Add(new SelectListItem()
                        {
                            Text = wei.ToString() + " 噸",
                            Value = wei.ToString(),
                            Selected = false

                        });
                    }
                    
                    if (wei == 3)
                        wei++;
                    
                }
            }

            for (int wei = 10; wei <= 100; wei += 10)
            {
                item.Add(new SelectListItem()
                {
                    Text = wei.ToString() + " 噸",
                    Value = wei.ToString()
                });
            }
            return item;
        }
        #endregion

        #region 取得 調控設施類別 DropDownList
        public List<SelectListItem> GetCntrlList()
        {
            Dry.Models.Service.CtrlDBService DBService = new Dry.Models.Service.CtrlDBService();
            List<SelectListItem> item = new List<SelectListItem>();
            item.Add(new SelectListItem() { Text = "請選擇", Value = "-1" });
            foreach (var category in DBService.GetCntrListDataList())
            {
                item.Add(new SelectListItem()
                {
                    Text = category.CntrlCNS.ToString(),
                    Value = category.CntrlCode.ToString()
                });
            }
            return item;
        }
        #endregion

        #region 取得勘察人員 DropDownList
        public List<SelectListItem> GetExamineMen(string Account)
        {
            GetData getData = new GetData();
            Guid AdminID = getData.GetAdminID(Account);
            List<SelectListItem> item = new List<SelectListItem>();
            item.Add(new SelectListItem() { Text = "請選擇", Value = "00000000-0000-0000-0000-000000000000" });
            foreach (var category in getData.GetExamineAdmin(AdminID))
            {
                item.Add(new SelectListItem()
                {
                    Text = category.Name,
                    Value = category.Admin_Id.ToString()
                });
            }
            return item;
        }
        public List<SelectListItem> GetExamineMen(Guid AdminID)
        {
            GetData getData = new GetData();
            List<SelectListItem> item = new List<SelectListItem>();
            item.Add(new SelectListItem() { Text = "請選擇", Value = "00000000-0000-0000-0000-000000000000" });
            foreach (var category in getData.GetExamineAdmin(AdminID))
            {
                item.Add(new SelectListItem()
                {
                    Text = category.Name,
                    Value = category.Admin_Id.ToString()
                });
            }
            return item;
        }
        #endregion

        #region 取得指定勘察人員 DropDownList
        public List<SelectListItem> GetExamineMenById(Guid AdminID, Guid PersonnelID)
        {
            GetData getData = new GetData();
            List<SelectListItem> item = new List<SelectListItem>();
            //item.Add(new SelectListItem() { Text = "請選擇", Value = "-1" });
            foreach (var category in getData.GetExamineAdmin(AdminID))
            {
                if (category.Admin_Id.Equals(PersonnelID))
                {
                    item.Add(new SelectListItem()
                    {
                        Text = category.Name,
                        Value = category.Admin_Id.ToString(),
                        Selected = true
                    });
                }
                else
                {
                    item.Add(new SelectListItem()
                    {
                        Text = category.Name,
                        Value = category.Admin_Id.ToString()
                    });
                }
            }
            return item;
        }
        #endregion
        #region 取得設計人員 DropDownList
        /// <summary>
        /// 取得設計人員 DropDownList
        /// </summary>
        /// <param name="UnitID"></param>
        /// <param name="UserName"></param>
        /// <returns></returns>
        public List<SelectListItem> GetDesinerList(short UnitID, string UserName)
        {
            GetData getData = new GetData();
            List<SelectListItem> data = new List<SelectListItem>();
            data.Add(new SelectListItem() { Text = "請選擇", Value = "00000000-0000-0000-0000-000000000000" });
            foreach (var item in getData.GetDesignerList(UnitID))
            {
                if (item.Designer_Name.Equals(UserName))
                {
                    data.Add(new SelectListItem()
                    {
                        Text = item.Designer_Name,
                        Value = item.Designer_Id.ToString(),
                        Selected = true
                    });
                }
                else
                {
                    data.Add(new SelectListItem()
                    {
                        Text = item.Designer_Name,
                        Value = item.Designer_Id.ToString()
                    });
                }
            }
            return data;
        }
        #endregion
        

        #region 取得 公版系統 DropDownList
        public List<SelectListItem> GetStdSysList(short UnitID)
        {
            GetData getData = new GetData();
            List<SelectListItem> item = new List<SelectListItem>();
            item.Add(new SelectListItem() { Text = "請選擇", Value = "-1" });
            foreach (var std in getData.GetStdSysByUnit(UnitID))
            {
                item.Add(new SelectListItem()
                {
                    Text = std.FacTypeName + "(" + std.FacName + ")",
                    Value = std.FacNo.ToString()
                });
            }
            return item;
        }
        public List<SelectListItem> GetStdSysList(byte EndType, short UnitID)
        {
            GetData getData = new GetData();
            List<SelectListItem> item = new List<SelectListItem>();
            item.Add(new SelectListItem() { Text = "請選擇", Value = "-1" });
            foreach (var std in getData.GetStdSysByEndType(EndType, UnitID))
            {
                item.Add(new SelectListItem()
                {
                    Text = std.FacTypeName + "(" + std.FacName + ")",
                    Value = std.FacNo.ToString()
                });
            }
            return item;
        }
        #endregion

        #region 取得 驗收狀況 DropDownList
        public List<SelectListItem> GetStatusList(string selectval = "-1")
        {
            GetData getData = new GetData();
            var list = getData.GetFacStatus();
            List<SelectListItem> item = new List<SelectListItem>();
            item.Add(new SelectListItem() { Text = "請選擇", Value = "-1" });
            foreach (var status in list)
            {
                if (selectval.Equals(status.statCode.ToString()))
                {
                    item.Add(new SelectListItem()
                    {
                        Text = status.statCNS,
                        Value = status.statCode.ToString(),
                        Selected = true
                    });
                }
                else
                {
                    item.Add(new SelectListItem()
                    {
                        Text = status.statCNS,
                        Value = status.statCode.ToString()
                    });
                }
            }
            return item;
        }
        #endregion

        #region 補助項目類別
        private List<DDL> GetApplyUnit()
        {
            List<DDL> ddl = new List<DDL>();
            ddl.Add(new DDL("請選擇", -1));
            ddl.Add(new DDL("農田水利署", 0));
            ddl.Add(new DDL("七星管理處", 16));
            ddl.Add(new DDL("瑠公管理處", 17));
            return ddl;
        }

        /// <summary>
        /// 判別是否為黃金廊道
        /// </summary>
        /// <param name="MapNo"></param>
        /// <returns></returns>
        public bool IsGold(int MapNo)
        {
            GetData getData = new GetData();
            return getData.GetCaseDataFromMapNo(MapNo).Gold;
        }

        private class DDL
        {
            private string _Text;
            private short _Idx;

            public DDL(string txt, short idx)
            {
                _Text = txt;
                _Idx = idx;
            }

            public string Text { get { return this._Text; } }

            public short Idx { get { return this._Idx; } }
        }

        #endregion

        #region 更新資料
        /// <summary>
        /// 更新目前步驟
        /// </summary>
        /// <param name="MapNo">版本編號</param>
        /// <param name="Step">目前步驟</param>
        /// <returns></returns>
        public DBCommon.DbEvent UpdateCase(int MapNo, byte Step, bool Status = false)
        {
            GetData getData = new GetData();
            DryEntities DryDB = new DryEntities();
            DBCommon.DbEvent dbstatus = new DBCommon.DbEvent();

            #region Special case
            Case caseDB = new GetData().GetCaseDataFromMapNo(MapNo);

            Case newCase = new Case();
            newCase.CId = caseDB.CId;
            newCase.CDate = caseDB.CDate;
            newCase.UDate = caseDB.UDate;
            newCase.EventNo = caseDB.EventNo;
            newCase.FId = caseDB.FId;
            newCase.ApplyYear = caseDB.ApplyYear;
            newCase.ApplyUnit = caseDB.ApplyUnit;
            newCase.Gold = caseDB.Gold;
            newCase.IANum = caseDB.IANum;
            newCase.Step = Step;
            newCase.Complete = Status;
            newCase.UDate = DateTime.Now;
            newCase.Enable = true;
            #endregion

            try
            {
                DryDB = new DryEntities();
                DryDB.Case.Attach(newCase);
                DryDB.Entry(newCase).State = System.Data.EntityState.Modified;
                DryDB.SaveChanges();
                dbstatus.DbMessage = "Success";
            }
            catch (Exception e)
            {
                dbstatus.DbMessage = "Modified Case Failed : " + e.Message + "<br>";
            }
            return dbstatus;
        }
        #endregion

        #region 取得所有物料模組DropDownList
        /// <summary>
        /// 取得所有物料模組DropDownList
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetModuleList()
        {
            DryEntities dry = new DryEntities();
            List<Mat_Module> list = dry.Mat_Module.ToList();
            List<SelectListItem> SelItem = new List<SelectListItem>();

            foreach (var item in list)
            {
                SelItem.Add(new SelectListItem()
                    {
                        Text = item.ModuleCNS,
                        Value = item.ModuleNo.ToString(),
                    });
            }
            return SelItem;
        }
        #endregion


        #region 載入縣市DropDownList
        /// <summary>
        /// 載入縣市DropDownList
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetCityDDL()
        {
            GetTownData getTownData = new GetTownData();
            List<SelectListItem> items = new List<SelectListItem>();
            var CityList = getTownData.GetCityList();
            items.Add(new SelectListItem() { Text = "縣市", Value = "-1" });
            foreach (var item in CityList)
            {
                items.Add(new SelectListItem()
                {
                    Text = item.City1,
                    Value = item.City_Code
                });
            }
            return items;
        }
        #endregion

        #region 載入鄉鎮市DropDownList
        /// <summary>
        /// 載入鄉鎮市DropDownList
        /// </summary>
        /// <param name="CityCode">縣市代碼</param>
        /// <returns></returns>
        public List<SelectListItem> GetTownDDL(string CityCode)
        {
            GetTownData getTownData = new GetTownData();
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem() { Text = "鄉鎮市區", Value = "-1" });
            var TownList = getTownData.GetTownList(CityCode.ToUpper());
            if (CityCode != "-1")
            {
                foreach (var item in TownList)
                {
                    items.Add(new SelectListItem()
                    {
                        Text = item.Town1,
                        Value = item.Town_Id.ToString()
                    });
                }
            }
            return items;
        }
        #endregion

        #region 載入地政事務所DropDownList
        /// <summary>
        /// 載入地政事務所DropDownList
        /// </summary>
        /// <param name="CityCode">縣市代碼</param>
        /// <returns></returns>
        public List<SelectListItem> GetLandOfficeDDL(string CityCode)
        {
            var OriDataList = new AERC.Models.CommonCls.GetTownData().GetLandOfficeByCityCode(CityCode);
            List<SelectListItem> DataList = new List<SelectListItem>();
            foreach(var item in OriDataList)
            {
                DataList.Add(new SelectListItem()
                {
                    Text = item.Land_Office_Code + " - " + item.Land_Office1,
                    Value = item.Land_Office_Id.ToString()
                });
            }
            return DataList;
        }
        #endregion
        
        /// <summary>
        /// 載入地段
        /// </summary>
        /// <param name="TownId"></param>
        /// <returns></returns>
        public List<SelectListItem> GetSectionDDL(string TownId)
        {

            List<SelectListItem> Data = new List<SelectListItem>();
            Data.Add(new SelectListItem() { Text = "地段", Value = "-1" });
            if (TownId != "")
            {
                List<AERC.Models.Section> List = new AERC.Models.CommonCls.GetSectionData().GetSectionList(Convert.ToInt16(TownId));
                foreach (var item in List)
                {
                    if (item.Subsection.Length <= 0)
                    {
                        Data.Add(new SelectListItem()
                        {
                            Text = item.Section1,
                            Value = item.Section_Id.ToString()
                        });
                    }
                    else
                    {
                        Data.Add(new SelectListItem()
                        {
                            Text = item.Section1 + " - " + item.Subsection,
                            Value = item.Section_Id.ToString()
                        });
                    }

                }
            }

            return Data;
        }
        /// <summary>
        /// 依版本編號取得申請年度
        /// </summary>
        /// <param name="MapNo"></param>
        /// <returns></returns>
        public byte GetApplyYear(int MapNo)
        {
            DryEntities dryDB = new DryEntities();
            var verMapping = dryDB.VerMapping.ToList();
            var cases = dryDB.Case.ToList();
            var EYear = (from vermapping in verMapping
                         join Cases in cases on vermapping.EventNo equals Cases.EventNo
                         where vermapping.MapNo == MapNo
                         select new { Cases.ApplyYear }).ToList();
            return EYear[0].ApplyYear;
        }
        /// <summary>
        /// 取得申請案件的年度列表
        /// </summary>
        /// <param name="UnitID">單位代碼</param>
        /// <returns></returns>
        public List<SelectListItem> GetApplyYearByUnit(short UnitID)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem() { Text = "歷年", Value = "0" });
            
            DryEntities dry = new DryEntities();
            var YearList = (from data in dry.Case where data.ApplyUnit.Equals(UnitID) && data.Enable == true select new { data.ApplyYear }).Distinct().OrderBy(o=> o.ApplyYear).ToList();
            byte year = YearList.Count > 0 ? (byte)YearList.Max(m => m.ApplyYear) : (byte)0; 
            if (UnitID == 99 || UnitID == 0 || UnitID == 20)
            {
                YearList = (from data in dry.Case select new { data.ApplyYear }).Distinct().OrderBy( d=> d.ApplyYear).ToList();
                year = YearList.LastOrDefault().ApplyYear;
            }
            
            foreach (var item in YearList)
            {
                var content = item.ApplyYear.ToString();
                items.Add(new SelectListItem()
                {
                    Text = content + "年",
                    Value = content,
                    Selected = item.ApplyYear == year ? true : false
                });
            }
            return items;
        }

        /// <summary>
        /// 取得物料材質DropDownList
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetQuality()
        {
            DryEntities DryDB = new DryEntities();
            List<MAT_Types> MatType = DryDB.MAT_Types.ToList();
            List<SelectListItem> Data = new List<SelectListItem>();
            foreach (var item in MatType)
            {
                Data.Add(new SelectListItem
                {
                    Text = item.TypeName,
                    Value = item.TypeNo.ToString()
                });
            }
            return Data;
        }

        /// <summary>
        /// 取得物料規格DropDownList
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetSpecDDL()
        {
            DryEntities DryDB = new DryEntities();
            List<MAT_Spec> SpecList = DryDB.MAT_Spec.OrderBy(m => m.Spec).ToList();
            List<SelectListItem> Data = new List<SelectListItem>();
            Data.Add(new SelectListItem
            {
                Text = "無",
                Value = "0"
            });
            foreach(var item in SpecList)
            {
                //if(item.Spec == 1)
                //{
                //    Data.Add(new SelectListItem
                //        {
                //            Text = item.SpecName,
                //            Value = item.SpecNo.ToString()/*,
                //            Selected = true*/
                //        });
                //}
                //else
                //{
                //    Data.Add(new SelectListItem
                //    {
                //        Text = item.SpecName,
                //        Value = item.SpecNo.ToString()
                //    });
                //}
                Data.Add(new SelectListItem
                {
                    Text = item.SpecName,
                    Value = item.SpecNo.ToString()
                });

            }
            return Data;
        }

        public List<SelectListItem> GetNozzleSpecByMatDDL(short BUnit, int ModuleNo)
        {
            DryEntities DryDB = new DryEntities();
            var data = (from facmat in DryDB.FacSysMAT
                        join matspec in DryDB.MAT_Spec on facmat.SpecNo1 equals matspec.SpecNo
                        where facmat.Bunit == BUnit && facmat.ModuleNo == ModuleNo

                        orderby matspec.Spec descending
                        select new
                        {
                            facmat.SpecNo1,
                            matspec.SpecName
                        }).Distinct();
            List<SelectListItem> Data = new List<SelectListItem>();
            foreach (var item in data)
            {
                Data.Add(new SelectListItem
                {
                    Text = item.SpecName,
                    Value = item.SpecNo1.ToString()
                });
            }
            return Data;
        }

        public List<SelectListItem> GetNozzleSpecByDrop(short BUnit,int BranchMatType)
        {
            DryEntities DryDB = new DryEntities();
            List<SelectListItem> Data = new List<SelectListItem>();
            if (BranchMatType == 1 || BranchMatType == 5)
            {
                var data = (from facmat in DryDB.FacSysMAT
                            join matspec in DryDB.MAT_Spec on facmat.SpecNo1 equals matspec.SpecNo
                            where facmat.Bunit == BUnit && facmat.ModuleNo == 1 && facmat.MatType == BranchMatType
                            orderby matspec.Spec descending
                            select new
                            {
                                facmat.SpecNo1,
                                matspec.SpecName
                            }).Distinct();
                foreach (var item in data)
                {
                    Data.Add(new SelectListItem
                    {
                        Text = item.SpecName,
                        Value = item.SpecNo1.ToString()
                    });
                }
            }
            else
            {
                var data = (from facmat in DryDB.FacSysMAT
                            join matspec in DryDB.MAT_Spec on facmat.SpecNo1 equals matspec.SpecNo
                            where facmat.Bunit == BUnit && facmat.ModuleNo == 1
                            orderby matspec.Spec descending
                            select new
                            {
                                facmat.SpecNo1,
                                matspec.SpecName
                            }).Distinct();
                foreach (var item in data)
                {
                    Data.Add(new SelectListItem
                    {
                        Text = item.SpecName,
                        Value = item.SpecNo1.ToString()
                    });
                }
            }


            return Data;
        }

        public List<SelectListItem> GetMatListByModelAndSpec(short unit,int ModuleNo, int Spec)
        {
            DryEntities DryDB = new DryEntities();
            var data = (from facmat in DryDB.FacSysMAT
                        join mattype in DryDB.MAT_Types on facmat.MatType equals mattype.TypeNo
                        join matspec in DryDB.MAT_Spec on facmat.SpecNo1 equals matspec.SpecNo
                        where facmat.ModuleNo == ModuleNo && facmat.SpecNo1 == Spec && facmat.Bunit == unit
                        select new
                        {
                            facmat.POMNo,
                            facmat.MName,
                            mattype.TypeName,
                            matspec.SpecName
                        });
            List<SelectListItem> Data = new List<SelectListItem>();
            foreach (var item in data)
            {
                Data.Add(new SelectListItem
                {
                    Text = item.MName + "-" + item.TypeName + "(" + item.SpecName + ")",
                    Value = item.POMNo.ToString()
                });
            }
            return Data;
        }
        public List<SelectListItem> GetNozzleMatListBySpec(int Spec,int EndType, short unit)
        {
            DryEntities DryDB = new DryEntities();
            int ModuleNo = 0;
            if (EndType == 1)
            {
                ModuleNo = 6;
            }
            else if (EndType == 2 || EndType == 6)
            {
                ModuleNo = 5;
            }
            else if (EndType == 3)
            {
                ModuleNo = 8;
            }
            else if (EndType == 7)
            {
                ModuleNo = 9;
            }
            else if (EndType == 8)
            {
                ModuleNo = 12;
            }
            var data = (from facmat in DryDB.FacSysMAT
                        join mattype in DryDB.MAT_Types on facmat.MatType equals mattype.TypeNo
                        join matspec in DryDB.MAT_Spec on facmat.SpecNo1 equals matspec.SpecNo
                        where facmat.ModuleNo == ModuleNo && facmat.SpecNo1 == Spec && facmat.Bunit == unit
                        select new
                        {
                            facmat.POMNo,
                            facmat.MName,
                            mattype.TypeName,
                            matspec.SpecName
                        });
            List<SelectListItem> Data = new List<SelectListItem>();
            foreach (var item in data)
            {
                Data.Add(new SelectListItem
                {
                    Text = item.MName + "-" + item.TypeName + "(" + item.SpecName + ")",
                    Value = item.POMNo.ToString()
                });
            }
            return Data;
        }
        public List<SelectListItem> GetGroupDDL ()
        {
            DryEntities DryDB = new DryEntities();
            List<SelectListItem> List = new List<SelectListItem>();
            List<MatGroup> groups = DryDB.MatGroup.ToList();
            foreach(var item in groups)
            {
                List.Add(new SelectListItem
                {
                    Value = item.GroupID.ToString(),
                    Text = item.GroupCNS
                });
            }
            return List;
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.Mvc;

namespace Dry.Models.Service
{
    public class FacSystemDBService
    {
        private DryEntities DryDB = new DryEntities();
        private DBCommon.DbEvent dbstatus = new DBCommon.DbEvent();

        #region 取得 末端型式 DropDownList
        public List<SelectListItem> GetEndTypeList()
        {
            List<SelectListItem> item = new List<SelectListItem>();

            var EndList = DryDB.EndTypeList.ToList();

            foreach (var end in EndList)
            {
                item.Add(new SelectListItem()
                {
                    Text = end.EndTypeCNS,
                    Value = end.EndType.ToString()
                });
            }
            return item;
        }
        #endregion

        #region 取得 噴頭種類 DropDownList
        public List<SelectListItem> GetFacTypeList()
        {
            List<SelectListItem> item = new List<SelectListItem>();

            var FacTypeList = DryDB.FacTypeList.ToList();

            foreach (var factype in FacTypeList)
            {
                item.Add(new SelectListItem()
                {
                    Text = factype.FTpeCNS,
                    Value = factype.FacType.ToString()
                });
            }
            return item;
        }
        #endregion

        #region 取得 材料表
        public List<Dry.Models.ViewModel.SysMat> GetFacSysMAT(short UnitID)
        {
            List<Dry.Models.ViewModel.SysMat> result = new List<ViewModel.SysMat>();
            var data = from facsysmat in DryDB.FacSysMAT
                       join matmodule in DryDB.Mat_Module
                       on facsysmat.ModuleNo equals matmodule.ModuleNo
                       where facsysmat.Bunit == UnitID
                       select new
                       {
                           matmodule.ModuleCNS,
                           facsysmat.POMNo,
                           facsysmat.MName,
                           facsysmat.SpecNo1,
                           facsysmat.SpecNo2,
                           facsysmat.SpecNo3,
                           facsysmat.ItemUnit,
                           facsysmat.Note
                       };
            List<MAT_Spec> spec = DryDB.MAT_Spec.ToList();
            foreach(var item in data)
            {
                result.Add(new ViewModel.SysMat { 
                    module = item.ModuleCNS, 
                    pomno = item.POMNo.ToString(), 
                    matname = item.MName,
                    spec1 = item.SpecNo1 == null ? "" : spec.Find(m => m.SpecNo == item.SpecNo1.Value).SpecName,
                    spec2 = item.SpecNo2 == null ? "" : spec.Find(m => m.SpecNo == item.SpecNo2.Value).SpecName,
                    spec3 = item.SpecNo3 == null ? "" : spec.Find(m => m.SpecNo == item.SpecNo3.Value).SpecName, 
                    itemunit = item.ItemUnit, 
                    description = item.Note });
            }
            return result;
        }
        #endregion

        #region 取得 材料表
        public IEnumerable GetFacSysMATByTerm(string term)
        {
            int length = 10;

            var Mat = DryDB.FacSysMAT.Where(m => m.POMNo.ToString().StartsWith(term))
                 .OrderByDescending(m => m.POMNo).Take(length)
                 .Select(m => new { Value = m.POMNo, Name = m.MName + "-" + m.Spec }).AsEnumerable();
            return Mat;
        }
        #endregion

        #region 取得材料依物料代碼
        public Dry.Models.ViewModel.SysMat GetMATbyPmno(int pomno)
        {
            if (DryDB.FacSysMAT.Any(m => m.POMNo == pomno))
            {
                FacSysMAT MatData = DryDB.FacSysMAT.Single(m => m.POMNo == pomno);
                List<Dry.Models.MAT_Spec> spec = DryDB.MAT_Spec.ToList();
                Dry.Models.ViewModel.SysMat MatjsData = new Dry.Models.ViewModel.SysMat()
                {
                    module = GetModuleName(MatData.ModuleNo),
                    matname = MatData.MName,
                    pomno = MatData.POMNo.ToString(),
                    spec1 = MatData.SpecNo1 == null ? "" : spec.Find(m => m.SpecNo == MatData.SpecNo1.Value).SpecName,
                    spec2 = MatData.SpecNo2 == null ? "" : spec.Find(m => m.SpecNo == MatData.SpecNo2.Value).SpecName,
                    spec3 = MatData.SpecNo3 == null ? "" : spec.Find(m => m.SpecNo == MatData.SpecNo3.Value).SpecName,
                    itemunit = MatData.ItemUnit,
                    description = MatData.Note
                };
                return MatjsData;
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region 取得 Module Mapping name
        public string GetModuleName(int moduleNo)
        {
            return DryDB.Mat_Module.Single(m => m.ModuleNo == moduleNo).ModuleCNS;
        }
        #endregion

        #region DB Event of StdFacSys
        public DBCommon.DbEvent CreateStdFacSys(StdFacSys data)
        {
            try
            {
                DryDB.StdFacSys.Add(data);
                DryDB.SaveChanges();
                dbstatus.KeyValue = data.FacNo;
                dbstatus.DbMessage = "Success";
            }
            catch (Exception e)
            {
                dbstatus.DbMessage = "Create StdFacSys Failed : " + e.Message + "<br>";
            }
            return dbstatus;
        }

        public DBCommon.DbEvent UpdateStdFacSys(StdFacSys data)
        {
            try
            {
                DryDB.StdFacSys.Attach(data);
                DryDB.Entry(data).State = System.Data.EntityState.Modified;
                DryDB.SaveChanges();
                dbstatus.KeyValue = data.FacNo;
                dbstatus.DbMessage = "Success";
            }
            catch (Exception e)
            {
                dbstatus.DbMessage = "Modified StdFacSys Failed : " + e.Message + "<br>";
            }
            return dbstatus;
        }

        public DBCommon.DbEvent DeleteStdFacSys(StdFacSys data)
        {
            try
            {
                dbstatus.KeyValue = data.FacNo;
                DryDB.StdFacSys.Remove(data);
                DryDB.Entry(data).State = System.Data.EntityState.Deleted;
                DryDB.SaveChanges();
                dbstatus.DbMessage = "Success";
            }
            catch (Exception e)
            {
                dbstatus.DbMessage = "Removed StdFacSys Failed : " + e.Message + "<br>";
            }
            return dbstatus;
        }
               
        #endregion

        #region DB Event of StdFacSysMAT
        public DBCommon.DbEvent CreateStdFacSysMat(StdFacSysMAT data)
        {
            try
            {
                DryDB.StdFacSysMAT.Add(data);
                DryDB.SaveChanges();
                dbstatus.KeyValue = data.No;
                dbstatus.DbMessage = "Success";
            }
            catch (Exception e)
            {
                dbstatus.DbMessage = "Create StdFacSysMAT Failed : " + e.Message + "<br>";
            }
            return dbstatus;
        }

        public DBCommon.DbEvent DelStdFacSysMatByFacNo(int facno)
        {
            try
            {
                var StdFacMatList = DryDB.StdFacSysMAT.Where(x => x.FacNo == facno).ToList();
                foreach (var data in StdFacMatList)
                {
                    DryDB.StdFacSysMAT.Remove(data);
                    DryDB.Entry(data).State = System.Data.EntityState.Deleted;
                    DryDB.SaveChanges();
                }
                dbstatus.DbMessage = "Success";
            }
            catch (Exception e)
            {
                dbstatus.DbMessage = "Removed StdFacMat By FacNo Failed : " + e.Message + "<br>";
            }
            return dbstatus;
        }
        #endregion

        #region Create StdFacSysMAT Object
        public StdFacSysMAT CreateStdMatObj(int FacNo, int pomno)
        {
            StdFacSysMAT matdata = new StdFacSysMAT();           
            matdata.FacNo = FacNo;
            matdata.POMNo = pomno;
            return matdata;
        }
        #endregion


        #region Insert FacSystem Data To DB
        public DBCommon.DbEvent InsertFacSystem(JsData ResultArry,short Unit)
        {
            string msg = "Success";
            using (TransactionScope scope = new TransactionScope())
            {
                StdFacSys stdsys = new StdFacSys();
                //stdsys.FacNo   = ResultArry.SysNo;
                stdsys.FacTypeName = ResultArry.SysTypeName;
                stdsys.FacName = ResultArry.SysName;

                stdsys.FacType = ResultArry.FacType;
                stdsys.EndType = ResultArry.EndType;
                if (ResultArry.SysDesp != null)
                    stdsys.Descript = ResultArry.SysDesp;
                stdsys.Unit = Unit;
                DBCommon.DbEvent rs = CreateStdFacSys(stdsys);
                int facNo = (int)rs.KeyValue;
                if (!rs.DbMessage.Equals("Success"))
                {
                    msg = rs.DbMessage;
                }
                else
                {
                    foreach (var pomno in ResultArry.MatNoAry)
                    {
                        Dry.Models.ViewModel.SysMat SysMat = GetMATbyPmno(pomno);
                        StdFacSysMAT mat = CreateStdMatObj(facNo, Convert.ToInt32(SysMat.pomno));

                        rs = CreateStdFacSysMat(mat);
                        if (!rs.DbMessage.Equals("Success"))
                        {
                            msg = rs.DbMessage;
                            break;
                        }
                    }
                }                
                if (msg.Equals("Success"))
                {
                    scope.Complete();
                    dbstatus.DbMessage = msg;
                }
            }
            return dbstatus;
        }
        #endregion
        
        #region Json Data Struct
        /// <summary>
        /// ////////////////////
        /// </summary>
        public class JsData
        {
            public string SysTypeName { get; set; }
            public string SysName { get; set; }
            public string SysDesp { get; set; }
            public byte EndType { get; set; }
            public byte FacType { get; set; }
            public List<int> MatNoAry { get; set; }           
        }        
        #endregion
        /// <summary>
        /// 農戶系統資料結構
        /// </summary>
        public class FarmerSysStruct
        {
            public string FarSysNo { get; set; }
            public int MapNo { get; set; }
            public string FacTypeName { get; set; }
            public int FacMoney { get; set; }
            public short ApplyUnit { get; set; }
        }
    }
}

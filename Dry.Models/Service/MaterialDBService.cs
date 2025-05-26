/*
-- =============================================
-- Author: WEI
-- Create date: 2014-10-27
-- Description: 物料管理資料庫存取
-- =============================================
*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dry.Models.ViewModel;
using System.Linq.Dynamic;
using System.Web.Mvc;

namespace Dry.Models.Service
{
    public class MaterialDBService
    {
        private DryEntities DryDB = new DryEntities();
        DBCommon.DbEvent dbStatus = new DBCommon.DbEvent();
        /// <summary>
        /// 讀取物料
        /// </summary>
        /// <param name="search">是否為搜尋</param>
        /// <param name="query">搜尋字串</param>
        /// <param name="sortIdx">排序欄位</param>
        /// <param name="UnitID">單位代碼</param>
        /// <returns></returns>
        public List<MaterialView> GetMaterialData(bool search, string query, string sortIdx, short UnitID)
        {
            var mat_module = DryDB.Mat_Module.ToList();
            var facsysmat = DryDB.FacSysMAT.Where(m => m.Bunit == UnitID).ToList();
            var mat_type = DryDB.MAT_Types.ToList();
            var mat_spec = DryDB.MAT_Spec.ToList();

            var Result = (from Mat_module in mat_module
                          join Facsysmat in facsysmat on Mat_module.ModuleNo equals Facsysmat.ModuleNo
                          //orderby (sortIdx)
                          orderby (Mat_module.ModuleNo)
                          orderby (Facsysmat.MName)
                          orderby (Facsysmat.Spec)
                          select new
                          {
                              Facsysmat.POMNo,
                              Mat_module.ModuleNo,
                              Mat_module.ModuleCNS,
                              Facsysmat.MName,
                              Facsysmat.Spec,
                              Facsysmat.SpecNo1,
                              Facsysmat.SpecNo2,
                              Facsysmat.SpecNo3,
                              Facsysmat.MatType,
                              Facsysmat.SpecLength,
                              Facsysmat.ItemUnit,
                              Facsysmat.Note
                          }).ToList();
            if (search)
            {
                Result = (from Mat_module in mat_module
                          join Facsysmat in facsysmat on Mat_module.ModuleNo equals Facsysmat.ModuleNo
                          where Facsysmat.MName.Contains(query)
                          orderby (sortIdx)
                          select new
                          {
                              Facsysmat.POMNo,
                              Mat_module.ModuleNo,
                              Mat_module.ModuleCNS,
                              Facsysmat.MName,
                              Facsysmat.Spec,
                              Facsysmat.SpecNo1,
                              Facsysmat.SpecNo2,
                              Facsysmat.SpecNo3,
                              Facsysmat.MatType,
                              Facsysmat.SpecLength,
                              Facsysmat.ItemUnit,
                              Facsysmat.Note
                          }).ToList();
            }
            List<MaterialView> MaterialList = new List<MaterialView>();
            foreach (var item in Result)
            {
                if (!item.MName.Contains("田間管路材料費")) 
                {
                    MaterialList.Add(new MaterialView
                    {
                        POMNo = item.POMNo,
                        ModuleNo = item.ModuleNo,
                        ModuleCNS = item.ModuleCNS,
                        MName = item.MName,
                        Spec = item.Spec,
                        Spec1 = item.SpecNo1 == null ? 0 : item.SpecNo1.Value,
                        Spec1CNS = item.SpecNo1 == null ? "無" : mat_spec.Find(m => m.SpecNo == item.SpecNo1).SpecName,

                        Spec2 = item.SpecNo2 == null ? 0 : item.SpecNo2.Value,
                        Spec2CNS = item.SpecNo2 == null ? "無" : mat_spec.Find(m => m.SpecNo == item.SpecNo2).SpecName,

                        Spec3 = item.SpecNo3 == null ? 0 : item.SpecNo3.Value,
                        Spec3CNS = item.SpecNo3 == null ? "無" : mat_spec.Find(m => m.SpecNo == item.SpecNo3).SpecName,

                        ItemUnit = item.ItemUnit,
                        SpecLength = item.SpecLength == null ? 0 : item.SpecLength.Value,
                        MatType = item.MatType,
                        MatTypeCNS = mat_type.Find(m => m.TypeNo == item.MatType).TypeName,
                        Note = item.Note
                    });
                }
            }
            return MaterialList;
        }

        /// <summary>
        /// 新增物料
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        public DBCommon.DbEvent AddMaterial(MaterialView Data)
        {
            FacSysMAT AddData = new FacSysMAT();
            AddData.ModuleNo = Data.ModuleNo;
            AddData.Bunit = Data.Bunit;
            AddData.MName = Data.MName;
            //AddData.Spec = Data.Spec;
            if (Data.Spec1 == 0) { AddData.SpecNo1 = null; } else { AddData.SpecNo1 = Data.Spec1; }
            if (Data.Spec2 == 0) { AddData.SpecNo2 = null; } else { AddData.SpecNo2 = Data.Spec2; }
            if (Data.Spec3 == 0) { AddData.SpecNo3 = null; } else { AddData.SpecNo3 = Data.Spec3; }
            AddData.MatType = Data.MatType;
            AddData.SpecLength = Data.SpecLength;
            AddData.ItemUnit = Data.ItemUnit;
            AddData.Note = Data.Note;
            //confirm the data has exists
            /*if (!new DryEntities().FacSysMAT.ToList().Any(m => m.MName.Contains(AddData.MName)
                && m.SpecLength == AddData.SpecLength
                && m.MatType == AddData.MatType
                && m.SpecNo1 == AddData.SpecNo1
                && m.SpecNo2 == AddData.SpecNo2
                && m.SpecNo3 == AddData.SpecNo3))
            {*/
                try
                {
                    DryDB.FacSysMAT.Add(AddData);
                    DryDB.SaveChanges();
                    dbStatus.DbMessage = "Success";
                }
                catch (Exception e)
                {
                    dbStatus.DbMessage = "Create FacSysMAT Failed : " + e.Message;
                }
            /*}
            else
            {
                dbStatus.DbMessage = "【" + AddData.MName + "】：資料已存在，請確認。";
            }*/
            return dbStatus;
        }

        /// <summary>
        /// 修改物料表
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        public DBCommon.DbEvent EditMaterial(MaterialView Data)
        {
            FacSysMAT EditData = DryDB.FacSysMAT.Single(m => m.POMNo == Data.POMNo);
            EditData.ModuleNo = Data.ModuleNo;
            EditData.MName = Data.MName;
            EditData.MatType = Data.MatType;
            EditData.SpecLength = Data.SpecLength;
            if (Data.Spec1 == 0) { EditData.SpecNo1 = null; } else { EditData.SpecNo1 = Data.Spec1; }
            if (Data.Spec2 == 0) { EditData.SpecNo2 = null; } else { EditData.SpecNo2 = Data.Spec2; }
            if (Data.Spec3 == 0) { EditData.SpecNo3 = null; } else { EditData.SpecNo3 = Data.Spec3; }

            EditData.ItemUnit = Data.ItemUnit;
            EditData.Note = Data.Note;

            //confirm the data has exists
            //if (!DryDB.FacSysMAT.Any(m => m.MName.Contains(EditData.MName)
            //    && m.SpecLength == EditData.SpecLength
            //    && m.MatType == EditData.MatType
            //    && m.SpecNo1 == EditData.SpecNo1
            //    && m.SpecNo2 == EditData.SpecNo2
            //    && m.SpecNo3 == EditData.SpecNo3))
            //{
                try
                {
                    DryDB.FacSysMAT.Attach(EditData);
                    DryDB.Entry(EditData).State = System.Data.EntityState.Modified;
                    DryDB.SaveChanges();

                    dbStatus.DbMessage = "Success";
                    dbStatus.KeyValue = EditData.POMNo;
                }
                catch (Exception e)
                {
                    dbStatus.DbMessage = "Modified FacSysMAT Failed : " + e.Message;
                }
            //}
            //else
            //{
            //    dbStatus.DbMessage = "【" + EditData.MName + "】：資料已存在，請確認。";
            //}
            return dbStatus;
        }

        /// <summary>
        /// 刪除物料
        /// </summary>
        /// <param name="POMNo"></param>
        /// <returns></returns>
        public DBCommon.DbEvent DelMaterial(int POMNo)
        {
            FacSysMAT DelData = DryDB.FacSysMAT.Single(p => p.POMNo == POMNo);
            try
            {
                DryDB.FacSysMAT.Remove(DelData);
                DryDB.SaveChanges();
                dbStatus.DbMessage = "Success";
            }
            catch(Exception e)
            {
                dbStatus.DbMessage = "Removed FacSysMAT Failed : " + e.Message;
            }
            return dbStatus;
        }

        /// <summary>
        /// 讀取物料價格
        /// </summary>
        /// <param name="POMNo"></param>
        /// <returns></returns>
        public List<MaterialPriceView> GetMaterialPrice(int POMNo)
        {
            List<PriceOfMat> Data = DryDB.PriceOfMat.Where(p => p.POMNo == POMNo).OrderByDescending(p => p.BYear).ToList();
            List<MaterialPriceView> List = new List<MaterialPriceView>();
            foreach(var item in Data)
            {
                List.Add(new MaterialPriceView { No = item.No, POMNo = item.POMNo, BYear = item.BYear, Price = item.Price });
            }
            return List;
        }
        /// <summary>
        /// 編輯物料價格
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        public DBCommon.DbEvent EditMaterialPrice(MaterialPriceView Data)
        {
            PriceOfMat EditData = DryDB.PriceOfMat.Single(p => p.No == Data.No);
            EditData.Price = Data.Price;
            try
            {
                DryDB.PriceOfMat.Attach(EditData);
                DryDB.Entry(EditData).State = System.Data.EntityState.Modified;
                DryDB.SaveChanges();

                dbStatus.DbMessage = "Success";
                dbStatus.KeyValue = EditData.No;
            }
            catch (Exception e)
            {
                dbStatus.DbMessage = "Modified PriceOfMat Failed : " + e.Message;
            }
            return dbStatus;
        }
        /// <summary>
        /// 新增物料價格
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        public DBCommon.DbEvent AddMaterialPrice(MaterialPriceView Data)
        {
            PriceOfMat AddData = new PriceOfMat();
            AddData.POMNo = Data.POMNo;
            AddData.BYear = Data.BYear;
            AddData.Price = Data.Price;
            try
            {
                DryDB.PriceOfMat.Add(AddData);
                DryDB.SaveChanges();
                dbStatus.DbMessage = "Success";
            }
            catch (Exception e)
            {
                dbStatus.DbMessage = "Create PriceOfMat Failed : " + e.Message;
            }
            return dbStatus;
        }
        /// <summary>
        /// 刪除物料價格
        /// </summary>
        /// <param name="No"></param>
        /// <returns></returns>
        public DBCommon.DbEvent DelMaterialPrice(int No)
        {
            PriceOfMat DelData = DryDB.PriceOfMat.Single(p => p.No == No);
            try
            {
                DryDB.PriceOfMat.Remove(DelData);
                DryDB.SaveChanges();
                dbStatus.DbMessage = "Success";
            }
            catch (Exception e)
            {
                dbStatus.DbMessage = "Removed PriceOfMat Failed : " + e.Message;
            }
            return dbStatus;
        }
    }
}

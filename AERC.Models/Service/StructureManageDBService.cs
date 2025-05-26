using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AERC.Models.ViewModel;

namespace AERC.Models.Service
{
    public class StructureManageDBService
    {
        CommonEntities comm = new CommonEntities();

        /// <summary>
        /// 修改構造物資料
        /// </summary>
        /// <param name="editData"></param>
        /// <returns></returns>
        public DBCommon.DbEvent EditStruct(StructureView.StructData editData)
        {
            AERC.Models.Structure data = comm.Structure.Find(editData.SId);
            DBCommon.DbEvent dbstatus = new DBCommon.DbEvent();
            if (data == null)
            {
                dbstatus.DbMessage = "Update Structure Failed : No Data Updated";
                return dbstatus;
            }
            else
            {
                data.SName = editData.SName;
                data.TypeId = editData.TypeId;
                data.Longitude = editData.Longitude;
                data.Latitude = editData.Latitude;
                data.Area = editData.Area;
                data.Length = editData.Length;
                data.Capacity = editData.Capacity;
                data.Unit_Id = editData.Unit_Id;
                comm.Structure.Attach(data);
                comm.Entry(data).State = System.Data.EntityState.Modified;
                try
                {
                    comm.SaveChanges();
                    dbstatus.DbMessage = "Success";
                }
                catch (Exception e)
                {
                    dbstatus.DbMessage = "Modified Structure Failed : " + e.Message;
                }
            }
            return dbstatus;
        }

        /// <summary>
        /// 新增構造物資料
        /// </summary>
        /// <param name="addData"></param>
        /// <returns></returns>
        public DBCommon.DbEvent AddStruct(Structure addData)
        {
            AERC.Models.Structure data = new AERC.Models.Structure();
            DBCommon.DbEvent dbstatus = new DBCommon.DbEvent();
            data.SId = Guid.NewGuid();
            data.SName = addData.SName;
            data.TypeId = addData.TypeId;
            data.Longitude = addData.Longitude;
            data.Latitude = addData.Latitude;
            data.Area = addData.Area;
            data.Length = addData.Length;
            data.Capacity = addData.Capacity;
            data.Unit_Id = addData.Unit_Id;
            comm.Structure.Add(data);
            try
            {
                comm.SaveChanges();
                dbstatus.DbMessage = "Success";
            }
            catch (Exception e)
            {
                dbstatus.DbMessage = "Inserted Structure Failed : " + e.Message;
            }
            return dbstatus;
        }

        /// <summary>
        /// 刪除構造物資料
        /// </summary>
        /// <param name="SId"></param>
        /// <returns></returns>
        public DBCommon.DbEvent DelStruct(Guid SId)
        {
            DBCommon.DbEvent dbstatus = new DBCommon.DbEvent();
            Structure data = comm.Structure.Find(SId);
            if (data == null)
            {
                dbstatus.DbMessage = "Delete Structure Failed : No Data Deleted";
                return dbstatus;
            }
            else
            {
                comm.Structure.Remove(data);
                try
                {
                    comm.SaveChanges();
                    dbstatus.DbMessage = "Success";
                }
                catch (Exception e)
                {
                    dbstatus.DbMessage = "Delete Structure Failed : " + e.Message;
                }
            }
            return dbstatus;
        }
    }
}

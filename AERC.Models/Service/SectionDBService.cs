/*
-- =============================================
-- Author:  Wei
-- Create date: 2014-11-10
-- Description: 地段資料的資料庫操作類別
-- =============================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AERC.Models.ViewModel;

namespace AERC.Models.Service
{
    public class SectionDBService
    {
        DBCommon.DbEvent status = new DBCommon.DbEvent();
        CommonEntities commDB = new CommonEntities();
        /// <summary>
        /// 編輯地段
        /// </summary>
        /// <param name="Section_Id">地段代碼</param>
        /// <param name="Section_Code">地段編號</param>
        /// <param name="SectionName">地段</param>
        /// <param name="Subsection">小段</param>
        /// <returns></returns>
        public DBCommon.DbEvent EditSection(int Section_Id, int Section_Code, string SectionName, string Subsection)
        {
            Section section = commDB.Section.Single(p => p.Section_Id == Section_Id);
            section.Section_Code = Section_Code;
            section.Section1 = SectionName;
            section.Subsection = Subsection;
            try
            {
                commDB.Section.Attach(section);
                commDB.Entry(section).State = System.Data.EntityState.Modified;
                commDB.SaveChanges();

                status.DbMessage = "Success";
                status.KeyValue = section.Section_Id;
            }
            catch (Exception e)
            {
                status.DbMessage = "Modified Section Failed : " + e.Message;
            }
            return status;
        }

        /// <summary>
        /// 刪除地段
        /// </summary>
        /// <param name="Section_Id">地段代碼</param>
        /// <returns></returns>
        public DBCommon.DbEvent DelSection(int Section_Id)
        {
            Section section = commDB.Section.Single(p => p.Section_Id == Section_Id);
            try
            {
                commDB.Section.Remove(section);
                commDB.SaveChanges();
                status.DbMessage = "Success";
            }
            catch (Exception e)
            {
                status.DbMessage = "Removed Section Failed : " + e.Message;
            }
            return status;
        }
        /// <summary>
        /// 新增地段
        /// </summary>
        /// <param name="Section_Code">地段編號</param>
        /// <param name="SectionName">地段</param>
        /// <param name="Subsection">小段</param>
        /// <param name="TownId">鄉鎮代碼</param>
        /// <returns></returns>
        public DBCommon.DbEvent AddSection(int Section_Code, string SectionName, string Subsection, short TownId)
        {
            Section section = new Section();
            section.Section_Code = Section_Code;
            section.Section1 = SectionName;
            section.Subsection = Subsection;
            section.Town_Id = TownId;
            try
            {
                commDB.Section.Add(section);
                commDB.SaveChanges();
                status.DbMessage = "Success";
            }
            catch (Exception e)
            {
                status.DbMessage = "Create Section Failed : " + e.Message;
            }
            return status;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dry.Models.ViewModel;

namespace Dry.Models.Service
{
    public class SectionDBService
    {
        DBCommon.DbEvent dbStatus = new DBCommon.DbEvent();
        /// <summary>
        /// 讀取地段資料
        /// </summary>
        /// <param name="TownId">鄉鎮代碼</param>
        /// <returns></returns>
        public List<SectionList> GetSectionList(short TownId)
        {
            List<AERC.Models.Section> Data = new AERC.Models.CommonCls.GetSectionData().GetSectionList(TownId);
            List<SectionList> List = new List<SectionList>();
            foreach(var item in Data)
            {
                List.Add(new SectionList
                {
                    Section_Id = item.Section_Id,
                    Section_Code = item.Section_Code.Value.ToString().PadLeft(4,'0'),
                    Section = item.Section1,
                    Subsection = item.Subsection,
                    Town_Id = item.Town_Id.Value
                });
            }
            return List;
        }
        /// <summary>
        /// 讀取地號資料
        /// </summary>
        /// <param name="SectionID">地段代碼</param>
        /// <returns></returns>
        public List<LandNumberList> GetLandNumberList(int SectionID)
        {
            List<AERC.Models.Land_Number> Data = new AERC.Models.CommonCls.GetLandNumber().GetLandNumberList(SectionID);
            List<LandNumberList> List = new List<LandNumberList>();
            foreach(var item in Data)
            {
                List.Add(new LandNumberList
                {
                    Land_Number_Id = item.Land_Number_Id,
                    Land_Code = item.Land_Code,
                    Section_Id = item.Section_Id == null ? 0 : item.Section_Id.Value,
                    Land_type_Id = item.Land_type_Id == null ? (byte)0 : item.Land_type_Id.Value,
                    Longitude = item.Longitude == null ? 0 : item.Longitude.Value,
                    Latitude = item.Latitude == null ? 0 : item.Latitude.Value
                });
            }
            return List;
        }

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
            AERC.Models.Service.DBCommon.DbEvent status = new AERC.Models.Service.SectionDBService().EditSection(Section_Id, Section_Code, SectionName, Subsection);
            dbStatus.DbMessage = status.DbMessage;
            dbStatus.KeyValue = status.KeyValue;
            return dbStatus;
        }
        /// <summary>
        /// 刪除地段
        /// </summary>
        /// <param name="Section_Id">地段代碼</param>
        /// <returns></returns>
        public DBCommon.DbEvent DelSection(int Section_Id)
        {
            AERC.Models.Service.DBCommon.DbEvent status = new AERC.Models.Service.SectionDBService().DelSection(Section_Id);
            dbStatus.DbMessage = status.DbMessage;
            dbStatus.KeyValue = status.KeyValue;
            return dbStatus;
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
            AERC.Models.Service.DBCommon.DbEvent status = new AERC.Models.Service.SectionDBService().AddSection(Section_Code, SectionName, Subsection, TownId);
            dbStatus.DbMessage = status.DbMessage;
            dbStatus.KeyValue = status.KeyValue;
            return dbStatus;
        }
        /// <summary>
        /// 編輯地號
        /// </summary>
        /// <param name="LandNumberID">地號代碼</param>
        /// <param name="Land_Code">地號</param>
        /// <param name="Longitude">經度</param>
        /// <param name="Latitude">緯度</param>
        /// <returns></returns>
        public DBCommon.DbEvent EditLandNumber(int LandNumberID, string Land_Code, decimal Longitude, decimal Latitude)
        {
            AERC.Models.Service.DBCommon.DbEvent status = new AERC.Models.Service.LandNumberDBService().EditLandNumber(LandNumberID, Land_Code, Longitude, Latitude);
            dbStatus.DbMessage = status.DbMessage;
            dbStatus.KeyValue = status.KeyValue;
            return dbStatus;
        }
        /// <summary>
        /// 刪除地號
        /// </summary>
        /// <param name="LandNumberID">地號代碼</param>
        /// <returns></returns>
        public DBCommon.DbEvent DelLandNumber(int LandNumberID)
        {
            AERC.Models.Service.DBCommon.DbEvent status = new AERC.Models.Service.LandNumberDBService().DelLandNumber(LandNumberID);
            dbStatus.DbMessage = status.DbMessage;
            dbStatus.KeyValue = status.KeyValue;
            return dbStatus;
        }
        /// <summary>
        /// 新增地號
        /// </summary>
        /// <param name="Section_Id">地段代碼</param>
        /// <param name="Land_Code">地號</param>
        /// <param name="Longitude">經度</param>
        /// <param name="Latitude">緯度</param>
        /// <returns></returns>
        public DBCommon.DbEvent AddLandNumber(int Section_Id, string Land_Code, decimal Longitude, decimal Latitude)
        {
            AERC.Models.Service.DBCommon.DbEvent status = new AERC.Models.Service.LandNumberDBService().AddLandNumber(Land_Code, Section_Id, Longitude, Latitude);
            dbStatus.DbMessage = status.DbMessage;
            dbStatus.KeyValue = status.KeyValue;
            return dbStatus;
        }
    }
}

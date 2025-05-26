/*
-- =============================================
-- Author:  Wei
-- Create date: 2014-11-5
-- Description: 讀取縣市及鄉鎮資料
-- =============================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AERC.Models.ViewModel;

namespace AERC.Models.CommonCls
{
    public class GetTownData
    {
        CommonEntities CommDB = new CommonEntities();
        /// <summary>
        /// 讀取縣市資料
        /// </summary>
        /// <returns></returns>
        public List<City> GetCityList()
        {
            return CommDB.City.ToList();
        }
        /// <summary>
        /// 讀取鄉鎮市資料
        /// </summary>
        /// <param name="CityCode">縣市代碼</param>
        /// <returns></returns>
        public List<Town> GetTownList(string CityCode)
        {
            return CommDB.Town.Where(p => p.City_Code == CityCode).ToList();
        }
        public Town GetTownList(short TownID)
        {
            return CommDB.Town.Find(TownID);
        }
        /// <summary>
        /// 讀取鄉鎮資料+地政事務所名稱
        /// </summary>
        /// <param name="CityCode"></param>
        /// <returns></returns>
        public List<TownVIew> GetTownListWithOfficeLand(string CityCode)
        {
            List<TownVIew> Data = new List<TownVIew>();
            var TownList = CommDB.Town.Where(p => p.City_Code == CityCode).ToList();
            var LandOfficeList = CommDB.Land_Office.Where(p => p.City_Code == CityCode).ToList();
            var DataList = (from townList in TownList
                            join landOfficeList in LandOfficeList on townList.Land_Office_Id equals landOfficeList.Land_Office_Id
                            select new
                            {
                                townList.Town_Id,
                                townList.Town_Code,
                                townList.Town1,
                                townList.Zip_Code,
                                landOfficeList.Land_Office_Id,
                                landOfficeList.Land_Office_Code,
                                landOfficeList.Land_Office1,
                                townList.City_Code
                            }).ToList();
            foreach(var item in DataList)
            {
                Data.Add(new TownVIew
                {
                    Town_Id = item.Town_Id,
                    Town_Code = item.Town_Code,
                    Town = item.Town1,
                    Zip_Code = item.Zip_Code.HasValue ? item.Zip_Code.Value : (short)0,
                    Land_Office_Id = item.Land_Office_Id,
                    Land_Office_Code = item.Land_Office_Code,
                    Land_Office = item.Land_Office1,
                    City_Code = item.City_Code
                });
            }
            return Data;
        }
        /// <summary>
        /// 讀取地政事務所資料
        /// </summary>
        /// <param name="CityCode">縣市代碼</param>
        /// <returns></returns>
        public List<Land_Office> GetLandOfficeByCityCode(string CityCode)
        {
            return CommDB.Land_Office.Where(p => p.City_Code == CityCode).OrderBy(p=>p.Land_Office_Code).ToList();
        }
        /// <summary>
        /// 從資料庫讀取郵遞區號
        /// </summary>
        /// <param name="City">縣市名稱</param>
        /// <param name="Town">鄉鎮名稱</param>
        /// <returns></returns>
        public short GetZipCode(string City, string Town)
        {
            City = AreaNameMapping(City);
            Town = Town.Substring(0, 2);
            return CommDB.City.Where(p => p.City1.Contains(City)).First().Town.Where(p => p.Town1.Contains(Town)).First().Zip_Code.Value;
        }

        private string AreaNameMapping(string Name)
        {
            switch(Name)
            {
                case "台北市":
                    return "臺北市";
                case "台北縣":
                    return "新北市";
                case "桃園縣":
                    return "桃園市";
                default:
                    return Name;
            }
        }
        /// <summary>
        /// 取得鄉鎮代碼
        /// </summary>
        /// <param name="TownID"></param>
        /// <returns></returns>
        public string GetTownCode(string TownID)
        {
            return CommDB.Town.Find(Convert.ToInt16(TownID)).Town_Code;
        }

    }
}

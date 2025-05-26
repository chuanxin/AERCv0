using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dry.Models.ViewModel;
using AERC.Models.Service;

namespace Dry.Models.Service
{
    public class TownManageDBService
    {
        DBCommon.DbEvent dbStatus = new DBCommon.DbEvent();
        /// <summary>
        /// 讀取鄉鎮資料
        /// </summary>
        /// <param name="CityCode"></param>
        /// <returns></returns>
        public List<TownDataView> GetTownData(string CityCode)
        {
            List<TownDataView> Data = new List<TownDataView>();
            var TownOrgiData = new AERC.Models.CommonCls.GetTownData().GetTownListWithOfficeLand(CityCode);
            
            foreach(var item in TownOrgiData)
            {
                Data.Add(new TownDataView
                {
                    Town_Id = item.Town_Id,
                    Town_Code = item.Town_Code,
                    Town = item.Town,
                    Zip_Code = item.Zip_Code,
                    Land_Office_Id = item.Land_Office_Id,
                    Land_Office_Code = item.Land_Office_Code,
                    Land_Office = item.Land_Office_Code + "-" + item.Land_Office,
                    City_Code = item.City_Code
                });
            }
            return Data;
        }

        public DBCommon.DbEvent EditTown(short TownId,short Zip_Code, string Town, short Land_Office)
        {
            AERC.Models.ViewModel.TownVIew dataView = new AERC.Models.ViewModel.TownVIew();
            dataView.Town_Id = TownId;
            dataView.Zip_Code = Zip_Code;
            dataView.Town = Town;
            dataView.Land_Office_Id = Land_Office;
            AERC.Models.Service.DBCommon.DbEvent status = new TownDBService().EditTown(dataView);
            dbStatus.DbMessage = status.DbMessage;
            dbStatus.KeyValue = status.KeyValue;
            return dbStatus;
        }

        public DBCommon.DbEvent AddTown(string CityCode,short Zip_Code, string Town_Code, string Town, short Land_Office)
        {
            AERC.Models.Service.DBCommon.DbEvent status = new TownDBService().AddTown(CityCode,Zip_Code, Town_Code, Town, Land_Office);
            dbStatus.DbMessage = status.DbMessage;
            dbStatus.KeyValue = status.KeyValue;
            return dbStatus;
        }

        public DBCommon.DbEvent DelTown(short id)
        {
            AERC.Models.Service.DBCommon.DbEvent status = new TownDBService().DelTown(id);
            dbStatus.DbMessage = status.DbMessage;
            dbStatus.KeyValue = status.KeyValue;
            return dbStatus;
        }
    }
}

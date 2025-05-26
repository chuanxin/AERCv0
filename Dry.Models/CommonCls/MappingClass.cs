using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dry.Models.CommonCls
{
    public class MappingClass
    {
        #region Get the Mapping Cntrl Type Name of Cntrl Type
        public string GetCntrltypeName(Int16 ctypecode)
        {
            DryEntities DryDB = new DryEntities();
            return DryDB.CntrlList.Single(ct => ct.CntrlCode == ctypecode).CntrlCNS;
        }
        #endregion

        #region  Get the Mapping Engine Name of Engine Type
        public string GetEngName(Int16 Engcode)
        {
            DryEntities DryDB = new DryEntities();
            return DryDB.EngineList.Single(x => x.EngCode == Engcode).EngCNS;
        }
        #endregion

        #region Get the Mapping Pool Name of Pool Type
        public string GetPooltypeName(Int16 ptypecode)
        {
            DryEntities DryDB = new DryEntities();
            return DryDB.PoolTypeList.Single(pt => pt.PtypeCode == ptypecode).PtypeCNS;
        }
        #endregion
        /// <summary>
        /// 取得WMS的水利會代號
        /// </summary>
        /// <param name="UnitID">水利會代碼</param>
        /// <returns></returns>
        public string GetWmsIaName(short UnitID)
        {
            switch(UnitID)
            {
                case 0:
                    break;
                case 1:
                    return "ilia_WMS";
                case 2:
                    return "pkia_WMS";
                case 3:
                    return "tyia_WMS";
                case 4:
                    return "smia_WMS";
                case 5:
                    return "hcia_WMS";
                case 6:
                    return "mlia_WMS";
                case 7:
                    return "tcia_WMS";
                case 8:
                    return "ntia_WMS";
                case 9:
                    return "chia_WMS";
                case 10:
                    return "ylia_WMS";
                case 11:
                    return "cnia_WMS";
                case 12:
                    return "ksia_WMS";
                case 13:
                    return "ptia_WMS";
                case 14:
                    return "ttia_WMS";
                case 15:
                    return "hlia_WMS";
                case 16:
                    return "csia_WMS";
                case 17:
                    return "lgia_WMS";
                default:
                    return "";
            }
            return "";
        }
    }
}

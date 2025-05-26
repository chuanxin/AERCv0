using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AERC.Models.CommonCls
{
    public class GetAdminData
    {
        private CommonEntities _entity = new CommonEntities();

        /// <summary>
        /// 取得admin資料
        /// </summary>
        /// <returns></returns>
        public List<View_Admin> GetAdmin(int Unit_Id)
        {
            if (Unit_Id == 99)
                return _entity.View_Admin.ToList();
            return _entity.View_Admin.Where(x => x.Unit_Id == Unit_Id).ToList();
        }
        /// <summary>
        /// 取得登入者資料
        /// </summary>
        /// <param name="Account"></param>
        /// <returns></returns>
        public Admin GetAdminByAccount(string Account)
        {
            return _entity.Admin.Where(p => p.Account == Account).First();
        }
    }
}

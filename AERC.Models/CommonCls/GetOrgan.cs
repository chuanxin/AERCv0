using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace AERC.Models.CommonCls
{
    public class GetOrgan
    {
        private CommonEntities _entity = new CommonEntities();

        /// <summary>
        /// 取得組室DDL
        /// </summary>
        /// <returns></returns>
        public SelectList GetOrgans()
        {
            IEnumerable<SelectListItem> OrganList =
            (from m in _entity.Organization select m).AsEnumerable()
            .Select(m => new SelectListItem()
            {
                Text = m.Organization1,
                Value = m.Organization_Id.ToString()
            }
          );
            return new SelectList(OrganList, "Value", "Text");
        }
        /// <summary>
        /// 取得組室名稱
        /// </summary>
        /// <param name="OrgansID"></param>
        /// <returns></returns>
        public string GetOrgans(short OrgansID)
        {
            return _entity.Organization.Find(OrgansID) == null ? null : _entity.Organization.Find(OrgansID).Organization1;
        }
    }
}

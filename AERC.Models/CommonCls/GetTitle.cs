using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace AERC.Models.CommonCls
{
    public class GetTitle
    {
        private CommonEntities _entity = new CommonEntities();

        /// <summary>
        /// 取得職稱
        /// </summary>
        /// <returns></returns>
        public SelectList GetTitles()
        {
            IEnumerable<SelectListItem> TitleList =
            (from m in _entity.Title select m).AsEnumerable()
            .Select(m => new SelectListItem()
            {
                Text = m.Title1,
                Value = m.Title_Id.ToString()
            }
          );
            return new SelectList(TitleList, "Value", "Text");
        }
        /// <summary>
        /// 取得職稱中文
        /// </summary>
        /// <param name="TitleID"></param>
        /// <returns></returns>
        public string GetTitleName(short TitleID)
        {
            return _entity.Title.Find(TitleID) == null ? null : _entity.Title.Find(TitleID).Title1;
        }
    }
}

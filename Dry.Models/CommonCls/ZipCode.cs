using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Dry.Models.CommonCls
{
    public class ZipCode
    {
        /// <summary>
        /// 郵遞區號查詢(3碼)
        /// </summary>
        /// <param name="City">縣市名稱</param>
        /// <param name="Town">鄉鎮名稱</param>
        /// <returns></returns>
        public string Get3Code(string City, string Town)
        {
            XDocument data = XDocument.Load(System.Web.HttpContext.Current.Server.MapPath("~/App_Data/Zip32.xml"));

            var dataList = data.Root.Elements("Zip32").Select(p => new
            {
                zipCode = p.Element("Zip5").Value,
                City = p.Element("City").Value,
                Town = p.Element("Area").Value
            }).Select(p =>
            {
                string town = p.Town;
                string city = p.City;
                if (city == "新竹市" || city == "嘉義市")
                    town = city;
                else if (city == "南海島")
                    city = "高雄市";
                else if (city == "釣魚台")
                    city = "宜蘭縣";
                return new
                {
                    City = city,
                    Town = town,
                    zipCode = p.zipCode
                };
            });
            var code = from datalist in dataList
                       where datalist.City == City && datalist.Town == Town
                       select datalist;
            string result = code.First().zipCode;
            return result[0].ToString() + result[1] + result[2];
        }
    }
}

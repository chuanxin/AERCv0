using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dry.Models.CommonCls
{
    public class NumberToChinese
    {
        #region 數字轉國字
        public string GetChineseNumber(int number)
        {
            string[] chineseNumber = { "零", "壹", "貳", "參", "肆", "伍", "陸", "柒", "捌", "玖" };
            string[] unit = { "", "拾", "佰", "仟", "萬", "拾萬", "佰萬" };
            System.Text.StringBuilder ret = new System.Text.StringBuilder();
            string inputNumber = number.ToString();
            int idx = 0;
            bool needAppendZero = false;

            idx = inputNumber.Length;
            foreach (char c in inputNumber)
            {
                idx--;
                if (c > '0')
                {
                    if (needAppendZero)
                    {
                        ret.Append(chineseNumber[0]);
                        needAppendZero = false;
                    }
                    ret.Append(chineseNumber[(int)(c - '0')] + unit[idx]);
                }
                else
                    needAppendZero = true;
            }

            System.Text.StringBuilder sb = ret;

            if (sb.Length == 0)
                return chineseNumber[0];   // 零

            if (sb[0] == '零')   // Remove the header zero.
                sb.Remove(0, 1);

            // Remove redundant units.
            string[] dupUnitName = { "萬", "億", "兆" };
            string text = sb.ToString();
            int pos = -1;
            foreach (string s in dupUnitName)
            {
                pos = text.LastIndexOf(s);
                if (pos != -1)
                {
                    sb.Replace(s, "", 0, pos);
                }
            }
            return sb.ToString();
            //return ret.Length == 0 ? chineseNumber[0] : ret.ToString();
        }
        #endregion
    }
}

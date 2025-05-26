/*
-- =============================================
-- Author: WEI
-- Create date: 2014-2-11
-- Description: 調控設施的顯示所有資料型別定義
-- =============================================
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dry.Models.ViewModel
{
    public class CtrlTableView
    {
        public short CntrlCode { get; set; }
        public string MatName { get; set; }
        public short MatAmt { get; set; }
        public double MatPrice { get; set; }
        public int MatNo { get; set; }
        public short MatAmtAply { get; set; }
        public double MatPriceAply { get; set; }
    }
    public class CtrlMatNoView
    {
        public int MatNo { get; set; }
    }
    public class JsonData
    {
        public List<CtrlTableView> CtrlMatAry { get; set; }
        public int FacMoney { get; set; }
        public short ApplyUnit { get; set; }
    }
}

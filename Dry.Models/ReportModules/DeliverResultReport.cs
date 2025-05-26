using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;
using System.Web;
using OfficeOpenXml;

namespace Dry.Models.ReportModules
{

    public class DeliverResultReport
    {
        public byte[] CreateReport_Old(int unit)
        {
            string sourcepath = @"~\ReportSample\DeliverResult.xlsx";
            FileStream fs = new FileStream(HttpContext.Current.Server.MapPath(sourcepath), FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            ExcelPackage excel = new ExcelPackage(fs);
            ExcelWorksheet sheet = excel.Workbook.Worksheets["成果統計"];
            string cnstring = ConfigurationManager.ConnectionStrings["DryDB"].ConnectionString;

            using (SqlConnection cn = new SqlConnection())
            {
                cn.ConnectionString = cnstring;
                cn.Open();

                SqlCommand cmd = new SqlCommand();
                StringBuilder cmdline = new StringBuilder();

                cmdline.AppendLine("select COUNT(*) as caseCount,catalogCNS,SUM(buildarea) / 10000 as farmarea,");
                cmdline.AppendLine("SUM(tab1.pw) as pw,SUM(tab1.pc) as pc,SUM(b.GovSubsidy+b.GoldSubsidy) as govpay,");
                cmdline.AppendLine("SUM(b.FarmerFee) as farmerpay,SUM(b.total) as total,N'瑠公' as dtype,a.ApplyYear");
                cmdline.AppendLine(",sum(tab2.engCount) as engCount from CaseDetail a");
                cmdline.AppendLine("left join (select SUM(poolweight) as pw,COUNT(*) as pc,MapNo from [Pool] group by MapNo) tab1");
                cmdline.AppendLine("on a.MapNo = tab1.MapNo left join TotalFee b on a.MapNo = b.MapNo");
                cmdline.AppendLine("left join (select COUNT(*) as engCount,MapNo from Engine group by mapno) tab2 on a.MapNo=tab2.MapNo");
                cmdline.AppendLine("where a.ApplyUnit=" + unit + " and a.mapno in (select mapno from [PoolApply] where applyunit=17 )");
                cmdline.AppendLine("and a.Step >= 7 group by ApplyYear,CatalogCNS union all");
                cmdline.AppendLine("select COUNT(*) as caseCount,catalogCNS,SUM(buildarea) / 10000 as farmarea,");
                cmdline.AppendLine("SUM(tab1.pw) as pw,SUM(tab1.pc) as pc,SUM(b.GovSubsidy+b.GoldSubsidy) as govpay,");
                cmdline.AppendLine("SUM(b.FarmerFee) as farmerpay,SUM(b.total) as total,'推廣' as dtype,a.ApplyYear");
                cmdline.AppendLine(",sum(tab2.engCount) as engCount from CaseDetail a");
                cmdline.AppendLine("left join (select SUM(poolweight) as pw,COUNT(*) as pc,MapNo from [Pool] group by MapNo) tab1");
                cmdline.AppendLine("on a.MapNo = tab1.MapNo left join TotalFee b on a.MapNo = b.MapNo");
                cmdline.AppendLine("left join (select COUNT(*) as engCount,MapNo from Engine group by mapno) tab2 on a.MapNo=tab2.MapNo");
                cmdline.AppendLine("where a.ApplyUnit=" + unit + " and a.mapno not in (select mapno from [PoolApply] where applyunit=17 )");
                cmdline.AppendLine("and a.Step >= 7 group by ApplyYear,CatalogCNS");
                cmdline.AppendLine(" order by applyyear,dtype,catalogcns");
                cmd.CommandText = cmdline.ToString();
                cmd.Connection = cn;
                SqlDataReader dr = cmd.ExecuteReader();


                int irow = 2;
                int startrow = 0;
                string oldapplyYear = "";
                int poolcount = 0;
                int poolweight = 0;

                while (dr.Read())
                {
                    irow++;
                    if (oldapplyYear == "")
                    {
                        oldapplyYear = dr["applyyear"].ToString();

                        startrow = irow;
                    }
                    else if (oldapplyYear != dr["applyyear"].ToString())
                    {
                        sheet.Cells[irow, 2].Value = "小計";
                        sheet.Cells[irow, 4].Formula = "Sum(D" + startrow + ":D" + (irow - 1) + ")";
                        sheet.Cells[irow, 5].Formula = "Sum(E" + startrow + ":E" + (irow - 1) + ")";
                        sheet.Cells[irow, 6].Formula = "Sum(F" + startrow + ":F" + (irow - 1) + ")";
                        sheet.Cells[irow, 7].Formula = "Sum(G" + startrow + ":G" + (irow - 1) + ")";
                        sheet.Cells[irow, 8].Formula = "Sum(H" + startrow + ":H" + (irow - 1) + ")";
                        sheet.Cells[irow, 9].Formula = "G" + irow + "/H" + irow;

                        sheet.Cells[irow, 10].Formula = "IF(E" + irow + ">0,G" + irow + "/E" + irow + ",0)";
                        sheet.Cells[irow, 11].Formula = "IF(E" + irow + ">0,H" + irow + "/E" + irow + ",0)";
                        sheet.Cells[irow, 12].Formula = string.Format("CONCATENATE({0},\"/\",{1})", poolweight, poolcount);
                        sheet.Cells[irow, 13].Formula = "Sum(M" + startrow + ":M" + (irow - 1) + ")";

                        sheet.Cells[startrow, 1, irow, 1].Merge = true;
                        irow++;
                        startrow = irow;
                        oldapplyYear = dr["applyyear"].ToString();
                        poolweight = 0;
                        poolcount = 0;

                    }

                    sheet.Cells[irow, 1].Value = dr["applyyear"];
                    sheet.Cells[irow, 2].Value = dr["dtype"];
                    sheet.Cells[irow, 3].Value = dr["catalogCNS"];
                    sheet.Cells[irow, 4].Value = dr["caseCount"];
                    sheet.Cells[irow, 5].Value = dr["farmarea"];
                    sheet.Cells[irow, 6].Value = dr["farmerpay"];
                    sheet.Cells[irow, 7].Value = dr["govpay"];
                    sheet.Cells[irow, 8].Value = dr["total"];
                    sheet.Cells[irow, 9].Formula = "G" + irow + "/H" + irow;
                    if (dr.GetDouble(dr.GetOrdinal("farmarea")) > 0) {
                        sheet.Cells[irow, 10].Formula = "G" + irow + "/E" + irow;
                        sheet.Cells[irow, 11].Formula = "H" + irow + "/E" + irow;

                    }

                    sheet.Cells[irow, 12].Formula = string.Format("CONCATENATE({0},\"/\",{1})", dr["pw"], dr["pc"]);
                    sheet.Cells[irow, 13].Value = dr["engCount"];
                    if (!DBNull.Value.Equals(dr["pw"]))
                    {
                        poolcount += dr.GetInt32(dr.GetOrdinal("pc"));
                        poolweight += dr.GetInt32(dr.GetOrdinal("pw"));
                    }
                }

                irow++;

                sheet.Cells[irow, 2].Value = "小計";
                sheet.Cells[irow, 4].Formula = "Sum(D" + startrow + ":D" + (irow - 1) + ")";
                sheet.Cells[irow, 5].Formula = "Sum(E" + startrow + ":E" + (irow - 1) + ")";
                sheet.Cells[irow, 6].Formula = "Sum(F" + startrow + ":F" + (irow - 1) + ")";
                sheet.Cells[irow, 7].Formula = "Sum(G" + startrow + ":G" + (irow - 1) + ")";
                sheet.Cells[irow, 8].Formula = "Sum(H" + startrow + ":H" + (irow - 1) + ")";
                sheet.Cells[irow, 9].Formula = "G" + irow + "/H" + irow;

                sheet.Cells[irow, 10].Formula = "IF(E" + irow + ">0,G" + irow + "/E" + irow + ",0)";
                sheet.Cells[irow, 11].Formula = "IF(E" + irow + ">0,H" + irow + "/E" + irow + ",0)";
                sheet.Cells[irow, 12].Formula = string.Format("CONCATENATE({0},\"/\",{1})", poolweight, poolcount);
                sheet.Cells[irow, 13].Formula = "Sum(M" + startrow + ":M" + (irow - 1) + ")";
                sheet.Cells[startrow, 1, irow, 1].Merge = true;
                


                for (int i = 1; i < 14; i++)
                {
                    for (int j = 1; j < irow + 1; j++)
                    {
                        sheet.Cells[j, i].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                    }
                }

            }

            return excel.GetAsByteArray();
        }
        /// <summary>
        /// 單位歷年補助統計表
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="endYear"></param>
        /// <param name="startYear"></param>
        /// <returns></returns>
        public byte[] CreateReport(int unit, int endYear, int startYear)
        {
            string sourcepath = @"~\ReportSample\DeliverResult.xlsx";
            FileStream fs = new FileStream(HttpContext.Current.Server.MapPath(sourcepath), FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            ExcelPackage excel = new ExcelPackage(fs);
            ExcelWorksheet sheet = excel.Workbook.Worksheets["成果統計"];


            List<HistoricalData> result = getStatDataByUnit(unit, endYear, startYear);
            int irow = 2;
            int startrow = 0;
            int startrowsub = 0;
            string oldapplyYear = string.Empty;
            string olddtype = string.Empty;
            int poolcount = 0;
            int poolweight = 0;
            int totalpoolcount = 0, totalpoolweight = 0, totalengcount = 0, totalcasecount = 0,
                totalfarmerpay = 0, totalgovpay = 0;
            double totalarea = 0;
            bool startInit = false;
            HistoricalData data穿孔管 = new HistoricalData();
            HistoricalData data噴頭 = new HistoricalData();
            HistoricalData data滴灌 = new HistoricalData();
            HistoricalData data微噴 = new HistoricalData();
            HistoricalData data多目標 = new HistoricalData();
            HistoricalData data其它 = new HistoricalData();


            
            foreach (var item in result)
            {
                irow++;
                if (startInit == false)
                {
                    
                    oldapplyYear = item.ApplyYear.ToString();
                    olddtype = item.dtype;
                    startrow = irow;
                    startrowsub = irow;
                    startInit = true;
                }
                if (oldapplyYear != item.ApplyYear.ToString())
                {
                    sheet.Cells[irow, 2].Value = olddtype;
                    sheet.Cells[irow, 3].Value = "小計";
                    sheet.Cells[irow, 4].Formula = "Sum(D" + startrowsub + ":D" + (irow - 1) + ")";
                    sheet.Cells[irow, 5].Formula = "Sum(E" + startrowsub + ":E" + (irow - 1) + ")";
                    sheet.Cells[irow, 6].Formula = "Sum(F" + startrowsub + ":F" + (irow - 1) + ")";
                    sheet.Cells[irow, 7].Formula = "Sum(G" + startrowsub + ":G" + (irow - 1) + ")";
                    sheet.Cells[irow, 8].Formula = "Sum(H" + startrowsub + ":H" + (irow - 1) + ")";
                    sheet.Cells[irow, 9].Formula = "G" + irow + "/H" + irow;
                    sheet.Cells[irow, 10].Formula = "IF(E" + irow + ">0,G" + irow + "/E" + irow + ",0)";
                    sheet.Cells[irow, 11].Formula = "IF(E" + irow + ">0,H" + irow + "/E" + irow + ",0)";
                    sheet.Cells[irow, 12].Formula = string.Format("CONCATENATE({0},\"/\",{1})", poolweight, poolcount);
                    sheet.Cells[irow, 13].Formula = "Sum(M" + startrowsub + ":M" + (irow - 1) + ")";
                    irow++;
                    sheet.Cells[irow, 2].Value = "小計";
                    sheet.Cells[irow, 4].Value = totalcasecount;
                    sheet.Cells[irow, 5].Value = totalarea;
                    sheet.Cells[irow, 6].Value = totalfarmerpay;
                    sheet.Cells[irow, 7].Value = totalgovpay;
                    sheet.Cells[irow, 8].Value = totalfarmerpay + totalgovpay;
                    sheet.Cells[irow, 9].Formula = "G" + irow + "/H" + irow;
                    sheet.Cells[irow, 10].Formula = "IF(E" + irow + ">0,G" + irow + "/E" + irow + ",0)";
                    sheet.Cells[irow, 11].Formula = "IF(E" + irow + ">0,H" + irow + "/E" + irow + ",0)";
                    sheet.Cells[irow, 12].Formula = string.Format("CONCATENATE({0},\"/\",{1})", totalpoolweight, totalpoolcount);
                    sheet.Cells[irow, 13].Value = totalengcount;
                    sheet.Cells[startrow, 1, irow, 1].Merge = true;
                    irow++;
                    startrow = irow;
                    startrowsub = irow;
                    //oldapplyYear = dr["applyyear"].ToString();
                    oldapplyYear = item.ApplyYear.ToString();
                    olddtype = item.dtype;
                    poolweight = 0;
                    poolcount = 0;
                    totalpoolcount = 0;
                    totalpoolweight = 0;
                    totalfarmerpay = 0;
                    totalgovpay = 0;
                    totalengcount = 0;
                    totalcasecount = 0;
                    totalarea = 0;

                } else
                {
                    if (olddtype != item.dtype)
                    {
                        sheet.Cells[irow, 2].Value = olddtype;
                        sheet.Cells[irow, 3].Value = "小計";
                        sheet.Cells[irow, 4].Formula = "Sum(D" + startrowsub + ":D" + (irow - 1) + ")";
                        sheet.Cells[irow, 5].Formula = "Sum(E" + startrowsub + ":E" + (irow - 1) + ")";
                        sheet.Cells[irow, 6].Formula = "Sum(F" + startrowsub + ":F" + (irow - 1) + ")";
                        sheet.Cells[irow, 7].Formula = "Sum(G" + startrowsub + ":G" + (irow - 1) + ")";
                        sheet.Cells[irow, 8].Formula = "Sum(H" + startrowsub + ":H" + (irow - 1) + ")";
                        sheet.Cells[irow, 9].Formula = "G" + irow + "/H" + irow;
                        sheet.Cells[irow, 10].Formula = "IF(E" + irow + ">0,G" + irow + "/E" + irow + ",0)";
                        sheet.Cells[irow, 11].Formula = "IF(E" + irow + ">0,H" + irow + "/E" + irow + ",0)";
                        sheet.Cells[irow, 12].Formula = string.Format("CONCATENATE({0},\"/\",{1})", poolweight, poolcount);
                        sheet.Cells[irow, 13].Formula = "Sum(M" + startrowsub + ":M" + (irow - 1) + ")";
                        irow++;
                        startrowsub = irow;
                        olddtype = item.dtype;
                        poolweight = 0;
                        poolcount = 0;
                    }
                }

                sheet.Cells[irow, 1].Value = item.ApplyYear;
                sheet.Cells[irow, 2].Value = item.dtype;
                sheet.Cells[irow, 3].Value = item.catalogCNS;
                sheet.Cells[irow, 4].Value = item.caseCount;
                sheet.Cells[irow, 5].Value = item.farmarea;
                sheet.Cells[irow, 6].Value = item.farmerpay;
                sheet.Cells[irow, 7].Value = item.govpay;
                sheet.Cells[irow, 8].Value = item.total;
                sheet.Cells[irow, 9].Formula = "IF(H" + irow + ">0,G" + irow + "/H" + irow + ",0)"; ;
                if (item.farmarea > 0)
                {
                    sheet.Cells[irow, 10].Formula = "G" + irow + "/E" + irow;
                    sheet.Cells[irow, 11].Formula = "H" + irow + "/E" + irow;

                }

                sheet.Cells[irow, 12].Formula = string.Format("CONCATENATE({0},\"/\",{1})", item.pw, item.pc);
                sheet.Cells[irow, 13].Value = item.engCount;

                poolcount += item.pc;
                poolweight += item.pw;
                totalcasecount += item.caseCount;
                totalarea += item.farmarea;
                totalfarmerpay += item.farmerpay;
                totalgovpay += item.govpay;
                totalpoolcount += item.pc;
                totalpoolweight += item.pw;
                totalengcount += item.engCount;

                switch (item.catalogCNS)
                {
                    case "穿孔管":
                        data穿孔管.caseCount += item.caseCount;
                        data穿孔管.farmarea += item.farmarea;
                        data穿孔管.farmerpay += item.farmerpay;
                        data穿孔管.govpay += item.govpay;
                        data穿孔管.total += item.total;
                        data穿孔管.pc += item.pc;
                        data穿孔管.pw += item.pw;
                        data穿孔管.engCount += item.engCount;
                        break;
                    case "噴頭":
                        data噴頭.caseCount += item.caseCount;
                        data噴頭.farmarea += item.farmarea;
                        data噴頭.farmerpay += item.farmerpay;
                        data噴頭.govpay += item.govpay;
                        data噴頭.total += item.total;
                        data噴頭.pc += item.pc;
                        data噴頭.pw += item.pw;
                        data噴頭.engCount += item.engCount;
                        break;
                    case "滴灌":
                        data滴灌.caseCount += item.caseCount;
                        data滴灌.farmarea += item.farmarea;
                        data滴灌.farmerpay += item.farmerpay;
                        data滴灌.govpay += item.govpay;
                        data滴灌.total += item.total;
                        data滴灌.pc += item.pc;
                        data滴灌.pw += item.pw;
                        data滴灌.engCount += item.engCount;
                        break;
                    case "微噴":
                        data微噴.caseCount += item.caseCount;
                        data微噴.farmarea += item.farmarea;
                        data微噴.farmerpay += item.farmerpay;
                        data微噴.govpay += item.govpay;
                        data微噴.total += item.total;
                        data微噴.pc += item.pc;
                        data微噴.pw += item.pw;
                        data微噴.engCount += item.engCount;
                        break;
                    case "多目標":
                        data多目標.caseCount += item.caseCount;
                        data多目標.farmarea += item.farmarea;
                        data多目標.farmerpay += item.farmerpay;
                        data多目標.govpay += item.govpay;
                        data多目標.total += item.total;
                        data多目標.pc += item.pc;
                        data多目標.pw += item.pw;
                        data多目標.engCount += item.engCount;
                        break;
                    default:
                        data其它.caseCount += item.caseCount;
                        data其它.farmarea += item.farmarea;
                        data其它.farmerpay += item.farmerpay;
                        data其它.govpay += item.govpay;
                        data其它.total += item.total;
                        data其它.pc += item.pc;
                        data其它.pw += item.pw;
                        data其它.engCount += item.engCount;
                        break;
                }

            }


            irow++;
            
            sheet.Cells[irow, 3].Value = "小計";
            sheet.Cells[irow, 4].Formula = "Sum(D" + startrowsub + ":D" + (irow - 1) + ")";
            sheet.Cells[irow, 5].Formula = "Sum(E" + startrowsub + ":E" + (irow - 1) + ")";
            sheet.Cells[irow, 6].Formula = "Sum(F" + startrowsub + ":F" + (irow - 1) + ")";
            sheet.Cells[irow, 7].Formula = "Sum(G" + startrowsub + ":G" + (irow - 1) + ")";
            sheet.Cells[irow, 8].Formula = "Sum(H" + startrowsub + ":H" + (irow - 1) + ")";
            sheet.Cells[irow, 9].Formula = "IF(H" + irow + ">0,G" + irow + "/H" + irow + ",0)";
            sheet.Cells[irow, 10].Formula = "IF(E" + irow + ">0,G" + irow + "/E" + irow + ",0)";
            sheet.Cells[irow, 11].Formula = "IF(E" + irow + ">0,H" + irow + "/E" + irow + ",0)";
            sheet.Cells[irow, 12].Formula = string.Format("CONCATENATE({0},\"/\",{1})", poolweight, poolcount);
            sheet.Cells[irow, 13].Formula = "Sum(M" + startrowsub + ":M" + (irow - 1) + ")";

            irow++;
            sheet.Cells[irow, 2].Value = "小計";
            sheet.Cells[irow, 4].Value = totalcasecount;
            sheet.Cells[irow, 5].Value = totalarea;
            sheet.Cells[irow, 6].Value = totalfarmerpay;
            sheet.Cells[irow, 7].Value = totalgovpay;
            sheet.Cells[irow, 8].Value = totalfarmerpay + totalgovpay;
            sheet.Cells[irow, 9].Formula = "IF(H" + irow + ">0,G" + irow + "/H" + irow + ",0)";
            sheet.Cells[irow, 10].Formula = "IF(E" + irow + ">0,G" + irow + "/E" + irow + ",0)";
            sheet.Cells[irow, 11].Formula = "IF(E" + irow + ">0,H" + irow + "/E" + irow + ",0)";
            sheet.Cells[irow, 12].Formula = string.Format("CONCATENATE({0},\"/\",{1})", totalpoolweight, totalpoolcount);
            sheet.Cells[irow, 13].Value = totalengcount;
            sheet.Cells[startrow, 1, irow, 1].Merge = true;
            
            irow++;
            startrow = irow;
            sheet.Cells[irow, 1].Value = "總計";
            sheet.Cells[irow, 3].Value = "穿孔管";
            sheet.Cells[irow, 4].Value = data穿孔管.caseCount;
            sheet.Cells[irow, 5].Value = data穿孔管.farmarea;
            sheet.Cells[irow, 6].Value = data穿孔管.farmerpay;
            sheet.Cells[irow, 7].Value = data穿孔管.govpay;
            sheet.Cells[irow, 8].Value = data穿孔管.farmerpay + data穿孔管.govpay;
            sheet.Cells[irow, 9].Formula = "IF(H" + irow + ">0,G" + irow + "/H" + irow + ",0)";
            sheet.Cells[irow, 10].Formula = "IF(E" + irow + ">0,G" + irow + "/E" + irow + ",0)";
            sheet.Cells[irow, 11].Formula = "IF(E" + irow + ">0,H" + irow + "/E" + irow + ",0)";
            sheet.Cells[irow, 12].Formula = string.Format("CONCATENATE({0},\"/\",{1})", data穿孔管.pw, data穿孔管.pc);
            sheet.Cells[irow, 13].Value = data穿孔管.engCount;
            irow++;
            sheet.Cells[irow, 3].Value = "噴頭";
            sheet.Cells[irow, 4].Value = data噴頭.caseCount;
            sheet.Cells[irow, 5].Value = data噴頭.farmarea;
            sheet.Cells[irow, 6].Value = data噴頭.farmerpay;
            sheet.Cells[irow, 7].Value = data噴頭.govpay;
            sheet.Cells[irow, 8].Value = data噴頭.farmerpay + data噴頭.govpay;
            sheet.Cells[irow, 9].Formula = "IF(H" + irow + ">0,G" + irow + "/H" + irow + ",0)";
            sheet.Cells[irow, 10].Formula = "IF(E" + irow + ">0,G" + irow + "/E" + irow + ",0)";
            sheet.Cells[irow, 11].Formula = "IF(E" + irow + ">0,H" + irow + "/E" + irow + ",0)";
            sheet.Cells[irow, 12].Formula = string.Format("CONCATENATE({0},\"/\",{1})", data噴頭.pw, data噴頭.pc);
            sheet.Cells[irow, 13].Value = data噴頭.engCount;
            irow++;
            sheet.Cells[irow, 3].Value = "滴灌";
            sheet.Cells[irow, 4].Value = data滴灌.caseCount;
            sheet.Cells[irow, 5].Value = data滴灌.farmarea;
            sheet.Cells[irow, 6].Value = data滴灌.farmerpay;
            sheet.Cells[irow, 7].Value = data滴灌.govpay;
            sheet.Cells[irow, 8].Value = data滴灌.farmerpay + data滴灌.govpay;
            sheet.Cells[irow, 9].Formula = "IF(H" + irow + ">0,G" + irow + "/H" + irow + ",0)";
            sheet.Cells[irow, 10].Formula = "IF(E" + irow + ">0,G" + irow + "/E" + irow + ",0)";
            sheet.Cells[irow, 11].Formula = "IF(E" + irow + ">0,H" + irow + "/E" + irow + ",0)";
            sheet.Cells[irow, 12].Formula = string.Format("CONCATENATE({0},\"/\",{1})", data滴灌.pw, data滴灌.pc);
            sheet.Cells[irow, 13].Value = data滴灌.engCount;
            irow++;
            sheet.Cells[irow, 3].Value = "微噴";
            sheet.Cells[irow, 4].Value = data微噴.caseCount;
            sheet.Cells[irow, 5].Value = data微噴.farmarea;
            sheet.Cells[irow, 6].Value = data微噴.farmerpay;
            sheet.Cells[irow, 7].Value = data微噴.govpay;
            sheet.Cells[irow, 8].Value = data微噴.farmerpay + data微噴.govpay;
            sheet.Cells[irow, 9].Formula = "IF(H" + irow + ">0,G" + irow + "/H" + irow + ",0)";
            sheet.Cells[irow, 10].Formula = "IF(E" + irow + ">0,G" + irow + "/E" + irow + ",0)";
            sheet.Cells[irow, 11].Formula = "IF(E" + irow + ">0,H" + irow + "/E" + irow + ",0)";
            sheet.Cells[irow, 12].Formula = string.Format("CONCATENATE({0},\"/\",{1})", data微噴.pw, data微噴.pc);
            sheet.Cells[irow, 13].Value = data微噴.engCount;
            irow++;
            sheet.Cells[irow, 3].Value = "多目標";
            sheet.Cells[irow, 4].Value = data多目標.caseCount;
            sheet.Cells[irow, 5].Value = data多目標.farmarea;
            sheet.Cells[irow, 6].Value = data多目標.farmerpay;
            sheet.Cells[irow, 7].Value = data多目標.govpay;
            sheet.Cells[irow, 8].Value = data多目標.farmerpay + data多目標.govpay;
            sheet.Cells[irow, 9].Formula = "IF(H" + irow + ">0,G" + irow + "/H" + irow + ",0)";
            sheet.Cells[irow, 10].Formula = "IF(E" + irow + ">0,G" + irow + "/E" + irow + ",0)";
            sheet.Cells[irow, 11].Formula = "IF(E" + irow + ">0,H" + irow + "/E" + irow + ",0)";
            sheet.Cells[irow, 12].Formula = string.Format("CONCATENATE({0},\"/\",{1})", data多目標.pw, data多目標.pc);
            sheet.Cells[irow, 13].Value = data多目標.engCount;
            irow++;
            sheet.Cells[irow, 3].Value = "其它";
            sheet.Cells[irow, 4].Value = data其它.caseCount;
            sheet.Cells[irow, 5].Value = data其它.farmarea;
            sheet.Cells[irow, 6].Value = data其它.farmerpay;
            sheet.Cells[irow, 7].Value = data其它.govpay;
            sheet.Cells[irow, 8].Value = data其它.farmerpay + data其它.govpay;
            sheet.Cells[irow, 9].Formula = "IF(H" + irow + ">0,G" + irow + "/H" + irow + ",0)";
            sheet.Cells[irow, 10].Formula = "IF(E" + irow + ">0,G" + irow + "/E" + irow + ",0)";
            sheet.Cells[irow, 11].Formula = "IF(E" + irow + ">0,H" + irow + "/E" + irow + ",0)";
            sheet.Cells[irow, 12].Formula = string.Format("CONCATENATE({0},\"/\",{1})", data其它.pw, data其它.pc);
            sheet.Cells[irow, 13].Value = data其它.engCount;

            irow++;
            sheet.Cells[irow, 3].Value = "小計";

            sheet.Cells[irow, 4].Formula = "Sum(D" + (irow - 6) + ":D" + (irow - 1) + ")";
            sheet.Cells[irow, 5].Formula = "Sum(E" + (irow - 6) + ":E" + (irow - 1) + ")";
            sheet.Cells[irow, 6].Formula = "Sum(F" + (irow - 6) + ":F" + (irow - 1) + ")";
            sheet.Cells[irow, 7].Formula = "Sum(G" + (irow - 6) + ":G" + (irow - 1) + ")";
            sheet.Cells[irow, 8].Formula = "Sum(H" + (irow - 6) + ":H" + (irow - 1) + ")";
            sheet.Cells[irow, 9].Formula = "IF(H" + irow + ">0,G" + irow + "/H" + irow + ",0)";
            sheet.Cells[irow, 10].Formula = "IF(E" + irow + ">0,G" + irow + "/E" + irow + ",0)";
            sheet.Cells[irow, 11].Formula = "IF(E" + irow + ">0,H" + irow + "/E" + irow + ",0)";

            sheet.Cells[irow, 12].Formula = string.Format("CONCATENATE({0},\"/\",{1})", data穿孔管.pw + data噴頭.pw + data滴灌.pw + data微噴.pw
                + data多目標.pw + data其它.pw, data穿孔管.pc + data噴頭.pc + data滴灌.pc + data微噴.pc + data多目標.pc + data其它.pc);
            sheet.Cells[irow, 13].Formula = "Sum(M" + startrow + ":M" + (irow - 1) + ")";

            sheet.Cells[startrow, 1, irow, 2].Merge = true;
            

            for (int i = 1; i < 14; i++)
            {
                for (int j = 1; j < irow + 1; j++)
                {
                    sheet.Cells[j, i].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                }
            }

            //}

            return excel.GetAsByteArray();
        }


        public byte[] CreateReportAll(int unit, int endYear, int startYear)
        {
            string sourcepath = @"~\ReportSample\DeliverResult.xlsx";
            FileStream fs = new FileStream(HttpContext.Current.Server.MapPath(sourcepath), FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            ExcelPackage excel = new ExcelPackage(fs);
            ExcelWorksheet sheet = excel.Workbook.Worksheets["成果統計"];


            List<HistoricalData> result = getStatDataByAll(unit, endYear, startYear);
            
            int irow = 2;
            int startrow = 0;
            int startrowsub = 0;
            string oldapplyYear = string.Empty;
            string olddtype = string.Empty;
            int poolcount = 0;
            int poolweight = 0;
            int totalpoolcount = 0, totalpoolweight = 0, totalengcount = 0, totalcasecount = 0,
                totalfarmerpay = 0, totalgovpay = 0;
            double totalarea = 0;
            bool startInit = false;
            HistoricalData data穿孔管 = new HistoricalData();
            HistoricalData data噴頭 = new HistoricalData();
            HistoricalData data滴灌 = new HistoricalData();
            HistoricalData data微噴 = new HistoricalData();
            HistoricalData data多目標 = new HistoricalData();
            HistoricalData data其它 = new HistoricalData();



            foreach (var item in result)
            {
                irow++;
                if (startInit == false)
                {
                    //oldapplyYear = dr["applyyear"].ToString();
                    oldapplyYear = item.ApplyYear.ToString();
                    olddtype = item.dtype;
                    startrow = irow;
                    startrowsub = irow;
                    startInit = true;
                }
                if (oldapplyYear != item.ApplyYear.ToString())
                {
                    sheet.Cells[irow, 2].Value = olddtype;
                    sheet.Cells[irow, 3].Value = "小計";
                    sheet.Cells[irow, 4].Formula = "Sum(D" + startrowsub + ":D" + (irow - 1) + ")";
                    sheet.Cells[irow, 5].Formula = "Sum(E" + startrowsub + ":E" + (irow - 1) + ")";
                    sheet.Cells[irow, 6].Formula = "Sum(F" + startrowsub + ":F" + (irow - 1) + ")";
                    sheet.Cells[irow, 7].Formula = "Sum(G" + startrowsub + ":G" + (irow - 1) + ")";
                    sheet.Cells[irow, 8].Formula = "Sum(H" + startrowsub + ":H" + (irow - 1) + ")";
                    sheet.Cells[irow, 9].Formula = "G" + irow + "/H" + irow;
                    sheet.Cells[irow, 10].Formula = "IF(E" + irow + ">0,G" + irow + "/E" + irow + ",0)";
                    sheet.Cells[irow, 11].Formula = "IF(E" + irow + ">0,H" + irow + "/E" + irow + ",0)";
                    sheet.Cells[irow, 12].Formula = string.Format("CONCATENATE({0},\"/\",{1})", poolweight, poolcount);
                    sheet.Cells[irow, 13].Formula = "Sum(M" + startrowsub + ":M" + (irow - 1) + ")";
                    irow++;
                    sheet.Cells[irow, 2].Value = "小計";
                    sheet.Cells[irow, 4].Value = totalcasecount;
                    sheet.Cells[irow, 5].Value = totalarea;
                    sheet.Cells[irow, 6].Value = totalfarmerpay;
                    sheet.Cells[irow, 7].Value = totalgovpay;
                    sheet.Cells[irow, 8].Value = totalfarmerpay + totalgovpay;
                    sheet.Cells[irow, 9].Formula = "G" + irow + "/H" + irow;
                    sheet.Cells[irow, 10].Formula = "IF(E" + irow + ">0,G" + irow + "/E" + irow + ",0)";
                    sheet.Cells[irow, 11].Formula = "IF(E" + irow + ">0,H" + irow + "/E" + irow + ",0)";
                    sheet.Cells[irow, 12].Formula = string.Format("CONCATENATE({0},\"/\",{1})", totalpoolweight, totalpoolcount);
                    sheet.Cells[irow, 13].Value = totalengcount;
                    sheet.Cells[startrow, 1, irow, 1].Merge = true;
                    irow++;
                    startrow = irow;
                    startrowsub = irow;
                    //oldapplyYear = dr["applyyear"].ToString();
                    oldapplyYear = item.ApplyYear.ToString();
                    olddtype = item.dtype;
                    poolweight = 0;
                    poolcount = 0;
                    totalpoolcount = 0;
                    totalpoolweight = 0;
                    totalfarmerpay = 0;
                    totalgovpay = 0;
                    totalengcount = 0;
                    totalcasecount = 0;
                    totalarea = 0;

                }
                else 
                {
                    if (olddtype != item.dtype)
                    {
                        sheet.Cells[irow, 2].Value = olddtype;
                        sheet.Cells[irow, 3].Value = "小計";
                        sheet.Cells[irow, 4].Formula = "Sum(D" + startrowsub + ":D" + (irow - 1) + ")";
                        sheet.Cells[irow, 5].Formula = "Sum(E" + startrowsub + ":E" + (irow - 1) + ")";
                        sheet.Cells[irow, 6].Formula = "Sum(F" + startrowsub + ":F" + (irow - 1) + ")";
                        sheet.Cells[irow, 7].Formula = "Sum(G" + startrowsub + ":G" + (irow - 1) + ")";
                        sheet.Cells[irow, 8].Formula = "Sum(H" + startrowsub + ":H" + (irow - 1) + ")";
                        sheet.Cells[irow, 9].Formula = "G" + irow + "/H" + irow;
                        sheet.Cells[irow, 10].Formula = "IF(E" + irow + ">0,G" + irow + "/E" + irow + ",0)";
                        sheet.Cells[irow, 11].Formula = "IF(E" + irow + ">0,H" + irow + "/E" + irow + ",0)";
                        sheet.Cells[irow, 12].Formula = string.Format("CONCATENATE({0},\"/\",{1})", poolweight, poolcount);
                        sheet.Cells[irow, 13].Formula = "Sum(M" + startrowsub + ":M" + (irow - 1) + ")";
                        irow++;
                        startrowsub = irow;
                        olddtype = item.dtype;
                        poolweight = 0;
                        poolcount = 0;
                    }
                }

                sheet.Cells[irow, 1].Value = item.ApplyYear;
                sheet.Cells[irow, 2].Value = item.dtype;
                sheet.Cells[irow, 3].Value = item.catalogCNS;
                sheet.Cells[irow, 4].Value = item.caseCount;
                sheet.Cells[irow, 5].Value = item.farmarea;
                sheet.Cells[irow, 6].Value = item.farmerpay;
                sheet.Cells[irow, 7].Value = item.govpay;
                sheet.Cells[irow, 8].Value = item.total;
                sheet.Cells[irow, 9].Formula = "IF(H" + irow + ">0,G" + irow + "/H" + irow + ",0)"; ;
                if (item.farmarea > 0)
                {
                    sheet.Cells[irow, 10].Formula = "G" + irow + "/E" + irow;
                    sheet.Cells[irow, 11].Formula = "H" + irow + "/E" + irow;

                }

                sheet.Cells[irow, 12].Formula = string.Format("CONCATENATE({0},\"/\",{1})", item.pw, item.pc);
                sheet.Cells[irow, 13].Value = item.engCount;

                poolcount += item.pc;
                poolweight += item.pw;
                totalcasecount += item.caseCount;
                totalarea += item.farmarea;
                totalfarmerpay += item.farmerpay;
                totalgovpay += item.govpay;
                totalpoolcount += item.pc;
                totalpoolweight += item.pw;
                totalengcount += item.engCount;

                switch (item.catalogCNS)
                {
                    case "穿孔管":
                        data穿孔管.caseCount += item.caseCount;
                        data穿孔管.farmarea += item.farmarea;
                        data穿孔管.farmerpay += item.farmerpay;
                        data穿孔管.govpay += item.govpay;
                        data穿孔管.total += item.total;
                        data穿孔管.pc += item.pc;
                        data穿孔管.pw += item.pw;
                        data穿孔管.engCount += item.engCount;
                        break;
                    case "噴頭":
                        data噴頭.caseCount += item.caseCount;
                        data噴頭.farmarea += item.farmarea;
                        data噴頭.farmerpay += item.farmerpay;
                        data噴頭.govpay += item.govpay;
                        data噴頭.total += item.total;
                        data噴頭.pc += item.pc;
                        data噴頭.pw += item.pw;
                        data噴頭.engCount += item.engCount;
                        break;
                    case "滴灌":
                        data滴灌.caseCount += item.caseCount;
                        data滴灌.farmarea += item.farmarea;
                        data滴灌.farmerpay += item.farmerpay;
                        data滴灌.govpay += item.govpay;
                        data滴灌.total += item.total;
                        data滴灌.pc += item.pc;
                        data滴灌.pw += item.pw;
                        data滴灌.engCount += item.engCount;
                        break;
                    case "微噴":
                        data微噴.caseCount += item.caseCount;
                        data微噴.farmarea += item.farmarea;
                        data微噴.farmerpay += item.farmerpay;
                        data微噴.govpay += item.govpay;
                        data微噴.total += item.total;
                        data微噴.pc += item.pc;
                        data微噴.pw += item.pw;
                        data微噴.engCount += item.engCount;
                        break;
                    case "多目標":
                        data多目標.caseCount += item.caseCount;
                        data多目標.farmarea += item.farmarea;
                        data多目標.farmerpay += item.farmerpay;
                        data多目標.govpay += item.govpay;
                        data多目標.total += item.total;
                        data多目標.pc += item.pc;
                        data多目標.pw += item.pw;
                        data多目標.engCount += item.engCount;
                        break;
                    default:
                        data其它.caseCount += item.caseCount;
                        data其它.farmarea += item.farmarea;
                        data其它.farmerpay += item.farmerpay;
                        data其它.govpay += item.govpay;
                        data其它.total += item.total;
                        data其它.pc += item.pc;
                        data其它.pw += item.pw;
                        data其它.engCount += item.engCount;
                        break;
                }

            }


            irow++;
            
            sheet.Cells[irow, 3].Value = "小計";
            sheet.Cells[irow, 4].Formula = "Sum(D" + startrowsub + ":D" + (irow - 1) + ")";
            sheet.Cells[irow, 5].Formula = "Sum(E" + startrowsub + ":E" + (irow - 1) + ")";
            sheet.Cells[irow, 6].Formula = "Sum(F" + startrowsub + ":F" + (irow - 1) + ")";
            sheet.Cells[irow, 7].Formula = "Sum(G" + startrowsub + ":G" + (irow - 1) + ")";
            sheet.Cells[irow, 8].Formula = "Sum(H" + startrowsub + ":H" + (irow - 1) + ")";
            sheet.Cells[irow, 9].Formula = "IF(H" + irow + ">0,G" + irow + "/H" + irow + ",0)";
            sheet.Cells[irow, 10].Formula = "IF(E" + irow + ">0,G" + irow + "/E" + irow + ",0)";
            sheet.Cells[irow, 11].Formula = "IF(E" + irow + ">0,H" + irow + "/E" + irow + ",0)";
            sheet.Cells[irow, 12].Formula = string.Format("CONCATENATE({0},\"/\",{1})", poolweight, poolcount);
            sheet.Cells[irow, 13].Formula = "Sum(M" + startrow + ":M" + (irow - 1) + ")";

            irow++;
            sheet.Cells[irow, 2].Value = "小計";
            sheet.Cells[irow, 4].Value = totalcasecount;
            sheet.Cells[irow, 5].Value = totalarea;
            sheet.Cells[irow, 6].Value = totalfarmerpay;
            sheet.Cells[irow, 7].Value = totalgovpay;
            sheet.Cells[irow, 8].Value = totalfarmerpay + totalgovpay;
            sheet.Cells[irow, 9].Formula = "IF(H" + irow + ">0,G" + irow + "/H" + irow + ",0)";
            sheet.Cells[irow, 10].Formula = "IF(E" + irow + ">0,G" + irow + "/E" + irow + ",0)";
            sheet.Cells[irow, 11].Formula = "IF(E" + irow + ">0,H" + irow + "/E" + irow + ",0)";
            sheet.Cells[irow, 12].Formula = string.Format("CONCATENATE({0},\"/\",{1})", totalpoolweight, totalpoolcount);
            sheet.Cells[irow, 13].Value = totalengcount;
            sheet.Cells[startrow, 1, irow, 1].Merge = true;
            
            irow++;
            startrow = irow;
            sheet.Cells[irow, 1].Value = "總計";
            sheet.Cells[irow, 3].Value = "穿孔管";
            sheet.Cells[irow, 4].Value = data穿孔管.caseCount;
            sheet.Cells[irow, 5].Value = data穿孔管.farmarea;
            sheet.Cells[irow, 6].Value = data穿孔管.farmerpay;
            sheet.Cells[irow, 7].Value = data穿孔管.govpay;
            sheet.Cells[irow, 8].Value = (decimal)data穿孔管.farmerpay + (decimal)data穿孔管.govpay;
            sheet.Cells[irow, 9].Formula = "IF(H" + irow + ">0,G" + irow + "/H" + irow + ",0)";
            sheet.Cells[irow, 10].Formula = "IF(E" + irow + ">0,G" + irow + "/E" + irow + ",0)";
            sheet.Cells[irow, 11].Formula = "IF(E" + irow + ">0,H" + irow + "/E" + irow + ",0)";
            sheet.Cells[irow, 12].Formula = string.Format("CONCATENATE({0},\"/\",{1})", data穿孔管.pw, data穿孔管.pc);
            sheet.Cells[irow, 13].Value = data穿孔管.engCount;
            irow++;
            sheet.Cells[irow, 3].Value = "噴頭";
            sheet.Cells[irow, 4].Value = data噴頭.caseCount;
            sheet.Cells[irow, 5].Value = data噴頭.farmarea;
            sheet.Cells[irow, 6].Value = data噴頭.farmerpay;
            sheet.Cells[irow, 7].Value = data噴頭.govpay;
            sheet.Cells[irow, 8].Value = (decimal)data噴頭.farmerpay + (decimal)data噴頭.govpay;
            sheet.Cells[irow, 9].Formula = "IF(H" + irow + ">0,G" + irow + "/H" + irow + ",0)";
            sheet.Cells[irow, 10].Formula = "IF(E" + irow + ">0,G" + irow + "/E" + irow + ",0)";
            sheet.Cells[irow, 11].Formula = "IF(E" + irow + ">0,H" + irow + "/E" + irow + ",0)";
            sheet.Cells[irow, 12].Formula = string.Format("CONCATENATE({0},\"/\",{1})", data噴頭.pw, data噴頭.pc);
            sheet.Cells[irow, 13].Value = data噴頭.engCount;
            irow++;
            sheet.Cells[irow, 3].Value = "滴灌";
            sheet.Cells[irow, 4].Value = data滴灌.caseCount;
            sheet.Cells[irow, 5].Value = data滴灌.farmarea;
            sheet.Cells[irow, 6].Value = data滴灌.farmerpay;
            sheet.Cells[irow, 7].Value = data滴灌.govpay;
            sheet.Cells[irow, 8].Value = (decimal)data滴灌.farmerpay + (decimal)data滴灌.govpay;
            sheet.Cells[irow, 9].Formula = "IF(H" + irow + ">0,G" + irow + "/H" + irow + ",0)";
            sheet.Cells[irow, 10].Formula = "IF(E" + irow + ">0,G" + irow + "/E" + irow + ",0)";
            sheet.Cells[irow, 11].Formula = "IF(E" + irow + ">0,H" + irow + "/E" + irow + ",0)";
            sheet.Cells[irow, 12].Formula = string.Format("CONCATENATE({0},\"/\",{1})", data滴灌.pw, data滴灌.pc);
            sheet.Cells[irow, 13].Value = data滴灌.engCount;
            irow++;
            sheet.Cells[irow, 3].Value = "微噴";
            sheet.Cells[irow, 4].Value = data微噴.caseCount;
            sheet.Cells[irow, 5].Value = data微噴.farmarea;
            sheet.Cells[irow, 6].Value = data微噴.farmerpay;
            sheet.Cells[irow, 7].Value = data微噴.govpay;
            sheet.Cells[irow, 8].Value = (decimal)data微噴.farmerpay + (decimal)data微噴.govpay;
            sheet.Cells[irow, 9].Formula = "IF(H" + irow + ">0,G" + irow + "/H" + irow + ",0)";
            sheet.Cells[irow, 10].Formula = "IF(E" + irow + ">0,G" + irow + "/E" + irow + ",0)";
            sheet.Cells[irow, 11].Formula = "IF(E" + irow + ">0,H" + irow + "/E" + irow + ",0)";
            sheet.Cells[irow, 12].Formula = string.Format("CONCATENATE({0},\"/\",{1})", data微噴.pw, data微噴.pc);
            sheet.Cells[irow, 13].Value = data微噴.engCount;
            irow++;
            sheet.Cells[irow, 3].Value = "多目標";
            sheet.Cells[irow, 4].Value = data多目標.caseCount;
            sheet.Cells[irow, 5].Value = data多目標.farmarea;
            sheet.Cells[irow, 6].Value = data多目標.farmerpay;
            sheet.Cells[irow, 7].Value = data多目標.govpay;
            sheet.Cells[irow, 8].Value = (decimal)data多目標.farmerpay + (decimal)data多目標.govpay;
            sheet.Cells[irow, 9].Formula = "IF(H" + irow + ">0,G" + irow + "/H" + irow + ",0)";
            sheet.Cells[irow, 10].Formula = "IF(E" + irow + ">0,G" + irow + "/E" + irow + ",0)";
            sheet.Cells[irow, 11].Formula = "IF(E" + irow + ">0,H" + irow + "/E" + irow + ",0)";
            sheet.Cells[irow, 12].Formula = string.Format("CONCATENATE({0},\"/\",{1})", data多目標.pw, data多目標.pc);
            sheet.Cells[irow, 13].Value = data多目標.engCount;
            irow++;
            sheet.Cells[irow, 3].Value = "其它";
            sheet.Cells[irow, 4].Value = data其它.caseCount;
            sheet.Cells[irow, 5].Value = data其它.farmarea;
            sheet.Cells[irow, 6].Value = data其它.farmerpay;
            sheet.Cells[irow, 7].Value = data其它.govpay;
            sheet.Cells[irow, 8].Value = (decimal)data其它.farmerpay + (decimal)data其它.govpay;
            sheet.Cells[irow, 9].Formula = "IF(H" + irow + ">0,G" + irow + "/H" + irow + ",0)";
            sheet.Cells[irow, 10].Formula = "IF(E" + irow + ">0,G" + irow + "/E" + irow + ",0)";
            sheet.Cells[irow, 11].Formula = "IF(E" + irow + ">0,H" + irow + "/E" + irow + ",0)";
            sheet.Cells[irow, 12].Formula = string.Format("CONCATENATE({0},\"/\",{1})", data其它.pw, data其它.pc);
            sheet.Cells[irow, 13].Value = data其它.engCount;

            irow++;
            sheet.Cells[irow, 3].Value = "小計";

            sheet.Cells[irow, 4].Formula = "Sum(D" + (irow - 6) + ":D" + (irow - 1) + ")";
            sheet.Cells[irow, 5].Formula = "Sum(E" + (irow - 6) + ":E" + (irow - 1) + ")";
            sheet.Cells[irow, 6].Formula = "Sum(F" + (irow - 6) + ":F" + (irow - 1) + ")";
            sheet.Cells[irow, 7].Formula = "Sum(G" + (irow - 6) + ":G" + (irow - 1) + ")";
            sheet.Cells[irow, 8].Formula = "Sum(H" + (irow - 6) + ":H" + (irow - 1) + ")";
            sheet.Cells[irow, 9].Formula = "IF(H" + irow + ">0,G" + irow + "/H" + irow + ",0)";
            sheet.Cells[irow, 10].Formula = "IF(E" + irow + ">0,G" + irow + "/E" + irow + ",0)";
            sheet.Cells[irow, 11].Formula = "IF(E" + irow + ">0,H" + irow + "/E" + irow + ",0)";

            sheet.Cells[irow, 12].Formula = string.Format("CONCATENATE({0},\"/\",{1})", data穿孔管.pw + data噴頭.pw + (decimal)data滴灌.pw + data微噴.pw
                + data多目標.pw + data其它.pw, data穿孔管.pc + data噴頭.pc + data滴灌.pc + data微噴.pc + data多目標.pc + data其它.pc);
            sheet.Cells[irow, 13].Formula = "Sum(M" + startrow + ":M" + (irow - 1) + ")";

            sheet.Cells[startrow, 1, irow, 2].Merge = true;
            

            for (int i = 1; i < 14; i++)
            {
                for (int j = 1; j < irow + 1; j++)
                {
                    sheet.Cells[j, i].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                }
            }

            //}

            return excel.GetAsByteArray();
        }
        private List<HistoricalData> getNewData(SqlDataReader dr)
        {
            List<HistoricalData> restul = new List<HistoricalData>();
            while (dr.Read())
            {
                HistoricalData data = new HistoricalData();
                data.caseCount = dr.GetInt32(dr.GetOrdinal("caseCount"));
                data.catalogCNS = dr.GetString(dr.GetOrdinal("catalogCNS"));
                data.farmarea = dr.GetDouble(dr.GetOrdinal("farmarea"));
                data.pw = dr["pw"] == DBNull.Value ? 0 : dr.GetInt32(dr.GetOrdinal("pw"));
                data.pc = dr["pc"] == DBNull.Value ? 0 : dr.GetInt32(dr.GetOrdinal("pc"));
                data.govpay = dr["govpay"] == DBNull.Value ? 0 : dr.GetInt32(dr.GetOrdinal("govpay"));
                data.farmerpay = dr["farmerpay"] == DBNull.Value ? 0 : dr.GetInt32(dr.GetOrdinal("farmerpay"));
                data.total = dr["total"] == DBNull.Value ? 0 : dr.GetInt32(dr.GetOrdinal("total"));
                data.dtype = dr.GetString(dr.GetOrdinal("dtype"));
                data.ApplyYear = int.Parse(dr["ApplyYear"].ToString());
                data.engCount = dr["engCount"] == DBNull.Value ? 0 : dr.GetInt32(dr.GetOrdinal("engCount"));
                switch (data.catalogCNS)
                {
                    case "其它":
                        data.sortCode = 9;
                        break;
                    case "穿孔管":
                        data.sortCode = 2;
                        break;
                    case "噴頭":
                        data.sortCode = 3;
                        break;
                    case "滴灌":
                        data.sortCode = 4;
                        break;
                    case "微噴":
                        data.sortCode = 5;
                        break;
                    default:
                        data.sortCode = 8;
                        break;
                }
                switch (data.dtype)
                {
                    case "推廣":
                        data.sortCode0 = 3;
                        break;
                    case "七星":
                        data.sortCode0 = 2;
                        break;
                    default:
                        data.sortCode0 = 1;
                        break;
                }
                restul.Add(data);

            }
            return restul;

        }
        /// <summary>
        /// 取得單位歷年補助資料
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="endYear"></param>
        /// <param name="startYear"></param>
        /// <returns></returns>
        private List<HistoricalData> getStatDataByUnit(int unit, int endYear, int startYear)
        {
            string cnstring = ConfigurationManager.ConnectionStrings["DryDB"].ConnectionString;
            List<HistoricalData> result = new List<HistoricalData>();
            using (SqlConnection cn = new SqlConnection())
            {
                cn.ConnectionString = cnstring;
                cn.Open();

                SqlCommand cmd = new SqlCommand();
                StringBuilder cmdline = new StringBuilder();
                cmdline.AppendLine("select COUNT(*) as caseCount,catalogCNS,SUM(buildarea) / 10000 as farmarea,");
                cmdline.AppendLine("SUM(tab1.pw) as pw,SUM(tab1.pc) as pc,SUM(b.GovSubsidy+b.GoldSubsidy) as govpay,");
                cmdline.AppendLine("SUM(b.FarmerFee) as farmerpay,SUM(b.total) as total,N'瑠公' as dtype,a.ApplyYear");
                cmdline.AppendLine(",sum(tab2.engCount) as engCount from CaseDetail a");
                cmdline.AppendLine("left join (select SUM(poolweight) as pw,COUNT(*) as pc,MapNo from [Pool] group by MapNo) tab1");
                cmdline.AppendLine("on a.MapNo = tab1.MapNo left join TotalFee b on a.MapNo = b.MapNo");
                cmdline.AppendLine("left join (select COUNT(*) as engCount,MapNo from Engine group by mapno) tab2 on a.MapNo=tab2.MapNo");
                cmdline.AppendLine("where a.ApplyUnit=" + unit + " and a.ApplyYear <= " + endYear);
                cmdline.AppendLine(" and a.mapno in (select mapno from CasePayUnit where GovPayUnit=1 and ApplyUnit = 17) ");
                //cmdline.AppendLine(" and a.mapno in (select a.mapno from [PoolApply]a,[Pool]b where a.MapNo=b.MapNo and applyunit=17 and poolprice>0 ");
                //cmdline.AppendLine("union select a.mapno from [EngApply]a,[Engine]b where a.MapNo=b.MapNo and ApplyUnit=17 and EngPrice >0) ");
                cmdline.AppendLine("and a.Complete = 1 and a.gold = 0 group by ApplyYear,CatalogCNS union all");
                //cx
                cmdline.AppendLine("select COUNT(*) as caseCount,catalogCNS,SUM(buildarea) / 10000 as farmarea,");
                cmdline.AppendLine("SUM(tab1.pw) as pw,SUM(tab1.pc) as pc,SUM(b.GovSubsidy+b.GoldSubsidy) as govpay,");
                cmdline.AppendLine("SUM(b.FarmerFee) as farmerpay,SUM(b.total) as total,N'七星' as dtype,a.ApplyYear");
                cmdline.AppendLine(",sum(tab2.engCount) as engCount from CaseDetail a");
                cmdline.AppendLine("left join (select SUM(poolweight) as pw,COUNT(*) as pc,MapNo from [Pool] group by MapNo) tab1");
                cmdline.AppendLine("on a.MapNo = tab1.MapNo left join TotalFee b on a.MapNo = b.MapNo");
                cmdline.AppendLine("left join (select COUNT(*) as engCount,MapNo from Engine group by mapno) tab2 on a.MapNo=tab2.MapNo");
                cmdline.AppendLine("where a.ApplyUnit=" + unit + " and a.ApplyYear <= " + endYear);
                cmdline.AppendLine(" and a.mapno in (select mapno from CasePayUnit where GovPayUnit=1 and ApplyUnit = 16) ");
                //cmdline.AppendLine(" and a.mapno in (select a.mapno from [PoolApply]a,[Pool]b where a.MapNo=b.MapNo and applyunit=16 and poolprice>0 ");
                //cmdline.AppendLine("union select a.mapno from [EngApply]a,[Engine]b where a.MapNo=b.MapNo and ApplyUnit=16 and EngPrice >0) ");
                cmdline.AppendLine("and a.Complete = 1 and a.gold = 0 group by ApplyYear,CatalogCNS union all");
                //coa
                cmdline.AppendLine("select COUNT(*) as caseCount,catalogCNS,SUM(buildarea) / 10000 as farmarea,");
                cmdline.AppendLine("SUM(tab1.pw) as pw,SUM(tab1.pc) as pc,SUM(b.GovSubsidy+b.GoldSubsidy) as govpay,");
                cmdline.AppendLine("SUM(b.FarmerFee) as farmerpay,SUM(b.total) as total,'推廣' as dtype,a.ApplyYear");
                cmdline.AppendLine(",sum(tab2.engCount) as engCount from CaseDetail a");
                cmdline.AppendLine("left join (select SUM(poolweight) as pw,COUNT(*) as pc,MapNo from [Pool] group by MapNo) tab1");
                cmdline.AppendLine("on a.MapNo = tab1.MapNo left join TotalFee b on a.MapNo = b.MapNo");
                cmdline.AppendLine("left join (select COUNT(*) as engCount,MapNo from Engine group by mapno) tab2 on a.MapNo=tab2.MapNo");
                cmdline.AppendLine("where a.ApplyUnit=" + unit + " and a.ApplyYear <= " + endYear);
                cmdline.AppendLine(" and a.mapno in (select mapno from CasePayUnit where GovPayUnit=1 and ApplyUnit = 0) ");
                //cmdline.AppendLine(" and a.mapno not in (select a.mapno from [PoolApply]a,[pool]b where a.MapNo=b.MapNo and (applyunit=17 or applyunit=16) and poolprice>0 ");
                //cmdline.AppendLine("union  select a.mapno from [EngApply]a,[Engine] b where  a.MapNo=b.MapNo and (applyunit=17 or applyunit=16) and EngPrice >0) ");
                cmdline.AppendLine("and a.Complete = 1 and a.gold = 0 group by ApplyYear,CatalogCNS union all");
                //gold
                cmdline.AppendLine("select COUNT(*) as caseCount,catalogCNS,SUM(buildarea) / 10000 as farmarea,");
                cmdline.AppendLine("SUM(tab1.pw) as pw,SUM(tab1.pc) as pc,SUM(b.GovSubsidy+b.GoldSubsidy) as govpay,");
                cmdline.AppendLine("SUM(b.FarmerFee) as farmerpay,SUM(b.total) as total,'推廣' as dtype,a.ApplyYear");
                cmdline.AppendLine(",sum(tab2.engCount) as engCount from CaseDetail a");
                cmdline.AppendLine("left join (select SUM(poolweight) as pw,COUNT(*) as pc,MapNo from [Pool] group by MapNo) tab1");
                cmdline.AppendLine("on a.MapNo = tab1.MapNo left join TotalFee b on a.MapNo = b.MapNo");
                cmdline.AppendLine("left join (select COUNT(*) as engCount,MapNo from Engine group by mapno) tab2 on a.MapNo=tab2.MapNo");
                cmdline.AppendLine("where a.ApplyUnit=" + unit + " and a.ApplyYear <= " + endYear);
                cmdline.AppendLine(" and a.Complete = 1 and a.gold = 1 group by ApplyYear,CatalogCNS union all");


                cmdline.AppendLine("select caseCount,catalogCns,farmarea,pw,pc,govpay,farmerpay,farmerpay+govpay as total,dtype,ApplyYear,engcount");
                cmdline.AppendLine("from drydb.dbo.HistoricalStat where UnitId=" + unit + "  order by applyyear,dtype,catalogcns");

                cmd.CommandText = cmdline.ToString();
                cmd.Connection = cn;
                SqlDataReader dr = cmd.ExecuteReader();
                
                List<HistoricalData> newdata = getNewData(dr);
                var sourcedata = from o in newdata

                                 group o by new { o.ApplyYear, o.dtype, o.catalogCNS, o.sortCode0, o.sortCode } into m
                                 select new
                                 {
                                     ApplyYear = m.Key.ApplyYear,
                                     dtype = m.Key.dtype,
                                     catalogCNS = m.Key.catalogCNS,
                                     sortCode0 = m.Key.sortCode0,
                                     sortCode = m.Key.sortCode,
                                     caseCount = m.Sum(f => f.caseCount),
                                     farmarea = m.Sum(f => f.farmarea),
                                     pw = m.Sum(f => f.pw),
                                     pc = m.Sum(f => f.pc),
                                     govpay = m.Sum(f => f.govpay),
                                     farmerpay = m.Sum(f => f.farmerpay),
                                     total = m.Sum(f => f.farmerpay + f.govpay),
                                     engCount = m.Sum(f => f.engCount)

                                 };

                var outputData = sourcedata.Where(m => m.ApplyYear >= startYear && m.ApplyYear <= endYear).OrderBy(m => m.ApplyYear).ThenBy(m => m.sortCode0).ThenBy(m => m.sortCode);
                foreach (var item in outputData)
                {
                    HistoricalData data = new HistoricalData();
                    data.ApplyYear = item.ApplyYear;
                    data.dtype = item.dtype;
                    data.catalogCNS = item.catalogCNS;
                    data.caseCount = item.caseCount;
                    data.farmarea = item.farmarea;
                    data.pw = item.pw;
                    data.pc = item.pc;
                    data.engCount = item.engCount;
                    data.govpay = item.govpay;
                    data.farmerpay = item.farmerpay;
                    data.farmarea = item.farmarea;
                    data.total = item.total;
                   
                    result.Add(data);
                }

            }
            return result;
        }
        /// <summary>
        /// 取得全國歷年補助資料
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="endYear"></param>
        /// <param name="startYear"></param>
        /// <returns></returns>
        private List<HistoricalData> getStatDataByAll(int unit,  int startYear, int endYear)
        {
            string cnstring = ConfigurationManager.ConnectionStrings["DryDB"].ConnectionString;
            List<HistoricalData> result = new List<HistoricalData>();
            using (SqlConnection cn = new SqlConnection())
            {
                cn.ConnectionString = cnstring;
                cn.Open();

                SqlCommand cmd = new SqlCommand();
                StringBuilder cmdline = new StringBuilder();
                cmdline.AppendLine("select COUNT(*) as caseCount,catalogCNS,SUM(buildarea) / 10000 as farmarea,");
                cmdline.AppendLine("SUM(tab1.pw) as pw,SUM(tab1.pc) as pc,SUM(b.GovSubsidy+b.GoldSubsidy) as govpay,");
                cmdline.AppendLine("SUM(b.FarmerFee) as farmerpay,SUM(b.total) as total,N'瑠公' as dtype,a.ApplyYear");
                cmdline.AppendLine(",sum(tab2.engCount) as engCount from CaseDetail a");
                cmdline.AppendLine("left join (select SUM(poolweight) as pw,COUNT(*) as pc,MapNo from [Pool] group by MapNo) tab1");
                cmdline.AppendLine("on a.MapNo = tab1.MapNo left join TotalFee b on a.MapNo = b.MapNo");
                cmdline.AppendLine("left join (select COUNT(*) as engCount,MapNo from Engine group by mapno) tab2 on a.MapNo=tab2.MapNo");
                cmdline.AppendLine("where a.ApplyYear <= " + endYear);
                cmdline.AppendLine(" and a.mapno in (select mapno from CasePayUnit where GovPayUnit=1 and ApplyUnit = 17) ");
                //cmdline.AppendLine(" and a.mapno in (select a.mapno from [PoolApply]a,[Pool]b where a.MapNo=b.MapNo and applyunit=17 and poolprice>0 ");
                //cmdline.AppendLine("union select a.mapno from [EngApply]a,[Engine]b where a.MapNo=b.MapNo and ApplyUnit=17 and EngPrice >0) ");
                cmdline.AppendLine("and a.Complete = 1 and a.gold = 0 group by ApplyYear,CatalogCNS union all");
                
                cmdline.AppendLine("select COUNT(*) as caseCount,catalogCNS,SUM(buildarea) / 10000 as farmarea,");
                cmdline.AppendLine("SUM(tab1.pw) as pw,SUM(tab1.pc) as pc,SUM(b.GovSubsidy+b.GoldSubsidy) as govpay,");
                cmdline.AppendLine("SUM(b.FarmerFee) as farmerpay,SUM(b.total) as total,N'七星' as dtype,a.ApplyYear");
                cmdline.AppendLine(",sum(tab2.engCount) as engCount from CaseDetail a");
                cmdline.AppendLine("left join (select SUM(poolweight) as pw,COUNT(*) as pc,MapNo from [Pool] group by MapNo) tab1");
                cmdline.AppendLine("on a.MapNo = tab1.MapNo left join TotalFee b on a.MapNo = b.MapNo");
                cmdline.AppendLine("left join (select COUNT(*) as engCount,MapNo from Engine group by mapno) tab2 on a.MapNo=tab2.MapNo");
                cmdline.AppendLine("where a.ApplyYear <= " + endYear);
                cmdline.AppendLine(" and a.mapno in (select mapno from CasePayUnit where GovPayUnit=1 and ApplyUnit = 16) ");
                //cmdline.AppendLine(" and a.mapno in (select a.mapno from [PoolApply]a,[Pool]b where a.MapNo=b.MapNo and applyunit=16 and poolprice>0 ");
                //cmdline.AppendLine("union select a.mapno from [EngApply]a,[Engine]b where a.MapNo=b.MapNo and ApplyUnit=16 and EngPrice >0) ");
                cmdline.AppendLine("and a.Complete = 1 and a.gold = 0 group by ApplyYear,CatalogCNS union all");
                
                cmdline.AppendLine("select COUNT(*) as caseCount,catalogCNS,SUM(buildarea) / 10000 as farmarea,");
                cmdline.AppendLine("SUM(tab1.pw) as pw,SUM(tab1.pc) as pc,SUM(b.GovSubsidy+b.GoldSubsidy) as govpay,");
                cmdline.AppendLine("SUM(b.FarmerFee) as farmerpay,SUM(b.total) as total,'推廣' as dtype,a.ApplyYear");
                cmdline.AppendLine(",sum(tab2.engCount) as engCount from CaseDetail a");
                cmdline.AppendLine("left join (select SUM(poolweight) as pw,COUNT(*) as pc,MapNo from [Pool] group by MapNo) tab1");
                cmdline.AppendLine("on a.MapNo = tab1.MapNo left join TotalFee b on a.MapNo = b.MapNo");
                cmdline.AppendLine("left join (select COUNT(*) as engCount,MapNo from Engine group by mapno) tab2 on a.MapNo=tab2.MapNo");
                cmdline.AppendLine("where a.ApplyYear <= " + endYear);
                cmdline.AppendLine(" and a.mapno in (select mapno from CasePayUnit where GovPayUnit=1 and ApplyUnit = 0) ");
                //cmdline.AppendLine(" and a.mapno not in (select a.mapno from [PoolApply]a,[pool]b where a.MapNo=b.MapNo and (applyunit=17 or applyunit=16) and poolprice>0 ");
                //cmdline.AppendLine("union  select a.mapno from [EngApply]a,[Engine] b where  a.MapNo=b.MapNo and (applyunit=17 or applyunit=16) and EngPrice >0) ");
                
                cmdline.AppendLine("and a.Complete = 1 and a.gold = 0 group by ApplyYear,CatalogCNS union all");
                cmdline.AppendLine("select COUNT(*) as caseCount,catalogCNS,SUM(buildarea) / 10000 as farmarea,");
                cmdline.AppendLine("SUM(tab1.pw) as pw,SUM(tab1.pc) as pc,SUM(b.GovSubsidy+b.GoldSubsidy) as govpay,");
                cmdline.AppendLine("SUM(b.FarmerFee) as farmerpay,SUM(b.total) as total,'推廣' as dtype,a.ApplyYear");
                cmdline.AppendLine(",sum(tab2.engCount) as engCount from CaseDetail a");
                cmdline.AppendLine("left join (select SUM(poolweight) as pw,COUNT(*) as pc,MapNo from [Pool] group by MapNo) tab1");
                cmdline.AppendLine("on a.MapNo = tab1.MapNo left join TotalFee b on a.MapNo = b.MapNo");
                cmdline.AppendLine("left join (select COUNT(*) as engCount,MapNo from Engine group by mapno) tab2 on a.MapNo=tab2.MapNo");
                cmdline.AppendLine("where a.ApplyYear <= " + endYear);
                cmdline.AppendLine(" and a.Complete = 1 and a.gold = 1 group by ApplyYear,CatalogCNS union all");


                cmdline.AppendLine("select caseCount,catalogCns,farmarea,pw,pc,govpay,farmerpay,farmerpay+govpay as total,dtype,ApplyYear,engcount");
                cmdline.AppendLine("from drydb.dbo.HistoricalStat order by applyyear,dtype,catalogcns");
                //cmdline.AppendLine("from drydb.dbo.HistoricalStat where UnitId=" + unit + "  order by applyyear,dtype,catalogcns");

                cmd.CommandText = cmdline.ToString();
                cmd.Connection = cn;
                SqlDataReader dr = cmd.ExecuteReader();
                
                List<HistoricalData> newdata = getNewData(dr);
                var sourcedata = from o in newdata

                                 group o by new { o.ApplyYear, o.dtype, o.catalogCNS, o.sortCode0, o.sortCode } into m
                                 select new
                                 {
                                     ApplyYear = m.Key.ApplyYear,
                                     dtype = m.Key.dtype,
                                     catalogCNS = m.Key.catalogCNS,
                                     sortCode0 = m.Key.sortCode0,
                                     sortCode = m.Key.sortCode,
                                     caseCount = m.Sum(f => f.caseCount),
                                     farmarea = m.Sum(f => f.farmarea),
                                     pw = m.Sum(f => f.pw),
                                     pc = m.Sum(f => f.pc),
                                     govpay = m.Sum(f => f.govpay),
                                     farmerpay = m.Sum(f => f.farmerpay),
                                     total = m.Sum(f => f.farmerpay + f.govpay),
                                     engCount = m.Sum(f => f.engCount)

                                 };

                var outputData = sourcedata.Where(m => m.ApplyYear >= startYear && m.ApplyYear <= endYear).OrderBy(m => m.ApplyYear).ThenBy(m => m.sortCode0).ThenBy(m => m.sortCode);
                foreach (var item in outputData)
                {
                    HistoricalData data = new HistoricalData();
                    data.ApplyYear = item.ApplyYear;
                    data.dtype = item.dtype;
                    data.catalogCNS = item.catalogCNS;
                    data.caseCount = item.caseCount;
                    data.farmarea = item.farmarea;
                    data.pw = item.pw;
                    data.pc = item.pc;
                    data.engCount = item.engCount;
                    data.govpay = item.govpay;
                    data.farmerpay = item.farmerpay;
                    data.farmarea = item.farmarea;
                    data.total = item.total;

                    result.Add(data);
                }

            }
            return result;
        }
        struct HistoricalData
        {
            public int caseCount { get; set; }
            public string catalogCNS { get; set; }
            public double farmarea { get; set; }
            public int pw { get; set; }
            public int pc { get; set; }
            public int govpay { get; set; }
            public int farmerpay { get; set; }
            public int total { get; set; }
            public string dtype { get; set; }
            public int ApplyYear { get; set; }
            public int engCount { get; set; }
            public int sortCode { get; set; }
            public int sortCode0 { get; set; }
        }
    }

}







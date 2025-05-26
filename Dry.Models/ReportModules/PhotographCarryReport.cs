using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPOI.HSSF.UserModel;
using CusNPOI;
using System.IO;
using System.Web;
using Dry.Models.CommonCls;

namespace Dry.Models.ReportModules
{
    public class PhotographCarryReport
    {
        DryEntities DryDB = new DryEntities();
        public byte[] GetReport_PhotographCarry(short unit, int year, int IANumStart, int IANumEnd, string sourcepath)
        {
            GetData gd = new GetData();
            List<DataStruct> data = new List<DataStruct>();
            List<Case> cases = DryDB.Case.Where(m => m.IANum >= IANumStart && m.IANum <= IANumEnd && m.ApplyUnit == unit && m.ApplyYear == year).ToList();
            List<AERC.Models.Crop> crop = new AERC.Models.CommonCls.GetCrop().GetCropList();
            var resdata = (from summview in DryDB.SummaryView
                           join lasvermno in DryDB.LastVerOfMapNo on summview.MapNo equals lasvermno.MapNo
                           where summview.ApplyUnit == unit && summview.ApplyYear == year && summview.IANum >= IANumStart && summview.IANum <= IANumEnd
                           select new
                           {
                               summview.MapNo,
                               summview.IANum,
                               summview.Name,
                               summview.IdNo,
                               summview.Tel,
                               summview.Addr,
                               summview.Phone
                           }).OrderBy(m => m.IANum).ToList();
            foreach (var caseItem in resdata)
            {
                DataStruct insertData = new DataStruct();
                insertData.IAName = gd.GetUnitName(unit);
                insertData.ApplyYear = year;
                insertData.IANum = caseItem.IANum;
                insertData.FarmerName = caseItem.Name;
                insertData.Tel = caseItem.Tel;
                insertData.Phone = caseItem.Phone;
                insertData.Addr = caseItem.Addr.Replace(" ","");
                var farmData = (from farm in DryDB.Farm
                                join landdata in DryDB.LandData on farm.Section equals landdata.Section_Id
                                where farm.MapNo == caseItem.MapNo
                                select new
                                {
                                    farm.FNo,
                                    landdata.Town,
                                    landdata.Section,
                                    landdata.Subsection,
                                    farm.LandNo,
                                    farm.BuildArea
                                }).OrderByDescending(m => m.Section).ToList();
                List<FarmList> farmList = new List<FarmList>();
                foreach(var farmItem in farmData)
                {
                    FarmList farm = new FarmList();
                    farm.Town = farmItem.Town;
                    farm.SectionName = farmItem.Section + (farmItem.Subsection.Length > 0 ? "-" + farmItem.Subsection : "");
                    farm.LandNo = farmItem.LandNo;
                    farm.Area = farmItem.BuildArea / 10000;

                    var crops = DryDB.FarmCrop.Where(m => m.FNo == farmItem.FNo).ToList();
                    farm.Crop = new List<string>();
                    foreach(var cropItem in crops)
                    {
                        string cropName = crop.Any(m => m.Crop_Id == cropItem.CropCode) ? crop.Find(m => m.Crop_Id == cropItem.CropCode).Crop1 : "";
                        farm.Crop.Add("□" + cropName);
                    }
                    var irrType = (from piging in DryDB.PigingConf
                                   join endType in DryDB.EndType on piging.MapNo equals endType.MapNo
                                   join endtypeList in DryDB.EndTypeList on endType.EndTypeCode equals endtypeList.EndType
                                   join facType in DryDB.FacTypeList on endType.FacType equals facType.FacType
                                   where piging.MapNo == caseItem.MapNo
                                   select new
                                   {
                                       endtypeList.EndTypeCNS,
                                       facType.FTpeCNS
                                   }).ToList();
                    farm.EndType = new List<string>();
                    farm.FacType = new List<string>();
                    foreach (var irrItem in irrType)
                    {
                        farm.EndType.Add(irrItem.EndTypeCNS);
                        farm.FacType.Add(irrItem.FTpeCNS);
                    }
                    farmList.Add(farm);
                }
                
                insertData.FarmData = farmList;
                data.Add(insertData);
            }
            byte[] returnData = PhotographCarry(data, sourcepath);
            return returnData;
        }
        public byte[] PhotographCarry(List<DataStruct> dt, string sourcepath)
        {
            
            CusCopyRow cus = new CusCopyRow();
            FileStream fs = new FileStream(HttpContext.Current.Server.MapPath(sourcepath), FileMode.Open, FileAccess.ReadWrite);
            HSSFWorkbook wk = new HSSFWorkbook(fs);

            int RowsCount = 0;
            foreach(var item in dt)
            {
                RowsCount += item.FarmData.Count();
            }

            

            int reportlength = 23;
            int count = Convert.ToInt16(Math.Ceiling(Convert.ToDouble(RowsCount / 19))) + 1;

            HSSFSheet wksheet = wk.GetSheetAt(0) as HSSFSheet;
            
            for (int j = 1; j < count; j++)
            {
                for (int row = 0; row < 23; row++)
                {
                    
                    cus.CopyRow(wksheet, row, reportlength * j + row);
                }
            }

            SetData(wk, wksheet, dt);
            MemoryStream files = new MemoryStream();
            wk.Write(files);
            files.Close();
            return files.ToArray();
        }
        public HSSFSheet SetData(HSSFWorkbook wk, HSSFSheet sh, List<DataStruct> dt)
        {
            HSSFCellStyle cs = wk.CreateCellStyle() as HSSFCellStyle;
            HSSFFont font1 = wk.CreateFont() as HSSFFont;
            int count = 0;
            foreach (var item in dt)
            {
                count += item.FarmData.Count;
            }
            int totalpage = Convert.ToInt16(Math.Ceiling(Convert.ToDouble(count / 19))) + 1;
            int pagecount = 1;
            int RowNo = 0;
            foreach (var main_item in dt)
            {
                bool AddMainData = true;

                for (int i = 0; i < main_item.FarmData.Count;i++ )
                {
                    
                    if (RowNo == 0 || RowNo % 23 == 0)
                    {
                        sh.GetRow(RowNo).GetCell(2).SetCellValue(main_item.IAName);
                        sh.GetRow(RowNo).GetCell(5).SetCellValue(main_item.ApplyYear);
                        RowNo = RowNo + 3;
                    }
                    
                    if ((RowNo - pagecount + 1) % 22 == 0)
                    {
                        sh.GetRow(RowNo).GetCell(10).SetCellValue("第 " + pagecount + " 頁，共 " + totalpage + " 頁");
                        pagecount++;
                        //RowNo--;
                        i--;
                    }
                    else
                    {
                        if (AddMainData)
                        {
                            
                            sh.GetRow(RowNo).GetCell(0).SetCellValue(main_item.IANum);
                            sh.GetRow(RowNo).GetCell(1).SetCellValue(main_item.FarmerName);
                            string phoneline = string.Empty;
                            if (!string.IsNullOrEmpty(main_item.Tel))
                            {
                                phoneline = "P:" + main_item.Tel;
                            }
                            if (!string.IsNullOrEmpty(main_item.Phone))
                            {
                                phoneline += "M:" + main_item.Phone;
                            }
                            sh.GetRow(RowNo).GetCell(9).SetCellValue(phoneline);
                            sh.GetRow(RowNo).GetCell(10).SetCellValue(main_item.Addr);
                            AddMainData = false;
                        }

                        string Factype = "";
                        string Endtype = "";
                        string Crop = "";
                        foreach (var facItem in main_item.FarmData[i].FacType)
                        {
                            Factype += facItem + " ";
                        }
                        foreach (var endItem in main_item.FarmData[i].EndType)
                        {
                            Endtype += endItem + " ";
                        }
                        foreach (var cropItem in main_item.FarmData[i].Crop)
                        {
                            Crop += cropItem;
                        }
                        sh.GetRow(RowNo).GetCell(2).SetCellValue(main_item.FarmData[i].Town);
                        sh.GetRow(RowNo).GetCell(3).SetCellValue(main_item.FarmData[i].SectionName);
                        sh.GetRow(RowNo).GetCell(4).SetCellValue(main_item.FarmData[i].LandNo);
                        sh.GetRow(RowNo).GetCell(5).SetCellValue(main_item.FarmData[i].Area);
                        if (Endtype.Trim() != "穿孔管系統")
                        {
                            sh.GetRow(RowNo).GetCell(6).SetCellValue(Factype);
                        }
                        //sh.GetRow(RowNo).GetCell(6).SetCellValue(Factype);
                        sh.GetRow(RowNo).GetCell(7).SetCellValue(Endtype);
                        sh.GetRow(RowNo).GetCell(8).SetCellValue(Crop);
                    }
                    RowNo++;
                }
            }
            sh.GetRow(23 * pagecount - 1).GetCell(10).SetCellValue("第 " + pagecount + " 頁，共 " + totalpage + " 頁");

            return sh;
        }
        public class DataStruct
        {
            public string IAName { get; set; }
            public int ApplyYear { get; set; }
            public int IANum { get; set; }
            public string FarmerName { get; set; }
            public string Tel { get; set; }
            public string Phone { get; set; }
            public string Addr { get; set; }
            public List<FarmList> FarmData { get; set; }
        }
        public class FarmList
        {
            public string Town { get; set; }
            public string SectionName { get; set; }
            public string LandNo { get; set; }
            public double Area { get; set; }
            public List<string> EndType { get; set; }
            public List<string> FacType { get; set; }
            public List<string> Crop { get; set; }
        }
    }
}

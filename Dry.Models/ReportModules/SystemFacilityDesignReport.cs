using CusNPOI;
using NPOI.HSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Dry.Models.ReportModules
{
    public class SystemFacilityDesignReport
    {
        DryEntities DryDB = new DryEntities();
        public byte[] GetReport_SystemFacilityDesign(short unit, int year, int IANumStart, int IANumEnd, string sourcepath)
        {
            List<Farm> farms = DryDB.Farm.ToList();
            List<DataStruct> data = new List<DataStruct>();

            var resdata = (from summview in DryDB.SummaryView
                           join lasvermno in DryDB.LastVerOfMapNo on summview.MapNo equals lasvermno.MapNo
                           where summview.ApplyUnit == unit && summview.ApplyYear == year && summview.IANum >= IANumStart && summview.IANum <= IANumEnd
                           select new
                           {
                               summview.MapNo,
                               summview.IANum,
                               summview.Name,
                               summview.IdNo,
                               summview.Addr
                           }).OrderBy(m => m.IANum).ToList();
            List<EndTypeList> endTypeList = DryDB.EndTypeList.ToList();
            List<FacTypeList> facTypeList = DryDB.FacTypeList.ToList();
            foreach(var main_item in resdata)
            {
                DataStruct datastruct = new DataStruct();
                ApplyList list = new ApplyList();
                
                int MapNo = main_item.MapNo.Value;
                List<EndType> endType = DryDB.EndType.Where(m => m.MapNo == MapNo).ToList();
                PigingConf piging = DryDB.PigingConf.Find(MapNo);
                datastruct.FarmerName = main_item.Name;
                datastruct.IANum = main_item.IANum;
                if(piging!=null)
                {
                    datastruct.L1 = piging.L1;
                    datastruct.L2 = piging.L2;
                }
                datastruct.ApplyData = new List<ApplyList>();
                foreach(var item in endType)
                {
                    if (item.EndTypeCode == 1)
                    {
                        list.FacEndType += endTypeList.Find(m => m.EndType == item.EndTypeCode).EndTypeCNS + " ";
                    }
                    else
                    {
                        list.FacEndType += facTypeList.Find(m => m.FacType == item.FacType).FTpeCNS + endTypeList.Find(m => m.EndType == item.EndTypeCode).EndTypeCNS + " ";
                    }
                    
                }
                if(endType.Count > 0)
                {
                    list.EndType = endType.FirstOrDefault().EndTypeCode;
                    list.SS = endType.FirstOrDefault().SS;
                    list.SL = endType.FirstOrDefault().SL;
                }
                
                
                var farmData = (from farm in farms
                                join landdata in DryDB.LandData on farm.Section equals landdata.Section_Id
                                where farm.MapNo == MapNo
                                select new
                                {
                                    farm.FNo,
                                    landdata.City,
                                    landdata.Town,
                                    landdata.Section,
                                    landdata.Subsection,
                                    farm.LandNo,
                                    farm.BuildArea
                                }).OrderByDescending(m => m.Section).ToList();
                if(farmData.Count > 0)
                {
                    list.Town = farmData.FirstOrDefault().Town;
                    list.SectionName = farmData.FirstOrDefault().Section;
                    list.SubSectionName = farmData.FirstOrDefault().Subsection;
                    int temp = 1;
                    foreach (var landno in farmData)
                    {
                        list.LandNo += landno.LandNo;
                        if (temp < farmData.Count)
                        {
                            list.LandNo += "、";
                        }
                        temp++;
                    }
                }
                
                datastruct.ApplyData.Add(list);
                data.Add(datastruct);
            }
            byte[] returnData = SFDesign(data, sourcepath);
            return returnData;
        }
        private byte[] SFDesign(List<DataStruct> dt, string sourcepath)
        {
            List<DataStruct> dt0 = new List<DataStruct>();
            List<DataStruct> dt1 = new List<DataStruct>();
            foreach(var item in dt)
            {
                if(item.ApplyData.Any(m=>m.EndType == 1))
                {
                    dt0.Add(item);
                }
                else
                {
                    dt1.Add(item);
                }
            }
            CusCopyRow cus = new CusCopyRow();
            FileStream fs = new FileStream(HttpContext.Current.Server.MapPath(sourcepath), FileMode.Open, FileAccess.ReadWrite);
            HSSFWorkbook wk = new HSSFWorkbook(fs);
            int count = dt.Count;
            int reportlength = 50;
            int HoldCount = dt0.Count;
            int DripCount = dt1.Count;

            HSSFSheet wksheet = wk.GetSheetAt(0) as HSSFSheet;
            for (int j = 1; j < HoldCount; j++)
            {
                for (int row = 0; row < 50; row++)
                {
                    cus.CopyRow(wksheet, row, reportlength * j + row);
                }
            }
            SetData(wk, wksheet, dt0, reportlength,0);
            wksheet = wk.GetSheetAt(1) as HSSFSheet;
            for (int j = 1; j < DripCount; j++)
            {
                for (int row = 0; row < 50; row++)
                {
                    cus.CopyRow(wksheet, row, reportlength * j + row);
                }
            }

            SetData(wk, wksheet, dt1, reportlength,1);
            MemoryStream files = new MemoryStream();
            wk.Write(files);
            files.Close();
            return files.ToArray();
        }
        private HSSFSheet SetData(HSSFWorkbook wk, HSSFSheet sh, List<DataStruct> dt, int reportlength, byte SheetNo)
        {
            int pictureIdx;
            HSSFClientAnchor anchor;
            HSSFPicture pic;
            HSSFPatriarch pa;
            int j = 0;
            //if (SheetNo == 0)
            //{
                for (int i = 0; i < dt.Count; i++)
                {
                    sh.GetRow(1 + j).GetCell(1).SetCellValue(dt[i].FarmerName);
                    sh.GetRow(1 + j).GetCell(4).SetCellValue(dt[i].ApplyData.FirstOrDefault().FacEndType);
                    sh.GetRow(1 + j).GetCell(9).SetCellValue(dt[i].IANum);
                    sh.GetRow(2 + j).GetCell(1).SetCellValue(dt[i].ApplyData.FirstOrDefault().Town);
                    sh.GetRow(2 + j).GetCell(3).SetCellValue(dt[i].ApplyData.FirstOrDefault().SectionName);
                    sh.GetRow(2 + j).GetCell(5).SetCellValue(dt[i].ApplyData.FirstOrDefault().SubSectionName);
                    sh.GetRow(3 + j).GetCell(1).SetCellValue(dt[i].ApplyData.FirstOrDefault().LandNo);
                    sh.GetRow(4 + j).GetCell(1).SetCellValue(dt[i].ApplyData.FirstOrDefault().SL);
                    if (SheetNo == 0)
                    {
                        sh.GetRow(4 + j).GetCell(6).SetCellValue(dt[i].L1);
                    }
                    else
                    {
                        sh.GetRow(4 + j).GetCell(5).SetCellValue(dt[i].ApplyData.FirstOrDefault().SS);
                        sh.GetRow(4 + j).GetCell(9).SetCellValue(dt[i].L1);
                    }
                   
                    //var image = File.ReadAllBytes(HttpContext.Current.Server.MapPath(dt.Rows[0][9].ToString()));
                    //pictureIdx = wk.AddPicture(image, PictureType.JPEG);
                    //pa = sh.CreateDrawingPatriarch() as HSSFPatriarch;
                    //anchor = new HSSFClientAnchor(0, 0, 400, 0, 0, reportlength * i + 8, 8, 300);
                    //pic = pa.CreatePicture(anchor, pictureIdx) as HSSFPicture;
                    //pic.Resize();
                    j = j + reportlength;
                }
            //}
            //else
            //{
            //    for (int i = 0; i < count; i++)
            //    {
            //        sh.GetRow(1 + j).GetCell(1).SetCellValue(dt.Rows[i][0].ToString());
            //        sh.GetRow(1 + j).GetCell(4).SetCellValue(dt.Rows[i][1].ToString());
            //        sh.GetRow(1 + j).GetCell(9).SetCellValue(dt.Rows[i][2].ToString());
            //        sh.GetRow(2 + j).GetCell(1).SetCellValue(dt.Rows[i][3].ToString());
            //        sh.GetRow(2 + j).GetCell(3).SetCellValue(dt.Rows[i][4].ToString());
            //        sh.GetRow(2 + j).GetCell(5).SetCellValue(dt.Rows[i][5].ToString());
            //        sh.GetRow(3 + j).GetCell(1).SetCellValue(dt.Rows[i][6].ToString());
            //        sh.GetRow(4 + j).GetCell(1).SetCellValue(dt.Rows[i][7].ToString());
            //        sh.GetRow(4 + j).GetCell(5).SetCellValue(dt.Rows[i][8].ToString());
            //        sh.GetRow(4 + j).GetCell(9).SetCellValue(dt.Rows[i][9].ToString());
            //        var image = File.ReadAllBytes(HttpContext.Current.Server.MapPath(dt.Rows[0][10].ToString()));
            //        pictureIdx = wk.AddPicture(image, PictureType.JPEG);
            //        pa = sh.CreateDrawingPatriarch() as HSSFPatriarch;
            //        anchor = new HSSFClientAnchor(0, 0, 400, 0, 0, reportlength * i + 8, 8, 3000);
            //        pic = pa.CreatePicture(anchor, pictureIdx) as HSSFPicture;
            //        j = j + reportlength;
            //    }
            //}

            return sh;
        }
        public class DataStruct
        {
            public int IANum { get; set; }
            public string FarmerName { get; set; }
            public double L1 { get; set; }
            public double L2 { get; set; }
            public List<ApplyList> ApplyData { get; set; }
        }
        public class ApplyList
        {
            public string Town { get; set; }
            public string SectionName { get; set; }
            public string SubSectionName { get; set; }
            public string LandNo { get; set; }
            public byte EndType { get; set; }
            public string FacEndType { get; set; }
            public double SS { get; set; }
            public double SL { get; set; }
        }
    }
}

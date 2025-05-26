using Dry.Models.CommonCls;
using Dry.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Dry.Models.Service
{
    public class ExamineDBService
    {
        private DryEntities DryDB = new DryEntities();
        private GetData getData = new GetData();
        private CommClass comm = new CommClass();

        public Examine GetExamineData(int MapNo)
        {
            return DryDB.Examine.Find(MapNo);
        }

        /// <summary>
        /// 寫入Examine資料表格
        /// </summary>
        /// <param name="ExamineData">ExamineView資料實體</param>
        /// <param name="MapNo">版本編號</param>
        /// <returns></returns>
        public DBCommon.DbEvent CreateData(ExamineJsonData ExamineData, int MapNo)
        {
            DBCommon.DbEvent dbstatus = new DBCommon.DbEvent();
            Examine exam = getData.GetExamineData(MapNo);
            //exam.MapNo = MapNo;
            //exam.EnginePnt = "";
            //exam.WaterPnt = "";
            //exam.Reason = ExamineData.Reason;
            //exam.Result = ExamineData.Result;
            //exam.Examiner = ExamineData.Examiner;
            //exam.EDate = Convert.ToDateTime(ExamineData.EDate);
            //exam.Note = ExamineData.Note;
            EFile eFile = new EFile();
            eFile.MapNo = MapNo;
            
            using (TransactionScope scope = new TransactionScope())
            {
                dbstatus = CreateExamine(exam, ExamineData, MapNo);
                if (dbstatus.DbMessage == "Success")
                {
                    dbstatus = CreateImgFile(ExamineData.ImgData, MapNo);

                    if (dbstatus.DbMessage.Equals("Success"))
                    {
                        dbstatus.DbMessage = comm.UpdateCase(MapNo, 7).DbMessage;
                    }
                    if (dbstatus.DbMessage == "Success")
                        scope.Complete();
                }
            }
            return dbstatus;
        }

        private DBCommon.DbEvent CreateExamine(Examine exam, ExamineJsonData ExamineData, int MapNo)
        {
            DBCommon.DbEvent res = new DBCommon.DbEvent();
            
            
            if (exam == null)
            {
                exam = new Examine();
                exam.MapNo = MapNo;
                exam.EnginePnt = "";
                exam.WaterPnt = "";
                exam.Reason = ExamineData.Reason ?? "";
                exam.Result = ExamineData.Result;
                exam.Examiner = ExamineData.Examiner;
                exam.EDate = Convert.ToDateTime(ExamineData.EDate);
                exam.Note = ExamineData.Note;
                DryDB.Examine.Add(exam);
                
                new CommClass().UpdateCase(MapNo, 7);
            }
            else
            {
                exam = new Examine();
                exam.MapNo = MapNo;
                exam.EnginePnt = "";
                exam.WaterPnt = "";
                exam.Reason = ExamineData.Reason;
                exam.Result = ExamineData.Result;
                exam.Examiner = ExamineData.Examiner;
                exam.EDate = Convert.ToDateTime(ExamineData.EDate);
                exam.Note = ExamineData.Note;
                DryDB.Examine.Attach(exam);
                DryDB.Entry(exam).State = System.Data.EntityState.Modified;
            }
            try
            {
                DryDB.SaveChanges();
                res.DbMessage = "Success";
            }
            catch (Exception e)
            {
                res.DbMessage = "Inserted Examine Failed : " + e.Message;
            }
            return res;
        }
        private DBCommon.DbEvent CreateSubsidy()
        {
            DBCommon.DbEvent res = new DBCommon.DbEvent();

            return res;
        }

        /// <summary>
        /// 取得檔案類型列表(現勘專用)
        /// </summary>
        /// <returns></returns>
        public List<FileTypeList> GetFileTypeList()
        {
            return DryDB.FileTypeList.Where(m => m.FileType == 15 || m.FileType == 16).ToList();
        }
        /// <summary>
        /// 取得照片(檔案類型參照GetFileTypeList())
        /// </summary>
        /// <param name="MapNo"></param>
        /// <returns></returns>
        public List<EFile> GetEFileList(int MapNo)
        {
            return DryDB.EFile.Where(m => m.MapNo == MapNo && (m.FileType == 15 || m.FileType == 16) && m.Enable == true).ToList();
        }
        /// <summary>
        /// 寫入上傳圖片的檔名
        /// </summary>
        /// <param name="eFile"></param>
        /// <returns></returns>
        public DBCommon.DbEvent CreateImgFile(List<ImgDataView> imgData,int MapNo)
        {
            DBCommon.DbEvent res = new DBCommon.DbEvent();
            if (imgData != null)
            {
                foreach (ImgDataView data in imgData)
                {
                    EFile eFile = new EFile();
                    eFile.MapNo = MapNo;
                    eFile.FileType = data.FileType;
                    eFile.Filepath = data.FilePath;
                    eFile.Coordinate = data.Coordinate;
                    eFile.UpldTime = DateTime.Now;
                    eFile.Enable = true;
                    if (!IsImgFileExist(MapNo, data.FilePath))
                        DryDB.EFile.Add(eFile);
                }
                try
                {
                    DryDB.SaveChanges();
                    res.DbMessage = "Success";
                }
                catch (Exception e)
                {
                    res.DbMessage = "Inserted EFile Failed : " + e.Message;
                }
            }
            else
            {
                res.DbMessage = "Success";
            }
            
            return res;
        }
        private bool IsImgFileExist(int MapNo, string fileName)
        {
            List<EFile> eFileList = GetEFileList(MapNo);
            foreach (EFile item in eFileList)
            {
                if (fileName.Equals(item.Filepath))
                    return true;
            }
            return false;
        }
        /// <summary>
        /// 複製整個資料夾
        /// </summary>
        /// <param name="srcDirectory">來源資料夾</param>
        /// <param name="dstDirectory">目的資料夾</param>
        public static void CopyDirectory(string srcDirectory, string dstDirectory)
        {
            if (!Directory.Exists(dstDirectory))
            {
                Directory.CreateDirectory(dstDirectory);
            }

            DirectoryInfo sdir = new DirectoryInfo(srcDirectory);
            foreach (FileInfo fi in sdir.GetFiles())
            {
                File.Copy(fi.FullName, dstDirectory + Path.DirectorySeparatorChar + fi.Name);
            }
            foreach (DirectoryInfo di in sdir.GetDirectories())
            {
                CopyDirectory(di.FullName, dstDirectory + Path.DirectorySeparatorChar + di.Name);
            }
        }

        /// <summary>
        /// 刪除照片
        /// </summary>
        /// <param name="fileName">檔案名稱</param>
        /// <returns></returns>
        public DBCommon.DbEvent DeleteImg(string fileName)
        {
            IQueryable<EFile> ef = DryDB.EFile.Where(p => p.Filepath == fileName);
            DBCommon.DbEvent dbMsg = new DBCommon.DbEvent();
            EFile[] efList = ef.ToArray();
            foreach(EFile efile in efList)
            {
                efile.Enable = false;
                DryDB.EFile.Attach(efile);
                DryDB.Entry(efile).State = System.Data.EntityState.Modified;
            }
            try
            {
                DryDB.SaveChanges();
                dbMsg.DbMessage = "Success";
            }
            catch(Exception e)
            {
                dbMsg.DbMessage = "Removed EFile Failed : " + e.Message;
            }
            return dbMsg;
        }
    }
}

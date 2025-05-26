using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dry.Models.ViewModel;

namespace Dry.Models.Service
{
    public class EfileDBService
    {
        private DryEntities DryDB = new DryEntities();
        private DBCommon.DbEvent dbMsg = new DBCommon.DbEvent();
        /// <summary>
        /// 上傳檔案路徑寫入資料庫
        /// </summary>
        /// <param name="fileData"></param>
        /// <returns></returns>
        public DBCommon.DbEvent CreateImg(EfileView fileData)
        {
            EFile eFile = new EFile();
            eFile.MapNo = fileData.MapNo;
            eFile.FileType = fileData.FileType;
            eFile.Filepath = fileData.Filepath;
            eFile.FileDes = fileData.FileDes;
            eFile.UpldTime = DateTime.Now;
            eFile.Enable = true;
            try
            {
                DryDB.EFile.Add(eFile);
                DryDB.SaveChanges();
                dbMsg.DbMessage = "Success";
                dbMsg.KeyValue = eFile.No;
            }
            catch (Exception e)
            {
                dbMsg.DbMessage = "Inserted EFile Failed : " + e.Message;
            }
            return dbMsg;
        }

        /// <summary>
        /// 刪除照片
        /// </summary>
        /// <param name="fileName">檔案名稱</param>
        /// <returns></returns>
        public DBCommon.DbEvent DeleteImg(string fileName)
        {
            IQueryable<EFile> ef = DryDB.EFile.Where(p => p.Filepath == fileName);
            
            EFile[] efList = ef.ToArray();
            foreach (EFile efile in efList)
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
            catch (Exception e)
            {
                dbMsg.DbMessage = "Removed EFile Failed : " + e.Message;
            }
            return dbMsg;
        }

        /// <summary>
        /// 刪除檔案
        /// </summary>
        /// <param name="No">流水號</param>
        /// <returns></returns>
        public DBCommon.DbEvent DeleteFile(int No)
        {
            EFile ef = DryDB.EFile.Single(p => p.No == No);

            ef.Enable = false;
            DryDB.EFile.Attach(ef);
            DryDB.Entry(ef).State = System.Data.EntityState.Modified;
            try
            {
                DryDB.SaveChanges();
                dbMsg.DbMessage = "Success";
            }
            catch (Exception e)
            {
                dbMsg.DbMessage = "Removed EFile Failed : " + e.Message;
            }
            return dbMsg;
        }
        /// <summary>
        /// 取得已上傳檔案資料
        /// </summary>
        /// <param name="MapNo"></param>
        /// <returns></returns>
        public List<EfileView> GetEfileList(int MapNo)
        {
            List<EfileView> DataList = new List<EfileView>();
            var List = DryDB.EFile.Where(p => p.MapNo == MapNo && p.Enable == true);
            foreach(var item in List)
            {
                DataList.Add(new EfileView { No = item.No, MapNo = item.MapNo, FileType = item.FileType, Filepath = item.Filepath, FileDes = item.FileDes, UpldTime = item.UpldTime.ToString("yyyy/MM/dd HH:mm") });
            }
            return DataList;
        }

        /// <summary>
        /// 取得電子檔上傳資料(電子檔案類別及一般檔案類別專用)
        /// </summary>
        /// <param name="MapNo"></param>
        /// <returns></returns>
        public List<EfileView> GetEfileListByeFile(int MapNo,bool eFileOrGentFile)
        {
            List<EfileView> NewData = new List<EfileView>();
            List<EfileView> OldData = GetEfileList(MapNo);
            //var fileTypeList = DryDB.FileTypeList.Where(p => p.FileType == 11 || p.FileType == 13 || p.FileType == 14 || p.FileType == 20 || p.FileType == 21 || p.FileType == 23).ToList();
            var fileTypeList = DryDB.FileTypeList;
            var TmpData = (from oldData in OldData
                           join FileTypelist in fileTypeList
                           on oldData.FileType equals FileTypelist.FileType
                           select new
                           {
                               oldData.No,
                               oldData.MapNo,
                               FileTypelist.FileTypeCNS,
                               oldData.FileType,
                               oldData.Filepath,
                               oldData.FileDes,
                               oldData.UpldTime
                           }
                            );
            if(eFileOrGentFile)
            {
                foreach (var item in TmpData.Where(oldData => oldData.FileType == 5 || oldData.FileType == 6 || oldData.FileType == 7 || oldData.FileType == 8))
                {
                    NewData.Add(new EfileView
                    {
                        No = item.No,
                        MapNo = item.MapNo,
                        FileType = item.FileType,
                        FileTypeCNS = item.FileTypeCNS,
                        Filepath = item.Filepath,
                        FileDes = item.FileDes,
                        UpldTime = item.UpldTime
                    });
                }
            }
            else
            {
                foreach (var item in TmpData.Where(oldData => oldData.FileType == 11 || oldData.FileType == 13 || oldData.FileType == 14 || oldData.FileType == 20 || oldData.FileType == 21 || oldData.FileType == 23 || oldData.FileType == 24))
                {
                    NewData.Add(new EfileView
                    {
                        No = item.No,
                        MapNo = item.MapNo,
                        FileType = item.FileType,
                        FileTypeCNS = item.FileTypeCNS,
                        Filepath = item.Filepath,
                        FileDes = item.FileDes,
                        UpldTime = item.UpldTime
                    });
                }
            }
            return NewData;
        }

        /// <summary>
        /// 取得檔案路徑(不包含完整目錄)
        /// </summary>
        /// <param name="No">檔案流水號</param>
        /// <returns></returns>
        public string GetFilePath(int No)
        {
            EFile eFile = DryDB.EFile.Single(p => p.No == No);
            return eFile.MapNo.ToString() + "/" + eFile.FileType.ToString() + "/" + eFile.Filepath;
        }
    }
}

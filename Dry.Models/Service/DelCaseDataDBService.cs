using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dry.Models.Service
{
    public class DelCaseDataDBService
    {
        DBCommon.DbEvent dbstatus = new DBCommon.DbEvent();
        public DBCommon.DbEvent ModifyCaseStatus(int EventNo, string rsn)
        {
            DryEntities DryDB = new DryEntities();
            Case data = DryDB.Case.Find(EventNo);
            data.Enable = false;
            data.Reason = rsn;
            data.UDate = DateTime.Now;
            try
            {
                DryDB.Case.Attach(data);
                DryDB.Entry(data).State = System.Data.EntityState.Modified;
                DryDB.SaveChanges();
                dbstatus.DbMessage = "Success";
            }
            catch (Exception e)
            {
                dbstatus.DbMessage = "Removed Case Failed : " + e.Message;
            }
            return dbstatus;
        }
    }
}

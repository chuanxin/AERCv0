using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dry.Models.Service
{
    public class DesingerChangeDBService
    {
        private DBCommon.DbEvent dbState = new DBCommon.DbEvent();
        private DryEntities DryDB = new DryEntities();
        private FacDesign Designer = new FacDesign();
        public DBCommon.DbEvent SaveDesigner(int sIANum, int eIANum, short Unit, int Year, Guid DesignId)
        {

            var datalist = (from CaseD in DryDB.CaseDetail
                            where CaseD.ApplyUnit == Unit && CaseD.ApplyYear == Year && CaseD.IANum >= sIANum && CaseD.IANum <= eIANum
                            select CaseD).ToList();



            foreach (var a in datalist)
            {
                if (a != null)
                {
                    Designer = DryDB.FacDesign.Single(x => x.MapNo == a.MapNo);
                    Designer.DesId = DesignId;
                    DryDB.FacDesign.Attach(Designer);
                    DryDB.Entry(Designer).State = System.Data.EntityState.Modified;

                }

            }


            try
            {
                DryDB.SaveChanges();
                dbState.DbMessage = "Success";
            }
            catch (Exception e)
            {
                dbState.DbMessage = "Modified Farmer Failed : " + e.Message;
            }
            return dbState;
        }
    }
}

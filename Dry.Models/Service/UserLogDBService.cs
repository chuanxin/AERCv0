using Dry.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dry.Models.Service
{
    public class UserLogDBService
    {
        DryEntities Dry = new DryEntities();

        public string SaveData(Guid id, string operation, string ip)
        {
            UserLog data = new UserLog();
            data.UserId = id;
            data.OperationLog = operation;
            data.OperationTime = DateTime.Now;
            data.Ip = ip;

            Dry.UserLog.Add(data);
            try
            {
                Dry.SaveChanges();
                return "Success";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return "Fail";
                throw;
            }
            
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dry.Models.ViewModel;
//此程式暫時沒用
namespace Dry.Models.MaterialModules
{
    /// <summary>
    /// 主管組
    /// </summary>
    public class MainPipeline
    {
        /// <summary>
        /// 取得主管材料
        /// PS. 尚未設定單價
        /// </summary>
        /// <param name="FacSysMat"></param>
        /// <param name="Data"></param>
        /// <param name="L1orL2">1:L1; 2:L2</param>
        /// <returns></returns>
        public StdSysMat GetMainPipeline(List<FacSysMAT> FacSysMat, FarmerSysView.StdMaterialStruct Data,int L1orL2)
        {
            DryEntities DryDB = new DryEntities();
            
            int BranchAmonut = (int)Math.Ceiling(Data.width / Data.L1Len);
            List<MAT_Spec> spec = DryDB.MAT_Spec.ToList();
            List<MAT_Types> mattype = DryDB.MAT_Types.ToList();
            var matlist = (from facSysMat in FacSysMat
                           where facSysMat.ModuleNo == 1 && facSysMat.SpecNo1 == Data.L1Spec && facSysMat.MatType == Data.L1Material
                           select new
                           {
                               facSysMat.POMNo,
                               facSysMat.ModuleNo,
                               facSysMat.Bunit,
                               facSysMat.MName,
                               facSysMat.MatType,
                               facSysMat.SpecNo1,
                               facSysMat.SpecNo2,
                               facSysMat.SpecNo3,
                               facSysMat.ItemUnit,
                               facSysMat.Note
                           }).ToList();
            StdSysMat itemData = new StdSysMat();
            if (matlist.Count > 0 && Data.L1MatAmt > 0)
            {
                itemData.POMNo = matlist.FirstOrDefault().POMNo;
                itemData.ModuleNo = matlist.FirstOrDefault().ModuleNo;
                itemData.ItemUnit = matlist.FirstOrDefault().ItemUnit;
                itemData.Bunit = matlist.FirstOrDefault().Bunit;
                itemData.MName = matlist.FirstOrDefault().MName;
                itemData.MatType = matlist.FirstOrDefault().MatType == null ? "" : mattype.Find(m => m.TypeNo == matlist.FirstOrDefault().MatType).TypeName;
                itemData.Note = matlist.FirstOrDefault().Note;
                itemData.Spec1 = matlist.FirstOrDefault().SpecNo1 == null ? "" : spec.Find(m => m.SpecNo == matlist.FirstOrDefault().SpecNo1.Value).SpecName;
                itemData.Spec2 = matlist.FirstOrDefault().SpecNo2 == null ? "" : spec.Find(m=>m.SpecNo ==  matlist.FirstOrDefault().SpecNo2.Value).SpecName;
                itemData.Spec3 = matlist.FirstOrDefault().SpecNo3 == null ? "" : spec.Find(m => m.SpecNo == matlist.FirstOrDefault().SpecNo3.Value).SpecName;
                itemData.Amount = BranchAmonut;
                return itemData;
            }
            return null;
        }
    }
}

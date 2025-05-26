using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dry.Models.ViewModel;

namespace Dry.Models.MaterialModules
{
    public class BranchPipeline
    {
        private DryEntities DryDB = new DryEntities();
        /// <summary>
        /// 取得支管材料
        /// PS. 支管單價由後端資料庫取得
        /// </summary>
        /// <param name="FacSysMat"></param>
        /// <param name="Data"></param>
        /// <returns></returns>
        public StdSysMat GetBranchPipeline(List<FacSysMAT> FacSysMat, FarmerSysView.StdMaterialStruct Data)
        {
            List<MAT_Spec> spec = DryDB.MAT_Spec.ToList();
            List<MAT_Types> mattype = DryDB.MAT_Types.ToList();
            var matlist = (from facSysMat in FacSysMat
                           //where facSysMat.MName.Contains("PVC B管") && facSysMat.SpecNo1 == Data.BranchSpec && facSysMat.MatType == Data.BranchMaterial
                           where facSysMat.ModuleNo == 1 && facSysMat.SpecNo1 == Data.BranchSpec && facSysMat.MatType == Data.BranchMaterial
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
                               facSysMat.Note,
                               facSysMat.SpecLength
                           }).ToList();
            StdSysMat itemData = new StdSysMat();
            if (matlist.Count > 0)
            {
                
                int BN = (int)(Data.Length / Data.SL);
                int BranchAmonut = (int)Math.Ceiling(BN * Data.width / (matlist.FirstOrDefault().SpecLength == null ? 4.0 : matlist.FirstOrDefault().SpecLength.Value));
                itemData.POMNo = matlist.FirstOrDefault().POMNo;
                itemData.ModuleNo = matlist.FirstOrDefault().ModuleNo;
                itemData.ItemUnit = matlist.FirstOrDefault().ItemUnit;
                itemData.Bunit = matlist.FirstOrDefault().Bunit;
                itemData.MName = matlist.FirstOrDefault().MName;
                itemData.Note = matlist.FirstOrDefault().Note;
                itemData.MatType = matlist.FirstOrDefault().MatType == null ? "" : mattype.Find(m => m.TypeNo == matlist.FirstOrDefault().MatType).TypeName;
                itemData.SpecNo1 = matlist.FirstOrDefault().SpecNo1;
                itemData.SpecNo2 = matlist.FirstOrDefault().SpecNo2;
                itemData.SpecNo3 = matlist.FirstOrDefault().SpecNo3;
                itemData.Spec1 = matlist.FirstOrDefault().SpecNo1 == null ? "" : spec.Find(m=>m.SpecNo ==  matlist.FirstOrDefault().SpecNo1.Value).SpecName;
                itemData.Spec2 = matlist.FirstOrDefault().SpecNo2 == null ? "" : spec.Find(m=>m.SpecNo ==  matlist.FirstOrDefault().SpecNo2.Value).SpecName;
                itemData.Spec3 = matlist.FirstOrDefault().SpecNo3 == null ? "" : spec.Find(m => m.SpecNo == matlist.FirstOrDefault().SpecNo3.Value).SpecName;
                itemData.Amount = BranchAmonut;
                return itemData;
            }
            return null;
        }

        /// <summary>
        /// 取得變徑支管材料
        /// PS. 支管單價由後端資料庫取得
        /// </summary>
        /// <param name="FacSysMat"></param>
        /// <param name="Data"></param>
        /// <returns></returns>
        public StdSysMat GetBranchPipeline(List<FacSysMAT> FacSysMat, FarmerSysView.StdMaterialStruct Data, double Cha_length, int Facspec)
        {
            List<MAT_Spec> spec = DryDB.MAT_Spec.ToList();
            List<MAT_Types> mattype = DryDB.MAT_Types.ToList();
            var matlist = (from facSysMat in FacSysMat
                           //where facSysMat.MName.Contains("PVC B管") && facSysMat.SpecNo1 == Data.BranchSpec && facSysMat.MatType == Data.BranchMaterial
                           where facSysMat.ModuleNo == 1 && facSysMat.SpecNo1 == Facspec && facSysMat.MatType == Data.BranchMaterial
                           //where facSysMat.ModuleNo == 1 && facSysMat.SpecNo1 == 27 && facSysMat.MatType == 6
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
                               facSysMat.Note,
                               facSysMat.SpecLength
                           }).ToList();
            StdSysMat itemData = new StdSysMat();
            if (matlist.Count > 0)
            {
                
                int BN = (int)(Data.Length / Data.SL);
                int BranchAmonut = (int)Math.Ceiling(BN * Cha_length / (matlist.FirstOrDefault().SpecLength == null ? 4.0 : matlist.FirstOrDefault().SpecLength.Value));
                itemData.POMNo = matlist.FirstOrDefault().POMNo;
                itemData.ModuleNo = matlist.FirstOrDefault().ModuleNo;
                itemData.ItemUnit = matlist.FirstOrDefault().ItemUnit;
                itemData.Bunit = matlist.FirstOrDefault().Bunit;
                itemData.MName = matlist.FirstOrDefault().MName;
                itemData.Note = matlist.FirstOrDefault().Note;
                itemData.MatType = matlist.FirstOrDefault().MatType == null ? "" : mattype.Find(m => m.TypeNo == matlist.FirstOrDefault().MatType).TypeName;
                itemData.SpecNo1 = Facspec;
                itemData.SpecNo2 = matlist.FirstOrDefault().SpecNo2;
                itemData.SpecNo3 = matlist.FirstOrDefault().SpecNo3;
                itemData.Spec1 = Facspec == null ? "" : spec.Find(m => m.SpecNo == Facspec).SpecName;
                itemData.Spec2 = matlist.FirstOrDefault().SpecNo2 == null ? "" : spec.Find(m => m.SpecNo == matlist.FirstOrDefault().SpecNo2.Value).SpecName;
                itemData.Spec3 = matlist.FirstOrDefault().SpecNo3 == null ? "" : spec.Find(m => m.SpecNo == matlist.FirstOrDefault().SpecNo3.Value).SpecName;
                itemData.Amount = BranchAmonut;
                return itemData;
            }
            return null;
        }
    }

}

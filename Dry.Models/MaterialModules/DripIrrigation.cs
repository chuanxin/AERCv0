using Dry.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dry.Models.MaterialModules
{
    public class DripIrrigation
    {
        DryEntities DryDB = new DryEntities();
        /// <summary>
        /// 滴嘴滴灌-滴水管
        /// </summary>
        /// <param name="FacSysMat"></param>
        /// <param name="Data"></param>
        /// <returns></returns>
        public StdSysMat GetDripIrrigation(List<FacSysMAT> FacSysMat, FarmerSysView.StdMaterialStruct Data)
        {
            var matlist = (from facSysMat in FacSysMat
                           where facSysMat.ModuleNo == 1 && facSysMat.SpecNo1 == Data.BranchSpec && facSysMat.MatType == Data.BranchMaterial
                           //where facSysMat.POMNo == MatPOMNO
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
            List<MAT_Spec> spec = DryDB.MAT_Spec.ToList();
            List<MAT_Types> mattype = DryDB.MAT_Types.ToList();
            StdSysMat itemData = new StdSysMat();
            if (matlist.Count > 0)
            {
                itemData.POMNo = matlist.FirstOrDefault().POMNo;
                itemData.ModuleNo = matlist.FirstOrDefault().ModuleNo;
                itemData.ItemUnit = matlist.FirstOrDefault().ItemUnit;
                itemData.Bunit = matlist.FirstOrDefault().Bunit;
                itemData.MName = matlist.FirstOrDefault().MName;
                itemData.MatType = matlist.FirstOrDefault().MatType == null ? "" : mattype.Find(m => m.TypeNo == matlist.FirstOrDefault().MatType).TypeName;
                itemData.Note = matlist.FirstOrDefault().Note;
                itemData.Spec1 = matlist.FirstOrDefault().SpecNo1 == null ? "" : spec.Find(m => m.SpecNo == matlist.FirstOrDefault().SpecNo1.Value).SpecName;
                itemData.Spec2 = matlist.FirstOrDefault().SpecNo2 == null ? "" : spec.Find(m => m.SpecNo == matlist.FirstOrDefault().SpecNo2.Value).SpecName;
                itemData.Spec3 = matlist.FirstOrDefault().SpecNo3 == null ? "" : spec.Find(m => m.SpecNo == matlist.FirstOrDefault().SpecNo3.Value).SpecName;
                itemData.SpecLength = matlist.FirstOrDefault().SpecLength;
                return itemData;
            }
            return null;
        }
        /// <summary>
        /// 滴水管滴灌-滴水帶
        /// </summary>
        /// <param name="FacSysMat"></param>
        /// <param name="Data"></param>
        /// <returns></returns>
        public StdSysMat GetDripPipeIrrigation(List<FacSysMAT> FacSysMat, int MatPOMNO)
        {
            var matlist = (from facSysMat in FacSysMat
                           //where facSysMat.MName.Contains("滴水帶")
                           where facSysMat.POMNo == MatPOMNO
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
            List<MAT_Spec> spec = DryDB.MAT_Spec.ToList();
            List<MAT_Types> mattype = DryDB.MAT_Types.ToList();
            StdSysMat itemData = new StdSysMat();
            if (matlist.Count > 0)
            {
                itemData.POMNo = matlist.FirstOrDefault().POMNo;
                itemData.ModuleNo = matlist.FirstOrDefault().ModuleNo;
                itemData.ItemUnit = matlist.FirstOrDefault().ItemUnit;
                itemData.Bunit = matlist.FirstOrDefault().Bunit;
                itemData.MName = matlist.FirstOrDefault().MName;
                itemData.MatType = matlist.FirstOrDefault().MatType == null ? "" : mattype.Find(m => m.TypeNo == matlist.FirstOrDefault().MatType).TypeName;
                itemData.Note = matlist.FirstOrDefault().Note;
                itemData.Spec1 = matlist.FirstOrDefault().SpecNo1 == null ? "" : spec.Find(m => m.SpecNo == matlist.FirstOrDefault().SpecNo1.Value).SpecName;
                itemData.Spec2 = matlist.FirstOrDefault().SpecNo2 == null ? "" : spec.Find(m => m.SpecNo == matlist.FirstOrDefault().SpecNo2.Value).SpecName;
                itemData.Spec3 = matlist.FirstOrDefault().SpecNo3 == null ? "" : spec.Find(m => m.SpecNo == matlist.FirstOrDefault().SpecNo3.Value).SpecName;
                itemData.SpecLength = matlist.FirstOrDefault().SpecLength;
                return itemData;
            }
            return null;
        }
    }
}

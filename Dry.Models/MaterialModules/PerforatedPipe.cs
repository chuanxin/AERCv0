using Dry.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dry.Models.MaterialModules
{
    public class PerforatedPipe
    {
        /// <summary>
        /// 穿孔管(old)
        /// </summary>
        /// <param name="FacSysMat"></param>
        /// <param name="Data"></param>
        /// <returns></returns>
        public StdSysMat GetPerforatedPipe(List<FacSysMAT> FacSysMat, FarmerSysView.StdMaterialStruct Data)
        {
            DryEntities DryDB = new DryEntities();
            var matlist = (from facSysMat in FacSysMat
                           where facSysMat.ModuleNo == 6 && facSysMat.SpecNo1 == Data.BranchSpec && facSysMat.MatType == Data.BranchMaterial
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
            List<MAT_Spec> spec = DryDB.MAT_Spec.ToList();
            List<MAT_Types> mattype = DryDB.MAT_Types.ToList();
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
                itemData.SpecNo1 = matlist.FirstOrDefault().SpecNo1;
                itemData.SpecNo2 = matlist.FirstOrDefault().SpecNo2;
                itemData.SpecNo3 = matlist.FirstOrDefault().SpecNo3;
                itemData.Spec1 = matlist.FirstOrDefault().SpecNo1 == null ? "" : spec.Find(m => m.SpecNo == matlist.FirstOrDefault().SpecNo1.Value).SpecName;
                itemData.Spec2 = matlist.FirstOrDefault().SpecNo2 == null ? "" : spec.Find(m => m.SpecNo == matlist.FirstOrDefault().SpecNo2.Value).SpecName;
                itemData.Spec3 = matlist.FirstOrDefault().SpecNo3 == null ? "" : spec.Find(m => m.SpecNo == matlist.FirstOrDefault().SpecNo3.Value).SpecName;
                return itemData;
            }
            return null;
        }
        /// <summary>
        /// 穿孔管(new)
        /// </summary>
        /// <param name="FacSysMat"></param>
        /// <param name="PerforatedPOMNO"></param>
        /// <returns></returns>
        public StdSysMat GetPerforatedPipe(List<FacSysMAT> FacSysMat, int PerforatedPOMNO)
        {
            DryEntities DryDB = new DryEntities();
            var matlist = (from facSysMat in FacSysMat
                           where facSysMat.POMNo == PerforatedPOMNO
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
                itemData.SpecNo1 = matlist.FirstOrDefault().SpecNo1;
                itemData.SpecNo2 = matlist.FirstOrDefault().SpecNo2;
                itemData.SpecNo3 = matlist.FirstOrDefault().SpecNo3;
                itemData.Spec1 = matlist.FirstOrDefault().SpecNo1 == null ? "" : spec.Find(m => m.SpecNo == matlist.FirstOrDefault().SpecNo1.Value).SpecName;
                itemData.Spec2 = matlist.FirstOrDefault().SpecNo2 == null ? "" : spec.Find(m => m.SpecNo == matlist.FirstOrDefault().SpecNo2.Value).SpecName;
                itemData.Spec3 = matlist.FirstOrDefault().SpecNo3 == null ? "" : spec.Find(m => m.SpecNo == matlist.FirstOrDefault().SpecNo3.Value).SpecName;
                //double speclength = 0;
                //if (matlist.FirstOrDefault().Spec != null)
                //{
                //    double.TryParse(matlist.FirstOrDefault().Spec, out speclength);
                //}
                //if (speclength > 0)
                //{
                //    itemData.SpecLength = speclength;
                //}
                itemData.SpecLength = matlist.FirstOrDefault().SpecLength;
                return itemData;
            }
            return null;
        }
    }
}

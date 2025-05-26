using Dry.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dry.Models.MaterialModules
{
    public class Standpipeline
    {
        /// <summary>
        /// 取得豎管材料
        /// </summary>
        /// <param name="FacSysMat"></param>
        /// <param name="Data"></param>
        /// <param name="BranchPipeAmount">支管數量</param>
        /// <returns></returns>
        public StdSysMat GetStandPipeline(List<FacSysMAT> FacSysMat, FarmerSysView.StdMaterialStruct Data)
        {
            DryEntities DryDB = new DryEntities();
            var matlist = (from facSysMat in FacSysMat
                           where (facSysMat.ModuleNo == 4/*1*/) && /*facSysMat.SpecLength >= Data.StdpipeHei && */facSysMat.MatType == Data.StdpipeMat && facSysMat.SpecNo1 == Data.StdpipeSpec
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
                           }).OrderBy(m => m.SpecLength).ToList();
            List<MAT_Spec> spec = DryDB.MAT_Spec.ToList();
            List<MAT_Types> mattype = DryDB.MAT_Types.ToList();
            StdSysMat itemData = new StdSysMat();
            if (matlist.Count > 0)
            {
                
                int StandAmonut = (int)Math.Ceiling((GetStandPipeAmount(Data) * Data.StdpipeHei / ((matlist.FirstOrDefault().SpecLength == null || matlist.FirstOrDefault().SpecLength == 0 ? 4 : matlist.FirstOrDefault().SpecLength.Value))));
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
                itemData.Amount = StandAmonut;
                return itemData;
            }
            return null;
        }
        /// <summary>
        /// 取得豎管數量
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        public float GetStandPipeAmount(FarmerSysView.StdMaterialStruct Data)
        {
            int BN = (int)(Data.Length / Data.SL);
            int SN = (int)(Data.width / Data.SS);
            return BN * SN;
        }
    }
}

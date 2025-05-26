using Dry.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dry.Models.MaterialModules
{
    public class HardwareMaterial
    {
        DryEntities DryDB = new DryEntities();
        /// <summary>
        /// 取得彎頭
        /// </summary>
        /// <param name="FacSysMat"></param>
        /// <param name="Data"></param>
        /// <param name="L1orL2">1: L1, 2: L2</param>
        /// <returns></returns>
        public StdSysMat GetBend(List<FacSysMAT> FacSysMat, FarmerSysView.StdMaterialStruct Data,int L1orL2)
        {
            List<MAT_Spec> spec = DryDB.MAT_Spec.ToList();
            List<MAT_Types> mattype = DryDB.MAT_Types.ToList();
            if(L1orL2==1)
            {
                var matlist = (from facSysMat in FacSysMat
                               where /*facSysMat.ModuleNo == 2 &&*/ facSysMat.MName.Contains("彎頭") && facSysMat.SpecNo1 == Data.L1Spec
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
                if (matlist.Count > 0 /*&& Data.L1MatAmt > 0*/)
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
                    itemData.Amount = 3;
                    return itemData;
                }
            }
            else
            {
                var matlist = (from facSysMat in FacSysMat
                               where facSysMat.ModuleNo == 2 && facSysMat.MName.Contains("彎頭") && facSysMat.SpecNo1 == Data.L2Spec
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
                if (matlist.Count > 0/* && Data.L1MatAmt > 0*/)
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
                    itemData.Amount = 3;
                    return itemData;
                }
            }
            
            return null;
        }
        /// <summary>
        /// 取得塞口
        /// </summary>
        /// <param name="FacSysMat"></param>
        /// <param name="Data"></param>
        /// <param name="WaterPipeAmount">水管數量</param>
        /// <returns></returns>
        public StdSysMat GetReceptacle(List<FacSysMAT> FacSysMat, FarmerSysView.StdMaterialStruct Data, int WaterPipeAmount,int spec)
        {
            var matlist = (from facSysMat in FacSysMat
                           where facSysMat.ModuleNo == 2 && facSysMat.MName.Contains("塞口") && facSysMat.SpecNo1 == spec
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
            List<MAT_Spec> specs = DryDB.MAT_Spec.ToList();
            List<MAT_Types> mattype = DryDB.MAT_Types.ToList();
            StdSysMat itemData = new StdSysMat();
            if (matlist.Count > 0 /*&& Data.L1MatAmt > 0*/)
            {
                itemData.POMNo = matlist.FirstOrDefault().POMNo;
                itemData.ModuleNo = matlist.FirstOrDefault().ModuleNo;
                itemData.ItemUnit = matlist.FirstOrDefault().ItemUnit;
                itemData.Bunit = matlist.FirstOrDefault().Bunit;
                itemData.MName = matlist.FirstOrDefault().MName;
                itemData.MatType = matlist.FirstOrDefault().MatType == null ? "" : mattype.Find(m => m.TypeNo == matlist.FirstOrDefault().MatType).TypeName;
                itemData.Note = matlist.FirstOrDefault().Note;
                itemData.Spec1 = matlist.FirstOrDefault().SpecNo1 == null ? "" : specs.Find(m => m.SpecNo == matlist.FirstOrDefault().SpecNo1.Value).SpecName;
                itemData.Spec2 = matlist.FirstOrDefault().SpecNo2 == null ? "" : specs.Find(m => m.SpecNo == matlist.FirstOrDefault().SpecNo2.Value).SpecName;
                itemData.Spec3 = matlist.FirstOrDefault().SpecNo3 == null ? "" : specs.Find(m => m.SpecNo == matlist.FirstOrDefault().SpecNo3.Value).SpecName;
                itemData.Amount = WaterPipeAmount;
                return itemData;
            }
            return null;
        }
        /// <summary>
        /// 取得三通管
        /// </summary>
        /// <param name="FacSysMat"></param>
        /// <param name="Data"></param>
        /// <param name="Amount">數量</param>
        /// <returns></returns>Reducing_joint
        public StdSysMat GetTeePipe(List<FacSysMAT> FacSysMat, FarmerSysView.StdMaterialStruct Data, int Amount, int MainPipeSpec,int BranchSpec)
        {
            var matlist = (from facSysMat in FacSysMat
                           where facSysMat.MName.Contains("三通") && facSysMat.SpecNo1 == MainPipeSpec && facSysMat.SpecNo2 == BranchSpec
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
            if (matlist.Count > 0 /*&& Data.L1MatAmt > 0*/)
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
                itemData.Amount = Amount;
                return itemData;
            }
            return null;
        }

        /// <summary>
        /// 取得異徑接頭
        /// </summary>
        /// <param name="FacSysMat"></param>
        /// <param name="Data"></param>
        /// <param name="Amount">數量</param>
        /// <returns></returns>Reducing_joint
        public StdSysMat GetReducingJoint(List<FacSysMAT> FacSysMat, FarmerSysView.StdMaterialStruct Data, int Amount, int MainPipeSpec, int BranchSpec)
        {
            var matlist = (from facSysMat in FacSysMat
                           where facSysMat.MName.Contains("異徑接頭") && facSysMat.SpecNo1 == MainPipeSpec && facSysMat.SpecNo2 == BranchSpec
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
            if (matlist.Count > 0 /*&& Data.L1MatAmt > 0*/)
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
                itemData.Amount = Amount;
                return itemData;
            }
            return null;
        }




        //四通
        public StdSysMat GetReducingCross(List<FacSysMAT> FacSysMat, FarmerSysView.StdMaterialStruct Data, int Amount, int MainPipeSpec,int BranchSpec)
        {
            var matlist = (from facSysMat in FacSysMat
                           where facSysMat.MName.Contains("四通") && facSysMat.SpecNo1 == MainPipeSpec && facSysMat.SpecNo2 == BranchSpec
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
            if (matlist.Count > 0 /*&& Data.L1MatAmt > 0*/)
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
                itemData.Amount = Amount;
                return itemData;
            }
            return null;
        }

        /// <summary>
        /// 取得制水閥
        /// </summary>
        /// <param name="FacSysMat"></param>
        /// <param name="Data"></param>
        /// <param name="WaterPipeAmount">水管數量</param>
        /// <returns></returns>
        public StdSysMat GetValves(List<FacSysMAT> FacSysMat, FarmerSysView.StdMaterialStruct Data, int WaterPipeAmount, int BranchSpec)
        {
            var matlist = (from facSysMat in FacSysMat
                           where facSysMat.ModuleNo == 10 && facSysMat.SpecNo1 == BranchSpec
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
            if (matlist.Count > 0/* && Data.L1MatAmt > 0*/)
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
                itemData.Amount = WaterPipeAmount;
                return itemData;
            }
            return null;
        }
        /// <summary>
        /// 取得直龍頭
        /// </summary>
        /// <param name="FacSysMat"></param>
        /// <param name="Data"></param>
        /// <param name="WaterPipeAmount">水管數量</param>
        /// <returns></returns>
        public StdSysMat GetStraightFaucet(List<FacSysMAT> FacSysMat, FarmerSysView.StdMaterialStruct Data, int WaterPipeAmount,int spec1, int spec2)
        {
            if (spec1 == spec2)
            {
                var matlist = (from facSysMat in FacSysMat
                               where facSysMat.MName.Contains("直龍") && facSysMat.SpecNo1 == spec1
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
                if (matlist.Count > 0 /*&& Data.L1MatAmt > 0*/)
                {
                    itemData.POMNo = matlist.FirstOrDefault().POMNo;
                    itemData.ModuleNo = matlist.FirstOrDefault().ModuleNo;
                    itemData.ItemUnit = matlist.FirstOrDefault().ItemUnit;
                    itemData.Bunit = matlist.FirstOrDefault().Bunit;
                    itemData.MName = matlist.FirstOrDefault().MName;
                    itemData.MatType = mattype.Find(m => m.TypeNo == matlist.FirstOrDefault().MatType).TypeName;
                    itemData.Note = matlist.FirstOrDefault().Note;
                    itemData.Spec1 = matlist.FirstOrDefault().SpecNo1 == null ? "" : spec.Find(m => m.SpecNo == matlist.FirstOrDefault().SpecNo1.Value).SpecName;
                    itemData.Spec2 = matlist.FirstOrDefault().SpecNo2 == null ? "" : spec.Find(m => m.SpecNo == matlist.FirstOrDefault().SpecNo2.Value).SpecName;
                    itemData.Spec3 = matlist.FirstOrDefault().SpecNo3 == null ? "" : spec.Find(m => m.SpecNo == matlist.FirstOrDefault().SpecNo3.Value).SpecName;
                    itemData.Amount = WaterPipeAmount;
                    return itemData;
                }
            }
            else
            {

                var matlist = (from facSysMat in FacSysMat
                               where facSysMat.MName.Contains("直龍") && facSysMat.SpecNo1 == spec1 && facSysMat.SpecNo2 == spec2
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
                if (matlist.Count > 0/* && Data.L1MatAmt > 0*/)
                {
                    itemData.POMNo = matlist.FirstOrDefault().POMNo;
                    itemData.ModuleNo = matlist.FirstOrDefault().ModuleNo;
                    itemData.ItemUnit = matlist.FirstOrDefault().ItemUnit;
                    itemData.Bunit = matlist.FirstOrDefault().Bunit;
                    itemData.MName = matlist.FirstOrDefault().MName;
                    itemData.MatType = mattype.Find(m => m.TypeNo == matlist.FirstOrDefault().MatType).TypeName;
                    itemData.Note = matlist.FirstOrDefault().Note;
                    itemData.Spec1 = matlist.FirstOrDefault().SpecNo1 == null ? "" : spec.Find(m => m.SpecNo == matlist.FirstOrDefault().SpecNo1.Value).SpecName;
                    itemData.Spec2 = matlist.FirstOrDefault().SpecNo2 == null ? "" : spec.Find(m => m.SpecNo == matlist.FirstOrDefault().SpecNo2.Value).SpecName;
                    itemData.Spec3 = matlist.FirstOrDefault().SpecNo3 == null ? "" : spec.Find(m => m.SpecNo == matlist.FirstOrDefault().SpecNo3.Value).SpecName;
                    itemData.Amount = WaterPipeAmount;
                    return itemData;
                }
            }
            
            return null;
        }
        /// <summary>
        /// 取得噴頭(old)
        /// </summary>
        /// <param name="FacSysMat"></param>
        /// <param name="Data"></param>
        /// <param name="Amount">數量</param>
        /// <param name="StandpipeSpec">規格</param>
        /// <returns></returns>
        public StdSysMat GetSprinklerHead(List<FacSysMAT> FacSysMat, FarmerSysView.StdMaterialStruct Data, int Amount, int Spec)
        {
            var matlist = (from facSysMat in FacSysMat
                           where facSysMat.ModuleNo == 5 && facSysMat.SpecNo1 == Spec
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
            if (matlist.Count > 0/* && Data.L1MatAmt > 0*/)
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
                itemData.Amount = Amount;
                return itemData;
            }
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="FacSysMat"></param>
        /// <param name="Data"></param>
        /// <param name="Amount"></param>
        /// <param name="Spec"></param>
        /// <returns></returns>
        public StdSysMat GetSprinklerHead(List<FacSysMAT> FacSysMat, int Amount, int SprinklerPOMNO)
        {
            var matlist = (from facSysMat in FacSysMat
                           where facSysMat.POMNo == SprinklerPOMNO
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
                itemData.Amount = Amount;
                return itemData;
            }
            return null;
        }
        /// <summary>
        /// 取得微噴頭
        /// </summary>
        /// <param name="FacSysMat"></param>
        /// <param name="Data"></param>
        /// <param name="Amount">數量</param>
        /// <param name="StandpipeSpec">規格</param>
        /// <returns></returns>
        public StdSysMat GetMicroSprinklerHead(List<FacSysMAT> FacSysMat, FarmerSysView.StdMaterialStruct Data, int Amount,/* int Spec, */int SprinklerPOMNO)
        {
            var matlist = (from facSysMat in FacSysMat
                           //where facSysMat.ModuleNo == 8 && facSysMat.SpecNo1 == Spec
                           where facSysMat.POMNo == SprinklerPOMNO
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
            if (matlist.Count > 0 /*&& Data.L1MatAmt > 0*/)
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
                itemData.Amount = Amount;
                return itemData;
            }
            return null;
        }
        /// <summary>
        /// 取得閥接頭
        /// </summary>
        /// <param name="FacSysMat"></param>
        /// <param name="Data"></param>
        /// <param name="Amount"></param>
        /// <param name="Spec"></param>
        /// <returns></returns>
        public StdSysMat GetValveAapter(List<FacSysMAT> FacSysMat, FarmerSysView.StdMaterialStruct Data, int Amount, int Spec)
        {
            var matlist = (from facSysMat in FacSysMat
                           where facSysMat.MName.Contains("閥接頭") && facSysMat.SpecNo1 == Spec
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
            if (matlist.Count > 0 /*&& Data.L1MatAmt > 0*/)
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
                itemData.Amount = Amount;
                return itemData;
            }
            return null;
        }

        /// <summary>
        /// 穿孔管接頭
        /// </summary>
        /// <param name="FacSysMat"></param>
        /// <param name="Data"></param>
        /// <param name="Amount"></param>
        /// <param name="Spec"></param>
        /// <returns></returns>
        public StdSysMat GetPerforatedFittings(List<FacSysMAT> FacSysMat, FarmerSysView.StdMaterialStruct Data, int Amount, int Spec)
        {
            var matlist = (from facSysMat in FacSysMat
                           where facSysMat.MName.Contains("穿孔管接頭") && facSysMat.SpecNo1 == Spec
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
            if (matlist.Count > 0 /*&& Data.L1MatAmt > 0*/)
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
                itemData.Amount = Amount;
                return itemData;
            }
            return null;
        }

        /// <summary>
        /// 固定設施-鍍鋅鋼管
        /// </summary>
        /// <param name="FacSysMat"></param>
        /// <param name="Data"></param>
        /// <param name="Amount"></param>
        /// <param name="Spec"></param>
        /// <returns></returns>
        public StdSysMat GetFixedFacilities(List<FacSysMAT> FacSysMat, FarmerSysView.StdMaterialStruct Data, int Spec)
        {
            var matlist = (from facSysMat in FacSysMat
                           where facSysMat.MName.Contains("鍍鋅鋼管") && facSysMat.SpecNo1 == Spec
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
            if (matlist.Count > 0/* && Data.L1MatAmt > 0*/)
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
                return itemData;
            }
            return null;
        }
        /// <summary>
        /// 管尾束
        /// </summary>
        /// <param name="FacSysMat"></param>
        /// <param name="Data"></param>
        /// <param name="Spec"></param>
        /// <returns></returns>
        public StdSysMat GetLastFolder(List<FacSysMAT> FacSysMat, FarmerSysView.StdMaterialStruct Data, int Spec, int Amount)
        {
            var matlist = (from facSysMat in FacSysMat
                           where facSysMat.MName.Contains("管尾束") && facSysMat.SpecNo1 == Spec
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
            if (matlist.Count > 0 /*&& Data.L1MatAmt > 0*/)
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
                itemData.Amount = Amount;
                return itemData;
            }
            return null;
        }
        /// <summary>
        /// 管首接頭
        /// </summary>
        /// <param name="FacSysMat"></param>
        /// <param name="Data"></param>
        /// <param name="Spec"></param>
        /// <param name="Amount"></param>
        /// <returns></returns>
        public StdSysMat GetFirstPipeFittings(List<FacSysMAT> FacSysMat, FarmerSysView.StdMaterialStruct Data, int Spec, int Amount)
        {
            var matlist = (from facSysMat in FacSysMat
                           where facSysMat.MName.Contains("管首接頭") && facSysMat.SpecNo1 == Spec
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
            if (matlist.Count > 0 /*&& Data.L1MatAmt > 0*/)
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
                itemData.Amount = Amount;
                return itemData;
            }
            return null;
        }

        public StdSysMat GetFirstPipeFittings(List<FacSysMAT> FacSysMat, FarmerSysView.StdMaterialStruct Data, int Spec,int Spec1, int Amount)
        {
            var matlist = (from facSysMat in FacSysMat
                           where facSysMat.MName.Contains("管首接頭") && facSysMat.SpecNo1 == Spec && facSysMat.SpecNo2 == Spec1
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
            if (matlist.Count > 0 /*&& Data.L1MatAmt > 0*/)
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
                itemData.Amount = Amount;
                return itemData;
            }
            return null;
        }

        public StdSysMat GetPerforatedFolder(List<FacSysMAT> FacSysMat, FarmerSysView.StdMaterialStruct Data, int Amount)
        {
            var matlist = (from facSysMat in FacSysMat
                           where facSysMat.MName.Contains("穿孔管尾夾")// && facSysMat.SpecNo1 == Spec
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
            if (matlist.Count > 0 /*&& Data.L1MatAmt > 0*/)
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
                itemData.Amount = Amount;
                return itemData;
            }
            return null;
        }
        /// <summary>
        /// 滴嘴
        /// </summary>
        /// <param name="FacSysMat"></param>
        /// <param name="Data"></param>
        /// <param name="Spec"></param>
        /// <param name="Amount"></param>
        /// <returns></returns>
        public StdSysMat GetDrip(List<FacSysMAT> FacSysMat, FarmerSysView.StdMaterialStruct Data, int Spec, int Amount)
        {
            var matlist = (from facSysMat in FacSysMat
                           //where facSysMat.ModuleNo == 9 && facSysMat.MName.Contains("滴嘴") //&& facSysMat.SpecNo1 == Spec
                           where facSysMat.POMNo == Data.NozzleMaterial
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
            if (matlist.Count > 0/* && Data.L1MatAmt > 0*/)
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
                itemData.Amount = Amount;
                return itemData;
            }
            return null;
        }
        /// <summary>
        /// 消耗性材料
        /// </summary>
        /// <param name="FacSysMat"></param>
        /// <param name="Data"></param>
        /// <returns></returns>
        public StdSysMat GetExpendable(List<FacSysMAT> FacSysMat, FarmerSysView.StdMaterialStruct Data, int MatTotalPrice)
        {
            var matlist = (from facSysMat in FacSysMat
                           where facSysMat.ModuleNo == 3 && facSysMat.MName.Contains("消耗性")
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
            if (matlist.Count > 0 /*&& Data.L1MatAmt > 0*/)
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
                itemData.Amount = 1;

                int OldMatTotalPrice = MatTotalPrice;
                int ExpendablePrice = (int)(MatTotalPrice * 0.02);

                MatTotalPrice += ExpendablePrice;
                if (MatTotalPrice.ToString().Length == 2)
                {
                    if (Convert.ToInt32(MatTotalPrice.ToString().Last()) < ExpendablePrice)
                    {
                        ExpendablePrice = MatTotalPrice - Convert.ToInt32(MatTotalPrice.ToString().Last());
                    }
                }
                else if (MatTotalPrice.ToString().Length >= 3)
                {
                    string IntTemp = "";
                    for (int i = MatTotalPrice.ToString().Length - 2; i < MatTotalPrice.ToString().Length; i++)
                    {
                        IntTemp += MatTotalPrice.ToString()[i].ToString();
                    }
                    if (ExpendablePrice > Convert.ToInt32(IntTemp))
                    {
                        ExpendablePrice -= Convert.ToInt32(IntTemp);
                    }
                    else
                    {
                        
                        MatTotalPrice = ((int)(MatTotalPrice / 100) + 1) * 100;
                        ExpendablePrice = MatTotalPrice - OldMatTotalPrice;
                    }
                }
                itemData.Price = ExpendablePrice;
                return itemData;
            }
            return null;
        }
    }
}

using Dry.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dry.Models.Service;
using System.Web;
using Dry.Models.CommonCls;

namespace Dry.Models.MaterialModules
{
    public class MaterialModule
    {
        private DryEntities DryDB = new DryEntities();
        private List<StdSysMat> MatList = new List<StdSysMat>();
        private int MainPipeSpec = 0;
        /// <summary>
        /// 末端管路材料(新版)
        /// </summary>
        /// <returns></returns>
        public List<StdSysMat> GetStdMatList(FarmerSysView.StdMaterialStruct Data, short Unit, int MapNo)
        {
            
            
            switch (Data.ddl_EndType)
            {
                case 1: 
                    
                    //if (Data.L1MatAmt > 0) //L1
                    //{
                    L1MainPipeLine(Data, Unit, MapNo);
                    PerforatedPipe(Data, Unit, MapNo);
                    //}
                    if (Data.L2MatAmt > 0) //L2
                    {
                        L2MainPipeLine(Data, Unit, MapNo);
                        //PerforatedPipe(Data, Unit, MapNo);
                    }                    
                    break;
                case 2: 
                    L1MainPipeLine(Data, Unit, MapNo);
                   
                    if (Data.ChangeBranchSpec == 0)
                    {
                        GetNozzle(Data, Unit, MapNo);
                    }
                    else
                    {
                        
                        GetNozzleChange(Data, Unit, MapNo);
                    }
                    
                    
                    if (Data.L2MatAmt > 0) 
                    {
                        L2MainPipeLine(Data, Unit, MapNo);
                        //GetNozzle(Data, Unit, MapNo);
                    }
                    break;
                case 3:                    
                    L1MainPipeLine(Data, Unit, MapNo);
                    if (Data.ChangeBranchSpec == 0)
                    {
                        GetMicroSprinklers(Data, Unit, MapNo);
                    }
                    else
                    {

                        GetMicroSprinklersChange(Data, Unit, MapNo);
                    }                    
                    
                    if (Data.L2MatAmt > 0)
                    {
                        L2MainPipeLine(Data, Unit, MapNo);
                        
                    }           
                    break;
                case 4:
                    switch (Data.ddl_Drop)
                    {
                        case 7: 
                          
                            L1MainPipeLine(Data, Unit, MapNo);
                            DripIrrigation(Data, Unit, MapNo);
                            
                            if (Data.L2MatAmt > 0)
                            {
                                L2MainPipeLine(Data, Unit, MapNo);
                                //DripIrrigation(Data, Unit, MapNo);
                            }                            
                            break;
                        case 8:
                    
                            L1MainPipeLine(Data, Unit, MapNo);
                            DripPipeIrrigation(Data, Unit, MapNo, 1);
                            
                            if (Data.L2MatAmt > 0)
                            {
                                L2MainPipeLine(Data, Unit, MapNo);
                                //DripPipeIrrigation(Data, Unit, MapNo, 1);
                            }                           
                            break;
                    }

                    break;
                case 5: //軟管
                    break;
            }

            return MatList;
        }
        /// <summary>
        /// 噴頭灌溉
        /// </summary>
        /// <param name="Data"></param>
        /// <param name="Unit"></param>
        /// <param name="MapNo"></param>
        /// <returns></returns>
        private void GetNozzle(FarmerSysView.StdMaterialStruct Data, short Unit, int MapNo)
        {
            List<FacSysMAT> FacSysMat = DryDB.FacSysMAT.Where(m => m.Bunit == Unit).ToList();
            StdSysMat StdMat = new StdSysMat();
            HardwareMaterial hardware = new HardwareMaterial();
            
           
            StdMat = GetBranchpipe(FacSysMat, Data, Unit, MapNo);
            int BN = (int)(Data.Length / Data.SL); 
            int SN = (int)(Data.width / Data.SS);
            int BranchSpec = 0; 
            if (StdMat != null)
            {
                StdMat.Price = GetBranchPipelinePrice(StdMat.POMNo, MapNo);
                StdMat.Group = 2;
                MatList.Add(StdMat);
                BranchSpec = StdMat.SpecNo1 == null ? 0 : StdMat.SpecNo1.Value;
            }
            
            if (BN > 0)
            {
                
                StdMat = hardware.GetTeePipe(FacSysMat, Data, BN, MainPipeSpec, BranchSpec);
                if (StdMat != null)
                {
                    StdMat.Price = GetBranchPipelinePrice(StdMat.POMNo, MapNo);
                    StdMat.Group = 2;
                    MatList.Add(StdMat);
                }
                
                StdMat = hardware.GetValves(FacSysMat, Data, BN, BranchSpec);
                if (StdMat != null)
                {
                    StdMat.Price = GetBranchPipelinePrice(StdMat.POMNo, MapNo);
                    StdMat.Group = 2;
                    MatList.Add(StdMat);
                }
                
                StdMat = hardware.GetValveAapter(FacSysMat, Data, BN * 2, BranchSpec);
                if (StdMat != null)
                {
                    StdMat.Price = GetBranchPipelinePrice(StdMat.POMNo, MapNo);
                    StdMat.Group = 2;
                    MatList.Add(StdMat);
                }
                
                StdMat = hardware.GetReceptacle(FacSysMat, Data, BN, BranchSpec);
                if (StdMat != null)
                {
                    StdMat.Price = GetBranchPipelinePrice(StdMat.POMNo, MapNo);
                    StdMat.Group = 2;
                    MatList.Add(StdMat);
                }
            }
            
            
            
            
            Standpipeline standPipe = new Standpipeline();
            if (Data.StdpipeHei > 0) 
            {
                StdMat = standPipe.GetStandPipeline(FacSysMat, Data);
                int StandSpec = 0;
                if (StdMat != null)
                {
                    StdMat.Price = GetBranchPipelinePrice(StdMat.POMNo, MapNo);
                    StdMat.Group = 5;
                    MatList.Add(StdMat);
                    StandSpec = StdMat.SpecNo1 == null ? 0 : StdMat.SpecNo1.Value;
                }
                if (BN * SN > 0)
                {
                    StdMat = hardware.GetTeePipe(FacSysMat, Data, BN * SN, BranchSpec, StandSpec);
                    if (StdMat != null && BN * SN > 0)
                    {
                        StdMat.Price = GetBranchPipelinePrice(StdMat.POMNo, MapNo);
                        StdMat.Group = 5;
                        MatList.Add(StdMat);
                    }
                    StdMat = hardware.GetStraightFaucet(FacSysMat, Data, BN * SN, Data.StdpipeSpec, Data.NozzleSpec);
                    if (StdMat != null && BN * SN > 0)
                    {
                        StdMat.Price = GetBranchPipelinePrice(StdMat.POMNo, MapNo);
                        StdMat.Group = 5;
                        MatList.Add(StdMat);
                    }
                    StdMat = hardware.GetValves(FacSysMat, Data, BN * SN, StandSpec);
                    if (StdMat != null)
                    {
                        StdMat.Price = GetBranchPipelinePrice(StdMat.POMNo, MapNo);
                        StdMat.Group = 5;
                        MatList.Add(StdMat);
                    }
                    StdMat = hardware.GetValveAapter(FacSysMat, Data, BN * SN * 2, StandSpec);
                    if (StdMat != null)
                    {
                        StdMat.Price = GetBranchPipelinePrice(StdMat.POMNo, MapNo);
                        StdMat.Group = 5;
                        MatList.Add(StdMat);
                    }
                    StdMat = hardware.GetFixedFacilities(FacSysMat, Data, StandSpec);
                    if (StdMat != null)
                    {
                        StdMat.Price = GetBranchPipelinePrice(StdMat.POMNo, MapNo);
                        StdMat.Amount = (int)Math.Ceiling(BN * SN * Data.StdpipeHei / (StdMat.SpecLength == null ? 1f : StdMat.SpecLength.Value));
                        StdMat.Group = 6;
                        MatList.Add(StdMat);
                    }
                }

                
                
            }
            else
            {
                if (BN * SN > 0)
                {
                    StdMat = hardware.GetTeePipe(FacSysMat, Data, BN * SN, BranchSpec, Data.StdpipeSpec);
                    if (StdMat != null)
                    {
                        StdMat.Price = GetBranchPipelinePrice(StdMat.POMNo, MapNo);
                        StdMat.Group = 5;
                        MatList.Add(StdMat);
                    }
                }
                
            }

            if (BN * SN > 0)
            {
                StdMat = hardware.GetSprinklerHead(FacSysMat, BN * SN, Data.NozzleMaterial);
                if (StdMat != null)
                {
                    StdMat.Price = GetBranchPipelinePrice(StdMat.POMNo, MapNo);
                    StdMat.Group = 8;
                    MatList.Add(StdMat);
                }
            }           
            
            
        }

        private void GetNozzleChange(FarmerSysView.StdMaterialStruct Data, short Unit, int MapNo)
        {
            
            if (Data.SL <= 0 && Data.SS <= 0)
            {
                return;
            }
            
            List<FacSysMAT> FacSysMat = DryDB.FacSysMAT.Where(m => m.Bunit == Unit).ToList();
            StdSysMat StdMat = new StdSysMat();
            HardwareMaterial hardware = new HardwareMaterial();
            int BN = (int)(Data.Length / Data.SL);
            int SN = (int)(Data.width / Data.SS);
            double beforeBranchChangeLegth = Math.Truncate(2.0 * Data.width / 3.0);
            double afterBranchChangeLegth = Data.width - beforeBranchChangeLegth;
            int BeforeCSN = (int)Math.Truncate(beforeBranchChangeLegth / Data.SS);
            int afterCSN = SN - BeforeCSN;
            int BranchSpec = 0;
            int beforeteepipe = 2 * BN * SN / 3;
            int afterteepipe = (BN * SN) - beforeteepipe;
            StdMat = GetBranchpipeChange(FacSysMat, Data, Unit, MapNo, beforeBranchChangeLegth, Data.BranchSpec);
            if (StdMat != null)
            {
                StdMat.Price = GetBranchPipelinePrice(StdMat.POMNo, MapNo);
                StdMat.Group = 2;
                MatList.Add(StdMat);

                BranchSpec = StdMat.SpecNo1 == null ? 0 : StdMat.SpecNo1.Value;
            }

            StdMat = GetBranchpipeChange(FacSysMat, Data, Unit, MapNo, afterBranchChangeLegth, Data.ChangeBranchSpec);
            if (StdMat != null)
            {
                StdMat.Price = GetBranchPipelinePrice(StdMat.POMNo, MapNo);
                StdMat.Group = 2;
                MatList.Add(StdMat);
            }
            StdMat = hardware.GetReducingJoint(FacSysMat, Data, BN, Data.BranchSpec, Data.ChangeBranchSpec);
            if (StdMat != null)
            {
                StdMat.Price = GetBranchPipelinePrice(StdMat.POMNo, MapNo);
                StdMat.Group = 2;
                MatList.Add(StdMat);
            }
            StdMat = hardware.GetTeePipe(FacSysMat, Data, BN, MainPipeSpec, Data.BranchSpec);
            if (StdMat != null)
            {
                StdMat.Price = GetBranchPipelinePrice(StdMat.POMNo, MapNo);
                StdMat.Group = 2;
                MatList.Add(StdMat);
            }
            StdMat = hardware.GetValves(FacSysMat, Data, BN, BranchSpec);
            if (StdMat != null)
            {
                StdMat.Price = GetBranchPipelinePrice(StdMat.POMNo, MapNo);
                StdMat.Group = 2;
                MatList.Add(StdMat);
            }            
            StdMat = hardware.GetValveAapter(FacSysMat, Data, BN * 2, BranchSpec);
            if (StdMat != null)
            {
                StdMat.Price = GetBranchPipelinePrice(StdMat.POMNo, MapNo);
                StdMat.Group = 2;
                MatList.Add(StdMat);
            }
            StdMat = hardware.GetReceptacle(FacSysMat, Data, BN, Data.ChangeBranchSpec);
            if (StdMat != null)
            {
                StdMat.Price = GetBranchPipelinePrice(StdMat.POMNo, MapNo);
                StdMat.Group = 2;
                MatList.Add(StdMat);
            }

            Standpipeline standPipe = new Standpipeline();
            if (Data.StdpipeHei > 0)
            {
                StdMat = standPipe.GetStandPipeline(FacSysMat, Data);
                int StandSpec = 0;
                if (StdMat != null)
                {
                    StdMat.Price = GetBranchPipelinePrice(StdMat.POMNo, MapNo);
                    StdMat.Group = 5;
                    MatList.Add(StdMat);
                    StandSpec = StdMat.SpecNo1 == null ? 0 : StdMat.SpecNo1.Value;
                }
                StdMat = hardware.GetTeePipe(FacSysMat, Data, beforeteepipe, Data.BranchSpec, Data.NozzleSpec);
                if (StdMat != null)
                {
                    StdMat.Price = GetBranchPipelinePrice(StdMat.POMNo, MapNo);
                    StdMat.Group = 5;
                    MatList.Add(StdMat);
                }

                StdMat = hardware.GetTeePipe(FacSysMat, Data, afterteepipe, Data.ChangeBranchSpec, Data.NozzleSpec);
                if (StdMat != null)
                {
                    StdMat.Price = GetBranchPipelinePrice(StdMat.POMNo, MapNo);
                    StdMat.Group = 5;
                    MatList.Add(StdMat);
                }

                StdMat = hardware.GetStraightFaucet(FacSysMat, Data, BN * SN, Data.StdpipeSpec, Data.NozzleSpec);
                if (StdMat != null)
                {
                    StdMat.Price = GetBranchPipelinePrice(StdMat.POMNo, MapNo);
                    StdMat.Group = 5;
                    MatList.Add(StdMat);
                }
                StdMat = hardware.GetValves(FacSysMat, Data, BN * SN, StandSpec);
                if (StdMat != null)
                {
                    StdMat.Price = GetBranchPipelinePrice(StdMat.POMNo, MapNo);
                    StdMat.Group = 5;
                    MatList.Add(StdMat);
                }
                StdMat = hardware.GetValveAapter(FacSysMat, Data, BN * SN * 2, StandSpec);
                if (StdMat != null)
                {
                    StdMat.Price = GetBranchPipelinePrice(StdMat.POMNo, MapNo);
                    StdMat.Group = 5;
                    MatList.Add(StdMat);
                }
                StdMat = hardware.GetFixedFacilities(FacSysMat, Data, StandSpec);
                if (StdMat != null)
                {
                    StdMat.Price = GetBranchPipelinePrice(StdMat.POMNo, MapNo);
                    StdMat.Amount = (int)Math.Ceiling(BN * SN * Data.StdpipeHei / (StdMat.SpecLength == null ? 1f : StdMat.SpecLength.Value));
                    StdMat.Group = 6;
                    MatList.Add(StdMat);
                }
            }
            else 
            {
                StdMat = hardware.GetTeePipe(FacSysMat, Data, BN * SN, BranchSpec, Data.StdpipeSpec);
                if (StdMat != null)
                {
                    StdMat.Price = GetBranchPipelinePrice(StdMat.POMNo, MapNo);
                    StdMat.Group = 5;
                    MatList.Add(StdMat);
                }
            }

            StdMat = hardware.GetSprinklerHead(FacSysMat, BN * SN, Data.NozzleMaterial);
            if (StdMat != null)
            {
                StdMat.Price = GetBranchPipelinePrice(StdMat.POMNo, MapNo);
                StdMat.Group = 8;
                MatList.Add(StdMat);
            }

            
            //int MatTotalPrice = (int)(MatList.Sum(m => m.Price * m.Amount));
            //StdMat = hardware.GetExpendable(FacSysMat, Data, MatTotalPrice);
            //if (StdMat != null)
            //{
            //    StdMat.Group = 7;
            //    MatList.Add(StdMat);
            //}

            //return MatList;
        }

        
        /// <summary>
        /// 微噴灌溉變徑
        /// </summary>
        /// <param name="Data"></param>
        /// <param name="Unit"></param>
        /// <param name="MapNo"></param>
        private void GetMicroSprinklersChange(FarmerSysView.StdMaterialStruct Data, short Unit, int MapNo)
        {
            
            if (Data.SL <= 0 && Data.SS <= 0)
            {
                return;
            }
            
            List<FacSysMAT> FacSysMat = DryDB.FacSysMAT.Where(m => m.Bunit == Unit).ToList();
            StdSysMat StdMat = new StdSysMat();
            HardwareMaterial hardware = new HardwareMaterial();
            StdMat = GetBranchpipe(FacSysMat, Data, Unit, MapNo);
            int BN = (int)(Data.Length / Data.SL); 
            int SN = (int)(Data.width / Data.SS);
            double beforeBranchChangeLegth = Math.Truncate(2.0 * Data.width / 3.0);
            double afterBranchChangeLegth = Data.width - beforeBranchChangeLegth;
            int BeforeCSN = (int)Math.Truncate(beforeBranchChangeLegth / Data.SS);
            int afterCSN = SN - BeforeCSN;
            int BranchSpec = 0; 
            StdMat = GetBranchpipeChange(FacSysMat, Data, Unit, MapNo, beforeBranchChangeLegth, Data.BranchSpec);

            if (StdMat != null)
            {
                StdMat.Price = GetBranchPipelinePrice(StdMat.POMNo, MapNo);
                StdMat.Group = 2;
                MatList.Add(StdMat);

                BranchSpec = StdMat.SpecNo1 == null ? 0 : StdMat.SpecNo1.Value;
            }

            StdMat = GetBranchpipeChange(FacSysMat, Data, Unit, MapNo, afterBranchChangeLegth, Data.ChangeBranchSpec);
            if (StdMat != null)
            {
                StdMat.Price = GetBranchPipelinePrice(StdMat.POMNo, MapNo);
                StdMat.Group = 2;
                MatList.Add(StdMat);

                //BranchSpec = StdMat.SpecNo1 == null ? 0 : StdMat.SpecNo1.Value;
            }

            StdMat = hardware.GetReducingJoint(FacSysMat, Data, BN, Data.BranchSpec, Data.ChangeBranchSpec);
            if (StdMat != null)
            {
                StdMat.Price = GetBranchPipelinePrice(StdMat.POMNo, MapNo);
                StdMat.Group = 2;
                MatList.Add(StdMat);
            }

            StdMat = hardware.GetTeePipe(FacSysMat, Data, BN, MainPipeSpec, BranchSpec);
            if (StdMat != null)
            {
                StdMat.Price = GetBranchPipelinePrice(StdMat.POMNo, MapNo);
                StdMat.Group = 2;
                MatList.Add(StdMat);
            }
            StdMat = hardware.GetValves(FacSysMat, Data, BN, BranchSpec);
            if (StdMat != null)
            {
                StdMat.Price = GetBranchPipelinePrice(StdMat.POMNo, MapNo);
                StdMat.Group = 2;
                MatList.Add(StdMat);
            }
            StdMat = hardware.GetValveAapter(FacSysMat, Data, BN * 2, BranchSpec);
            if (StdMat != null)
            {
                StdMat.Price = GetBranchPipelinePrice(StdMat.POMNo, MapNo);
                StdMat.Group = 2;
                MatList.Add(StdMat);
            }
            StdMat = hardware.GetReceptacle(FacSysMat, Data, BN, Data.ChangeBranchSpec);
            if (StdMat != null)
            {
                StdMat.Price = GetBranchPipelinePrice(StdMat.POMNo, MapNo);
                StdMat.Group = 2;
                MatList.Add(StdMat);
            }
            Standpipeline standPipe = new Standpipeline();
            if (Data.StdpipeHei > 0)
            {
                StdMat = standPipe.GetStandPipeline(FacSysMat, Data);
                float StandPipeAmount = standPipe.GetStandPipeAmount(Data);
                int StandSpec = Data.StdpipeSpec;
                if (StdMat != null)
                {
                    StdMat.Price = GetBranchPipelinePrice(StdMat.POMNo, MapNo);
                    StdMat.Group = 5;
                    MatList.Add(StdMat);
                }

                StdMat = hardware.GetTeePipe(FacSysMat, Data, BeforeCSN * BN, Data.BranchSpec, Data.NozzleSpec);
                if (StdMat != null)
                {
                    StdMat.Price = GetBranchPipelinePrice(StdMat.POMNo, MapNo);
                    StdMat.Group = 5;
                    MatList.Add(StdMat);
                }

                StdMat = hardware.GetTeePipe(FacSysMat, Data, afterCSN * BN, Data.ChangeBranchSpec, Data.NozzleSpec);
                if (StdMat != null)
                {
                    StdMat.Price = GetBranchPipelinePrice(StdMat.POMNo, MapNo);
                    StdMat.Group = 5;
                    MatList.Add(StdMat);
                }
                StdMat = hardware.GetValves(FacSysMat, Data, BN * SN, StandSpec);
                if (StdMat != null)
                {
                    StdMat.Price = GetBranchPipelinePrice(StdMat.POMNo, MapNo);
                    StdMat.Group = 5;
                    MatList.Add(StdMat);
                }
                StdMat = hardware.GetValveAapter(FacSysMat, Data, BN * SN * 2, StandSpec);
                if (StdMat != null)
                {
                    StdMat.Price = GetBranchPipelinePrice(StdMat.POMNo, MapNo);
                    StdMat.Group = 5;
                    MatList.Add(StdMat);
                }
            }
            else 
            {
                StdMat = hardware.GetTeePipe(FacSysMat, Data, BN * SN, BranchSpec, Data.StdpipeSpec);
                if (StdMat != null)
                {
                    StdMat.Price = GetBranchPipelinePrice(StdMat.POMNo, MapNo);
                    StdMat.Group = 5;
                    MatList.Add(StdMat);
                }
            }
            StdMat = hardware.GetMicroSprinklerHead(FacSysMat, Data, BN * SN, Data.NozzleMaterial);
            if (StdMat != null)
            {
                StdMat.Price = GetBranchPipelinePrice(StdMat.POMNo, MapNo);
                StdMat.Group = 8;
                MatList.Add(StdMat);
            }

            //==消耗性材料組==
            //int MatTotalPrice = (int)(MatList.Sum(m => m.Price * m.Amount));
            //StdMat = hardware.GetExpendable(FacSysMat, Data, MatTotalPrice);
            //if (StdMat != null)
            //{
            //    StdMat.Group = 7;
            //    MatList.Add(StdMat);
            //}
        }

        /// <summary>
        /// 微噴灌溉
        /// </summary>
        /// <param name="Data"></param>
        /// <param name="Unit"></param>
        /// <param name="MapNo"></param>

        private void GetMicroSprinklers(FarmerSysView.StdMaterialStruct Data, short Unit, int MapNo)
        {
            if (Data.SL <= 0 && Data.SS <= 0)
            {
                return;
            }
            List<FacSysMAT> FacSysMat = DryDB.FacSysMAT.Where(m => m.Bunit == Unit).ToList();
            StdSysMat StdMat = new StdSysMat();
            HardwareMaterial hardware = new HardwareMaterial();

            StdMat = GetBranchpipe(FacSysMat, Data, Unit, MapNo);
            int BN = (int)(Data.Length / Data.SL); 
            int SN = (int)(Data.width / Data.SS);
            int BranchSpec = Data.BranchSpec == null ? 0 : Data.BranchSpec; 
            if (StdMat != null)
            {
                StdMat.Price = GetBranchPipelinePrice(StdMat.POMNo, MapNo);
                StdMat.Group = 2;
                MatList.Add(StdMat);
                //BranchSpec = StdMat.SpecNo1 == null ? 0 : StdMat.SpecNo1.Value;
            }
            StdMat = hardware.GetTeePipe(FacSysMat, Data, BN, MainPipeSpec, BranchSpec);
            if (StdMat != null)
            {
                StdMat.Price = GetBranchPipelinePrice(StdMat.POMNo, MapNo);
                StdMat.Group = 2;
                MatList.Add(StdMat);
            }
            StdMat = hardware.GetValves(FacSysMat, Data, BN, BranchSpec);
            if (StdMat != null)
            {
                StdMat.Price = GetBranchPipelinePrice(StdMat.POMNo, MapNo);
                StdMat.Group = 2;
                MatList.Add(StdMat);
            }
            StdMat = hardware.GetValveAapter(FacSysMat, Data, BN * 2, BranchSpec);
            if (StdMat != null)
            {
                StdMat.Price = GetBranchPipelinePrice(StdMat.POMNo, MapNo);
                StdMat.Group = 2;
                MatList.Add(StdMat);
            }
            StdMat = hardware.GetReceptacle(FacSysMat, Data, BN, BranchSpec);
            if (StdMat != null)
            {
                StdMat.Price = GetBranchPipelinePrice(StdMat.POMNo, MapNo);
                StdMat.Group = 2;
                MatList.Add(StdMat);
            }
            Standpipeline standPipe = new Standpipeline();
            if (Data.StdpipeHei > 0)
            {
                StdMat = standPipe.GetStandPipeline(FacSysMat, Data);
                float StandPipeAmount = standPipe.GetStandPipeAmount(Data);
                int StandSpec = Data.StdpipeSpec;
                if (StdMat != null)
                {
                    StdMat.Price = GetBranchPipelinePrice(StdMat.POMNo, MapNo);
                    StdMat.Group = 5;
                    MatList.Add(StdMat);
                }
                StdMat = hardware.GetTeePipe(FacSysMat, Data, BN * SN, BranchSpec, StandSpec);
                if (StdMat != null)
                {
                    StdMat.Price = GetBranchPipelinePrice(StdMat.POMNo, MapNo);
                    StdMat.Group = 5;
                    MatList.Add(StdMat);
                }
                StdMat = hardware.GetStraightFaucet(FacSysMat, Data, BN * SN, Data.StdpipeSpec, Data.NozzleSpec);
                if (StdMat != null)
                {
                    StdMat.Price = GetBranchPipelinePrice(StdMat.POMNo, MapNo);
                    StdMat.Group = 5;
                    MatList.Add(StdMat);
                }
                StdMat = hardware.GetValves(FacSysMat, Data, BN * SN, StandSpec);
                if (StdMat != null)
                {
                    StdMat.Price = GetBranchPipelinePrice(StdMat.POMNo, MapNo);
                    StdMat.Group = 5;
                    MatList.Add(StdMat);
                }
                StdMat = hardware.GetValveAapter(FacSysMat, Data, BN * SN * 2, StandSpec);
                if (StdMat != null)
                {
                    StdMat.Price = GetBranchPipelinePrice(StdMat.POMNo, MapNo);
                    StdMat.Group = 5;
                    MatList.Add(StdMat);
                }
            }
            else 
            {
                StdMat = hardware.GetTeePipe(FacSysMat, Data, BN * SN, BranchSpec, Data.StdpipeSpec);
                if (StdMat != null)
                {
                    StdMat.Price = GetBranchPipelinePrice(StdMat.POMNo, MapNo);
                    StdMat.Group = 5;
                    MatList.Add(StdMat);
                }
            }
            StdMat = hardware.GetMicroSprinklerHead(FacSysMat, Data, BN * SN, Data.NozzleMaterial);
            if (StdMat != null)
            {
                StdMat.Price = GetBranchPipelinePrice(StdMat.POMNo, MapNo);
                StdMat.Group = 8;
                MatList.Add(StdMat);
            }

            
            //int MatTotalPrice = (int)(MatList.Sum(m => m.Price * m.Amount));
            //StdMat = hardware.GetExpendable(FacSysMat, Data, MatTotalPrice);
            //if (StdMat != null)
            //{
            //    StdMat.Group = 7;
            //    MatList.Add(StdMat);
            //}
        }

      


        /// <summary>
        /// 穿孔管
        /// </summary>
        /// <param name="Data"></param>
        /// <param name="Unit"></param>
        /// <param name="MapNo"></param>
        private void PerforatedPipe(FarmerSysView.StdMaterialStruct Data, short Unit, int MapNo)
        {
            if (Data.SL <=0 )
            {
                return;
            }
            List<FacSysMAT> FacSysMat = DryDB.FacSysMAT.Where(m => m.Bunit == Unit).ToList();
            StdSysMat StdMat = new StdSysMat();
            int BN = (int)(Data.Length / Data.SL);
            HardwareMaterial hardware = new HardwareMaterial();
            PerforatedPipe perforatedPipe = new PerforatedPipe();
            StdMat = perforatedPipe.GetPerforatedPipe(FacSysMat, Data.NozzleMaterial);
            int PerforatedPipeSpec = 0;
            if (StdMat != null)
            {
                StdMat.Price = GetBranchPipelinePrice(StdMat.POMNo, MapNo);

                //StdMat.Amount = (int)Math.Ceiling((BN * Data.width) / (StdMat.SpecLength == null ? 100 : StdMat.SpecLength.Value));
                if (StdMat.SpecLength == null || StdMat.SpecLength == 0)
                    StdMat.Amount = (int)Math.Ceiling((BN * Data.width) / 100.0);
                else
                    StdMat.Amount = (int)Math.Ceiling((BN * Data.width) / StdMat.SpecLength.Value);
                if (Data.PerforatedPipe == 2)
                {
                    ///////////////////////////////////
                    //StdMat.Amount = StdMat.Amount * 2;
                }

                StdMat.Group = 3;
                MatList.Add(StdMat);
                PerforatedPipeSpec = StdMat.SpecNo1 == null ? 0 : StdMat.SpecNo1.Value;
            }
            if (Data.PerforatedPipe == 1)
            {
                StdMat = hardware.GetTeePipe(FacSysMat, Data, BN, MainPipeSpec, PerforatedPipeSpec);
                if (StdMat != null)
                {
                    StdMat.Price = GetBranchPipelinePrice(StdMat.POMNo, MapNo);
                    StdMat.Group = 3;
                    MatList.Add(StdMat);
                }
            }
            else
            {
                StdMat = hardware.GetReducingCross(FacSysMat, Data, BN, MainPipeSpec, PerforatedPipeSpec);
                if (StdMat != null)
                {
                    StdMat.Price = GetBranchPipelinePrice(StdMat.POMNo, MapNo);
                    StdMat.Group = 3;
                    MatList.Add(StdMat);
                }
            }
            
            StdMat = hardware.GetValves(FacSysMat, Data, BN, PerforatedPipeSpec);
            if (StdMat != null)
            {
                StdMat.Price = GetBranchPipelinePrice(StdMat.POMNo, MapNo);
                StdMat.Group = 3;
                if (Data.PerforatedPipe == 2)
                {
                    StdMat.Amount = StdMat.Amount * 2;
                }
                MatList.Add(StdMat);
            }
            StdMat = hardware.GetPerforatedFittings(FacSysMat, Data, BN, PerforatedPipeSpec);
            if (StdMat != null)
            {
                StdMat.Price = GetBranchPipelinePrice(StdMat.POMNo, MapNo);
                StdMat.Group = 3;
                if (Data.PerforatedPipe == 2)
                {
                    StdMat.Amount = StdMat.Amount * 2;
                }
                MatList.Add(StdMat);
            }
            StdMat = hardware.GetPerforatedFolder(FacSysMat, Data, BN);
            if (StdMat != null)
            {
                StdMat.Price = GetBranchPipelinePrice(StdMat.POMNo, MapNo);
                StdMat.Group = 3;
                if (Data.PerforatedPipe == 2)
                {
                    StdMat.Amount = StdMat.Amount * 2;
                }
                MatList.Add(StdMat);
            }
            
            //int MatTotalPrice = (int)(MatList.Sum(m => m.Price * m.Amount));
            //StdMat = hardware.GetExpendable(FacSysMat, Data, MatTotalPrice);
            //if (StdMat != null)
            //{
            //    StdMat.Group = 7;
            //    MatList.Add(StdMat);
            //}
        }
        /// <summary>
        /// 滴嘴滴灌
        /// </summary>
        /// <param name="Data"></param>
        /// <param name="Unit"></param>
        /// <param name="MapNo"></param>
        public void DripIrrigation(FarmerSysView.StdMaterialStruct Data, short Unit, int MapNo)
        {
            if (Data.SL <= 0 )
            {
                return;
            }
            List<FacSysMAT> FacSysMat = DryDB.FacSysMAT.Where(m => m.Bunit == Unit).ToList();
            StdSysMat StdMat = new StdSysMat();
            HardwareMaterial hardware = new HardwareMaterial();
            DripIrrigation dripIrr = new DripIrrigation();
            int BN = (int)(Data.Length / Data.SL);
            int SN = (int)(Data.width / Data.SS);
            StdMat = dripIrr.GetDripIrrigation(FacSysMat, Data);
            int DripPipeSpec = 0; 
            if (StdMat != null)
            {
                StdMat.Price = GetBranchPipelinePrice(StdMat.POMNo, MapNo);
                //StdMat.Amount = (int)Math.Ceiling((BN * Data.width) / (StdMat.SpecLength == null ? (StdMat.MatType == "PVC" ? 4 : 100) : StdMat.SpecLength.Value));
                if (StdMat.SpecLength == null)
                {
                    if (string.Equals(StdMat.MatType,"PVC"))
                        StdMat.Amount = (int)Math.Ceiling((BN * Data.width) / 4.0);
                    else
                        StdMat.Amount = (int)Math.Ceiling((BN * Data.width) / 100.0);
                }
                else
                {
                    StdMat.Amount = (int)Math.Ceiling((BN * Data.width) / StdMat.SpecLength.Value);
                }
                
           
                StdMat.Group = 4;
                MatList.Add(StdMat);
                //DripPipeSpec = StdMat.SpecNo1 == null ? 0 : StdMat.SpecNo1.Value;
            }
            DripPipeSpec = Data.BranchSpec;
            StdMat = hardware.GetTeePipe(FacSysMat, Data, BN, MainPipeSpec, DripPipeSpec);
            if (StdMat != null)
            {
                StdMat.Price = GetBranchPipelinePrice(StdMat.POMNo, MapNo);
                StdMat.Group = 4;
                MatList.Add(StdMat);
            }
            StdMat = hardware.GetValves(FacSysMat, Data, BN, DripPipeSpec);
            if (StdMat != null)
            {
                StdMat.Price = GetBranchPipelinePrice(StdMat.POMNo, MapNo);
                StdMat.Group = 4;
                MatList.Add(StdMat);
            }
            StdMat = hardware.GetValveAapter(FacSysMat, Data, BN * 2, DripPipeSpec);
            if (StdMat != null)
            {
                StdMat.Price = GetBranchPipelinePrice(StdMat.POMNo, MapNo);
                StdMat.Group = 4;
                MatList.Add(StdMat);
            }
            if (Data.BranchMaterial == 1 || Data.BranchMaterial == 6 || Data.BranchMaterial == 7 || Data.BranchMaterial == 8 || Data.BranchMaterial == 9 || Data.BranchMaterial == 10)//PVC
            {
                StdMat = hardware.GetReceptacle(FacSysMat, Data, BN, DripPipeSpec);
                if (StdMat != null)
                {
                    StdMat.Price = GetBranchPipelinePrice(StdMat.POMNo, MapNo);
                    StdMat.Group = 4;
                    MatList.Add(StdMat);
                }
            }


            if (Data.BranchMaterial == 5) //PE
            {
                if (Data.BranchSpec == 44)
                {
                    StdMat = hardware.GetTeePipe(FacSysMat, Data, BN, MainPipeSpec, 27);
                    if (StdMat != null)
                    {
                        StdMat.Price = GetBranchPipelinePrice(StdMat.POMNo, MapNo);
                        StdMat.Group = 4;
                        MatList.Add(StdMat);
                    }
                    StdMat = hardware.GetValves(FacSysMat, Data, BN, 27);
                    if (StdMat != null)
                    {
                        StdMat.Price = GetBranchPipelinePrice(StdMat.POMNo, MapNo);
                        StdMat.Group = 4;
                        MatList.Add(StdMat);
                    }
                    StdMat = hardware.GetFirstPipeFittings(FacSysMat, Data, 27, 44, BN * 2);
                    if (StdMat != null)
                    {
                        StdMat.Price = GetBranchPipelinePrice(StdMat.POMNo, MapNo);
                        StdMat.Group = 4;
                        MatList.Add(StdMat);
                    }
                }
                else if(Data.BranchSpec == 46)
                {
                    StdMat = hardware.GetTeePipe(FacSysMat, Data, BN, MainPipeSpec, 28);
                    if (StdMat != null)
                    {
                        StdMat.Price = GetBranchPipelinePrice(StdMat.POMNo, MapNo);
                        StdMat.Group = 4;
                        MatList.Add(StdMat);
                    }
                    StdMat = hardware.GetValves(FacSysMat, Data, BN, 28);
                    if (StdMat != null)
                    {
                        StdMat.Price = GetBranchPipelinePrice(StdMat.POMNo, MapNo);
                        StdMat.Group = 4;
                        MatList.Add(StdMat);
                    }
                    StdMat = hardware.GetFirstPipeFittings(FacSysMat, Data, 28, 46, BN * 2 );
                    if (StdMat != null)
                    {
                        StdMat.Price = GetBranchPipelinePrice(StdMat.POMNo, MapNo);
                        StdMat.Group = 4;
                        MatList.Add(StdMat);
                    }
                }
                else if(Data.BranchSpec == 47)
                {
                    StdMat = hardware.GetTeePipe(FacSysMat, Data, BN, MainPipeSpec, 28);
                    if (StdMat != null)
                    {
                        StdMat.Price = GetBranchPipelinePrice(StdMat.POMNo, MapNo);
                        StdMat.Group = 4;
                        MatList.Add(StdMat);
                    }
                    StdMat = hardware.GetValves(FacSysMat, Data, BN, 28);
                    if (StdMat != null)
                    {
                        StdMat.Price = GetBranchPipelinePrice(StdMat.POMNo, MapNo);
                        StdMat.Group = 4;
                        MatList.Add(StdMat);
                    }                    
                    
                    StdMat = hardware.GetFirstPipeFittings(FacSysMat, Data, 28, 47, BN * 2);
                    if (StdMat != null)
                    {
                        StdMat.Price = GetBranchPipelinePrice(StdMat.POMNo, MapNo);
                        StdMat.Group = 4;
                        MatList.Add(StdMat);
                    }
                }
                else if(Data.BranchSpec == 48)
                {
                    StdMat = hardware.GetTeePipe(FacSysMat, Data, BN, MainPipeSpec, 29);
                    if (StdMat != null)
                    {
                        StdMat.Price = GetBranchPipelinePrice(StdMat.POMNo, MapNo);
                        StdMat.Group = 4;
                        MatList.Add(StdMat);
                    }
                    StdMat = hardware.GetValves(FacSysMat, Data, BN, 29);
                    if (StdMat != null)
                    {
                        StdMat.Price = GetBranchPipelinePrice(StdMat.POMNo, MapNo);
                        StdMat.Group = 4;
                        MatList.Add(StdMat);
                    }
                    
                    StdMat = hardware.GetFirstPipeFittings(FacSysMat, Data, 29, 48, BN * 2);
                    if (StdMat != null)
                    {
                        StdMat.Price = GetBranchPipelinePrice(StdMat.POMNo, MapNo);
                        StdMat.Group = 4;
                        MatList.Add(StdMat);
                    }
                }
                else
                {
                    StdMat = hardware.GetFirstPipeFittings(FacSysMat, Data, DripPipeSpec, BN * 2);
                    if (StdMat != null)
                    {
                        StdMat.Price = GetBranchPipelinePrice(StdMat.POMNo, MapNo);
                        StdMat.Group = 4;
                        MatList.Add(StdMat);
                    }
                }
                StdMat = hardware.GetLastFolder(FacSysMat, Data, DripPipeSpec, BN);
                if (StdMat != null)
                {
                    StdMat.Price = GetBranchPipelinePrice(StdMat.POMNo, MapNo);
                    StdMat.Group = 4;
                    MatList.Add(StdMat);
                }
            }
            else
            {
                StdMat = hardware.GetStraightFaucet(FacSysMat, Data, BN * SN, Data.BranchSpec, Data.NozzleSpec);
                if (StdMat != null)
                {
                    StdMat.Price = GetBranchPipelinePrice(StdMat.POMNo, MapNo);
                    StdMat.Group = 4;
                    MatList.Add(StdMat);
                }
            }
            
            StdMat = hardware.GetDrip(FacSysMat, Data, Data.NozzleSpec, BN * SN);
            if (StdMat != null)
            {
                StdMat.Price = GetBranchPipelinePrice(StdMat.POMNo, MapNo);
                StdMat.Group = 8;
                MatList.Add(StdMat);
            }
            
            //int MatTotalPrice = (int)(MatList.Sum(m => m.Price * m.Amount));
            //StdMat = hardware.GetExpendable(FacSysMat, Data, MatTotalPrice);
            //if (StdMat != null)
            //{
            //    StdMat.Group = 7;
            //    MatList.Add(StdMat);
            //}
        }
        /// <summary>
        /// 滴水管滴灌
        /// </summary>
        /// <param name="Data"></param>
        /// <param name="Unit"></param>
        /// <param name="MapNo"></param>
        /// <param name="L1orL2">1: L1, 2: L2</param>
        public void DripPipeIrrigation(FarmerSysView.StdMaterialStruct Data, short Unit, int MapNo, int L1orL2)
        {
            if (Data.SL <= 0 )
            {
                return;
            }
            List<FacSysMAT> FacSysMat = DryDB.FacSysMAT.Where(m => m.Bunit == Unit).ToList();
            StdSysMat StdMat = new StdSysMat();
            HardwareMaterial hardware = new HardwareMaterial();
            DripIrrigation dripIrr = new DripIrrigation();
            int BN = (int)(Data.Length / Data.SL);
            int SN = (int)(Data.width / Data.SS);
            StdMat = dripIrr.GetDripPipeIrrigation(FacSysMat, Data.NozzleMaterial);
            int DripPipeSpec = 0;
            DripPipeSpec = Data.BranchSpec;
                       
            if (StdMat != null)
            {
                StdMat.Price = GetBranchPipelinePrice(StdMat.POMNo, MapNo);
                //StdMat.Amount = (int)Math.Ceiling((BN * Data.width) / StdMat.SpecLength == null ? 100 : StdMat.SpecLength.Value);
                
                //StdMat.Amount = (int)Math.Ceiling(BN * (double)Data.width);
                if (StdMat.SpecLength == null)
                    StdMat.Amount = (int)Math.Ceiling((BN * Data.width) / 100.0);
                else
                    StdMat.Amount = (int)Math.Ceiling((BN * Data.width) / StdMat.SpecLength.Value);
                 
                StdMat.Group = 4;
                MatList.Add(StdMat);
            }
            StdMat = hardware.GetTeePipe(FacSysMat, Data, BN, MainPipeSpec, DripPipeSpec);
            if (StdMat != null)
            {
                StdMat.Price = GetBranchPipelinePrice(StdMat.POMNo, MapNo);
                StdMat.Group = 4;
                MatList.Add(StdMat);
            }
            StdMat = hardware.GetValves(FacSysMat, Data, BN, DripPipeSpec);
            if (StdMat != null)
            {
                StdMat.Price = GetBranchPipelinePrice(StdMat.POMNo, MapNo);
                StdMat.Group = 4;
                MatList.Add(StdMat);
            }
            
            //StdMat = hardware.GetValveAapter(FacSysMat, Data, BN * 2, DripPipeSpec);
            //if (StdMat != null)
            //{
            //    StdMat.Price = GetBranchPipelinePrice(StdMat.POMNo, MapNo);
            //    StdMat.Group = 4;
            //    StdMat.Order = 4;
            //    MatList.Add(StdMat);
            //}
            
            //StdMat = hardware.GetFirstPipeFittings(FacSysMat, Data, DripPipeSpec, BN * 2);///DripPipeSpec
            StdMat = hardware.GetFirstPipeFittings(FacSysMat, Data, DripPipeSpec, Data.NozzleSpec, BN * 2);           
            if (StdMat != null)
            {
                StdMat.Price = GetBranchPipelinePrice(StdMat.POMNo, MapNo);
                StdMat.Group = 4;
                MatList.Add(StdMat);
            }
            StdMat = hardware.GetLastFolder(FacSysMat, Data, Data.NozzleSpec, BN);
            if (StdMat != null)
            {
                StdMat.Price = GetBranchPipelinePrice(StdMat.POMNo, MapNo);
                StdMat.Group = 4;
                MatList.Add(StdMat);
            }

            
            //int MatTotalPrice = (int)(MatList.Sum(m => m.Price * m.Amount));
            //StdMat = hardware.GetExpendable(FacSysMat, Data, MatTotalPrice);
            //if (StdMat != null)
            //{
            //    StdMat.Group = 7;
            //    MatList.Add(StdMat);
            //}
        }
        /// <summary>
        /// L1主管組
        /// </summary>
        /// <param name="Data"></param>
        /// <param name="Unit"></param>
        /// <param name="MapNo"></param>
        private void L1MainPipeLine(FarmerSysView.StdMaterialStruct Data, short Unit, int MapNo)
        {
            List<FacSysMAT> FacSysMat = DryDB.FacSysMAT.Where(m => m.Bunit == Unit).ToList();
            StdSysMat StdMat = new StdSysMat();
            HardwareMaterial hardware = new HardwareMaterial();
            MainPipeSpec = Data.L1Spec;
            if (Data.L1Material == 20)
            {
                Data.L1Material = 1;
                Data.ddl_EndType = 1;
                StdMat = hardware.GetValves(FacSysMat, Data, 1, 29);
                if (StdMat != null)
                {
                    StdMat.Price = GetBranchPipelinePrice(StdMat.POMNo, MapNo);
                    StdMat.Group = 4;
                    MatList.Add(StdMat);
                }
            }
            
            //if (StdMat != null)
            //{
            //    if (Data.L1MatAmt != 0)
            //        StdMat.Amount = Data.L1MatAmt;
            //    MatList.Add(StdMat);
            //}

            StdMat = hardware.GetBend(FacSysMat, Data, 1);
            if (StdMat != null)
            {
                StdMat.Price = GetBranchPipelinePrice(StdMat.POMNo, MapNo);
                StdMat.Group = 1;
                MatList.Add(StdMat);
            }
            StdMat = hardware.GetReceptacle(FacSysMat, Data, 1, MainPipeSpec);
            if (StdMat != null)
            {
                StdMat.Price = GetBranchPipelinePrice(StdMat.POMNo, MapNo);
                StdMat.Group = 1;
                MatList.Add(StdMat);
            }


        }
        /// <summary>
        /// L2主管組
        /// </summary>
        /// <param name="Data"></param>
        /// <param name="Unit"></param>
        /// <param name="MapNo"></param>
        private void L2MainPipeLine(FarmerSysView.StdMaterialStruct Data, short Unit, int MapNo)
        {
            List<FacSysMAT> FacSysMat = DryDB.FacSysMAT.Where(m => m.Bunit == Unit).ToList();
            StdSysMat StdMat = new StdSysMat();
            HardwareMaterial hardware = new HardwareMaterial();
            
            //StdMat = GetMainpipe(FacSysMat, Data, Unit, MapNo,2);
            //MainPipeSpec = StdMat == null ? 0 : StdMat.SpecNo1 == null ? 0 : Convert.ToInt32(StdMat.SpecNo1);
            MainPipeSpec = Data.L2Spec;
            //if (StdMat != null)
            //{
            //    if (Data.L1MatAmt != 0)
            //        StdMat.Amount = Data.L1MatAmt;
            //    MatList.Add(StdMat);
            //}

            StdMat = hardware.GetBend(FacSysMat, Data,2);
            if (StdMat != null)
            {
                StdMat.Price = GetBranchPipelinePrice(StdMat.POMNo, MapNo);
                StdMat.Group = 1;
                if(MatList.Any(m=>m.POMNo == StdMat.POMNo))
                {
                    MatList.Where(m => m.POMNo == StdMat.POMNo).FirstOrDefault().Amount += StdMat.Amount;
                }
                //MatList.Add(StdMat);
            }
            StdMat = hardware.GetReceptacle(FacSysMat, Data, 1, MainPipeSpec);
            if (StdMat != null)
            {
                StdMat.Price = GetBranchPipelinePrice(StdMat.POMNo, MapNo);
                StdMat.Group = 1;
                if (MatList.Any(m => m.POMNo == StdMat.POMNo))
                {
                    MatList.Where(m => m.POMNo == StdMat.POMNo).FirstOrDefault().Amount += StdMat.Amount;
                }
                //MatList.Add(StdMat);
            }
        }
        /// <summary>
        /// 取得主管材料
        /// </summary>
        /// <param name="FacSysMat"></param>
        /// <param name="Data"></param>
        /// <param name="Unit"></param>
        /// <param name="MapNo"></param>
        /// <param name="L1orL2">1: L1, 2: L2</param>
        /// <returns></returns>
        private StdSysMat GetMainpipe(List<FacSysMAT> FacSysMat, FarmerSysView.StdMaterialStruct Data, short Unit, int MapNo,int L1orL2)
        {
            
            switch (L1orL2)
            {
                case 1:
                    StdSysMat StdMat_L1 = new MainPipeline().GetMainPipeline(FacSysMat, Data,1);
                    if (StdMat_L1 != null)
                    {
                        StdMat_L1.Price = Data.L1Price;
                        return StdMat_L1;
                    }
                    break;
                case 2:
                    StdSysMat StdMat_L2 = new MainPipeline().GetMainPipeline(FacSysMat, Data, 2);
                    if (StdMat_L2 != null)
                    {
                        StdMat_L2.Price = Data.L2Price; 
                        return StdMat_L2;
                    }
                    break;
            }
            
            return null;
        }

        private void GetExpendableMat(FarmerSysView.StdMaterialStruct Data, short Unit, int MapNo)
        {
            if (Data.L1Len <= 0 && Data.SL <=0)
            {
                return;
            }

            HardwareMaterial hardware = new HardwareMaterial();
            StdSysMat StdMat = new StdSysMat();
            List<FacSysMAT> FacSysMat = DryDB.FacSysMAT.Where(m => m.Bunit == Unit).ToList();
            int MatTotalPrice = (int)(MatList.Sum(m => m.Price * m.Amount));
            StdMat = hardware.GetExpendable(FacSysMat, Data, MatTotalPrice);
            if (StdMat != null)
            {
                StdMat.Group = 7;
                MatList.Add(StdMat);
            }
        }

        /// <summary>
        /// 取得支管資料
        /// </summary>
        /// <param name="FacSysMat"></param>
        /// <param name="Data"></param>
        /// <param name="Unit"></param>
        /// <param name="MapNo"></param>
        /// <returns></returns>
        private StdSysMat GetBranchpipe(List<FacSysMAT> FacSysMat, FarmerSysView.StdMaterialStruct Data, short Unit, int MapNo)
        {        
            StdSysMat StdMat = new BranchPipeline().GetBranchPipeline(FacSysMat, Data);
           
            if (StdMat != null)
            {
                StdMat.Price = GetBranchPipelinePrice(StdMat.POMNo, MapNo);
                return StdMat;
            }
            return null;
        }

        /// <summary>
        /// 取得變徑支管資料
        /// </summary>
        /// <param name="FacSysMat"></param>
        /// <param name="Data"></param>
        /// <param name="Unit"></param>
        /// <param name="MapNo"></param>
        /// <returns></returns>
        private StdSysMat GetBranchpipeChange(List<FacSysMAT> FacSysMat, FarmerSysView.StdMaterialStruct Data, short Unit, int MapNo, double ch_length, int ch_spec)
        {
            StdSysMat StdMat = new BranchPipeline().GetBranchPipeline(FacSysMat, Data, ch_length, ch_spec);

            if (StdMat != null)
            {
                StdMat.Price = GetBranchPipelinePrice(StdMat.POMNo, MapNo); 
                return StdMat;
            }
            return null;
        }

        /// <summary>
        /// 取得物料價格
        /// </summary>
        /// <param name="POMNo"></param>
        /// <returns></returns>
        private double GetBranchPipelinePrice(int POMNo, int Mapno)
        {
            SummaryView cases = DryDB.SummaryView.Where(m => m.MapNo == Mapno).ToList().FirstOrDefault(); 
            PriceOfMat PriceData = new FarmerSysDBService().GetMatPrice(cases.ApplyYear, cases.ApplyUnit, POMNo);
            GetData gd = new GetData();
            bool slope = gd.GetIs12FromMapNo((int)cases.MapNo);
            if (PriceData == null)
            {
                return 0;
            } else
            {
                if (slope == true)
                {
                    return (int)(PriceData.Price * 1.2);
                }
                else
                {
                    return PriceData.Price;
                }
            }
            
        }
    }
}

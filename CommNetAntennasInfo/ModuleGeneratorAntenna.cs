
using KSP.Localization;
using System.Collections.Generic;
using System.Linq;
using static CommNetAntennasInfo.Logging;

namespace CommNetAntennasInfo
{
    public class ModuleGeneratorAntenna : ModuleGenerator
    {
        [KSPField(guiActive = true, guiActiveEditor = true, guiName = "#CAE_PAW_ConsumptionPermanent")]
        string ECConsumptionStr = "";

        ModuleDeployableAntenna moduleDeployable;

        public override string GetModuleDisplayName()
        {
            return Localizer.Format("#CAE_ModuleGeneratorAntennaDisplayName");
        }

        //private void OnAnimationGroupStateChanged(ModuleAnimationGroup data0, bool data1)
        //{
        //    Log("OnAnimationGroupStateChanged: " + data0 + " " + data1);
        //}

        public void LateUpdate()
        {
            if (moduleDeployable != null)
            {
                if (moduleDeployable.deployState == ModuleDeployablePart.DeployState.EXTENDED)
                {
                    if (!this.generatorIsActive)
                    {
                        //Log("Activate()");
                        Activate();
                    }
                }
                else
                {
                    if (this.generatorIsActive)
                    {
                        //Log("Shutdown()");
                        Shutdown();
                    }
                }
            }
        }

        //public void OnDisable()
        //{
        //    G﻿ameEvents.OnAnimationGroupStateChanged.Remove(OnAnimationGroupStateChanged);
        //}

        public void Start()
        {
            //Log("ModuleGeneratorAntenna Start");
            //G﻿ameEvents.OnAnimationGroupStateChanged.Add(OnAnimationGroupStateChanged);

            List<ModuleDeployableAntenna> MDAs = part.Modules.OfType<ModuleDeployableAntenna>().ToList();
            if (MDAs.Count == 1)
                moduleDeployable = MDAs[0];

            if (moduleDeployable == null)
                Activate();

            double rate = this.resHandler.inputResources[0].rate;
            ECConsumptionStr = Formatter.StringRate(rate);

            Fields["efficiency"].guiActive = false;
            Fields["efficiency"].guiActiveEditor = false;
            Fields["displayStatus"].guiActive = false;
            Fields["displayStatus"].guiActiveEditor = false;

            Events["Activate"].guiActive = false;
            Events["Shutdown"].guiActive = false;

            //CommNet.CommNetParams commNetParams = HighLogic.CurrentGame.Parameters.CustomParams<CommNet.CommNetParams>();
            //commNetParams.rangeModifier  

            //foreach (var f in this.Events)
            //        Log(f.name + " | " + f.guiName + " | " + f.GUIName);
        }


        public override string GetInfo()
        {
            //Log("ModuleGeneratorAntenna GetInfo");
            moduleName = "ModuleGeneratorAntenna";
            string text = "";

            List<ModuleDeployableAntenna> MDAs = part.Modules.OfType<ModuleDeployableAntenna>().ToList();
            if (MDAs.Count == 1)
                moduleDeployable = MDAs[0];

            if (moduleDeployable == null)
                text += Localizer.Format("#CAE_ConsumptionMessage");
            else
                text += Localizer.Format("#CAE_ConsumptionMessageExt");


            if (this.resHandler.inputResources.Count == 1
                && this.resHandler.outputResources.Count == 0
                && this.resHandler.inputResources[0].name == "ElectricCharge")
            {
                double rate = this.resHandler.inputResources[0].rate;
                text+= Localizer.Format("#CAE_Consumption", Formatter.StringRate(rate));
            }
            else
                text += base.GetInfo();

            return text;

        }  
    }
}

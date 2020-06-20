
using KSP.Localization;
using System.Collections.Generic;
using System.Linq;
using static CommNetAntennasInfo.Logging;

namespace CommNetAntennasInfo
{
    public class ModuleGeneratorAntenna : ModuleGenerator, IModuleInfo
    {
        [KSPField(guiActive = true, guiActiveEditor = true, guiName = "#CAE_PAW_ConsumptionPermanent")]
        string ECConsumptionStr = "";


        //ModuleDeployableAntenna moduleDeployable;
        ModuleDataTransmitter moduleDT;

        //private void OnAnimationGroupStateChanged(ModuleAnimationGroup data0, bool data1)
        //{
        //    Log("OnAnimationGroupStateChanged: " + data0 + " " + data1);
        //}


        public void LateUpdate()
        {
            if (HighLogic.LoadedScene == GameScenes.FLIGHT && moduleDT != null)
            {
                if (moduleDT.CanComm())
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

            //Log("{0} {1} {2} {3}", moduleDT.isEnabled, moduleDT.isActiveAndEnabled, moduleDT.moduleIsEnabled, moduleDT.CanComm());
        }

        //public void OnDisable()
        //{
        //    G﻿ameEvents.OnAnimationGroupStateChanged.Remove(OnAnimationGroupStateChanged);
        //}

        public void Start()
        {
            //Log("ModuleGeneratorAntenna Start");
            //G﻿ameEvents.OnAnimationGroupStateChanged.Add(OnAnimationGroupStateChanged);

            //List<ModuleDeployableAntenna> MDAs = part.Modules.OfType<ModuleDeployableAntenna>().ToList();
            //if (MDAs.Count == 1)
            //    moduleDeployable = MDAs[0];

            List<ModuleDataTransmitter> MDTs = part.Modules.OfType<ModuleDataTransmitter>().ToList();
            if (MDTs.Count == 1)
                moduleDT = MDTs[0];

            if (HighLogic.LoadedScene == GameScenes.FLIGHT && /*moduleDeployable*/ moduleDT == null)
                Activate();

            double rate = this.resHandler.inputResources[0].rate;
            ECConsumptionStr = Formatter.StringRate(rate);

            Fields["efficiency"].guiActive = false;
            Fields["efficiency"].guiActiveEditor = false;
            Fields["displayStatus"].guiActive = false;
            Fields["displayStatus"].guiActiveEditor = false;

            Events["Activate"].guiActive = false;
            Events["Shutdown"].guiActive = false;

            Actions["ToggleAction"].active = false;
            Actions["ActivateAction"].active = false;
            Actions["ShutdownAction"].active = false;

            //CommNet.CommNetParams commNetParams = HighLogic.CurrentGame.Parameters.CustomParams<CommNet.CommNetParams>();
            //commNetParams.rangeModifier  

            //foreach (var f in this.Events)
            //        Log(f.name + " | " + f.guiName + " | " + f.GUIName);
        }


        public override string GetModuleDisplayName()
        {
            return Localizer.Format("#CAE_ModuleGeneratorAntennaDisplayName");
        }
        public override string GetInfo()
        {
            //Log("ModuleGeneratorAntenna GetInfo");
            //moduleName = "ModuleGeneratorAntenna";
            string text = "";

            List<ModuleDeployableAntenna> MDAs = part.Modules.OfType<ModuleDeployableAntenna>().ToList();
            ModuleDeployableAntenna moduleDeployable = null;
            if (MDAs.Count == 1)
                moduleDeployable = MDAs[0];


            bool ContainsDMSIGINT = part.Modules.Contains("DMSIGINT");
            bool ContainsDMSoilMoisture = part.Modules.Contains("DMSoilMoisture");

            //List<ModuleDataTransmitter> MDTs = part.Modules.OfType<ModuleDataTransmitter>().ToList();
            //if (MDTs.Count == 1)
            //    moduleDT = MDTs[0];

            if (moduleDeployable == null && !ContainsDMSIGINT && !ContainsDMSoilMoisture)
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

        /// <summary>
        /// Return a method delegate to draw a custom panel, or null if not necessary.
        /// </summary>
        /// <returns></returns>
        public Callback<UnityEngine.Rect> GetDrawModulePanelCallback() => null;

        /// <summary>
        /// Return a string title for your module.
        /// </summary>
        /// <returns></returns>
        public string GetModuleTitle() => Localizer.Format("#CAE_ModuleGeneratorAntennaDisplayName");

        /// <summary>
        /// Return a string to be displayed in the main 
        /// information box on the tooltip, 
        /// or null if nothing is that important to be up there.
        /// </summary>
        /// <returns></returns>
        public string GetPrimaryField() => null;
    }
}

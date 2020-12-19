
using KSP.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using static CommNetAntennasInfo.Logging;

namespace CommNetAntennasInfo
{
    public class ModuleAntennaInfo : PartModule
    {
        [KSPField(guiActive = false, guiActiveEditor = false, guiName = "#autoLOC_6001352", 
            groupName = "CommNetA", groupDisplayName = "#CommNetA_Name", groupStartCollapsed = true)]
        string StatusStr;
        [KSPField(guiActive = true, guiActiveEditor = false, guiName = "#autoLOC_6001428",
            groupName = "CommNetA", groupDisplayName = "#CommNetA_Name", groupStartCollapsed = true)]
        string AntennaStateStr;

        [KSPField(guiActive = true, guiActiveEditor = true, guiName = "#CAE_PAW_Type",
            groupName = "CommNetA", groupDisplayName = "#CommNetA_Name", groupStartCollapsed = true)]
        string AntennaTypeStr;

        [KSPField(guiActive = true, guiActiveEditor = true, guiName = "#autoLOC_6001429", 
            groupName = "CommNetA", groupDisplayName = "#CommNetA_Name", groupStartCollapsed = true)]
        string AntennaRatingStr;

        [KSPField(guiActive = true, guiActiveEditor = true, guiName = "#CAE_PAW_Combinability_Exponent", advancedTweakable = true,
            groupName = "CommNetA", groupDisplayName = "#CommNetA_Name", groupStartCollapsed = true)]
        string CombinabilityExponentStr;

        [KSPField(guiActive = true, guiActiveEditor = true, guiName = "#CAE_PAW_Packet", advancedTweakable = true,
            groupName = "CommNetA", groupDisplayName = "#CommNetA_Name", groupStartCollapsed = true)]
        string PacketStr;

        [KSPField(guiActive = true, guiActiveEditor = true, guiName = "#CAE_PAW_Bandwidth", advancedTweakable = true,
            groupName = "CommNetA", groupDisplayName = "#CommNetA_Name", groupStartCollapsed = true)]
        string BandwidthStr;

        [KSPField(guiActive = true, guiActiveEditor = true, guiName = "#CAE_PAW_ConsumptionTransmit", advancedTweakable = true,
            groupName = "CommNetA", groupDisplayName = "#CommNetA_Name", groupStartCollapsed = true)]
        string DataResourceCostStr;


        ModuleDeployableAntenna moduleDA;
        ModuleDataTransmitter moduleDT;

        void LateUpdate()
        {
            if (moduleDA)
                StatusStr = moduleDA.status;

            if (moduleDT)
                AntennaStateStr = moduleDT.statusText;

        }

        public void Start()
        {
            CommNet.CommNetParams commNetParams = HighLogic.CurrentGame.Parameters.CustomParams<CommNet.CommNetParams>();
            List<ModuleDataTransmitter> MDTs = part.Modules.OfType<ModuleDataTransmitter>().ToList();
            List<ModuleDeployableAntenna> MDAs = part.Modules.OfType<ModuleDeployableAntenna>().ToList();

            if (MDTs.Count != 1)
            {
                foreach (var mdt in MDTs)
                    mdt.Fields["powerText"].SetValue(
                        Formatter.ValueShort(mdt.antennaPower * commNetParams.rangeModifier) +
                            (mdt.antennaCombinable
                            ? string.Format(" ({0}, {1}: {2})", Localizer.Format("#CAE_PAW_Combinability"),
                            Localizer.Format("#CAE_PAW_Combinab_Exponent_Short"), mdt.antennaCombinableExponent)
                            : "")
                        , mdt);

                foreach (var field in Fields) {
                    field.guiActive = false;
                    field.guiActiveEditor = false;
                }

                return;
            }

            moduleDT = MDTs[0];

            //ModuleDeployableAntenna
            // status | Status | Retracted Retracting.. Extended Extending..
            //moduleDA.status

            //statusText   Antenna State   Idle
            //moduleDT.statusText;

            if (MDAs.Count == 1)
            {
                moduleDA = MDAs[0];
                moduleDA.Fields["status"].guiActive = false;
                
                Fields["StatusStr"].guiActive = true;
                Fields["StatusStr"].guiActiveEditor = true;
            }

            moduleDT.Fields["statusText"].guiActive = false;

            moduleDT.Fields["powerText"].guiActive = false;
            moduleDT.Fields["powerText"].guiActiveEditor = false;

            double antennaPowerModified = moduleDT.antennaPower * commNetParams.rangeModifier;

            AntennaRatingStr = Formatter.ValueShort(antennaPowerModified);
            AntennaTypeStr = Formatter.ToTitleCase(moduleDT.antennaType.displayDescription());
            DataResourceCostStr = Localizer.Format("#CAE_EC_Mit", moduleDT.DataResourceCost.ToString("#.##"));
            BandwidthStr = Localizer.Format("#CAE_Mit_S", (moduleDT.packetSize / moduleDT.packetInterval).ToString("#.##"));   

            PacketStr = Localizer.Format("#CAE_Mit", moduleDT.packetSize.ToString("#.#")) + " & " +
            Localizer.Format("#CAE_EC", moduleDT.packetResourceCost.ToString("#.##")); 
            
            if (moduleDT.antennaCombinable)
            {
                AntennaRatingStr += " " + Localizer.Format("#autoLOC_236248");
                CombinabilityExponentStr = moduleDT.antennaCombinableExponent.ToString();
            }
            else
            {
                Fields["CombinabilityExponentStr"].guiName = "#CAE_PAW_Combinability";
                CombinabilityExponentStr = Localizer.Format("#autoLOC_439840");
            }

            if (moduleDT.antennaType == AntennaType.INTERNAL)
            {
                foreach (var f in Fields)
                {
                    if (f.name != "AntennaTypeStr" && f.name != "AntennaRatingStr")
                    {
                        f.guiActive = false;
                        f.guiActiveEditor = false;
                    }
                }
            }
        }
    }
}

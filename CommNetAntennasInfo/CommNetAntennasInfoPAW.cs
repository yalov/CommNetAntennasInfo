
using KSP.Localization;
using System.Collections.Generic;
using static CommNetAntennasInfo.Logging;

namespace CommNetAntennasInfo
{
    public class ModuleAntennaInfo : PartModule
    {
        [KSPField(guiActive = true, guiActiveEditor = true, guiName = "#CAE_PAW_Type")]
        string AntennaTypeStr;

        [KSPField(guiActive = true, guiActiveEditor = true, guiName = "#autoLOC_6001429")]
        string AntennaRatingStr;

        [KSPField(guiActive = true, guiActiveEditor = true, guiName = "#CAE_PAW_Combinability_Exponent", advancedTweakable = true)]
        string CombinabilityExponentStr;

        [KSPField(guiActive = true, guiActiveEditor = false, guiName = "#CAE_PAW_Consumption", advancedTweakable = true)]
        string DataResourceCostStr;


        public void Start()
        {
            CommNet.CommNetParams commNetParams = HighLogic.CurrentGame.Parameters.CustomParams<CommNet.CommNetParams>();
            List<ModuleDataTransmitter> MDTs = part.Modules.GetModules<ModuleDataTransmitter>();
            
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

            ModuleDataTransmitter moduleDT = MDTs[0];
            
             
            //ModuleDeployableAntenna
            //statusText   Antenna State   Idle
            //moduleDT.statusText;
            moduleDT.Fields["powerText"].guiActive = false;
            moduleDT.Fields["powerText"].guiActiveEditor = false;

            double antennaPowerModified = moduleDT.antennaPower * commNetParams.rangeModifier;

            AntennaRatingStr = Formatter.ValueShort(antennaPowerModified);
            AntennaTypeStr = Formatter.ToTitleCase(moduleDT.antennaType.displayDescription());
            DataResourceCostStr = Localizer.Format("#CAE_PAW_EC_Mit", moduleDT.DataResourceCost.ToString("#.##"));

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
                moduleDT.Fields["statusText"].guiActive = false; 

                AntennaTypeStr += string.Format(" ({0}: {1}", Localizer.Format("#CAE_PAW_Rating_Short"), AntennaRatingStr);

                if (moduleDT.antennaCombinable)
                    AntennaTypeStr += string.Format(", {0}: {1}", Localizer.Format("#CAE_PAW_Combinab_Exponent_Short"), moduleDT.antennaCombinableExponent.ToString());

                AntennaTypeStr += ")";

                foreach (var f in Fields)
                {
                    if (f.name != "AntennaTypeStr")
                    {
                        f.guiActive = false;
                        f.guiActiveEditor = false;
                    }
                }
            }
        }
    }
}

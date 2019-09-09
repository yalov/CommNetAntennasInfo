
using KSP.Localization;
using System.Collections.Generic;

namespace CommNetAntennasInfo
{
    public class ModuleAntennaInfo : PartModule
    {
        [KSPField(guiActive = true, guiActiveEditor = true, guiName = "#autoLOC_6001429")]
        string AntennaRatingStr;

        [KSPField(guiActive = true, guiActiveEditor = true, guiName = "#CAE_F_Combinability_Exponent", advancedTweakable = true)]
        string CombinabilityExponentStr;

        [KSPField(guiActive = true, guiActiveEditor = true, guiName = "#CAE_F_Type", advancedTweakable = true)]
        string AntennaTypeStr;

        public void Start()
        {
            CommNet.CommNetParams commNetParams = HighLogic.CurrentGame.Parameters.CustomParams<CommNet.CommNetParams>();
            List<ModuleDataTransmitter> listmDT = part.Modules.GetModules<ModuleDataTransmitter>();

            if (listmDT.Count != 1)
                return;

            ModuleDataTransmitter moduleDT = listmDT[0];
            moduleDT.Fields["powerText"].guiActive = false;
            moduleDT.Fields["powerText"].guiActiveEditor = false;

            double antennaPowerModified = moduleDT.antennaPower * commNetParams.rangeModifier;

            AntennaRatingStr = Formatter.ValueShort(antennaPowerModified);

            AntennaTypeStr = Formatter.ToTitleCase(moduleDT.antennaType.displayDescription());

            if (moduleDT.antennaCombinable)
            {
                CombinabilityExponentStr = moduleDT.antennaCombinableExponent.ToString();
                AntennaRatingStr += " " + Localizer.Format("#autoLOC_236248");
            }
            else
            {
                Fields["CombinabilityExponentStr"].guiName = "#CAE_F_Combinability";
                CombinabilityExponentStr = Localizer.Format("#autoLOC_439840");
            }

            if (moduleDT.antennaType == AntennaType.INTERNAL)
            {
                Fields["AntennaTypeStr"].advancedTweakable = false;
                AntennaTypeStr += string.Format(" ({0}: {1}", Localizer.Format("#CAE_F_Rating_Short"), AntennaRatingStr);

                if (moduleDT.antennaCombinable)
                    AntennaTypeStr += string.Format(", {0}: {1}", Localizer.Format("#CAE_F_Combinab_Exponent_Short"), moduleDT.antennaCombinableExponent.ToString());

                AntennaTypeStr += ")";

                Fields["AntennaRatingStr"].guiActive = false;
                Fields["AntennaRatingStr"].guiActiveEditor = false;
                Fields["CombinabilityExponentStr"].guiActive = false;
                Fields["CombinabilityExponentStr"].guiActiveEditor = false;
            }


        }
    }
}
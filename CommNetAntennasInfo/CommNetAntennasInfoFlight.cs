
using KSP.Localization;
using System.Collections.Generic;

namespace CommNetAntennasInfo
{
    public class ModuleAntennaInfo : PartModule
    {
        [KSPField(guiActive = true, guiActiveEditor = true, guiName = "#CAE_F_Type", advancedTweakable = true)]
        string AntennaType_str;

        [KSPField(guiActive = true, guiActiveEditor = true, guiName = "#CAE_F_Combinability_Exponent", advancedTweakable = true)]
        string Combinability_Exponent_str;

        public void Start()
        {
            List<ModuleDataTransmitter> listmDT = part.Modules.GetModules<ModuleDataTransmitter>();

            if (listmDT.Count != 1)
                return;

            ModuleDataTransmitter moduleDT = listmDT[0];

            AntennaType_str = Formatter.ToTitleCase(moduleDT.antennaType.displayDescription());

            if (moduleDT.antennaCombinable)
                Combinability_Exponent_str = moduleDT.antennaCombinableExponent.ToString();
            else
            {
                Fields["Combinability_Exponent_str"].guiName = "#CAE_F_Combinability";
                Combinability_Exponent_str = Localizer.Format("#autoLOC_439840");
            }
        }
    }
}
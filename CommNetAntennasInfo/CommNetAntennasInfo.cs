using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KSP.Localization;




namespace CommnetAntennaExtension
{
    [KSPAddon(KSPAddon.Startup.EditorAny, false)]
    public class SituationModule : MonoBehaviour
    {
        CommNet.CommNetParams commNetParams;
        int CurrentTrackingStationIndex;

        private string SmartAlphaChannel(int i, bool start = true)
        {
            if (i == CurrentTrackingStationIndex)
                return start ? "<color=#ffffffff>" : "</color>";
            else
                return start ? "<color=#ffffff7f>" : "</color>";
        }

        private double GetPowerMostCommonInternalAntenna(List<AvailablePart> parts)
        {
            List<double> powers = new List<double>();

            foreach (AvailablePart part in parts)
            {
                List<ModuleDataTransmitter> modules = part.partPrefab.Modules.GetModules<ModuleDataTransmitter>();
                if (modules.Count != 1) continue;

                ModuleDataTransmitter moduleDT = modules[0];
                if (moduleDT.CommType != AntennaType.INTERNAL) continue;

                powers.Add(moduleDT.CommPower);
            }

            if (powers.Count > 0)
                return powers.GroupBy(i => i).OrderByDescending(grp => grp.Count()).Select(grp => grp.Key).First();
            else
                return 5000;
        }

        private void Start()
        {
            
            commNetParams = HighLogic.CurrentGame.Parameters.CustomParams<CommNet.CommNetParams>();
            int LevelsTracking = 3;

            CBK cbk = new CBK();
            
            if (cbk.founded)
                LevelsTracking = cbk.levelsTracking;

            float[] TrackingStationLvls = new float[LevelsTracking];  //{ 0.0f, 0.5f, 1.0f };
            for (int i = 0; i < LevelsTracking; i++)
                TrackingStationLvls[i] = (float)i / (LevelsTracking - 1);

            float CurrentTrackingStationLvl = ScenarioUpgradeableFacilities.GetFacilityLevel(SpaceCenterFacility.TrackingStation);
            CurrentTrackingStationIndex = TrackingStationLvls.IndexOf(CurrentTrackingStationLvl);

            double[] DSNPowerModified = new double[LevelsTracking];
            string[] DSNPowerModified_str = new string[LevelsTracking];
            for (int i = 0; i < LevelsTracking; i++)
            {
                double dsnPower = GameVariables.Instance.GetDSNRange(i / (LevelsTracking - 1f));
                DSNPowerModified[i] = dsnPower * commNetParams.DSNModifier;
                DSNPowerModified_str[i] = Formatter.ValueExtraShortSpaced(DSNPowerModified[i]);
            }

            List<AvailablePart> partsDT = PartLoader.LoadedPartsList.Where
                    (p => p.partPrefab.Modules.GetModules<ModuleDataTransmitter>().Any()).ToList();

            double BuiltInPowerModified = GetPowerMostCommonInternalAntenna(partsDT) * commNetParams.rangeModifier;
            string BuiltInPowerModified_str = Formatter.ValueExtraShortSpaced(BuiltInPowerModified);
            
            foreach (AvailablePart part in partsDT)
            {
                List<ModuleDataTransmitter> modules = part.partPrefab.Modules.GetModules<ModuleDataTransmitter>();

                if (modules.Count != 1) continue;

                ModuleDataTransmitter moduleDT = modules[0];

                List<AvailablePart.ModuleInfo> modinfos = part.moduleInfos;

                foreach (AvailablePart.ModuleInfo modinfo in modinfos)
                {
                    if (modinfo.moduleName == moduleDT.GUIName)
                    {
                        double antennaPowerModified = moduleDT.antennaPower * commNetParams.rangeModifier;

                        string[] DSNranges_str = new string[LevelsTracking];
                        double[] DSNranges = new double[LevelsTracking];
                        for (int i = 0; i < LevelsTracking; i++)
                            DSNranges[i] = Math.Sqrt(DSNPowerModified[i] * antennaPowerModified);

                        if (moduleDT.CommType != AntennaType.INTERNAL)
                        {
                            string BuiltInranges_str = Formatter.DistanceShort(Math.Sqrt(BuiltInPowerModified * antennaPowerModified));

                            for (int i = 0; i < LevelsTracking; i++)
                                DSNranges_str[i] = Formatter.DistanceShort(DSNranges[i]);

                            modinfo.info =

                            Localizer.Format("#autoLOC_7001005", Formatter.ToTitleCase(moduleDT.antennaType.displayDescription()))
                                + Localizer.Format("#autoLOC_7001006", Formatter.ValueShort(antennaPowerModified))
                                + (moduleDT.CommCombinable
                                ? Localizer.Format("#CAE_Combinability_Exponent", moduleDT.CommCombinableExponent)
                                
                                : Localizer.Format("#CAE_Not_Combinable"))
                                + Localizer.Format("#CAE_Title_vs");
                           
                            

                            if (moduleDT.CommType == AntennaType.RELAY)
                                modinfo.info += BuiltInPowerModified_str + Localizer.Format("#CAE_Built_In") 
                                + Localizer.Format("#CAE_Spaces") + BuiltInranges_str + "\n";

                            for (int i = 0; i < LevelsTracking; i++)
                                modinfo.info += 
                                    SmartAlphaChannel(i) +
                                        DSNPowerModified_str[i] + Localizer.Format("#CAE_DSN_LN", i + 1) 
                                        + Localizer.Format("#CAE_Spaces") + DSNranges_str[i]
                                    + SmartAlphaChannel(i, false) + "\n";

                            modinfo.info += Localizer.Format("#autoLOC_236840"/*\n<b>Packet size: </b><<1>> Mits\n*/, moduleDT.packetSize.ToString("F1"))                                
                                + Localizer.Format("#autoLOC_236841"/*<b>Bandwidth: </b><<1>> Mits/sec\n*/, (moduleDT.packetSize / moduleDT.packetInterval).ToString("F2"))    

                                + Localizer.Format("#autoLOC_236842"/*\n\nWhen Transmitting:*/)
                                + Localizer.Format("#CAE_NOB", Localizer.Format("#autoLOC_244332"))
                                + Localizer.Format("#autoLOC_244197", Localizer.Format("#autoLOC_501004"/*Electric Charge*/),
                                (moduleDT.packetResourceCost / moduleDT.packetInterval).ToString("F1"));
                        }
                        else
                        {
                            for (int i = 0; i < LevelsTracking; i++)
                                DSNranges_str[i] = Formatter.DistanceExtraShort(DSNranges[i]);

                            string type = Formatter.ToTitleCase(moduleDT.CommType.displayDescription());
                            //Internal - ok
                            if (type.Length > 8) type = type.Substring(0, 7) + ".";
                            
                            modinfo.info =
                                Localizer.Format("#CAE_Type", type) + ", "
                                + Localizer.Format("#CAE_Rating", Formatter.ValueShort(antennaPowerModified))
                                + (moduleDT.CommCombinable ? ", e:" + moduleDT.CommCombinableExponent : "")
                                + Localizer.Format("#CAE_DSN_Short") + " ";


                            if (LevelsTracking % 4 == 0) modinfo.info += "<nobr>";

                            for (int i = 0; i < LevelsTracking; i++)
                                modinfo.info += 
                                    SmartAlphaChannel(i) + 
                                        DSNranges_str[i] + (i != (LevelsTracking - 1)?", ":"") + 
                                    SmartAlphaChannel(i, false);

                            if (LevelsTracking % 4 == 0) modinfo.info += "</nobr>";

                            modinfo.info += Localizer.Format("#CAE_Orange", Localizer.Format("#autoLOC_236846"));  // #autoLOC_236846 = \n<i>Cannot transmit science</i>\n
                        }
                    }
                }
            }
        }
    }
}

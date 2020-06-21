using System;
using System.Collections.Generic;
using UnityEngine;
using KSP.Localization;
using System.Linq;
using static CommNetAntennasInfo.Logging;


namespace CommNetAntennasInfo
{
    [KSPAddon(KSPAddon.Startup.EditorAny, false)]
    public class SituationModule : MonoBehaviour
    {
        private void Start()
        {
            
            commNetParams = HighLogic.CurrentGame.Parameters.CustomParams<CommNet.CommNetParams>();
            int TrackingLevels = 3;

            CBK cbk = new CBK();
            
            if (cbk.founded)
                TrackingLevels = cbk.levelsTracking;

            float[] TrackingStationFloats = new float[TrackingLevels];  //{ 0.0f, 0.5f, 1.0f };
            for (int i = 0; i < TrackingLevels; i++)
                TrackingStationFloats[i] = (float)i / (TrackingLevels - 1);

            float CurrentTrackingStationFloat = ScenarioUpgradeableFacilities.GetFacilityLevel(SpaceCenterFacility.TrackingStation);
            CurrentTrackingStationIndex = TrackingStationFloats.IndexOf(CurrentTrackingStationFloat);

            double[] DSNPowerModified = new double[TrackingLevels];
            string[] DSNPowerModified_str = new string[TrackingLevels];
            for (int i = 0; i < TrackingLevels; i++)
            {
                double dsnPower = GameVariables.Instance.GetDSNRange(TrackingStationFloats[i]);
                DSNPowerModified[i] = dsnPower /* * commNetParams.DSNModifier*/;
                DSNPowerModified_str[i] = Formatter.ValueExtraShortSpaced(DSNPowerModified[i]);
            }

            List<AvailablePart> partsDT = PartLoader.LoadedPartsList.Where
                    (p => p.partPrefab.Modules.OfType<ModuleDataTransmitter>().Any()).ToList();

            //Log("partsDT.count: " + partsDT.Count);

            double BuiltInPowerModified = GetPowerMostCommonInternalAntenna(partsDT) * commNetParams.rangeModifier;
            string BuiltInPowerModified_str = Formatter.ValueExtraShortSpaced(BuiltInPowerModified);
            
            foreach (AvailablePart part in partsDT)
            {
                List<ModuleDataTransmitter> modules = part.partPrefab.Modules.OfType<ModuleDataTransmitter>().ToList();
                //Log("modules.count: " + modules.Count);
                if (modules.Count == 0) continue;

                //foreach (var i in part.moduleInfos)
                    //Log(i.moduleName + " " + modules[0].GUIName);
                    
                
                var modinfos = part.moduleInfos.Where(i => modules[0].GUIName.Contains(i.moduleName)).ToList();
                //Log("modinfos.count: " + modinfos.Count);
                if (modules.Count != modinfos.Count) continue;

                for (int index= 0; index < modules.Count ; index++)
                {
                    ModuleDataTransmitter moduleDT = modules[index];
                    AvailablePart.ModuleInfo modinfo = modinfos[index];

                    double antennaPowerModified = moduleDT.antennaPower * commNetParams.rangeModifier;

                    string[] DSNranges_str = new string[TrackingLevels];
                    double[] DSNranges = new double[TrackingLevels];
                    for (int i = 0; i < TrackingLevels; i++)
                        DSNranges[i] = Math.Sqrt(DSNPowerModified[i] * antennaPowerModified);

                    if (moduleDT.CommType != AntennaType.INTERNAL)
                    {
                        string BuiltInranges_str = Formatter.DistanceShort(Math.Sqrt(BuiltInPowerModified * antennaPowerModified));

                        for (int i = 0; i < TrackingLevels; i++)
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

                        for (int i = 0; i < TrackingLevels; i++)
                            modinfo.info += 
                                SmartAlphaChannel(i) +
                                    DSNPowerModified_str[i] + Localizer.Format("#CAE_DSN_LN", i + 1) 
                                    + Localizer.Format("#CAE_Spaces") + DSNranges_str[i]
                                + SmartAlphaChannel(i, false) + "\n";

                        modinfo.info += Localizer.Format("#autoLOC_236840"/*\n<b>Packet size: </b><<1>> Mits\n*/, moduleDT.packetSize.ToString("F1")).TrimEnd()
                                               + " & " + Localizer.Format("#CAE_EC", moduleDT.packetResourceCost.ToString("#.##")) + "\n"
                            + Localizer.Format("#autoLOC_236841"/*<b>Bandwidth: </b><<1>> Mits/sec\n*/, (moduleDT.packetSize / moduleDT.packetInterval).ToString("F2"))

                            + "\n"+Localizer.Format("#autoLOC_236842"/*\n\nWhen Transmitting:*/).Trim()
                            + Localizer.Format("#CAE_Consumption",
                                    Localizer.Format("#CAE_EC_Mit", moduleDT.DataResourceCost.ToString("#.#")))
                            ;


                    }
                    else // INTERNAL
                    {
                        for (int i = 0; i < TrackingLevels; i++)
                            DSNranges_str[i] = Formatter.DistanceExtraShort(DSNranges[i]);

                        string type = Formatter.ToTitleCase(moduleDT.CommType.displayDescription());
                        //Internal - ok
                        if (type.Length > 8) type = type.Substring(0, 7) + ".";
                            
                        modinfo.info =
                            Localizer.Format("#CAE_Type", type) + ", "
                            + Localizer.Format("#CAE_Rating", Formatter.ValueShort(antennaPowerModified))
                            + (moduleDT.CommCombinable ? ", e:" + moduleDT.CommCombinableExponent : "")
                            + Localizer.Format("#CAE_DSN_Short") + " ";


                        if (TrackingLevels % 4 == 0) modinfo.info += "<nobr>";

                        for (int i = 0; i < TrackingLevels; i++)
                            modinfo.info += 
                                SmartAlphaChannel(i) + 
                                    DSNranges_str[i] + (i != (TrackingLevels - 1)?", ":"") + 
                                SmartAlphaChannel(i, false);

                        if (TrackingLevels % 4 == 0) modinfo.info += "</nobr>";

                        modinfo.info += Localizer.Format("#CAE_Orange", Localizer.Format("#autoLOC_236846"));  // #autoLOC_236846 = \n<i>Cannot transmit science</i>\n
                    }
                }
            }
        }

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
                List<ModuleDataTransmitter> modules = part.partPrefab.Modules.OfType<ModuleDataTransmitter>().ToList();
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

        CommNet.CommNetParams commNetParams;
        int CurrentTrackingStationIndex;
    }
}

﻿using KSP.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using static CommNetAntennasInfo.Logging;

namespace CommNetAntennasInfo
{
    public class ModuleAntennaInfo : PartModule
    {
        [KSPField(guiActive = true, guiActiveEditor = true, guiName = "#CAE_PAW_Type")]
        protected string AntennaTypeStr;

        [KSPField(guiActive = true, guiActiveEditor = true, guiName = "#autoLOC_6001429")]
        protected string AntennaRatingStr;

        [KSPField(guiActive = true, guiActiveEditor = true, guiName = "#CAE_PAW_Combinability_Exponent", advancedTweakable = true)]
        protected string CombinabilityExponentStr;

        [KSPField(guiActive = true, guiActiveEditor = true, guiName = "#CAE_PAW_Packet", advancedTweakable = true)]
        protected string PacketStr;

        [KSPField(guiActive = true, guiActiveEditor = true, guiName = "#CAE_PAW_Bandwidth", advancedTweakable = true)]
        protected string BandwidthStr;

        [KSPField(guiActive = true, guiActiveEditor = true, guiName = "#CAE_PAW_ConsumptionTransmit", advancedTweakable = true)]
        protected string DataResourceCostStr;

        [KSPField(guiActive = false, guiActiveEditor = false, guiName = "#CAE_PAW_OtherVesselRating", guiUnits = "G"),
            UI_FloatRange(minValue = 0f, maxValue = 10000f, stepIncrement = 0.001f)]
        public float OtherVesselRating = 0.001f;

        [KSPField(guiActive = false, guiActiveEditor = false, guiName = "#CAE_PAW_Antennas")]
        protected string VesselRatingStr;

        [KSPField(guiActive = false, guiActiveEditor = false, guiName = "#autoLOC_6001722")]
        protected string VesselRelayRatingStr;

        [KSPEvent(guiActive = true, guiActiveEditor = true, active = false, guiName = "#CAE_PAW_Show_VR")]
        protected void VesselRatingUpdate()
        {
            Fields["VesselRatingStr"].guiActive = true;
            Fields["VesselRatingStr"].guiActiveEditor = true;
            Fields["VesselRelayRatingStr"].guiActive = true;
            Fields["VesselRelayRatingStr"].guiActiveEditor = true;

            Fields["OtherVesselRating"].guiActive = true;
            Fields["OtherVesselRating"].guiActiveEditor = true;

            List<ModuleDataTransmitter> DTs;
            if (HighLogic.LoadedScene == GameScenes.EDITOR || HighLogic.LoadedScene == GameScenes.FLIGHT)
            {
                if (HighLogic.LoadedScene == GameScenes.EDITOR)
                {
                    DTs = EditorLogic.fetch.ship.parts.SelectMany(p => p.Modules.OfType<ModuleDataTransmitter>()).ToList();
                }
                else //if (HighLogic.LoadedScene == GameScenes.FLIGHT)
                {
                    DTs = vessel.FindPartModulesImplementing<ModuleDataTransmitter>();
                }

                VesselRating(DTs, out string all_value, out string relay_value);

                VesselRatingStr = all_value;
                VesselRelayRatingStr = relay_value;

                Events["VesselRatingUpdate"].guiName = Localizer.Format("#CAE_PAW_Update_VR");
            }
        }

        static CommNet.CommNetParams commNetParams;

        public static double AntennaSum(ICollection<ModuleDataTransmitter> list)
        {
            var strongestAntenna = 0d;
            var sum = 0d;
            var weighedSum = 0d;
            

            foreach (var a in list)
            {
                double antennaPowerModified = a.antennaPower * commNetParams.rangeModifier;
                if (antennaPowerModified > strongestAntenna)
                    strongestAntenna = antennaPowerModified;

                sum += antennaPowerModified;

                double exp;
                if (a.CommType == AntennaType.INTERNAL) 
                    exp = 0;
                else
                    exp = a.CommCombinableExponent;

                weighedSum += antennaPowerModified * exp;
            }

            var averageExp = weighedSum / sum;
            var result = strongestAntenna * Math.Pow(sum / strongestAntenna, averageExp);

            //Log("{0}, {1}, {2} {3}", strongestAntenna, sum, averageExp, result);

            return result;
        }

        public void VesselRating(ICollection<ModuleDataTransmitter> DTs, 
            out string all_value, out string relay_value)
        {
            double power = AntennaSum(DTs);
            all_value = Localizer.Format("#CAE_PAW_Rating", DTs.Count, Formatter.ValueShort(power, 2));

            double distance = Math.Pow(power * OtherVesselRating * 1e9, 0.5);
            all_value += ", " + Localizer.Format("#CAE_PAW_Distance", Formatter.DistanceShort(distance));

            var Relays = DTs.Where(z => z.CommType == AntennaType.RELAY).ToList();

            if (Relays.Count != 0)
            {
                double powerRelay = AntennaSum(Relays);
                relay_value = Localizer.Format("#CAE_PAW_Rating", Relays.Count, Formatter.ValueShort(powerRelay, 2));

                double relay_distance = Math.Pow(powerRelay * OtherVesselRating * 1e9, 0.5);
                relay_value += ", " + Localizer.Format("#CAE_PAW_Distance", Formatter.DistanceShort(relay_distance));

            }
            else
            {
                relay_value = Localizer.Format("#CAE_PAW_Rating", 0, 0);
            }
        }

        public void Start()
        {
            BasePAWGroup CommunicationGroup = new BasePAWGroup("CF_Comms", "#CAE_PAW_Group_Name", true);
            
            Fields[nameof(AntennaTypeStr)].group = CommunicationGroup;
            Fields[nameof(AntennaRatingStr)].group = CommunicationGroup;
            Fields[nameof(CombinabilityExponentStr)].group = CommunicationGroup;
            Fields[nameof(PacketStr)].group = CommunicationGroup;
            Fields[nameof(BandwidthStr)].group = CommunicationGroup;
            Fields[nameof(DataResourceCostStr)].group = CommunicationGroup;
            Fields[nameof(OtherVesselRating)].group = CommunicationGroup;
            Fields[nameof(VesselRatingStr)].group = CommunicationGroup;
            Fields[nameof(VesselRelayRatingStr)].group = CommunicationGroup;
            Events[nameof(VesselRatingUpdate)].group = CommunicationGroup;

            commNetParams = HighLogic.CurrentGame.Parameters.CustomParams<CommNet.CommNetParams>();
            List<ModuleDataTransmitter> MDTs = part.Modules.OfType<ModuleDataTransmitter>().ToList();
            List<ModuleDeployableAntenna> MDAs = part.Modules.OfType<ModuleDeployableAntenna>().ToList();

            var dsnpower = GameVariables.Instance.GetDSNRange(
                ScenarioUpgradeableFacilities.GetFacilityLevel(SpaceCenterFacility.TrackingStation));

            OtherVesselRating = (float)(dsnpower / 1e9);

            Fields["OtherVesselRating"].guiActive = false;
            Fields["OtherVesselRating"].guiActiveEditor = false;

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

            var moduleDT = MDTs[0];
            moduleDT.Fields["statusText"].group = CommunicationGroup;
            moduleDT.Fields["powerText"].guiActive = false;
            moduleDT.Fields["powerText"].guiActiveEditor = false;

            //ModuleDeployableAntenna
            // status | Status | Retracted Retracting.. Extended Extending..
            //moduleDA.status

            //statusText   Antenna State   Idle
            //moduleDT.statusText;

            if (MDAs.Count == 1)
            {
                var moduleDA = MDAs[0];
                moduleDA.Fields["status"].group = CommunicationGroup;
                moduleDA.Fields["status"].guiActiveEditor = true;
            }

            //List<ModuleCommand> MCs = part.Modules.OfType<ModuleCommand>().ToList();
            //
            //if (MCs.Count == 1)
            //{
            //    MCs[0].Fields["commNetSignal"].group = CommNetApawGroup;
            //    MCs[0].Fields["commNetFirstHopDistance"].group = CommNetApawGroup;
            //}

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
                Events["VesselRatingUpdate"].active = true;

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

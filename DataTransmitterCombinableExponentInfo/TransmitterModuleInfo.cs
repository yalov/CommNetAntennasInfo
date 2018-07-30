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
        
        private List<AvailablePart> parts = new List<AvailablePart>();

        private void Start()
        {
            //Logging.Log("start");

            parts = PartLoader.LoadedPartsList.Where
                    (p => p.partPrefab.Modules.GetModules<ModuleDataTransmitter>().Any()).ToList();

            foreach (AvailablePart part in parts)
            {
                List<ModuleDataTransmitter> modules = part.partPrefab.Modules.GetModules<ModuleDataTransmitter>();

                if (modules.Count != 1) continue;

                ModuleDataTransmitter moduleDT = modules[0];

                //Logging.Log("part="+part.name + " " + moduleDT.CommCombinable + " " + moduleDT.CommCombinableExponent);

                if (moduleDT.CommCombinable)
                {
                    List<AvailablePart.ModuleInfo> modinfos = part.moduleInfos;

                    foreach (AvailablePart.ModuleInfo modinfo in modinfos)
                    {
                        // modinfo.moduleName
                        // modinfo.moduleDisplayName
                        // modinfo.moduleName
                        // moduleDT.moduleName
                        // moduleDT.name
                        // moduleDT.GetModuleDisplayName()
                        // moduleDT.GUIName
                        // moduleDT.ClassName


                        // TODO: CHECK THIS ON nonENGLISH!
                        if (modinfo.moduleName == moduleDT.GUIName)
                        {
                            modinfo.info = modinfo.info.Replace(
                                Localizer.Format("#autoLOC_236248"),
                                Localizer.Format("#Combinability_Exponent", moduleDT.CommCombinableExponent));

                           // modinfo.info += "\n\n" + "Combinability Exponent: " + moduleDT.CommCombinableExponent;
                        }
                    }

                }

            }
        }

        //private void OnDestroy()
        //{
        //    foreach (AvailablePart part in _partsWithDataTransmitter)
        //    {
        //        List<AvailablePart.ModuleInfo> moduleInfos = part.partPrefab.partInfo.moduleInfos;
        //        foreach (AvailablePart.ModuleInfo moduleInfo in moduleInfos)
        //        {
        //            int startIndex = moduleInfo.info.IndexOf("--------------------------------", StringComparison.Ordinal);
        //            if (startIndex < 0) continue;
        //            moduleInfo.info = moduleInfo.info.Remove(startIndex - 1);
        //        }
        //    }
        //}
    }
}

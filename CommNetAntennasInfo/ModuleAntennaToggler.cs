
using KSP.Localization;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static CommNetAntennasInfo.Logging;

namespace CommNetAntennasInfo
{
    class ModuleAntennaToggler : PartModule, IScalarModule, IModuleInfo
    {
        [KSPField(isPersistant = true)]
        public bool AntennaEnabled = true;

        [KSPField(guiName = "#autoLOC_6001352" /*Status*/, guiActive = true, guiActiveEditor = true)]
        public string statusText = Localizer.Format("#autoLOC_6001072"); //"Enabled";

        private float scalar;

        public override void OnStart(PartModule.StartState state)
        {
            SetScalar(AntennaEnabled ? 1f : 0f);
        }

        [KSPEvent(guiName = "#CAE_PAW_DisableAntenna", guiActive = true, guiActiveEditor = true, active = true)]
        public void ToggleAntenna()
        {
            AntennaEnabled = !AntennaEnabled;
            SetScalar(AntennaEnabled ? 1f : 0f);
        }


        private void updatePAWText()
        {
            Events["ToggleAntenna"].guiName = AntennaEnabled ? Localizer.Format("#CAE_PAW_DisableAntenna") : Localizer.Format("#CAE_PAW_EnableAntenna");
            statusText = AntennaEnabled ? Localizer.Format("#autoLOC_6001072")/*Enabled*/ : Localizer.Format("#autoLOC_6001071")/*Disabled*/;
        }

        // -------IScalarModule-----------

        public string ScalarModuleID => "ModuleAntennaToggler";
        public float GetScalar => scalar;
        public void SetScalar(float t) 
        {
            //Log("SetScalar: " + t);
            scalar = t;
            updatePAWText();
        }

        public bool CanMove => true;

        public EventData<float, float> OnMoving => null;

        public EventData<float> OnStop => null;

        public bool IsMoving() => false;

        public void SetUIRead(bool state)
        {
        }

        public void SetUIWrite(bool state)
        {
        }


        //---------IModuleInfo-------------------

        public string GetModuleTitle() => Localizer.Format("#CAE_ModuleAntennaTogglerInfoTitle");


        public override string GetInfo() => Localizer.Format("#CAE_ModuleAntennaTogglerInfo");


        public Callback<Rect> GetDrawModulePanelCallback() => null;

        public string GetPrimaryField() => null;
    }
}

using System;
using UnityEngine;
using TownOfUsReworked.Classes;
using HarmonyLib;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.Functions;
using System.Linq;

namespace TownOfUsReworked.Modules
{
    [HarmonyPatch]
    public static class StartStop
    {
        private static Color oldColor;

        public static void StartRewind(TimeLord role)
        {
            RecordRewind.rewinding = true;
            RecordRewind.whoIsRewinding = role;
            PlayerControl.LocalPlayer.moveable = false;
            oldColor = HudManager.Instance.FullScreen.color;
            HudManager.Instance.FullScreen.color = new Color(0f, 0.5f, 0.8f, 0.3f);
            HudManager.Instance.FullScreen.enabled = true;
            role.StartRewind = DateTime.UtcNow;
        }

        public static void StopRewind(TimeLord role)
        {
            role.FinishRewind = DateTime.UtcNow;
            RecordRewind.rewinding = false;
            PlayerControl.LocalPlayer.moveable = true;
            var fs = false;

            switch (GameOptionsManager.Instance.currentNormalGameOptions.MapId)
            {
                case 0:
                case 1:
                case 3:
                    var reactor1 = ShipStatus.Instance.Systems[SystemTypes.Reactor].Cast<ReactorSystemType>();
                    var oxygen1 = ShipStatus.Instance.Systems[SystemTypes.LifeSupp].Cast<LifeSuppSystemType>();
                    fs = reactor1.IsActive || oxygen1.IsActive;
                    break;

                case 2:
                    var seismic = ShipStatus.Instance.Systems[SystemTypes.Laboratory].Cast<ReactorSystemType>();
                    fs = seismic.IsActive;
                    break;

                case 4:
                    var reactor = ShipStatus.Instance.Systems[SystemTypes.Reactor].Cast<HeliSabotageSystem>();
                    fs = reactor.IsActive;
                    break;

                case 5:
                    fs = PlayerControl.LocalPlayer.myTasks.ToArray().Any(x => x.TaskType == SubmergedCompatibility.RetrieveOxygenMask);
                    break;

                case 6:
                    var reactor3 = ShipStatus.Instance.Systems[SystemTypes.Reactor].Cast<ReactorSystemType>();
                    var oxygen3 = ShipStatus.Instance.Systems[SystemTypes.LifeSupp].Cast<LifeSuppSystemType>();
                    var seismic2 = ShipStatus.Instance.Systems[SystemTypes.Laboratory].Cast<ReactorSystemType>();
                    fs = reactor3.IsActive || seismic2.IsActive || oxygen3.IsActive;
                    break;
            }

            HudManager.Instance.FullScreen.enabled = fs;
            HudManager.Instance.FullScreen.color = oldColor;
        }
    }
}
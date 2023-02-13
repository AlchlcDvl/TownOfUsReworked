using System;
using UnityEngine;
using TownOfUsReworked.Enums;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.Classes
{
    public class StartStop
    {
        public static Color oldColor;

        public static void StartRewind(Role role)
        {
            if (role.RoleType != RoleEnum.TimeLord && role.RoleType != RoleEnum.Retributionist)
                return;

            RecordRewind.rewinding = true;
            RecordRewind.whoIsRewinding = role;
            PlayerControl.LocalPlayer.moveable = false;
            oldColor = HudManager.Instance.FullScreen.color;
            HudManager.Instance.FullScreen.gameObject.active = true;
            HudManager.Instance.FullScreen.color = new Color32(0, 0, 255, 128);
            HudManager.Instance.FullScreen.enabled = true;

            if (role.RoleType != RoleEnum.TimeLord)
                ((TimeLord)role).StartRewind = DateTime.UtcNow;
            else if (role.RoleType != RoleEnum.Retributionist)
                ((Retributionist)role).StartRewind = DateTime.UtcNow;
        }

        public static void StopRewind(Role role)
        {
            if (role.RoleType != RoleEnum.TimeLord && role.RoleType != RoleEnum.Retributionist)
                return;

            if (role.RoleType != RoleEnum.TimeLord)
                ((TimeLord)role).FinishRewind = DateTime.UtcNow;
            else if (role.RoleType != RoleEnum.Retributionist)
                ((Retributionist)role).FinishRewind = DateTime.UtcNow;

            RecordRewind.rewinding = false;
            PlayerControl.LocalPlayer.moveable = true;
            SubmergedCompatibility.CheckOutOfBoundsElevator(PlayerControl.LocalPlayer);
            HudManager.Instance.FullScreen.enabled = false;
            HudManager.Instance.FullScreen.color = oldColor;
        }
    }
}
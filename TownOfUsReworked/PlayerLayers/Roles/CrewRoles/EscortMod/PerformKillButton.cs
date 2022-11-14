/*using HarmonyLib;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.PlayerLayers.Modifiers;
using UnityEngine;
using Object = UnityEngine.Object;
using System.Collections.Generic;
using System.Linq;
using System;
using Hazel;
using TownOfUsReworked.Extensions;
using InnerNet;
using System.Collections;
using Reactor.Utilities;
using Reactor.Utilities.Extensions;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.EscortMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Escort))
                return true;

            var role = Role.GetRole<Escort>(PlayerControl.LocalPlayer);

            if (!PlayerControl.LocalPlayer.CanMove)
                return false;

            if (PlayerControl.LocalPlayer.Data.IsDead)
                return false;

            if (!__instance.enabled)
                return false;

            if (role.RoleblockTimer() != 0f)
                return false;

            role.LastBlocked = DateTime.UtcNow;

            try
            {
                AudioClip MediumSFX = TownOfUsReworked.loadAudioClipFromResources("TownOfUsReworked.Resources.Seer.raw");
                SoundManager.Instance.PlaySound(MediumSFX, false, 0.4f);
            }
            catch {}

            return false;
        }
    }
}*/
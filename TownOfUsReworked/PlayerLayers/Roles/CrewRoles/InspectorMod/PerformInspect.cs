using System;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using Il2CppSystem.Collections.Generic;
using Random = UnityEngine.Random;
using System.Linq;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.InspectorMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformInspect
    {
        public static bool Prefix(KillButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Inspector))
                return false;

            var role = Role.GetRole<Inspector>(PlayerControl.LocalPlayer);

            if (__instance == role.InspectButton)
            {
                if (!Utils.ButtonUsable(__instance))
                    return false;

                if (role.InspectTimer() != 0f)
                    return false;

                if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                    return false;

                if (role.InspectedPlayers.Contains(role.ClosestPlayer.PlayerId))
                    return false;

                var interact = Utils.Interact(role.Player, role.ClosestPlayer, Role.GetRoleValue(RoleEnum.Pestilence));

                if (interact[3] == true)
                {
                    role.InspectedPlayers.Add(role.ClosestPlayer.PlayerId);
                    var results = new List<Role>();
                    var targetRole = Role.GetRole(role.ClosestPlayer);
                    results.Add(targetRole);

                    var i = 0;

                    while (i < 4)
                    {
                        if (results.Count >= PlayerControl.AllPlayerControls.Count)
                            break;

                        var random = Random.RandomRangeInt(0, Role.AllRoles.Count());
                        var role2 = Role.AllRoles.ToList()[random];

                        if (role2.RoleType != targetRole.RoleType)
                        {
                            results.Add(role2);
                            i++;
                        }

                        results.Shuffle();
                    }

                    role.InspectResults.Add(role.ClosestPlayer.PlayerId, results);

                    try
                    {
                        //SoundManager.Instance.PlaySound(TownOfUsReworked.PhantomWin, false, 1f);
                    } catch {}
                }
                
                if (interact[0] == true)
                    role.LastInspected = DateTime.UtcNow;
                else if (interact[1] == true)
                    role.LastInspected.AddSeconds(CustomGameOptions.ProtectKCReset);

                return false;
            }
            
            return false;
        }
    }
}

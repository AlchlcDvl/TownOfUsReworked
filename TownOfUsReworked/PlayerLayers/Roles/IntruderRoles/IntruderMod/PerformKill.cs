using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using System;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.IntruderMod
{
    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
    public static class PerformKill
    {
        public static bool Prefix(AbilityButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, Faction.Intruder))
                return true;

            if (PlayerControl.LocalPlayer.Is(ObjectifierEnum.Allied) || PlayerControl.LocalPlayer.Is(ObjectifierEnum.Traitor) || PlayerControl.LocalPlayer.Is(ObjectifierEnum.Fanatic))
                return false;

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Ghoul))
                return false;

            var role = Role.GetRole<IntruderRole>(PlayerControl.LocalPlayer);

            if (__instance == role.KillButton)
            {
                if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                    return false;

                if (role.KillTimer() != 0f)
                    return false;

                var interact = Utils.Interact(role.Player, role.ClosestPlayer, true);

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Janitor))
                {
                    var jani = (Janitor)role;

                    if (interact[3]  || interact[0])
                    {
                        jani.LastKilled = DateTime.UtcNow;

                        if (CustomGameOptions.JaniCooldownsLinked)
                            jani.LastCleaned = DateTime.UtcNow;
                    }
                    else if (interact[1])
                    {
                        jani.LastKilled.AddSeconds(CustomGameOptions.ProtectKCReset);

                        if (CustomGameOptions.JaniCooldownsLinked)
                            jani.LastCleaned.AddSeconds(CustomGameOptions.ProtectKCReset);
                    }
                    else if (interact[2])
                    {
                        jani.LastKilled.AddSeconds(CustomGameOptions.VestKCReset);

                        if (CustomGameOptions.JaniCooldownsLinked)
                            jani.LastCleaned.AddSeconds(CustomGameOptions.VestKCReset);
                    }

                    return false;
                }
                else if (PlayerControl.LocalPlayer.Is(RoleEnum.Godfather))
                {
                    var gf = (Godfather)role;

                    if (interact[3]  || interact[0])
                    {
                        gf.LastKilled = DateTime.UtcNow;

                        if (CustomGameOptions.JaniCooldownsLinked && gf.FormerRole?.RoleType == RoleEnum.Janitor)
                            gf.LastCleaned = DateTime.UtcNow;
                    }
                    else if (interact[1])
                    {
                        gf.LastKilled.AddSeconds(CustomGameOptions.ProtectKCReset);

                        if (CustomGameOptions.JaniCooldownsLinked && gf.FormerRole?.RoleType == RoleEnum.Janitor)
                            gf.LastCleaned.AddSeconds(CustomGameOptions.ProtectKCReset);
                    }
                    else if (interact[2])
                    {
                        gf.LastKilled.AddSeconds(CustomGameOptions.VestKCReset);

                        if (CustomGameOptions.JaniCooldownsLinked && gf.FormerRole?.RoleType == RoleEnum.Janitor)
                            gf.LastCleaned.AddSeconds(CustomGameOptions.VestKCReset);
                    }

                    return false;
                }
                else
                {
                    if (interact[3] || interact[0])
                        role.LastKilled = DateTime.UtcNow;
                    else if (interact[1])
                        role.LastKilled.AddSeconds(CustomGameOptions.ProtectKCReset);
                    else if (interact[2])
                        role.LastKilled.AddSeconds(CustomGameOptions.VestKCReset);

                    return false;
                }
            }

            return true;
        }
    }
}
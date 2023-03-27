using System;
using HarmonyLib;
using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Extensions;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.ShifterMod
{
    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
    public static class PerformShift
    {
        public static bool Prefix(AbilityButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Shifter))
                return true;

            var role = Role.GetRole<Shifter>(PlayerControl.LocalPlayer);

            if (__instance == role.ShiftButton)
            {
                if (role.ShiftTimer() != 0f)
                    return false;

                if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                    return false;

                var interact = Utils.Interact(role.Player, role.ClosestPlayer);

                if (interact[3])
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                    writer.Write((byte)ActionsRPC.Shift);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer.Write(role.ClosestPlayer.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Shift(role, role.ClosestPlayer);
                }

                if (interact[0])
                    role.LastShifted = DateTime.UtcNow;
                else if (interact[1])
                    role.LastShifted.AddSeconds(CustomGameOptions.ProtectKCReset);

                return false;
            }

            return true;
        }

        public static void Shift(Shifter shifterRole, PlayerControl other)
        {
            var role = Role.GetRole(other);
            var shifter = shifterRole.Player;

            if (!other.Is(Faction.Crew) || other.IsFramed())
            {
                Utils.RpcMurderPlayer(shifter, shifter);
                return;
            }

            Role newRole = role.RoleType switch
            {
                RoleEnum.Agent => new Agent(shifter),
                RoleEnum.Altruist => new Altruist(shifter),
                RoleEnum.Coroner => new Coroner(shifter),
                RoleEnum.Crewmate => new Crewmate(shifter),
                RoleEnum.Detective => new Detective(shifter),
                RoleEnum.Engineer => new Engineer(shifter),
                RoleEnum.Escort => new Escort(shifter),
                RoleEnum.Inspector => new Inspector(shifter) { Inspected = ((Inspector)role).Inspected },
                RoleEnum.Sheriff => new Sheriff(shifter) { Interrogated = ((Sheriff)role).Interrogated },
                RoleEnum.Mayor => new Mayor(shifter),
                RoleEnum.Swapper => new Swapper(shifter),
                RoleEnum.Medic => new Medic(shifter),
                RoleEnum.Tracker => new Tracker(shifter) { TrackerArrows = ((Tracker)role).TrackerArrows },
                RoleEnum.Transporter => new Transporter(shifter),
                RoleEnum.Medium => new Medium(shifter),
                RoleEnum.Operative => new Operative(shifter),
                RoleEnum.TimeLord => new TimeLord(shifter),
                RoleEnum.VampireHunter => new VampireHunter(shifter),
                RoleEnum.Veteran => new Veteran(shifter),
                RoleEnum.Vigilante => new Vigilante(shifter),
                RoleEnum.Mystic => new Mystic(shifter),
                RoleEnum.Seer => new Seer(shifter),
                RoleEnum.Chameleon => new Chameleon(shifter),
                RoleEnum.Retributionist => new Retributionist(shifter)
                {
                    TrackerArrows = ((Retributionist)role).TrackerArrows,
                    Inspected = ((Retributionist)role).Inspected,
                    Interrogated = ((Retributionist)role).Interrogated
                },
                _ => new Shifter(shifter),
            };

            newRole.RoleUpdate(shifterRole);
            Role newRole2;

            if (CustomGameOptions.ShiftedBecomes == BecomeEnum.Shifter)
                newRole2 = new Shifter(other);
            else
                newRole2 = new Crewmate(other);

            newRole2.RoleUpdate(role);
        }
    }
}
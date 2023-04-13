using System;
using HarmonyLib;
using Hazel;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Data;
using TownOfUsReworked.Modules;

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

            if (PlayerControl.LocalPlayer == other)
            {
                Utils.Flash(shifterRole.Color, "Someone has stolen your role!");
                role.OnLobby();
            }

            if (PlayerControl.LocalPlayer == shifter)
            {
                Utils.Flash(shifterRole.Color, "You stole someone's role!");
                shifterRole.ShiftButton.gameObject.SetActive(false);
                shifterRole.OnLobby();
                CustomButtons.ResetCustomTimers(false);
            }

            Role newRole = role.RoleType switch
            {
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
                    Interrogated = ((Retributionist)role).Interrogated,
                    RevivedRole = ((Retributionist)role).RevivedRole
                },
                _ => new Shifter(shifter),
            };

            switch (role.RoleType)
            {
                case RoleEnum.Altruist:
                    ((Altruist)role).ReviveButton.gameObject.SetActive(false);
                    break;
                case RoleEnum.Coroner:
                    ((Coroner)role).CompareButton.gameObject.SetActive(false);
                    ((Coroner)role).AutopsyButton.gameObject.SetActive(false);
                    break;
                case RoleEnum.Detective:
                    ((Detective)role).ExamineButton.gameObject.SetActive(false);
                    break;
                case RoleEnum.Engineer:
                    ((Engineer)role).FixButton.gameObject.SetActive(false);
                    break;
                case RoleEnum.Escort:
                    ((Escort)role).BlockButton.gameObject.SetActive(false);
                    break;
                case RoleEnum.Inspector:
                    ((Inspector)role).InspectButton.gameObject.SetActive(false);
                    break;
                case RoleEnum.Sheriff:
                    ((Sheriff)role).InterrogateButton.gameObject.SetActive(false);
                    break;
                case RoleEnum.Medic:
                    ((Medic)role).ShieldButton.gameObject.SetActive(false);
                    break;
                case RoleEnum.Tracker:
                    ((Tracker)role).TrackButton.gameObject.SetActive(false);
                    break;
                case RoleEnum.Transporter:
                    ((Transporter)role).TransportButton.gameObject.SetActive(false);
                    break;
                case RoleEnum.Medium:
                    ((Medium)role).MediateButton.gameObject.SetActive(false);
                    break;
                case RoleEnum.Operative:
                    ((Operative)role).BugButton.gameObject.SetActive(false);
                    break;
                case RoleEnum.TimeLord:
                    ((TimeLord)role).RewindButton.gameObject.SetActive(false);
                    break;
                case RoleEnum.VampireHunter:
                    ((VampireHunter)role).StakeButton.gameObject.SetActive(false);
                    break;
                case RoleEnum.Veteran:
                    ((Veteran)role).AlertButton.gameObject.SetActive(false);
                    break;
                case RoleEnum.Vigilante:
                    ((Vigilante)role).ShootButton.gameObject.SetActive(false);
                    break;
                case RoleEnum.Mystic:
                    ((Mystic)role).RevealButton.gameObject.SetActive(false);
                    break;
                case RoleEnum.Seer:
                    ((Seer)role).SeerButton.gameObject.SetActive(false);
                    break;
                case RoleEnum.Chameleon:
                    ((Chameleon)role).SwoopButton.gameObject.SetActive(false);
                    break;
                case RoleEnum.Retributionist:
                    ((Retributionist)role).ReviveButton.gameObject.SetActive(false);
                    ((Retributionist)role).CompareButton.gameObject.SetActive(false);
                    ((Retributionist)role).AutopsyButton.gameObject.SetActive(false);
                    ((Retributionist)role).ExamineButton.gameObject.SetActive(false);
                    ((Retributionist)role).FixButton.gameObject.SetActive(false);
                    ((Retributionist)role).SwoopButton.gameObject.SetActive(false);
                    ((Retributionist)role).SeerButton.gameObject.SetActive(false);
                    ((Retributionist)role).RevealButton.gameObject.SetActive(false);
                    ((Retributionist)role).ShootButton.gameObject.SetActive(false);
                    ((Retributionist)role).AlertButton.gameObject.SetActive(false);
                    ((Retributionist)role).StakeButton.gameObject.SetActive(false);
                    ((Retributionist)role).BugButton.gameObject.SetActive(false);
                    ((Retributionist)role).MediateButton.gameObject.SetActive(false);
                    ((Retributionist)role).TransportButton.gameObject.SetActive(false);
                    ((Retributionist)role).TrackButton.gameObject.SetActive(false);
                    ((Retributionist)role).ShieldButton.gameObject.SetActive(false);
                    ((Retributionist)role).InterrogateButton.gameObject.SetActive(false);
                    ((Retributionist)role).InspectButton.gameObject.SetActive(false);
                    ((Retributionist)role).BlockButton.gameObject.SetActive(false);
                    break;
            }

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
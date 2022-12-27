using System;
using HarmonyLib;
using Hazel;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.InvestigatorMod;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.MedicMod;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.OperativeMod;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using TownOfUsReworked.PlayerLayers.Abilities;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Patches;
using System.Linq;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.DraculaMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformConvertButton
    {
        public static bool Prefix(KillButton __instance)
        {
            if (__instance != DestroyableSingleton<HudManager>.Instance.KillButton)
                return true;

            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Dracula);

            if (!flag)
                return true;

            var role = Role.GetRole<Dracula>(PlayerControl.LocalPlayer);

            if (!PlayerControl.LocalPlayer.CanMove)
                return false;

            if (PlayerControl.LocalPlayer.Data.IsDead)
                return false;

            var flag2 = role.ConvertTimer() == 0f;

            if (!flag2)
                return false;

            if (!__instance.enabled)
                return false;

            var maxDistance = GameOptionsData.KillDistances[CustomGameOptions.InteractionDistance];
            var distBetweenPlayers = Utils.GetDistBetweenPlayers(PlayerControl.LocalPlayer, role.ClosestPlayer);
            var flag3 = distBetweenPlayers < maxDistance;

            if (!flag3)
                return false;

            if (role.ClosestPlayer == null)
                return false;

            var playerId = role.ClosestPlayer.PlayerId;

            if (role.ClosestPlayer.IsInfected())
            {
                foreach (var pb in Role.GetRoles(RoleEnum.Plaguebearer))
                    ((Plaguebearer)pb).RpcSpreadInfection(role.ClosestPlayer, role.Player);
            }

            if (role.ClosestPlayer.IsOnAlert() | role.ClosestPlayer.Is(RoleEnum.Pestilence) | role.ClosestPlayer.Is(RoleEnum.VampireHunter))
            {
                if (role.ClosestPlayer.IsShielded())
                {
                    var medic = role.ClosestPlayer.GetMedic().Player.PlayerId;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.AttemptSound,
                        SendOption.Reliable, -1);
                    writer.Write(medic);
                    writer.Write(role.ClosestPlayer.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);

                    if (CustomGameOptions.ShieldBreaks)
                        role.LastBitten = DateTime.UtcNow;

                    StopKill.BreakShield(medic, role.ClosestPlayer.PlayerId, CustomGameOptions.ShieldBreaks);

                    if (!role.Player.IsProtected())
                        Utils.RpcMurderPlayer(role.ClosestPlayer, role.Player);
                }
                else if (role.Player.IsShielded())
                {
                    var medic = role.Player.GetMedic().Player.PlayerId;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.AttemptSound,
                        SendOption.Reliable, -1);
                    writer.Write(medic);
                    writer.Write(role.Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);

                    if (CustomGameOptions.ShieldBreaks)
                        role.LastBitten = DateTime.UtcNow;

                    StopKill.BreakShield(medic, role.Player.PlayerId, CustomGameOptions.ShieldBreaks);
                }
                else
                    Utils.RpcMurderPlayer(role.ClosestPlayer, role.Player);
                    
                return false;
            }
            else if (role.ClosestPlayer.IsShielded())
            {
                var medic = role.ClosestPlayer.GetMedic().Player.PlayerId;

                var writer1 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                    (byte) CustomRPC.AttemptSound, SendOption.Reliable, -1);
                writer1.Write(medic);
                writer1.Write(role.ClosestPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer1);

                if (CustomGameOptions.ShieldBreaks)
                    role.LastBitten = DateTime.UtcNow;

                StopKill.BreakShield(medic, role.ClosestPlayer.PlayerId, CustomGameOptions.ShieldBreaks);

                return false;
            }
            else if (role.ClosestPlayer.IsVesting())
            {
                role.LastBitten.AddSeconds(CustomGameOptions.VestKCReset);

                return false;
            }
            else if (role.ClosestPlayer.IsProtected())
            {
                role.LastBitten.AddSeconds(CustomGameOptions.ProtectKCReset);

                return false;
            }

            var vampCount = role.AliveVamps().Count();

            if (vampCount + 1 > CustomGameOptions.AliveVampCount)
            {
                Utils.RpcMurderPlayer(role.Player, role.ClosestPlayer);
                return false;
            }

            if (role.ClosestPlayer.Is(SubFaction.Undead))
                return false;

            unchecked
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Convert,
                    SendOption.Reliable, -1);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                writer.Write(playerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }

            Convert(role, role.ClosestPlayer);
            role.LastBitten = DateTime.UtcNow;

            return false;
        }

        public static void Convert(Dracula dracRole, PlayerControl other)
        {
            var role = Role.GetRole(other);
            var drac = dracRole.Player;
            var ability = Ability.GetAbility(other);

            var convert = false;
            var convertNeut = false;
            var convertNK = false;
            var convertCK = false;
            var alreadyVamp = false;

            switch (role.RoleType)
            {
                case RoleEnum.Vampire:
                case RoleEnum.Dampyr:
                case RoleEnum.Dracula:

                    alreadyVamp = true;

                    break;

                case RoleEnum.Sheriff:
                case RoleEnum.Engineer:
                case RoleEnum.Mayor:
                case RoleEnum.Swapper:
                case RoleEnum.Investigator:
                case RoleEnum.TimeLord:
                case RoleEnum.Medic:
                case RoleEnum.Agent:
                case RoleEnum.Altruist:
                case RoleEnum.Crewmate:
                case RoleEnum.Tracker:
                case RoleEnum.Transporter:
                case RoleEnum.Medium:
                case RoleEnum.Coroner:
                case RoleEnum.Operative:
                case RoleEnum.Detective:
                case RoleEnum.Shifter:
                case RoleEnum.Inspector:
                case RoleEnum.Escort:

                    convert = true;

                    break;

                case RoleEnum.Vigilante:
                case RoleEnum.Veteran:

                    convertCK = true;
                    convert = true;

                    break;

                case RoleEnum.Amnesiac:
                case RoleEnum.Survivor:
                case RoleEnum.Jester:
                case RoleEnum.Cannibal:
                case RoleEnum.Executioner:
                case RoleEnum.Jackal:
                case RoleEnum.Recruit:

                    convertNeut = true;

                    break;

                case RoleEnum.Cryomaniac:
                case RoleEnum.Thief:
                case RoleEnum.Glitch:
                case RoleEnum.Plaguebearer:
                case RoleEnum.Werewolf:
                case RoleEnum.Murderer:
                case RoleEnum.SerialKiller:
                case RoleEnum.Arsonist:

                    convertNK = true;
                    convertNeut = true;

                    break;
            }

            dracRole.Converted.Add(other);

            if (alreadyVamp)
                return;
                            
            Role.RoleDictionary.Remove(other.PlayerId);

            if (convertNeut == true && CustomGameOptions.DraculaConvertNeuts)
            {
                if (!convertNK)
                {
                    if (role.RoleType == RoleEnum.Amnesiac)
                    {
                        var amne = Role.GetRole<Amnesiac>(other);
                        amne.BodyArrows.Values.DestroyAll();
                        amne.BodyArrows.Clear();
                        amne.CurrentTarget.bodyRenderer.material.SetFloat("_Outline", 0f);
                    }
                    else if (role.RoleType == RoleEnum.Cannibal)
                    {
                        var can = Role.GetRole<Cannibal>(other);
                        can.BodyArrows.Values.DestroyAll();
                        can.BodyArrows.Clear();
                        can.CurrentTarget.bodyRenderer.material.SetFloat("_Outline", 0f);
                    }

                    var newRole = new Vampire(other);
                    newRole.Player = other;
                }
                else
                {
                    var newRole = new Dampyr(other);
                    newRole.Player = other;
                }
            }
            else if (convert == true)
            {
                if (!convertCK)
                {
                    if (role.RoleType == RoleEnum.Investigator)
                    {
                        var invRole = Role.GetRole<Investigator>(other);
                        Footprint.DestroyAll(invRole);
                    }
                    else if (role.RoleType == RoleEnum.Tracker)
                    {
                        var trackerRole = Role.GetRole<Tracker>(other);
                        trackerRole.TrackerArrows.Values.DestroyAll();
                        trackerRole.TrackerArrows.Clear();
                        trackerRole.UsesLeft = CustomGameOptions.MaxTracks;
                    }
                    else if (role.RoleType == RoleEnum.Coroner)
                    {
                        var coronerRole = Role.GetRole<Coroner>(other);
                        coronerRole.BodyArrows.Values.DestroyAll();
                        coronerRole.BodyArrows.Clear();
                        DestroyableSingleton<HudManager>.Instance.KillButton.gameObject.SetActive(false);
                    }
                    else if (role.RoleType == RoleEnum.Operative)
                    {
                        var opRole = Role.GetRole<Operative>(other);
                        opRole.UsesLeft = CustomGameOptions.MaxBugs;
                        opRole.buggedPlayers.Clear();
                        opRole.bugs.ClearBugs();
                    }
                    else if (role.RoleType == RoleEnum.Medic)
                    {
                        var medicRole = Role.GetRole<Medic>(other);
                        medicRole.ShieldedPlayer = null;
                    }

                    var newRole = new Vampire(other);
                    newRole.Player = other;
                }
                else
                {
                    var newRole = new Dampyr(other);
                    newRole.Player = other;
                }
            }
            else if (!other.Is(SubFaction.Undead))
                Utils.RpcMurderPlayer(drac, other);

            foreach (var role2 in Role.GetRoles(RoleEnum.Dampyr))
            {
                var dampyr = (Dampyr)role2;
                dampyr.LastKill = DateTime.UtcNow;

                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (player == PlayerControl.LocalPlayer)
                        dampyr.RegenTask();
                }
            }

            foreach (var role2 in Role.GetRoles(RoleEnum.Vampire))
            {
                var vampire = (Vampire)role2;

                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (player == PlayerControl.LocalPlayer)
                        vampire.RegenTask();
                }
            }
        }
    }
}

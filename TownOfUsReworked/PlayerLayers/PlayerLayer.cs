using System.Collections.Generic;
using System.Linq;
using Reactor.Utilities.Extensions;
using UnityEngine;
using HarmonyLib;
using TownOfUsReworked.Data;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Custom;
using Hazel;
using TownOfUsReworked.PlayerLayers.Objectifiers;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.Patches;
using System;
using TownOfUsReworked.PlayerLayers.Abilities;
using TownOfUsReworked.PlayerLayers.Modifiers;

namespace TownOfUsReworked.PlayerLayers
{
    [HarmonyPatch]
    public abstract class PlayerLayer
    {
        public Color32 Color = Colors.Layer;
        public string Name = "Layerless";

        public PlayerLayerEnum LayerType = PlayerLayerEnum.None;
        public RoleEnum RoleType = RoleEnum.None;
        public ModifierEnum ModifierType = ModifierEnum.None;
        public ObjectifierEnum ObjectifierType = ObjectifierEnum.None;
        public AbilityEnum AbilityType = AbilityEnum.None;
        public LayerEnum Type = LayerEnum.None;

        public string KilledBy = "";
        public DeathReasonEnum DeathReason = DeathReasonEnum.Alive;

        public bool IsBlocked;
        public bool RoleBlockImmune;

        public bool Winner;

        public bool IsDead => Player.Data.IsDead;
        public bool Disconnected => Player.Data.Disconnected;

        public readonly static List<PlayerLayer> AllLayers = new();

        public virtual void OnLobby() => EndGame.Reset();

        public virtual void UpdateHud(HudManager __instance)
        {
            if (ConstantVariables.Inactive || LobbyBehaviour.Instance || MeetingHud.Instance)
                return;

            __instance.KillButton.SetTarget(null);
            __instance.KillButton.gameObject.SetActive(false);

            var Vent = __instance.ImpostorVentButton.graphic.sprite;

            if (IsBlocked)
                Vent = AssetManager.GetSprite("Blocked");
            else if (Player.Is(Faction.Intruder))
                Vent = AssetManager.GetSprite("IntruderVent");
            else if (Player.Is(Faction.Syndicate))
                Vent = AssetManager.GetSprite("SyndicateVent");
            else if (Player.Is(Faction.Crew))
                Vent = AssetManager.GetSprite("CrewVent");
            else if (Player.Is(Faction.Neutral))
                Vent = AssetManager.GetSprite("NeutralVent");

            __instance.ImpostorVentButton.graphic.sprite = Vent;
            __instance.ImpostorVentButton.buttonLabelText.text = IsBlocked ? "BLOCKED" : "VENT";
            __instance.ImpostorVentButton.buttonLabelText.fontSharedMaterial = __instance.SabotageButton.buttonLabelText.fontSharedMaterial;
            __instance.ImpostorVentButton.gameObject.SetActive(Player.CanVent() || Player.inVent);

            var closestDead = Player.GetClosestDeadPlayer(CustomGameOptions.ReportDistance);

            if (closestDead == null || Player.CannotUse())
                __instance.ReportButton.SetDisabled();
            else
                __instance.ReportButton.SetEnabled();

            __instance.ReportButton.buttonLabelText.text = IsBlocked ? "BLOCKED" : "REPORT";
            __instance.ReportButton.graphic.sprite = IsBlocked ? AssetManager.GetSprite("Blocked") : AssetManager.GetSprite("Report");
            __instance.ReportButton.buttonLabelText.fontSharedMaterial = __instance.SabotageButton.buttonLabelText.fontSharedMaterial;

            __instance.UseButton.buttonLabelText.text = IsBlocked ? "BLOCKED" : "USE";
            __instance.UseButton.graphic.sprite = IsBlocked ? AssetManager.GetSprite("Blocked") : AssetManager.Use;
            __instance.UseButton.buttonLabelText.fontSharedMaterial = __instance.SabotageButton.buttonLabelText.fontSharedMaterial;

            __instance.PetButton.buttonLabelText.text = IsBlocked ? "BLOCKED" : "PET";
            __instance.PetButton.graphic.sprite = IsBlocked ? AssetManager.GetSprite("Blocked") : AssetManager.GetSprite("Pet");
            __instance.PetButton.buttonLabelText.fontSharedMaterial = __instance.SabotageButton.buttonLabelText.fontSharedMaterial;

            if (Player.CannotUse())
                __instance.SabotageButton.SetDisabled();
            else
                __instance.SabotageButton.SetEnabled();

            __instance.SabotageButton.graphic.sprite = IsBlocked ? AssetManager.GetSprite("Blocked") : (Player.Is(Faction.Syndicate) ? AssetManager.GetSprite("SyndicateSabotage") :
                AssetManager.GetSprite("Sabotage"));

            if (IsBlocked && Minigame.Instance)
                Minigame.Instance.Close();

            if (MapBehaviour.Instance && IsBlocked)
                MapBehaviour.Instance.Close();
        }

        public virtual void OnMeetingStart(MeetingHud __instance)
        {
            foreach (var player in PlayerControl.AllPlayerControls)
                player.RegenTask();

            EndGame.Reset();
        }

        public virtual void OnMeetingEnd(MeetingHud __instance)
        {
            foreach (var player in PlayerControl.AllPlayerControls)
                player.RegenTask();

            EndGame.Reset();
        }

        public virtual void OnBodyReport(GameData.PlayerInfo info) => EndGame.Reset();

        protected PlayerLayer(PlayerControl player)
        {
            Player = player;
            PlayerName = player.Data.PlayerName;
            AllLayers.Add(this);
        }

        public string PlayerName;
        public PlayerControl Player;

        public int TasksLeft => Player.Data.Tasks.ToArray().Count(x => !x.Complete);
        public int TasksCompleted => Player.Data.Tasks.ToArray().Count(x => x.Complete);
        public int TotalTasks => Player.Data.Tasks.ToArray().Length;
        public bool TasksDone => TasksLeft <= 0 || TasksCompleted >= TotalTasks;

        public string ColorString => $"<color=#{Color.ToHtmlStringRGBA()}>";

        public bool GameEnd()
        {
            if (Disconnected)
                return true;
            else if (IsDead)
            {
                if (Type == LayerEnum.Phantom && TasksDone)
                {
                    Role.PhantomWins = true;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                    writer.Write((byte)WinLoseRPC.PhantomWin);
                    writer.Write(Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Utils.EndGame();
                }

                return true;
            }
            else if (LayerType == PlayerLayerEnum.Objectifier)
            {
                if (Type == LayerEnum.Corrupted && ConstantVariables.CorruptedWin(Player))
                {
                    Objectifier.CorruptedWins = true;

                    if (CustomGameOptions.AllCorruptedWin)
                    {
                        foreach (var corr in Objectifier.GetObjectifiers<Corrupted>(ObjectifierEnum.Corrupted))
                            corr.Winner = true;
                    }

                    Winner = true;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                    writer.Write((byte)WinLoseRPC.CorruptedWin);
                    writer.Write(Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Utils.EndGame();
                    return true;
                }
                else if (Type == LayerEnum.Lovers && ConstantVariables.LoversWin(Player))
                {
                    Objectifier.LoveWins = true;
                    Winner = true;
                    Objectifier.GetObjectifier(((Lovers)this).OtherLover).Winner = true;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                    writer.Write((byte)WinLoseRPC.LoveWin);
                    writer.Write(Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Utils.EndGame();
                    return true;
                }
                else if (Type == LayerEnum.Rivals && ConstantVariables.RivalsWin(Player))
                {
                    Objectifier.RivalWins = true;
                    Winner = true;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                    writer.Write((byte)WinLoseRPC.RivalWin);
                    writer.Write(Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Utils.EndGame();
                    return true;
                }
                else if (Type == LayerEnum.Taskmaster && TasksDone)
                {
                    Objectifier.TaskmasterWins = true;
                    Winner = true;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                    writer.Write((byte)WinLoseRPC.TaskmasterWin);
                    writer.Write(Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Utils.EndGame();
                    return true;
                }
                else if (Type == LayerEnum.Mafia && ConstantVariables.MafiaWin)
                {
                    Objectifier.MafiaWins = true;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                    writer.Write((byte)WinLoseRPC.MafiaWins);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Utils.EndGame();
                    return true;
                }
                else if (Type == LayerEnum.Overlord && MeetingPatches.MeetingCount >= CustomGameOptions.OverlordMeetingWinCount && ((Overlord)this).IsAlive)
                {
                    Objectifier.OverlordWins = true;

                    foreach (var ov in Objectifier.GetObjectifiers<Overlord>(ObjectifierEnum.Overlord))
                    {
                        if (ov.IsAlive)
                            ov.Winner = true;
                    }

                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                    writer.Write((byte)WinLoseRPC.OverlordWin);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Utils.EndGame();
                    return true;
                }

                return !(Type is LayerEnum.Corrupted or LayerEnum.Allied or LayerEnum.Overlord or LayerEnum.Mafia || (ObjectifierType == ObjectifierEnum.Lovers &&
                    ((Lovers)this).LoversAlive()) || Player.IsWinningRival() || (ObjectifierType == ObjectifierEnum.Traitor && ((Traitor)this).Turned) || (ObjectifierType ==
                    ObjectifierEnum.Fanatic && ((Fanatic)this).Turned));
            }
            else if (LayerType == PlayerLayerEnum.Role)
            {
                var role = (Role)this;

                if ((role.IsRecruit || Type == LayerEnum.Jackal) && ConstantVariables.CabalWin)
                {
                    Role.CabalWin = true;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                    writer.Write((byte)WinLoseRPC.CabalWin);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Utils.EndGame();
                    return true;
                }
                else if ((role.IsPersuaded || Type == LayerEnum.Whisperer) && ConstantVariables.SectWin)
                {
                    Role.SectWin = true;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                    writer.Write((byte)WinLoseRPC.SectWin);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Utils.EndGame();
                    return true;
                }
                else if ((role.IsBitten || Type == LayerEnum.Dracula) && ConstantVariables.UndeadWin)
                {
                    Role.UndeadWin = true;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                    writer.Write((byte)WinLoseRPC.UndeadWin);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Utils.EndGame();
                    return true;
                }
                else if ((role.IsResurrected || Type == LayerEnum.Necromancer) && ConstantVariables.ReanimatedWin)
                {
                    Role.ReanimatedWin = true;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                    writer.Write((byte)WinLoseRPC.ReanimatedWin);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Utils.EndGame();
                    return true;
                }
                else if (role.Faction == Faction.Syndicate && (role.NotDefective || Type == LayerEnum.Betrayer || role.IsSynAlly || role.IsSynFanatic || role.IsSynTraitor) &&
                    ConstantVariables.SyndicateWins)
                {
                    Role.SyndicateWin = true;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                    writer.Write((byte)WinLoseRPC.SyndicateWin);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Utils.EndGame();
                    return true;
                }
                else if (role.Faction == Faction.Intruder && (role.NotDefective || Type == LayerEnum.Betrayer || role.IsIntAlly || role.IsIntFanatic || role.IsIntTraitor) &&
                    ConstantVariables.IntrudersWin)
                {
                    Role.IntruderWin = true;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                    writer.Write((byte)WinLoseRPC.IntruderWin);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Utils.EndGame();
                    return true;
                }
                else if (role.Faction == Faction.Crew && (role.NotDefective || role.IsCrewAlly) && ConstantVariables.CrewWins)
                {
                    Role.CrewWin = true;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                    writer.Write((byte)WinLoseRPC.CrewWin);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Utils.EndGame();
                    return true;
                }
                else if (role.NotDefective && ConstantVariables.PestOrPBWins && Type is LayerEnum.Plaguebearer or LayerEnum.Pestilence)
                {
                    Role.InfectorsWin = true;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                    writer.Write((byte)WinLoseRPC.InfectorsWin);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Utils.EndGame();
                    return true;
                }
                else if (ConstantVariables.AllNeutralsWin && role.NotDefective)
                {
                    Role.AllNeutralsWin = true;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                    writer.Write((byte)WinLoseRPC.AllNeutralsWin);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Utils.EndGame();
                    return true;
                }
                else if (ConstantVariables.AllNKsWin && role.NotDefective && role.RoleAlignment == RoleAlignment.NeutralKill)
                {
                    Role.NKWins = true;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                    writer.Write((byte)WinLoseRPC.AllNKsWin);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Utils.EndGame();
                    return true;
                }
                else if (role.NotDefective && role.RoleAlignment == RoleAlignment.NeutralKill && (ConstantVariables.SameNKWins(RoleType) ||
                    ConstantVariables.SoloNKWins(RoleType, Player)))
                {
                    switch (RoleType)
                    {
                        case RoleEnum.Glitch:
                            Role.GlitchWins = true;
                            break;

                        case RoleEnum.Arsonist:
                            Role.ArsonistWins = true;
                            break;

                        case RoleEnum.Cryomaniac:
                            Role.CryomaniacWins = true;
                            break;

                        case RoleEnum.Juggernaut:
                            Role.JuggernautWins = true;
                            break;

                        case RoleEnum.Murderer:
                            Role.MurdererWins = true;
                            break;

                        case RoleEnum.Werewolf:
                            Role.WerewolfWins = true;
                            break;

                        case RoleEnum.SerialKiller:
                            Role.SerialKillerWins = true;
                            break;
                    }

                    if (CustomGameOptions.NoSolo == NoSolo.SameNKs)
                    {
                        foreach (var role2 in Role.GetRoles(RoleType))
                        {
                            if (!role2.Disconnected && role2.NotDefective)
                                role2.Winner = true;
                        }
                    }

                    Winner = true;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                    writer.Write((byte)(CustomGameOptions.NoSolo == NoSolo.SameNKs ? WinLoseRPC.SameNKWins : WinLoseRPC.SoloNKWins));
                    writer.Write(Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Utils.EndGame();
                    return true;
                }

                return role.RoleAlignment != RoleAlignment.NeutralKill && role.RoleAlignment != RoleAlignment.NeutralNeo && (Type is not LayerEnum.Pestilence and not LayerEnum.Betrayer) &&
                    role.NotDefective && role.Faction is not Faction.Crew and not Faction.Intruder and not Faction.None and not Faction.Syndicate;
            }
            else
                return true;
        }

        public static bool operator ==(PlayerLayer a, PlayerLayer b)
        {
            if (a is null && b is null)
                return true;

            if (a is null || b is null)
                return false;

            return a.Type == b.Type && a.Player == b.Player;
        }

        public static bool operator !=(PlayerLayer a, PlayerLayer b) => !(a == b);

        private bool Equals(PlayerLayer other) => Equals(Player, other.Player) && Type == other.Type;

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            if (obj.GetType() != typeof(PlayerLayer))
                return false;

            return Equals((PlayerLayer)obj);
        }

        public override int GetHashCode() => HashCode.Combine(Player, (int)Type);

        public static void DeleteAll()
        {
            foreach (var layer in AllLayers)
            {
                layer.OnLobby();
                layer.Player = null;
            }

            Role.AllRoles.Clear();
            Objectifier.AllObjectifiers.Clear();
            Modifier.AllModifiers.Clear();
            Ability.AllAbilities.Clear();
        }

        public static List<PlayerLayer> GetLayers(PlayerControl player) => AllLayers.Where(x => x.Player == player).ToList();
    }
}
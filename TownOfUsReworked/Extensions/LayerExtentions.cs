using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using Reactor.Utilities.Extensions;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.PlayerLayers.Objectifiers;
using TownOfUsReworked.PlayerLayers.Modifiers;
using TownOfUsReworked.PlayerLayers.Abilities;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using UnityEngine;
using TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.SerialKillerMod;
using TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.UndertakerMod;
using TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.SyndicateMod;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.Extensions
{
    [HarmonyPatch]
    public static class LayerExtentions
    {
        public readonly static List<WinningPlayerData> PotentialWinners = new();
        public static string RoleColorString => $"<color=#{Colors.Role.ToHtmlStringRGBA()}>";
        public static string AlignmentColorString => $"<color=#{Colors.Alignment.ToHtmlStringRGBA()}>";
        public static string ObjectivesColorString => $"<color=#{Colors.Objectives.ToHtmlStringRGBA()}>";
        public static string AttributesColorString => $"<color=#{Colors.Attributes.ToHtmlStringRGBA()}>";
        public static string AbilitiesColorString => $"<color=#{Colors.Abilities.ToHtmlStringRGBA()}>";
        public static string ObjectifierColorString => $"<color=#{Colors.Objectifier.ToHtmlStringRGBA()}>";
        public static string ModifierColorString => $"<color=#{Colors.Modifier.ToHtmlStringRGBA()}>";
        public static string AbilityColorString => $"<color=#{Colors.Ability.ToHtmlStringRGBA()}>";

        public static bool Is(this PlayerControl player, RoleEnum roleType) => Role.GetRole(player)?.RoleType == roleType;

        public static bool Is(this Role role, RoleEnum roleType) => role?.RoleType == roleType;

        public static bool Is(this PlayerControl player, Role role) => Role.GetRole(player).Player == role.Player;

        public static bool Is(this PlayerControl player, SubFaction subFaction) => Role.GetRole(player)?.SubFaction == subFaction;

        public static bool Is(this PlayerControl player, ModifierEnum modifierType) => Modifier.GetModifier(player)?.ModifierType == modifierType;

        public static bool Is(this PlayerControl player, ObjectifierEnum abilityType) => Objectifier.GetObjectifier(player)?.ObjectifierType == abilityType;

        public static bool Is(this PlayerControl player, AbilityEnum ability) => Ability.GetAbility(player)?.AbilityType == ability;

        public static bool Is(this PlayerControl player, Faction faction) => Role.GetRole(player)?.Faction == faction;

        public static bool Is(this PlayerControl player, RoleAlignment alignment) => Role.GetRole(player)?.RoleAlignment == alignment;

        public static bool Is(this PlayerVoteArea player, RoleEnum roleType) => Utils.PlayerByVoteArea(player).Is(roleType);

        public static bool Is(this PlayerVoteArea player, Role role) => Utils.PlayerByVoteArea(player).Is(role);

        public static bool Is(this PlayerVoteArea player, SubFaction subFaction) => Utils.PlayerByVoteArea(player).Is(subFaction);

        public static bool Is(this PlayerVoteArea player, ModifierEnum modifierType) => Utils.PlayerByVoteArea(player).Is(modifierType);

        public static bool Is(this PlayerVoteArea player, ObjectifierEnum abilityType) => Utils.PlayerByVoteArea(player).Is(abilityType);

        public static bool Is(this PlayerVoteArea player, AbilityEnum ability) => Utils.PlayerByVoteArea(player).Is(ability);

        public static bool Is(this PlayerVoteArea player, Faction faction) => Utils.PlayerByVoteArea(player).Is(faction);

        public static bool Is(this PlayerVoteArea player, RoleAlignment alignment) => Utils.PlayerByVoteArea(player).Is(alignment);

        public static Faction GetFaction(this PlayerControl player)
        {
            if (player == null)
                return Faction.None;

            var role = Role.GetRole(player);

            if (role == null)
                return player.Data.IsImpostor() ? Faction.Intruder : Faction.Crew;

            return role.Faction;
        }

        public static SubFaction GetSubFaction(this PlayerControl player)
        {
            if (player == null)
                return SubFaction.None;

            var role = Role.GetRole(player);

            if (role == null)
                return SubFaction.None;

            return role.SubFaction;
        }

        public static Faction GetFaction(this PlayerVoteArea player) => Utils.PlayerByVoteArea(player).GetFaction();

        public static SubFaction GetSubFaction(this PlayerVoteArea player) => Utils.PlayerByVoteArea(player).GetSubFaction();

        public static bool IsRecruit(this PlayerControl player)
        {
            if (player == null)
                return false;

            var role = Role.GetRole(player);

            if (role == null)
                return false;

            return role.IsRecruit;
        }

        public static bool IsBitten(this PlayerControl player)
        {
            if (player == null)
                return false;

            var role = Role.GetRole(player);

            if (role == null)
                return false;

            return role.IsBitten;
        }

        public static bool IsPersuaded(this PlayerControl player)
        {
            if (player == null)
                return false;

            var role = Role.GetRole(player);

            if (role == null)
                return false;

            return role.IsPersuaded;
        }

        public static bool IsResurrected(this PlayerControl player)
        {
            if (player == null)
                return false;

            var role = Role.GetRole(player);

            if (role == null)
                return false;

            return role.IsResurrected;
        }

        public static bool IsRecruit(this PlayerVoteArea player) => Utils.PlayerByVoteArea(player).IsRecruit();

        public static bool IsBitten(this PlayerVoteArea player) => Utils.PlayerByVoteArea(player).IsBitten();

        public static bool IsPersuaded(this PlayerVoteArea player) => Utils.PlayerByVoteArea(player).IsPersuaded();

        public static bool IsResurrected(this PlayerVoteArea player) => Utils.PlayerByVoteArea(player).IsResurrected();

        public static bool NotOnTheSameSide(this PlayerControl player)
        {
            if (player == null)
                return false;

            var traitorFlag = player.IsTurnedTraitor();
            var fanaticFlag = player.IsTurnedFanatic();
            var recruitFlag = player.IsRecruit();
            var bittenFlag = player.IsBitten();
            var sectFlag = player.IsPersuaded();
            var revivedFlag = player.IsResurrected();
            var rivalFlag = player.IsWinningRival();
            var corruptedFlag = player.Is(ObjectifierEnum.Corrupted);
            var allyFlag = player.Is(ObjectifierEnum.Corrupted);
            var loverFlag = player.Is(ObjectifierEnum.Lovers);
            return traitorFlag || recruitFlag || sectFlag || revivedFlag || rivalFlag || fanaticFlag || corruptedFlag || bittenFlag || allyFlag || loverFlag;
        }

        public static bool IsBase(this PlayerControl player)
        {
            if (player == null)
                return false;

            var role = Role.GetRole(player);

            if (role == null)
                return true;

            return role.Base;
        }

        public static bool IsGATarget(this PlayerControl player)
        {
            if (player == null)
                return false;

            foreach (var ga in Role.GetRoles(RoleEnum.GuardianAngel).Cast<GuardianAngel>())
            {
                if (ga.TargetPlayer == null)
                    continue;

                if (player.PlayerId == ga.TargetPlayer.PlayerId)
                    return true;
            }

            return false;
        }

        public static bool IsExeTarget(this PlayerControl player)
        {
            if (player == null)
                return false;

            foreach (var exe in Role.GetRoles(RoleEnum.Executioner).Cast<Executioner>())
            {
                if (exe.TargetPlayer == null)
                    continue;

                if (player.PlayerId == exe.TargetPlayer.PlayerId)
                    return true;
            }

            return false;
        }

        public static bool IsBHTarget(this PlayerControl player)
        {
            if (player == null)
                return false;

            foreach (var bh in Role.GetRoles(RoleEnum.BountyHunter).Cast<BountyHunter>())
            {
                if (bh.TargetPlayer == null)
                    continue;

                if (player.PlayerId == bh.TargetPlayer.PlayerId)
                    return true;
            }

            return false;
        }

        public static bool IsGuessTarget(this PlayerControl player)
        {
            if (player == null)
                return false;

            foreach (var guess in Role.GetRoles(RoleEnum.Guesser).Cast<Guesser>())
            {
                if (guess.TargetPlayer == null)
                    continue;

                if (player.PlayerId == guess.TargetPlayer.PlayerId)
                    return true;
            }

            return false;
        }

        public static PlayerControl GetTarget(this PlayerControl player)
        {
            var role = Role.GetRole(player);

            if (!role.HasTarget())
                return null;

            if (player.Is(RoleEnum.Executioner))
                return ((Executioner)role).TargetPlayer;
            else if (player.Is(RoleEnum.GuardianAngel))
                return ((GuardianAngel)role).TargetPlayer;
            else if (player.Is(RoleEnum.Guesser))
                return ((Guesser)role).TargetPlayer;
            else if (player.Is(RoleEnum.BountyHunter))
                return ((BountyHunter)role).TargetPlayer;

            return null;
        }

        public static Role GetLeader(this PlayerControl player)
        {
            if (!player.Is(RoleEnum.Mafioso) && !player.Is(RoleEnum.Sidekick))
                return null;

            var role = Role.GetRole(player);

            if (role == null)
                return null;

            if (player.Is(RoleEnum.Mafioso))
                return ((Mafioso)role).Godfather;
            else if (player.Is(RoleEnum.Sidekick))
                return ((Sidekick)role).Rebel;

            return null;
        }

        public static InspectorResults GetActorList(this PlayerControl player)
        {
            if (!player.Is(RoleEnum.Actor))
                return InspectorResults.None;

            var role = Role.GetRole(player);

            if (role == null)
                return InspectorResults.None;

            if (player.Is(RoleEnum.Actor))
                return ((Actor)role).PretendRoles;

            return InspectorResults.None;
        }

        public static bool IsGATarget(this PlayerVoteArea player) => Utils.PlayerByVoteArea(player).IsGATarget();

        public static bool IsExeTarget(this PlayerVoteArea player) => Utils.PlayerByVoteArea(player).IsExeTarget();

        public static bool IsBHTarget(this PlayerVoteArea player) => Utils.PlayerByVoteArea(player).IsBHTarget();

        public static bool IsGuessTarget(this PlayerVoteArea player) => Utils.PlayerByVoteArea(player).IsGuessTarget();

        public static bool IsTarget(this PlayerControl player) => player.IsBHTarget() || player.IsGuessTarget() || player.IsGATarget() || player.IsExeTarget();

        public static bool IsTarget(this PlayerVoteArea player) => player.IsBHTarget() || player.IsGuessTarget() || player.IsGATarget() || player.IsExeTarget();

        public static bool CanDoTasks(this PlayerControl player)
        {
            if (player == null)
                return false;

            if (Role.GetRole(player) == null)
                return !player.Data.IsImpostor();

            var crewflag = player.Is(Faction.Crew);
            var neutralflag = player.Is(Faction.Neutral);
            var intruderflag = player.Is(Faction.Intruder);
            var syndicateflag = player.Is(Faction.Syndicate);

            var phantomflag = player.Is(RoleEnum.Phantom);

            var NotOnTheSameSide = player.NotOnTheSameSide();
            var taskmasterflag = player.Is(ObjectifierEnum.Taskmaster);

            var isdead = player.Data.IsDead;

            var flag1 = crewflag && !NotOnTheSameSide;
            var flag2 = neutralflag && (taskmasterflag || (phantomflag && isdead));
            var flag3 = intruderflag && taskmasterflag;
            var flag4 = syndicateflag && taskmasterflag;
            return flag1 || flag2 || flag3 || flag4;
        }

        public static bool CanDoTasks(this PlayerVoteArea player) => Utils.PlayerByVoteArea(player).CanDoTasks();

        public static Jackal GetJackal(this PlayerControl player)
        {
            if (player == null)
                return null;

            if (!player.IsRecruit())
                return null;

            return Role.GetRoles(RoleEnum.Jackal).Find(role => ((Jackal)role).Recruited.Contains(player.PlayerId)) as Jackal;
        }

        public static Necromancer GetNecromancer(this PlayerControl player)
        {
            if (player == null)
                return null;

            if (!player.IsResurrected())
                return null;

            return Role.GetRoles(RoleEnum.Necromancer).Find(role => ((Necromancer)role).Resurrected.Contains(player.PlayerId)) as Necromancer;
        }

        public static Dracula GetDracula(this PlayerControl player)
        {
            if (player == null)
                return null;

            if (!player.Is(SubFaction.Undead))
                return null;

            return Role.GetRoles(RoleEnum.Dracula).Find(role => ((Dracula)role).Converted.Contains(player.PlayerId)) as Dracula;
        }

        public static Whisperer GetWhisperer(this PlayerControl player)
        {
            if (player == null)
                return null;

            if (!player.IsPersuaded())
                return null;

            return Role.GetRoles(RoleEnum.Whisperer).Find(role => ((Whisperer)role).Persuaded.Contains(player.PlayerId)) as Whisperer;
        }

        public static Jackal GetJackal(this PlayerVoteArea player) => Utils.PlayerByVoteArea(player).GetJackal();

        public static Necromancer GetNecromancer(this PlayerVoteArea player) => Utils.PlayerByVoteArea(player).GetNecromancer();

        public static Dracula GetDracula(this PlayerVoteArea player) => Utils.PlayerByVoteArea(player).GetDracula();

        public static Whisperer GetWhisperer(this PlayerVoteArea player) => Utils.PlayerByVoteArea(player).GetWhisperer();

        public static bool IsShielded(this PlayerControl player)
        {
            return Role.GetRoles(RoleEnum.Medic).Any(role =>
            {
                var shieldedPlayer = ((Medic)role).ShieldedPlayer;
                return shieldedPlayer != null && player.PlayerId == shieldedPlayer.PlayerId;
            });
        }

        public static bool IsRetShielded(this PlayerControl player)
        {
            return Role.GetRoles(RoleEnum.Retributionist).Any(role =>
            {
                var shieldedPlayer = ((Retributionist)role).ShieldedPlayer;
                return shieldedPlayer != null && player.PlayerId == shieldedPlayer.PlayerId && ((Retributionist)role).RevivedRole?.RoleType == RoleEnum.Medic;
            });
        }

        public static bool IsShielded(this PlayerVoteArea player) => Utils.PlayerByVoteArea(player).IsShielded();

        public static bool IsRetShielded(this PlayerVoteArea player) => Utils.PlayerByVoteArea(player).IsRetShielded();

        public static Medic GetMedic(this PlayerControl player)
        {
            return Role.GetRoles(RoleEnum.Medic).Find(role =>
            {
                var shieldedPlayer = ((Medic)role).ShieldedPlayer;
                return shieldedPlayer != null && player == shieldedPlayer;
            }) as Medic;
        }

        public static Retributionist GetRetMedic(this PlayerControl player)
        {
            return Role.GetRoles(RoleEnum.Retributionist).Find(role =>
            {
                var shieldedPlayer = ((Retributionist)role).ShieldedPlayer;
                return shieldedPlayer != null && player == shieldedPlayer && ((Retributionist)role).RevivedRole?.RoleType == RoleEnum.Medic;
            }) as Retributionist;
        }

        public static Crusader GetCrusader(this PlayerControl player)
        {
            return Role.GetRoles(RoleEnum.Crusader).Find(role =>
            {
                var crusaded = ((Crusader)role).CrusadedPlayer;
                return crusaded != null && player == crusaded;
            }) as Crusader;
        }

        public static bool IsOnAlert(this PlayerControl player)
        {
            var vetFlag = Role.GetRoles(RoleEnum.Veteran).Any(role =>
            {
                var veteran = (Veteran)role;
                return veteran?.OnAlert == true && player == veteran.Player;
            });

            var retFlag = Role.GetRoles(RoleEnum.Retributionist).Any(role =>
            {
                var retributionist = (Retributionist)role;
                return retributionist?.OnAlert == true && player == retributionist.Player;
            });

            return vetFlag || retFlag;
        }

        public static bool IsVesting(this PlayerControl player)
        {
            return Role.GetRoles(RoleEnum.Survivor).Any(role =>
            {
                var surv = (Survivor)role;
                return surv?.Vesting == true && player == surv.Player;
            });
        }

        public static bool IsMarked(this PlayerControl player)
        {
            return Role.GetRoles(RoleEnum.Ghoul).Any(role =>
            {
                var ghoul = (Ghoul)role;
                return ghoul?.MarkedPlayer != null && player == ghoul.MarkedPlayer;
            });
        }

        public static bool IsAmbushed(this PlayerControl player)
        {
            return Role.GetRoles(RoleEnum.Ambusher).Any(role =>
            {
                var amb = (Ambusher)role;
                return amb?.OnAmbush == true && player == amb.AmbushedPlayer;
            });
        }

        public static bool IsCrusaded(this PlayerControl player)
        {
            return Role.GetRoles(RoleEnum.Crusader).Any(role =>
            {
                var crus = (Crusader)role;
                return crus?.OnCrusade == true && player == crus.CrusadedPlayer;
            });
        }

        public static bool IsProtected(this PlayerControl player)
        {
            return Role.GetRoles(RoleEnum.GuardianAngel).Any(role =>
            {
                var gaTarget = ((GuardianAngel)role).TargetPlayer;
                var ga = (GuardianAngel)role;
                return gaTarget != null && ga.Protecting && player == gaTarget;
            });
        }

        public static bool IsInfected(this PlayerControl player)
        {
            return Role.GetRoles(RoleEnum.Plaguebearer).Any(role =>
            {
                var plaguebearer = (Plaguebearer)role;
                return plaguebearer != null && (plaguebearer.InfectedPlayers.Contains(player.PlayerId) || player == plaguebearer.Player);
            });
        }

        public static bool IsFramed(this PlayerControl player)
        {
            return Role.GetRoles(RoleEnum.Framer).Any(role =>
            {
                var framer = (Framer)role;
                return framer?.Framed.Contains(player.PlayerId) == true;
            });
        }

        public static bool IsInfected(this PlayerVoteArea player) => Utils.PlayerByVoteArea(player).IsInfected();

        public static bool IsFramed(this PlayerVoteArea player) => Utils.PlayerByVoteArea(player).IsFramed();

        public static bool IsMarked(this PlayerVoteArea player) => Utils.PlayerByVoteArea(player).IsMarked();

        public static bool IsWinningRival(this PlayerControl player)
        {
            if (player == null || player.Data == null)
                return false;

            if (!player.Is(ObjectifierEnum.Rivals))
                return false;

            var rival = Objectifier.GetObjectifier<Rivals>(player);

            return rival.RivalDead();
        }

        public static bool IsTurnedTraitor(this PlayerControl player) => player.IsIntTraitor() || player.IsSynTraitor();

        public static bool IsTurnedFanatic(this PlayerControl player) => player.IsIntFanatic() || player.IsSynFanatic();

        public static bool IsTurnedTraitor(this PlayerVoteArea player) => Utils.PlayerByVoteArea(player).IsTurnedTraitor();

        public static bool IsTurnedFanatic(this PlayerVoteArea player) => Utils.PlayerByVoteArea(player).IsTurnedFanatic();

        public static bool IsUnturnedFanatic(this PlayerControl player)
        {
            if (!player.Is(ObjectifierEnum.Fanatic))
                return false;

            var fanatic = Objectifier.GetObjectifier<Fanatic>(player);
            return !fanatic.Turned;
        }

        public static bool IsIntFanatic(this PlayerControl player)
        {
            if (!player.Is(ObjectifierEnum.Fanatic))
                return false;

            var traitor = Role.GetRole(player);
            return traitor.IsIntFanatic;
        }

        public static bool IsSynFanatic(this PlayerControl player)
        {
            if (!player.Is(ObjectifierEnum.Fanatic))
                return false;

            var traitor = Role.GetRole(player);
            return traitor.IsSynFanatic;
        }

        public static bool IsIntTraitor(this PlayerControl player)
        {
            if (!player.Is(ObjectifierEnum.Traitor))
                return false;

            var traitor = Role.GetRole(player);
            return traitor.IsIntTraitor;
        }

        public static bool IsSynTraitor(this PlayerControl player)
        {
            if (!player.Is(ObjectifierEnum.Traitor))
                return false;

            var traitor = Role.GetRole(player);
            return traitor.IsSynTraitor;
        }

        public static bool IsCrewAlly(this PlayerControl player)
        {
            if (!player.Is(ObjectifierEnum.Allied))
                return false;

            var traitor = Role.GetRole(player);
            return traitor.IsCrewAlly;
        }

        public static bool IsSynAlly(this PlayerControl player)
        {
            if (!player.Is(ObjectifierEnum.Allied))
                return false;

            var traitor = Role.GetRole(player);
            return traitor.IsSynAlly;
        }

        public static bool IsIntAlly(this PlayerControl player)
        {
            if (!player.Is(ObjectifierEnum.Allied))
                return false;

            var traitor = Role.GetRole(player);
            return traitor.IsIntAlly;
        }

        public static bool IsOtherRival(this PlayerControl player, PlayerControl refPlayer)
        {
            if (!player.Is(ObjectifierEnum.Rivals) || !refPlayer.Is(ObjectifierEnum.Rivals))
                return false;

            var rival1 = Objectifier.GetObjectifier<Rivals>(player);
            var rival2 = Objectifier.GetObjectifier<Rivals>(refPlayer);
            return rival1.OtherRival == refPlayer && rival2.OtherRival == player;
        }

        public static bool IsOtherLover(this PlayerControl player, PlayerControl refPlayer)
        {
            if (!player.Is(ObjectifierEnum.Lovers) || !refPlayer.Is(ObjectifierEnum.Lovers))
                return false;

            var lover1 = Objectifier.GetObjectifier<Lovers>(player);
            var lover2 = Objectifier.GetObjectifier<Lovers>(refPlayer);
            return lover1.OtherLover == refPlayer && lover2.OtherLover == player;
        }

        public static bool SyndicateSided(this PlayerControl player) => player.IsSynTraitor() || player.IsSynFanatic() || player.IsSynAlly();

        public static bool IntruderSided(this PlayerControl player) => player.IsIntTraitor() || player.IsIntAlly() || player.IsIntFanatic();

        public static void EndGame(GameOverReason reason = GameOverReason.ImpostorByVote, bool showAds = false) => GameManager.Instance.RpcEndGame(reason, showAds);

        public static bool LastImp() => PlayerControl.AllPlayerControls.ToArray().Count(x => x.Is(Faction.Intruder) && !(x.Data.IsDead || x.Data.Disconnected)) == 1;

        public static bool LastSyn() => PlayerControl.AllPlayerControls.ToArray().Count(x => x.Is(Faction.Syndicate) && !(x.Data.IsDead || x.Data.Disconnected)) == 1;

        public static bool Last(PlayerControl player) => (LastImp() && player.Is(Faction.Intruder)) || (LastSyn() && player.Is(Faction.Syndicate));

        public static bool TasksDone()
        {
            var allCrew = new List<PlayerControl>();
            var crewWithNoTasks = new List<PlayerControl>();

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player.CanDoTasks() && player.Is(Faction.Crew))
                {
                    allCrew.Add(player);

                    if (Role.GetRole(player).TasksDone)
                        crewWithNoTasks.Add(player);
                }
            }

            return allCrew.Count == crewWithNoTasks.Count;
        }

        public static bool Sabotaged()
        {
            if (ShipStatus.Instance.Systems != null)
            {
                if (ShipStatus.Instance.Systems.ContainsKey(SystemTypes.LifeSupp))
                {
                    var lifeSuppSystemType = ShipStatus.Instance.Systems[SystemTypes.LifeSupp].Cast<LifeSuppSystemType>();

                    if (lifeSuppSystemType.Countdown < 0f)
                        return true;
                }

                if (ShipStatus.Instance.Systems.ContainsKey(SystemTypes.Laboratory))
                {
                    var reactorSystemType = ShipStatus.Instance.Systems[SystemTypes.Laboratory].Cast<ReactorSystemType>();

                    if (reactorSystemType.Countdown < 0f)
                        return true;
                }

                if (ShipStatus.Instance.Systems.ContainsKey(SystemTypes.Reactor))
                {
                    var reactorSystemType = ShipStatus.Instance.Systems[SystemTypes.Reactor].Cast<ICriticalSabotage>();

                    if (reactorSystemType.Countdown < 0f)
                        return true;
                }
            }

            return false;
        }

        public static bool CanVent(this PlayerControl player, GameData.PlayerInfo playerInfo)
        {
            if (ConstantVariables.IsHnS)
                return !playerInfo.IsImpostor();

            if (player == null || playerInfo == null || (playerInfo.IsDead && !(player.Is(RoleEnum.Revealer) || player.Is(RoleEnum.Phantom) || player.Is(RoleEnum.Ghoul) ||
                player.Is(RoleEnum.Banshee))) || playerInfo.Disconnected || CustomGameOptions.WhoCanVent == WhoCanVentOptions.Noone || !ConstantVariables.IsRoaming || ConstantVariables.IsLobby ||
                player.inMovingPlat || ConstantVariables.IsMeeting)
            {
                return false;
            }
            else if (player.inVent || CustomGameOptions.WhoCanVent == WhoCanVentOptions.Everyone)
                return true;

            var playerRole = Role.GetRole(player);

            bool mainflag;

            if (playerRole == null)
                mainflag = playerInfo.IsImpostor();
            else if (playerRole.IsBlocked)
                mainflag = false;
            else if (player.IsRecruit())
                mainflag = CustomGameOptions.RecruitVent;
            else if (player.IsResurrected())
                mainflag = CustomGameOptions.ResurrectVent;
            else if (player.IsPersuaded())
                mainflag = CustomGameOptions.PersuadedVent;
            else if (player.IsBitten())
                mainflag = CustomGameOptions.UndeadVent;
            else if (player.Is(Faction.Syndicate))
            {
                mainflag = (Role.SyndicateHasChaosDrive && CustomGameOptions.SyndicateVent == SyndicateVentOptions.ChaosDrive) || CustomGameOptions.SyndicateVent ==
                    SyndicateVentOptions.Always;
            }
            else if (player.Is(Faction.Intruder))
            {
                if (CustomGameOptions.IntrudersVent)
                {
                    var flag = (player.Is(RoleEnum.Morphling) && CustomGameOptions.MorphlingVent) || (player.Is(RoleEnum.Wraith) && CustomGameOptions.WraithVent) ||
                        (player.Is(RoleEnum.Grenadier) && CustomGameOptions.GrenadierVent) || (player.Is(RoleEnum.Teleporter) && CustomGameOptions.TeleVent);

                    if (flag)
                        mainflag = true;
                    else if (player.Is(RoleEnum.Undertaker))
                    {
                        var undertaker = (Undertaker)playerRole;

                        mainflag = CustomGameOptions.UndertakerVentOptions == UndertakerOptions.Always || (undertaker.CurrentlyDragging != null &&
                            CustomGameOptions.UndertakerVentOptions == UndertakerOptions.Body) || (undertaker.CurrentlyDragging == null &&
                            CustomGameOptions.UndertakerVentOptions == UndertakerOptions.Bodyless);
                    }
                    else
                        mainflag = true;
                }
                else
                    mainflag = false;
            }
            else if (player.Is(Faction.Crew) && !player.Is(RoleEnum.Revealer))
            {
                if (player.Is(AbilityEnum.Tunneler) && !player.Is(RoleEnum.Engineer))
                {
                    var tunneler = Role.GetRole(player);
                    mainflag = tunneler.TasksDone;
                }
                else if (player.Is(RoleEnum.Engineer))
                    mainflag = true;
                else if (CustomGameOptions.CrewVent)
                    mainflag = true;
                else
                    mainflag = false;
            }
            else if (player.Is(Faction.Neutral))
            {
                var flag = (player.Is(RoleEnum.Murderer) && CustomGameOptions.MurdVent) || (player.Is(RoleEnum.Glitch) && CustomGameOptions.GlitchVent) ||
                    (player.Is(RoleEnum.Juggernaut) && CustomGameOptions.JuggVent) || (player.Is(RoleEnum.Pestilence) && CustomGameOptions.PestVent) ||
                    (player.Is(RoleEnum.Jester) && CustomGameOptions.JesterVent) || (player.Is(RoleEnum.Plaguebearer) && CustomGameOptions.PBVent) ||
                    (player.Is(RoleEnum.Arsonist) && CustomGameOptions.ArsoVent) || (player.Is(RoleEnum.Executioner) && CustomGameOptions.ExeVent) ||
                    (player.Is(RoleEnum.Cannibal) && CustomGameOptions.CannibalVent) || (player.Is(RoleEnum.Dracula) && CustomGameOptions.DracVent) ||
                    (player.Is(RoleEnum.Survivor) && CustomGameOptions.SurvVent) || (player.Is(RoleEnum.Actor) && CustomGameOptions.ActorVent) ||
                    (player.Is(RoleEnum.GuardianAngel) && CustomGameOptions.GAVent) || (player.Is(RoleEnum.Amnesiac) && CustomGameOptions.AmneVent) ||
                    (player.Is(RoleEnum.Werewolf) && CustomGameOptions.WerewolfVent) || (player.Is(RoleEnum.Jackal) && CustomGameOptions.JackalVent) ||
                    (player.Is(RoleEnum.BountyHunter) && CustomGameOptions.BHVent);

                if (flag)
                    mainflag = flag;
                else if (player.Is(RoleEnum.SerialKiller))
                {
                    var role2 = (SerialKiller)playerRole;

                    if (CustomGameOptions.SKVentOptions == SKVentOptions.Always)
                        mainflag = true;
                    else if (role2.Lusted && CustomGameOptions.SKVentOptions == SKVentOptions.Bloodlust)
                        mainflag = true;
                    else if (!role2.Lusted && CustomGameOptions.SKVentOptions == SKVentOptions.NoLust)
                        mainflag = true;
                    else
                        mainflag = false;
                }
                else
                    mainflag = false;
            }
            else if ((player.Is(RoleEnum.Revealer) || player.Is(RoleEnum.Phantom) || player.Is(RoleEnum.Banshee) || player.Is(RoleEnum.Ghoul)) && player.inVent)
                mainflag = true;
            else
                mainflag = false;

            return mainflag;
        }

        public static InspectorResults GetInspResults(this PlayerControl player)
        {
            if (player == null)
                return InspectorResults.None;

            var role = Role.GetRole(player);

            if (role == null)
                return InspectorResults.None;

            return role.InspectorResults;
        }

        public static InspectorResults GetInspResults(this PlayerVoteArea player) => Utils.PlayerByVoteArea(player).GetInspResults();

        public static bool IsBlocked(this PlayerControl player)
        {
            var role = Role.GetRole(player);

            if (role == null)
                return false;

            return role.IsBlocked;
        }

        public static bool SeemsEvil(this PlayerControl player)
        {
            var intruderFlag = player.Is(Faction.Intruder) && !player.Is(ObjectifierEnum.Traitor) && !player.Is(ObjectifierEnum.Fanatic) && !player.Is(RoleEnum.Godfather);
            var syndicateFlag = player.Is(Faction.Syndicate) && !player.Is(ObjectifierEnum.Traitor) && !player.Is(ObjectifierEnum.Fanatic) && !player.Is(RoleEnum.Rebel);
            var traitorFlag = player.IsTurnedTraitor() && CustomGameOptions.TraitorColourSwap;
            var fanaticFlag = player.IsTurnedFanatic() && CustomGameOptions.FanaticColourSwap;
            var nkFlag = player.Is(RoleAlignment.NeutralKill) && !CustomGameOptions.NeutKillingRed;
            var neFlag = player.Is(RoleAlignment.NeutralEvil) && !CustomGameOptions.NeutEvilRed;
            var framedFlag = player.IsFramed();
            return intruderFlag || syndicateFlag || traitorFlag || nkFlag || neFlag || framedFlag || fanaticFlag;
        }

        public static bool SeemsGood(this PlayerControl player) => !SeemsEvil(player);

        public static bool SeemsEvil(this PlayerVoteArea player) => Utils.PlayerByVoteArea(player).SeemsEvil();

        public static bool SeemsGood(this PlayerVoteArea player) => Utils.PlayerByVoteArea(player).SeemsGood();

        public static bool IsBlockImmune(PlayerControl player) => (bool)Role.GetRole(player)?.RoleBlockImmune;

        public static void Spread(PlayerControl interacter, PlayerControl target)
        {
            if (interacter.IsInfected() || target.IsInfected() || target.Is(RoleEnum.Plaguebearer))
            {
                foreach (var pb in Role.GetRoles(RoleEnum.Plaguebearer))
                    ((Plaguebearer)pb).RpcSpreadInfection(interacter, target);
            }

            if (target.Is(RoleEnum.Arsonist))
            {
                foreach (var arso in Role.GetRoles(RoleEnum.Arsonist))
                    ((Arsonist)arso).RpcSpreadDouse(target, interacter);
            }

            if (target.Is(RoleEnum.Cryomaniac))
            {
                foreach (var cryo in Role.GetRoles(RoleEnum.Cryomaniac))
                    ((Cryomaniac)cryo).RpcSpreadDouse(target, interacter);
            }
        }

        public static bool HasTarget(this Role role) => role.RoleType == RoleEnum.Executioner || role.RoleType == RoleEnum.GuardianAngel || role.RoleType == RoleEnum.Guesser ||
            role.RoleType == RoleEnum.BountyHunter;

        public static List<object> AllPlayerInfo(this PlayerControl player)
        {
            var role = Role.GetRole(player);
            var modifier = Modifier.GetModifier(player);
            var ability = Ability.GetAbility(player);
            var objectifier = Objectifier.GetObjectifier(player);

            return new List<object>
            {
                role, //0
                modifier, //1
                ability, //2
                objectifier //3
            };
        }

        public static List<object> AllPlayerInfo(this PlayerVoteArea player) => Utils.PlayerByVoteArea(player).AllPlayerInfo();

        public static PlayerControl GetOtherLover(this PlayerControl player)
        {
            if (!player.Is(ObjectifierEnum.Lovers))
                return null;

            return Objectifier.GetObjectifier<Lovers>(player).OtherLover;
        }

        public static PlayerControl GetOtherRival(this PlayerControl player)
        {
            if (!player.Is(ObjectifierEnum.Rivals))
                return null;

            return Objectifier.GetObjectifier<Rivals>(player).OtherRival;
        }

        public static bool NeutralHasUnfinishedBusiness(PlayerControl player)
        {
            if (player.Is(RoleEnum.GuardianAngel))
            {
                var ga = Role.GetRole<GuardianAngel>(player);
                return ga.TargetAlive;
            }
            else if (player.Is(RoleEnum.Executioner))
            {
                var exe = Role.GetRole<Executioner>(player);
                return exe.TargetVotedOut;
            }
            else if (player.Is(RoleEnum.Jester))
            {
                var jest = Role.GetRole<Jester>(player);
                return jest.VotedOut;
            }
            else if (player.Is(RoleEnum.Guesser))
            {
                var guess = Role.GetRole<Guesser>(player);
                return guess.TargetGuessed;
            }
            else if (player.Is(RoleEnum.BountyHunter))
            {
                var bh = Role.GetRole<BountyHunter>(player);
                return bh.TargetKilled;
            }
            else if (player.Is(RoleEnum.Actor))
            {
                var act = Role.GetRole<Actor>(player);
                return act.Guessed;
            }
            else if (player.Is(RoleEnum.Troll))
            {
                var troll = Role.GetRole<Troll>(player);
                return troll.Killed;
            }

            return false;
        }

        public static string GetTaskList(this PlayerControl player)
        {
            var info = player.AllPlayerInfo();

            var role = info[0] as Role;
            var modifier = info[1] as Modifier;
            var ability = info[2] as Ability;
            var objectifier = info[3] as Objectifier;

            string objectives = $"{ObjectivesColorString}Objectives:";
            string abilities = $"{AbilitiesColorString}Abilities:";
            string attributes = $"{AttributesColorString}Attributes:";
            string roleName = $"{RoleColorString}Role: ";
            string objectifierName = $"{ObjectifierColorString}Objectifier: ";
            string abilityName = $"{AbilityColorString}Ability: ";
            string modifierName = $"{ModifierColorString}Modifier: ";
            string alignment = $"{AlignmentColorString}Alignment: ";

            if (info[0] != null)
            {
                roleName += $"{role.ColorString}{role.Name}</color>";
                objectives += $"\n{role.ColorString}{role.Objectives}</color>";
                alignment += $"{role.AlignmentName}";
            }
            else
            {
                roleName += "None";
                objectives += "\n- None.";
                alignment += "None";
            }

            roleName += "</color>";
            alignment += "</color>";

            if (info[3] != null)
            {
                if (!objectifier.Hidden)
                {
                    objectives += $"\n{objectifier.ColorString}{objectifier.TaskText}</color>";
                    objectifierName += $"{objectifier.ColorString}{objectifier.Name}</color>";
                }
            }
            else
                objectifierName += "None";

            objectifierName += "</color>";

            if (info[2] != null)
            {
                if (!ability.Hidden)
                    abilityName += $"{ability.ColorString}{ability.Name}</color>";
            }
            else
                abilityName += "None";

            abilityName += "</color>";

            if (info[1] != null)
            {
                if (!modifier.Hidden)
                    modifierName += $"{modifier.ColorString}{modifier.Name}</color>";
            }
            else
                modifierName += "None";

            modifierName += "</color>";

            if (player.IsRecruit())
            {
                var jackal = player.GetJackal();
                objectives += $"\n<color=#{Colors.Cabal.ToHtmlStringRGBA()}>- You are a member of the Cabal. Help {jackal.PlayerName} in taking over the mission.</color>";
            }
            else if (player.IsResurrected())
            {
                var necromancer = player.GetNecromancer();
                objectives += $"\n<color=#{Colors.Reanimated.ToHtmlStringRGBA()}>- You are a member of the Reanimated. Help {necromancer.PlayerName} in taking over the mission.</color>";
            }
            else if (player.IsPersuaded())
            {
                var whisperer = player.GetWhisperer();
                objectives += $"\n<color=#{Colors.Sect.ToHtmlStringRGBA()}>- You are a member of the Sect. Help {whisperer.PlayerName} in taking over the mission.</color>";
            }
            else if (player.IsBitten())
            {
                var dracula = player.GetDracula();
                objectives += $"\n<color=#{Colors.Undead.ToHtmlStringRGBA()}>- You are a member of the Undead. Help {dracula.PlayerName} in taking over the mission." +
                    "\n- Attempting to interact with a <color=#C0C0C0FF>Vampire Hunter</color> will force them to kill you.</color>";
            }

            objectives += "</color>";
            var hassomething = false;

            if (info[0] != null)
            {
                abilities += $"\n{role.ColorString}{role.AbilitiesText}</color>";
                hassomething = true;
            }

            if (info[2] != null && !ability.Hidden)
            {
                abilities += $"\n{ability.ColorString}{ability.TaskText}</color>";
                hassomething = true;
            }

            if (!hassomething)
                abilities += "\n- None.";

            abilities += "</color>";
            var hasnothing = true;

            if (info[1] != null && !modifier.Hidden)
            {
                attributes += $"\n{modifier.ColorString}{modifier.TaskText}</color>";
                hasnothing = false;
            }

            if (player.IsGuessTarget() && CustomGameOptions.GuesserTargetKnows)
            {
                attributes += "\n<color=#EEE5BEFF>- Someone wants to guess you.</color>";
                hasnothing = false;
            }

            if (player.IsExeTarget() && CustomGameOptions.ExeTargetKnows)
            {
                attributes += "\n<color=#CCCCCCFF>- Someone wants you ejected.</color>";
                hasnothing = false;
            }

            if (player.IsGATarget() && CustomGameOptions.GATargetKnows)
            {
                attributes += "\n<color=#FFFFFFFF>- Someone wants to protect you.</color>";
                hasnothing = false;
            }

            if (player.IsBHTarget())
            {
                attributes += "\n<color=#B51E39FF>- There is a bounty on your head.</color>";
                hasnothing = false;
            }

            if (player.Data.IsDead)
            {
                attributes += "\n<color=#FF0000FF>- You are dead.</color>";
                hasnothing = false;
            }

            if (!player.CanDoTasks())
            {
                attributes += "\n- Your tasks are fake.";
                hasnothing = false;
            }

            if (hasnothing)
                attributes += "\n- None.";

            attributes += "</color>";
            return $"{roleName}\n{objectifierName}\n{abilityName}\n{modifierName}\n{alignment}\n{objectives}\n{abilities}\n{attributes}\n<color=#FFFFFFFF>Tasks:</color>";
        }

        public static void RegenTask(this PlayerControl player)
        {
            try
            {
                foreach (var task2 in player.myTasks.ToArray())
                {
                    var importantTextTask = task2.Cast<ImportantTextTask>();

                    if (importantTextTask.Text.Contains("Sabotage and kill everyone") || importantTextTask.Text.Contains("Fake Tasks") || importantTextTask.Text.Contains("Role") ||
                        importantTextTask.Text.Contains("tasks to win"))
                    {
                        player.myTasks.Remove(importantTextTask);
                    }
                }
            } catch {}

            var task = new GameObject("DetailTask").AddComponent<ImportantTextTask>();
            task.transform.SetParent(player.transform, false);
            task.Text = player.GetTaskList();
            player.myTasks.Insert(0, task);
        }

        public static void RoleUpdate(this Role newRole, Role former)
        {
            newRole.RoleHistory.Add(former);
            newRole.RoleHistory.AddRange(former.RoleHistory);
            newRole.Faction = former.Faction;
            newRole.SubFaction = former.SubFaction;
            newRole.FactionColor = former.FactionColor;
            newRole.SubFactionColor = former.SubFactionColor;
            newRole.DeathReason = former.DeathReason;
            newRole.KilledBy = former.KilledBy;
            newRole.IsBitten = former.IsBitten;
            newRole.IsRecruit = former.IsRecruit;
            newRole.IsResurrected = former.IsResurrected;
            newRole.IsPersuaded = former.IsPersuaded;
            newRole.IsIntFanatic = former.IsIntFanatic;
            newRole.IsIntTraitor = former.IsIntTraitor;
            newRole.IsSynFanatic = former.IsSynFanatic;
            newRole.IsSynTraitor = former.IsSynTraitor;
            newRole.IsIntAlly = former.IsIntAlly;
            newRole.IsSynAlly = former.IsSynAlly;
            newRole.IsCrewAlly = former.IsCrewAlly;
            newRole.Player.RegenTask();
        }
    }
}
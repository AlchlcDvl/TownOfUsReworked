using HarmonyLib;
using Hazel;
using System;
using System.Collections.Generic;
using System.Linq;
using Reactor.Extensions;
using TMPro;
using TownOfUs.Roles.Modifiers;
using UnhollowerBaseLib;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
using TownOfUs.Extensions;
using TownOfUs.ImpostorRoles.TraitorMod;

namespace TownOfUs.Roles
{
    public abstract class Role
    {
        public static readonly Dictionary<byte, Role> RoleDictionary = new Dictionary<byte, Role>();
        public static readonly List<KeyValuePair<byte, RoleEnum>> RoleHistory = new List<KeyValuePair<byte, RoleEnum>>();

        public static bool NobodyWins;
        public static bool SurvOnlyWins;

        public List<KillButton> ExtraButtons = new List<KillButton>();

        public Func<string> ImpostorText;
        public Func<string> TaskText;

        protected Role(PlayerControl player)
        {
            Player = player;
            RoleDictionary.Add(player.PlayerId, this);
        }

        public static IEnumerable<Role> AllRoles => RoleDictionary.Values.ToList();
        protected internal string Name { get; set; }
        protected internal string FactionName { get; set; }
        protected internal string AlignmentName { get; set; }
        protected internal string impostorText { get; set; }

        private PlayerControl _player { get; set; }

        public PlayerControl Player
        {
            get => _player;
            set
            {
                if (_player != null) _player.nameText().color = Color.white;
                _player = value;
                PlayerName = value.Data.PlayerName;
            }
        }

        protected float Scale { get; set; } = 1f;
        protected internal Color Color { get; set; }
        protected internal Color FactionColor { get; set; }
        protected internal RoleEnum RoleType { get; set; }
        public bool LostByRPC { get; protected set; }
        protected internal int TasksLeft => Player.Data.Tasks.ToArray().Count(x => !x.Complete);
        protected internal int TotalTasks => Player.Data.Tasks.Count;
        protected internal Faction Faction { get; set; }
        protected internal SubFaction SubFaction { get; set; }
        protected internal Alignment Alignment { get; set; }

        public bool Local => PlayerControl.LocalPlayer.PlayerId == Player.PlayerId;

        protected internal bool Hidden { get; set; } = true;

        public static uint NetId => PlayerControl.LocalPlayer.NetId;
        public string PlayerName { get; set; }

        public string ColorString => "<color=#" + Color.ToHtmlStringRGBA() + ">";
        public string ColorEnd => "</color>";

        private bool Equals(Role other)
        {
            return Equals(Player, other.Player) && RoleType == other.RoleType;
        }

        public void AddToRoleHistory(RoleEnum role)
        {
            RoleHistory.Add(KeyValuePair.Create(_player.PlayerId, role));
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(Role)) return false;
            return Equals((Role)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Player, (int)RoleType);
        }

        internal virtual bool Criteria()
        {
            Player.nameText().transform.localPosition = new Vector3(
                0f,
                Player.Data.DefaultOutfit.HatId == "hat_NoHat" ? 1.5f : 2.0f,
                -0.5f
            );
            return (DeadCriteria() || ImpostorCriteria() || LoverCriteria() || SelfCriteria() || RoleCriteria() || GuardianAngelCriteria() || ExecutionerCriteria() || Local);
        }

        internal virtual bool ColorCriteria()
        {
            return SelfCriteria() || DeadCriteria() || ImpostorCriteria() || RoleCriteria() || GuardianAngelCriteria() || ExecutionerCriteria();
        }

        internal virtual bool DeadCriteria()
        {
            if (PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeRoles) return Utils.ShowDeadBodies;
            return false;
        }

        internal virtual bool ImpostorCriteria()
        {
            if (Faction == Faction.Intruders && PlayerControl.LocalPlayer.Data.IsImpostor() && CustomGameOptions.ImpostorSeeRoles) return true;
            return false;
        }

        internal virtual bool LoverCriteria()
        {
            if (PlayerControl.LocalPlayer.Is(ModifierEnum.Lover))
            {
                if (Local) return true;
                var lover = Modifier.GetModifier<Lover>(PlayerControl.LocalPlayer);
                return lover.OtherLover.Player == Player;
            }
            return false;
        }

        internal virtual bool SelfCriteria()
        {
            return GetRole(PlayerControl.LocalPlayer) == this;
        }

        internal virtual bool RoleCriteria()
        {
            return PlayerControl.LocalPlayer.Is(RoleEnum.Mystic) && Role.GetRole<Mystic>(PlayerControl.LocalPlayer).Reported.Contains(Player.PlayerId);;
        }

        internal virtual bool GuardianAngelCriteria()
        {
            return PlayerControl.LocalPlayer.Is(RoleEnum.GuardianAngel) && CustomGameOptions.GAKnowsTargetRole && Player == GetRole<GuardianAngel>(PlayerControl.LocalPlayer).target;
        }

        internal virtual bool ExecutionerCriteria()
        {
            return PlayerControl.LocalPlayer.Is(RoleEnum.Executioner) && CustomGameOptions.ExeKnowsTargetRole && Player == GetRole<Executioner>(PlayerControl.LocalPlayer).target;
        }

        protected virtual void IntroPrefix(IntroCutscene._ShowTeam_d__21 __instance) {}

        public static void NobodyWinsFunc()
        {
            NobodyWins = true;
        }

        public static void SurvOnlyWin()
        {
            SurvOnlyWins = true;
        }

        internal static bool NobodyEndCriteria(ShipStatus __instance)
        {
            bool CheckNoImpsNoCrews()
            {
                var alives = PlayerControl.AllPlayerControls.ToArray()
                    .Where(x => !x.Data.IsDead && !x.Data.Disconnected).ToList();
                if (alives.Count == 0) return false;
                var flag = alives.All(x =>
                {
                    var role = GetRole(x);
                    if (role == null) return false;
                    var flag2 = role.Faction == Faction.Neutral && !x.Is(Alignment.NeutralKill);

                    return flag2;
                });

                return flag;
            }

            bool SurvOnly()
            {
                var alives = PlayerControl.AllPlayerControls.ToArray()
                    .Where(x => !x.Data.IsDead && !x.Data.Disconnected).ToList();
                if (alives.Count == 0) return false;
                var flag = false;
                foreach (var player in alives)
                {
                    if (player.Is(RoleEnum.Survivor)) flag = true;
                }
                return flag;
            }

            if (CheckNoImpsNoCrews())
            {
                if (SurvOnly())
                {
                    var messageWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                        (byte)CustomRPC.SurvivorOnlyWin, SendOption.Reliable, -1);
                    AmongUsClient.Instance.FinishRpcImmediately(messageWriter);

                    SurvOnlyWin();
                    Utils.EndGame();
                    return false;
                }
                else
                {
                    var messageWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                        (byte)CustomRPC.NobodyWins, SendOption.Reliable, -1);
                    AmongUsClient.Instance.FinishRpcImmediately(messageWriter);

                    NobodyWinsFunc();
                    Utils.EndGame();
                    return false;
                }
            }
            return true;
        }

        internal virtual bool EABBNOODFGL(ShipStatus __instance)
        {
            return true;
        }

        protected virtual string NameText(bool revealTasks, bool revealRole, bool revealModifier, bool revealLover, PlayerVoteArea player = null)
        {
            if (CamouflageUnCamouflage.IsCamoed && player == null) return "";

            if (Player == null) return "";

            String PlayerName = Player.GetDefaultOutfit().PlayerName;

            foreach (var role in GetRoles(RoleEnum.GuardianAngel))
            {
                var ga = (GuardianAngel)role;
                if (Player == ga.target && Player == PlayerControl.LocalPlayer && CustomGameOptions.GATargetKnows)
                {
                    PlayerName += "<color=#FFFFFFFF> ★</color>";
                }
            }

            foreach (var role in GetRoles(RoleEnum.Executioner))
            {
                var exe = (Executioner)role;
                if (Player == exe.target && Player == PlayerControl.LocalPlayer && CustomGameOptions.ExeTargetKnows)
                {
                    PlayerName += "<color=#CCCCCCFF> §</color>";
                }
            }

            var modifier = Modifier.GetModifier(Player);
            if (modifier != null && modifier.GetColoredSymbol() != null)
            {
                if (modifier.ModifierType == ModifierEnum.Lover && (revealModifier || revealLover))
                    PlayerName += $" {modifier.GetColoredSymbol()}";
                else if (modifier.ModifierType != ModifierEnum.Lover && revealModifier)
                    PlayerName += $" {modifier.GetColoredSymbol()}";
            }

            if (revealTasks && (Faction == Faction.Crewmates || RoleType == RoleEnum.Phantom || RoleType == RoleEnum.Taskmaster))
            {
                if ((PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.SeeTasksWhenDead) || (MeetingHud.Instance && CustomGameOptions.SeeTasksDuringMeeting) || (!PlayerControl.LocalPlayer.Data.IsDead && !MeetingHud.Instance && CustomGameOptions.SeeTasksDuringRound))
                {
                    PlayerName += $" ({TotalTasks - TasksLeft}/{TotalTasks})";
                }
            }

            if (player != null && (MeetingHud.Instance.state == MeetingHud.VoteStates.Proceeding || MeetingHud.Instance.state == MeetingHud.VoteStates.Results))
                return PlayerName;

            if (!revealRole) return PlayerName;

            Player.nameText().transform.localPosition = new Vector3(0f, Player.CurrentOutfit.HatId == "hat_NoHat" ? 1.5f : 2.0f, -0.5f);

            return PlayerName + "\n" + Name;
        }

        public static bool operator ==(Role a, Role b)
        {
            if (a is null && b is null) return true;
            if (a is null || b is null) return false;
            return a.RoleType == b.RoleType && a.Player.PlayerId == b.Player.PlayerId;
        }

        public static bool operator !=(Role a, Role b)
        {
            return !(a == b);
        }

        public void RegenTask()
        {
            bool createTask;
            try
            {
                var firstText = Player.myTasks.ToArray()[0].Cast<ImportantTextTask>();
                createTask = !firstText.Text.Contains("Role:");
            }
            catch (InvalidCastException)
            {
                createTask = true;
            }

            if (createTask)
            {
                var task = new GameObject(Name + "Task").AddComponent<ImportantTextTask>();
                task.transform.SetParent(Player.transform, false);
                task.Text = $"{ColorString}Role: {Name}\n{TaskText()}</color>";
                Player.myTasks.Insert(0, task);
                return;
            }

            Player.myTasks.ToArray()[0].Cast<ImportantTextTask>().Text = $"{ColorString}Role: {Name}\n{TaskText()}</color>";
        }

        public static T Gen<T>(Type type, PlayerControl player, CustomRPC rpc)
        {
            var role = (T)Activator.CreateInstance(type, new object[] { player });

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)rpc, SendOption.Reliable, -1);
            writer.Write(player.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            return role;
        }

        public static T Gen<T>(Type type, List<PlayerControl> players, CustomRPC rpc)
        {
            var player = players[Random.RandomRangeInt(0, players.Count)];
            
            var role = Gen<T>(type, player, rpc);
            players.Remove(player);
            return role;
        }
        
        public static Role GetRole(PlayerControl player)
        {
            if (player == null) return null;
            if (RoleDictionary.TryGetValue(player.PlayerId, out var role))
                return role;

            return null;
        }
        
        public static T GetRole<T>(PlayerControl player) where T : Role
        {
            return GetRole(player) as T;
        }

        public static Role GetRole(PlayerVoteArea area)
        {
            var player = PlayerControl.AllPlayerControls.ToArray()
                .FirstOrDefault(x => x.PlayerId == area.TargetPlayerId);
            return player == null ? null : GetRole(player);
        }

        public static IEnumerable<Role> GetRoles(RoleEnum roletype)
        {
            return AllRoles.Where(x => x.RoleType == roletype);
        }

        public static class IntroCutScenePatch
        {
            public static TextMeshPro ModifierText;

            public static float Scale;

            [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.BeginCrewmate))]
            public static class IntroCutscene_BeginCrewmate
            {
                public static void Postfix(IntroCutscene __instance)
                {
                    //System.Console.WriteLine("REACHED HERE - CREW");
                    var modifier = Modifier.GetModifier(PlayerControl.LocalPlayer);
                    if (modifier != null)
                        ModifierText = Object.Instantiate(__instance.RoleText, __instance.RoleText.transform.parent, false);
                    //System.Console.WriteLine("MODIFIER TEXT PLEASE WORK");
                    //                        Scale = ModifierText.scale;
                    else
                        ModifierText = null;

                    Lights.SetLights();
                }
            }

            [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.BeginImpostor))]
            public static class IntroCutscene_BeginImpostor
            {
                public static void Postfix(IntroCutscene __instance)
                {
                    //System.Console.WriteLine("REACHED HERE - IMP");
                    var modifier = Modifier.GetModifier(PlayerControl.LocalPlayer);
                    if (modifier != null)
                        ModifierText = Object.Instantiate(__instance.RoleText, __instance.RoleText.transform.parent, false);
                    //System.Console.WriteLine("MODIFIER TEXT PLEASE WORK");
                    //                        Scale = ModifierText.scale;
                    else
                        ModifierText = null;

                    Lights.SetLights();
                }
            }
            
            [HarmonyPatch(typeof(IntroCutscene._CoBegin_d__19), nameof(IntroCutscene._CoBegin_d__19.MoveNext))]
            public static class IntroCutscene_CoBegin_d__19
            {
                public static void Postfix(IntroCutscene._CoBegin_d__19 __instance)
                {
                    var role = GetRole(PlayerControl.LocalPlayer);
                    var modifier2 = Modifier.GetModifier(PlayerControl.LocalPlayer);

                    if (role != null)
                    {
                        __instance.__4__this.TeamTitle.text = role.FactionName;
                        __instance.__4__this.TeamTitle.color = role.FactionColor;
                        __instance.__4__this.RoleText.text = role.Name;
                        __instance.__4__this.RoleText.color = role.Color;
                        __instance.__4__this.YouAreText.color = role.Color;
                        __instance.__4__this.RoleBlurbText.color = role.Color;
                        __instance.__4__this.RoleBlurbText.text = role.ImpostorText();
                        __instance.__4__this.BackgroundBar.material.color = role.Color;
                    }

                    if (ModifierText != null)
                    {
                        var modifier = Modifier.GetModifier(PlayerControl.LocalPlayer);
                        if (modifier.GetType() == typeof(Lover))
                        {
                            ModifierText.text = $"<size=3>{modifier.TaskText()}</size>";
                        }
                        else
                        {
                            ModifierText.text = "<size=4>Modifier: " + modifier.Name + "</size>";
                        }
                        ModifierText.color = modifier.Color;

                        ModifierText.transform.position =
                            __instance.__4__this.transform.position - new Vector3(0f, 1.6f, 0f);
                        ModifierText.gameObject.SetActive(true);
                    }

                    if (CustomGameOptions.GameMode == GameMode.AllAny && CustomGameOptions.RandomNumberImps)
                        __instance.__4__this.ImpostorText.text = "There Is An <color=#FF0000FF>Unknown Number</color> Of <color=#FF0000FF>Intruders</color> Among Us";
                }
            }

            [HarmonyPatch(typeof(IntroCutscene._ShowTeam_d__21), nameof(IntroCutscene._ShowTeam_d__21.MoveNext))]
            public static class IntroCutscene_ShowTeam__d_21
            {
                public static void Prefix(IntroCutscene._ShowTeam_d__21 __instance)
                {
                    var role = GetRole(PlayerControl.LocalPlayer);

                    if (role != null) role.IntroPrefix(__instance);
                }

                public static void Postfix(IntroCutscene._ShowRole_d__24 __instance)
                {
                    var role = GetRole(PlayerControl.LocalPlayer);
                    var modifier2 = Modifier.GetModifier(PlayerControl.LocalPlayer);

                    if (role != null)
                    {
                        __instance.__4__this.TeamTitle.text = role.FactionName;
                        __instance.__4__this.TeamTitle.color = role.FactionColor;
                        __instance.__4__this.RoleText.text = role.Name;
                        __instance.__4__this.RoleText.color = role.Color;
                        __instance.__4__this.YouAreText.color = role.Color;
                        __instance.__4__this.RoleBlurbText.color = role.Color;
                        __instance.__4__this.RoleBlurbText.text = role.ImpostorText();
                        __instance.__4__this.BackgroundBar.material.color = role.Color;
                    }

                    if (ModifierText != null)
                    {
                        var modifier = Modifier.GetModifier(PlayerControl.LocalPlayer);
                        if (modifier.GetType() == typeof(Lover))
                        {
                            ModifierText.text = $"<size=3>{modifier.TaskText()}</size>";
                        }
                        else
                        {
                            ModifierText.text = "<size=4>Modifier: " + modifier.Name + "</size>";
                        }
                        ModifierText.color = modifier.Color;

                        ModifierText.transform.position = __instance.__4__this.transform.position - new Vector3(0f, 1.6f, 0f);
                        ModifierText.gameObject.SetActive(true);
                    }
                }
            }

            [HarmonyPatch(typeof(IntroCutscene._ShowRole_d__24), nameof(IntroCutscene._ShowRole_d__24.MoveNext))]
            public static class IntroCutscene_ShowRole_d__24
            {
                public static void Postfix(IntroCutscene._ShowRole_d__24 __instance)
                {
                    var role = GetRole(PlayerControl.LocalPlayer);
                    var modifier2 = Modifier.GetModifier(PlayerControl.LocalPlayer);

                    if (role != null)
                    {
                        __instance.__4__this.TeamTitle.text = role.FactionName;
                        __instance.__4__this.TeamTitle.color = role.FactionColor;
                        __instance.__4__this.RoleText.text = role.Name;
                        __instance.__4__this.RoleText.color = role.Color;
                        __instance.__4__this.YouAreText.color = role.Color;
                        __instance.__4__this.RoleBlurbText.text = role.ImpostorText();
                        __instance.__4__this.RoleBlurbText.color = role.Color;
                        __instance.__4__this.BackgroundBar.material.color = role.Color;
                    }

                    if (ModifierText != null)
                    {
                        var modifier = Modifier.GetModifier(PlayerControl.LocalPlayer);
                        if (modifier.GetType() == typeof(Lover))
                        {
                            ModifierText.text = $"<size=3>{modifier.TaskText()}</size>";
                        }
                        else
                        {
                            ModifierText.text = "<size=4>Modifier: " + modifier.Name + "</size>";
                        }

                        ModifierText.color = modifier.Color;
                        ModifierText.transform.position = __instance.__4__this.transform.position - new Vector3(0f, 1.6f, 0f);
                        ModifierText.gameObject.SetActive(true);
                    }

                    if (CustomGameOptions.GameMode == GameMode.AllAny && CustomGameOptions.RandomNumberImps)
                        __instance.__4__this.ImpostorText.text = "There Is An <color=#FF0000FF>Unknown Number</color> Of <color=#FF0000FF>Intruders</color> Among Us";
                }
            }
        }

        [HarmonyPatch(typeof(PlayerControl._CoSetTasks_d__107), nameof(PlayerControl._CoSetTasks_d__107.MoveNext))]
        public static class PlayerControl_SetTasks
        {
            public static void Postfix(PlayerControl._CoSetTasks_d__107 __instance)
            {
                if (__instance == null) return;
                var player = __instance.__4__this;
                var role = GetRole(player);
                var modifier = Modifier.GetModifier(player);

                if (modifier != null)
                {
                    var modTask = new GameObject(modifier.Name + "Task").AddComponent<ImportantTextTask>();
                    modTask.transform.SetParent(player.transform, false);
                    modTask.Text =
                        $"{modifier.ColorString}Modifier: {modifier.Name}\n{modifier.TaskText()}</color>";
                    player.myTasks.Insert(0, modTask);
                }

                if (role == null) return;
                if (role.RoleType == RoleEnum.Amnesiac && role.Player != PlayerControl.LocalPlayer) return;
                var task = new GameObject(role.Name + "Task").AddComponent<ImportantTextTask>();
                task.transform.SetParent(player.transform, false);
                task.Text = $"{role.ColorString}Role: {role.Name}\n{role.TaskText()}</color>";
                player.myTasks.Insert(0, task);
            }
        }

        [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.CheckEndCriteria))]
        public static class ShipStatus_KMPKPPGPNIH
        {
            public static bool Prefix(ShipStatus __instance)
            {
                //System.Console.WriteLine("EABBNOODFGL");
                if (!AmongUsClient.Instance.AmHost) return false;
                if (__instance.Systems.ContainsKey(SystemTypes.LifeSupp))
                {
                    var lifeSuppSystemType = __instance.Systems[SystemTypes.LifeSupp].Cast<LifeSuppSystemType>();
                    if (lifeSuppSystemType.Countdown < 0f) return true;
                }

                if (__instance.Systems.ContainsKey(SystemTypes.Laboratory))
                {
                    var reactorSystemType = __instance.Systems[SystemTypes.Laboratory].Cast<ReactorSystemType>();
                    if (reactorSystemType.Countdown < 0f) return true;
                }

                if (__instance.Systems.ContainsKey(SystemTypes.Reactor))
                {
                    var reactorSystemType = __instance.Systems[SystemTypes.Reactor].Cast<ICriticalSabotage>();
                    if (reactorSystemType.Countdown < 0f) return true;
                }

                if (GameData.Instance.TotalTasks <= GameData.Instance.CompletedTasks) return true;

                var result = true;
                foreach (var role in AllRoles)
                {
                    var roleIsEnd = role.EABBNOODFGL(__instance);
                    var modifier = Modifier.GetModifier(role.Player);
                    bool modifierIsEnd = true;
                    var alives = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Data.IsDead && !x.Data.Disconnected).ToList();
                    var impsAlive = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Data.IsDead && !x.Data.Disconnected && x.Data.IsImpostor()).ToList();
                    var traitorIsEnd = true;
                    if (SetTraitor.WillBeTraitor != null)
                    {
                        traitorIsEnd = SetTraitor.WillBeTraitor.Data.IsDead || SetTraitor.WillBeTraitor.Data.Disconnected || alives.Count < CustomGameOptions.LatestSpawn || impsAlive.Count * 2 >= alives.Count;
                    }
                    if (modifier != null)
                        modifierIsEnd = modifier.EABBNOODFGL(__instance);
                    if (!roleIsEnd || !modifierIsEnd || !traitorIsEnd) result = false;
                }

                if (!NobodyEndCriteria(__instance)) result = false;

                return result;
            }
        }
        
        /*private void CheckEndCriteria()
        {
            if (!GameData.Instance)
            {
                return;
            }

            ISystemType systemType;

            if (this.Systems.TryGetValue(SystemTypes.LifeSupp, out systemType))
            {
                LifeSuppSystemType lifeSuppSystemType = (LifeSuppSystemType)systemType;
                if (lifeSuppSystemType.Countdown < 0f)
                {
                    this.EndGameForSabotage();
                    lifeSuppSystemType.Countdown = 10000f;
                }
            }

            ISystemType systemType2;

            if ((this.Systems.TryGetValue(SystemTypes.Reactor, out systemType2) || this.Systems.TryGetValue(SystemTypes.Laboratory, out systemType2)) && systemType2 is ICriticalSabotage)
            {
                ICriticalSabotage criticalSabotage = (ICriticalSabotage)systemType2;
                if (criticalSabotage.Countdown < 0f)
                {
                    this.EndGameForSabotage();
                    criticalSabotage.ClearSabotage();
                }
            }

            int num = 0;
            int num2 = 0;
            int num3 = 0;

            for (int i = 0; i < GameData.Instance.PlayerCount; i++)
            {
                GameData.PlayerInfo playerInfo = GameData.Instance.AllPlayers[i];
                if (!playerInfo.Disconnected)
                {
                    if (playerInfo.Role.IsImpostor)
                    {
                        num3++;
                    }
                    if (!playerInfo.IsDead)
                    {
                        if (playerInfo.Role.IsImpostor)
                        {
                            num2++;
                        }
                        else
                        {
                            num++;
                        }
                    }
                }
            }
            if (num2 <= 0 && (!DestroyableSingleton<TutorialManager>.InstanceExists || num3 > 0))
            {
                if (!DestroyableSingleton<TutorialManager>.InstanceExists)
                {
                    this.ShouldCheckForGameEnd = false;
                    ShipStatus.RpcEndGame(GameOverReason.HumansByVote, !SaveManager.BoughtNoAds);
                    return;
                }
                if (!Minigame.Instance)
                {
                    DestroyableSingleton<HudManager>.Instance.ShowPopUp(DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.GameOverImpostorDead, Array.Empty<object>()));
                    ShipStatus.ReviveEveryone();
                    return;
                }
            }
            else if (num <= num2)
            {
                if (DestroyableSingleton<TutorialManager>.InstanceExists)
                {
                    DestroyableSingleton<HudManager>.Instance.ShowPopUp(DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.GameOverImpostorKills, Array.Empty<object>()));
                    ShipStatus.ReviveEveryone();
                    return;
                }
                if (PlayerControl.GameOptions.gameType == GameType.Normal)
                {
                    this.ShouldCheckForGameEnd = false;
                    GameOverReason endReason;
                    switch (TempData.LastDeathReason)
                    {
                    case DeathReason.Exile:
                        endReason = GameOverReason.ImpostorByVote;
                        break;
                    case DeathReason.Kill:
                        endReason = GameOverReason.ImpostorByKill;
                        break;
                    default:
                        endReason = GameOverReason.HumansDisconnect;
                        break;
                    }
                    ShipStatus.RpcEndGame(endReason, !SaveManager.BoughtNoAds);
                    return;
                }
            }
            else if (!DestroyableSingleton<TutorialManager>.InstanceExists)
            {
                if (PlayerControl.GameOptions.gameType == GameType.Normal && GameData.Instance.TotalTasks <= GameData.Instance.CompletedTasks)
                {
                    base.enabled = false;
                    ShipStatus.RpcEndGame(GameOverReason.HumansByTask, !SaveManager.BoughtNoAds);
                    return;
                }
            }
            else if (Enumerable.All<PlayerTask>(PlayerControl.LocalPlayer.myTasks, (PlayerTask t) => t.IsComplete))
            {
                DestroyableSingleton<HudManager>.Instance.ShowPopUp(DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.GameOverTaskWin, Array.Empty<object>()));
                this.Begin();
            }
        }*/

        [HarmonyPatch(typeof(LobbyBehaviour), nameof(LobbyBehaviour.Start))]
        public static class LobbyBehaviour_Start
        {
            private static void Postfix(LobbyBehaviour __instance)
            {
                foreach (var role in AllRoles.Where(x => x.RoleType == RoleEnum.Snitch))
                {
                    ((Snitch)role).ImpArrows.DestroyAll();
                    ((Snitch)role).SnitchArrows.Values.DestroyAll();
                    ((Snitch)role).SnitchArrows.Clear();
                }
                foreach (var role in AllRoles.Where(x => x.RoleType == RoleEnum.Tracker))
                {
                    ((Tracker)role).TrackerArrows.Values.DestroyAll();
                    ((Tracker)role).TrackerArrows.Clear();
                }
                foreach (var role in AllRoles.Where(x => x.RoleType == RoleEnum.Amnesiac))
                {
                    ((Amnesiac)role).BodyArrows.Values.DestroyAll();
                    ((Amnesiac)role).BodyArrows.Clear();
                }
                foreach (var role in AllRoles.Where(x => x.RoleType == RoleEnum.Medium))
                {
                    ((Medium)role).MediatedPlayers.Values.DestroyAll();
                    ((Medium)role).MediatedPlayers.Clear();
                }
                foreach (var role in AllRoles.Where(x => x.RoleType == RoleEnum.Mystic))
                {
                    ((Mystic)role).BodyArrows.Values.DestroyAll();
                    ((Mystic)role).BodyArrows.Clear();
                }
                foreach (var role in AllRoles.Where(x => x.RoleType == RoleEnum.Taskmaster))
                {
                    ((Taskmaster)role).ImpArrows.DestroyAll();
                    ((Taskmaster)role).TMArrows.Values.DestroyAll();
                    ((Taskmaster)role).TMArrows.Clear();
                }

                RoleDictionary.Clear();
                RoleHistory.Clear();
                Modifier.ModifierDictionary.Clear();
                Ability.AbilityDictionary.Clear();
                Lights.SetLights(Color.white);
            }
        }

        [HarmonyPatch(typeof(TranslationController), nameof(TranslationController.GetString), typeof(StringNames),
            typeof(Il2CppReferenceArray<Il2CppSystem.Object>))]
        public static class TranslationController_GetString
        {
            public static void Postfix(ref string __result, [HarmonyArgument(0)] StringNames name)
            {
                if (ExileController.Instance == null || ExileController.Instance.exiled == null) return;

                switch (name)
                {
                    case StringNames.ExileTextPN:
                    case StringNames.ExileTextSN:
                    case StringNames.ExileTextPP:
                    case StringNames.ExileTextSP:
                        {
                            var info = ExileController.Instance.exiled;
                            var role = GetRole(info.Object);
                            if (role == null) return;
                            var roleName = role.RoleType == RoleEnum.Glitch ? role.Name : $"{role.Name}";
                            __result = $"{info.PlayerName} was {roleName}.";
                            return;
                        }
                }
            }
        }

        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        public static class HudManager_Update
        {
            private static Vector3 oldScale = Vector3.zero;
            private static Vector3 oldPosition = Vector3.zero;

            private static void UpdateMeeting(MeetingHud __instance)
            {
                foreach (var player in __instance.playerStates)
                {
                    var role = GetRole(player);
                    if (role != null && role.Criteria())
                    {
                        bool selfFlag = role.SelfCriteria();
                        bool deadFlag = role.DeadCriteria();
                        bool impostorFlag = role.ImpostorCriteria();
                        bool loverFlag = role.LoverCriteria();
                        bool roleFlag = role.RoleCriteria();
                        bool targetFlag = role.GuardianAngelCriteria() || role.ExecutionerCriteria();
                        player.NameText.text = role.NameText(
                            selfFlag || deadFlag || role.Local,
                            selfFlag || deadFlag || impostorFlag || roleFlag || targetFlag,
                            selfFlag || deadFlag, loverFlag, player
                        );
                        if(role.ColorCriteria())
                            player.NameText.color = role.Color;
                    }
                    else
                    {
                        try
                        {
                            player.NameText.text = role.Player.GetDefaultOutfit().PlayerName;
                        }
                        catch {}
                    }
                }
            }

            [HarmonyPriority(Priority.First)]
            private static void Postfix(HudManager __instance)
            {
                if (MeetingHud.Instance != null) UpdateMeeting(MeetingHud.Instance);

                if (PlayerControl.AllPlayerControls.Count <= 1) return;
                if (PlayerControl.LocalPlayer == null) return;
                if (PlayerControl.LocalPlayer.Data == null) return;

                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (!(player.Data != null && player.Data.IsImpostor() && PlayerControl.LocalPlayer.Data.IsImpostor()))
                    {
                        player.nameText().text = player.name;
                        player.nameText().color = Color.white;
                    }

                    var role = GetRole(player);
                    if (role != null)
                        if (role.Criteria())
                        {

                            bool selfFlag = role.SelfCriteria();
                            bool deadFlag = role.DeadCriteria();
                            bool impostorFlag = role.ImpostorCriteria();
                            bool loverFlag = role.LoverCriteria();
                            bool roleFlag = role.RoleCriteria();
                            bool targetFlag = role.GuardianAngelCriteria() || role.ExecutionerCriteria();
                            player.nameText().text = role.NameText(
                                selfFlag || deadFlag || role.Local,
                                selfFlag || deadFlag || impostorFlag || roleFlag || targetFlag,
                                selfFlag || deadFlag, loverFlag
                             );

                            if (role.ColorCriteria())
                                player.nameText().color = role.Color;
                        }

                    if (player.Data != null && PlayerControl.LocalPlayer.Data.IsImpostor() && player.Data.IsImpostor()) continue;
                }
            }
        }
    }
}
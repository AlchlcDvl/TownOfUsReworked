using HarmonyLib;
using Hazel;
using System;
using System.Collections.Generic;
using System.Linq;
using Reactor.Utilities.Extensions;
using TMPro;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Patches;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Enums;
using TownOfUsReworked.PlayerLayers.Modifiers;
using TownOfUsReworked.PlayerLayers.Objectifiers;
using TownOfUsReworked.PlayerLayers.Abilities;
using TownOfUsReworked.PlayerLayers.Objectifiers.TraitorMod;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public abstract class Role
    {
        public static readonly Dictionary<byte, Role> RoleDictionary = new Dictionary<byte, Role>();
        public static readonly List<KeyValuePair<byte, RoleEnum>> RoleHistory = new List<KeyValuePair<byte, RoleEnum>>();
        public static readonly List<string> Vowels = new List<string>();
        public static readonly Dictionary<int, string> LightDarkColors = new Dictionary<int, string>();

        public static bool NobodyWins;
        public static bool NBOnlyWin;

        public List<KillButton> ExtraButtons = new List<KillButton>();

        protected internal Color32 Color { get; set; }
        protected internal Color32 FactionColor { get; set; }
        protected internal RoleEnum RoleType { get; set; }
        protected internal Faction Faction { get; set; }
        protected internal RoleAlignment RoleAlignment { get; set; }
        protected internal InspResults Results { get; set; }
        protected internal SubFaction SubFaction { get; set; }
        protected internal string Name { get; set; }
        protected internal string FactionName { get; set; }
        protected internal string IntroText { get; set; }
        protected internal string CoronerDeadReport { get; set; }
        protected internal string CoronerKillerReport { get; set; }
        protected internal AudioClip IntroSound { get; set; }
        public Func<string> ImpostorText { get; set; }
        public Func<string> TaskText { get; set; }
        public Func<string> AlignmentName { get; set; }

        protected Role(PlayerControl player)
        {
            Player = player;
            RoleDictionary.Add(player.PlayerId, this);
        }

        public static IEnumerable<Role> AllRoles => RoleDictionary.Values.ToList();

        private PlayerControl _player { get; set; }

        public PlayerControl Player
        {
            get => _player;
            set
            {
                if (_player != null)
                    _player.nameText().color = new Color32(255, 255, 255, 255);

                _player = value;
                PlayerName = value.Data.PlayerName;
            }
        }

        protected float Scale { get; set; } = 1f;
        public bool LostByRPC { get; protected set; }
        protected internal int TasksLeft => Player.Data.Tasks.ToArray().Count(x => !x.Complete);
        protected internal int TotalTasks => Player.Data.Tasks.Count;

        public bool Local => PlayerControl.LocalPlayer.PlayerId == Player.PlayerId;

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
            if (ReferenceEquals(null, obj))
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            if (obj.GetType() != typeof(Role))
                return false;

            return Equals((Role)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Player, (int)RoleType);
        }

        protected virtual void IntroPrefix(IntroCutscene._ShowTeam_d__21 __instance) {}

        protected virtual void IntroFactionPrefix(IntroCutscene._CoBegin_d__19 __instance) {}

        public static void NobodyWinsFunc()
        {
            NobodyWins = true;
        }

        public static void NBOnlyWins()
        {
            NBOnlyWin = true;
        }

        internal static bool NobodyEndCriteria(ShipStatus __instance)
        {
            bool CheckNoImpsNoCrews()
            {
                var alives = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Data.IsDead && !x.Data.Disconnected).ToList();

                if (alives.Count == 0)
                    return false;

                var flag = alives.All(x =>
                {
                    var role = GetRole(x);

                    if (role == null)
                        return false;

                    var flag2 = role.Faction == Faction.Neutral && !(x.Is(RoleAlignment.NeutralKill) | x.Is(RoleAlignment.NeutralPower));

                    return flag2;
                });

                return flag;
            }

            bool SurvOnly()
            {
                var alives = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Data.IsDead && !x.Data.Disconnected).ToList();
                var flag = false;

                if (alives.Count == 0)
                    return flag;

                foreach (var player in alives)
                {
                    if (player.Is(RoleEnum.Survivor))
                        flag = true;
                }

                return flag;
            }

            if (CheckNoImpsNoCrews())
            {
                if (SurvOnly())
                {
                    var messageWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.NBOnlyWin,
                        SendOption.Reliable, -1);
                    AmongUsClient.Instance.FinishRpcImmediately(messageWriter);

                    NBOnlyWins();
                    Utils.EndGame();
                    return false;
                }
                else
                {
                    var messageWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.NobodyWins,
                        SendOption.Reliable, -1);
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

        protected virtual string NameText(PlayerVoteArea player = null)
        {
            if (Player == null)
                return "";

            if (CamouflageUnCamouflage.IsCamoed && player == null)
                return "";

            string PlayerName = Player.GetDefaultOutfit().PlayerName;
            int ColorID = Player.CurrentOutfit.ColorId;

            var modifier = Modifier.GetModifier(Player);
            var objectifier = Objectifier.GetObjectifier(Player);
            var role2 = GetRole(Player);

            if ((role2.Faction == Faction.Crew && objectifier.ObjectifierType != ObjectifierEnum.Lovers) | role2.RoleType == RoleEnum.Taskmaster |
                (objectifier.ObjectifierType == ObjectifierEnum.Phantom && Player.Data.IsDead))
            {
                if (CustomGameOptions.SeeTasks)
                    PlayerName += $" ({TotalTasks - TasksLeft}/{TotalTasks})";
            }

            if (CustomGameOptions.LighterDarker)
            {
                LightDarkColors.Add(0, "darker"); //Red
                LightDarkColors.Add(1, "darker"); //Blue
                LightDarkColors.Add(2, "darker"); //Green
                LightDarkColors.Add(3, "lighter"); //Pink
                LightDarkColors.Add(4, "lighter"); //Orange
                LightDarkColors.Add(5, "lighter"); //Yellow
                LightDarkColors.Add(6, "darker"); //Black
                LightDarkColors.Add(7, "lighter"); //White
                LightDarkColors.Add(8, "darker"); //Purple
                LightDarkColors.Add(9, "darker"); //Brown
                LightDarkColors.Add(10, "lighter"); //Cyan
                LightDarkColors.Add(11, "lighter"); //Lime
                LightDarkColors.Add(12, "darker"); //Maroon
                LightDarkColors.Add(13, "lighter"); //Rose
                LightDarkColors.Add(14, "lighter"); //Banana
                LightDarkColors.Add(15, "darker"); //Grey
                LightDarkColors.Add(16, "darker"); //Tan
                LightDarkColors.Add(17, "lighter"); //Coral
                LightDarkColors.Add(18, "darker"); //Watermelon
                LightDarkColors.Add(19, "darker"); //Chocolate
                LightDarkColors.Add(20, "lighter"); //Sky Blue
                LightDarkColors.Add(21, "darker"); //Biege
                LightDarkColors.Add(22, "lighter"); //Hot Pink
                LightDarkColors.Add(23, "lighter"); //Turquoise
                LightDarkColors.Add(24, "lighter"); //Lilac
                LightDarkColors.Add(25, "darker"); //Olive
                LightDarkColors.Add(26, "lighter"); //Azure
                LightDarkColors.Add(27, "lighter"); //Tomato
                LightDarkColors.Add(28, "darker"); //backrooms
                LightDarkColors.Add(29, "lighter"); //Gold
                LightDarkColors.Add(30, "darker"); //Space
                LightDarkColors.Add(31, "lighter"); //Ice
                LightDarkColors.Add(32, "lighter"); //Mint
                LightDarkColors.Add(33, "darker"); //BTS
                LightDarkColors.Add(34, "darker"); //Forest Green
                LightDarkColors.Add(35, "lighter"); //Donation
                LightDarkColors.Add(36, "darker"); //Cherry
                LightDarkColors.Add(37, "lighter"); //Toy
                LightDarkColors.Add(38, "lighter"); //Pizzaria
                LightDarkColors.Add(39, "lighter"); //Starlight
                LightDarkColors.Add(40, "lighter"); //Softball
                LightDarkColors.Add(41, "darker"); //Dark Jester
                LightDarkColors.Add(42, "darker"); //FRESH
                LightDarkColors.Add(43, "darker"); //Goner
                LightDarkColors.Add(44, "lighter"); //Psychic Friend
                LightDarkColors.Add(45, "lighter"); //Frost
                LightDarkColors.Add(46, "darker"); //Abyss Green
                LightDarkColors.Add(47, "darker"); //Midnight
                LightDarkColors.Add(48, "darker"); //<3
                LightDarkColors.Add(49, "lighter"); //Heat From Fire
                LightDarkColors.Add(50, "lighter"); //Fire From Heat
                LightDarkColors.Add(51, "lighter"); //Determination
                LightDarkColors.Add(52, "lighter"); //Patience
                LightDarkColors.Add(53, "darker"); //Bravery
                LightDarkColors.Add(54, "darker"); //Integrity
                LightDarkColors.Add(55, "darker"); //Perserverance
                LightDarkColors.Add(56, "darker"); //Kindness
                LightDarkColors.Add(57, "lighter"); //Bravery
                LightDarkColors.Add(58, "darker"); //Purple Plumber
                LightDarkColors.Add(59, "lighter"); //Rainbow

                var color = LightDarkColors[ColorID];

                if (color == "lighter")
                    PlayerName += " [L]";
                else if (color == "darker")
                    PlayerName += " [D]";
            }
            
            if (objectifier != null)
                PlayerName += $" {objectifier.GetColoredSymbol()}";

            foreach (var role in GetRoles(RoleEnum.GuardianAngel))
            {
                var ga = (GuardianAngel)role;

                if (Player == ga.target && Player == PlayerControl.LocalPlayer && CustomGameOptions.GATargetKnows)
                    PlayerName += "<color=#FFFFFFFF> ★</color>";
            }

            foreach (var role in GetRoles(RoleEnum.Executioner))
            {
                var exe = (Executioner)role;

                if (Player == exe.target && Player == PlayerControl.LocalPlayer && CustomGameOptions.ExeTargetKnows)
                    PlayerName += "<color=#CCCCCCFF> §</color>";
            }

            if (player != null && (MeetingHud.Instance.state == MeetingHud.VoteStates.Proceeding | MeetingHud.Instance.state ==
                MeetingHud.VoteStates.Results))
                return PlayerName;

            Player.nameText().transform.localPosition = new Vector3(0f, Player.CurrentOutfit.HatId == "hat_NoHat" ? 1.5f : 2.0f, -0.5f);

            return PlayerName + $"\n" + Name;
        }

        public static bool operator ==(Role a, Role b)
        {
            if (a is null && b is null)
                return true;

            if (a is null | b is null)
                return false;

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
                task.Text = $"{ColorString}Role: {Name}\nAlignment:{AlignmentName}\n{TaskText()}</color>";
                Player.myTasks.Insert(0, task);
                return;
            }

            Player.myTasks.ToArray()[0].Cast<ImportantTextTask>().Text = $"{ColorString}Role: {Name}\nAlignment:{AlignmentName}\n{TaskText()}</color>";
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
            if (player == null)
                return null;

            if (RoleDictionary.TryGetValue(player.PlayerId, out var role))
                return role;

            return null;
        }
        
        public static Role GetRoleValue(RoleEnum roleEnum)
        {
            foreach (var role in AllRoles)
            {
                if (role.RoleType == roleEnum)
                    return role;
            }

            return null;
        }
        
        public static T GetRoleValue<T>(RoleEnum roleEnum) where T : Role
        {
            return GetRoleValue(roleEnum) as T;
        }
        
        public static T GetRole<T>(PlayerControl player) where T : Role
        {
            return GetRole(player) as T;
        }

        public static Role GetRole(PlayerVoteArea area)
        {
            var player = PlayerControl.AllPlayerControls.ToArray().FirstOrDefault(x => x.PlayerId == area.TargetPlayerId);
            return player == null ? null : GetRole(player);
        }

        public static IEnumerable<Role> GetRoles(RoleEnum roletype)
        {
            return AllRoles.Where(x => x.RoleType == roletype);
        }

        public static IEnumerable<Role> GetFactions(Faction faction)
        {
            return AllRoles.Where(x => x.Faction == faction);
        }

        public static class IntroCutScenePatch
        {
            public static TextMeshPro StatusText;
            public static float Scale;

            [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.BeginCrewmate))]
            public static class IntroCutscene_BeginCrewmate
            {
                public static void Postfix(IntroCutscene __instance)
                {
                    var player = PlayerControl.LocalPlayer;
                    var modifier = Modifier.GetModifier(player);
                    var objectifier = Objectifier.GetObjectifier(player);
                    var ability = Ability.GetAbility(player);
                    var flag = modifier != null && ability != null && objectifier != null;

                    if (flag)
                        StatusText = Object.Instantiate(__instance.RoleText, __instance.RoleText.transform.parent, false);
                    else
                        StatusText = null;
                }
            }

            [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.BeginImpostor))]
            public static class IntroCutscene_BeginImpostor
            {
                public static void Postfix(IntroCutscene __instance)
                {
                    var player = PlayerControl.LocalPlayer;
                    var modifier = Modifier.GetModifier(player);
                    var objectifier = Objectifier.GetObjectifier(player);
                    var ability = Ability.GetAbility(player);
                    var flag = modifier != null | ability != null | objectifier != null;

                    if (flag)
                        StatusText = Object.Instantiate(__instance.RoleText, __instance.RoleText.transform.parent, false);
                    else
                        StatusText = null;
                }
            }
            
            [HarmonyPatch(typeof(IntroCutscene._CoBegin_d__19), nameof(IntroCutscene._CoBegin_d__19.MoveNext))]
            public static class IntroCutscene_CoBegin_d__19
            {
                public static void Postfix(IntroCutscene._CoBegin_d__19 __instance)
                {
                    var role = GetRole(PlayerControl.LocalPlayer);

                    if (role != null)
                    {
                        __instance.__4__this.TeamTitle.text = role.FactionName;
                        __instance.__4__this.TeamTitle.color = role.FactionColor;
                        __instance.__4__this.TeamTitle.outlineColor = new Color32(0, 0, 0, 255);
                        __instance.__4__this.RoleText.text = role.Name;
                        __instance.__4__this.RoleText.color = role.Color;
                        __instance.__4__this.RoleText.outlineColor = role.FactionColor;
                        __instance.__4__this.YouAreText.color = role.Color;
                        __instance.__4__this.RoleBlurbText.color = role.Color;
                        __instance.__4__this.BackgroundBar.material.color = role.Color;
                        SoundManager.Instance.PlaySound(role.IntroSound, false, 0.4f);
                    }

                    if (StatusText != null)
                    {
                        var player = PlayerControl.LocalPlayer;
                        var modifier = Modifier.GetModifier(player);
                        var objectifier = Objectifier.GetObjectifier(player);
                        var ability = Ability.GetAbility(player);
                        var statusString = "Status:";

                        StatusText.color = Colors.Status;

                        if (modifier != null)
                            statusString += " <color=#" + modifier.Color.ToHtmlStringRGBA() + ">" + modifier.Name + "</color>,";

                        if (objectifier != null)
                            statusString += " <color=#" + objectifier.Color.ToHtmlStringRGBA() + ">" + objectifier.Name + "</color>,";

                        if (ability != null)
                            statusString += " <color=#" + ability.Color.ToHtmlStringRGBA() + ">" + ability.Name + "</color>";
                        
                        if (ability == null)
                            statusString = statusString.Remove(statusString.Length - 1);

                        StatusText.text = "<size=4>" + statusString + "</size>";

                        StatusText.transform.position = __instance.__4__this.transform.position - new Vector3(0f, 1.6f, 0f);
                        StatusText.gameObject.SetActive(true);
                    }
                }
            }

            [HarmonyPatch(typeof(IntroCutscene._ShowTeam_d__21), nameof(IntroCutscene._ShowTeam_d__21.MoveNext))]
            public static class IntroCutscene_ShowTeam__d_21
            {
                public static void Prefix(IntroCutscene._ShowTeam_d__21 __instance)
                {
                    var role = GetRole(PlayerControl.LocalPlayer);

                    if (role != null)
                        role.IntroPrefix(__instance);
                }

                public static void Postfix(IntroCutscene._ShowRole_d__24 __instance)
                {
                    var role = GetRole(PlayerControl.LocalPlayer);

                    if (role != null)
                    {
                        __instance.__4__this.TeamTitle.text = role.FactionName;
                        __instance.__4__this.TeamTitle.color = role.FactionColor;
                        __instance.__4__this.TeamTitle.outlineColor = new Color32(0, 0, 0, 255);
                        __instance.__4__this.RoleText.text = role.Name;
                        __instance.__4__this.RoleText.color = role.Color;
                        __instance.__4__this.RoleText.outlineColor = role.FactionColor;
                        __instance.__4__this.YouAreText.color = role.Color;
                        __instance.__4__this.RoleBlurbText.color = role.Color;
                        __instance.__4__this.BackgroundBar.material.color = role.Color;
                        SoundManager.Instance.PlaySound(role.IntroSound, false, 0.4f);
                    }

                    if (StatusText != null)
                    {
                        var player = PlayerControl.LocalPlayer;
                        var modifier = Modifier.GetModifier(player);
                        var objectifier = Objectifier.GetObjectifier(player);
                        var ability = Ability.GetAbility(player);
                        var statusString = "Status:";

                        StatusText.color = Colors.Status;

                        if (modifier != null)
                            statusString += " <color=#" + modifier.Color.ToHtmlStringRGBA() + ">" + modifier.Name + "</color>,";

                        if (objectifier != null)
                            statusString += " <color=#" + objectifier.Color.ToHtmlStringRGBA() + ">" + objectifier.Name + "</color>,";

                        if (ability != null)
                            statusString += " <color=#" + ability.Color.ToHtmlStringRGBA() + ">" + ability.Name + "</color>";
                        
                        if (ability == null)
                            statusString = statusString.Remove(statusString.Length - 1);

                        StatusText.text = "<size=4>" + statusString + "</size>";

                        StatusText.transform.position = __instance.__4__this.transform.position - new Vector3(0f, 1.6f, 0f);
                        StatusText.gameObject.SetActive(true);
                    }
                }
            }

            [HarmonyPatch(typeof(IntroCutscene._ShowRole_d__24), nameof(IntroCutscene._ShowRole_d__24.MoveNext))]
            public static class IntroCutscene_ShowRole_d__24
            {
                public static void Postfix(IntroCutscene._ShowRole_d__24 __instance)
                {
                    var role = GetRole(PlayerControl.LocalPlayer);

                    if (role != null)
                    {
                        __instance.__4__this.TeamTitle.text = role.FactionName;
                        __instance.__4__this.TeamTitle.color = role.FactionColor;
                        __instance.__4__this.TeamTitle.outlineColor = new Color32(0, 0, 0, 255);
                        __instance.__4__this.RoleText.text = role.Name;
                        __instance.__4__this.RoleText.color = role.Color;
                        __instance.__4__this.RoleText.outlineColor = role.FactionColor;
                        __instance.__4__this.YouAreText.color = role.Color;
                        __instance.__4__this.RoleBlurbText.color = role.Color;
                        __instance.__4__this.BackgroundBar.material.color = role.Color;
                        SoundManager.Instance.PlaySound(role.IntroSound, false, 0.4f);
                    }

                    if (StatusText != null)
                    {
                        var player = PlayerControl.LocalPlayer;
                        var modifier = Modifier.GetModifier(player);
                        var objectifier = Objectifier.GetObjectifier(player);
                        var ability = Ability.GetAbility(player);
                        var statusString = "Status:";

                        StatusText.color = Colors.Status;

                        if (modifier != null)
                            statusString += " <color=#" + modifier.Color.ToHtmlStringRGBA() + ">" + modifier.Name + "</color>,";

                        if (objectifier != null)
                            statusString += " <color=#" + objectifier.Color.ToHtmlStringRGBA() + ">" + objectifier.Name + "</color>,";

                        if (ability != null)
                            statusString += " <color=#" + ability.Color.ToHtmlStringRGBA() + ">" + ability.Name + "</color>";
                        
                        if (ability == null)
                            statusString = statusString.Remove(statusString.Length - 1);

                        StatusText.text = "<size=4>" + statusString + "</size>";

                        StatusText.transform.position = __instance.__4__this.transform.position - new Vector3(0f, 1.6f, 0f);
                        StatusText.gameObject.SetActive(true);
                    }
                }
            }
        }

        [HarmonyPatch(typeof(PlayerControl._CoSetTasks_d__110), nameof(PlayerControl._CoSetTasks_d__110.MoveNext))]
        public static class PlayerControl_SetTasks
        {
            public static void Postfix(PlayerControl._CoSetTasks_d__110 __instance)
            {
                if (__instance == null)
                    return;

                var player = __instance.__4__this;
                var role = GetRole(player);
                var modifier = Modifier.GetModifier(player);
                var objectifier = Objectifier.GetObjectifier(player);
                var ability = Ability.GetAbility(player);

                if (modifier != null)
                {
                    var modTask = new GameObject(modifier.Name + "Task").AddComponent<ImportantTextTask>();
                    modTask.transform.SetParent(player.transform, false);
                    modTask.Text = $"{modifier.ColorString}Modifier: {modifier.Name}\n{modifier.TaskText()}</color>";
                    player.myTasks.Insert(0, modTask);
                }

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
                if (!AmongUsClient.Instance.AmHost)
                    return false;

                if (__instance.Systems.ContainsKey(SystemTypes.LifeSupp))
                {
                    var lifeSuppSystemType = __instance.Systems[SystemTypes.LifeSupp].Cast<LifeSuppSystemType>();

                    if (lifeSuppSystemType.Countdown < 0f)
                        return true;
                }

                if (__instance.Systems.ContainsKey(SystemTypes.Laboratory))
                {
                    var reactorSystemType = __instance.Systems[SystemTypes.Laboratory].Cast<ReactorSystemType>();

                    if (reactorSystemType.Countdown < 0f)
                        return true;
                }

                if (__instance.Systems.ContainsKey(SystemTypes.Reactor))
                {
                    var reactorSystemType = __instance.Systems[SystemTypes.Reactor].Cast<ICriticalSabotage>();

                    if (reactorSystemType.Countdown < 0f)
                        return true;
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
                        traitorIsEnd = SetTraitor.WillBeTraitor.Data.IsDead | SetTraitor.WillBeTraitor.Data.Disconnected |
                            alives.Count < CustomGameOptions.LatestSpawn | impsAlive.Count * 2 >= alives.Count;
                    }

                    if (modifier != null)
                        modifierIsEnd = modifier.EABBNOODFGL(__instance);

                    if (!roleIsEnd | !modifierIsEnd | !traitorIsEnd)
                        result = false;
                }

                if (!NobodyEndCriteria(__instance))
                    result = false;

                return result;
            }
        }

        [HarmonyPatch(typeof(LobbyBehaviour), nameof(LobbyBehaviour.Start))]
        public static class LobbyBehaviour_Start
        {
            private static void Postfix(LobbyBehaviour __instance)
            {
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

                foreach (var role in AllRoles.Where(x => x.RoleType == RoleEnum.Coroner))
                {
                    ((Coroner)role).BodyArrows.Values.DestroyAll();
                    ((Coroner)role).BodyArrows.Clear();
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
                Modifier.ModifierHistory.Clear();
                Objectifier.ObjectifierDictionary.Clear();
                Objectifier.ObjectifierHistory.Clear();
                Ability.AbilityDictionary.Clear();
                Ability.AbilityHistory.Clear();
            }
        }

        [HarmonyPatch(typeof(TranslationController), nameof(TranslationController.GetString), typeof(StringNames),
            typeof(Il2CppReferenceArray<Il2CppSystem.Object>))]
        public static class TranslationController_GetString
        {
            public static void Postfix(ref string __result, [HarmonyArgument(0)] StringNames name)
            {
                if (ExileController.Instance == null || ExileController.Instance.exiled == null)
                    return;

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
                            var roleName = role.RoleType == RoleEnum.Glitch ? role.Name : $"The {role.Name}";
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

                    if (role != null)
                    {
                        player.NameText.text = role.NameText(player);
                        player.NameText.color = role.Color;
                    }
                    else
                        player.NameText.text = role.Player.GetDefaultOutfit().PlayerName;
                }
            }

            [HarmonyPriority(Priority.First)]
            private static void Postfix(HudManager __instance)
            {
                if (MeetingHud.Instance != null)
                    UpdateMeeting(MeetingHud.Instance);

                if (PlayerControl.AllPlayerControls.Count <= 1)
                    return;

                if (PlayerControl.LocalPlayer == null)
                    return;

                if (PlayerControl.LocalPlayer.Data == null)
                    return;

                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (!(player.Data != null && player.Data.IsImpostor() && PlayerControl.LocalPlayer.Data.IsImpostor()))
                    {
                        player.nameText().text = player.name;
                        player.nameText().color = new Color32(255, 255, 255, 255);
                    }

                    var role = GetRole(player);

                    if (role != null)
                    {
                        player.nameText().text = role.NameText();
                        player.nameText().color = role.Color;
                    }

                    if (player.Data != null && PlayerControl.LocalPlayer.Data.IsImpostor() && player.Data.IsImpostor())
                        continue;
                }
            }
        }
    }
}
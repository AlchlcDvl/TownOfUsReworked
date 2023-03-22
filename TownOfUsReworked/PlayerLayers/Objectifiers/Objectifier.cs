using System;
using System.Collections.Generic;
using System.Linq;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Enums;
using Hazel;
using HarmonyLib;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.Patches;
using TownOfUsReworked.CustomOptions;

namespace TownOfUsReworked.PlayerLayers.Objectifiers
{
    public abstract class Objectifier : PlayerLayer
    {
        public static readonly Dictionary<byte, Objectifier> ObjectifierDictionary = new Dictionary<byte, Objectifier>();
        public static IEnumerable<Objectifier> AllObjectifiers => ObjectifierDictionary.Values.ToList();

        public static bool NobodyWins;

        public static bool LoveWins;
        public static bool RivalWins;
        public static bool TaskmasterWins;
        public static bool CorruptedWins;
        public static bool OverlordWins;

        protected Objectifier(PlayerControl player) : base(player)
        {
            Player = player;

            if (ObjectifierDictionary.ContainsKey(player.PlayerId))
                ObjectifierDictionary.Remove(player.PlayerId);

            ObjectifierDictionary.Add(player.PlayerId, this);
            Color = Colors.Objectifier;
        }

        protected internal ObjectifierEnum ObjectifierType = ObjectifierEnum.None;
        protected internal string SymbolName = ":";
        protected internal string TaskText = "- None.";
        protected internal bool Hidden = false;
        protected internal bool Winner = false;

        protected internal string GetColoredSymbol() => $"{ColorString}{SymbolName}</color>";

        private bool Equals(Objectifier other) => Equals(Player, other.Player) && ObjectifierType == other.ObjectifierType;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            if (obj.GetType() != typeof(Objectifier))
                return false;

            return Equals((Objectifier)obj);
        }

        public override int GetHashCode() => HashCode.Combine(Player, (int)ObjectifierType);

        public static bool operator == (Objectifier a, Objectifier b)
        {
            if (a is null && b is null)
                return true;

            if (a is null || b is null)
                return false;

            return a.ObjectifierType == b.ObjectifierType && a.Player.PlayerId == b.Player.PlayerId;
        }

        public static Objectifier GetObjectifierValue(ObjectifierEnum objEnum)
        {
            foreach (var obj in AllObjectifiers)
            {
                if (obj.ObjectifierType == objEnum)
                    return obj;
            }

            return null;
        }

        public static T GetObjectifierValue<T>(ObjectifierEnum objEnum) where T : Objectifier => GetObjectifierValue(objEnum) as T;

        public static bool operator != (Objectifier a, Objectifier b) => !(a == b);

        public static Objectifier GetObjectifier(PlayerControl player) => AllObjectifiers.FirstOrDefault(x => x.Player == player);

        public static T GetObjectifier<T>(PlayerControl player) where T : Objectifier => GetObjectifier(player) as T;

        public static Objectifier GetObjectifier(PlayerVoteArea area) => GetObjectifier(Utils.PlayerByVoteArea(area));

        public static IEnumerable<Objectifier> GetObjectifiers(ObjectifierEnum objectifiertype) => AllObjectifiers.Where(x => x.ObjectifierType == objectifiertype);

        public static T GenObjectifier<T>(Type type, PlayerControl player, int id, List<PlayerControl> players = null)
        {
            var objectifier = (T)((object)Activator.CreateInstance(type, new object[] { player }));
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetObjectifier, SendOption.Reliable);
            writer.Write(player.PlayerId);
            writer.Write(id);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            return objectifier;
        }

        internal override bool GameEnd(LogicGameFlowNormal __instance)
        {
            if (Player.Data.IsDead || Player.Data.Disconnected)
                return true;

            if (Utils.CorruptedWin(Player))
            {
                CorruptedWins = true;
                
                if (CustomGameOptions.AllCorruptedWin)
                {
                    foreach (Corrupted corr in Objectifier.GetObjectifiers(ObjectifierEnum.Corrupted))
                        corr.Winner = true;
                }
                else
                    Winner = true;

                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                writer.Write((byte)WinLoseRPC.CorruptedWin);
                writer.Write(CustomGameOptions.AllCorruptedWin);
                writer.Write(Player.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }
            else if (Utils.LoversWin(Player))
            {
                LoveWins = true;
                Winner = true;
                GetObjectifier(((Lovers)this).OtherLover).Winner = true;
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.WinLose, SendOption.Reliable);
                writer.Write((byte)WinLoseRPC.LoveWin);
                writer.Write(Player.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }
            else if (Utils.RivalsWin(Player))
            {
                RivalWins = true;
                Winner = true;
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                writer.Write((byte)WinLoseRPC.RivalWin);
                writer.Write(Player.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }
            else if (TasksDone && ObjectifierType == ObjectifierEnum.Taskmaster)
            {
                TaskmasterWins = true;
                Winner = true;
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                writer.Write((byte)WinLoseRPC.TaskmasterWin);
                writer.Write(Player.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }
            else if (MeetingPatches.MeetingCount >= CustomGameOptions.OverlordMeetingWinCount && ObjectifierType == ObjectifierEnum.Overlord)
            {
                OverlordWins = true;

                foreach (Overlord ov in GetObjectifiers(ObjectifierEnum.Overlord))
                    ov.Winner = true;

                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                writer.Write((byte)WinLoseRPC.OverlordWin);
                writer.Write(Player.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }

            var flag = Player.Is(ObjectifierEnum.Corrupted) || Player.Is(ObjectifierEnum.Allied) || Player.Is(ObjectifierEnum.Overlord) || (Player.Is(ObjectifierEnum.Lovers) && 
                ((Lovers)this).LoversAlive()) || (Player.Is(ObjectifierEnum.Rivals) && ((Rivals)this).RivalDead());
            return !flag;
        }

        [HarmonyPatch]
        public static class CheckEndGame
        {
            [HarmonyPatch(typeof(LogicGameFlowNormal), nameof(LogicGameFlowNormal.CheckEndCriteria))]
            public static bool Prefix(LogicGameFlowNormal __instance)
            {
                if (GameStates.IsHnS)
                    return true;

                if (!AmongUsClient.Instance.AmHost)
                    return false;

                if (Utils.NoOneWins())
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                    writer.Write((byte)WinLoseRPC.NobodyWins);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Utils.EndGame();
                    Role.NobodyWins = true;
                    Objectifier.NobodyWins = true;
                    return true;
                }
                else
                {
                    foreach (var obj in AllObjectifiers)
                    {
                        if (!obj.GameEnd(__instance))
                            return false;
                    }
                }

                return Utils.GameHasEnded();
            }
        }
    }
}
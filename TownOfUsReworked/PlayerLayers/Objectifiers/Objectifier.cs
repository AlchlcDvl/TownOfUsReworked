using System.Collections.Generic;
using System.Linq;
using TownOfUsReworked.Classes;
using Hazel;
using HarmonyLib;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.Patches;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Objectifiers
{
    [HarmonyPatch]
    public class Objectifier : PlayerLayer
    {
        public static readonly List<Objectifier> AllObjectifiers = new();

        #pragma warning disable
        public static bool NobodyWins;

        public static bool LoveWins;
        public static bool RivalWins;
        public static bool TaskmasterWins;
        public static bool CorruptedWins;
        public static bool OverlordWins;
        #pragma warning restore

        public static bool ObjectifierWins => LoveWins || RivalWins || TaskmasterWins || CorruptedWins || OverlordWins;

        protected Objectifier(PlayerControl player) : base(player)
        {
            Color = Colors.Objectifier;
            LayerType = PlayerLayerEnum.Objectifier;
            AllObjectifiers.Add(this);
        }

        public string SymbolName = ":";
        public string TaskText = "- None.";
        public bool Hidden;
        public bool Winner;

        public string ColoredSymbol => $"{ColorString}{SymbolName}</color>";

        public static Objectifier GetObjectifier(PlayerControl player) => AllObjectifiers.Find(x => x.Player == player);

        public static T GetObjectifier<T>(PlayerControl player) where T : Objectifier => GetObjectifier(player) as T;

        public static Objectifier GetObjectifier(PlayerVoteArea area) => GetObjectifier(Utils.PlayerByVoteArea(area));

        public static List<Objectifier> GetObjectifiers(ObjectifierEnum objectifiertype) => AllObjectifiers.Where(x => x.ObjectifierType == objectifiertype).ToList();

        public static List<T> GetObjectifiers<T>(ObjectifierEnum objectifiertype) where T : Objectifier => GetObjectifiers(objectifiertype).Cast<T>().ToList();

        public override bool GameEnd(LogicGameFlowNormal __instance)
        {
            if (Player.Data.IsDead || Player.Data.Disconnected)
                return true;

            if (ConstantVariables.CorruptedWin(Player))
            {
                CorruptedWins = true;

                if (CustomGameOptions.AllCorruptedWin)
                {
                    foreach (var corr in GetObjectifiers<Corrupted>(ObjectifierEnum.Corrupted))
                        corr.Winner = true;
                }

                Winner = true;
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                writer.Write((byte)WinLoseRPC.CorruptedWin);
                writer.Write(CustomGameOptions.AllCorruptedWin);
                writer.Write(Player.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }
            else if (ConstantVariables.LoversWin(Player))
            {
                LoveWins = true;
                Winner = true;
                GetObjectifier(((Lovers)this).OtherLover).Winner = true;
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                writer.Write((byte)WinLoseRPC.LoveWin);
                writer.Write(Player.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }
            else if (ConstantVariables.RivalsWin(Player))
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
            else if (MeetingPatches.MeetingCount >= CustomGameOptions.OverlordMeetingWinCount && ObjectifierType == ObjectifierEnum.Overlord && ((Overlord)this).IsAlive)
            {
                OverlordWins = true;

                foreach (var ov in GetObjectifiers<Overlord>(ObjectifierEnum.Overlord))
                {
                    if (ov.IsAlive)
                        ov.Winner = true;
                }

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
                if (ConstantVariables.IsHnS)
                    return true;

                if (!AmongUsClient.Instance.AmHost)
                    return false;

                if (ConstantVariables.NoOneWins)
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                    writer.Write((byte)WinLoseRPC.NobodyWins);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Utils.EndGame();
                    Role.NobodyWins = true;
                    NobodyWins = true;
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

                return ConstantVariables.GameHasEnded;
            }
        }
    }
}
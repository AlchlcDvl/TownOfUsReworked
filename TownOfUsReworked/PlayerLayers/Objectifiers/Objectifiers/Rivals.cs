using System.Collections.Generic;
using System.Linq;
using Hazel;
using TownOfUsReworked.Patches;
using UnityEngine;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Enums;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Objectifiers.Objectifiers
{
    public class Rivals : Objectifier
    {
        public Rivals OtherRival { get; set; }
        public PlayerControl OtherRivalPlayer { get; set; }
        public bool RivalWins { get; set; }
        public int Num { get; set; }

        public Rivals(PlayerControl player) : base(player)
        {
            Name = "Rival";
            SymbolName = "❧";
            TaskText = () => "Your Rival is " + OtherRival.Player.name;
            Color = CustomGameOptions.CustomObjectifierColors ? Colors.Rivals : Colors.Objectifier;
            ObjectifierType = ObjectifierEnum.Rivals;
            AddToObjectifierHistory(ObjectifierType);
        }

        public List<PlayerControl> GetTeammates()
        {
            var rivalTeam = new List<PlayerControl>
            {
                PlayerControl.LocalPlayer,
                OtherRival.Player
            };

            return rivalTeam;
        }

        public static void Gen(List<PlayerControl> canHaveObjectifiers)
        {
            List<PlayerControl> all = new List<PlayerControl>();

            foreach(var player in canHaveObjectifiers)
                all.Add(player);

            if (all.Count < 3)
                return;

            all.Shuffle();

            PlayerControl firstLover;
            var num = Random.RandomRangeInt(0, all.Count);
            firstLover = all[num];
            canHaveObjectifiers.Remove(firstLover);

            PlayerControl secondLover;
            var num2 = Random.RandomRangeInt(0, all.Count);
            secondLover = all[num2];
            canHaveObjectifiers.Remove(secondLover);

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.SetCouple,
                SendOption.Reliable, -1);
            writer.Write(firstLover.PlayerId);
            writer.Write(secondLover.PlayerId);
            var lover1 = new Lovers(firstLover);
            var lover2 = new Lovers(secondLover);

            lover1.OtherLover = lover2;
            lover2.OtherLover = lover1;

            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }

        internal override bool EABBNOODFGL(ShipStatus __instance)
        {
            if (FourPeopleLeft())
                return false;

            if (CheckLoversWin())
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.LoveWin,
                    SendOption.Reliable, -1);
                writer.Write(Player.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Win();
                Utils.EndGame();
                return false;
            }

            return true;
        }

        private bool FourPeopleLeft()
        {
            var players = PlayerControl.AllPlayerControls.ToArray();
            var alives = players.Where(x => !x.Data.IsDead).ToList();
            var lover1 = Player;
            var lover2 = OtherRival.Player;
            
            return !lover1.Data.IsDead && !lover1.Data.Disconnected && !lover2.Data.IsDead && !lover2.Data.Disconnected && alives.Count() == 4 &&
                (lover1.Is(Faction.Intruders) | lover2.Is(Faction.Intruders));
        }

        private bool CheckLoversWin()
        {
            //System.Console.WriteLine("CHECKWIN");
            var players = PlayerControl.AllPlayerControls.ToArray();
            var alives = players.Where(x => !x.Data.IsDead).ToList();
            var lover1 = Player;
            var lover2 = OtherRival.Player;

            return !lover1.Data.IsDead && !lover1.Data.Disconnected && !lover2.Data.IsDead && !lover2.Data.Disconnected && (alives.Count == 3) |
                (alives.Count == 2);
        }

        public void Win()
        {
            if (Role.GetRoles(RoleEnum.Taskmaster).Any(x => ((Taskmaster)x).WinTasksDone))
                return;

            if (Role.GetRoles(RoleEnum.Cannibal).Any(x => ((Cannibal)x).EatNeed == 0))
                return;

            if (Objectifier.GetObjectifiers(ObjectifierEnum.Phantom).Any(x => ((Phantom)x).CompletedTasks))
                return;

            if (Objectifier.GetObjectifiers(ObjectifierEnum.Lovers).Any(x => ((Lovers)x).LoveCoupleWins))
                return;

            RivalWins = true;
            OtherRival.RivalWins = true;
        }
    }
}
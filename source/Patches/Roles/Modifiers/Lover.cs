using System.Collections.Generic;
using System.Linq;
using Hazel;
using TownOfUs.Patches;
using UnityEngine;

namespace TownOfUs.Roles.Modifiers
{
    public class Lover : Modifier
    {
        public Lover(PlayerControl player) : base(player)
        {
            Name = "Lover";
            SymbolName = "♥";
            TaskText = () => "You are in Love with " + OtherLover.Player.name;
            Color = Colors.Lovers;
            ModifierType = ModifierEnum.Lover;
        }

        public Lover OtherLover { get; set; }
        public bool LoveCoupleWins { get; set; }
        public int Num { get; set; }

        public override List<PlayerControl> GetTeammates()
        {
            var loverTeam = new List<PlayerControl>
            {
                PlayerControl.LocalPlayer,
                OtherLover.Player
            };
            return loverTeam;
        }

        public static void Gen(List<PlayerControl> canHaveModifiers)
        {
            List<PlayerControl> all = new List<PlayerControl>();

            foreach(var player in canHaveModifiers)
                all.Add(player);

            if (all.Count < 3) return;

            all.Shuffle();

            PlayerControl firstLover;
            var num = Random.RandomRangeInt(0, all.Count);
            firstLover = all[num];
            canHaveModifiers.Remove(firstLover);

            PlayerControl secondLover;
            var num2 = Random.RandomRangeInt(0, all.Count);
            secondLover = all[num2];
            canHaveModifiers.Remove(secondLover);

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.SetCouple, SendOption.Reliable, -1);
            writer.Write(firstLover.PlayerId);
            writer.Write(secondLover.PlayerId);
            var lover1 = new Lover(firstLover);
            var lover2 = new Lover(secondLover);

            lover1.OtherLover = lover2;
            lover2.OtherLover = lover1;

            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }

        internal override bool EABBNOODFGL(ShipStatus __instance)
        {
            if (FourPeopleLeft()) return false;

            if (CheckLoversWin())
            {
                //System.Console.WriteLine("LOVERS WIN");
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                    (byte) CustomRPC.LoveWin, SendOption.Reliable, -1);
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
            var lover2 = OtherLover.Player;
            
            return !lover1.Data.IsDead && !lover1.Data.Disconnected && !lover2.Data.IsDead && !lover2.Data.Disconnected && alives.Count() == 4 && (lover1.Is(Faction.Intruders) | lover2.Is(Faction.Intruders));
        }

        private bool CheckLoversWin()
        {
            //System.Console.WriteLine("CHECKWIN");
            var players = PlayerControl.AllPlayerControls.ToArray();
            var alives = players.Where(x => !x.Data.IsDead).ToList();
            var lover1 = Player;
            var lover2 = OtherLover.Player;

            return !lover1.Data.IsDead && !lover1.Data.Disconnected && !lover2.Data.IsDead && !lover2.Data.Disconnected && (alives.Count == 3) | (alives.Count == 2);
        }

        public void Win()
        {
            if (Role.AllRoles.Where(x => x.RoleType == RoleEnum.Jester).Any(x => ((Jester) x).VotedOut)) return;
            LoveCoupleWins = true;
            OtherLover.LoveCoupleWins = true;
        }
    }
}
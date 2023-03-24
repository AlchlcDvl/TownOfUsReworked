using HarmonyLib;
using System.Linq;
using Hazel;
using UnityEngine;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.PhantomMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class RepickPhantom
    {
        public static void Postfix()
        {
            if (PlayerControl.AllPlayerControls.Count <= 1 || PlayerControl.LocalPlayer == null || PlayerControl.LocalPlayer.Data == null)
                return;

            if (PlayerControl.LocalPlayer != SetPhantom.WillBePhantom || PlayerControl.LocalPlayer.Data.IsDead)
                return;

            var toChooseFrom = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Neutral) && x.Data.IsDead && !x.Data.Disconnected).ToList();

            if (toChooseFrom.Count == 0)
                return;

            var hasWon = true;

            while (hasWon)
            {
                var rand = Random.RandomRangeInt(0, toChooseFrom.Count);
                var pc = toChooseFrom[rand];
                SetPhantom.WillBePhantom = pc;

                var role = Role.GetRole(pc);

                if (role.Is(RoleEnum.Troll))
                    hasWon = ((Troll)role).Killed;
                else if (role.Is(RoleEnum.Jester))
                    hasWon = ((Jester)role).VotedOut;
                else if (role.Is(RoleEnum.Executioner))
                    hasWon = ((Executioner)role).TargetVotedOut;
                else if (role.Is(RoleEnum.BountyHunter))
                    hasWon = ((BountyHunter)role).TargetKilled;
                else if (role.Is(RoleEnum.Cannibal))
                    hasWon = ((Cannibal)role).EatWin;
                else if (role.Is(RoleEnum.Guesser))
                    hasWon = ((Guesser)role).TargetGuessed;
                else if (role.Is(RoleEnum.Actor))
                    hasWon = ((Actor)role).Guessed;
                else
                    hasWon = false;
            }

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetPhantom, SendOption.Reliable);
            writer.Write(SetPhantom.WillBePhantom.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }
    }
}
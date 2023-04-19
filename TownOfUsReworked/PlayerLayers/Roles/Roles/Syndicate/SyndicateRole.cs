using Il2CppSystem.Collections.Generic;
using TownOfUsReworked.Classes;
using Hazel;
using System;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Data;
using TownOfUsReworked.Custom;
using System.Linq;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public abstract class SyndicateRole : Role
    {
        public DateTime LastKilled;
        public PlayerControl ClosestPlayer;
        public CustomButton KillButton;
        public bool HoldsDrive => Player == DriveHolder || (CustomGameOptions.GlobalDrive && SyndicateHasChaosDrive);

        protected SyndicateRole(PlayerControl player) : base(player)
        {
            Faction = Faction.Syndicate;
            FactionColor = Colors.Syndicate;
            Color = Colors.Syndicate;
            Objectives = SyndicateWinCon;
            BaseFaction = Faction.Syndicate;
            AbilitiesText = (RoleType is not RoleEnum.Anarchist and not RoleEnum.Sidekick && RoleAlignment != RoleAlignment.SyndicateKill ? "- With the Chaos Drive, you can kill " +
                "players directly" : "- You can kill") + (CustomGameOptions.AltImps ? "- You can sabotage the systems to distract the <color=#8BFDFDFF>Crew</color>" : "");
            KillButton = new(this, AssetManager.SyndicateKill, AbilityTypes.Direct, "ActionSecondary", Kill);
        }

        public float KillTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastKilled;
            var num = Player.GetModifiedCooldown(CustomGameOptions.ChaosDriveKillCooldown + (!HoldsDrive && RoleType is RoleEnum.Anarchist
                ? CustomGameOptions.ChaosDriveCooldownDecrease : 0)) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public override void IntroPrefix(IntroCutscene._ShowTeam_d__36 __instance)
        {
            if (Player != PlayerControl.LocalPlayer)
                return;

            var team = new List<PlayerControl>();
            team.Add(PlayerControl.LocalPlayer);

            if (IsRecruit)
            {
                var jackal = Player.GetJackal();

                team.Add(jackal.Player);
                team.Add(jackal.GoodRecruit);
            }

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player.Is(Faction) && player != PlayerControl.LocalPlayer)
                    team.Add(player);
            }

            if (Player.Is(ObjectifierEnum.Lovers))
                team.Add(Player.GetOtherLover());
            else if (Player.Is(ObjectifierEnum.Rivals))
                team.Add(Player.GetOtherRival());

            __instance.teamToShow = team;
        }

        public override bool GameEnd(LogicGameFlowNormal __instance)
        {
            if (Player.Data.IsDead || Player.Data.Disconnected)
                return true;

            if (IsRecruit && ConstantVariables.CabalWin)
            {
                CabalWin = true;
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                writer.Write((byte)WinLoseRPC.CabalWin);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }
            else if (IsPersuaded && ConstantVariables.SectWin)
            {
                SectWin = true;
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                writer.Write((byte)WinLoseRPC.SectWin);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }
            else if (IsBitten && ConstantVariables.UndeadWin)
            {
                UndeadWin = true;
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                writer.Write((byte)WinLoseRPC.UndeadWin);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }
            else if (IsResurrected && ConstantVariables.ReanimatedWin)
            {
                ReanimatedWin = true;
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                writer.Write((byte)WinLoseRPC.ReanimatedWin);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }
            else if (ConstantVariables.SyndicateWins && NotDefective)
            {
                SyndicateWin = true;
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                writer.Write((byte)WinLoseRPC.SyndicateWin);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }

            return false;
        }

        public void Kill()
        {
            if (Utils.IsTooFar(Player, ClosestPlayer) || KillTimer() != 0f)
                return;

            var interact = Utils.Interact(Player, ClosestPlayer, true);

            if (interact[3])
            {
                if (Player.Is(RoleEnum.Politician))
                    ((Politician)this).VoteBank++;
                else if (Player.Is(RoleEnum.PromotedRebel))
                {
                    var reb = (PromotedRebel)this;

                    if (reb.IsPol)
                        reb.VoteBank++;
                }
            }

            if (interact[0])
                LastKilled = DateTime.UtcNow;
            else if (interact[1])
                LastKilled.AddSeconds(CustomGameOptions.ProtectKCReset);
            else if (interact[2])
                LastKilled.AddSeconds(CustomGameOptions.VestKCReset);
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            var notSyn = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(Faction.Syndicate)).ToList();
            KillButton.Update("KILL", KillTimer(), CustomGameOptions.ChaosDriveKillCooldown, notSyn);
        }
    }
}
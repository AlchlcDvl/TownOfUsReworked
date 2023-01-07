using System;
using UnityEngine;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using Hazel;
using TownOfUsReworked.Patches;
using Il2CppSystem.Collections.Generic;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Concealer : Role
    {
        private KillButton _concealButton;
        public bool Enabled;
        public DateTime LastConcealed { get; set; }
        public float TimeRemaining;
        public bool Concealed => TimeRemaining > 0f;

        public Concealer(PlayerControl player) : base(player)
        {
            Name = "Concealer";
            StartText = "Make The <color=#8BFDFDFF>Crew</color> Invisible For Some Chaos";
            AbilitiesText = "- You can turn off player's ability to see things properly, making all other players appear invisible to themselves.";
            Color = IsRecruit ? Colors.Cabal : (CustomGameOptions.CustomSynColors ? Colors.Concealer : Colors.Syndicate);
            RoleType = RoleEnum.Concealer;
            Faction = Faction.Syndicate;
            FactionName = "Syndicate";
            FactionColor = Colors.Syndicate;
            RoleAlignment = RoleAlignment.SyndicateSupport;
            AlignmentName = "Syndicate (Support)";
            IntroText = SyndicateIntro;
            Results = InspResults.ConcealGorg;
            RoleDescription = "You are a Concealer! You can turn everyone invisible to everyone else but themselves by making them unable to see things properly. " +
                "Use this to get away from crime scenes as fast as possible!";
            AlignmentDescription = SSuDescription;
            FactionDescription = SyndicateFactionDescription;
            Objectives = IsRecruit ? JackalWinCon : SyndicateWinCon;
            AddToRoleHistory(RoleType);
        }

        public KillButton ConcealButton
        {
            get => _concealButton;
            set
            {
                _concealButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        public void Conceal()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;
            Utils.Conceal();
        }

        public void UnConceal()
        {
            Enabled = false;
            LastConcealed = DateTime.UtcNow;
            Utils.DefaultOutfitAll();
        }

        public float ConcealTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastConcealed;
            var num = CustomGameOptions.ConcealCooldown * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__21 __instance)
        {
            var team = new List<PlayerControl>();

            team.Add(PlayerControl.LocalPlayer);

            if (!IsRecruit)
            {
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (player.Is(Faction) && player != PlayerControl.LocalPlayer)
                        team.Add(player);
                }
            }
            else
            {
                var jackal = Player.GetJackal();

                team.Add(jackal.Player);
                team.Add(jackal.GoodRecruit);
            }

            __instance.teamToShow = team;
        }

        public override void Wins()
        {
            if (IsRecruit)
                CabalWin = true;
            else
                SyndicateWin = true;
        }

        public override void Loses()
        {
            LostByRPC = true;
        }

        internal override bool EABBNOODFGL(ShipStatus __instance)
        {
            if (Player.Data.IsDead || Player.Data.Disconnected)
                return true;

            if (IsRecruit)
            {
                if (Utils.CabalWin())
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.CabalWin,
                        SendOption.Reliable, -1);
                    writer.Write(Player.PlayerId);
                    Wins();
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Utils.EndGame();
                    return false;
                }
            }
            else if (Utils.SyndicateWins())
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SyndicateWin,
                    SendOption.Reliable, -1);
                writer.Write(Player.PlayerId);
                Wins();
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }

            return false;
        }
    }
}
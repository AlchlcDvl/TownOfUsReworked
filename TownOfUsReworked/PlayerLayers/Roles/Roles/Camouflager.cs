using System;
using UnityEngine;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Patches;
using Il2CppSystem.Collections.Generic;
using Hazel;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Camouflager : Role
    {
        private KillButton _camouflageButton;
        public bool Enabled;
        public DateTime LastCamouflaged { get; set; }
        public float TimeRemaining;
        public bool Camouflaged => TimeRemaining > 0f;

        public Camouflager(PlayerControl player) : base(player)
        {
            Name = "Camouflager";
            StartText = "Hinder The <color=#8BFDFDFF>Crew</color>'s Recognition";
            AbilitiesText = "- You can disrupt everyone's vision, causing them to be unable to tell players apart.";
            AttributesText = "- When camouflaged, everyone will appear grey with no name or cosmetics.\n- If the Colorblind Meeting options is on, this effect " +
                "\ncarries over into the meeting if a meeting is called during a camouflage.";
            Color = IsRecruit ? Colors.Cabal : (CustomGameOptions.CustomIntColors ? Colors.Camouflager : Colors.Intruder);
            RoleType = RoleEnum.Camouflager;
            Faction = Faction.Intruder;
            FactionName = "Intruder";
            FactionColor = Colors.Intruder;
            RoleAlignment = RoleAlignment.IntruderSupport;
            AlignmentName = "Intruder (Concealing)";
            Results = InspResults.DisgMorphCamoAgent;
            Objectives = IsRecruit ? JackalWinCon : IntrudersWinCon;
            AlignmentDescription = ICDescription;
            FactionDescription = IntruderFactionDescription;
            RoleDescription = "You are a Camouflager! You can choose to disrupt everyone's vision, causing them to be unable to recognise others. Use this to your " +
                "advantage and kill while unsuspected in front of everyone!";
            Attack = AttackEnum.Basic;
            AttackString = "Basic";
            AddToRoleHistory(RoleType);
        }

        public KillButton CamouflageButton
        {
            get => _camouflageButton;
            set
            {
                _camouflageButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        public void Camouflage()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;
            Utils.Camouflage();
        }

        public void UnCamouflage()
        {
            Enabled = false;
            LastCamouflaged = DateTime.UtcNow;
            Utils.DefaultOutfitAll();
        }

        public float CamouflageTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastCamouflaged;
            var num = CustomGameOptions.CamouflagerCd * 1000f;
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
                IntruderWin = true;
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
            else if (Utils.IntrudersWin())
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.IntruderWin,
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
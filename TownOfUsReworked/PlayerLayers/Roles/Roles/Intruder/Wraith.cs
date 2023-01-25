using System;
using TownOfUsReworked.Extensions;
using UnityEngine;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Patches;
using Il2CppSystem.Collections.Generic;
using Hazel;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Wraith : Role
    {
        private KillButton _invisButton;
        public bool Enabled;
        public DateTime LastInvis;
        public float TimeRemaining;
        public bool IsInvis => TimeRemaining > 0f;
        private KillButton _killButton;

        public Wraith(PlayerControl player) : base(player)
        {
            Name = "Wraith";
            StartText = "Sneaky Sneaky";
            AbilitiesText = "Turn invisible and kill undetected";
            Color = CustomGameOptions.CustomIntColors ? Colors.Wraith : Colors.Intruder;
            LastInvis = DateTime.UtcNow;
            RoleType = RoleEnum.Wraith;
            Faction = Faction.Intruder;
            FactionName = "Intruder";
            FactionColor = Colors.Intruder;
            RoleAlignment = RoleAlignment.IntruderConceal;
            AlignmentName = "Intruder (Concealing)";
            Results = InspResults.TeleWarpTransWraith;
        }

        public KillButton KillButton
        {
            get => _killButton;
            set
            {
                _killButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        public KillButton InvisButton
        {
            get => _invisButton;
            set
            {
                _invisButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        public float InvisTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastInvis;
            var num = CustomGameOptions.InvisCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        public void Invis()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;
            
            if (Player.Data.IsDead)
                TimeRemaining = 0f;

            var color = new Color32(0, 0, 0, 0);

            if (PlayerControl.LocalPlayer.Is(Faction) || PlayerControl.LocalPlayer.Data.IsDead)
                color.a = 26;

            if (Player.GetCustomOutfitType() != CustomPlayerOutfitType.Invis)
            {
                Player.SetOutfit(CustomPlayerOutfitType.Invis, new GameData.PlayerOutfit()
                {
                    ColorId = Player.CurrentOutfit.ColorId,
                    HatId = "",
                    SkinId = "",
                    VisorId = "",
                    PlayerName = " "
                });

                Player.myRend().color = color;
                Player.nameText().color = new Color32(0, 0, 0, 0);
                Player.cosmetics.colorBlindText.color = new Color32(0, 0, 0, 0);
            }
        }

        public void Uninvis()
        {
            Enabled = false;
            LastInvis = DateTime.UtcNow;
            Utils.DefaultOutfit(Player);
            Player.myRend().color = new Color32(255, 255, 255, 255);
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__32 __instance)
        {
            if (Player != PlayerControl.LocalPlayer)
                return;
                
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

        internal override bool GameEnd(LogicGameFlowNormal __instance)
        {
            if (Player.Data.IsDead || Player.Data.Disconnected)
                return true;

            if (IsRecruit)
            {
                if (Utils.CabalWin())
                {
                    Wins();
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable, -1);
                    writer.Write((byte)WinLoseRPC.CabalWin);
                    writer.Write(Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Utils.EndGame();
                    return false;
                }
            }
            else if (Utils.IntrudersWin())
            {
                Wins();
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable, -1);
                writer.Write((byte)WinLoseRPC.IntruderWin);
                writer.Write(Player.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }

            return false;
        }
    }
}
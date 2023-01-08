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
    public class Poisoner : Role
    {
        private KillButton _poisonButton;
        public PlayerControl ClosestPlayer;
        public DateTime LastPoisoned;
        public PlayerControl PoisonedPlayer;
        public float TimeRemaining;
        public bool Enabled = false;

        public Poisoner(PlayerControl player) : base(player)
        {
            Name = "Poisoner";
            StartText = "Poison A <color=#8BFDFDFF>Crewmate</color> To Kill Them Later";
            AbilitiesText = "Poison the <color=#8BFDFDFF>Crew</color>";
            Color = CustomGameOptions.CustomIntColors? Colors.Poisoner : Colors.Intruder;
            LastPoisoned = DateTime.UtcNow;
            RoleType = RoleEnum.Poisoner;
            Faction = Faction.Intruder;
            PoisonedPlayer = null;
            FactionName = "Intruder";
            FactionColor = Colors.Intruder;
            RoleAlignment = RoleAlignment.IntruderDecep;
            AlignmentName = "Intruder (Deception)";
            Results = InspResults.EscConsGliPois;
            Attack = AttackEnum.Basic;
            AttackString = "Basic";
            AddToRoleHistory(RoleType);
        }
        
        public KillButton PoisonButton
        {
            get => _poisonButton;
            set
            {
                _poisonButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        public bool Poisoned => TimeRemaining > 0f;
        
        public void Poison()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;

            if (MeetingHud.Instance)
                TimeRemaining = 0;
                
            if (TimeRemaining <= 0)
                PoisonKill();
        }

        public void PoisonKill()
        {
            if (!PoisonedPlayer.Is(RoleEnum.Pestilence))
            {
                Utils.RpcMurderPlayer(Player, PoisonedPlayer);

                if (!PoisonedPlayer.Data.IsDead)
                {
                    try
                    {
                        SoundManager.Instance.PlaySound(TownOfUsReworked.KillSFX, false, 1f);
                    } catch {}
                }
            }

            PoisonedPlayer = null;
            Enabled = false;
            LastPoisoned = DateTime.UtcNow;
        }
        
        public float PoisonTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastPoisoned;
            var num = CustomGameOptions.PoisonCd * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
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
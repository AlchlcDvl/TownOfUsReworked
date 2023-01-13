using System;
using TownOfUsReworked.Extensions;
using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Patches;
using TownOfUsReworked.PlayerLayers.Modifiers;
using Il2CppSystem.Collections.Generic;
using UnityEngine;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Disguiser : Role, IVisualAlteration
    {
        private KillButton _disguiseButton;
        public PlayerControl ClosestPlayer;
        public DateTime LastDisguised;
        public PlayerControl DisguisedPlayer;
        public PlayerControl MeasuredPlayer;
        public PlayerControl TargetPlayer;
        public PlayerControl disguised { get; private set; }
        public float TimeBeforeDisguised { get; private set; }
        public float DisguiseTimeRemaining { get; private set; }
        public float TimeRemaining;
        public bool Disguised => TimeRemaining > 0f;

        public Disguiser(PlayerControl player) : base(player)
        {
            Name = "Disguiser";
            StartText = "Disguise The <color=#8BFDFDFF>Crew</color> To Frame Them";
            AbilitiesText = "- You can disguise a player into someone else's appearance.";
            Color = IsRecruit ? Colors.Cabal : (CustomGameOptions.CustomIntColors ? Colors.Disguiser : Colors.Intruder);
            RoleType = RoleEnum.Disguiser;
            Faction = Faction.Intruder;
            FactionName = "Intruder";
            FactionColor = Colors.Intruder;
            RoleAlignment = RoleAlignment.IntruderDecep;
            AlignmentName = "Intruder (Deception)";
            Results = InspResults.DisgMorphCamoAgent;
            FactionDescription = IntruderFactionDescription;
            Objectives = IsRecruit ? JackalWinCon : IntrudersWinCon;
            Attack = AttackEnum.Basic;
            AttackString = "Basic";
            AlignmentDescription = IDDescription;
            RoleDescription = "You are a Disguiser! Cause some chaos by changing people's appearances and fooling everyone around you.";
        }

        public KillButton DisguiseButton
        {
            get => _disguiseButton;
            set
            {
                _disguiseButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        public void Disguise()
        {
            TimeRemaining -= Time.deltaTime;
            Utils.Morph(DisguisedPlayer, Player);

            if (Player.Data.IsDead)
                TimeRemaining = 0f;
        }

        public void Undisguise()
        {
            DisguisedPlayer = null;
            Utils.DefaultOutfit(DisguisedPlayer);
            LastDisguised = DateTime.UtcNow;
        }

        public float DisguiseTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastDisguised;
            var num = CustomGameOptions.DisguiseCooldown * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        public bool TryGetModifiedAppearance(out VisualAppearance appearance)
        {
            if (Disguised)
            {
                appearance = DisguisedPlayer.GetDefaultAppearance();
                var modifier = Modifier.GetModifier(DisguisedPlayer);

                if (modifier is IVisualAlteration alteration)
                    alteration.TryGetModifiedAppearance(out appearance);
                
                return true;
            }

            appearance = Player.GetDefaultAppearance();
            return false;
        }

        public void StartDisguise(PlayerControl target)
        {
            disguised = target;
            TimeBeforeDisguised = CustomGameOptions.TimeToDisguise;
        }

        public void DisguiseTick()
        {
            if (disguised == null)
                return;

            if (TimeBeforeDisguised > 0)
            {
                TimeBeforeDisguised = Math.Clamp(TimeBeforeDisguised - Time.deltaTime, 0, TimeBeforeDisguised);
                
                if (TimeBeforeDisguised <= 0f)
                    DisguiseTimeRemaining = CustomGameOptions.DisguiseDuration;
            }
            else if (DisguiseTimeRemaining > 0)
            {
                DisguiseTimeRemaining -= Time.deltaTime;
                Disguise();
            }
            else
                Undisguise();
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

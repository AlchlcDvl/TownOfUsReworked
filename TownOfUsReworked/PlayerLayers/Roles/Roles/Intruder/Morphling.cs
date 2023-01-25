using System;
using TownOfUsReworked.Extensions;
using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Patches;
using TownOfUsReworked.PlayerLayers.Modifiers;
using UnityEngine;
using Il2CppSystem.Collections.Generic;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Morphling : Role, IVisualAlteration
    {
        private KillButton _morphButton;
        private KillButton _killButton;
        public PlayerControl ClosestPlayer;
        public DateTime LastMorphed;
        public PlayerControl MorphedPlayer;
        public PlayerControl SampledPlayer;
        public float TimeRemaining;
        public bool Morphed => TimeRemaining > 0f;

        public Morphling(PlayerControl player) : base(player)
        {
            Name = "Morphling";
            StartText = "Transform Into <color=#8BFDFDFF>Crewmates</color> to frame them";
            AbilitiesText = "Morph into <color=#8BFDFD>Crewmates</color> to frame them";
            Color = CustomGameOptions.CustomIntColors ? Colors.Morphling : Colors.Intruder;
            LastMorphed = DateTime.UtcNow;
            RoleType = RoleEnum.Morphling;
            Faction = Faction.Intruder;
            FactionName = "Intruder";
            RoleAlignment = RoleAlignment.IntruderDecep;
            FactionColor = Colors.Intruder;
            AlignmentName = "Intruder (Deception)";
            Results = InspResults.MorphGliEscCons;
            //IntroSound = TownOfUsReworked.MorphlingIntro;
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

        public KillButton MorphButton
        {
            get => _morphButton;
            set
            {
                _morphButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        public void Morph()
        {
            TimeRemaining -= Time.deltaTime;
            Utils.Morph(Player, MorphedPlayer);

            if (Player.Data.IsDead)
                TimeRemaining = 0f;
        }

        public void Unmorph()
        {
            MorphedPlayer = null;
            Utils.DefaultOutfit(Player);
            LastMorphed = DateTime.UtcNow;
        }

        public float MorphTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastMorphed;
            var num = CustomGameOptions.MorphlingCd * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        public bool TryGetModifiedAppearance(out VisualAppearance appearance)
        {
            if (Morphed)
            {
                appearance = MorphedPlayer.GetDefaultAppearance();
                var modifier = Modifier.GetModifier(MorphedPlayer);

                if (modifier is IVisualAlteration alteration)
                    alteration.TryGetModifiedAppearance(out appearance);

                return true;
            }

            appearance = Player.GetDefaultAppearance();
            return false;
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

using System;
using TownOfUsReworked.Extensions;
using Hazel;
using System.Linq;
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
            AbilitiesText = "- You can disguise a player to frame them.";
            AttributesText = "- None.";
            Color = CustomGameOptions.CustomImpColors ? Colors.Disguiser : Colors.Intruder;
            LastDisguised = DateTime.UtcNow;
            RoleType = RoleEnum.Disguiser;
            Faction = Faction.Intruders;
            FactionName = "Intruder";
            FactionColor = Colors.Intruder;
            RoleAlignment = RoleAlignment.IntruderDecep;
            AlignmentName = "Intruder (Deception)";
            IntroText = "Kill those who oppose you";
            CoronerDeadReport = "The makeup on the body suggests they are a Disguiser!";
            CoronerKillerReport = "The fake eyelashes and makeup all over the body indicates that they were killed by a Disguiser!";
            Results = InspResults.DisgMorphCamoAgent;
            SubFaction = SubFaction.None;
            FactionDescription = "You are an Intruder! Your main task is to kill anyone who dares to oppose you. Sabotage the systems, murder the crew, do anything" +
                " to ensure your victory over others.";
            Objectives = "- Kill: <color=#008000FF>Syndicate</color>, <color=#8BFDFD>Crew</color> and <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Killers</color>," +
                " <color=#1D7CF2FF>Proselytes</color> and <color=#1D7CF2FF>Neophytes</color>.\n   or\n- Have a critical sabotage reach 0 seconds.";
            Attack = AttackEnum.Basic;
            Defense = DefenseEnum.None;
            AttackString = "Basic";
            DefenseString = "None";
            IntroSound = null;
            AlignmentDescription = "You are an Intruder (Deception) role! It's your job to ensure there's only false information spreading around about you. Keep the " +
                "misinformation circulating, for it can be advantageous to completely fool even one player.";
            RoleDescription = "You are a Disguiser! Cause some chaos by changing people's appearances and fooling everyone around you.";
            AddToRoleHistory(RoleType);
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

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__32 __instance)
        {
            var intTeam = new List<PlayerControl>();

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player.Is(Faction.Intruders))
                    intTeam.Add(player);
            }
            __instance.teamToShow = intTeam;
        }

        public override void Wins()
        {
            IntruderWin = true;
        }

        public override void Loses()
        {
            LostByRPC = true;
        }

        internal override bool EABBNOODFGL(ShipStatus __instance)
        {
            if (Player.Data.IsDead | Player.Data.Disconnected)
                return true;

            if ((PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && (x.Is(Faction.Crew) |
                x.Is(RoleAlignment.NeutralKill) | x.Is(Faction.Syndicate) | x.Is(RoleAlignment.NeutralNeo) | x.Is(RoleAlignment.NeutralPros))) == 0) |
                Utils.Sabotaged())
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

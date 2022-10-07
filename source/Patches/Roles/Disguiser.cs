using System;
using TownOfUs.Extensions;
using TownOfUs.Roles.Modifiers;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class Disguiser : Role, IVisualAlteration

    {
        public KillButton _disguiseButton;
        public PlayerControl ClosestPlayer;
        public DateTime LastDisguised;
        public PlayerControl DisguisedPlayer;
        public PlayerControl MeasuredPlayer;
        public PlayerControl TargetPlayer;
        public PlayerControl disguised { get; private set; }
        public float TimeBeforeDisguised { get; private set; }
        public float DisguiseTimeRemaining { get; private set; }
        public float TimeRemaining;

        public Disguiser(PlayerControl player) : base(player)
        {
            Name = "Disguiser";
            ImpostorText = () => "Disguise <color=#8BFDFDFF>Crewmates</color> to frame them";
            TaskText = () => "Disguise <color=#8BFDFD>Crewmates</color> to frame them";
            if (CustomGameOptions.CustomImpColors) Color = Patches.Colors.Disguiser;
            else Color = Patches.Colors.Impostor;
            LastDisguised = DateTime.UtcNow;
            RoleType = RoleEnum.Disguiser;
            Faction = Faction.Intruders;
            FactionName = "Intruder";
            FactionColor = Patches.Colors.Impostor;
            AlignmentName = "Intruder (Deception)";
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

        public bool Disguised => TimeRemaining > 0f;

        public void Disguise()
        {
            TimeRemaining -= Time.deltaTime;
            Utils.Morph(DisguisedPlayer, Player);
            if (Player.Data.IsDead)
            {
                TimeRemaining = 0f;
            }
        }

        public void Undisguise()
        {
            DisguisedPlayer = null;
            Utils.Unmorph(DisguisedPlayer);
            LastDisguised = DateTime.UtcNow;
        }

        public float DisguiseTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastDisguised;
            var num = CustomGameOptions.DisguiseCooldown * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        public bool TryGetModifiedAppearance(out VisualAppearance appearance)
        {
            if (Disguised)
            {
                appearance = DisguisedPlayer.GetDefaultAppearance();
                var modifier = Modifier.GetModifier(DisguisedPlayer);
                return true;
            }

            appearance = Player.GetDefaultAppearance();
            return false;
        }

        public void StartDisguise(PlayerControl framed)
        {
            disguised = framed;
            TimeBeforeDisguised = CustomGameOptions.TimeToDisguise;
        }

        public void DisguiseTick()
        {
            if (disguised == null)
            {
                return;
            }

            if (TimeBeforeDisguised > 0)
            {
                TimeBeforeDisguised = Math.Clamp(TimeBeforeDisguised - Time.deltaTime, 0, TimeBeforeDisguised);
                
                if (TimeBeforeDisguised <= 0f)
                {
                    DisguiseTimeRemaining = CustomGameOptions.DisguiseDuration;
                }
            }
            else if (DisguiseTimeRemaining > 0)
            {
                DisguiseTimeRemaining -= Time.deltaTime;
                Disguise();
            }
            else
            {
                Undisguise();
            }
        }
    }
}

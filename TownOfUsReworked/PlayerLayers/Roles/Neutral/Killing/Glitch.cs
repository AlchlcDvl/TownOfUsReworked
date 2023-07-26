namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Glitch : Neutral
    {
        public DateTime LastMimic;
        public DateTime LastHack;
        public DateTime LastKilled;
        public CustomButton HackButton;
        public CustomButton MimicButton;
        public CustomButton NeutraliseButton;
        public PlayerControl HackTarget;
        public bool IsUsingMimic => TimeRemaining2 > 0f;
        public float TimeRemaining;
        public float TimeRemaining2;
        public bool IsUsingHack => TimeRemaining > 0f;
        public bool MimicEnabled;
        public bool HackEnabled;
        public PlayerControl MimicTarget;
        public CustomMenu MimicMenu;

        public override Color32 Color => ClientGameOptions.CustomNeutColors ? Colors.Glitch : Colors.Neutral;
        public override string Name => "Glitch";
        public override LayerEnum Type => LayerEnum.Glitch;
        public override RoleEnum RoleType => RoleEnum.Glitch;
        public override Func<string> StartText => () => "foreach var PlayerControl Glitch.MurderPlayer";
        public override Func<string> AbilitiesText => () => "- You can mimic players' appearances whenever you want to\n- You can hack players to stop them from using their abilities\n- " +
                "Hacking blocks your target from being able to use their abilities for a short while\n- You are immune to blocks\n- If you hack a <color=#336EFFFF>Serial Killer</color> " +
                "they will be forced to kill you";
        public override InspectorResults InspectorResults => InspectorResults.HindersOthers;

        public Glitch(PlayerControl owner) : base(owner)
        {
            Objectives = () => "- Neutralise anyone who can oppose you";
            RoleAlignment = RoleAlignment.NeutralKill;
            MimicMenu = new(Player, Click, Exception3);
            RoleBlockImmune = true;
            NeutraliseButton = new(this, "Neutralise", AbilityTypes.Direct, "ActionSecondary", Neutralise, Exception1);
            HackButton = new(this, "Hack", AbilityTypes.Direct, "Secondary", HitHack, Exception2);
            MimicButton = new(this, "Mimic", AbilityTypes.Effect, "Tertiary", HitMimic);
        }

        public float HackTimer()
        {
            var timespan = DateTime.UtcNow - LastHack;
            var num = Player.GetModifiedCooldown(CustomGameOptions.HackCooldown) * 1000f;
            var time = num - (float)timespan.TotalMilliseconds;
            var flag2 = time < 0f;
            return (flag2 ? 0f : time) / 1000f;
        }

        public float MimicTimer()
        {
            var timespan = DateTime.UtcNow - LastMimic;
            var num = Player.GetModifiedCooldown(CustomGameOptions.MimicCooldown) * 1000f;
            var time = num - (float)timespan.TotalMilliseconds;
            var flag2 = time < 0f;
            return (flag2 ? 0f : time) / 1000f;
        }

        public void UnHack()
        {
            HackEnabled = false;

            foreach (var layer in GetLayers(HackTarget))
                layer.IsBlocked = false;

            HackTarget = null;
            LastHack = DateTime.UtcNow;
        }

        public void Hack()
        {
            HackEnabled = true;
            TimeRemaining -= Time.deltaTime;

            if (Meeting || IsDead || HackTarget.Data.IsDead || HackTarget.Data.Disconnected)
                TimeRemaining = 0f;
        }

        public void Mimic()
        {
            TimeRemaining2 -= Time.deltaTime;
            Morph(Player, MimicTarget);
            MimicEnabled = true;

            if (IsDead || Meeting)
                TimeRemaining2 = 0f;
        }

        public void UnMimic()
        {
            MimicTarget = null;
            MimicEnabled = false;
            DefaultOutfit(Player);
            LastMimic = DateTime.UtcNow;
        }

        public float NeutraliseTimer()
        {
            var timespan = DateTime.UtcNow - LastKilled;
            var num = Player.GetModifiedCooldown(CustomGameOptions.GlitchKillCooldown) * 1000f;
            var time = num - (float)timespan.TotalMilliseconds;
            var flag2 = time < 0f;
            return (flag2 ? 0f : time) / 1000f;
        }

        public void Click(PlayerControl player)
        {
            LogSomething($"Mimcking {player.name}");
            MimicTarget = player;
        }

        public void HitHack()
        {
            if (HackTimer() != 0f || IsTooFar(Player, HackButton.TargetPlayer) || IsUsingHack)
                return;

            var interact = Interact(Player, HackButton.TargetPlayer);

            if (interact[3])
            {
                HackTarget = HackButton.TargetPlayer;
                TimeRemaining = CustomGameOptions.HackDuration;
                CallRpc(CustomRPC.Action, ActionsRPC.GlitchRoleblock, this, HackTarget);
                Hack();

                foreach (var layer in GetLayers(HackTarget))
                    layer.IsBlocked = !GetRole(HackTarget).RoleBlockImmune;
            }
            else if (interact[0])
                LastHack = DateTime.UtcNow;
            else if (interact[1])
                LastHack.AddSeconds(CustomGameOptions.ProtectKCReset);
        }

        public void Neutralise()
        {
            if (IsTooFar(Player, NeutraliseButton.TargetPlayer) || NeutraliseTimer() != 0f)
                return;

            var interact = Interact(Player, NeutraliseButton.TargetPlayer, true);

            if (interact[3] || interact[0])
                LastKilled = DateTime.UtcNow;
            else if (interact[1])
                LastKilled.AddSeconds(CustomGameOptions.ProtectKCReset);
            else if (interact[2])
                LastKilled.AddSeconds(CustomGameOptions.VestKCReset);
        }

        public void HitMimic()
        {
            if (MimicTimer() != 0f)
                return;

            if (MimicTarget == null)
                MimicMenu.Open();
            else
            {
                CallRpc(CustomRPC.Action, ActionsRPC.Mimic, this, MimicTarget);
                TimeRemaining2 = CustomGameOptions.MimicDuration;
                Mimic();
            }
        }

        public bool Exception1(PlayerControl player) => (player.Is(SubFaction) && SubFaction != SubFaction.None) || (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate)
            || Player.IsLinkedTo(player);

        public bool Exception2(PlayerControl player) => player == HackTarget;

        public bool Exception3(PlayerControl player) => player == Player;

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            NeutraliseButton.Update("NEUTRALISE", NeutraliseTimer(), CustomGameOptions.GlitchKillCooldown);
            HackButton.Update("HACK", HackTimer(), CustomGameOptions.HackCooldown, IsUsingHack, TimeRemaining, CustomGameOptions.HackDuration);
            MimicButton.Update("MIMIC", MimicTimer(), CustomGameOptions.MimicCooldown, IsUsingMimic, TimeRemaining2, CustomGameOptions.MimicDuration);

            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                if (MimicTarget != null && !IsUsingMimic)
                    MimicTarget = null;

                LogSomething("Removed a target");
            }
        }
    }
}
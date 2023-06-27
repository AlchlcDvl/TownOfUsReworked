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

        public Glitch(PlayerControl owner) : base(owner)
        {
            Name = "Glitch";
            Color = CustomGameOptions.CustomNeutColors ? Colors.Glitch : Colors.Neutral;
            RoleType = RoleEnum.Glitch;
            StartText = () => "foreach PlayerControl Glitch.MurderPlayer";
            AbilitiesText = () => "- You can mimic players' appearances whenever you want to\n- You can hack players to stop them from using their abilities\n- Hacking blocks your target "
                + "from being able to use their abilities for a short while\n- You are immune to blocks\n- If you block a <color=#336EFFFF>Serial Killer</color>, they will be forced to" +
                " kill you";
            Objectives = () => "- Neutralise anyone who can oppose you";
            RoleAlignment = RoleAlignment.NeutralKill;
            MimicMenu = new(Player, Click, Exception3);
            RoleBlockImmune = true;
            Type = LayerEnum.Glitch;
            NeutraliseButton = new(this, "Neutralise", AbilityTypes.Direct, "ActionSecondary", Neutralise, Exception1);
            HackButton = new(this, "Hack", AbilityTypes.Direct, "Secondary", HitHack, Exception2);
            MimicButton = new(this, "Mimic", AbilityTypes.Effect, "Tertiary", HitMimic);
            InspectorResults = InspectorResults.HindersOthers;

            if (TownOfUsReworked.IsTest)
                Utils.LogSomething($"{Player.name} is {Name}");
        }

        public float HackTimer()
        {
            var timespan = DateTime.UtcNow - LastHack;
            var num = Player.GetModifiedCooldown(CustomGameOptions.HackCooldown) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public float MimicTimer()
        {
            var timespan = DateTime.UtcNow - LastMimic;
            var num = Player.GetModifiedCooldown(CustomGameOptions.MimicCooldown) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
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

            if (Utils.Meeting || IsDead || HackTarget.Data.IsDead || HackTarget.Data.Disconnected)
                TimeRemaining = 0f;
        }

        public void Mimic()
        {
            TimeRemaining2 -= Time.deltaTime;
            Utils.Morph(Player, MimicTarget);
            MimicEnabled = true;

            if (IsDead || Utils.Meeting)
                TimeRemaining2 = 0f;
        }

        public void UnMimic()
        {
            MimicTarget = null;
            MimicEnabled = false;
            Utils.DefaultOutfit(Player);
            LastMimic = DateTime.UtcNow;
        }

        public float NeutraliseTimer()
        {
            var timespan = DateTime.UtcNow - LastKilled;
            var num = Player.GetModifiedCooldown(CustomGameOptions.GlitchKillCooldown) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void Click(PlayerControl player)
        {
            Utils.LogSomething($"Mimcking {player.name}");
            MimicTarget = player;
        }

        public void HitHack()
        {
            if (HackTimer() != 0f || Utils.IsTooFar(Player, HackButton.TargetPlayer) || IsUsingHack)
                return;

            var interact = Utils.Interact(Player, HackButton.TargetPlayer);

            if (interact[3])
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.GlitchRoleblock);
                writer.Write(PlayerId);
                writer.Write(HackButton.TargetPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                HackTarget = HackButton.TargetPlayer;
                TimeRemaining = CustomGameOptions.HackDuration;
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
            if (Utils.IsTooFar(Player, NeutraliseButton.TargetPlayer) || NeutraliseTimer() != 0f)
                return;

            var interact = Utils.Interact(Player, NeutraliseButton.TargetPlayer, true);

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
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.Mimic);
                writer.Write(PlayerId);
                writer.Write(MimicTarget.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                TimeRemaining2 = CustomGameOptions.MimicDuration;
                Mimic();
            }
        }

        public bool Exception1(PlayerControl player) => (player.Is(SubFaction) && SubFaction != SubFaction.None) || (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate)
            || player == Player.GetOtherLover() || player == Player.GetOtherRival() || (player.Is(ObjectifierEnum.Mafia) && Player.Is(ObjectifierEnum.Mafia));

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

                Utils.LogSomething("Removed a target");
            }
        }
    }
}
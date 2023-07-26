namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Necromancer : Neutral
    {
        public DeadBody ResurrectingBody;
        public bool Success;
        public CustomButton ResurrectButton;
        public CustomButton KillButton;
        public List<byte> Resurrected = new();
        public int ResurrectUsesLeft;
        public bool ResurrectButtonUsable => ResurrectUsesLeft > 0;
        public int KillUsesLeft;
        public bool KillButtonUsable => KillUsesLeft > 0;
        public DateTime LastKilled;
        public DateTime LastResurrected;
        public int ResurrectedCount;
        public int KillCount;
        public bool Resurrecting;
        public float TimeRemaining;
        public bool IsResurrecting => TimeRemaining > 0f;

        public override Color32 Color => ClientGameOptions.CustomNeutColors ? Colors.Necromancer : Colors.Neutral;
        public override string Name => "Necromancer";
        public override LayerEnum Type => LayerEnum.Necromancer;
        public override RoleEnum RoleType => RoleEnum.Necromancer;
        public override Func<string> StartText => () => "Resurrect The Dead Into Doing Your Bidding";
        public override Func<string> AbilitiesText => () => "- You can resurrect a dead body and bring them into the <color=#E6108AFF>Reanimated</color>\n- You can kill players to speed " +
            "up the process";
        public override InspectorResults InspectorResults => InspectorResults.PreservesLife;

        public Necromancer(PlayerControl player) : base(player)
        {
            Objectives = () => "- Resurrect or kill anyone who can oppose the <color=#E6108AFF>Reanimated</color>";
            RoleAlignment = RoleAlignment.NeutralNeo;
            Objectives = () => "- Resurrect the dead into helping you gain control of the crew";
            SubFaction = SubFaction.Reanimated;
            SubFactionColor = Colors.Reanimated;
            ResurrectUsesLeft = CustomGameOptions.ResurrectCount;
            KillUsesLeft = CustomGameOptions.NecroKillCount;
            ResurrectedCount = 0;
            KillCount = 0;
            Resurrected = new() { Player.PlayerId };
            ResurrectButton = new(this, "Ressurect", AbilityTypes.Dead, "ActionSecondary", HitResurrect, Exception2, true);
            KillButton = new(this, "NecroKill", AbilityTypes.Direct, "Secondary", Kill, Exception1, true);
            SubFactionSymbol = "Σ";
        }

        public float ResurrectTimer()
        {
            var timespan = DateTime.UtcNow - LastResurrected;
            var num = Player.GetModifiedCooldown(CustomGameOptions.ResurrectCooldown, ResurrectedCount * CustomGameOptions.ResurrectCooldownIncrease) * 1000f;
            var time = num - (float)timespan.TotalMilliseconds;
            var flag2 = time < 0f;
            return (flag2 ? 0f : time) / 1000f;
        }

        public float KillTimer()
        {
            var timespan = DateTime.UtcNow - LastKilled;
            var num = Player.GetModifiedCooldown(CustomGameOptions.NecroKillCooldown, KillCount * CustomGameOptions.NecroKillCooldownIncrease) * 1000f;
            var time = num - (float)timespan.TotalMilliseconds;
            var flag2 = time < 0f;
            return (flag2 ? 0f : time) / 1000f;
        }

        public void Resurrect()
        {
            if (!Resurrecting && CustomPlayer.Local.PlayerId == ResurrectButton.TargetBody.ParentId)
            {
                Flash(Colors.Reanimated, CustomGameOptions.NecroResurrectDuration);

                if (CustomGameOptions.NecromancerTargetBody)
                    ResurrectButton.TargetBody?.gameObject.Destroy();
            }

            Resurrecting = true;
            TimeRemaining -= Time.deltaTime;

            if (Meeting || IsDead)
            {
                Success = false;
                TimeRemaining = 0f;
            }
        }

        public void UnResurrect()
        {
            Resurrecting = false;
            LastResurrected = DateTime.UtcNow;

            if (Success)
                FinishResurrect();
        }

        private void FinishResurrect()
        {
            var player = PlayerByBody(ResurrectingBody);

            if (!player.Data.IsDead)
                return;

            var targetRole = GetRole(player);
            targetRole.DeathReason = DeathReasonEnum.Revived;
            targetRole.KilledBy = " By " + PlayerName;
            RoleGen.Convert(player.PlayerId, Player.PlayerId, SubFaction.Reanimated, false);
            ResurrectedCount++;
            ResurrectUsesLeft--;
            player.Revive();

            if (player.Is(ObjectifierEnum.Lovers) && CustomGameOptions.BothLoversDie)
            {
                var lover = player.GetOtherLover();
                var loverRole = GetRole(lover);
                loverRole.DeathReason = DeathReasonEnum.Revived;
                loverRole.KilledBy = " By " + PlayerName;
                RoleGen.Convert(lover.PlayerId, Player.PlayerId, SubFaction.Reanimated, false);
                lover.Revive();
            }
        }

        public bool Exception1(PlayerControl player) => Resurrected.Contains(player.PlayerId) || Player.IsLinkedTo(player);

        public bool Exception2(PlayerControl player) => Resurrected.Contains(player.PlayerId);

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            KillButton.Update("KILL", KillTimer(), CustomGameOptions.NecroKillCooldown, KillUsesLeft, CustomGameOptions.NecroKillCooldownIncreases ? KillCount *
                CustomGameOptions.NecroKillCooldownIncrease : 0, true, KillButtonUsable);
            ResurrectButton.Update("RESURRECT", ResurrectTimer(), CustomGameOptions.ResurrectCooldown, ResurrectUsesLeft, IsResurrecting, TimeRemaining,
                CustomGameOptions.NecroResurrectDuration, CustomGameOptions.ResurrectCooldownIncreases ? ResurrectedCount * CustomGameOptions.ResurrectCooldownIncrease : 0, true,
                ResurrectButtonUsable);
        }

        public void HitResurrect()
        {
            if (IsTooFar(Player, ResurrectButton.TargetBody) || ResurrectTimer() != 0f || !ResurrectButtonUsable)
                return;

            if (RoleGen.Convertible <= 0 || !PlayerByBody(ResurrectButton.TargetBody).Is(SubFaction.None))
            {
                Flash(new(255, 0, 0, 255));
                LastResurrected = DateTime.UtcNow;
            }
            else
            {
                Spread(Player, PlayerByBody(ResurrectingBody));
                CallRpc(CustomRPC.Action, ActionsRPC.NecromancerResurrect, this, ResurrectingBody);
                TimeRemaining = CustomGameOptions.NecroResurrectDuration;
                Success = true;
                Resurrect();

                if (CustomGameOptions.KillResurrectCooldownsLinked)
                    LastKilled = DateTime.UtcNow;
            }
        }

        public void Kill()
        {
            if (KillTimer() != 0f || IsTooFar(Player, KillButton.TargetPlayer) || !KillButtonUsable)
                return;

            var interact = Interact(Player, KillButton.TargetPlayer, true);

            if (interact[3])
            {
                KillCount++;
                KillUsesLeft--;
            }

            if (interact[0])
            {
                LastKilled = DateTime.UtcNow;

                if (CustomGameOptions.KillResurrectCooldownsLinked)
                    LastResurrected = DateTime.UtcNow;
            }
            else if (interact[1])
            {
                LastKilled.AddSeconds(CustomGameOptions.ProtectKCReset);

                if (CustomGameOptions.KillResurrectCooldownsLinked)
                    LastResurrected.AddSeconds(CustomGameOptions.ProtectKCReset);
            }
            else if (interact[2])
            {
                LastKilled.AddSeconds(CustomGameOptions.VestKCReset);

                if (CustomGameOptions.KillResurrectCooldownsLinked)
                    LastResurrected.AddSeconds(CustomGameOptions.VestKCReset);
            }
        }
    }
}
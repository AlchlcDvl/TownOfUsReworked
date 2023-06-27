namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Altruist : Crew
    {
        public CustomButton ReviveButton;
        public int UsesLeft;
        public bool ButtonUsable => UsesLeft > 0;
        public bool Reviving;
        public float TimeRemaining;
        public bool IsReviving => TimeRemaining > 0f;
        public DeadBody RevivingBody;
        public bool Success;
        public DateTime LastRevived;

        public Altruist(PlayerControl player) : base(player)
        {
            Name = "Altruist";
            StartText = () => "Sacrifice Yourself To Save Another";
            AbilitiesText = () => $"- You can revive a dead body\n- Reviving someone takes {CustomGameOptions.AltReviveDuration}s\n- If a meeting is called during your revive, the revive "
                + "fails";
            Color = CustomGameOptions.CustomCrewColors ? Colors.Altruist : Colors.Crew;
            RoleType = RoleEnum.Altruist;
            RoleAlignment = RoleAlignment.CrewProt;
            InspectorResults = InspectorResults.PreservesLife;
            Type = LayerEnum.Altruist;
            UsesLeft = CustomGameOptions.ReviveCount;
            ReviveButton = new(this, "Revive", AbilityTypes.Dead, "ActionSecondary", HitRevive, true);

            if (TownOfUsReworked.IsTest)
                Utils.LogSomething($"{Player.name} is {Name}");
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            ReviveButton.Update("REVIVE", ReviveTimer(), CustomGameOptions.ReviveCooldown, UsesLeft, IsReviving, TimeRemaining, CustomGameOptions.AltReviveDuration, true, ButtonUsable);
        }

        public float ReviveTimer()
        {
            var timespan = DateTime.UtcNow - LastRevived;
            var num = Player.GetModifiedCooldown(CustomGameOptions.ReviveCooldown) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void Revive()
        {
            if (!Reviving && CustomPlayer.Local.PlayerId == ReviveButton.TargetBody.ParentId)
            {
                Utils.Flash(Color, CustomGameOptions.AltReviveDuration);

                if (CustomGameOptions.AltruistTargetBody)
                    ReviveButton.TargetBody?.gameObject.Destroy();
            }

            Reviving = true;
            TimeRemaining -= Time.deltaTime;

            if (Utils.Meeting || IsDead)
            {
                Success = false;
                TimeRemaining = 0f;
            }
        }

        public void UnRevive()
        {
            Reviving = false;
            LastRevived = DateTime.UtcNow;

            if (Success)
                FinishRevive();
        }

        private void FinishRevive()
        {
            var player = Utils.PlayerByBody(RevivingBody);
            var targetRole = GetRole(player);
            var formerKiller = targetRole.KilledBy;
            targetRole.DeathReason = DeathReasonEnum.Revived;
            targetRole.KilledBy = " By " + PlayerName;
            Utils.Revive(player);
            UsesLeft--;

            if (player.Is(ObjectifierEnum.Lovers) && CustomGameOptions.BothLoversDie)
            {
                var lover = player.GetOtherLover();
                var loverRole = GetRole(lover);
                loverRole.DeathReason = DeathReasonEnum.Revived;
                loverRole.KilledBy = " By " + PlayerName;
                Utils.Revive(lover);
            }

            if (UsesLeft == 0)
                Utils.RpcMurderPlayer(Player, Player);

            if (formerKiller.Contains(CustomPlayer.LocalCustom.Data.PlayerName))
            {
                LocalRole.AllArrows.Add(player.PlayerId, new(PlayerControl.LocalPlayer, Color));
                Utils.Flash(Color);
            }
        }

        public void HitRevive()
        {
            if (Utils.IsTooFar(Player, ReviveButton.TargetBody) || ReviveTimer() != 0f || !ButtonUsable)
                return;

            var playerId = ReviveButton.TargetBody.ParentId;
            RevivingBody = ReviveButton.TargetBody;
            var player = Utils.PlayerById(playerId);
            Utils.Spread(Player, player);
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
            writer.Write((byte)ActionsRPC.AltruistRevive);
            writer.Write(PlayerId);
            writer.Write(playerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            TimeRemaining = CustomGameOptions.AltReviveDuration;
            Success = true;
            Revive();
        }
    }
}
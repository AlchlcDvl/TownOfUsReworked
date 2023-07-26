namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class VampireHunter : Crew
    {
        public DateTime LastStaked;
        public static bool VampsDead => !CustomPlayer.AllPlayers.Any(x => !x.Data.IsDead && !x.Data.Disconnected && x.Is(SubFaction.Undead));
        public CustomButton StakeButton;

        public override Color32 Color => ClientGameOptions.CustomCrewColors ? Colors.VampireHunter : Colors.Crew;
        public override string Name => "Vampire Hunter";
        public override LayerEnum Type => LayerEnum.VampireHunter;
        public override RoleEnum RoleType => RoleEnum.VampireHunter;
        public override Func<string> StartText => () => "Stake The <color=#7B8968FF>Undead</color>";
        public override Func<string> AbilitiesText => () => "- You can stake players to see if they have been turned\n- When you stake a turned person, or an <color=#7B8968FF>Undead" +
            "</color> tries to interact with you, you will kill them\n- When all <color=#7B8968FF>Undead</color> players die, you will become a <color=#FFFF00FF>Vigilante</color>";
        public override InspectorResults InspectorResults => InspectorResults.TracksOthers;

        public VampireHunter(PlayerControl player) : base(player)
        {
            RoleAlignment = RoleAlignment.CrewAudit;
            StakeButton = new(this, "Stake", AbilityTypes.Direct, "ActionSecondary", Stake);
        }

        public float StakeTimer()
        {
            var timespan = DateTime.UtcNow - LastStaked;
            var num = Player.GetModifiedCooldown(CustomGameOptions.StakeCooldown) * 1000f;
            var time = num - (float)timespan.TotalMilliseconds;
            var flag2 = time < 0f;
            return (flag2 ? 0f : time) / 1000f;
        }

        public void TurnVigilante()
        {
            var newRole = new Vigilante(Player);
            newRole.RoleUpdate(this);

            if (Local)
                Flash(Colors.Vigilante);

            if (CustomPlayer.Local.Is(RoleEnum.Seer))
                Flash(Colors.Seer);
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            StakeButton.Update("STAKE", StakeTimer(), CustomGameOptions.StakeCooldown);

            if (VampsDead && !IsDead)
            {
                CallRpc(CustomRPC.Change, TurnRPC.TurnVigilante, this);
                TurnVigilante();
            }
        }

        public void Stake()
        {
            if (IsTooFar(Player, StakeButton.TargetPlayer) || StakeTimer() != 0f)
                return;

            var interact = Interact(Player, StakeButton.TargetPlayer, StakeButton.TargetPlayer.Is(SubFaction.Undead) || StakeButton.TargetPlayer.IsFramed());

            if (interact[3] || interact[0])
                LastStaked = DateTime.UtcNow;
            else if (interact[1])
                LastStaked.AddSeconds(CustomGameOptions.ProtectKCReset);
            else if (interact[2])
                LastStaked.AddSeconds(CustomGameOptions.VestKCReset);
        }
    }
}
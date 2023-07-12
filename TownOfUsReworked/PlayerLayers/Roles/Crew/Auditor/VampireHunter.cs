using static TownOfUsReworked.Languages.Language;
namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class VampireHunter : Crew
    {
        public DateTime LastStaked;
        public static bool VampsDead => !CustomPlayer.AllPlayers.Any(x => !x.Data.IsDead && !x.Data.Disconnected && x.Is(SubFaction.Undead));
        public CustomButton StakeButton;

        public VampireHunter(PlayerControl player) : base(player)
        {
            Name = GetString("VampireHunter");
            StartText = () => GetString("VampireHunterStartText");
            AbilitiesText = () => GetString("VampireHunterAbilitiesText1")
                + GetString("VampireHunterAbilitiesText2");
            Color = CustomGameOptions.CustomCrewColors ? Colors.VampireHunter : Colors.Crew;
            RoleType = RoleEnum.VampireHunter;
            RoleAlignment = RoleAlignment.CrewAudit;
            InspectorResults = InspectorResults.TracksOthers;
            Type = LayerEnum.VampireHunter;
            StakeButton = new(this, "Stake", AbilityTypes.Direct, "ActionSecondary", Stake);

            if (TownOfUsReworked.IsTest)
                Utils.LogSomething($"{Player.name} is {Name}");
        }

        public float StakeTimer()
        {
            var timespan = DateTime.UtcNow - LastStaked;
            var num = Player.GetModifiedCooldown(CustomGameOptions.StakeCooldown) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void TurnVigilante()
        {
            var newRole = new Vigilante(Player);
            newRole.RoleUpdate(this);

            if (Local && !IntroCutscene.Instance)
                Utils.Flash(Colors.Vigilante);

            if (CustomPlayer.Local.Is(RoleEnum.Seer) && !IntroCutscene.Instance)
                Utils.Flash(Colors.Seer);
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            StakeButton.Update("Stake", StakeTimer(), CustomGameOptions.StakeCooldown);

            if (VampsDead && !IsDead)
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Change, SendOption.Reliable);
                writer.Write((byte)TurnRPC.TurnVigilante);
                writer.Write(PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                TurnVigilante();
            }
        }

        public void Stake()
        {
            if (Utils.IsTooFar(Player, StakeButton.TargetPlayer) || StakeTimer() != 0f)
                return;

            var interact = Utils.Interact(Player, StakeButton.TargetPlayer, StakeButton.TargetPlayer.Is(SubFaction.Undead) || StakeButton.TargetPlayer.IsFramed());

            if (interact[3] || interact[0])
                LastStaked = DateTime.UtcNow;
            else if (interact[1])
                LastStaked.AddSeconds(CustomGameOptions.ProtectKCReset);
            else if (interact[2])
                LastStaked.AddSeconds(CustomGameOptions.VestKCReset);
        }
    }
}
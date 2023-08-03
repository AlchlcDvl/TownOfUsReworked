namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Mayor : Crew
    {
        public bool RoundOne { get; set; }
        public CustomButton RevealButton { get; set; }
        public bool Revealed { get; set; }
        public byte Voted { get; set; }

        public override Color32 Color => ClientGameOptions.CustomCrewColors ? Colors.Mayor : Colors.Crew;
        public override string Name => "Mayor";
        public override LayerEnum Type => LayerEnum.Mayor;
        public override RoleEnum RoleType => RoleEnum.Mayor;
        public override Func<string> StartText => () => "Reveal Yourself To Commit Voter Fraud";
        public override Func<string> Description => () => $"- You can reveal yourself to the crew\n- When revealed, your votes count {CustomGameOptions.MayorVoteCount + 1} times but you "
            + "cannot be protected";
        public override InspectorResults InspectorResults => InspectorResults.LeadsTheGroup;

        public Mayor(PlayerControl player) : base(player)
        {
            RoleAlignment = RoleAlignment.CrewSov;
            Voted = 255;
            RevealButton = new(this, "MayorReveal", AbilityTypes.Effect, "ActionSecondary", Reveal);
        }

        public void Reveal()
        {
            if (RoundOne)
                return;

            CallRpc(CustomRPC.Action, ActionsRPC.MayorReveal, this);
            Revealed = true;
            Flash(Color);
            BreakShield(Player, true);
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            RevealButton.Update("REVEAL", !Revealed, !Revealed && !RoundOne);
        }

        public override void OnMeetingEnd(MeetingHud __instance)
        {
            base.OnMeetingEnd(__instance);
            Voted = 255;
        }
    }
}
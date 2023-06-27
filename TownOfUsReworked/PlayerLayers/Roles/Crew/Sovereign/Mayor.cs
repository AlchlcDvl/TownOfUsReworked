namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Mayor : Crew
    {
        public bool RoundOne;
        public CustomButton RevealButton;
        public bool Revealed;
        public byte Voted;

        public Mayor(PlayerControl player) : base(player)
        {
            Name = "Mayor";
            StartText = () => "Reveal Yourself To Commit Voter Fraud";
            AbilitiesText = () => $"- You can reveal yourself to the crew\n- When revealed, your votes count {CustomGameOptions.MayorVoteCount + 1} times but you cannot be protected";
            Color = CustomGameOptions.CustomCrewColors ? Colors.Mayor : Colors.Crew;
            RoleType = RoleEnum.Mayor;
            RoleAlignment = RoleAlignment.CrewSov;
            InspectorResults = InspectorResults.LeadsTheGroup;
            Type = LayerEnum.Mayor;
            Voted = 255;
            RevealButton = new(this, "MayorReveal", AbilityTypes.Effect, "ActionSecondary", Reveal);

            if (TownOfUsReworked.IsTest)
                Utils.LogSomething($"{Player.name} is {Name}");
        }

        public void Reveal()
        {
            if (RoundOne)
                return;

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
            writer.Write((byte)ActionsRPC.MayorReveal);
            writer.Write(PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            Revealed = true;
            Utils.Flash(Color);
            BreakShield(PlayerId, true);
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            RevealButton.Update("REVEAL", !Revealed, !Revealed && !RoundOne);
        }
    }
}
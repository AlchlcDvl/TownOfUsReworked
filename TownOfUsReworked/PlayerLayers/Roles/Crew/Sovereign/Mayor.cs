using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Data;
using TownOfUsReworked.Custom;
using Hazel;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Mayor : CrewRole
    {
        public bool RoundOne;
        public CustomButton RevealButton;
        public bool Revealed;
        public byte Voted;

        public Mayor(PlayerControl player) : base(player)
        {
            Name = "Mayor";
            StartText = "Reveal Yourself To Commit Voter Fraud";
            AbilitiesText = $"- You can reveal yourself to the crew\n- When revealed, your votes count {CustomGameOptions.MayorVoteCount + 1} times and you cannot be protected";
            Color = CustomGameOptions.CustomCrewColors ? Colors.Mayor : Colors.Crew;
            RoleType = RoleEnum.Mayor;
            RoleAlignment = RoleAlignment.CrewSov;
            AlignmentName = CSv;
            InspectorResults = InspectorResults.LeadsTheGroup;
            Type = LayerEnum.Mayor;
            Voted = 255;
            RevealButton = new(this, "MayorReveal", AbilityTypes.Effect, "ActionSecondary", Reveal);
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

            foreach (var medic in GetRoles<Medic>(RoleEnum.Medic))
            {
                if (medic.ShieldedPlayer == Player)
                    Medic.BreakShield(medic.PlayerId, PlayerId, true);
            }

            foreach (var ret in GetRoles<Retributionist>(RoleEnum.Retributionist))
            {
                if (ret.ShieldedPlayer == Player)
                    Retributionist.BreakShield(ret.PlayerId, PlayerId, true);
            }
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            RevealButton.Update("REVEAL", 0, 1, !Revealed, !Revealed && !RoundOne);
        }
    }
}
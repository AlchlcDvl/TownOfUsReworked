using TownOfUsReworked.CustomOptions;
using System;
using TownOfUsReworked.Data;
using TownOfUsReworked.Classes;
using Hazel;
using TownOfUsReworked.Custom;
using TownOfUsReworked.Extensions;
using System.Linq;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Rebel : SyndicateRole
    {
        public bool HasDeclared;
        public CustomButton SidekickButton;
        public DateTime LastDeclared;

        public Rebel(PlayerControl player) : base(player)
        {
            Name = "Rebel";
            RoleType = RoleEnum.Rebel;
            StartText = "Promote Your Fellow <color=#008000FF>Syndicate</color> To Do Better";
            AbilitiesText = "- You can promote a fellow <color=#008000FF>Syndicate</color> into becoming your successor\n- Promoting an <color=#008000FF>Syndicate</color> turns them " +
                "into a <color=#979C9FFF>Sidekick</color>\n- If you die, the <color=#979C9FFF>Sidekick</color> become the new <color=#FFFCCEFF>Rebel</color>\nand inherits better " +
                $"abilities of their former role\n{AbilitiesText}";
            Color = CustomGameOptions.CustomSynColors ? Colors.Rebel : Colors.Syndicate;
            RoleAlignment = RoleAlignment.SyndicateSupport;
            AlignmentName = SSu;
            Type = LayerEnum.Rebel;
            SidekickButton = new(this, "Sidekick", AbilityTypes.Direct, "Secondary", Sidekick);
        }

        public static void Sidekick(Rebel reb, PlayerControl target)
        {
            reb.HasDeclared = true;
            target.DisableButtons();
            var formerRole = GetRole(target);

            var sidekick = new Sidekick(target)
            {
                FormerRole = formerRole,
                Rebel = reb
            };

            sidekick.RoleUpdate(formerRole);
            target.EnableButtons();

            if (target == PlayerControl.LocalPlayer)
                Utils.Flash(Colors.Rebel);

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Seer))
                Utils.Flash(Colors.Seer);
        }

        public void Sidekick()
        {
            if (Utils.IsTooFar(Player, SidekickButton.TargetPlayer) || HasDeclared)
                return;

            var interact = Utils.Interact(Player, SidekickButton.TargetPlayer);

            if (interact[3])
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.Sidekick);
                writer.Write(Player.PlayerId);
                writer.Write(SidekickButton.TargetPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Sidekick(this, SidekickButton.TargetPlayer);
            }
            else if (interact[0])
                LastDeclared = DateTime.UtcNow;
            else if (interact[1])
                LastDeclared.AddSeconds(CustomGameOptions.ProtectKCReset);
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            var Syn = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Syndicate) && !(x.GetRole() is RoleEnum.Banshee or RoleEnum.Sidekick or RoleEnum.PromotedRebel or
                RoleEnum.Rebel)).ToList();
            SidekickButton.Update("SIDEKICK", 0, 1, Syn, true, !HasDeclared);
        }
    }
}
using TownOfUsReworked.Data;
using TownOfUsReworked.CustomOptions;
using System;
using TownOfUsReworked.Custom;
using Hazel;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Extensions;
using System.Linq;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Godfather : IntruderRole
    {
        public PlayerControl ClosestIntruder;
        public bool HasDeclared;
        public CustomButton DeclareButton;
        public DateTime LastDeclared;

        public Godfather(PlayerControl player) : base(player)
        {
            Name = "Godfather";
            RoleType = RoleEnum.Godfather;
            StartText = "Promote Your Fellow <color=#FF0000FF>Intruders</color> To Do Better";
            AbilitiesText = "- You can promote a fellow <color=#FF0000FF>Intruder</color> into becoming your successor\n- Promoting an <color=#FF0000FF>Intruder</color> turns them " +
                "into a <color=#6400FFFF>Mafioso</color>\n- If you die, the <color=#6400FFFF>Mafioso</color> will become the new <color=#404C08FF>Godfather</color>\nand inherits better " +
                $"abilities of their former role\n{AbilitiesText}";
            Color = CustomGameOptions.CustomIntColors ? Colors.Godfather : Colors.Intruder;
            RoleAlignment = RoleAlignment.IntruderSupport;
            AlignmentName = IS;
            Type = LayerEnum.Godfather;
            DeclareButton = new(this, AssetManager.Promote, AbilityTypes.Direct, "Secondary", Declare);
        }

        public static void Declare(Godfather gf, PlayerControl target)
        {
            gf.HasDeclared = true;
            target.DisableButtons();
            var formerRole = GetRole(target);

            var mafioso = new Mafioso(target)
            {
                FormerRole = formerRole,
                Godfather = gf
            };

            mafioso.RoleUpdate(formerRole);
            target.EnableButtons();

            if (target == PlayerControl.LocalPlayer)
                Utils.Flash(Colors.Mafioso);

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Seer))
                Utils.Flash(Colors.Seer);
        }

        public void Declare()
        {
            if (Utils.IsTooFar(Player, ClosestIntruder) || HasDeclared)
                return;

            var interact = Utils.Interact(Player, ClosestIntruder);

            if (interact[3])
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.Declare);
                writer.Write(Player.PlayerId);
                writer.Write(ClosestIntruder.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Declare(this, ClosestIntruder);
            }
            else if (interact[0])
                LastDeclared = DateTime.UtcNow;
            else if (interact[1])
                LastDeclared.AddSeconds(CustomGameOptions.ProtectKCReset);
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            var Imp = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Intruder) && !x.Is(RoleEnum.Ghoul) && !x.Is(RoleEnum.Mafioso) && !x.Is(RoleEnum.Godfather) &&
                !x.Is(RoleEnum.PromotedGodfather)).ToList();
            DeclareButton.Update("PROMOTE", 0, 1, Imp, true, !HasDeclared);
        }
    }
}
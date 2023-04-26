using TownOfUsReworked.Data;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Extensions;
using Hazel;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Mafioso : IntruderRole
    {
        public Role FormerRole;
        public Godfather Godfather;
        public bool CanPromote => (Godfather.IsDead || Godfather.Disconnected) && !IsDead;

        public Mafioso(PlayerControl player) : base(player)
        {
            Name = "Mafioso";
            RoleType = RoleEnum.Mafioso;
            StartText = "Succeed The <color=#404C08FF>Godfather</color>";
            AbilitiesText = "- When the <color=#404C08FF>Godfather</color> dies, you will become the new <color=#404C08FF>Godfather</color> with boosted abilities of your former role" +
                $"\n{AbilitiesText}";
            Color = CustomGameOptions.CustomIntColors ? Colors.Mafioso : Colors.Intruder;
            RoleAlignment = RoleAlignment.IntruderUtil;
            AlignmentName = IU;
            Type = LayerEnum.Mafioso;
        }

        public void TurnGodfather()
        {
            var newRole = new PromotedGodfather(Player)
            {
                FormerRole = FormerRole,
                RoleBlockImmune = FormerRole.RoleBlockImmune,
                RoleAlignment = FormerRole.RoleAlignment,
                AlignmentName = FormerRole.AlignmentName
            };

            newRole.RoleUpdate(this);

            if (Player == PlayerControl.LocalPlayer)
                Utils.Flash(Colors.Godfather);

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Seer))
                Utils.Flash(Colors.Seer);
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);

            if (CanPromote && !Player.Data.IsDead)
            {
                TurnGodfather();
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Change, SendOption.Reliable);
                writer.Write((byte)TurnRPC.TurnGodfather);
                writer.Write(Player.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }
        }
    }
}
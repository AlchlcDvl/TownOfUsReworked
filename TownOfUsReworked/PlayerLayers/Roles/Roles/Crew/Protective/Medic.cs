using TownOfUsReworked.Data;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using System.Linq;
using TownOfUsReworked.Custom;
using Hazel;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Medic : CrewRole
    {
        public PlayerControl ClosestPlayer;
        public bool UsedAbility => ShieldedPlayer != null || ExShielded != null;
        public PlayerControl ShieldedPlayer;
        public PlayerControl ExShielded;
        public CustomButton ShieldButton;

        public Medic(PlayerControl player) : base(player)
        {
            Name = "Medic";
            StartText = "Shield A Player To Protect Them";
            AbilitiesText = "- You can shield a player to prevent them from dying to others\n- If your target is attacked, you will be notified of it by default\n- Your shield does " +
                "not save your target from suicides";
            Color = CustomGameOptions.CustomCrewColors ? Colors.Medic : Colors.Crew;
            RoleType = RoleEnum.Medic;
            ShieldedPlayer = null;
            ExShielded = null;
            RoleAlignment = RoleAlignment.CrewProt;
            AlignmentName = CP;
            InspectorResults = InspectorResults.SeeksToProtect;
            Type = LayerEnum.Medic;
            ShieldButton = new(this, AssetManager.Shield, AbilityTypes.Direct, "ActionSecondary", Protect);
        }

        public void Protect()
        {
            if (Utils.IsTooFar(Player, ClosestPlayer))
                return;

            var interact = Utils.Interact(Player, ClosestPlayer);

            if (interact[3])
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.Protect);
                writer.Write(Player.PlayerId);
                writer.Write(ClosestPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                ShieldedPlayer = ClosestPlayer;
            }
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            var notShielded = PlayerControl.AllPlayerControls.ToArray().Where(x => x != ShieldedPlayer).ToList();
            ShieldButton.Update("SHIELD", 0, 1, notShielded, !UsedAbility, !UsedAbility);
        }

        public static void BreakShield(byte medicId, byte playerId, bool flag)
        {
            var role = GetRole<Medic>(Utils.PlayerById(medicId));

            if ((PlayerControl.LocalPlayer.PlayerId == playerId && (CustomGameOptions.NotificationShield == NotificationOptions.Shielded || CustomGameOptions.NotificationShield ==
                NotificationOptions.ShieldedAndMedic)) || (PlayerControl.LocalPlayer.PlayerId == medicId && (CustomGameOptions.NotificationShield == NotificationOptions.Medic ||
                CustomGameOptions.NotificationShield == NotificationOptions.ShieldedAndMedic)) || CustomGameOptions.NotificationShield == NotificationOptions.Everyone)
            {
                Utils.Flash(role.Color);
            }

            if (!flag)
                return;

            var player = Utils.PlayerById(playerId);

            foreach (var role2 in GetRoles<Medic>(RoleEnum.Medic))
            {
                if (role2.ShieldedPlayer.PlayerId == playerId)
                {
                    role2.ShieldedPlayer = null;
                    role2.ExShielded = player;
                    Utils.LogSomething(player.name + " Is Ex-Shielded");
                }
            }

            player.MyRend().material.SetColor("_VisorColor", Palette.VisorColor);
            player.MyRend().material.SetFloat("_Outline", 0f);
        }
    }
}
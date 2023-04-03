using HarmonyLib;
using TownOfUsReworked.Classes;
using System.Linq;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.RebelMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDEverything
    {
        public static void Postfix()
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Rebel))
                return;

            var role = Role.GetRole<Rebel>(PlayerControl.LocalPlayer);

            if (role.DeclareButton == null)
                role.DeclareButton = CustomButtons.InstantiateButton();

            var Syn = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Syndicate) && !x.Is(RoleEnum.Gorgon)).ToList();
            role.DeclareButton.UpdateButton(role, "SIDEKICK", 0, 1, AssetManager.Sidekick, AbilityTypes.Direct, "Secondary", Syn, !role.HasDeclared);

            if (role.FormerRole == null || !role.WasSidekick || role.FormerRole?.Type == RoleEnum.Anarchist)
                return;

            var formerRole = role.FormerRole.Type;

            if (formerRole == RoleEnum.Concealer)
            {
                if (role.ConcealButton == null)
                    role.ConcealButton = CustomButtons.InstantiateButton();

                role.ConcealButton.UpdateButton(role, "CONCEAL", role.ConcealTimer(), CustomGameOptions.ConcealCooldown, AssetManager.Placeholder, AbilityTypes.Effect, "Secondary", null,
                    true, !role.Concealed, role.Concealed, role.ConcealTimeRemaining, CustomGameOptions.ConcealDuration);
            }
            else if (formerRole == RoleEnum.Framer)
            {
                if (role.FrameButton == null)
                    role.FrameButton = CustomButtons.InstantiateButton();

                var notFramed = PlayerControl.AllPlayerControls.ToArray().Where(x => !role.Framed.Contains(x.PlayerId) && !x.Is(Faction.Syndicate)).ToList();
                role.FrameButton.UpdateButton(role, "FRAME", role.FrameTimer(), CustomGameOptions.FrameCooldown, AssetManager.Placeholder, Role.SyndicateHasChaosDrive ?
                    AbilityTypes.Effect : AbilityTypes.Direct, "Secondary", notFramed);
            }
            else if (formerRole == RoleEnum.Shapeshifter)
            {
                if (role.ShapeshiftButton == null)
                    role.ShapeshiftButton = CustomButtons.InstantiateButton();

                role.ShapeshiftButton.UpdateButton(role, "SHAPESHIFT", role.ShapeshiftTimer(), CustomGameOptions.ShapeshiftCooldown, AssetManager.Shapeshift, AbilityTypes.Effect, "Secondary",
                    role.Shapeshifted, role.ShapeshiftTimeRemaining, CustomGameOptions.ShapeshiftDuration);
            }
            else if (formerRole == RoleEnum.Warper)
            {
                if (role.WarpButton == null)
                    role.WarpButton = CustomButtons.InstantiateButton();

                role.WarpButton.UpdateButton(role, "WARP", role.WarpTimer(), CustomGameOptions.WarpCooldown, AssetManager.Warp, AbilityTypes.Effect, "Secondary");
            }
            else if (formerRole == RoleEnum.Poisoner)
            {
                if (role.PoisonButton == null)
                    role.PoisonButton = CustomButtons.InstantiateButton();

                var notSyn = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(Faction.Syndicate) && x != role.PoisonedPlayer).ToList();
                role.PoisonButton.UpdateButton(role, role.Poisoned ? "POISONED" : "POISON", role.PoisonTimer(), CustomGameOptions.PoisonCd, role.Poisoned ? AssetManager.Poisoned :
                    AssetManager.Poison, AbilityTypes.Direct, "Secondary", notSyn, true, !role.Poisoned, role.Poisoned && !Role.SyndicateHasChaosDrive, role.PoisonTimeRemaining,
                    CustomGameOptions.PoisonDuration);
            }
            else if (formerRole == RoleEnum.Bomber)
            {
                if (role.BombButton == null)
                    role.BombButton = CustomButtons.InstantiateButton();

                role.BombButton.UpdateButton(role, "PLANT", role.BombTimer(), CustomGameOptions.BombCooldown, AssetManager.Plant, AbilityTypes.Effect, "Secondary");

                if (role.DetonateButton == null)
                    role.DetonateButton = CustomButtons.InstantiateButton();

                role.DetonateButton.UpdateButton(role, "DETONATE", role.DetonateTimer(), CustomGameOptions.DetonateCooldown, AssetManager.Detonate, AbilityTypes.Effect, "Tertiary",
                    role.Bombs.Count > 0);
            }
            else if (formerRole == RoleEnum.Drunkard)
            {
                if (role.ConfuseButton == null)
                    role.ConfuseButton = CustomButtons.InstantiateButton();

                role.ConfuseButton.UpdateButton(role, "CONFUSE", role.DrunkTimer(), CustomGameOptions.ConfuseCooldown, AssetManager.Placeholder, AbilityTypes.Effect, "Secondary",
                    role.Confused, role.ConfuseTimeRemaining, CustomGameOptions.ConfuseDuration, true, !role.Confused);
            }
        }
    }
}
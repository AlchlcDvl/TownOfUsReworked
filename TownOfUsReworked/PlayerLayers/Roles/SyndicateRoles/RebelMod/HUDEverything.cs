using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using System.Linq;
using TownOfUsReworked.CustomOptions;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.RebelMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDEverything
    {
        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Rebel))
                return;

            var role = Role.GetRole<Rebel>(PlayerControl.LocalPlayer);

            if (role.DeclareButton == null)
                role.DeclareButton = Utils.InstantiateButton();

            var Syn = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Syndicate) && !x.Is(RoleEnum.Gorgon)).ToList();
            role.DeclareButton.UpdateButton(role, "SIDEKICK", 0, 1, TownOfUsReworked.SidekickSprite, AbilityTypes.Direct, Syn, !role.HasDeclared);

            if (role.FormerRole == null || !role.WasSidekick || role.FormerRole?.RoleType == RoleEnum.Anarchist)
                return;

            var formerRole = role.FormerRole.RoleType;

            if (formerRole == RoleEnum.Concealer)
            {
                if (role.ConcealButton == null)
                    role.ConcealButton = Utils.InstantiateButton();

                role.ConcealButton.UpdateButton(role, "CONCEAL", role.ConcealTimer(), CustomGameOptions.ConcealCooldown, TownOfUsReworked.Placeholder, AbilityTypes.Effect,
                    null, true, !role.Concealed, role.Concealed, role.ConcealTimeRemaining, CustomGameOptions.ConcealDuration);
            }
            else if (formerRole == RoleEnum.Framer)
            {
                if (role.FrameButton == null)
                    role.FrameButton = Utils.InstantiateButton();

                var notFramed = PlayerControl.AllPlayerControls.ToArray().Where(x => !role.Framed.Contains(x.PlayerId) && !x.Is(Faction.Syndicate)).ToList();
                role.FrameButton.UpdateButton(role, "FRAME", role.FrameTimer(), CustomGameOptions.FrameCooldown, TownOfUsReworked.Placeholder, Role.SyndicateHasChaosDrive ?
                    AbilityTypes.Effect : AbilityTypes.Direct, notFramed);
            }
            else if (formerRole == RoleEnum.Shapeshifter)
            {
                if (role.ShapeshiftButton == null)
                    role.ShapeshiftButton = Utils.InstantiateButton();

                role.ShapeshiftButton.UpdateButton(role, "SHAPESHIFT", role.ShapeshiftTimer(), CustomGameOptions.ShapeshiftCooldown, TownOfUsReworked.Shapeshift, AbilityTypes.Effect, 
                    role.Shapeshifted, role.ShapeshiftTimeRemaining, CustomGameOptions.ShapeshiftDuration);
            }
            else if (formerRole == RoleEnum.Warper)
            {
                if (role.WarpButton == null)
                    role.WarpButton = Utils.InstantiateButton();

                role.WarpButton.UpdateButton(role, "WARP", role.WarpTimer(), CustomGameOptions.WarpCooldown, TownOfUsReworked.WarpSprite, AbilityTypes.Effect);
            }
            else if (formerRole == RoleEnum.Poisoner)
            {
                if (role.PoisonButton == null)
                    role.PoisonButton = Utils.InstantiateButton();

                var notSyn = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(Faction.Syndicate) && x != role.PoisonedPlayer).ToList();
                role.PoisonButton.UpdateButton(role, role.Poisoned ? "POISONED" : "POISON", role.PoisonTimer(), CustomGameOptions.PoisonCd, role.Poisoned ? TownOfUsReworked.PoisonedSprite :
                    TownOfUsReworked.PoisonSprite, AbilityTypes.Direct, notSyn, true, !role.Poisoned, role.Poisoned && !Role.SyndicateHasChaosDrive, role.PoisonTimeRemaining,
                    CustomGameOptions.PoisonDuration);
            }
            else if (formerRole == RoleEnum.Bomber)
            {
                if (role.BombButton == null)
                    role.BombButton = Utils.InstantiateButton();

                role.BombButton.UpdateButton(role, "PLANT", role.BombTimer(), CustomGameOptions.BombCooldown, TownOfUsReworked.PlantSprite, AbilityTypes.Effect);

                if (role.DetonateButton == null)
                    role.DetonateButton = Utils.InstantiateButton();

                role.BombButton.UpdateButton(role, "DETONATE", role.DetonateTimer(), CustomGameOptions.DetonateCooldown, TownOfUsReworked.DetonateSprite, AbilityTypes.Effect,
                    role.Bombs.Count > 0);
            }
            else if (formerRole == RoleEnum.Drunkard)
            {
                if (role.ConfuseButton == null)
                    role.ConfuseButton = Utils.InstantiateButton();

                role.ConfuseButton.UpdateButton(role, "CONFUSE", role.DrunkTimer(), CustomGameOptions.ConfuseCooldown, TownOfUsReworked.Placeholder, AbilityTypes.Effect, role.Confused,
                    role.ConfuseTimeRemaining, CustomGameOptions.ConfuseDuration, true, !role.Confused);
            }
        }
    }
}
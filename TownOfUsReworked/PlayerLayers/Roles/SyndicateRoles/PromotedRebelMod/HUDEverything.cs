using HarmonyLib;
using TownOfUsReworked.Classes;
using System.Linq;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Data;
using UnityEngine;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.PromotedRebelMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDEverything
    {
        public static void Postfix()
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.PromotedRebel))
                return;

            var role = Role.GetRole<PromotedRebel>(PlayerControl.LocalPlayer);

            if (role.FormerRole == null || role.FormerRole?.RoleType == RoleEnum.Anarchist)
                return;

            var formerRole = role.FormerRole.RoleType;

            if (formerRole == RoleEnum.Concealer)
            {
                if (role.ConcealButton == null)
                    role.ConcealButton = CustomButtons.InstantiateButton();

                var flag = role.ConcealedPlayer == null && !role.HoldsDrive;
                role.ConcealButton.UpdateButton(role, flag ? "SET TARGET" : "CONCEAL", role.ConcealTimer(), CustomGameOptions.ConcealCooldown, AssetManager.Placeholder, AbilityTypes.Effect,
                    "Secondary", null, true, !role.Concealed, role.Concealed, role.ConcealTimeRemaining, CustomGameOptions.ConcealDuration);
            }
            else if (formerRole == RoleEnum.Framer)
            {
                if (role.FrameButton == null)
                    role.FrameButton = CustomButtons.InstantiateButton();

                var notFramed = PlayerControl.AllPlayerControls.ToArray().Where(x => !role.Framed.Contains(x.PlayerId) && !x.Is(Faction.Syndicate)).ToList();
                role.FrameButton.UpdateButton(role, "FRAME", role.FrameTimer(), CustomGameOptions.FrameCooldown, AssetManager.Placeholder, role.HoldsDrive ? AbilityTypes.Effect :
                    AbilityTypes.Direct, "Secondary", notFramed);
            }
            else if (formerRole == RoleEnum.Shapeshifter)
            {
                if (role.ShapeshiftButton == null)
                    role.ShapeshiftButton = CustomButtons.InstantiateButton();

                var flag1 = role.ShapeshiftPlayer1 == null && !role.HoldsDrive;
                var flag2 = role.ShapeshiftPlayer2 == null && !role.HoldsDrive;
                role.ShapeshiftButton.UpdateButton(role, flag1 ? "FIRST TARGET" : (flag2 ? "SECOND TARGET": "SHAPESHIFT"), role.ShapeshiftTimer(), CustomGameOptions.ShapeshiftCooldown,
                    AssetManager.Shapeshift, AbilityTypes.Effect, "Secondary", role.Shapeshifted, role.ShapeshiftTimeRemaining, CustomGameOptions.ShapeshiftDuration);
            }
            else if (formerRole == RoleEnum.Warper)
            {
                if (role.WarpButton == null)
                    role.WarpButton = CustomButtons.InstantiateButton();

                var flag1 = role.WarpPlayer1 == null && !role.HoldsDrive;
                var flag2 = role.WarpPlayer2 == null && !role.HoldsDrive;
                role.WarpButton.UpdateButton(role, flag1 ? "FIRST TARGET" : (flag2 ? "SECOND TARGET": "WARP"), role.WarpTimer(), CustomGameOptions.WarpCooldown, AssetManager.Placeholder,
                    AbilityTypes.Effect, "ActionSecondary");
            }
            else if (formerRole == RoleEnum.Poisoner)
            {
                if (role.PoisonButton == null)
                    role.PoisonButton = CustomButtons.InstantiateButton();

                var flag = role.PoisonedPlayer == null && role.HoldsDrive;
                var notSyn = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(Faction.Syndicate) && x != role.PoisonedPlayer).ToList();
                role.PoisonButton.UpdateButton(role, flag ? "SET POISON" : "POISON", role.PoisonTimer(), CustomGameOptions.PoisonCd, AssetManager.Poison, AbilityTypes.Direct,
                    "Secondary", notSyn, true, !role.Poisoned, role.Poisoned, role.PoisonTimeRemaining, CustomGameOptions.PoisonDuration);
            }
            else if (formerRole == RoleEnum.Bomber)
            {
                if (role.BombButton == null)
                    role.BombButton = CustomButtons.InstantiateButton();

                if (role.DetonateButton == null)
                    role.DetonateButton = CustomButtons.InstantiateButton();

                role.BombButton.UpdateButton(role, "PLANT", role.BombTimer(), CustomGameOptions.BombCooldown, AssetManager.Plant, AbilityTypes.Effect, "Secondary");
                role.DetonateButton.UpdateButton(role, "DETONATE", role.DetonateTimer(), CustomGameOptions.DetonateCooldown, AssetManager.Detonate, AbilityTypes.Effect, "Tertiary",
                    role.Bombs.Count > 0);
            }
            else if (formerRole == RoleEnum.Drunkard)
            {
                if (role.ConfuseButton == null)
                    role.ConfuseButton = CustomButtons.InstantiateButton();

                var flag = role.ConfusedPlayer == null && !role.HoldsDrive;
                role.ConfuseButton.UpdateButton(role, flag ? "SET CONFUSE" : "CONFUSE", role.DrunkTimer(), CustomGameOptions.ConfuseCooldown, AssetManager.Placeholder,
                    AbilityTypes.Effect, "Secondary", role.Confused, role.ConfuseTimeRemaining, CustomGameOptions.ConfuseDuration, true, !role.Confused);
            }
            else if (formerRole == RoleEnum.Crusader)
            {
                if (role.CrusadeButton == null)
                    role.CrusadeButton = CustomButtons.InstantiateButton();

                role.CrusadeButton.UpdateButton(role, "CRUSADE", role.CrusadeTimer(), CustomGameOptions.CrusadeCooldown, AssetManager.Placeholder, AbilityTypes.Direct, "Secondary",
                    role.OnCrusade, role.CrusadeTimeRemaining, CustomGameOptions.CrusadeDuration, true, !role.OnCrusade);
            }

            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                if (role.ShapeshiftPlayer2 != null && !role.HoldsDrive && !role.Shapeshifted)
                    role.ShapeshiftPlayer2 = null;
                else if (role.ShapeshiftPlayer1 != null && !role.HoldsDrive && !role.Shapeshifted)
                    role.ShapeshiftPlayer1 = null;
                else if (role.ConcealedPlayer != null && !role.HoldsDrive && !role.Concealed)
                    role.ConcealedPlayer = null;
                else if (role.WarpPlayer1 != null && !role.HoldsDrive)
                    role.WarpPlayer1 = null;
                else if (role.WarpPlayer2 != null && !role.HoldsDrive)
                    role.WarpPlayer2 = null;
                else if (role.ConfusedPlayer != null && !role.HoldsDrive && !role.Concealed)
                    role.ConfusedPlayer = null;
                else if (role.PoisonedPlayer != null && role.HoldsDrive && !role.Poisoned)
                    role.PoisonedPlayer = null;

                Utils.LogSomething("Removed a target");
            }
        }
    }
}
using HarmonyLib;
using Rewired;
using Rewired.Data;

namespace TownOfUsReworked.Patches
{
    //Thanks to TheOtherRolesAU/TheOtherRoles/pull/347 by dadoum for the patch and extension
    [HarmonyPatch(typeof(InputManager_Base), nameof(InputManager_Base.Awake))]
    public static class Keybinds
    {
        public static void Prefix(InputManager_Base __instance)
        {
            __instance.userData.GetAction("ActionSecondary").descriptiveName = "Primary Ability";
            __instance.userData.GetAction("UseVent").descriptiveName = "Vent";
            __instance.userData.RegisterBind("Secondary", "Secondary Ability", KeyboardKeyCode.G);
            __instance.userData.RegisterBind("Tertiary", "Tertiary Ability", KeyboardKeyCode.X);
            __instance.userData.RegisterBind("Quarternary", "Quartnary Ability", KeyboardKeyCode.Z);
        }

        private static int RegisterBind(this UserData self, string name, string description, KeyboardKeyCode keycode, int elementIdentifierId = -1, int category = 0, InputActionType
            type = InputActionType.Button)
        {
            self.AddAction(category);
            var action = self.GetAction(self.actions.Count - 1)!;

            action.name = name;
            action.descriptiveName = description;
            action.categoryId = category;
            action.type = type;
            action.userAssignable = true;

            var map = new ActionElementMap
            {
                _elementIdentifierId = elementIdentifierId,
                _actionId = action.id,
                _elementType = ControllerElementType.Button,
                _axisContribution = Pole.Positive,
                _keyboardKeyCode = keycode,
                _modifierKey1 = ModifierKey.None,
                _modifierKey2 = ModifierKey.None,
                _modifierKey3 = ModifierKey.None,
            };

            self.keyboardMaps[0].actionElementMaps.Add(map);
            self.joystickMaps[0].actionElementMaps.Add(map);
            return action.id;
        }
    }
}
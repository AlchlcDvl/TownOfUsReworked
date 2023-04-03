using HarmonyLib;
using TownOfUsReworked.PlayerLayers.Modifiers;
using TownOfUsReworked.PlayerLayers.Objectifiers;
using TownOfUsReworked.PlayerLayers.Abilities;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.BetterMaps.Airship;
using TownOfUsReworked.PlayerLayers;
using TownOfUsReworked.MultiClientInstancing;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(LobbyBehaviour), nameof(LobbyBehaviour.Start))]
    static class LobbyBehaviourPatch
    {
        public static void Postfix()
        {
            //Fix Grenadier and screwed blind in lobby
            HudManager.Instance.FullScreen.gameObject.active = false;

            foreach (var layer in PlayerLayer.Layers)
                layer.OnLobby();

            Role.RoleDictionary.Clear();
            Modifier.ModifierDictionary.Clear();
            Ability.AbilityDictionary.Clear();
            Objectifier.ObjectifierDictionary.Clear();
            PlayerLayer.Layers.Clear();

            Tasks.AllCustomPlateform.Clear();
            Tasks.NearestTask = null;

            if (InstanceControl.Clients.Count != 0 && TownOfUsReworked.MCIActive && ConstantVariables.IsLocalGame)
            {
                int count = InstanceControl.Clients.Count;
                InstanceControl.Clients.Clear();
                InstanceControl.PlayerIdClientId.Clear();

                if (TownOfUsReworked.Persistence)
                {
                    for (var i = 0; i < count; i++)
                        MCIUtils.CreatePlayerInstance();
                }
            }
        }
    }
}
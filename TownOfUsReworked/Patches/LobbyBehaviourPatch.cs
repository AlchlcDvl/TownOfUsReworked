using HarmonyLib;
using UnityEngine;
using System.Linq;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Enums;
using TownOfUsReworked.PlayerLayers.Modifiers;
using TownOfUsReworked.PlayerLayers.Objectifiers;
using TownOfUsReworked.PlayerLayers.Abilities;
using TownOfUsReworked.PlayerLayers.Roles;

namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(LobbyBehaviour), nameof(LobbyBehaviour.Start))]
    static class LobbyBehaviourPatch
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            //Fix Grenadier and screwed blind in lobby
            ((Renderer)HudManager.Instance.FullScreen).gameObject.active = false;

            foreach (var role in Role.AllRoles.Where(x => x.RoleType == RoleEnum.Tracker))
            {
                ((Tracker)role).TrackerArrows.Values.DestroyAll();
                ((Tracker)role).TrackerArrows.Clear();
            }

            foreach (var role in Role.AllRoles.Where(x => x.RoleType == RoleEnum.Amnesiac))
            {
                ((Amnesiac)role).BodyArrows.Values.DestroyAll();
                ((Amnesiac)role).BodyArrows.Clear();
            }

            foreach (var role in Role.AllRoles.Where(x => x.RoleType == RoleEnum.Cannibal))
            {
                ((Cannibal)role).BodyArrows.Values.DestroyAll();
                ((Cannibal)role).BodyArrows.Clear();
            }

            foreach (var role in Role.AllRoles.Where(x => x.RoleType == RoleEnum.Medium))
            {
                ((Medium)role).MediatedPlayers.Values.DestroyAll();
                ((Medium)role).MediatedPlayers.Clear();
            }

            foreach (var role in Role.AllRoles.Where(x => x.RoleType == RoleEnum.Coroner))
            {
                ((Coroner)role).BodyArrows.Values.DestroyAll();
                ((Coroner)role).BodyArrows.Clear();
            }

            foreach (var role in Objectifier.AllObjectifiers.Where(x => x.ObjectifierType == ObjectifierEnum.Taskmaster))
            {
                ((Taskmaster)role).ImpArrows.DestroyAll();
                ((Taskmaster)role).TMArrows.Values.DestroyAll();
                ((Taskmaster)role).TMArrows.Clear();
            }

            foreach (var role in Ability.AllAbilities.Where(x => x.AbilityType == AbilityEnum.Snitch))
            {
                ((Snitch)role).ImpArrows.DestroyAll();
                ((Snitch)role).SnitchArrows.Values.DestroyAll();
                ((Snitch)role).SnitchArrows.Clear();
            }

            foreach (var role in Ability.AllAbilities.Where(x => x.AbilityType == AbilityEnum.Radar))
                ((Radar)role).RadarArrow.DestroyAll();

            Role.RoleDictionary.Clear();
            Modifier.ModifierDictionary.Clear();
            Ability.AbilityDictionary.Clear();
            Objectifier.ObjectifierDictionary.Clear();
        }
    }
}

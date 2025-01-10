global using AmongUs.Data;
global using AmongUs.GameOptions;
global using TBMode = AmongUs.GameOptions.TaskBarMode;

global using BepInEx;
global using BepInEx.Unity.IL2CPP;
global using BepInEx.Configuration;
global using BepInEx.Unity.IL2CPP.Utils.Collections;

global using Il2CppInterop.Runtime;
global using Il2CppInterop.Runtime.Injection;
global using Il2CppInterop.Runtime.Attributes;
global using Il2CppInterop.Runtime.InteropTypes;
global using Il2CppInterop.Runtime.InteropTypes.Arrays;

global using Reactor;
global using Reactor.Utilities;
global using Reactor.Networking;
global using Reactor.Utilities.Extensions;
global using Reactor.Networking.Attributes;
global using Reactor.Networking.Extensions;

global using TownOfUsReworked.Data;
global using TownOfUsReworked.Monos;
global using TownOfUsReworked.Custom;
global using TownOfUsReworked.Classes;
global using TownOfUsReworked.Patches;
global using TownOfUsReworked.Modules;
global using TownOfUsReworked.Options;
global using TownOfUsReworked.RoleGen;
global using TownOfUsReworked.Managers;
global using TownOfUsReworked.Debugger;
global using TownOfUsReworked.BetterMaps;
global using TownOfUsReworked.Extensions;
global using TownOfUsReworked.PlayerLayers;
global using TownOfUsReworked.IPlayerLayers;
global using TownOfUsReworked.PlayerLayers.Roles;
global using static TownOfUsReworked.Classes.RPC;
global using static TownOfUsReworked.Classes.Utils;
global using static TownOfUsReworked.Classes.Blanks;
global using static TownOfUsReworked.Classes.Logging;
global using static TownOfUsReworked.Data.References;
global using TownOfUsReworked.PlayerLayers.Abilities;
global using TownOfUsReworked.PlayerLayers.Modifiers;
global using static TownOfUsReworked.Classes.GameStates;
global using TownOfUsReworked.PlayerLayers.Dispositions;
global using static TownOfUsReworked.Modules.ChatCommand;
global using static TownOfUsReworked.Classes.Interactions;
global using static TownOfUsReworked.Managers.AssetManager;
global using static TownOfUsReworked.Classes.ModCompatibility;
global using static TownOfUsReworked.Extensions.LayerExtentions;

global using System;
global using System.IO;
global using System.Text;
global using System.Linq;
global using System.Text.Json;
global using System.Reflection;
global using System.Collections;
global using System.Collections.Generic;
global using System.Text.Json.Serialization;

global using UnityEngine;
global using UnityEngine.Networking;
global using UColor = UnityEngine.Color;
global using URandom = UnityEngine.Random;
global using UObject = UnityEngine.Object;

global using TMPro;
global using Hazel;
global using Twitch;
global using InnerNet;
global using HarmonyLib;

global using ISystem = Il2CppSystem.Collections.Generic;
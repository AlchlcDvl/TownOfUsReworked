using System.Runtime.InteropServices;
using BepInEx.Unity.IL2CPP.Hook;
using Il2CppInterop.Common;
using Il2CppInterop.Runtime.Runtime;
using UnityEngine.AddressableAssets;

namespace TownOfUsReworked.Patches.Technical;

// Yoinked more code from Daemon with some modification
[HarmonyPatch(typeof(AssetReference), nameof(AssetReference.RuntimeKeyIsValid))]
public static class AddressablesPatch
{
    // I'm not sure if the methodInfoPtr is actually a method info pointer; it just seems really funky
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private unsafe delegate IntPtr LoadAssetAsyncDel(IntPtr thisPtr, IntPtr keyPtr, Il2CppMethodInfo* methodInfoPtr);

    private static LoadAssetAsyncDel Original;

    public static unsafe void Initialize()
    {
        var originalMethodType = AccessTools.Method(typeof(Addressables), nameof(Addressables.LoadAssetAsync), [ typeof(UObject) ]).MakeGenericMethod(typeof(UObject));
        var methodInfoPtr = (Il2CppMethodInfo*)Il2CppInteropUtils.GetIl2CppMethodInfoPointerFieldForGeneratedMethod(originalMethodType).GetValue<IntPtr>(null)!;
        var methodPtr = UnityVersionHandler.Wrap(methodInfoPtr).MethodPointer;
        Info($"Patching Addressables.LoadAssetAsync at 0x{methodPtr:X}");
        INativeDetour.CreateAndApply(methodPtr, DetourMethod, out Original);
    }

    private static unsafe IntPtr DetourMethod(IntPtr thisPtr, IntPtr keyPtr, Il2CppMethodInfo* methodInfoPtr)
    {
        var keyClassPtr = IL2CPP.il2cpp_object_get_class(keyPtr);

        var assetGuid = keyClassPtr == Il2CppClassPointerStore<AssetReference>.NativeClassPtr
            ? new AssetReference(keyPtr).AssetGUID
            : (keyClassPtr == Il2CppClassPointerStore<string>.NativeClassPtr
                ? IL2CPP.Il2CppStringToManaged(keyPtr)
                : null);

        if (IsNullEmptyOrWhiteSpace(assetGuid) || !CustomAddressable.CustomAddressables.TryGetValue(assetGuid!, out var addressable))
            return Original(thisPtr, keyPtr, methodInfoPtr);

        var op = addressable.LoadAsync();
        return op.IsValid() ? IL2CPP.il2cpp_object_unbox(op.Pointer) : Original(thisPtr, keyPtr, methodInfoPtr);
    }

    public static bool Prefix(AssetReference __instance, ref bool __result)
    {
        __result = RuntimeKeyIsValidOriginal(__instance) || CustomAddressable.CustomAddressables.ContainsKey(__instance.AssetGUID);
        return false;
    }

    [HarmonyReversePatch]
    public static bool RuntimeKeyIsValidOriginal(AssetReference instance) => throw new NotSupportedException();
}
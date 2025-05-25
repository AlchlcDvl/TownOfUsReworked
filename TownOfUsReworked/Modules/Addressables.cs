using UnityEngine.AddressableAssets;

namespace TownOfUsReworked.Modules;

public sealed class CustomAddressable<T> : CustomAddressable
    where T : UObject
{
    public CustomAddressable(T item, string guid) : base($"asset/reworked:{guid.ToLower().Trim()}")
    {
        item = item.DontDestroy();
        Ref.OperationHandle = Addressables.ResourceManager.CreateCompletedOperation(item, item ? string.Empty : "Unable to find asset");
    }
}

public abstract class CustomAddressable
{
    public static readonly Dictionary<string, CustomAddressable> CustomAddressables = [];

    public AssetReference Ref { get; }

    protected CustomAddressable(string guid)
    {
        Ref = new(guid);
        CustomAddressables[guid] = this;
    }
}
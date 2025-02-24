using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace TownOfUsReworked.Modules;

public class CustomAddressable<T>(T item, string guid) : CustomAddressable($"asset/reworked:{guid.ToLower().Trim()}") where T : UObject
{
    private T Asset { get; } = item;

    public override AsyncOperationHandle LoadAsync() => Handle ??= Addressables.ResourceManager.CreateCompletedOperation(Asset, Asset ? string.Empty : "Unable to find asset");
}

public abstract class CustomAddressable
{
    public static readonly Dictionary<string, CustomAddressable> CustomAddressables = [];

    public AssetReference Ref { get; }
    protected AsyncOperationHandle Handle { get; set; }

    protected CustomAddressable(string guid)
    {
        Ref = new(guid);
        CustomAddressables[guid] = this;
    }

    public abstract AsyncOperationHandle LoadAsync();
}
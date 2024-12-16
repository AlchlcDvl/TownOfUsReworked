namespace TownOfUsReworked.RoleGen2;

public abstract class BaseGen
{
    public abstract void Assign();

    public virtual void PostAssignment() {}

    public virtual void Clear() {}

    public virtual void BeginFiltering() {}

    public virtual void EndFiltering() {}

    public virtual void InitList() {}
}
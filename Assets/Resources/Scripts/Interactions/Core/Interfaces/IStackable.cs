public interface IStackable
{
    int CurrentStack { get; }

    int MaxStack { get; }

    bool CanStack(IStackable other);

    int AddToStack(int amount);

    int RemoveFromStack(int amount);
}
public interface IItemStorage
{
    bool CanInput(
        PhysicalItem item
    );

    bool Input(
        PhysicalItem item
    );

    bool CanOutput();

    bool TryOutput(
        out StoredItemStack stack
    );

    int CurrentCount { get; }

    int Capacity { get; }
}
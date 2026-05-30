public interface IStackStorageInput
{
    bool CanInputStack(
        StoredItemStack stack
    );

    bool InputStack(
        StoredItemStack stack
    );
}
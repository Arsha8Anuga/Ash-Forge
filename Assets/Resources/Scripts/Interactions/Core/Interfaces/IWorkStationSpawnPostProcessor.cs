using System.Collections.Generic;
using UnityEngine;

public interface IWorkstationSpawnPostProcessor
{
    void ProcessSpawnedObject(
        GameObject spawnedObject,
        StoredItemStack stack,
        WorkstationRecipeData recipe,
        List<PhysicalItem> inputItems
    );
}
using UnityEngine;
using System.Collections;

public class PooledObject :
    MonoBehaviour
{
    private GameObjectPool ownerPool;
    private Coroutine returnRoutine;

    public bool HasPool =>
        ownerPool != null;

    public void SetPool(
        GameObjectPool pool)
    {
        ownerPool = pool;
    }

    public void CancelReturn()
    {
        if (returnRoutine == null)
            return;

        StopCoroutine(
            returnRoutine
        );

        returnRoutine = null;
    }

    public void ReturnNow()
    {
        CancelReturn();

        if (ownerPool != null)
        {
            ownerPool.Release(
                gameObject
            );

            return;
        }

        gameObject.SetActive(false);
    }

    public void ReturnAfter(
        float delay)
    {
        CancelReturn();

        if (!gameObject.activeInHierarchy)
            return;

        returnRoutine =
            StartCoroutine(
                ReturnAfterRoutine(
                    Mathf.Max(
                        0f,
                        delay
                    )
                )
            );
    }

    IEnumerator ReturnAfterRoutine(
        float delay)
    {
        yield return new WaitForSeconds(
            delay
        );

        returnRoutine = null;
        ReturnNow();
    }
}

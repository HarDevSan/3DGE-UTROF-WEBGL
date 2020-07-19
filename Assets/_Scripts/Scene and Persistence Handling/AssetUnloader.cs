using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement;

public class AssetUnloader : MonoBehaviour
{
    private void Awake()
    {
        SceneLoader.OnScene_Has_UnLoaded += UnloadAllUnusedAssets;
    }

    void UnloadAllUnusedAssets()
    {
        StartCoroutine(UnloadAllAssetsRoutine());
    }

    IEnumerator UnloadAllAssetsRoutine()
    {
        AsyncOperation unloadOperation = Resources.UnloadUnusedAssets();

        while (unloadOperation.isDone == false)
        {
            Debug.Log("UnLOading all Assets");
            yield return null;
        }
        Debug.Log("UnLOading all Assets is Done");
    }
}

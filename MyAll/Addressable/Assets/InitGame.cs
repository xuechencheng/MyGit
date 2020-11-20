using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class InitGame : MonoBehaviour
{
    public Canvas canvas;
    // Start is called before the first frame update
    void Start()
    {
        LoadMyCube();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LoadMyCube() {
        //Addressables.LoadAssetAsync<GameObject>("AssetAddress");
        AsyncOperationHandle<GameObject> handle = Addressables.InstantiateAsync("Assets/MyCube.prefab");
        handle.Completed += Callback;
        Addressables.InstantiateAsync("Assets/MyImage.prefab", canvas.transform);
        
    }

    private void Callback(AsyncOperationHandle<GameObject> obj)
    {
        Debug.Log("callBack");
    }
}

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class CreateScriptableObject : MonoBehaviour {


    [MenuItem("Custom Editor/Create Data Asset")]
    static void CreateDataAsset()
    {

        //資料 Asset 路徑
        string holderAssetPath = "Assets/Resources/";

        if (!Directory.Exists(holderAssetPath)) Directory.CreateDirectory(holderAssetPath);

        //建立實體
        AudioList holder = ScriptableObject.CreateInstance<AudioList>();

        //使用 holder 建立名為 dataHolder.asset 的資源
        AssetDatabase.CreateAsset(holder, holderAssetPath + "AudioList.asset");
    }
}


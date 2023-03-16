using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable CS0162 // 접근할 수 없는 코드가 있습니다.

namespace HappyTools
{
    public static class RuntimeInitializer
    {
        const string BOOTSTRAPPER_PREFAB_PATH = "Prefabs/Bootstrapper";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void InstantiateBootstrapper()
        {
            if (BOOTSTRAPPER_PREFAB_PATH == "")
                return;
            Object resourceOb = Resources.Load(BOOTSTRAPPER_PREFAB_PATH);

            if (resourceOb == null)
            {
                Debug.LogError("Failed to find bootstrapper at: " + BOOTSTRAPPER_PREFAB_PATH + ".\nChange the path by changing BOOTSTRAPPER_PREFAB_PATH at RuntimeInitializer.cs. It should be relative to the Resources directory.\nIf you don't want to use a bootstrapper, set the path to \"\".\n");
                return;
            }

            GameObject ob = Object.Instantiate(resourceOb) as GameObject;
            Object.DontDestroyOnLoad(ob);
            ob.GetComponent<GameBootstrapper>().InitGame();
        }
    }
}
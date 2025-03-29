using UnityEngine;

// #pragma warning disable CS0162 // 접근할 수 없는 코드가 있습니다.

namespace HappyTools
{
    public static class RuntimeInitializer
    {
        private const string BootstrapperPrefabPath = "Prefabs/Bootstrapper";

        /// <summary>
        /// 씬이 로드되기 전에 자동으로 실행되는 함수. 게임을 초기화한다.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InstantiateBootstrapper()
        {
            // 부트스트랩 프리팹을 로드한다
            Object resourcePrefab = Resources.Load(BootstrapperPrefabPath);

            // 부트스트랩을 찾지 못했을 경우 에러를 출력한다
            if (resourcePrefab == null)
            {
                Debug.LogError("Failed to find bootstrapper at: " + BootstrapperPrefabPath + ".\nChange the path by changing BOOTSTRAPPER_PREFAB_PATH at RuntimeInitializer.cs. It should be relative to the Resources directory.\nIf you don't want to use a bootstrapper, set the path to \"\".\n");
                return;
            }

            // 부트스트랩을 인스턴스화한다
            GameObject resourceGameObject = Object.Instantiate(resourcePrefab) as GameObject;       // MonoBehavior가 아닌 곳에서 Instantiate할 수 있다

            // 부트스트랩을 씬이 바뀌어도 파괴되지 않게 한다
            Object.DontDestroyOnLoad(resourceGameObject);

            // 부트스트랩이 존재하면 게임을 초기화한다
            if (resourceGameObject != null)
            {
                var gameBootstrapper = resourceGameObject.GetComponent<GameBootstrapper>();
                gameBootstrapper.InitSetting();
                gameBootstrapper.InitGame();
            }
        }
    }
}
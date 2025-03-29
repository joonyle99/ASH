using UnityEngine;

// #pragma warning disable CS0162 // ������ �� ���� �ڵ尡 �ֽ��ϴ�.

namespace HappyTools
{
    public static class RuntimeInitializer
    {
        private const string BootstrapperPrefabPath = "Prefabs/Bootstrapper";

        /// <summary>
        /// ���� �ε�Ǳ� ���� �ڵ����� ����Ǵ� �Լ�. ������ �ʱ�ȭ�Ѵ�.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InstantiateBootstrapper()
        {
            // ��Ʈ��Ʈ�� �������� �ε��Ѵ�
            Object resourcePrefab = Resources.Load(BootstrapperPrefabPath);

            // ��Ʈ��Ʈ���� ã�� ������ ��� ������ ����Ѵ�
            if (resourcePrefab == null)
            {
                Debug.LogError("Failed to find bootstrapper at: " + BootstrapperPrefabPath + ".\nChange the path by changing BOOTSTRAPPER_PREFAB_PATH at RuntimeInitializer.cs. It should be relative to the Resources directory.\nIf you don't want to use a bootstrapper, set the path to \"\".\n");
                return;
            }

            // ��Ʈ��Ʈ���� �ν��Ͻ�ȭ�Ѵ�
            GameObject resourceGameObject = Object.Instantiate(resourcePrefab) as GameObject;       // MonoBehavior�� �ƴ� ������ Instantiate�� �� �ִ�

            // ��Ʈ��Ʈ���� ���� �ٲ� �ı����� �ʰ� �Ѵ�
            Object.DontDestroyOnLoad(resourceGameObject);

            // ��Ʈ��Ʈ���� �����ϸ� ������ �ʱ�ȭ�Ѵ�
            if (resourceGameObject != null)
            {
                var gameBootstrapper = resourceGameObject.GetComponent<GameBootstrapper>();
                gameBootstrapper.InitSetting();
                gameBootstrapper.InitGame();
            }
        }
    }
}
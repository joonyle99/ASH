using joonyle99;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class DebugBehavior : MonoBehaviour
{
    public LayerMask TargetLayer;
    public MonoScript TargetMono;

    void Update()
    {
        // CHEAT: F11 Ű�� ������ ����Ű ȹ��
        if (Input.GetKeyDown(KeyCode.F11))
        {
            BossDungeonManager.Instance.OnKeyObtained();
        }

        // CHEAT: F4 Ű�� ������ ���̾�α� �˴ٿ�
        if (Input.GetKeyDown(KeyCode.F4))
        {
            DialogueController.Instance.ShutdownDialogue();
        }

#if UNITY_EDITOR
        // DEBUG: ���� ������ CheckPoint�� ǥ���Ѵ� (���ٸ� �Ա��� ǥ���Ѵ�)
        if (Input.GetKeyDown(KeyCode.F1))
        {
            var checkPoint = SceneContext.Current.CheckPointManager.LatestCheckpointPosition;

            // ���� ���� ������ �׷� checkPoint�� �������� * ����� �׸���
            const float LENGTH = 3f;
            Debug.DrawLine(checkPoint + Vector3.up * LENGTH, checkPoint + Vector3.down * LENGTH, Color.red, 5f);
            Debug.DrawLine(checkPoint + Vector3.left * LENGTH, checkPoint + Vector3.right * LENGTH, Color.red, 5f);
            var diagonal1 = (Vector3.up + Vector3.right).normalized;
            var diagonal2 = (Vector3.up + Vector3.left).normalized;
            var diagonal3 = (Vector3.down + Vector3.left).normalized;
            var diagonal4 = (Vector3.down + Vector3.right).normalized;
            Debug.DrawLine(checkPoint + diagonal1 * LENGTH, checkPoint + diagonal3 * LENGTH, Color.red, 5f);
            Debug.DrawLine(checkPoint + diagonal2 * LENGTH, checkPoint + diagonal4 * LENGTH, Color.red, 5f);
        }
#endif
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(DebugBehavior))]
public class DebugBehaviorEditor : Editor
{
    private DebugBehavior refDebugBehavior;
    private SerializedProperty targetLayerProperty;
    private SerializedProperty targetMonoProperty;

    /// <summary>
    /// �ش� ������Ʈ�� �����Ϳ� ǥ�õ� �� ȣ��Ǵ� �ݹ� �Լ�
    /// </summary>
    private void OnEnable()
    {
        refDebugBehavior = (DebugBehavior)base.target;

        // ����ȭ�� ������Ʈ�� ������Ƽ�� �����Ѵ�. (Generic�� ������)
        targetLayerProperty = serializedObject.FindProperty(nameof(DebugBehavior.TargetLayer));
        targetMonoProperty = serializedObject.FindProperty(nameof(DebugBehavior.TargetMono));
    }

    public override void OnInspectorGUI()
    {
        // Update
        serializedObject.Update();

        // Preserve State
        {
            EditorGUILayout.LabelField("Preserve State", EditorStyles.boldLabel);
            if (GUILayout.Button("Find All Preserve State"))
            {
                FindAllPreserveState();
            }

            EditorGUILayout.Space(10);
        }

        // Objects In Layer
        {
            EditorGUILayout.LabelField("Objects In Layer", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(targetLayerProperty);
            if (GUILayout.Button("Find All Objects In Layer"))
            {
                FindObjectsInLayer(targetLayerProperty.intValue);
            }

            EditorGUILayout.Space(10);
        }

        // TestFunction
        {
            EditorGUILayout.LabelField("Frustum Intersection", EditorStyles.boldLabel);
            if (GUILayout.Button("Calculate Frustum Intersection"))
            {
                CalcFrustumIntersection();
            }

            EditorGUILayout.Space(10);
        }

        // Find All Objects has MonoScript
        {
            EditorGUILayout.LabelField("Objects has MonoScript", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(targetMonoProperty);
            if (GUILayout.Button("Find All Objects has MonoScript"))
            {
                MonoScript targetMono = targetMonoProperty.objectReferenceValue as MonoScript;
                FindAllObjectsHasMono(targetMono);
            }
        }

        // Apply
        serializedObject.ApplyModifiedProperties();
    }

    /// <summary>
    /// �ش� ������ Preserve State ������Ʈ�� ������ ��� ������Ʈ�� ����Ѵ�
    /// </summary>
    private void FindAllPreserveState()
    {
        PreserveState[] preserveStates = FindObjectsOfType<PreserveState>();

        foreach (var preserveState in preserveStates)
        {
            Debug.Log($"<color=orange><b>{preserveState.name}</b></color>", preserveState.gameObject);
        }
    }

    /// <summary>
    /// �ش� ������ TargetLayer�� �ش��ϴ� ��� ������Ʈ�� ����Ѵ�
    /// </summary>
    private void FindObjectsInLayer(int layerMaskValue)
    {
        GameObject[] allGameObjects = FindObjectsOfType<GameObject>(true);

        foreach (GameObject gameObject in allGameObjects)
        {
            if (gameObject.layer.GetLayerValue() == layerMaskValue)
            {
                Debug.Log($"<color=orange><b>Layer</b></color>: {LayerMask.LayerToName(layerMaskValue.GetLayerNumber())} ========> <color=yellow><b>Object</b></color>: {gameObject.name}", gameObject);
            }
        }
    }

    private void CalcFrustumIntersection()
    {
        // 3D ī�޶��� ���������� 2D ���� �� (Z = 0��) �� ���ϴ� ������ ���Ѵ�

        var mainCamera = Camera.main;

        // ����Ʈ(ī�޶� ���� ȭ���� '����ȭ'�� 2D ��ǥ �ý���)�� 4�� �ڳ� ��ǥ
        Vector3[] viewportCorners = new Vector3[]
        {
            new Vector3(0, 0, mainCamera.nearClipPlane), // ���ϴ�
            new Vector3(1, 0, mainCamera.nearClipPlane), // ���ϴ�
            new Vector3(1, 1, mainCamera.nearClipPlane), // ����
            new Vector3(0, 1, mainCamera.nearClipPlane)  // �»��

            // (0,1)-------------(1,1)
            //   |                 |
            //   |                 |
            //   |                 |
            //   |                 |
            //   |                 |
            // (0,0)-------------(1,0)
        };

        // ���� ���������� �������� �ڳ� ����
        Vector3[] worldCorners = new Vector3[4];
        // Z == 0 ������ ������
        Vector3[] intersectionPoints = new Vector3[4];

        for (int i = 0; i < 4; i++)
        {
            // ����Ʈ ��ǥ(���ϴ�, ���ϴ�, ����, �»��)�� ���� ��ǥ�� ��ȯ
            worldCorners[i] = mainCamera.ViewportToWorldPoint(viewportCorners[i]);

            // ī�޶� ��ġ���� ���� �ڳ� ���������� ���� ����
            Vector3 direction = worldCorners[i] - mainCamera.transform.position;
            // direction�� Z == 0�� XY ���� �̷�� ����
            float ratio = (-1) * mainCamera.transform.position.z / direction.z;
            // direction�� Z == 0�� XY �������� ��� ����
            Vector3 newDirection = new Vector3(direction.x * ratio, direction.y * ratio, (-1) * mainCamera.transform.position.z);
            // newDirection�� Z == 0�� XY ����� ������
            intersectionPoints[i] = mainCamera.transform.position + newDirection;

            Debug.DrawLine(mainCamera.transform.position, intersectionPoints[i], Color.cyan, 5f);
        }
    }

    private void FindAllObjectsHasMono(MonoScript monoScript)
    {
        var monoScriptClass = monoScript.GetClass();
        var allObjects = FindObjectsOfType<GameObject>();
        foreach (var obj in allObjects)
        {
            if (obj.GetComponent(monoScriptClass) != null)
                Debug.Log($"<color=orange><b>{obj.name}</b></color>", obj.gameObject);
        }
    }
}
#endif
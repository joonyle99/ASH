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
        // CHEAT: F11 키를 누르면 보스키 획득
        if (Input.GetKeyDown(KeyCode.F11))
        {
            BossDungeonManager.Instance.OnKeyObtained();
        }

        // CHEAT: F4 키를 누르면 다이얼로그 셧다운
        if (Input.GetKeyDown(KeyCode.F4))
        {
            DialogueController.Instance.ShutdownDialogue();
        }

#if UNITY_EDITOR
        // DEBUG: 현재 마지막 CheckPoint를 표시한다 (없다면 입구를 표시한다)
        if (Input.GetKeyDown(KeyCode.F1))
        {
            var checkPoint = SceneContext.Current.CheckPointManager.LatestCheckpointPosition;

            // 여려 개의 라인을 그려 checkPoint를 중점으로 * 모양을 그린다
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
    /// 해당 컴포넌트가 에디터에 표시될 때 호출되는 콜백 함수
    /// </summary>
    private void OnEnable()
    {
        refDebugBehavior = (DebugBehavior)base.target;

        // 직렬화된 오브젝트로 프로퍼티를 저장한다. (Generic한 데이터)
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
    /// 해당 씬에서 Preserve State 컴포넌트를 가지는 모든 오브젝트를 출력한다
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
    /// 해당 씬에서 TargetLayer에 해당하는 모든 오브젝트를 출력한다
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
        // 3D 카메라의 프러스텀이 2D 상의 맵 (Z = 0인) 에 접하는 지점을 구한다

        var mainCamera = Camera.main;

        // 뷰포트(카메라가 보는 화면의 '정규화'된 2D 좌표 시스템)의 4개 코너 좌표
        Vector3[] viewportCorners = new Vector3[]
        {
            new Vector3(0, 0, mainCamera.nearClipPlane), // 좌하단
            new Vector3(1, 0, mainCamera.nearClipPlane), // 우하단
            new Vector3(1, 1, mainCamera.nearClipPlane), // 우상단
            new Vector3(0, 1, mainCamera.nearClipPlane)  // 좌상단

            // (0,1)-------------(1,1)
            //   |                 |
            //   |                 |
            //   |                 |
            //   |                 |
            //   |                 |
            // (0,0)-------------(1,0)
        };

        // 월드 공간에서의 프러스텀 코너 지점
        Vector3[] worldCorners = new Vector3[4];
        // Z == 0 평면과의 교차점
        Vector3[] intersectionPoints = new Vector3[4];

        for (int i = 0; i < 4; i++)
        {
            // 뷰포트 좌표(좌하단, 우하단, 우상단, 좌상단)를 월드 좌표로 변환
            worldCorners[i] = mainCamera.ViewportToWorldPoint(viewportCorners[i]);

            // 카메라 위치에서 월드 코너 지점으로의 방향 벡터
            Vector3 direction = worldCorners[i] - mainCamera.transform.position;
            // direction이 Z == 0인 XY 평면과 이루는 비율
            float ratio = (-1) * mainCamera.transform.position.z / direction.z;
            // direction을 Z == 0인 XY 평면까지의 쏘는 벡터
            Vector3 newDirection = new Vector3(direction.x * ratio, direction.y * ratio, (-1) * mainCamera.transform.position.z);
            // newDirection과 Z == 0인 XY 평면의 교차점
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
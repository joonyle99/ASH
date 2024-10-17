using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class TeleportNode : MonoBehaviour
{
    public int ID;

    [SerializeField] private TextMeshPro _numberText;
    // [FormerlySerializedAs("connectNodes")]
    [SerializeField] private List<TeleportNode> connectedNodes = new List<TeleportNode>();

    public TextMeshPro NumberText => _numberText;
    public List<TeleportNode> ConnectedNodes => connectedNodes;

    /// <summary>
    /// Awake() 함수는 Start(), Update() 함수와 달리 '스크립트'를 비활성화해도 호출된다.
    /// Awake() 함수만 존재할 때, 스크립트에 활성화 체크박스가 없는 이유이다.
    /// 하지만 게임 오브젝트를 비활성화 한다면 마찬가지로 호출되지 않는다.
    /// </summary>
    private void Awake()
    {
        /*
            public int GetInstanceID()
            {
                EnsureRunningOnMainThread();
                return m_InstanceID;
            }

            public override int GetHashCode()
            {
                return m_InstanceID;
            }
        */

        /*
        // Unity 엔진 내부에서 각 객체에 고유한 ID를 부여한다
        int instanceID = this.GetInstanceID();

        // .Net에서 객체를 식별하는 용도로 해시 코드를 반환한다
        // int, float에 대한 GetHashCode()는 해당 '값 자체'를 반환한다
        // 하지만 claas와 같은 참조 타입에 대해서는 '객체의 참조 위치 (메모리 주소)'를 기반으로 해시 코드를 반환한다.
        // 메모리 주소 기반이기 때문에 해당 객체의 메모리가 해제된 후 다른 객체가 해당 메모리를 사용하게 되면 해시 코드가 중복될 수 있다.
        int hashCode = this.GetHashCode();

        //Debug.Log($"{this.gameObject.name}의 인스턴스 ID: {instanceID} / 해시 코드: {hashCode}");

        TeleportNode targetNode = this;
        int targetInstanceID = targetNode.GetInstanceID();
        int targetHashCode = targetNode.GetHashCode();

        //Debug.Log($"{targetNode.gameObject.name}의 인스턴스 ID: {targetInstanceID} / 해시 코드: {targetHashCode}");

        int a = 512125;
        int b = 512125;
        string str = "hello";
        string str2 = "hello";

        Debug.Log($"a: {a.GetHashCode()} b: {b.GetHashCode()} str: {str.GetHashCode()} str2: {str2.GetHashCode()}");
        */
    }
    private void OnValidate()
    {
        InitializeNode();
    }

    public void InitializeNode()
    {
        var number = joonyle99.Util.ExtractNumber(_numberText.text);
        if (number == -1)
        {
            Debug.LogError($"there is no number");
            return;
        }
        ID = number;
    }
    public TeleportNode GetRandomConnectedNode()
    {
        var nextIndex = Random.Range(0, connectedNodes.Count);
        var nextNode = connectedNodes[nextIndex];
        if (nextNode == null)
        {
            Debug.LogError($"nextNode is invalid");
            return null;
        }
        return connectedNodes[nextIndex];
    }

    private void OnDrawGizmosSelected()
    {
        foreach (var target in connectedNodes)
        {
            var vectorToTarget = target.transform.position - this.transform.position;
            joonyle99.Util.GizmosDrawArrow(this.transform.position, vectorToTarget.normalized, bodyColor: Color.yellow, headColor: Color.yellow, bodyLength: vectorToTarget.magnitude);
        }
    }
}

[CustomEditor(typeof(TeleportNode), true)]
public class TeleportNodeEditor : Editor
{
    // Unity 에디터에서 인스펙터 GUI를 그리는 메서드입니다.
    // OnInspectorGUI 메서드를 오버라이드하여 커스텀 GUI를 정의합니다.
    public override void OnInspectorGUI()
    {
        // 현재 인스펙터에서 편집 중인 TeleportNode '객체'를 가져옵니다.
        TeleportNode t = (TeleportNode)target;

        // 기본 인스펙터 GUI를 그립니다.
        DrawDefaultInspector();

        // 레이블이 있는 버튼을 만듭니다.
        // 버튼을 클릭하면 내부 코드를 실행합니다.
        if (GUILayout.Button("Sorted by Number"))
        {
            if (t.ConnectedNodes.Count == 0)
            {
                Debug.LogError($"there is no connected node");
                return;
            }

            t.ConnectedNodes.Sort((node1, node2) => node1.ID.CompareTo(node2.ID));
        }

        // 인스펙터를 다시 그리도록 합니다.
        Repaint();
    }
}
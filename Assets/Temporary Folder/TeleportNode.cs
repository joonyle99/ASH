using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class TeleportNode : MonoBehaviour
{
    [SerializeField] private List<TeleportNode> connectedNodes = new List<TeleportNode>();
    public List<TeleportNode> ConnectedNodes => connectedNodes;

    /// <summary>
    /// Awake() �Լ��� Start(), Update() �Լ��� �޸� '��ũ��Ʈ'�� ��Ȱ��ȭ�ص� ȣ��ȴ�.
    /// Awake() �Լ��� ������ ��, ��ũ��Ʈ�� Ȱ��ȭ üũ�ڽ��� ���� �����̴�.
    /// ������ ���� ������Ʈ�� ��Ȱ��ȭ �Ѵٸ� ���������� ȣ����� �ʴ´�.
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
        // Unity ���� ���ο��� �� ��ü�� ������ ID�� �ο��Ѵ�
        int instanceID = this.GetInstanceID();

        // .Net���� ��ü�� �ĺ��ϴ� �뵵�� �ؽ� �ڵ带 ��ȯ�Ѵ�
        // int, float�� ���� GetHashCode()�� �ش� '�� ��ü'�� ��ȯ�Ѵ�
        // ������ claas�� ���� ���� Ÿ�Կ� ���ؼ��� '��ü�� ���� ��ġ (�޸� �ּ�)'�� ������� �ؽ� �ڵ带 ��ȯ�Ѵ�.
        // �޸� �ּ� ����̱� ������ �ش� ��ü�� �޸𸮰� ������ �� �ٸ� ��ü�� �ش� �޸𸮸� ����ϰ� �Ǹ� �ؽ� �ڵ尡 �ߺ��� �� �ִ�.
        int hashCode = this.GetHashCode();

        //Debug.Log($"{this.gameObject.name}�� �ν��Ͻ� ID: {instanceID} / �ؽ� �ڵ�: {hashCode}");

        TeleportNode targetNode = this;
        int targetInstanceID = targetNode.GetInstanceID();
        int targetHashCode = targetNode.GetHashCode();

        //Debug.Log($"{targetNode.gameObject.name}�� �ν��Ͻ� ID: {targetInstanceID} / �ؽ� �ڵ�: {targetHashCode}");

        int a = 512125;
        int b = 512125;
        string str = "hello";
        string str2 = "hello";

        Debug.Log($"a: {a.GetHashCode()} b: {b.GetHashCode()} str: {str.GetHashCode()} str2: {str2.GetHashCode()}");
        */
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

#if UNITY_EDITOR
[CustomEditor(typeof(TeleportNode), true)]
public class TeleportNodeEditor : Editor
{
    // Unity �����Ϳ��� �ν����� GUI�� �׸��� �޼����Դϴ�.
    // OnInspectorGUI �޼��带 �������̵��Ͽ� Ŀ���� GUI�� �����մϴ�.
    public override void OnInspectorGUI()
    {
        // ���� �ν����Ϳ��� ���� ���� TeleportNode '��ü'�� �����ɴϴ�.
        TeleportNode t = (TeleportNode)target;

        // �⺻ �ν����� GUI�� �׸��ϴ�.
        DrawDefaultInspector();

        // ���̺��� �ִ� ��ư�� ����ϴ�.
        // ��ư�� Ŭ���ϸ� ���� �ڵ带 �����մϴ�.
        if (GUILayout.Button("Sorted by Number"))
        {
            if (t.ConnectedNodes.Count == 0)
            {
                Debug.LogError($"there is no connected node");
                return;
            }

            // ID�� ������
            // t.ConnectedNodes.Sort((node1, node2) => node1.ID.CompareTo(node2.ID));
        }

        // �ν����͸� �ٽ� �׸����� �մϴ�.
        Repaint();
    }
}
#endif
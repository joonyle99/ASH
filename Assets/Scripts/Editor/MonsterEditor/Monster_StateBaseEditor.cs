using UnityEditor;

[CustomEditor(typeof(Monster_StateBase), true)]      // ��ӹ޴� ��� Ŭ������ ���� Ŀ���� �����Ͱ� ���ȴ�.
// [CustomEditor(typeof(Monster_StateBase))]                            // �ش� Ŭ������ ���� Ŀ���� �����͸� ����Ѵ�.
public class Monster_StateBaseEditor : Editor
{
    private void OnEnable()
    {
        EditorApplication.update -= ForceRepaint;
        EditorApplication.update += ForceRepaint;
    }
    private void OnDisable()
    {
        EditorApplication.update -= ForceRepaint;
    }

    void ForceRepaint()
    {
        // �����͸� ������ ������Ʈ�Ѵ�.
        Repaint();
    }

    public override void OnInspectorGUI()
    {
        // Serialized Object�� ������Ʈ
        serializedObject.Update();

        SerializedProperty isAutoStateTransition = serializedObject.FindProperty("isAutoStateTransition");
        if (!isAutoStateTransition.boolValue)
        {
            SerializedProperty iterator = serializedObject.GetIterator();
            while (iterator.NextVisible(true))
            {
                // isAutoStateTransition ������ ǥ��
                switch (iterator.name)
                {
                    case "isAutoStateTransition":
                        EditorGUILayout.PropertyField(iterator);
                        break;
                }
            }
        }
        else
        {
            base.OnInspectorGUI();
        }

        // ���� ���� ����
        serializedObject.ApplyModifiedProperties();
    }
}

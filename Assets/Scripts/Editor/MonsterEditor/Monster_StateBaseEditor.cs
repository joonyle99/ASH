using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Monster_StateBase), true)]     // 상속받는 모든 클래스에 대해 커스텀 에디터가 사용된다.
// [CustomEditor(typeof(Monster_StateBase))]        // 해당 클래스에 대한 커스텀 에디터를 사용한다.
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
        // 에디터를 강제로 리페인트한다.
        Repaint();
    }

    public override void OnInspectorGUI()
    {
        // SerializedObject를 업데이트
        serializedObject.Update();

        SerializedProperty isAutoStateTransition = serializedObject.FindProperty("isAutoStateTransition");
        if (!isAutoStateTransition.boolValue)
        {
            SerializedProperty iterator = serializedObject.GetIterator();
            while (iterator.NextVisible(true))
            {
                // 특정 프로퍼티를 숨긴다.
                switch (iterator.name)
                {
                    case "targetTransitionParam":
                    case "_minStayTime":
                    case "_maxStayTime":
                    case "targetStayTime":
                    case "elapsedStayTime":
                        continue;
                    default:
                        EditorGUILayout.PropertyField(iterator, true);
                        break;
                }
            }
        }
        else
        {
            base.OnInspectorGUI();
        }

        // 변경 사항 적용
        serializedObject.ApplyModifiedProperties();
    }
}

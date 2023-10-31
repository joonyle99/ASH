using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//From TMPro.Examples.VertexJitter

public class TextShaker : MonoBehaviour
{
    public float AngleMultiplier = 1.0f;
    public float MoveMultiplier = 1.0f;
    public float SpeedMultiplier = 1.0f;

    private TMP_Text _textComponent;
    private bool hasTextChanged;

    Coroutine _shakeCoroutine;

    public TextShakeParams shakeParams 
    { 
        set
        {
            AngleMultiplier = value.RotationPower;
            MoveMultiplier = value.MovePower;
            SpeedMultiplier = value.Speed;
        } 
    }

    /// <summary>
    /// Structure to hold pre-computed animation data.
    /// </summary>
    private struct VertexAnim
    {
        public float angleRange;
        public float angle;
        public float speed;
    }

    void Awake()
    {
        _textComponent = GetComponent<TMP_Text>();
    }

    void OnEnable()
    {
        // Subscribe to event fired when text object has been regenerated.
        TMPro_EventManager.TEXT_CHANGED_EVENT.Add(ON_TEXT_CHANGED);
    }
    void OnDisable()
    {
        TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(ON_TEXT_CHANGED);
    }
    public void StartShake()
    {
        if(_shakeCoroutine == null)
            _shakeCoroutine = StartCoroutine(ShakeText());
    }
    public void StopShake()
    {
        if (_shakeCoroutine != null)
        {
            StopCoroutine(_shakeCoroutine);
            _shakeCoroutine = null;
        }
    }
    void ON_TEXT_CHANGED(Object obj)
    {
        if (obj == _textComponent)
            hasTextChanged = true;
    }
    /// <summary>
    /// Method to animate vertex colors of a TMP Text object.
    /// </summary>
    /// <returns></returns>
    IEnumerator ShakeText()
    {

        // We force an update of the text object since it would only be updated at the end of the frame. Ie. before this code is executed on the first frame.
        // Alternatively, we could yield and wait until the end of the frame when the text object will be generated.
        _textComponent.ForceMeshUpdate();

        TMP_TextInfo textInfo = _textComponent.textInfo;

        Matrix4x4 matrix;

        int loopCount = 0;
        hasTextChanged = true;

        // Create an Array which contains pre-computed Angle Ranges and Speeds for a bunch of characters.
        VertexAnim[] vertexAnim = new VertexAnim[1024];
        for (int i = 0; i < 1024; i++)
        {
            vertexAnim[i].angleRange = Random.Range(10f, 25f);
            vertexAnim[i].speed = Random.Range(1f, 3f);
        }

        // Cache the vertex data of the text object as the Jitter FX is applied to the original position of the characters.
        TMP_MeshInfo[] cachedMeshInfo = textInfo.CopyMeshInfoVertexData();

        while (true)
        {
            // Get new copy of vertex data if the text has changed.
            if (hasTextChanged)
            {
                // Update the copy of the vertex data for the text object.
                cachedMeshInfo = textInfo.CopyMeshInfoVertexData();

                hasTextChanged = false;
            }

            int characterCount = textInfo.characterCount;

            // If No Characters then just yield and wait for some text to be added
            if (characterCount == 0)
            {
                yield return new WaitForSeconds(0.25f);
                continue;
            }


            for (int i = 0; i < characterCount; i++)
            {
                TMP_CharacterInfo charInfo = textInfo.characterInfo[i];

                // Skip characters that are not visible and thus have no geometry to manipulate.
                if (!charInfo.isVisible)
                    continue;

                // Retrieve the pre-computed animation data for the given character.
                VertexAnim vertAnim = vertexAnim[i];

                // Get the index of the material used by the current character.
                int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;

                // Get the index of the first vertex used by this text element.
                int vertexIndex = textInfo.characterInfo[i].vertexIndex;

                // Get the cached vertices of the mesh used by this text element (character or sprite).
                Vector3[] sourceVertices = cachedMeshInfo[materialIndex].vertices;

                // Determine the center point of each character at the baseline.
                //Vector2 charMidBasline = new Vector2((sourceVertices[vertexIndex + 0].x + sourceVertices[vertexIndex + 2].x) / 2, charInfo.baseLine);
                // Determine the center point of each character.
                Vector2 charMidBasline = (sourceVertices[vertexIndex + 0] + sourceVertices[vertexIndex + 2]) / 2;

                // Need to translate all 4 vertices of each quad to aligned with middle of character / baseline.
                // This is needed so the matrix TRS is applied at the origin for each character.
                Vector3 offset = charMidBasline;

                Vector3[] destinationVertices = textInfo.meshInfo[materialIndex].vertices;

                destinationVertices[vertexIndex + 0] = sourceVertices[vertexIndex + 0] - offset;
                destinationVertices[vertexIndex + 1] = sourceVertices[vertexIndex + 1] - offset;
                destinationVertices[vertexIndex + 2] = sourceVertices[vertexIndex + 2] - offset;
                destinationVertices[vertexIndex + 3] = sourceVertices[vertexIndex + 3] - offset;

                vertAnim.angle = Mathf.SmoothStep(-vertAnim.angleRange, vertAnim.angleRange, Mathf.PingPong(loopCount / 25f * vertAnim.speed, 1f));
                Vector3 jitterOffset = new Vector3(Random.Range(-.25f, .25f), Random.Range(-.25f, .25f), 0);

                matrix = Matrix4x4.TRS(jitterOffset * MoveMultiplier, Quaternion.Euler(0, 0, Random.Range(-5f, 5f) * AngleMultiplier), Vector3.one);

                destinationVertices[vertexIndex + 0] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 0]);
                destinationVertices[vertexIndex + 1] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 1]);
                destinationVertices[vertexIndex + 2] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 2]);
                destinationVertices[vertexIndex + 3] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 3]);

                destinationVertices[vertexIndex + 0] += offset;
                destinationVertices[vertexIndex + 1] += offset;
                destinationVertices[vertexIndex + 2] += offset;
                destinationVertices[vertexIndex + 3] += offset;

                vertexAnim[i] = vertAnim;
            }

            // Push changes into meshes
            for (int i = 0; i < textInfo.meshInfo.Length; i++)
            {
                textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
                _textComponent.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
            }

            loopCount += 1;
            if (SpeedMultiplier <= 0)
                yield return new WaitForSeconds(0.1f);
            else
                yield return new WaitForSeconds(0.1f / SpeedMultiplier);
        }
    }

}
#pragma warning disable CS0660 // 형식은 == 연산자 또는 != 연산자를 정의하지만 Object.Equals(object o)를 재정의하지 않습니다.
#pragma warning disable CS0661 // 형식은 == 연산자 또는 != 연산자를 정의하지만 Object.GetHashCode()를 재정의하지 않습니다.
public struct TextShakeParams
{
    public static TextShakeParams None { get { return new TextShakeParams(0, 0, 0); } }

    public float RotationPower;
    public float MovePower;
    public float Speed;

    public TextShakeParams(float rotationPower = 1, float movePower = 1, float speed = 1)
    {
        RotationPower = rotationPower;
        MovePower = movePower;
        Speed = speed;
    }
    public static bool operator ==(TextShakeParams a, TextShakeParams b)
    {
        return a.MovePower == b.MovePower && a.RotationPower == b.RotationPower &&
                a.Speed == b.Speed;
    }
    public static bool operator !=(TextShakeParams a, TextShakeParams b)
    {
        return !(a == b);
    }
}

#pragma warning restore CS0661 // 형식은 == 연산자 또는 != 연산자를 정의하지만 Object.GetHashCode()를 재정의하지 않습니다.
#pragma warning restore CS0660 // 형식은 == 연산자 또는 != 연산자를 정의하지만 Object.Equals(object o)를 재정의하지 않습니다.
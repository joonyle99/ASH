using System.Collections;
using UnityEngine;

public class Tornado : MonoBehaviour
{
    public IEnumerator TornadoAnimation()
    {
        // 크기가 커진다 (0, 0, 2.3) -> (2.3, 2.3, 2.3)

        // N초 대기

        // 속도가 빨라지면서 밝아진다

        // N초 대기

        // 다시 느려지면서 어두워지고 크기가 작아진다

        // 오브젝트 삭제

        yield return null;
    }
}

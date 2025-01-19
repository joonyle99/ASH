using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeySettingBox : MonoBehaviour
{
    //키세팅 변경 시 키세팅 버튼을 클릭하게 되면,
    //OnChangedKeyboardSetting에서 키 입력을 받고 그 즉시 다시 OnClick버튼이 클릭되어 로직이 재실행 됨.
    //이를 확인하기 위한 변수
    public int MouseClickCount = 0;
}

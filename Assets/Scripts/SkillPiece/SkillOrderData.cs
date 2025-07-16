using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SkillData
{
    public string Key;
    public Sprite PieceIcon;
    public Sprite UnlockImage;
    public string NameId;
    public string DetailTextId;
    //public int RequiredPieceCount;

    public SkillData(SkillData skillData)
    {
        Key = skillData.Key;
        PieceIcon = skillData.PieceIcon;
        UnlockImage = skillData.UnlockImage;
        NameId = skillData.NameId;
        DetailTextId = skillData.DetailTextId;
    }
}

[System.Serializable]
public struct SkillElement
{
    public string Key;
    public SkillData Data;
}

[CreateAssetMenu(fileName ="New Skill Data", menuName ="Skill Order Data")]
public class SkillOrderData : ScriptableObject
{
    [SerializeField] private List<SkillData> _skillDatas=  new List<SkillData>();
    [SerializeField] private List<SkillElement> _skillDictionary = new List<SkillElement>();

    public SkillData this[int i]
    {
        get
        {
            if (i < _skillDatas.Count)
            {
                return _skillDatas[i];
            }

            Debug.Log("스킬 획득 가능 횟수 초과");

            return _skillDatas[^1];
        }
    }

    public SkillData GetFromDict(string key)
    {
        return _skillDictionary.Find(x=> x.Key == key).Data;
    }
}

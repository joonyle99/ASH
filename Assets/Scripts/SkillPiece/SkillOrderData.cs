using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SkillData
{
    public string Key;
    public Sprite PieceIcon;
    public Sprite UnlockImage;
    public string Name;
    public string DetailText;
    //public int RequiredPieceCount;
}

[CreateAssetMenu(fileName ="New Skill Data", menuName ="Skill Order Data")]
public class SkillOrderData : ScriptableObject
{
    [System.Serializable]
    public struct SkillElement
    {
        public string Key;
        public SkillData Data;
    }
    [SerializeField] List<SkillData> _skillDatas=  new List<SkillData>();
    [SerializeField] List<SkillElement> _skillDictionary = new List<SkillElement>();
    public SkillData this[int i]
    {
        get
        {
            if (i<_skillDatas.Count)
                return _skillDatas[i];
            else
                return _skillDatas[_skillDatas.Count-1];
        }
    }
    public SkillData GetFromDict(string key)
    {
        return _skillDictionary.Find(x=> x.Key == key).Data;
    }
}

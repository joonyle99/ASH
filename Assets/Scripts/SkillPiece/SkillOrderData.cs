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

            Debug.LogWarning("½ºÅ³È¹µæ °¡´É È½¼ö ÃÊ°ú !!");
            return _skillDatas[^1];
        }
    }

    public SkillData GetFromDict(string key)
    {
        return _skillDictionary.Find(x=> x.Key == key).Data;
    }
}

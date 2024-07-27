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

    public SkillData(SkillData skillData)
    {
        Key = skillData.Key;
        PieceIcon = skillData.PieceIcon;
        UnlockImage = skillData.UnlockImage;
        Name = skillData.Name;
        DetailText = skillData.DetailText;
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

            Debug.Log("½ºÅ³ È¹µæ °¡´É È½¼ö ÃÊ°ú");

            return _skillDatas[^1];
        }
    }

    public SkillData GetFromDict(string key)
    {
        return _skillDictionary.Find(x=> x.Key == key).Data;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObtainInfoData : MonoBehaviour
{
    [SerializeField] ItemObtainPanel.ItemObtainInfo _itemObtainInfo = new ItemObtainPanel.ItemObtainInfo();

    public void OpenItemObtainPanel()
    {
        GameUIManager.OpenItemObtainPanel(_itemObtainInfo);
    }
}

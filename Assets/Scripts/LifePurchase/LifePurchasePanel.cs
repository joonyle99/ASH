using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

public class LifePurchasePanel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _priceText;
    [SerializeField] TextMeshProUGUI _countText;
    [SerializeField] TextMeshProUGUI _availableText;
    [SerializeField] Button _increaseButton;
    [SerializeField] Button _decreaseButton;
    [SerializeField] TextMeshProUGUI _totalPriceText;

    int _currentCount = 1;
    int _price = 100;
    void UpdateUIs()
    {
        int currentGold = PersistentDataManager.GetByGlobal<int>("gold");
        _availableText.text = (PersistentDataManager.GetByGlobal<int>("gold") / _price).ToString();
        _priceText.text = _price.ToString();
        _countText.text = _currentCount.ToString();
        _totalPriceText.text = (_price * _currentCount).ToString();
        _decreaseButton.interactable = _currentCount > 0 && currentGold >= (_currentCount - 1) * _price;
        _increaseButton.interactable = currentGold >= (_currentCount + 1) * _price;
    }
    public void Open()
    {
        _currentCount = PersistentDataManager.GetByGlobal<int>("gold") / _price;
        UpdateUIs();
        gameObject.SetActive(true);
    }
    public void AddCount(int count)
    {
        _currentCount += count;
        UpdateUIs();
    }
    public void Close()
    {
        gameObject.SetActive(false);
    }
    public void Purchase()
    {
        PersistentDataManager.UpdateValueByGlobal<int>("gold", x => x - _currentCount * _price);
        // TODO : life +
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MarketItem : MonoBehaviour
{
    public int itemID, wearID;
    public int price;

    public Button buyButton, equipButton, unequipButton;
    public Text priceText;

    public GameObject itemPrefab;


    public bool HasItem()
    {
        // 0 : Daha Satýn alýnmamýþ
        // 1 : Satýn alýnmýþ ama giyilmemiþ
        // 2 : Hem satýn alýnmýþ hem de giyinmiþ
        bool hasItem = PlayerPrefs.GetInt("item" + itemID.ToString()) != 0;
        return hasItem;
    }

    public bool IsEquipped()
    {

        bool equippedItem = PlayerPrefs.GetInt("item" + itemID.ToString()) != 0;
        return equippedItem;
    }

    public void InitializeItem()
    {
        priceText.text = price.ToString();
        if(HasItem())
        {
            buyButton.gameObject.SetActive(false);
            if(IsEquipped())
            {
                EquipItem();
            }
            else
            {
                equipButton.gameObject.SetActive(true);
            }
        }
        else
        {
            buyButton.gameObject.SetActive(true);
        }
    }


    public void BuyItem()
    {
        if(!HasItem()) 
        {
            int money = PlayerPrefs.GetInt("money");
            if(money >= price)
            {
                PlayerController.Current.ItemAudioSource.PlayOneShot(PlayerController.Current.buyAudioClip, 0.1f);
                LevelController.Current.GiveMoneyToPlayer(-price);
                PlayerPrefs.SetInt("item" + itemID.ToString(),1);
                buyButton.gameObject.SetActive(false);
                equipButton.gameObject.SetActive(true);
            }
        }
    }

    public void EquipItem()
    {
        UnequipItem();
        MarketController.Current.equippedItem[wearID] = Instantiate(itemPrefab, PlayerController.Current.wearSpots[wearID].transform).GetComponent<Item>();
        MarketController.Current.equippedItem[wearID].itemID = itemID;
        equipButton.gameObject.SetActive(false);
        unequipButton.gameObject.SetActive(true);
        PlayerPrefs.SetInt("item" + itemID.ToString(), 2);

    }

    public void UnequipItem()
    {
        Item equippedItem = MarketController.Current.equippedItem[wearID];
        if (equippedItem != null)
        {
            MarketItem marketItem = MarketController.Current.items[equippedItem.itemID];
            PlayerPrefs.SetInt("item" + marketItem.itemID, 1);
            marketItem.equipButton.gameObject.SetActive(true);
            marketItem.unequipButton.gameObject.SetActive(false);
            Destroy(equipButton.gameObject);
        }
    }

    public void EuipItemButton()
    {
        PlayerController.Current.ItemAudioSource.PlayOneShot(PlayerController.Current.equipItemAudioClip, 0.1f);
        EquipItem();
    }

    public void UneuipItemButton()
    {
        PlayerController.Current.ItemAudioSource.PlayOneShot(PlayerController.Current.unequipItemAudioClip, 0.1f);
        UnequipItem();
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class InventoryItem : MonoBehaviour
{

    public Inventory inventorySO;

    public string itemName;

    public GameObject inventoryObject;

    public AudioSource inventoryItemSound;

    public CanvasGroup youHaveCollectedxGroup;

    public float blendInOutYouHaveCollectedxHintSpeed;

    public delegate void ItemHasBeenCollected();
    public static event ItemHasBeenCollected OnItemHasBeenCollected;
    public delegate void BlendOutRoutineFinished();
    public static event BlendOutRoutineFinished OnBlendOutRoutineFinished;

    public static bool isItemCollected;

    private void Awake()
    {
        youHaveCollectedxGroup.alpha = 0f;
       OnItemHasBeenCollected += ChangeMaskToDefault;
    }
    //Change the layer before the gameobject gets deactivated, so the player cant interact again when he chose to collect the item
    void ChangeMaskToDefault()
    {
        gameObject.layer = LayerMask.NameToLayer("Default");    
    }

    public void AddItemToInventory()
    {
        inventorySO.AddItemToList(itemName);
        inventoryObject.SetActive(false);
        OnItemHasBeenCollected.Invoke();
        isItemCollected = true;
        BlendInYouHaveCollectedTxt();
    }
    void BlendInYouHaveCollectedTxt()
    {
        youHaveCollectedxGroup.GetComponentInChildren<TextMeshProUGUI>().maxVisibleCharacters = youHaveCollectedxGroup.GetComponentInChildren<TextMeshProUGUI>().textInfo.characterCount;

        StartCoroutine(ShowYouHaveCollectedItemblendInRoutine());
    }
    void BlendOutYouHaveCollectedTxt()
    {
        StartCoroutine(ShowYouHaveCollectedItemblendOutRoutine());

    }

    IEnumerator ShowYouHaveCollectedItemblendInRoutine()
    {
        while (youHaveCollectedxGroup.alpha < .999f)
        {
            youHaveCollectedxGroup.alpha = Mathf.Lerp(youHaveCollectedxGroup.alpha, 1, blendInOutYouHaveCollectedxHintSpeed * Time.deltaTime);
            yield return null;
        }
        BlendOutYouHaveCollectedTxt();

    }
    IEnumerator ShowYouHaveCollectedItemblendOutRoutine()
    {
        while (youHaveCollectedxGroup.alpha >= 0.015f)
        {
            youHaveCollectedxGroup.alpha = Mathf.Lerp(youHaveCollectedxGroup.alpha, 0, blendInOutYouHaveCollectedxHintSpeed * Time.deltaTime);
            yield return null;
        }
        if(OnBlendOutRoutineFinished != null)
        OnBlendOutRoutineFinished.Invoke();
    }


}

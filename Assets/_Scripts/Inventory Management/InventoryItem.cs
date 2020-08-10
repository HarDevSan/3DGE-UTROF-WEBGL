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

    private void Awake()
    {
        youHaveCollectedxGroup.alpha = 0;
   
    }
  
    public void AddItemToInventory()
    {
        inventorySO.AddItemToList(itemName);
        inventoryObject.SetActive(false);
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
        while (youHaveCollectedxGroup.alpha < 0.999)
        {
            youHaveCollectedxGroup.alpha = Mathf.Lerp(youHaveCollectedxGroup.alpha, 1, blendInOutYouHaveCollectedxHintSpeed * Time.deltaTime);
            yield return null;
        }
        BlendOutYouHaveCollectedTxt();

    }
    IEnumerator ShowYouHaveCollectedItemblendOutRoutine()
    {

        while (youHaveCollectedxGroup.alpha > 0.001)
        {
            youHaveCollectedxGroup.alpha = Mathf.Lerp(youHaveCollectedxGroup.alpha, 0, blendInOutYouHaveCollectedxHintSpeed * Time.deltaTime);
            yield return null;
        }
    }


}

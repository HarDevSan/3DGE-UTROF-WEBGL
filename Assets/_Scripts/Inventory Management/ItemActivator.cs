using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemActivator : SceneTransitionActivator
{

    private void Awake()
    {
        InventoryItem.OnItemHasBeenCollected += ChangeMaskToDefault;
        //TextEvent_withCollectionPrompt.OnButtonsAreBlendedOutAfterPlayerChoseYes += DisableGameObject;
    }

    void ChangeMaskToDefault()
    {
        gameObject.layer = LayerMask.NameToLayer("Default");
    }

 

    //More harm than, it does not need to be disabled, chaning layermask suffices
    //void DisableGameObject()
    //{
    //    gameObject.SetActive(false);
    //}

    public override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);
        //if (InventoryItem.isItemCollected)
        //   // DisableGameObject();
    }

    private void OnDisable()
    {
        InventoryItem.OnItemHasBeenCollected -= ChangeMaskToDefault;
    }
}

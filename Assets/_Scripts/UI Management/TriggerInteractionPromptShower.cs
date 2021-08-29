using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*Class shows an icon hinting that an interaction is possible. It does so via asking a static bool from the PlayerController class.
 * A bool flip flop construct needs to be used to only trigger the Coroutines that are responsible for the Blending I/O only once.
 */
public class TriggerInteractionPromptShower : MonoBehaviour
{
    //can be hand or door icon with a canvas group attached
    public CanvasGroup interactGRP;

    [RangeAttribute(0, .5f)]
    public float blendInSpeed = 0;
    [RangeAttribute(0, .5f)]
    public float blendOutSpeed = 0;

    bool isHasSeenOnce;

    private void Update()
    {
        //Flip Flop and trigger Coroutine only once, depending on the first frame the player starts to see or unsee something
        if (isHasSeenOnce == false)
        {
            if (PlayerController.isPlayerCanInteractBecauseHeLooksAtSmth_item)
            {
                StartCoroutine(blendInInteractionPrompt());
                isHasSeenOnce = true;
            }
        }
        else if (isHasSeenOnce == true)
        {
            if (PlayerController.isPlayerCanInteractBecauseHeLooksAtSmth_item == false)
            {
                StartCoroutine(blendOutInteractionPrompt());
                isHasSeenOnce = false;
            }
        }

        //Blend In/Out interacton icon
        IEnumerator blendInInteractionPrompt()
        {
            float start = 0;
            float end = 1;
            float value = 0;
            float t = 0;

            while (value < end)
            {
                t += blendInSpeed;
                value = Mathf.Lerp(start, end, t);
                interactGRP.alpha = value;
                yield return null;
            }

            yield return null;
        }
        IEnumerator blendOutInteractionPrompt()
        {
            float start = 1;
            float end = 0;
            float value = 1;
            float t = 0;

            while (value > end)
            {
                t += blendOutSpeed;
                value = Mathf.Lerp(start, end, t);
                interactGRP.alpha = value;
                yield return null;
            }

            yield return null;
        }
    }
}






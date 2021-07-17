using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/* Note that this class in its current state does not differ from the also included lass called "Scripted Events". but it could potentially be more specialized for the task of adding horror elements than generalized events. Fear Emitter == Scripted Events though.
 * It is a technqiue adapted from Electronic Arts and Visceral Games's Dead Space series. The game design theory is that any element like soundFX or visualFX 
 * shall occur when the player enters certain areas or spots of interest. It could also be somehtign simple like a sound effect playing when the players enters a room
 * opens a door.
 */

public class FearEmitter : MonoBehaviour
{
    /*Serialized events in Unity makes this process acessable and propagates the complexity to the edirs hierarchy and inspector instead of hard coupling elements through
     * code.This makes it possible to modularly stack up these "Fear emitters". The emitter does not care what calls it. Any function can be used to trigger the Unit 
     * Event. the benefit of this approach is that seemingly complex dynamic behavior and interactions in the game world can be created through minimal coupling and complexity. Furthermore it is non destructive through its modular approach, which offers a lot of control at editing time.
     */
    public UnityEvent playFearEmitter;

    //List of evetns that can be wired to other functiosn to trigger THIS Unity Event
    public delegate void PlayerPressedUsedStartEmitter();
    public static event PlayerPressedUsedStartEmitter OnPlayerPressedUsedStartEmitter;

    //check if the player is in proimity via the trigger component's callback
    bool isPlayerInsideTrigger;

    private void Start()
    {
        //See function for details
        InputReceiver.On_F_Inpu += StartFearEmitterOnPlayerPressedUse;
    }

    /* More specific example of using the emitter. It can catch other events to occur in the game world, e.g. the player pressign the use key at a specific location.
     * than it can trigger Unity event to play whatever other functions or components are attached to it. This could be other function calls or component behaviour like
     * playing an animation clip.
     */
    void StartFearEmitterOnPlayerPressedUse()
    {
        if(isPlayerInsideTrigger)
        playFearEmitter.Invoke();
    }

    /* This is the simplest form of execution. On entering the assigned trigger, all assigned functions or animations get fired.
     * More complex behaviour could be authored when other functions, see functions above. this behavior can be bypassed by simply not addign a trigger to THIS object.
     */
    private void OnTriggerEnter(Collider other)
    {
        playFearEmitter.Invoke();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
            isPlayerInsideTrigger = true;
        else
        {
            isPlayerInsideTrigger = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
            isPlayerInsideTrigger = false;
        else
        {
            isPlayerInsideTrigger = true;
        }
    }


}

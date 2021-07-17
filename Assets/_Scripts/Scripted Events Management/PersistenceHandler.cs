using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class should handle object and gameplay states and stats between scenes.
public class PersistenceHandler : MonoBehaviour
{
    //Example could be that the player could have a hallucinations only once, and when he returns form the next scene,this halucination should have disappeared.
    public static bool isSHadowMenAlreadAppered;
}

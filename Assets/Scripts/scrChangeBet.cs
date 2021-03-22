using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scrChangeBet : MonoBehaviour
{
    public GameObject PlayArea;
   /* private static Script PlayAreaScriptComponent = */
   
        

    
    public void IncreaseBet() {
        var PlayAreaScript = PlayArea.GetComponent<scrPlayArea>();
        PlayAreaScript.IncreaseBet();
    }

    public void DecreaseBet() {
        var PlayAreaScript = PlayArea.GetComponent<scrPlayArea>();
        PlayAreaScript.DecreaseBet();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scrPlayBtn : MonoBehaviour
{
    public GameObject PlayArea;

    public void OnClick() {        
        var playAreaScript = PlayArea.GetComponent<scrPlayArea>();
        // Remove the bet and start the game
        playAreaScript.StartGame();        
    }
}

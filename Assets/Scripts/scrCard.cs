using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class scrCard : MonoBehaviour
{   
    public Sprite CardSprite;
    public Sprite FaceSprite;
    private GameObject PlayArea;

    private Image CardImage;
    private GameObject ButtonText;
    private bool CardFlipped = false;

    public void Start() {
        CardImage = gameObject.GetComponent<Image>();       
        ButtonText = gameObject.transform.Find("Button").transform.Find("Text").gameObject;
        ResetCard();
    }

    public void onClick() {
        var PlayAreaScript = PlayArea.GetComponent<scrPlayArea>();

        if (!CardFlipped && PlayAreaScript.IsGameInProgress()) {
            // Get the value for the card and display it
           
            var Prize = PlayAreaScript.GetNextPrize();            
            PlayAreaScript.IncreaseBalance(Prize);
            ShowCardFace(Prize);

            // Check for finish condition.
            if (Prize == 0.0f) {
                //Todo: Show loss text
                PlayAreaScript.EndGame();
            }

        }
    }    

    public void ShowCardFace(float prize) {
        CardFlipped = true;
        CardImage.sprite = FaceSprite;        
        ButtonText.GetComponent<Text>().text = "$" + prize.ToString("F2");
        ButtonText.SetActive(true);
        return;
    }


    public void ResetCard() {        
        CardFlipped = false;
        CardImage.sprite = CardSprite;
        ButtonText.SetActive(false);
    }

    public void SetPlayArea(GameObject NewPlayArea) {
        PlayArea = NewPlayArea;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scrCardArea : MonoBehaviour
{
    public GameObject Card;
    public GameObject PlayArea;
   
    private List<GameObject> Cards = new List<GameObject>();
    private bool FirstRun = true;
    
    
    
    public void SetupCards() {
        if (FirstRun) {
            FirstRun = false;
            var MaxCards = PlayArea.GetComponent<scrPlayArea>().MaxCards;

            for (int NumCards = 0; NumCards < MaxCards; NumCards++) {
                var NewCard = Instantiate(Card, new Vector3(0, 0, 0), Quaternion.identity);
                NewCard.GetComponent<scrCard>().SetPlayArea(PlayArea);
                NewCard.transform.SetParent(gameObject.transform, false);
                Cards.Add(NewCard);

            }
            return;
        }

        foreach (var Card in Cards) {
            Card.GetComponent<scrCard>().ResetCard();
        }
    } 
    
    public void ResetCards() {
        foreach (var Card in Cards) {
            Card.transform.SetParent(null);
            Destroy(Card);
            
        }

        Cards = new List<GameObject>();
        FirstRun = true;
    }
}

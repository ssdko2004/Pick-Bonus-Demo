using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.UI;

public class scrPlayArea : MonoBehaviour {
    public GameObject CardArea;
    public GameObject BalanceText;
    public GameObject CurrentBetText;
    public GameObject LastWinText;
    public GameObject LossImage;
    public Button PlayButton;
    public Button IncreaseButton;
    public Button DecreaseButton;
    public Button NewGameButton;
    public float StartingBalance = 10.00f;


    private float Balance;
    private float LastWin;
    private float CurrentBet;    
    private float CurrentPrize = 0.0f;
    private bool GameInProgress = false;   
    private int PrizePickedCount = 0;
    public int MaxCards = 9;

    private List<float> CurrentPrizeList;

    //Might be better ways to store this so it is easier to expand
    private static List<int> ChanceList = new List<int>() { 50, 80, 95, 100 };
    private static Dictionary<int, List<float>> WinMultipliers = new Dictionary<int, List<float>>() {
        {ChanceList[0], new List<float>() {0.0f} },
        {ChanceList[1], new List<float>() {1.0f, 2.0f, 3.0f, 4.0f, 5.0f, 6.0f, 7.0f, 8.0f, 9.0f, 10.0f} },
        {ChanceList[2], new List<float>() {12.0f, 16.0f, 24.0f, 32.0f, 48.0f, 64.0f } },
        {ChanceList[3], new List<float>() {100.0f, 200.0f, 300.0f, 400.0f, 500.0f } }
    };

    private static int SelectedBetIndex = 0;
    private static List<float> ValidBets = new List<float>() { 0.25f, 0.50f, 1.00f, 2.00f };



    // Start is called before the first frame update
    void Start()
    {
        ResetGame();
    }    

    public void ResetGame() {
        var CardAreaScript = CardArea.GetComponent<scrCardArea>();
        CardAreaScript.ResetCards();
        SetBalance(StartingBalance);
        SetLastWin(0.00f);
        ResetBet();
        PrizePickedCount = 0;
        GameInProgress = false;
        ToggleButtons(true);
        ToggleLossScreen(false);
    }

    public void StartGame() {
        GameInProgress = true;
        PrizePickedCount = 0;
        ToggleButtons(false);
        DecreaseBalance(CurrentBet);
        DealCards();
    }

    public void EndGame() {
        SetLastWin(CurrentPrize);
        ToggleButtons(true);
        ResetBet();
        GameInProgress = false;

        // Handle an empty Balance
        if (Balance == 0.0f) {
            ToggleLossScreen(true);
            ToggleButtons(false);
        }
    }

    public void ToggleButtons(bool enable) {
        if (PlayButton) { 
            PlayButton.GetComponent<Button>().interactable = enable;
        }
        if (IncreaseButton) {
            IncreaseButton.GetComponent<Button>().interactable = enable;
        }
        if (DecreaseButton) { 
            DecreaseButton.GetComponent<Button>().interactable = enable;
        }
    }

    public void ToggleLossScreen(bool enable) {
        if (LossImage) {
            LossImage.SetActive(enable);
        }
    }

    public float GetPrizeMultiplier() {

        // Determine the max win for this round
        var RandChance = Random.Range(0, 100) + 1;
        List<float> PrizeList = new List<float>();

        foreach (var Key in ChanceList) { 
            if (RandChance <= Key) {               

                PrizeList = WinMultipliers[Key];
                break;
            }
        }
        var RandChanceTwo = (int) Random.Range(0, PrizeList.Count);
        return PrizeList[RandChanceTwo];
    }

    public List<float> GetPrizes(float PrizeMultiplier) {
        //Divide into 9, with at least 1 'pooper'
        if (PrizeMultiplier == 0.0f) {
            CurrentPrize = 0;
            return new List<float>() {0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f};
        }

        var TotalPrize = CurrentBet * PrizeMultiplier;
        CurrentPrize = TotalPrize; // Save to add to the last win later

        // See how many times it can be divided by $.05
        var MaxGoodPrizes = (TotalPrize / 0.05f) < 8.0f ? (int)(TotalPrize / 0.05f) : 8;
        var PrizeList = new List<float>();
        
        // Distribute into smaller prizes
        var MaxPrizeRow = MaxGoodPrizes;
        while (MaxPrizeRow > 0) {
            for (int i = 0; i < MaxPrizeRow && TotalPrize > 0; i++) {
                TotalPrize -= 0.25f;
                if (PrizeList.Count <= MaxPrizeRow) {
                    PrizeList.Add(0.25f);
                    continue;
                }

                PrizeList[i] += 0.25f;
            }
            MaxPrizeRow--;
        }

        //Randomize the order
        var PrizeIndex = PrizeList.Count;
        while (PrizeIndex > 1) {
            PrizeIndex--;
            var NewIndex = Random.Range(0, PrizeIndex + 1);
            var Prize = PrizeList[NewIndex];
            PrizeList[NewIndex] = PrizeList[PrizeIndex];
            PrizeList[PrizeIndex] = Prize;
        }

        // Set poopers;
        var NeededPoopers = 9 - MaxGoodPrizes;
        for (int i = 0; i < NeededPoopers; i++) {
            PrizeList.Add(0.0f); // The pooper
        }

        PrizeList[0] += TotalPrize;
        
        return PrizeList;
    }

    public void DealCards() {
        var PrizeMultiplier = GetPrizeMultiplier();        
        CurrentPrizeList = GetPrizes(PrizeMultiplier);
        var CardAreaScript = CardArea.GetComponent<scrCardArea>();
        CardAreaScript.SetupCards();
    }

    public void SetBalance(float newBalance) {
        Balance = newBalance;
        BalanceText.GetComponent<Text>().text = '$' + Balance.ToString("F2");
    }

    public float GetBalance() {
        return Balance;
    }

    public void IncreaseBalance(float amount) {
        Balance += amount;
        if (Balance < 0.0f) {
            Balance = 0.0f;
        }
        BalanceText.GetComponent<Text>().text = '$' + Balance.ToString("F2");
    }

    public void DecreaseBalance(float amount) {        
        IncreaseBalance(-amount);
    }    

    public void SetLastWin(float newLastWin) {
        LastWin = newLastWin;
        LastWinText.GetComponent<Text>().text = '$' + LastWin.ToString("F2");
    }
 
    public void IncreaseLastWin(float amount) {
        LastWin += amount;
        if (LastWin < 0.0f) {
            LastWin = 0.0f;
        }
        LastWinText.GetComponent<Text>().text = '$' + LastWin.ToString("F2");
    }

    public void DecreaseLastWin(float amount) {
        IncreaseLastWin(-amount);
    }

    public void IncreaseBet() {
        if (SelectedBetIndex < ValidBets.Count - 1 && ValidBets[SelectedBetIndex + 1] <= Balance) {
            SelectedBetIndex++;
            UpdateBet();
        }
    }

    public void DecreaseBet() {
        if (SelectedBetIndex > 0) {
            SelectedBetIndex--;
            UpdateBet();
        }
    }

    private void UpdateBet() {
        CurrentBet = ValidBets[SelectedBetIndex];
        CurrentBetText.GetComponent<Text>().text = '$' + CurrentBet.ToString("F2");
    }    

    public void ResetBet() {
        SelectedBetIndex = 0;
        UpdateBet();
    }

    public float GetCurrentPrize() {
        return CurrentPrize;
    }

    // Used by card when displaying the prize recieved prize
    public float GetNextPrize() {
        if (CurrentPrizeList.Count == 0 || PrizePickedCount == MaxCards) {
            //Todo: cause an exception
            return 0;
        }

        return CurrentPrizeList[PrizePickedCount++];
    }

    public bool IsGameInProgress() {
        return GameInProgress;
    }

}

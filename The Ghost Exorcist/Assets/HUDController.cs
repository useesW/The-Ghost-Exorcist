using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.UI;
using TMPro;

public class HUDController : MonoBehaviour
{

    public TMP_Text ghostKilledCounter;
    public TMP_Text ghostRemainingCounter;

    public void SetGhostCounter(int totalNum, int maxNum){
        ghostKilledCounter.text = "Ghosts Killed: " + totalNum;
        ghostRemainingCounter.text = maxNum + " Ghosts Remain";
    }

    public TMP_Text totemsCollectedCounter;
    public TMP_Text totemsRemainingCounter;

    public void SetTotemCounter(int totalNum, int maxNum){
        totemsCollectedCounter.text = "Totems Collected: " + totalNum;
        totemsRemainingCounter.text = maxNum + " Totems Remain";
    }


    public GameObject WinLosePanel;
    public TMP_Text EndStateText;
    public void ActivateWin(){
        WinLosePanel.SetActive(true);
        EndStateText.text = "YOU WIN";
    }
    public void ActivateLose(){
        WinLosePanel.SetActive(true);
        EndStateText.text = "YOU LOSE";
    }
}

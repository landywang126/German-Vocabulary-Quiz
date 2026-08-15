using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NewMonoBehaviourScript : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI PlayerScore;
    [SerializeField] private TextMeshProUGUI MasterScore;
    [SerializeField] private TextMeshProUGUI WurstCountText;
    [SerializeField] private TextMeshProUGUI accuracyNumberText;

    private int playerScore;
    private int masterScore;

    [SerializeField] private GameObject win;
    [SerializeField] private GameObject lose;
    [SerializeField] private GameObject draw;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 1. 讀取儲存的分數
        playerScore = PlayerPrefs.GetInt("PlayerScore", 0);
        masterScore = PlayerPrefs.GetInt("MasterScore", 0);

        // 2. 更新分數顯示
        if (PlayerScore != null) PlayerScore.text = playerScore.ToString();
        if (MasterScore != null) MasterScore.text = masterScore.ToString();

        // 3. 先把 3 個 UI 物件預設全都關閉（防呆）
        if (win != null) win.SetActive(false);
        if (lose != null) lose.SetActive(false);
        if (draw != null) draw.SetActive(false);

        // 4. 根據勝負，開啟對應 UI 並顯示這局變動的香腸數量
        if (playerScore > masterScore)
        {
            if (win != null) win.SetActive(true);
            if (WurstCountText != null) WurstCountText.text = "+8";
        }
        else if (playerScore < masterScore)
        {
            if (lose != null) lose.SetActive(true);
            if (WurstCountText != null) WurstCountText.text = "-4";
        }
        else
        {
            if (draw != null) draw.SetActive(true);
            if (WurstCountText != null) WurstCountText.text = "+0";
        }

        int accuracy = PlayerPrefs.GetInt("Accuracy", 0);

        // 2. 只顯示數字與 % 符號
        if (accuracyNumberText != null)
        {
            accuracyNumberText.text = PlayerPrefs.GetInt("Accuracy", 0).ToString() + "%";
        }
    }


    public void WinGame()
    {
        win.SetActive(true);
    }

    public void loseGame()
    {
        lose.SetActive(true);
    }

    public void drawGame()
    {
        draw.SetActive(true);
    }
}

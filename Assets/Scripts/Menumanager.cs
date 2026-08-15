using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class Menumanager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI WurstCount;
    [SerializeField] private GameObject questionCountPanel; // 彈出面板 UI (Panel)
    [SerializeField] private GameObject LevelPanel;
    [SerializeField] private TextMeshProUGUI currentCountText;
    [SerializeField] private TextMeshProUGUI currentLevelText;
    [SerializeField] private TextMeshProUGUI lastaccuracy;

    private int wurstCount;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        wurstCount = PlayerPrefs.GetInt("WurstCount",0);
        WurstCount.text = wurstCount.ToString();
        lastaccuracy.text = $"{PlayerPrefs.GetInt("Accuracy", 0)}%";

        string savedLevel = PlayerPrefs.GetString("SelectedVocabulary", "A1words");
        UpdateLevelUI(savedLevel);

        int savedMaxQuestions = PlayerPrefs.GetInt("MaxQuestions", 10);
        UpdateCountUI(savedMaxQuestions);

        if (questionCountPanel != null)
            questionCountPanel.SetActive(false);
        if (LevelPanel != null)
            LevelPanel.SetActive(false);
    }

    public void OpenQuestionPanel()
    {
        if (questionCountPanel != null)
            questionCountPanel.SetActive(true);
    }

    // 💡 按下「關閉/背景」按鈕時呼叫：關閉彈出視窗
    public void CloseQuestionPanel()
    {
        if (questionCountPanel != null)
            questionCountPanel.SetActive(false);
    }

    public void OpenLevelPanel()
    {
        if (LevelPanel != null)
            LevelPanel.SetActive(true);
    }

    public void CloseLevelPanel()
    {
        if (LevelPanel != null)
            LevelPanel.SetActive(false);
    }

    public void SelectQuestionCount(int selectedCount)
    {
        // 1. 儲存題數
        PlayerPrefs.SetInt("MaxQuestions", selectedCount);
        PlayerPrefs.Save();

        // 2. 更新主畫面顯示
        UpdateCountUI(selectedCount);

        // 3. 💡 點選後直接把面板關閉！
        CloseQuestionPanel();

        Debug.Log("已儲存題數：" + selectedCount);
    }

    public void SelectVocabulary(string vocabFileName)
    {
        // 傳入檔名，例如 "A1words" 或 "A2words"
        PlayerPrefs.SetString("SelectedVocabulary", vocabFileName);
        PlayerPrefs.Save();
        UpdateLevelUI(vocabFileName);
        CloseLevelPanel();
        Debug.Log($"已成功切換並更新 UI 為：{vocabFileName}");
    }

    private void UpdateCountUI(int count)
    {
        if (currentCountText != null)
        {
            currentCountText.text = count.ToString(); // 或改為 $"{count} 題"
        }
    }


    private void UpdateLevelUI(string vocabFileName)
    {
        if (currentLevelText != null)
        {
            // 可以根據你的需求格式化顯示，例如 "A1" 或 "A1words"
            currentLevelText.text = vocabFileName;
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [System.Serializable]
    public class WordItem
    {
        public string german;  // 德文題目
        public string english; // 英文正確答案
    }

    // ✅ CSV 讀進來的資料會存放在這裡
    private List<WordItem> wordBank = new List<WordItem>();
    private string correctAnswerText; // 紀錄當前題目的正確答案字串

    private int indexQuestions;
    public TextMeshProUGUI showQuestions; // 顯示題目的文字框
    public Button[] AnswerButtons;        // 四個選項按鈕的陣列 (Size 請設為 4)

    public string answers;                // 儲存當前正確答案

    public Image timerAmount;               // 畫面的計時條圖片
    [SerializeField] private float timer = 10f;// 預設倒數 10 秒
    [SerializeField] private Color green;
    [SerializeField] private Color red;
    [SerializeField] private TextMeshProUGUI PlayerScore;
    [SerializeField] private TextMeshProUGUI MasterScore;
    [SerializeField] private TextMeshProUGUI QuestionCount;
    [SerializeField] private TextMeshProUGUI MaxQuestions;
    private int playerScore;
    private int masterScore; 
    private int questionCount;
    private int wurstCount;
    private int maxQuestions;
    private int totalQuestions;          // 總題數

    public void Start()
    {
        maxQuestions = PlayerPrefs.GetInt("MaxQuestions", 10);
        wurstCount = PlayerPrefs.GetInt("WurstCount");
        totalQuestions = PlayerPrefs.GetInt("MaxQuestions", 10);
        LoadCSVData();
        CheckQuestions();
    }



    private void Update()
    {
        PlayerScore.text = playerScore.ToString();
        MasterScore.text = masterScore.ToString();
        QuestionCount.text = questionCount.ToString();
        MaxQuestions.text= maxQuestions.ToString();

        timer -= Time.deltaTime;
        timerAmount.fillAmount = timer / 10f;
        if (timer <= 0)
        {
            OnTimeOut();
        }

        if (questionCount >= maxQuestions)
        {
            Wursts();
            SceneManager.LoadScene("EndGame");
        }
        
    }

    void LoadCSVData()
    {
        // 💡 1. 從 PlayerPrefs 讀取玩家選的檔名，預設為 "A1words"
        // (如果在選單選了 A2，這裡就會拿到 "A2words")
        string selectedVocab = PlayerPrefs.GetString("SelectedVocabulary", "A1words");

        // 💡 2. 動態載入 Resources 資料夾底下的 CSV 檔
        TextAsset csvFile = Resources.Load<TextAsset>(selectedVocab);

        if (csvFile == null)
        {
            Debug.LogError($"找不到 {selectedVocab}.csv！請確認該檔案有放在 Assets/Resources/ 資料夾內！");
            return;
        }

        string[] lines = csvFile.text.Split('\n');
        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            // 使用 Regex 拆分，避免德文內部的逗號導致拆錯
            string[] values = Regex.Split(line, ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");

            if (values.Length >= 2)
            {
                WordItem item = new WordItem();
                item.german = values[0].Trim().Trim('"');
                item.english = values[1].Trim().Trim('"');
                wordBank.Add(item);
            }
        }
    }

    private void CheckQuestions()
    {
        if (wordBank.Count < 3)
        {
            Debug.LogError("題庫單字量不足 3 個！");
            return;
        }
        // A. 隨機選一個單字作為題目
        int randomIndex = Random.Range(0, wordBank.Count);
        WordItem currentWord = wordBank[randomIndex];

        // 顯示德文題目
        showQuestions.text = currentWord.german;

        // B. 記錄這題的正確英文答案
        correctAnswerText = currentWord.english;

        // C. 準備 3 個英文選項 (1 個正確 + 2 個隨機干擾項)
        List<string> options = new List<string>();
        options.Add(currentWord.english);

        while (options.Count < 3)
        {
            int randIndex = Random.Range(0, wordBank.Count);
            string randomEng = wordBank[randIndex].english;

            if (!options.Contains(randomEng))
            {
                options.Add(randomEng);
            }
        }

        // D. 打亂 3 個選項的順序
        ShuffleList(options);

        // E. 填入 UI 按鈕上的 TextMeshProUGUI
        for (int i = 0; i < AnswerButtons.Length; i++)
        {
            AnswerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = options[i];
        }
    }

    public void ClickButtons(int check)
    {
        answers = AnswerButtons[check].GetComponentInChildren<TextMeshProUGUI>().text;
        CheckAnswers(check);
    }

    private void CheckAnswers(int check)
    {
        timer = 999f;
        if (answers== correctAnswerText)
        {
            
            AnswerButtons[check].GetComponent<Image>().color = green;
            playerScore++;
            questionCount++;
            PlayerPrefs.SetInt("PlayerScore", playerScore);
            PlayerPrefs.SetInt("MasterScore", masterScore);
            PlayerPrefs.Save();
        }

        else
        {
            AnswerButtons[check].GetComponent<Image>().color = red;
            masterScore ++;
            questionCount++;
            PlayerPrefs.SetInt("PlayerScore", playerScore);
            PlayerPrefs.SetInt("MasterScore", masterScore);
            PlayerPrefs.Save();
        }

        float accuracy = 0f;
        if (questionCount > 0)
        {
            accuracy = ((float)playerScore / questionCount) * 100f;
        }
        int accuracyInt = Mathf.RoundToInt(accuracy); // 轉成整數，例如 80
        PlayerPrefs.SetInt("Accuracy", accuracyInt);

        StartCoroutine(WaitForNextQuestion(check));
             
    }

    private void ShuffleList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            T temp = list[i];
            int rand = Random.Range(i, list.Count);
            list[i] = list[rand];
            list[rand] = temp;
        }
    }

    // 超時未答的處理邏輯
    private void OnTimeOut()
    {
        // 重置計時器，避免 Update 在 0.5 秒協程期間重複觸發 OnTimeOut
        timer = 10f;

        // 對手得分，總題數 +1
        masterScore++;
        questionCount++;

        // 儲存資料
        PlayerPrefs.SetInt("PlayerScore", playerScore);
        PlayerPrefs.SetInt("MasterScore", masterScore);
        PlayerPrefs.Save();

        // 0.5 秒後自動進入下一題
        StartCoroutine(WaitForNextQuestionTimeOut());
    }

    // 超時專用的切換協程 (不需要傳入 check 參數)
    IEnumerator WaitForNextQuestionTimeOut()
    {
        yield return new WaitForSeconds(0.5f);
        CheckQuestions();
        timer = 10f; // 重置下一題的時間
    }

    IEnumerator  WaitForNextQuestion(int check)
    {
        yield return new WaitForSeconds(0.5f);
        CheckQuestions();
        AnswerButtons[check].GetComponent<Image>().color = Color.white;
        timer = 10f;
    }
            
    void Wursts()
    {
        if (playerScore>masterScore)
        {
            wurstCount += 8;
        }
        else if (playerScore<masterScore)
        {
            wurstCount -= 4;
        }
        PlayerPrefs.SetInt("WurstCount", wurstCount);
        PlayerPrefs.Save();

    }
    

}


    
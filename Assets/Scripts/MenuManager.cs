using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance; 
    [SerializeField]
    private TMP_InputField playerNameInput;
    [SerializeField]
    private TMP_Text bestScoreMainMenu;

    public string playerName;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        LoadHighScore();
        DontDestroyOnLoad(gameObject);
    }

    public void StartGame()
    {
        if (!playerNameInput.text.Equals(""))
        {
            playerName = playerNameInput.text;
            SceneManager.LoadScene(1);
        }
    }

    private void LoadHighScore()
    {
        string path = Application.persistentDataPath + "/scorefile.json";
        if(File.Exists(path))
        {
            string json = File.ReadAllText(path);
            MainManager.SaveData highScore = JsonUtility.FromJson<MainManager.SaveData>(json);
            Debug.Log(highScore.playerName);

            bestScoreMainMenu.text = "Best Score: " + highScore.playerName + " : " + highScore.highScore;
        }
    }
}

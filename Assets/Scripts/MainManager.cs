using System;
using Random = UnityEngine.Random;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class MainManager : MonoBehaviour
{
    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody Ball;

    public Text ScoreText;
    public Text bestScore;
    public GameObject GameOverText;

    private bool m_Started = false;
    private int m_Points;

    private bool m_GameOver = false;

    

    // Start is called before the first frame update
    void Start()
    {
        const float step = 0.6f;
        int perLine = Mathf.FloorToInt(4.0f / step);

        int[] pointCountArray = new[] { 1, 1, 2, 2, 5, 5 };
        for (int i = 0; i < LineCount; ++i)
        {
            for (int x = 0; x < perLine; ++x)
            {
                Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                brick.PointValue = pointCountArray[i];
                brick.onDestroyed.AddListener(AddPoint);
            }
        }
        SaveData highScoreData = LoadPlayerScore();
        if(highScoreData != null)
        {
            bestScore.text = "Best Score : " + highScoreData.playerName + " : " + highScoreData.highScore;
        }

    }

    private void Update()
    {
        if (!m_Started)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_Started = true;
                float randomDirection = Random.Range(-1.0f, 1.0f);
                Vector3 forceDir = new Vector3(randomDirection, 1, 0);
                forceDir.Normalize();

                Ball.transform.SetParent(null);
                Ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
            }
        }
        else if (m_GameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }

    void AddPoint(int point)
    {
        m_Points += point;
        ScoreText.text = $"Score : {m_Points}";
    }

    public void GameOver()
    {
        m_GameOver = true;

        GameOverText.SetActive(true);

        if (isNewHighScore())
        {
            SavePlayerScore();
        }
    }

    [Serializable]
    public class SaveData
    {
        public int highScore;
        public string playerName;
    }

    public bool isNewHighScore()
    {
        SaveData currentHighScore = LoadPlayerScore();

        if(currentHighScore != null)
        {
            if(m_Points > currentHighScore.highScore)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    public void SavePlayerScore()
    {
        SaveData data = new SaveData();
        data.highScore = m_Points;
        data.playerName = MenuManager.instance.playerName; 

        string json = JsonUtility.ToJson(data);

        File.WriteAllText(Application.persistentDataPath + "/scorefile.json", json);
    }

    public SaveData LoadPlayerScore()
    {
        string path = Application.persistentDataPath + "/scorefile.json";
        if(File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            return data;

        }
        else
        {
            return null;
        }
    }

    
}

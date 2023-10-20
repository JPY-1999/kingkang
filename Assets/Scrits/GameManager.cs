using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private int level;
    private int lives;
    private int score;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        NewGame();
    }

    private void NewGame()
    {
        lives = 3;
        score = 0;

        LoadLevel(1); //加载第一关
    }

    private void LoadLevel(int index)
    {
        level = index;

        Camera camera = Camera.main;
        if(camera != null)
        {
            camera.cullingMask = 0;  //停止渲染
        }

        Invoke(nameof(LoadScene), 1f); //重新加载场景的时候停止渲染1秒
    }

    private void LoadScene()
    {
        SceneManager.LoadScene(level);
    }

    public void LevelComplete()
    {
        score += 1000;
        int nextLevel = level + 1;
        if (nextLevel < SceneManager.sceneCountInBuildSettings)
        {
            LoadLevel(nextLevel); //依次载入关卡，最后一关后载入第一关
        }
        else
        {
            LoadLevel(1);
        }
    }

    public void LevelFailed()
    {
        lives--;

        if(lives <= 0)
        {
            NewGame();
        }
        else
        {
            LoadLevel(level); //失败重新加载当前关卡
        }
    }
}

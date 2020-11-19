using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelExit : MonoBehaviour
{
    [SerializeField] float timeToWait = 2f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        StartCoroutine(LoadNextLevelWithDelay(timeToWait));
    }

    private IEnumerator LoadNextLevelWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        LoadNextScene();
    }

    public void LoadNextScene()
    {
        DestroyScenePersistObject();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    
    public void LoadMainMenu()
    {
        DestroyScenePersistObject();
        DestroyGameSession();
        SceneManager.LoadScene(0);
    }

    private void DestroyScenePersistObject()
    {
        ScenePersist scenePersist = FindObjectOfType<ScenePersist>();
        if (scenePersist)
            Destroy(scenePersist.gameObject);
    }

    private void DestroyGameSession()
    {
        GameSession gameSession = FindObjectOfType<GameSession>();
        if (gameSession)
            Destroy(gameSession.gameObject);
    }
}

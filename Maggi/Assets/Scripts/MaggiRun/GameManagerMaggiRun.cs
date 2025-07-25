using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerMaggiRun : MonoBehaviour
{
    [SerializeField] private SceneRestartEventChannelSO restartChannel;
    [SerializeField] private float restartDelay = 2f;

    private void OnEnable()
    {
        restartChannel.OnEventRaised += HandleRestart;
    }

    private void OnDisable()
    {
        restartChannel.OnEventRaised -= HandleRestart;
    }

    private void HandleRestart()
    {
        Time.timeScale = 1f;
        Invoke(nameof(ReloadScene), restartDelay);
    }

    private void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
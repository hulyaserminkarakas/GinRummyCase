using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    private void Awake()
    {
        Application.targetFrameRate = 60;
        Input.multiTouchEnabled = false;
    }

    public void OnPlayButtonPressed()
    {
        SceneManager.LoadScene("GameScene");
    }
}

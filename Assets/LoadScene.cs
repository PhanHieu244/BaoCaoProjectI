using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    private void Start()
    {
        LoadDataAndScene();
    }

    private void LoadDataAndScene()
    {
         SceneManager.LoadScene("Home");
    }
}


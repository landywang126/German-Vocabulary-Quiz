using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScenes: MonoBehaviour
{
    public void Change_Scenes (string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}

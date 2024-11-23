using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenuWindow : MonoBehaviour {
    public void ExitGameWindow()  {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
}

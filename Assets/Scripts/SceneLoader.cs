using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public int scene;
    public void changeScene(){
        SceneManager.LoadScene(scene);}

    void reloadScene(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

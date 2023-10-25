using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    private Animator _Anim;

    private void Awake()
    {
        if (!_Anim)
            _Anim = GetComponent<Animator>();
    }
    public void StartNewGame()
    {
        GameController.Instance.SaveGame.SaveGameID = "";
        SceneManager.LoadScene("Intro");
    }

    public void OnSelectStart()
    {
        _Anim.SetTrigger("StartGame");
    }
}

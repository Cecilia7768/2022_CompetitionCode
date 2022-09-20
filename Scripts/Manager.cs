using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
  private static T instance = null;
  public static T Instance
  {
    get
    {
      if (instance == null)
      {
        instance = FindObjectOfType(typeof(T)) as T;

        if (instance == null)
        {
          instance = new GameObject(typeof(T).ToString(), typeof(T)).AddComponent<T>();
        }
      }

      return instance;
    }
  }
}

namespace Cat
{
  public class Manager : MonoSingleton<Manager>
  {
    public bool gameStart = false;
    public bool gameOver = false;
    public bool gameClear = false;

    public UnityEvent gameClearEvent;
    public UnityEvent gameOverEvent;


    public GameObject player;
    private void Start()
    {
      gameOver = false;
      gameClear = false;

      this.UpdateAsObservable()
        .Where(x => gameClear)
        .Subscribe(_ =>
        {
          Invoke("GameClear", 4.5f);
        });

      this.UpdateAsObservable()
        .Where(x => gameOver)
        .Subscribe(_ =>
        {
          GameOver();
        });
    }

    void GameClear()
    {
      gameClearEvent.Invoke();
    }
    void GameOver()
    {
      gameOverEvent.Invoke();
    }

  }
}

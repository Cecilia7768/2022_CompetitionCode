using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;
using UniRx.Triggers;
using System;
using UnityEngine.SceneManagement;

namespace Cat
{
  public class UIManager : MonoBehaviour
  {
    #region LinkData

    [Header("시작시 UI")]
    public GameObject startUIImg;
    public GameObject footImg;
    public GameObject[] footImgs;

    public Image startImg;
    bool isStartClick = false;

    [Header("로딩 UI")]
    public GameObject loadingUIImg;
    public TMP_Text loadingTxt;
    public Image loadingImg;
    public Sprite[] loadSprites;
    public AudioClip[] loadingAudioClip;

    [Header("설명 UI")]
    public GameObject infoUIImg;
    private AudioSource inGameAudio;

    [Header("설명 후 인게임")]
    public GameObject startTextImg;
    public GameObject ingameUIImg;

    [Header ("HP 슬라이더")]
    public GameObject hpSliderImg;

    [Header("게임오버 UI")]
    public GameObject gameOverUI;
    [Header("게임성공 UI")]
    public GameObject gameClearUI;

    #endregion

    private bool gameDone = false;
    private bool doubleTouch = false;

    WaitForSeconds wait02 = new WaitForSeconds(.2f);
    WaitForSeconds wait05 = new WaitForSeconds(.5f);
    WaitForSeconds wait4 = new WaitForSeconds(4f);


    private void Start()
    {
      inGameAudio = this.GetComponent<AudioSource>();

      StartCoroutine(StartButtonEffect());
      StartCoroutine(FootImgOnOff());

      //게임 끝났을때
      var clickStream = this.UpdateAsObservable().Where(_ => Input.GetMouseButtonDown(0));
      clickStream
          .Buffer(clickStream.Throttle(TimeSpan.FromMilliseconds(200)))
          .Where(x => x.Count >= 2)
          .Subscribe(_ => { doubleTouch = true; });

      this.UpdateAsObservable()
        .Where(x => gameDone)
        .Subscribe(_ =>
        {
          inGameAudio.Stop();
          if (doubleTouch)
          {   
            SceneManager.LoadScene(0);
            doubleTouch = false;
          }
        });
      this.UpdateAsObservable()
        .Where(x => startUIImg.activeSelf.Equals(true))
        .Subscribe(_ =>
        {
          if (doubleTouch)
          {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); // 어플리케이션 종료
#endif
            doubleTouch = false;
          }
          else if (Input.GetMouseButtonDown(0))
            StartCoroutine(OnclickStart());
        });
  }
    IEnumerator StartButtonEffect()
    {
      bool charging = false;
      while (!isStartClick)
      {
        var color = startImg.color;
        if (!charging)
        {
          color.a -= .1f;
          startImg.color = color;
          if (color.a < .3f)
            charging = !charging;
          yield return wait02;
        }
        else
        {
          color.a += .1f;
          startImg.color = color;
          if (color.a >= 1f)
            charging = !charging;
          yield return wait02;
        }
      }
    }

    IEnumerator FootImgOnOff()
    {
      for(int i = 0; i < footImgs.Length; i++)
      {
        footImgs[i].SetActive(true);
        yield return wait02;
      }
      while(!isStartClick)
      {
        footImg.SetActive(true);
        float ranNum = UnityEngine.Random.Range(0f, 3f);
        yield return new WaitForSeconds(ranNum);
        footImg.SetActive(false);
        ranNum = UnityEngine.Random.Range(0f, 3f);
        yield return new WaitForSeconds(ranNum);
      }
    }


    IEnumerator LoadingOnVoice()
    {
      AudioSource audioSource = loadingUIImg.GetComponent<AudioSource>();
      for (int i = 0; i < 2; i++)
      {
        var ranImage = UnityEngine.Random.Range(0, loadSprites.Length);
        loadingImg.sprite = loadSprites[ranImage];
        audioSource.clip = loadingAudioClip[ranImage];
        audioSource.Play();
        yield return wait4;
      }
    }
    IEnumerator OnclickStart()
    {
      yield return wait05;
      if (!doubleTouch)
      {
        isStartClick = !isStartClick;
        startUIImg.SetActive(false);
        loadingUIImg.SetActive(true);
        StartCoroutine(LoadingText());
        StartCoroutine(LoadingOnVoice());
      }
    }
    public void OnclickInGame()
    {
      infoUIImg.SetActive(false);
      inGameAudio.volume = .1f;
      startTextImg.SetActive(true);
      StartCoroutine(SizeUpStartTextImg());
    }

    IEnumerator SizeUpStartTextImg()
    {
      for (float i = .1f; i <= 1f; i += .1f)
      {
        var x = startTextImg.transform.localScale.x;
        var y = startTextImg.transform.localScale.y;
        startTextImg.transform.localScale = new Vector2(x + .1f, y + .1f);
        yield return new WaitForSeconds(.025f);
      }
      yield return wait05;
      startTextImg.SetActive(false);
      yield return wait05;
      ingameUIImg.SetActive(true);
      Manager.Instance.gameStart = true;
      StartCoroutine(MoveHPSlider());
    }

    IEnumerator MoveHPSlider()
    {
      float minNum = -900f;
      for (;;)
      {
        var x = hpSliderImg.transform.localPosition.x;
        hpSliderImg.transform.localPosition = new Vector2(x - 2f, -10f);
        if(x < minNum)
          break;
        yield return wait02;
      }
      if(!gameDone)
        Manager.Instance.gameOver = true;
    }

    public void GameOverUI()
    {
      ingameUIImg.SetActive(false);
      gameOverUI.SetActive(true);
      gameDone = true;
    }

    public void GameClearUI()
    {
      ingameUIImg.SetActive(false);
      gameClearUI.SetActive(true);
      gameDone = true;
    }
  }  
}
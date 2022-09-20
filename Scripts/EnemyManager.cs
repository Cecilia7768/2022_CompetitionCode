using System.Collections.Generic;
using UnityEngine;


namespace Cat
{
  public class EnemyManager : MonoBehaviour
  {
    #region Data
    public enum CurrDirection
    {
      none = -1,
      Left,
      Right,
    }
    [Header("��ֹ� ������Ʈ")]
    public GameObject[] enemies;

    [Header("��ֹ� ��ġ ����")]
    public Transform[] obstacleCreateTrans; //0~2 Right , 3~5 Left

    //��ֹ� ������ ����ü
    public struct ObsPosition
    {
      public GameObject gameObj;
      public CurrDirection currDirection;
      public Transform obsPositon;

      public ObsPosition(GameObject gameObj, CurrDirection currDirection, Transform obsPositon)
      {
        this.gameObj = gameObj;
        this.currDirection = currDirection;
        this.obsPositon = obsPositon;
      }
    }
    public List<ObsPosition> obstaclesList = new List<ObsPosition>();
    public List<GameObject> currObsList = new List<GameObject>(); //���� ������ ��ֹ�

    #endregion

    private void Awake()
    {
      for (int i = 0; i < obstacleCreateTrans.Length; i++)
      {
        if (i < 3)
        {
          var objPosi = new ObsPosition(null, CurrDirection.Right, obstacleCreateTrans[i]);
          obstaclesList.Add(objPosi);
        }
        else
        {
          var objPosi = new ObsPosition(null, CurrDirection.Left, obstacleCreateTrans[i]);
          obstaclesList.Add(objPosi);
        }
      }
      SetObstacle();
    }

    private void SetObstacle()
    {
      int makeNum = 6;
      //Random.Range(2, 6);
      for (int i = 0; i < makeNum; i++)
      {
        int tmpObj = Random.Range(0, enemies.Length);
        var obj = Instantiate(enemies[tmpObj]);
        obj.transform.parent = this.transform;


        //currObsList ����Ʈ�� �ߺ��� ���� �ִٸ� �ٽ� Ž��
        GetRandomObsPosi(obj);
      }
    }

    private void GetRandomObsPosi(GameObject obj)
    {
      int tmpPosiNum = 0;
      for (int i = 0; i < obstaclesList.Count; i++)
      {
        bool findSamePosi = false;
        tmpPosiNum = Random.Range(0, obstaclesList.Count);
        if (!currObsList.Count.Equals(0))
        {
          foreach (var data in currObsList)
          {
            if (data.transform.position.Equals(obstaclesList[tmpPosiNum].obsPositon))
            {
              findSamePosi = true;
              break;
            }
          }
          if (!findSamePosi) //�������� ã�� ���ߴٸ� �ش� �������� ����
          {
            obj.transform.position = obstaclesList[tmpPosiNum].obsPositon.position;
            break;
          }
        }
        else
        {
          obj.transform.position = obstaclesList[tmpPosiNum].obsPositon.position;
          currObsList.Add(obj);
        }
      }

      if (obstaclesList[tmpPosiNum].currDirection.Equals(CurrDirection.Right))
      {
        obj.GetComponent<Enemies>().enemyType = 1;
      }
      else
      {
        obj.GetComponent<Enemies>().enemyType = 0;
      }
      currObsList.Add(obj);
      obj.SetActive(true);
    }

  }
}
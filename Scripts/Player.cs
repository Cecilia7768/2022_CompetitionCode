using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Cat
{
  public class Player : MonoBehaviour
  {
    enum Audio
    {
      Attacked,
      Fail,
      max,
    }

    public float moveSpeed = .2f;
    private Animator anim;
    private CharacterController characterController;
    private AudioSource audioSource;

    private void Awake()
    {
      anim = this.GetComponent<Animator>();
      audioSource = this.GetComponent<AudioSource>();
      characterController = GetComponent<CharacterController>();

      this.UpdateAsObservable()
        .Where(_ => !Input.GetMouseButton(0)
        || Manager.Instance.gameOver || Manager.Instance.gameClear)
        .Subscribe(_ =>
        {
          if (Manager.Instance.gameClear)
          {
            this.transform.position = new Vector3(this.transform.position.x,
              this.transform.position.y, 229f);
            anim.SetBool("IsRun", false);
            anim.SetBool("Clear", true);
          }
          else
            anim.SetBool("IsRun", false);
        });

      this.UpdateAsObservable()
        .Where(x => !Manager.Instance.gameClear)
        .Where(x => Manager.Instance.gameStart && Input.GetMouseButton(0))
        .Subscribe(_ =>
        {       
            characterController.Move(Vector3.forward * moveSpeed);
            anim.SetBool("IsRun", true);         
        });
    }


    private void OnTriggerEnter(Collider other)
    {
      if (other.tag.Equals("Enemy"))
      {
        if(!Manager.Instance.gameOver)
          audioSource.Play();
      }
      else if (other.tag.Equals("Goal"))
      {        
        Manager.Instance.gameClear = true;
        Manager.Instance.gameStart = false;
      }
    }
  }
}
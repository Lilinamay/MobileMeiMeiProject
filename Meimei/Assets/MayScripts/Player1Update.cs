using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class Player1Update : MonoBehaviour
{
    private Touch t;
    public float x;
    public static float boyHP = 100;
    public static float girlHP = 100;
    [SerializeField] Image boy;
    [SerializeField] Image girl;
    [SerializeField] TMP_Text rank;
    float textTimer = 0;
    float hitTimer = 0;
    int c = 0;
    float ct = 0;
    Animator animator;
    [SerializeField] Animator heartanimator;
    [SerializeField] Animator boyanimator;
    [SerializeField] GameObject Girl;
    int hit = 0;
    int hitHealth = 0;
    [SerializeField] chatBox chat;
    public static bool heartMove = false;
    [SerializeField] Image speedLine;
    [SerializeField] Image TxtImage;
    [SerializeField] float min;
    [SerializeField] float max;
    public static bool game = true;
    bool sta = false;
    [SerializeField] Sprite emoteExplode;
    public static float cgTimer = 3; //5the amount of time for cutscene before accepting player input
    bool final = false;
    // Start is called before the first frame update
    void Start()
    {
        rank.text = "";
        animator = GetComponent<Animator>();
        //play power up animation from start or in update if(cgTimer = 3....etc) play animation
        //StartCoroutine(turnS());
    }

    // Update is called once per frame
    void Update()
    {
        HPbar();
        HitTimer();
        Winlose();
        //x = this.transform.position.x - BoyBehavior.currentHeart.transform.position.x;
        if (cgTimer >= 0)
        {
            cgTimer -= Time.deltaTime;
        }
        if(cgTimer<=2&& !sta)
        {
            animator.SetTrigger("turn");
            Audiomanager.Instance.PlaySound(Audiomanager.Instance.becomeStrong, Audiomanager.Instance.strongVolume);
            sta = true;
        }
        if (chatBox.perfectTime > 0.5f)
        {
            heartMove = true;
            chatBox.heart = false;
        }
        if (Input.touchCount > 0 && cgTimer < 0 &&game)
        {
            t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Began)
            {
                Vector3 wp = Camera.main.ScreenToWorldPoint(t.position);
                Vector2 touchPos = new Vector2(wp.x, wp.y);
                Collider2D hit = Physics2D.OverlapPoint(touchPos);

                if (hit && hit == gameObject.GetComponent<Collider2D>())
                {
                    animator.SetTrigger("attack1");
                    foreach (Image i in chat.emoteImage)
                    {
                        if(i.gameObject.transform.position.x >= min && i.gameObject.transform.position.x <= max)
                        {
                            i.gameObject.GetComponent<emoteBehavior>().hit--;
                            if (i.gameObject.GetComponent<emoteBehavior>().hit <= 0)
                            {
                                if (!i.gameObject.GetComponent<emoteBehavior>().isheart)
                                {
                                    //i.sprite = emoteExplode;
                                    //i.enabled = false;
                                    HitSet(2, 3);
                                    StartCoroutine(changeEmote(i));
                                    
                                    
                                }
                                //i.gameObject.GetComponent<Animator>().SetTrigger("small");
                                else
                                {
                                    HitSet(2, 6);
                                }
                            }
                            if(i.gameObject.GetComponent<emoteBehavior>().hit <= 1)
                            {
                                Audiomanager.Instance.PlaySound(Audiomanager.Instance.boyHurt, Audiomanager.Instance.boyHurtVolume);
                            }
                        }
                    }
                    //Debug.Log("perfect timeL " + chatBox.perfectTime);
                    /*if (chatBox.perfectTime>0)
                    {
                        
                            //x = this.transform.position.x - BoyBehavior.currentHeart.transform.position.x;
                            if (chatBox.perfectTime <=0.5f)
                            {
                                chatBox.heart = false;
                                //player attack 1 animation
                                //animator.SetTrigger("attack1");
                                //Debug.Log(x);
                                if (chatBox.perfectTime <= 0.2f)
                                {
                                    HitSet(2, 15);
                                    //PerfectHit();
                                    Debug.Log("perfect!");

                                }
                                else if (chatBox.perfectTime <= 0.5f)
                                {
                                    HitSet(1, 5);
                                    Debug.Log("ok");
                                }
                                
                            }
                            
                            /*else if (BoyBehavior.currentHeart.tag == "heartB")
                            {
                                c++;
                                if (x <= 3.1f && x > 1.6f)
                                {
                                    ct += x;
                                    if (c == 2)
                                    {
                                        //player attack part 2
                                        Debug.Log("big attack: " + ct);
                                        animator.SetTrigger("attack2");
                                        if (ct <= 4.8)
                                        {
                                            HitSet(2, 20);
                                            Debug.Log("big perfect!");
                                        }
                                        else if (ct <= 5.6)
                                        {
                                            HitSet(1, 10);
                                            Debug.Log("big ok!");
                                        }
                                        c = 0;
                                        ct = 0;
                                    }
                                    else
                                    {
                                        //player attack 1
                                        animator.SetTrigger("attack1");
                                    }
                                }
                            
                        }
                    }*/
                }
            }
        }
    }

    void HPbar()
    {
        boy.fillAmount = boyHP / 100;
        girl.fillAmount = girlHP / 100;
        if (textTimer > 0)
        {
            textTimer -= Time.deltaTime;
        }
        else
        {
            rank.text = "";
            //TxtImage.enabled = false;
        }
    }
    void HitTimer()
    {
        if (hitTimer > 0)
        {
            hitTimer -= Time.deltaTime;
        }
        else
        {
            if (hit > 0)
            {
                StartCoroutine(HitAfter());
                if (hit == 2)
                {
                    StartCoroutine(HitPerfAfter());
                    TxtImage.enabled = true;
                    rank.text = "perfect";
                    //Time.timeScale = 0.4f;

                }
                else if (hit == 1)
                {
                    TxtImage.enabled = true;
                    rank.text = "OK";
                }
                boyHP -= hitHealth;
                //DOTween.To(() => boyHP, x => Time.timeScale = x, boyHP-hitHealth, 0.2f).SetEase(Ease.InQuart);
                textTimer = 1.5f;
                //BoyBehavior.currentHeart.SetActive(false);

                hit = 0;
                hitHealth = 0;
                Debug.Log("onHit");

            }
        }
    }

    IEnumerator turnS()
    {
        yield return new WaitForSeconds(1f);//2.5f
        animator.SetTrigger("turn");
        Audiomanager.Instance.PlaySound(Audiomanager.Instance.becomeStrong, Audiomanager.Instance.strongVolume);
    }
    IEnumerator HitAfter()
    {
        yield return new WaitForSeconds(0.03f);
        speedLine.DOFade(0, 0.1F);
        TxtImage.enabled = false;
        //chat.disableEmote();
        if (hit == 2)
        {
            //DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 1, 0.2f).SetEase(Ease.InQuart);  //if perfect hit
        }
        Camera.main.DOOrthoSize(5f, 0.05f).SetEase(Ease.InQuart);
    }
    IEnumerator HitPerfAfter()
    {
        //chat.disableEmote();
        yield return new WaitForSeconds(0.1f);
        //DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 1, 0.1f).SetEase(Ease.InQuart);  //if perfect hit
    }
    void HitSet(int h, int health)
    {
        Debug.Log("origin size" + Camera.main.orthographicSize);
        boyanimator.SetTrigger("hurt");
        Audiomanager.Instance.PlaySound(Audiomanager.Instance.boyHurt, Audiomanager.Instance.boyHurtVolume);
        heartanimator.SetTrigger("small");          //animation?
        TxtImage.enabled = true;
        hit = h;
        hitTimer = 0.4f;
        hitHealth = health;
        Camera.main.DOOrthoSize(4.9f, 0.05f).SetEase(Ease.InQuart);
        speedLine.DOFade(1, 0.2F);
        //Debug.Log("after size" + Camera.main.orthographicSize);
    }

    void Winlose()
    {
        if (boyHP <= 0 && !final)
        {
            Debug.Log("Win");
            game = false;
            Girl.SetActive(false);

            animator.SetTrigger("win");
            final = true;
            Audiomanager.Instance.PlaySound(Audiomanager.Instance.win, Audiomanager.Instance.winVolume);
            //Play win state Animation
        }
        else if (girlHP <= 0 && !final)
        {
            Debug.Log("Lose");
            animator.SetTrigger("lose");
            game = false;
            final = true;
            Audiomanager.Instance.PlaySound(Audiomanager.Instance.lose, Audiomanager.Instance.loseVolume);
            //Play lose  state Animation
        }
    }
    IEnumerator disableEmote(Image i)
    {
        yield return new WaitForSeconds(0.05f);
        i.enabled = false;
    }
    IEnumerator changeEmote(Image i)
    {
        yield return new WaitForSeconds(0.05f);
        i.sprite = emoteExplode;
        StartCoroutine(disableEmote(i));
    }



}


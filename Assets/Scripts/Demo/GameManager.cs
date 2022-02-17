using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using live2d;

public class GameManager : MonoBehaviour
{
    //创建单例  便于在外部调用
    private static GameManager _instance;

    public static GameManager Instance{
        get{
            return _instance;
        }
    }

    //相关判断
    public bool gameOver = false;
    public bool isSad;


    //属性
    public int gold;
    public int favor;
    public int leftDay;

    //赋值
    public Text goldText;
    public Text favorText;
    public Text leftDayText; 
    public GameObject actionBtns;
    public GameObject workBtns;
    public LAppModelProxy lAppModelProxy;
    public Sprite[] workSprites;
    public Image workImg;
    public GameObject workUI;
    public GameObject talkUI;
    public Text talkUIText;
    public GameObject chatUI;
    public SpriteRenderer bgImage;
    public Sprite[] bgSpirte;

    public Live2dSimpleModel badBoy;
    public GameObject tip;
    public GameObject gameOverBtn;
    public Texture2D newClothes;


    //鼠标特效制作
    public GameObject clickEffect;
    public Canvas canvas;


    //场景过渡
    public Image mask;
    public bool toAnotherDay;
    public bool toBeDay;
    private float timeVal;

    private void Awake() {
        _instance = this;//单例的实例化
        gold = favor = 0;
        leftDay = 20;
        UpdateUI();
    }

    // Update is called once per frame
    void Update()
    {
        //鼠标特效
        if(Input.GetMouseButtonDown(0)){
            Vector2 mousePos = Vector2.one;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                Input.mousePosition,
                canvas.worldCamera,
                out mousePos   
            );
            GameObject go = Instantiate(clickEffect);
            go.transform.SetParent(canvas.transform);//将特效的父对象设置为canvas画布  用以显示
            go.transform.localPosition = mousePos;//特效位置
        }


        //过渡到第二天
        if(toAnotherDay){
            if(toBeDay){
                if(timeVal >= 2){
                    timeVal = 0;
                    ToDay();
                }
                else{
                    timeVal += Time.deltaTime;
                }
            }
            else{
                ToDark();
            }
        }

        //游戏结束
        if(gameOver){
            talkUI.SetActive(true);
            gameOverBtn.SetActive(true);
            actionBtns.SetActive(false);
            if(favor >= 100){
                talkUIText.text = "Happy Ending";
            }
            else if(leftDay != 0 && isSad){
                talkUIText.text = "Bad Ending";
            }
            else if(leftDay == 0){
                talkUIText.text = "Time Out";
            }
        }
    }

    private void UpdateUI(){
        goldText.text = gold.ToString();
        favorText.text = favor.ToString();
        leftDayText.text = leftDay.ToString();
    }

    public void changeGold(int goldValue){
        gold += goldValue; 
        if(gold<0){
            gold = 0;
        }
        UpdateUI();
    }

    public void changeFavor(int favorValue){
        favor += favorValue; 
        if(favor<0){
            favor = 0;
        }
        UpdateUI();
    }

    public void ToBeDark(){
        toAnotherDay = true;
    }

    private void ToDark(){
        mask.color += new Color(0,0,0,Mathf.Lerp(0,1,0.05f));//透明度从0到1变化，0.05s变化一次
        if(mask.color.a >= 0.8f){
            mask.color = new Color(0,0,0,1);//当快天黑时 直接将值设为1
            toBeDay = true;
            ResetUI();
            UpdateUI();   
        }
    }

    private void ToDay(){
        mask.color -= new Color(0,0,0,Mathf.Lerp(1,0,0.05f));
        if(mask.color.a <= 0.2f){
            mask.color = new Color(0,0,0,0);
            toBeDay = false;
            toAnotherDay = false;   
        }
    }

    public void ClickWorkBtn(){
        actionBtns.SetActive(false);
        workBtns.SetActive(true);
        lAppModelProxy.SetVisible(false);
    }

    public void GetMoney(int workIndex){
        workBtns.SetActive(false);
        changeGold((4-workIndex)*20);
        workImg.sprite = workSprites[workIndex];
        workUI.SetActive(true);
        talkUI.SetActive(true);
        talkUIText.text = "Work Start , Get "+((4-workIndex)*20).ToString()+" golds ";
    }

    private void ResetUI(){
        workUI.SetActive(false);
        talkUI.SetActive(false);
        actionBtns.SetActive(true);
        lAppModelProxy.SetVisible(true);
        lAppModelProxy.GetModel().SetExpression("f01");
        bgImage.sprite = bgSpirte[0];
        leftDay--;
        if(leftDay == 5){
            CreateBadBoy();   
        }
        else if(leftDay == 10){
            Live2DModelUnity live2DModelUnity = lAppModelProxy.GetModel().GetLive2DModelUnity();//获取到ModelUnity
            live2DModelUnity.setTexture(2,newClothes);
        }
        else if(leftDay == 0){
            gameOver = true;
        }
    }

    public void ClickChatBtn(){
        actionBtns.SetActive(false);
        chatUI.SetActive(true);
        if(favor >= 100){
            lAppModelProxy.GetModel().StartMotion("tap_body" , 1 , 2 );//使用框架播放动作 分组名，序号，优先级
        }
        else{
            lAppModelProxy.GetModel().StartMotion("tap_body" , 0 , 2 );
        }
    }

    public void GetFavor(int chatIndex){
        chatUI.SetActive(false);
        talkUI.SetActive(true);
        switch(chatIndex){
            case 0:
                if(favor > 20){
                    changeFavor(10);
                    talkUIText.text = "Conversation2";
                    lAppModelProxy.GetModel().SetExpression("f06");
                }
                else{
                    changeFavor(2);
                    talkUIText.text = "Conversation1";
                    lAppModelProxy.GetModel().SetExpression("f08");
                }
                break;

            case 1:
                if(favor > 60){
                    changeFavor(20);
                    talkUIText.text = "Conversation4";
                    lAppModelProxy.GetModel().SetExpression("f07");
                }
                else{
                    changeFavor(-10);
                    talkUIText.text = "Conversation3";
                    lAppModelProxy.GetModel().SetExpression("f03");
                }
                break;

            case 2:
                if(favor > 90){
                    changeFavor(40);
                    talkUIText.text = "Conversation6";
                    lAppModelProxy.GetModel().SetExpression("f05");
                }
                else{
                    changeFavor(-20);
                    talkUIText.text = "Conversation5";
                    lAppModelProxy.GetModel().SetExpression("f04");
                }
                break;
            default:
                break;
        }
    }

    public void ClickDateBtn(){
        actionBtns.SetActive(false);
        talkUI.SetActive(true);
        int randomNum = Random.Range(1 , 4);
        bool isEnoughGold = false;
        bgImage.sprite = bgSpirte[randomNum];
        switch(randomNum){
            case 1:
                if(gold >= 50){
                    changeGold(-50);
                    changeFavor(20);
                    talkUIText.text = "Conversation1";
                    isEnoughGold = true;
                }
                else{
                    changeFavor(-20);
                    talkUIText.text = "Conversation2";
                }
                break;
            case 2:
                if(gold >= 80){
                    changeGold(-80);
                    changeFavor(30);
                    talkUIText.text = "Conversation3";
                    isEnoughGold = true;
                }
                else{
                    changeFavor(-20);
                    talkUIText.text = "Conversation4";
                }
                break;
            case 3:
                if(gold >= 100){
                    changeGold(-100);
                    changeFavor(50);
                    talkUIText.text = "Conversation5";
                    isEnoughGold = true;
                }
                else{
                    changeFavor(-10);
                    talkUIText.text = "Conversation6";
                }
                break;
            default:
                break;
        }
        if(isEnoughGold){
            lAppModelProxy.GetModel().StartMotion("pinch_in",0,2);
        }
        else{
            lAppModelProxy.GetModel().StartMotion("flick_head",0,2);
        }
    }

    public void ClickLoveBtn(){
        actionBtns.SetActive(false);
        talkUI.SetActive(true);
        if(favor >= 100){
            talkUIText.text = "Congrauation  Ending";
            bgImage.sprite = bgSpirte[4];
            lAppModelProxy.GetModel().StartMotion("pinch_out",0,2);
            lAppModelProxy.GetModel().SetExpression("f07");
            gameOver = true;
        }
        else{
            talkUIText.text = "Sorry,Game Over";
            lAppModelProxy.GetModel().StartMotion("shake",0,2);
            lAppModelProxy.GetModel().SetExpression("f04");
            gameOver = true;
        }
        
    }

    private void CreateBadBoy(){
        lAppModelProxy.isRunning = true;
        badBoy.gameObject.SetActive(true);
        lAppModelProxy.GetModel().SetExpression("f04");
        actionBtns.SetActive(false);
        tip.SetActive(true);
    }

    public void CloseTip(){
        tip.SetActive(false);
    }

    public void DefeatBadBoy(){
        lAppModelProxy.GetModel().StartMotion("shake",0,2);
        talkUI.SetActive(true);
        talkUIText.text = "Thanks";
        changeFavor(50);
        badBoy.gameObject.SetActive(false);
    }

    public void LoadScence(int SceneNum){
        SceneManager.LoadScene(SceneNum);
    } 
}



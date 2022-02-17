using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using live2d;

public class Live2dSimpleModel : MonoBehaviour
{
    //模型生成定义
    public TextAsset modelFile;
    public Texture2D texture;
    public TextAsset motionFile;
    public GameObject heroine;
    //模型显示
    private Live2DModelUnity live2DModel;
    private Matrix4x4 live2DCanvasPos;
    //动作及表情定义
    private Live2DMotion live2DMotion;
    private MotionQueueManager motionQueueManager;
    //自动眨眼效果
    private EyeBlinkMotion eyeBlinkMotion;

    public bool isDefeat;
    public float speed;
    private Vector3 initPos;
    private int hitCount;

    // Start is called before the first frame update
    void Start()
    {
        Live2D.init();//初始化
        live2DModel = Live2DModelUnity.loadModel(modelFile.bytes);//使用二进制加载模型文件
        live2DModel.setTexture(0,texture);//设置贴图
        float modelWidth = live2DModel.getCanvasWidth();
        live2DCanvasPos = Matrix4x4.Ortho(0,modelWidth,modelWidth,0,-50f,50f);//将位置参数设为正交矩阵

        live2DMotion = Live2DMotion.loadMotion(motionFile.bytes);

        live2DMotion.setLoop(true);
        motionQueueManager = new MotionQueueManager();
        
        eyeBlinkMotion = new EyeBlinkMotion();

        motionQueueManager.startMotion(live2DMotion);

        initPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        live2DModel.setMatrix(transform.localToWorldMatrix*live2DCanvasPos);//显示模型
        motionQueueManager.updateParam(live2DModel);
        eyeBlinkMotion.setParam(live2DModel);

        live2DModel.update();
        
        //游戏结束的方法
        if(GameManager.Instance.gameOver){
            return;
        }
        if((heroine.transform.position.x - transform.position.x)<3){
            GameManager.Instance.gameOver = true;
            GameManager.Instance.isSad = true;
        }

        //移动判定
        if(isDefeat){
            transform.position = Vector3.Lerp(transform.position,initPos,0.2f);//逐渐从当前位置移动到初始位置
        }
        else{
            transform.Translate(Vector3.right*speed*Time.deltaTime);
        }
    }

    private void OnRenderObject() {
        live2DModel.draw();//绘制模型    
    }

    private void OnMouseDown() {
        if(GameManager.Instance.gameOver){
            return;
        }

        if(hitCount >= 10){
            isDefeat = true;
            GameManager.Instance.DefeatBadBoy();
        }
        else{
            hitCount++;
        }
    }
}









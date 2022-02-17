using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using live2d;
using live2d.framework;

public class live2dMoudel : MonoBehaviour
{
    private Live2DModelUnity live2DMoudel;
    public TextAsset modelFile;//从unity面板中赋值
    public Texture2D[] texture2D;//贴图的赋值


    private Matrix4x4 live2dCanvasPos;//定义一个4x4矩阵 模型的显示位置


    public TextAsset[] motionFile;//动作的赋值
    private Live2DMotion[] motions;


    private MotionQueueManager motionManager;//动作管理器
    private MotionQueueManager motionManager2;//多个动作时需要多个动作管理器

    private int motionIndex = 0;//动作循环播放

    private L2DMotionManager manager;//动作优先级的设置

    private EyeBlinkMotion eyeBlinkMotion;//自动眨眼

    private L2DTargetPoint drag;//实现鼠标拖拽的动作变化


    private PhysicsHair physicsHairSideLeft;//定义两侧的左边头发物理系统
    private PhysicsHair physicsHairSideRight;//定义两侧右边头发物理系统
    private PhysicsHair physicsHairBackLeft;//定义后面左侧的头发物理系统
    private PhysicsHair physicsHairBackRight;//定义后面右侧的头发物理系统
    //private PhysicsHair physicsHairFront;//定义前面的头发物理系统


    //表情管理设置
    public TextAsset[] expressionFiles;
    public L2DExpressionMotion[] expressions;
    private MotionQueueManager expressionManager;



    public float weight;

    // Start is called before the first frame update
    void Start()
    {
        Live2D.init();//初始化

        //贴图的加载
        live2DMoudel = Live2DModelUnity.loadModel(modelFile.bytes);//获取贴图
        for(int i=0;i<texture2D.Length;i++){
            live2DMoudel.setTexture(i,texture2D[i]);
        }

        //指定显示位置和尺寸(参数的值固定)
        float modelWidth = live2DMoudel.getCanvasWidth();//获取模型宽度
        live2dCanvasPos = Matrix4x4.Ortho(0,modelWidth,modelWidth,0,-50,50);

        //播放动作   1.动作文件的加载
        motions = new Live2DMotion[motionFile.Length];
        for(int i=0;i<motions.Length;i++){
            motions[i] = Live2DMotion.loadMotion(motionFile[i].bytes);
        }

        //播放动作   2.几个重要API
        //重复播放动作淡入，即动作较丝滑
        //motions[motionIndex].setLoopFadeIn(false);
        //动作淡出淡入时间  1000毫秒 默认值也为1000
        //motions[motionIndex].setFadeOut(1000);
        //motions[motionIndex].setFadeIn(1000);
        //动画是否循环播放
        // motions[motionIndex].setLoop(true);

        // //动作播放
        // motionManager = new MotionQueueManager();
        // motionManager.startMotion(motions[motionIndex]);//两个参数  是否自动删除 默认为false

        // motions[5].setLoop(true);
        // motionManager2 = new MotionQueueManager();
        // motionManager2.startMotion(motions[5]);
        
        manager = new L2DMotionManager();

        //自动眨眼
        eyeBlinkMotion = new EyeBlinkMotion();

        drag = new L2DTargetPoint();

        //头发的摆动
        physicsHairSideLeft = new PhysicsHair();
        physicsHairSideRight = new PhysicsHair();
        physicsHairBackLeft = new PhysicsHair();
        physicsHairBackRight = new PhysicsHair();

        //左侧头发
        //套用物理运算
        physicsHairSideLeft.setup(0.2f,0.5f,0.14f);
        //设置输入参数  部分变动是进行的运算
        PhysicsHair.Src srcXLeft = PhysicsHair.Src.SRC_TO_X;//横向摇摆
        //3. 影响度  4. 权重
        physicsHairSideLeft.addSrcParam(srcXLeft,"PARAM_ANGLE_X",0.005f,1);
        //设置输出表现的表现形式   
        PhysicsHair.Target targetLeft = PhysicsHair.Target.TARGET_FROM_ANGLE;
        physicsHairSideLeft.addTargetParam(targetLeft,"PARAM_HAIR_SIDE_L",0.005f,1);

        
        //右侧头发
        physicsHairSideRight.setup(0.2f,0.5f,0.14f);
        //设置输入参数  部分变动是进行的运算
        PhysicsHair.Src srcXRight = PhysicsHair.Src.SRC_TO_X;//横向摇摆
        //3. 影响度  4. 权重
        physicsHairSideRight.addSrcParam(srcXRight,"PARAM_ANGLE_X",0.0005f,1);
        //设置输出表现的表现形式   
        PhysicsHair.Target targetRight = PhysicsHair.Target.TARGET_FROM_ANGLE;
        physicsHairSideRight.addTargetParam(targetRight,"PARAM_HAIR_SIDE_R",0.005f,1);


        //后侧头发
        //左边 
        physicsHairBackLeft.setup(0.24f,0.5f,0.18f);

        PhysicsHair.Src srcBackXLeft = PhysicsHair.Src.SRC_TO_X;//横向摇摆
        PhysicsHair.Src srcZLeft = PhysicsHair.Src.SRC_TO_G_ANGLE;

        physicsHairBackLeft.addSrcParam(srcBackXLeft,"PARAM_ANGLE_X",0.005f,1); 
        physicsHairBackLeft.addSrcParam(srcZLeft,"PARAM_ANGLE_Z",0.8f,1);

        PhysicsHair.Target targetBackLeft = PhysicsHair.Target.TARGET_FROM_ANGLE;
        physicsHairBackLeft.addTargetParam(targetBackLeft,"PARAM_HAIR_BACK_L",0.005f,1);

        //右边
        physicsHairBackRight.setup(0.24f,0.5f,0.18f);

        PhysicsHair.Src srcBackXRight = PhysicsHair.Src.SRC_TO_X;//横向摇摆
        PhysicsHair.Src srcZRight = PhysicsHair.Src.SRC_TO_G_ANGLE;

        physicsHairBackRight.addSrcParam(srcBackXRight,"PARAM_ANGLE_X",0.005f,1); 
        physicsHairBackRight.addSrcParam(srcZRight,"PARAM_ANGLE_Z",0.8f,1);

        PhysicsHair.Target targetBackRight = PhysicsHair.Target.TARGET_FROM_ANGLE;
        physicsHairBackRight.addTargetParam(targetBackRight,"PARAM_HAIR_BACK_R",0.005f,1);


        //表情文件的加载
        expressionManager = new MotionQueueManager();
        expressions = new L2DExpressionMotion[expressionFiles.Length];
        for(int i = 0;i<expressions.Length;i++){
            expressions[i] = L2DExpressionMotion.loadJson(expressionFiles[i].bytes); 
        }

    }

    // Update is called once per frame
    void Update()
    {
        live2DMoudel.setMatrix(transform.localToWorldMatrix*live2dCanvasPos);//能够实时更新位置

        //motionManager.updateParam(live2DMoudel);//确定要实现动作的模型
        //motionManager2.updateParam(live2DMoudel);

        //按下空格后播放下一个动作
        // if(Input.GetKeyDown(KeyCode.Space)){
        //     motionIndex++;
        //     if(motionIndex >= motions.Length){
        //         motionIndex = 0;
        //     }
        //     motionManager.startMotion(motions[motionIndex]);
        // }

        //判断待机动作
        // if(manager.isFinished()){
        //     StartMotion(0,1);
        // }else if(Input.GetKeyDown(KeyCode.Space)){
        //     StartMotion(14,2);
        // }
        // manager.updateParam(live2DMoudel);

        // //设置模型的具体参数
        // live2DMoudel.setParamFloat("PARAM_ANGLE_X",1);
        // if(Input.GetKeyDown(KeyCode.Space)){
        //     live2DMoudel.addToParamFloat("PARAM_ANGLE_X",weight);
        // }
        // live2DMoudel.multParamFloat("PARAM_ANGLE_X",weight);

        // //通过索引的方式获取参数   较方便
        // int paramAngel = live2DMoudel.getParamIndex("PARAM_ANGLE_X");
        // live2DMoudel.setParamFloat(paramAngel,30);

        //参数的保存和回复  包括模型的所有参数
        // live2DMoudel.setParamFloat("PARAM_ANGLE_X",30);
        // live2DMoudel.saveParam();
        // live2DMoudel.loadParam();

        //设置模型的不透明度  0为完全透明 ， 1为不透明
        //live2DMoudel.setPartsOpacity("PARTS_01_FACE_001",0);

        //鼠标拖拽引起的动作变化
        Vector3 pos = Input.mousePosition;//获取到鼠标当前的坐标
        if(Input.GetMouseButton(0)){
            //将当前鼠标坐标转换为live2d所能识别的坐标   固定公式  screen 屏幕
            drag.Set(pos.x/Screen.width*2-1,pos.y/Screen.height*2-1);
        }else if(Input.GetMouseButtonUp(0)){
            drag.Set(0,0);//将坐标调回原点  即动作回正
        }

        //参数及时更新
        drag.update();

        //模型的转向
        if(drag.getX() != 0 || drag.getY() != 0){
            live2DMoudel.setParamFloat("PARAM_ANGLE_X",30*drag.getX());
            live2DMoudel.setParamFloat("PARAM_ANGLE_Y",30*drag.getY());

            live2DMoudel.setParamFloat("PARAM_BODY_ANGLE_X",10*drag.getX());


            live2DMoudel.setParamFloat("PARAM_EYE_BALL_X",drag.getX());
            live2DMoudel.setParamFloat("PARAM_EYE_BALL_Y",drag.getY());
        }

        //自动眨眼
        eyeBlinkMotion.setParam(live2DMoudel);

        //模型头发的摆动
        long time = UtSystem.getSystemTimeMSec();//获取执行时间
        physicsHairSideLeft.update(live2DMoudel,time);
        physicsHairSideRight.update(live2DMoudel,time);
        physicsHairBackLeft.update(live2DMoudel,time);
        physicsHairBackRight.update(live2DMoudel,time);

        //表情动作的播放
        if(Input.GetKeyDown(KeyCode.Space)){
            motionIndex++;
            if(motionIndex >= expressions.Length){
                motionIndex = 0;
            }
            expressionManager.startMotion(expressions[motionIndex]);
        }
        
        expressionManager.updateParam(live2DMoudel);//确定要实现动作的模型

        live2DMoudel.update();//更新模型的所有信息
    }

    private void OnRenderObject() {
        //生命周期函数的一种  用于画出模型
        live2DMoudel.draw();
    }

    private void StartMotion(int MotionIndex,int priority){
        //当要播放的动作优先比当前正在执行的动作的优先级小时，不执行
        if(manager.getCurrentPriority() >= priority){
            return;
        }
        manager.startMotion(motions[motionIndex]);
    }
}

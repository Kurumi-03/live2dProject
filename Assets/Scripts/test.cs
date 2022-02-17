using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using live2d;
using live2d.framework;
public class test : MonoBehaviour {
 
    public TextAsset modelFile;
 
    private Live2DModelUnity live2DModel;
    public Texture2D[] textures;
    public TextAsset[] motionFiles;
    //动作优先级管理
    //优先级的设置标准 
    //1.动作未进行的状态，优先级为0
    //2.待机动作发生时，优先级为1
    //3.其他动作进行时，优先级为2
    //4.无视优先级，强制发生的动作
    private L2DMotionManager l2DMotionManager;
 
    //动作数组
    private Live2DMotion[] motions;
 
    //动作管理队列,管理动画的播放
    private MotionQueueManager motionQueueManager;
 
    //第二个动作管理,要同时播放几个动画就要有几个管理者
    private MotionQueueManager motionQueueManagerA;
 
    //动作索引
    public int motionIndex;
 
    //live2d的画布
    private Matrix4x4 live2dCanvasPos;
    //动作对象
    //private Live2DMotion live2DMotionIdle;
 
	// Use this for initialization
	void Start () {
        //初始化环境  使用环境前一定要调用一次  连续调用/不调用会出错
        //可以释放后再次初始化重新使用
        Live2D.init();
 
        //释放环境 没有初始化是不能释放的
        //Live2D.dispose();
 
        //1.读取模型 
        //Live2DModelUnity专门负责模型的加载
        //加载moc文件一定要有.moc后缀
        //加载moc↓
        //Live2DModelUnity.loadModel(Application.dataPath + "/Resources/Epsilon/Epsilon/runtime/Epsilon.moc");  //二进制文件或者moc文件路径
        //加载二进制文件  复制moc文件，在副本文件名后面加上.bytes后缀变为二进制文件
        //使用textAsset接收二进制文件
        //加载二进制 ↓
        //TextAsset mocFile = Resources.Load<TextAsset>("Epsilon/Epsilon/runtime/Epsilon.moc");
        //Live2DModelUnity.loadModel(mocFile.bytes);
        //方法返回一个类型为Live2DModelUnity的模型对象，对应于加载的这个模型
         live2DModel =  Live2DModelUnity.loadModel(modelFile.bytes);
 
        //2.联系贴图
        //只能通过路径读取
        //Texture2D texture2D = Resources.Load<Texture2D>("Epsilon/Epsilon/runtime/Epsilon.1024");
        //live2DModel.setTexture(0, texture2D);
        //设置贴图  前一个参数是贴图序号，由建模师决定
        //设置多张贴图
        //Texture2D texture2D1 = Resources.Load<Texture2D>("Epsilon/Epsilon/runtime/Epsilon.1024/texture_00");
        //Texture2D texture2D2 = Resources.Load<Texture2D>("Epsilon/Epsilon/runtime/Epsilon.1024/texture_01");
        //Texture2D texture2D3 = Resources.Load<Texture2D>("Epsilon/Epsilon/runtime/Epsilon.1024/texture_02");
        //live2DModel.setTexture(0, texture2D1);
        //live2DModel.setTexture(1, texture2D2);
        //live2DModel.setTexture(2, texture2D3);
 
        for (int i = 0; i < textures.Length; i++)
        {
            live2DModel.setTexture(i, textures[i]);
        }
 
        //3与绘图环境建立链接（unity不需要)
        //4指定显示位置与尺寸
        //定义画布 Matrix4x4 live2dCanvasPos;
        //创建正交的投影矩阵 参数是左右上下近视口远视口
        float modelWidth = live2DModel.getCanvasWidth();
        //+-50由官方提供
        //将相机设置为正交相机
        live2dCanvasPos = Matrix4x4.Ortho(0, modelWidth, modelWidth, 0, 50, -50);
 
        //播放动作
        //实例化动作对象 加载mtn文件
        //live2DMotionIdle = Live2DMotion.loadMotion(Application.dataPath + "");
        //TextAsset mtnFile = Resources.Load<TextAsset>("");
        //live2DMotionIdle =  Live2DMotion.loadMotion(mtnFile.bytes);
        //将读入的motion文件加载入对应的Live2DMotion数组中
        motions = new Live2DMotion[motionFiles.Length];
        for (int i = 0; i < motions.Length; i++)
        {
            motions[i] = Live2DMotion.loadMotion(motionFiles[i].bytes);
        }
        /*
        //设置动画的属性
        //重复播放时是否淡入,动作跨度大设置可以看起来更自然 参数为bool
        motions[0].setLoopFadeIn(false);//重复播放不淡入
        //设置淡入淡出时间
        //淡出，不设置默认值也为1000毫秒。
        motions[0].setFadeOut(1000);
        //淡入
        motions[0].setFadeIn(1000);
        //设置动画是否循环播放
        motions[0].setLoop(true);
        motionQueueManager = new MotionQueueManager();
        //播放动画
        motionQueueManager.startMotion(motions[0]);
        //动画的重叠
        motions[5].setLoop(true);
        motionQueueManagerA = new MotionQueueManager();
        motionQueueManagerA.startMotion(motions[5]);*/
 
        //动作的优先级使用
        l2DMotionManager = new L2DMotionManager();
    }
	
	// Update is called once per frame
	void Update () {
        //设置矩阵  两个矩阵相乘   
        //localToWorldMatrix局部转世界
        live2DModel.setMatrix(transform.localToWorldMatrix * live2dCanvasPos );
        //更新顶点，贴图等
 
        //按M更改动画
        //if (Input.GetKeyDown(KeyCode.M))
        //{
        //    motionIndex++;
        //    if(motionIndex >= motions.Length)
        //    {
        //        motionIndex = 0;
        //    }
        //    motionQueueManager.startMotion(motions[motionIndex]);
        //}
 
        //指定哪一个模型播放动画
        //motionQueueManager.updateParam(live2DModel);
        //motionQueueManagerA.updateParam(live2DModel);
 
        //优先级设置
        //判断待机动作,isFinished判断动作完毕否
        if (l2DMotionManager.isFinished())
        {
            StartMotion(0, 1);//0为idle动画，优先级是1
        }
        else if (Input.GetKeyDown(KeyCode.M))
        {
            StartMotion(14, 2);
        }
        //更新动作数据
        l2DMotionManager.updateParam(live2DModel);
 
 
        live2DModel.update();
    }
    //绘图方法
    private void OnRenderObject()
    {
        live2DModel.draw();
    }
    //播放动画的方法封装
    public void StartMotion(int motionIndex, int priority)
    {
        //getCurrentPriority()正在播放动画的优先级
        //与要播放的动画的优先级对比，同优先级或者大于要播放的动画的优先级，不操作
        if (l2DMotionManager.getCurrentPriority()>= priority)
        {
            return;
        }
        //要播放的动画优先级比现在的大
        l2DMotionManager.startMotion(motions[motionIndex]);
    }
    
}
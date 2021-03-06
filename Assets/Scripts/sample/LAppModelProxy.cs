/**
 *
 *  You can modify and use this source freely
 *  only for the development of application related Live2D.
 *
 *  (c) Live2D Inc. All rights reserved.
 */
using UnityEngine;
using System;
using live2d;

[ExecuteInEditMode]
public class LAppModelProxy : MonoBehaviour
{
    public String path="";
    public int sceneNo = 0;

    private LAppModel model;

	private bool				isVisible = false ;
	
    //后加的
    public bool isRunning;
    public float speed;
    private Vector3 initPos;
    //后加的

	void Awake()
    {
        if (path == "") return;
        model = new LAppModel(this);

        LAppLive2DManager.Instance.AddModel(this);

        var filename = FileManager.getFilename(path);
        var dir = FileManager.getDirName(path);

        Debug.Log("Load " + dir +"  filename:"+ filename);
		model.LoadFromStreamingAssets(dir, filename);

        //后加的
        initPos = transform.position;
        //后加的
    }
	

	void OnRenderObject()
	{
		if(!isVisible) return;
		if(model==null)return;

		if(model.GetLive2DModelUnity().getRenderMode() == Live2D.L2D_RENDER_DRAW_MESH_NOW)
		{
            model.Draw();
		}
		
		if (LAppDefine.DEBUG_DRAW_HIT_AREA)
		{
			// デバッグ用当たり判定の描画
			model.DrawHitArea();
		}
	}
	

	void Update()
	{
		if(!isVisible) return;
        if (model == null) return;

        model.Update();
        if (model.GetLive2DModelUnity().getRenderMode() == Live2D.L2D_RENDER_DRAW_MESH)
        {
            model.Draw();
        }

        //后加的
        if(GameManager.Instance != null ){
            if(GameManager.Instance.gameOver){
                isRunning = false;
                return;
            }
        }
        if(isRunning){
            transform.Translate(Vector3.right*speed*Time.deltaTime);
        }
        if(GameManager.Instance != null ){
            if(GameManager.Instance.badBoy.isDefeat){
                transform.position = Vector3.Lerp(transform.position,initPos,0.1f);
            }
        }
        //后加的
	}


    public LAppModel GetModel()
    {
        return model;
    }


	public void SetVisible(bool isVisible)
	{
		this.isVisible = isVisible;
	}


	public bool GetVisible()
	{
		return isVisible;
	}


    public void ResetAudioSource()
    {
        Component[] components = gameObject.GetComponents<AudioSource>();
        for (int i = 0; i < components.Length; i++)
        {
            Destroy(components[i]);
        }
    }
}
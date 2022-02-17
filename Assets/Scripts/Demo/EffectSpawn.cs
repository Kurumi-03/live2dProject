using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectSpawn : MonoBehaviour
{
    
    public GameObject[] effectGos;
    public Transform canvasTrans;
    
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("CreatEffect",0,2);//0s开始调用，每隔2s调用一次
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void CreatEffect(){
        int randomIndex = Random.Range(0,2);
        transform.rotation = Quaternion.Euler(new Vector3(0,0,Random.Range(0,45)));//改变朝向
        GameObject effectGo = Instantiate(effectGos[randomIndex],transform.position,transform.rotation);
        effectGo.transform.SetParent(canvasTrans);//设置特效的父对象为特效孵化器
    }
}






using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectMove : MonoBehaviour
{
    public float moveSpeed;
    public int randomYPos;
    public float timeVal;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject,12);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(-transform.right*moveSpeed*Time.deltaTime);
        if(timeVal >=1 ) {
            randomYPos = Random.Range(-1,2);//随机3个数 -1, 0 ,1
            timeVal = 0;
        }
        else{
            transform.Translate(transform.up*randomYPos*Time.deltaTime*moveSpeed/5);
            timeVal += Time.deltaTime;
        }
    }
}

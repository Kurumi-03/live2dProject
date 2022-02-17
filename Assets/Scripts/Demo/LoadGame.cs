using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadGame : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(LoadGameSence);//注册方法事件    
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LoadGameSence(){
        SceneManager.LoadScene(1);
    }
}

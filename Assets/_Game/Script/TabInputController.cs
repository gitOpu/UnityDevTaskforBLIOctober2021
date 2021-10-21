using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabInputController : MonoBehaviour
{


    public int inputSelected = 0;

    void Awake()
    {
        UIPlayerView.instance.name.Select();
    }
    void Update()
    {
       if(Input.GetKeyDown(KeyCode.Tab))
        {
            if (UIPlayerView.instance.name.isFocused)
            {
                UIPlayerView.instance.age.Select();
            }
            else if (UIPlayerView.instance.age.isFocused)
            {
                UIPlayerView.instance.type.Select();
            }
            
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            UIPlayerView.instance.AddPlayer();

        }
    }
}

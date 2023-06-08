using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeStoreTabsColor : MonoBehaviour
{

    public GameObject[] tabs;
    private Color normalColor;
    private Color otherColor = new Color(0.66f,0.66f, 0.66f);
    // Use this for initialization
    void Start()
    {
        normalColor = tabs[1].GetComponent<Image>().color;

        SetSelectectedTab(1);
    }

    public void SetSelectectedTab(int index)
    {
        for (int i = 0; i < tabs.Length; i++)
        {
            if (i != index)
            {
                tabs[i].GetComponent<Image>().color = otherColor;

            }
            else
            {
                tabs[i].GetComponent<Image>().color = normalColor;
            }
        }

    }

    // Update is called once per frame
    void Update()
    {

    }
}

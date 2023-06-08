using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using AssemblyCSharp;
using UnityEngine;
using UnityEngine.UI;

public class EmojiShopController : MonoBehaviour
{

    public GameObject priceText;
    public GameObject chatName;
    public GameObject button;
    public GameObject buttonText;
    private float price;
    private int index;
    public GameObject[] bubbles;
    public GameObject parent;
    public GameObject emojiPrefab;
    Sprite[] emojiSprites;
    int emojiPerPack;
    int packsCount;
    // Use this for initialization
    void Start()
    {

    }

    public void fillData(int i)
    {
        emojiSprites = GameObject.Find("StaticGameVariablesContainer").GetComponent<StaticGameVariablesController>().emoji;
        emojiPerPack = GameObject.Find("StaticGameVariablesContainer").GetComponent<StaticGameVariablesController>().emojiPerPack;
        packsCount = GameObject.Find("StaticGameVariablesContainer").GetComponent<StaticGameVariablesController>().packsCount;

        this.index = i;

        float price = StaticStrings.emojiPrice;
        this.price = price;
        priceText.GetComponent<Text>().text = price.ToString();


        for (int j = 0; j < emojiPerPack; j++)
        {
            GameObject emoji = Instantiate(emojiPrefab);
            emoji.transform.SetParent(parent.transform, false);
            emoji.GetComponent<Image>().sprite = emojiSprites[(i + 1) * emojiPerPack + j];
        }



        if (GameManager.Instance.myPlayerData.GetEmoji() != null &&
            GameManager.Instance.myPlayerData.GetEmoji().Length > 0 && GameManager.Instance.myPlayerData.GetEmoji().Contains("'" + i + "'"))
        {
            button.GetComponent<Button>().interactable = false;
            buttonText.GetComponent<Text>().text = "Owned";
        }
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void buyEmoji()
    {
        //GameManager.Instance.IAPControl.OnBuyClicked("EMOJI", index, this.gameObject);
        GameManager.Instance.IAPControl.OnBuyClickedTesting("EMOJI", index, this.gameObject);

        //if (GameManager.Instance.myPlayerData.GetCoins() >= this.price)
        //GameManager.Instance.IAPControl.OnBuyClicked("EMOJI",index,this.gameObject);
        /*{
            GameManager.Instance.playfabManager.addCoinsRequest(-this.price);
            
            button.GetComponent<Button>().interactable = false;
            buttonText.GetComponent<Text>().text = "Owned";
        }
        else
        {
            GameManager.Instance.dialog.SetActive(true);
        }*/
    }

    public void SetButtonText(){
        button.GetComponent<Button>().interactable = false;
        buttonText.GetComponent<Text>().text = "Owned";
    }
}


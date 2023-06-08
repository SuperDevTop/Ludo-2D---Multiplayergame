using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Globalization;
using AssemblyCSharp;

public class ChatShopController : MonoBehaviour
{


    public GameObject priceText;
    public GameObject chatName;
    public GameObject button;
    public GameObject buttonText;
    private float price;
    private int index;
    public GameObject[] bubbles;

    // Use this for initialization
    void Start()
    {


    }

    public void fillData(int i)
    {
        this.index = i;
        string[] messages = StaticStrings.chatMessagesExtended[i];
        float price = StaticStrings.chatPrice;
        string name = StaticStrings.chatNames[i];
        this.price = price;
        priceText.GetComponent<Text>().text = price.ToString();
        chatName.GetComponent<Text>().text = name;

        for (int j = 0; j < messages.Length; j++)
        {
            bubbles[j].transform.GetChild(0).GetComponent<Text>().text = messages[j];
            bubbles[j].SetActive(true);
        }

        for (int j = 5; j >= messages.Length; j--)
        {
            bubbles[j].SetActive(false);
        }



        if (GameManager.Instance.myPlayerData.GetChats() != null &&
            GameManager.Instance.myPlayerData.GetChats().Length > 0 && GameManager.Instance.myPlayerData.GetChats().Contains("'" + i + "'"))
        {
            button.GetComponent<Button>().interactable = false;
            buttonText.GetComponent<Text>().text = "Owned";
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void buyChat()
    {
        //GameManager.Instance.IAPControl.OnBuyClicked("CHAT", this.index, this.gameObject);
        GameManager.Instance.IAPControl.OnBuyClickedTesting("CHAT", this.index, this.gameObject);

        /*if (GameManager.Instance.myPlayerData.GetCoins() >= this.price)
        {
            GameManager.Instance.playfabManager.addCoinsRequest(-this.price);
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
    
    public void buyEmoji()
    {
        if (GameManager.Instance.myPlayerData.GetCoins() >= this.price)
        {
            GameManager.Instance.playfabManager.addCoinsRequest(-this.price);
            GameManager.Instance.playfabManager.UpdateBoughtEmojis(this.index);
            button.GetComponent<Button>().interactable = false;
            buttonText.GetComponent<Text>().text = "Owned";
        }
        else
        {
            GameManager.Instance.dialog.SetActive(true);
        }
    }
}

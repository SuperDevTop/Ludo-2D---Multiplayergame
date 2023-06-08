using UnityEngine;
using System.Collections;
using UnityEngine.Purchasing;
using System;

public class IAPController : MonoBehaviour, IStoreListener
{

    public string SKU_1000_COINS = "pool_5000_coins";
    public string SKU_5000_COINS = "pool_10000_coins";
    public string SKU_10000_COINS = "pool_25000_coins";
    public string SKU_50000_COINS = "pool_75000_coins";
    public string SKU_100000_COINS = "pool_200000_coins";

    public string[] SKU_CHAT = {"chat_1","chat_2","chat_3","chat_4","chat_5","chat_6"};
    
    public string[] SKU_EMOJI = {"emoji_1","emoji_2","emoji_3","emoji_4","emoji_5"};

    public IStoreController controller;
    private IExtensionProvider extensions;
    GameObject currentObject;

    // Use this for initialization
    void Start()
    {
        DontDestroyOnLoad(transform.gameObject);
        GameManager.Instance.IAPControl = this;
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        /*builder.AddProduct(SKU_1000_COINS, ProductType.Consumable);
        builder.AddProduct(SKU_5000_COINS, ProductType.Consumable);
        builder.AddProduct(SKU_10000_COINS, ProductType.Consumable);
        builder.AddProduct(SKU_50000_COINS, ProductType.Consumable);
        builder.AddProduct(SKU_100000_COINS, ProductType.Consumable);*/

        foreach(var value in SKU_CHAT){
            builder.AddProduct(value,ProductType.NonConsumable);
        }

        foreach(var value in SKU_EMOJI){
            builder.AddProduct(value,ProductType.NonConsumable);
        }        
    
        UnityPurchasing.Initialize(this, builder);
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        Debug.Log("IAP Initizalization complete!!");
        this.controller = controller;
        this.extensions = extensions;
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.Log("IAP Initizalization FAILED!! " + error.ToString());
    }

    public void OnPurchaseFailed(Product i, PurchaseFailureReason p)
    {
        Debug.Log("IAP purchase FAILED!! " + p.ToString());
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
    {
        if (e.purchasedProduct.definition.id == SKU_1000_COINS)
        {
            GameManager.Instance.playfabManager.addCoinsRequest(5000);
        }
        else if (e.purchasedProduct.definition.id == SKU_5000_COINS)
        {
            GameManager.Instance.playfabManager.addCoinsRequest(10000);
        }
        else if (e.purchasedProduct.definition.id == SKU_10000_COINS)
        {
            GameManager.Instance.playfabManager.addCoinsRequest(25000);
        }
        else if (e.purchasedProduct.definition.id == SKU_50000_COINS)
        {
            GameManager.Instance.playfabManager.addCoinsRequest(75000);
        }
        else if (e.purchasedProduct.definition.id == SKU_100000_COINS)
        {
            GameManager.Instance.playfabManager.addCoinsRequest(200000);
        }

        for(int i = 0;i < SKU_CHAT.Length;i++){
            if(SKU_CHAT[i] == e.purchasedProduct.definition.id){
                GameManager.Instance.playfabManager.updateBoughtChats(i);
                currentObject.GetComponent<ChatShopController>().SetButtonText();
                return PurchaseProcessingResult.Complete;
            }
        }

        for(int i = 0;i < SKU_EMOJI.Length;i++){
            if(SKU_EMOJI[i] == e.purchasedProduct.definition.id){
                GameManager.Instance.playfabManager.UpdateBoughtEmojis(i);
                currentObject.GetComponent<EmojiShopController>().SetButtonText();
                return PurchaseProcessingResult.Complete;
            }
        }

        return PurchaseProcessingResult.Complete;
    }

    public void OnBuyClicked(string productType,int productId,GameObject currentObject){
        this.currentObject = currentObject;

        if(productType.Equals("CHAT")){
            controller.InitiatePurchase(SKU_CHAT[productId]);
        }else{
            if(productType.Equals("EMOJI")){
                controller.InitiatePurchase(SKU_EMOJI[productId]);
            }
        }
    }

    public void OnBuyClickedTesting(string productType, int productId, GameObject currentObject)
    {
        if (productType.Equals("CHAT"))
        {
            GameManager.Instance.playfabManager.updateBoughtChats(productId);
            currentObject.GetComponent<ChatShopController>().SetButtonText();
        }
        else
        {
            if (productType.Equals("EMOJI"))
            {
                GameManager.Instance.playfabManager.UpdateBoughtEmojis(productId);
                currentObject.GetComponent<EmojiShopController>().SetButtonText();
            }
        }
    }

    public void OnPurchaseClicked(int productId)
    {
        if (controller != null)
        {
            if (productId == 1)
            {
                controller.InitiatePurchase(SKU_1000_COINS);
            }
            else if (productId == 2)
            {
                controller.InitiatePurchase(SKU_5000_COINS);
            }
            else if (productId == 3)
            {
                controller.InitiatePurchase(SKU_10000_COINS);
            }
            else if (productId == 4)
            {
                controller.InitiatePurchase(SKU_50000_COINS);
            }
            else if (productId == 5)
            {
                controller.InitiatePurchase(SKU_100000_COINS);
            }
        }
    }

    // Apple currently requires explicit purchase restoration for IAP, conditionally displaying a password prompt.
    public void RestorePurchases()
    {
        // If we are running on an Apple device ... 
        if (Application.platform == RuntimePlatform.IPhonePlayer ||
            Application.platform == RuntimePlatform.OSXPlayer)
        {
            // ... begin restoring purchases
            Debug.Log("RestorePurchases started ...");

            // Fetch the Apple store-specific subsystem.
            var apple = extensions.GetExtension<IAppleExtensions>();
            // Begin the asynchronous process of restoring purchases. Expect a confirmation response in 
            // the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
            apple.RestoreTransactions((result) =>
            {
                // The first phase of restoration. If no more responses are received on ProcessPurchase then 
                // no purchases are available to be restored.
                Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
            });
        }
        // Otherwise ...
        else
        {
            // We are not running on an Apple device. No work is necessary to restore purchases.
            Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
        }
    }
}

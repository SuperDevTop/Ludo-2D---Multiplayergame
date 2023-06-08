using UnityEngine;
using UnityEngine.UI;

public class UpdateCoinsFrameValue : MonoBehaviour
{

    private float currentValue = 0;
    private Text text;

    // Use this for initialization
    void Start()
    {
        text = GetComponent<Text>();
        //get genie values
        InvokeRepeating("CheckAndUpdateValue", 0, 0.2f);
    }

    private void CheckAndUpdateValue()
    {
        if (currentValue != Ludo.IN.GameManager.Instance.Genie_user_funds)
        {
            currentValue = Ludo.IN.GameManager.Instance.Genie_user_funds;
            text.text = Ludo.IN.GameManager.Instance.Genie_user_funds.ToString("F2");
        }
    }
}

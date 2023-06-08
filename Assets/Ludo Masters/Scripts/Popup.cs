using UnityEngine;
using UnityEngine.UI;

public class Popup : MonoBehaviour {
	public Text text;
	public InitMenuScript initMenu;
	
	public void OnYesButton(int index){
		initMenu.ShowGameConfiguration(index);
	}

	public void OnNoButton(){
		Destroy(gameObject);
	}

	public void SetGameInformation(string information){
		text.text = information;
	}
}

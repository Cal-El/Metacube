using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EndPlaytestGUI : MonoBehaviour {

    [SerializeField]
    InputField textBox;

    void Update () {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void QuitButton() {
        if(textBox.text != null && textBox.text.Length > 0) {
            PlaytestData.LogApplicationEvent("Email: " + textBox.text);
        }
        Application.Quit();
    }
}

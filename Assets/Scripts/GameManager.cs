using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    public static GameManager instance;
    public GameObject winPanel;
    public Text textbox;
    public float duration;
    public float typeDuration;
    private float timer;
    private float typingTimer;
    private string winText = "YOU WIN\nPRESS 'R' TO RESTART";
    private int charIndex;

    public bool isWin;
    private Image winPanelImage;

    private void Awake() {
        if (instance == null) {
            instance = this;
        }

        winPanelImage = winPanel.GetComponent<Image>();
    }

    private void Update() {
        if (!isWin) return;
        EventSystem.current.SetSelectedGameObject(null);
        Color tempColor;
        if (Input.GetKey(KeyCode.R)) {
            SceneManager.LoadScene("main");
        }
        else {
            timer += Time.deltaTime;
            tempColor = winPanelImage.color;
            tempColor.a = Mathf.Lerp(0f, 1f, timer / duration);
            winPanelImage.color = tempColor;
            if (timer > duration) {
                typingTimer += Time.deltaTime;
                if (typingTimer > typeDuration && charIndex < winText.Length) {
                    typingTimer = 0f;
                    textbox.text += winText[charIndex];
                    charIndex++;
                }
            }
        }
    }
}
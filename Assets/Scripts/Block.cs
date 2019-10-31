using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Block : MonoBehaviour, IPointerDownHandler, ISelectHandler, IDeselectHandler {
    public AnimationCurve curve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);

    public float duration;
    public Sprite highlighted;
    public Sprite shaded;
    private float startX;
    private float startY;
    private Vector2 targetPosition;
    private Vector2 startPosition;
    private float width;
    private RectTransform rectTransform;
    private float timer;
    private Button button;
    private Transform activeCanvas;
    private Transform parent;
    private bool isHighlighted;
    private Image imageComponent;

    private void Start() {
        rectTransform = GetComponent<RectTransform>();
        startPosition = rectTransform.anchoredPosition;
        targetPosition = startPosition;
        width = rectTransform.rect.width;
        button = GetComponent<Button>();
        parent = transform.parent;
        activeCanvas = parent.Find("ActiveElement");
    }

    private void Awake() {
        imageComponent = GetComponent<Image>();
    }

    public void OnPointerDown(PointerEventData eventData) {
        if (button.IsInteractable()) {
            button.Select();
        }
    }

    public void OnSelect(BaseEventData eventData) {
        transform.SetParent(activeCanvas);
    }

    public void OnDeselect(BaseEventData eventData) {
        transform.SetParent(parent);
    }

    private void Update() {
        if (!GameManager.instance.isWin) {
            if (EventSystem.current.currentSelectedGameObject == gameObject ||
                !rectTransform.anchoredPosition.Equals(targetPosition)) {
                if (rectTransform.anchoredPosition.Equals(targetPosition)) {
                    var up = Input.GetKey(KeyCode.UpArrow);
                    var down = Input.GetKey(KeyCode.DownArrow);
                    var left = Input.GetKey(KeyCode.LeftArrow);
                    var right = Input.GetKey(KeyCode.RightArrow);
                    if (down) {
                        timer = 0f;
                        startPosition = rectTransform.anchoredPosition;
                        targetPosition = new Vector2(startPosition.x, startPosition.y - width);
                    }
                    else if (up) {
                        timer = 0f;
                        startPosition = rectTransform.anchoredPosition;
                        targetPosition = new Vector2(startPosition.x, startPosition.y + width);
                    }

                    if (left) {
                        timer = 0f;
                        startPosition = rectTransform.anchoredPosition;
                        targetPosition = new Vector2(startPosition.x - width, startPosition.y);
                    }
                    else if (right) {
                        timer = 0f;
                        startPosition = rectTransform.anchoredPosition;
                        targetPosition = new Vector2(startPosition.x + width, startPosition.y);
                    }
                }
                else {
                    timer += Time.deltaTime;
                    rectTransform.anchoredPosition =
                        Vector3.Lerp(startPosition, targetPosition, curve.Evaluate(timer / duration));
                }
            }
        }
    }

    public void setHighlighted(bool isHighlighted) {
        if (isHighlighted != this.isHighlighted) {
            this.isHighlighted = isHighlighted;
            imageComponent.sprite = isHighlighted ? highlighted : shaded;
        }
    }

    public bool IsHighlighted() {
        return isHighlighted;
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (EventSystem.current.currentSelectedGameObject == gameObject) {
            timer = 0f;
            targetPosition = startPosition;
            startPosition = rectTransform.anchoredPosition;
        }
    }
}
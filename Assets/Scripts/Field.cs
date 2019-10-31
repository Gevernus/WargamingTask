using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Field : MonoBehaviour {
    public int gameBlocksCount;
    public int fixedBlocksCount;
    public int freeBlocksCount;
    public GameObject[] blockPrefabs;
    public GameObject fixedBlockPrefab;
    public Boundary boundary;

    private readonly List<RectTransform> blocks = new List<RectTransform>();
    private readonly List<RectTransform> fixedBlocks = new List<RectTransform>();
    private readonly List<RectTransform> conditionBlocks = new List<RectTransform>();
    private readonly List<Vector2> positions = new List<Vector2>();
    private readonly HashSet<Vector2> fixedPositions = new HashSet<Vector2>();
    private readonly Dictionary<string, float> winPositions = new Dictionary<string, float>();
    private readonly Dictionary<string, int> winCounters = new Dictionary<string, int>();

    // Start is called before the first frame update
    void Start() {
        var component = GetComponent<Transform>();
        var rectWidth = fixedBlockPrefab.GetComponent<RectTransform>().rect.width;
        for (var i = 0; i < blockPrefabs.Length; i++) {
            var blockPrefab = blockPrefabs[i];
            for (int j = 0; j < ((gameBlocksCount - fixedBlocksCount - freeBlocksCount) / blockPrefabs.Length); j++) {
                blocks.Add(Instantiate(blockPrefab, component).GetComponent<RectTransform>());
            }

            var conditionBlock = Instantiate(blockPrefab, component);
            conditionBlock.GetComponent<Button>().interactable = false;
            var rectTransform = conditionBlock.GetComponent<RectTransform>();
            conditionBlocks.Add(rectTransform);
            rectTransform.anchoredPosition =
                new Vector2(i * rectWidth * 2, rectWidth * 3 / 2);
        }

        for (int j = 0; j < fixedBlocksCount; j++) {
            fixedBlocks.Add(Instantiate(fixedBlockPrefab, component).GetComponent<RectTransform>());
        }

        while (fixedPositions.Count < 3) {
            fixedPositions.Add(new Vector2(1 * rectWidth, -Random.Range(0, 5) * rectWidth));
        }

        while (fixedPositions.Count < 6) {
            fixedPositions.Add(new Vector2(3 * rectWidth, -Random.Range(0, 5) * rectWidth));
        }

        for (int i = boundary.left; i < boundary.right; i += boundary.delta) {
            for (int j = boundary.top; j < boundary.bottom; j += boundary.delta) {
                positions.Add(new Vector2(i * rectWidth, -j * rectWidth));
            }
        }

        Restart();
    }

    private void FixedUpdate() {
        if (!GameManager.instance.isWin) {
            foreach (var rectTransform in blocks) {
                CheckHighlight(rectTransform);
            }

            bool isWin = true;
            for (var i = 0; i < conditionBlocks.Count; i++) {
                var highlightComponent = conditionBlocks[i].GetComponent<Block>();
                if (winCounters[conditionBlocks[i].tag] != 0) {
                    isWin = false;
                    highlightComponent.setHighlighted(false);
                }
                else {
                    highlightComponent.setHighlighted(true);
                }
            }

            if (isWin) {
                GameManager.instance.isWin = true;
            }
        }
    }

    private void CheckHighlight(RectTransform component) {
        var highlightComponent = component.GetComponent<Block>();
        if (winPositions[component.tag].Equals(component.anchoredPosition.x)) {
            if (!highlightComponent.IsHighlighted()) {
                highlightComponent.setHighlighted(true);
                winCounters[component.tag]--;
            }
        }
        else {
            if (highlightComponent.IsHighlighted()) {
                highlightComponent.setHighlighted(false);
                winCounters[component.tag]++;
            }
        }
    }

    private void Restart() {
        winCounters.Clear();
        winPositions.Clear();
        for (var i = 0; i < conditionBlocks.Count; i++) {
            var dest = Random.Range(i, conditionBlocks.Count);
            var position = conditionBlocks[i].anchoredPosition;
            conditionBlocks[i].anchoredPosition = conditionBlocks[dest].anchoredPosition;
            conditionBlocks[dest].anchoredPosition = position;
        }

        for (var i = 0; i < conditionBlocks.Count; i++) {
            winPositions.Add(conditionBlocks[i].tag, conditionBlocks[i].anchoredPosition.x);
        }

        for (var i = 0; i < positions.Count; i++) {
            var dest = Random.Range(i, positions.Count);
            var position = positions[i];
            positions[i] = positions[dest];
            positions[dest] = position;
        }

        var fixedBlockEnumerator = fixedBlocks.GetEnumerator();
        var blockEnumerator = blocks.GetEnumerator();
        for (var i = 0; i < positions.Count; i++) {
            if (fixedPositions.Contains(positions[i])) {
                if (fixedBlockEnumerator.MoveNext() && fixedBlockEnumerator.Current != null) {
                    fixedBlockEnumerator.Current.anchoredPosition = positions[i];
                }
            }
            else {
                if (blockEnumerator.MoveNext() && blockEnumerator.Current != null) {
                    blockEnumerator.Current.anchoredPosition = positions[i];
                    int counter = 1;
                    if (winCounters.ContainsKey(blockEnumerator.Current.tag)) {
                        winCounters[blockEnumerator.Current.tag]++;
                    }
                    else {
                        winCounters.Add(blockEnumerator.Current.tag, counter);
                    }
                }
            }
        }

        fixedBlockEnumerator.Dispose();
        blockEnumerator.Dispose();
    }

    [System.Serializable]
    public struct Boundary {
        public int left;
        public int right;
        public int top;
        public int bottom;
        public int delta;
    }
}
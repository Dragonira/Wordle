using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class Tile : MonoBehaviour
{
    

    private Image image;
    private Outline outline;
    private TextMeshProUGUI textMesh;
    public State state { get; private set; }
    public char letter { get; private set; }
   
    [System.Serializable]
    public class State
    {
       public Color fillColor;
       public Color outlineColor;
    }
    private void Awake()
    {
        textMesh = GetComponentInChildren<TextMeshProUGUI>();
        image = GetComponent<Image>();
        outline = GetComponent<Outline>();
    } 

    public void SetLetter(char letter)
    {
        this.letter = letter;
        textMesh.text = letter.ToString();
    }

    public void SetState(State state)
    {
        this.state = state;
        image.color = state.fillColor;
        outline.effectColor = state.outlineColor;

    }
}

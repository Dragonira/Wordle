
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEngine.Rendering.DebugUI.Table;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;


public class Board : MonoBehaviour
{

    private static readonly KeyCode[] rowKeys = { KeyCode.A, KeyCode.B, KeyCode.C, KeyCode.D, KeyCode.E, KeyCode.F,
                                                  KeyCode.G, KeyCode.H, KeyCode.I, KeyCode.J, KeyCode.K, KeyCode.L,
                                                  KeyCode.M, KeyCode.N, KeyCode.O, KeyCode.P, KeyCode.Q, KeyCode.R,
                                                  KeyCode.S, KeyCode.T, KeyCode.U, KeyCode.V, KeyCode.W, KeyCode.X,
                                                  KeyCode.Y, KeyCode.Z};

    private string[] solution;
    private string[] validWords;
    private string word;
    private Row[] rows;
    private int rowindex;
    private int colindex;

    public Tile.State emptyState;
    public Tile.State occupiedState;
    public Tile.State correctState;
    public Tile.State wrongSpotState;
    public Tile.State incorrectState;

   [SerializeField] private TextMeshProUGUI textMesh;
   [SerializeField] private TextMeshProUGUI Theword;
   [SerializeField] private Button PlayagainButton;
   [SerializeField] private Button TryAgainButton;

    private void Awake()
    {
        rows = GetComponentsInChildren<Row>();
        


    }
    private void Start()
    {
        LoadData();
        NewGame();
    }

    public void NewGame()
    {
        ClearBoard();
        SetRandomWord();
        textMesh.gameObject.SetActive(false);
        enabled = true;
    }
    public void TryAgain()
    {
        ClearBoard();
        textMesh.gameObject.SetActive(false);

        enabled = true;
    }
    private void SetRandomWord()
    {
        word = solution[Random.Range(0, solution.Length)];
        word = word.ToLower().Trim();

    }
    private void LoadData()
    {
     TextAsset textFile = Resources.Load<TextAsset>("official_wordle_all") as TextAsset;
        validWords = textFile.text.Split('\n');

       textFile = Resources.Load<TextAsset>("official_wordle_common") as TextAsset;
        solution = textFile.text.Split('\n');
    }
    private void Update()
    {
      
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            colindex = Mathf.Max(colindex - 1, 0);
            rows[rowindex].tiles[colindex].SetLetter('\0');
            rows[rowindex].tiles[colindex].SetState(emptyState);

            textMesh.gameObject.SetActive(false);

        }
        else if (colindex == rows[rowindex].tiles.Length)
        {
           if (Input.GetKeyDown(KeyCode.Return))
            {
                SubmitRow(rows[rowindex]);
            }
        }
        else 
        {
            for (int i = 0; i < rowKeys.Length; i++)
            {
                if (Input.GetKeyDown(rowKeys[i]))
                {
                    rows[rowindex].tiles[colindex].transform.DOScaleX(0.1f, 0.1f).SetLoops(2, LoopType.Yoyo);
                    rows[rowindex].tiles[colindex].SetLetter((char)rowKeys[i]);
                    rows[rowindex].tiles[colindex].SetState(occupiedState);
                    colindex++;
                    break;
                }
            }
        }

       
        
    }
    private void SubmitRow(Row row)
    {
        
            if (!IsValidWord(row.word))
        {
            rows[rowindex].transform.DOShakeScale(0.1f, 0.1f, 10, 90, false);
            textMesh.text = "Incorrect Word";
            textMesh.gameObject.SetActive(true);

            textMesh.transform.DOShakePosition(0.5f, 10, 90, 90, false, true);


            return;
        }
   

        string remainingWord = word;
        for (int i = 0; i < row.tiles.Length; i++)
        {
            if (row.tiles[i].letter == word[i])
            {
                row.tiles[i].SetState(correctState);
                remainingWord = remainingWord.Remove(i, 1);
                remainingWord = remainingWord.Insert(i, " ");
            }
            else if (!word.Contains(row.tiles[i].letter))
            {
              
                row.tiles[i].SetState(incorrectState);
                
            }
        }
        for (int i = 0; i < row.tiles.Length; i++)
        {
            if (row.tiles[i].state != correctState && row.tiles[i].state != incorrectState)
            {
                if (remainingWord.Contains(row.tiles[i].letter))
                {
                    row.tiles[i].SetState(wrongSpotState);
                    int index = remainingWord.IndexOf(row.tiles[i].letter);
                    remainingWord = remainingWord.Remove(index, 1);
                    remainingWord = remainingWord.Insert(index, " ");
                }
                else
                {
                    row.tiles[i].SetState(incorrectState);
                    remainingWord = remainingWord.Remove(i, 1);
                    remainingWord = remainingWord.Insert(i, " ");
                }
            }
        }
        if (HasWon(row)) {
            
            textMesh.gameObject.SetActive(true);
            textMesh.text = "You WIN";
            Theword.text = " ";
            enabled = false; }

        rowindex++;
        colindex = 0;

        if (rowindex >= rows.Length)
        {
            enabled = false;
            Theword.text = "Correct word '"+word+"'";
        }
    }

    public void nothing(Row row)
    {
        for (int i = 0; i < row.tiles.Length; i++)
        {
            Tile tile = row.tiles[i];
            if (tile.letter == word[i])
            {
                tile.SetState(correctState);
            }
            else if (word.Contains(tile.letter))
            {
                tile.SetState(wrongSpotState);
            }
            else
            {
                tile.SetState(incorrectState);
            }
        }
    }

    private bool IsValidWord(string word)
    {
        for (int i = 0; i < validWords.Length; i++)
        {
            if (word == validWords[i])
            {
                return true;
            }
        }
        return false;
    }
 
    private bool HasWon(Row row)
    {
        for (int i = 0; i < row.tiles.Length; i++)
        {
            if (row.tiles[i].state != correctState)
            {
                return false;
            }
        }
        return true;
    }

    private void OnEnable()
    {
        PlayagainButton.gameObject.SetActive(false);
        TryAgainButton.gameObject.SetActive(false);
        Theword.gameObject.SetActive(false);
     

    }
    private void OnDisable()
    {
        PlayagainButton.gameObject.SetActive(true);
        TryAgainButton.gameObject.SetActive(true);
        Theword.gameObject.SetActive(true);
    }

    private void ClearBoard()
    {
        for (int i = 0; i < rows.Length; i++)
        {
           for (int j = 0; j < rows[i].tiles.Length; j++)
            {
                rows[i].tiles[j].SetLetter('\0');
                rows[i].tiles[j].SetState(emptyState);
            }
        }

        rowindex = 0;
        colindex = 0;
    }
}

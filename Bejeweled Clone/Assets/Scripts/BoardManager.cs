using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public GameObject jewelPreFab;
    public int height = 8; // Board Height
    public int width = 8; // Board Width
    public GameObject[,] board; // Board Matrix to keep all the jewels
    public bool canMove = true; // Boolean to stop possible inputs from the player
                                // during checks and that of the pieces

    public Sprite[] textures = new Sprite[5]; // Aray to keep all the jewel textures
    private string[] colors = new string[5] {"Green", "Red", "Blue", "Yellow", "Pink"}; // Array to store color strings that will be used as tags 

    private int JTRAmount = 0; // Amount of jewels to removed
    private GameObject[] jewelToRemove; // Array to keep the jewels that need to be destroyed

    private int score;
    private int scoreLimit = 1000;
    private int combo;
    private int bestCombo;
    public CanvasManager canvasManager;    

    void Start()
    {
        jewelToRemove = new GameObject[height*width]; // Starting the array with the maximum number of jewels that can be destroyed
        board = new GameObject[height,width]; // Startind the board matrix
        FillUp();
    }

    void FillUp(){ // Fill the board only once at the beginning
        for(int i = 0; i < height; i++){
            for(int j = 0; j < width; j++){
                int r = Random.Range(0,5); // Create a random number to set the jewel color (textures e colors array sinergy)
                while(HasNeighbor(colors[r], i, j)){ // Check for possible matches during the first fill
                    r = Random.Range(0,5); // If a match is found, create a new random number to keep a no matches first board
                }
                board[i,j] = NewJewel(r,i,j); // Put a jewel in the matrix
            }
        }
    }

    private GameObject NewJewel(int r, int i, int j){ // Create and return a new jewel with a | r | color, in i and j positions
        GameObject m = (GameObject)Instantiate(jewelPreFab, new Vector2(j,i+1), Quaternion.identity); // Instantiate a new one
        m.GetComponent<Jewel>().SetData(colors[r], j, i+1); // Set the data of that jewel (i+1) Causes the jewelry to start in a position
                                                            // higher than it should to create the fall impression
        m.GetComponent<SpriteRenderer>().sprite = textures[r];
        m.transform.parent = transform;
        m.name = "( "+i+", "+j+" )";
        return m;
    }

    private bool HasNeighbor(string colorTag, int i, int j){ // Verify possibles matches during the first fill
        if(i > 1 && j > 1){
            if(board[i-1, j].tag == colorTag && board[i-2, j].tag == colorTag){
                return true;
            }
            if(board[i, j-1].tag == colorTag && board[i, j-2].tag == colorTag){
                return true;
            }
        }else if(i <= 1 || j <= 1){
            if(i > 1){
                if(board[i-1, j].tag == colorTag && board[i-2,j].tag == colorTag){
                    return true;
                }
            }else if(j > 1){
                if(board[i, j-1].tag == colorTag && board[i,j-2].tag == colorTag){
                    return true;
                }   
            }
        }
        return false;
    }

    public void FindMatchesWith(GameObject J1 = null, GameObject J2 = null){ // Find matches after a swap and the fall
        for(int i = 0; i < height; i++){
            for(int j = 0; j < width-2; j++){ // Check Horizontally first
                string JTAG = board[i,j].tag; // [i,j] j in the columns -> horizontal for
                int indexJ = 0; // Counts how many jewel were found with the same tag
                while(j+indexJ < width && board[i,j+indexJ].tag == JTAG){ // While finding jewels of the same color, count 
                    indexJ += 1;
                }
                if(indexJ >= 3){ // If 3 or more are found
                    for(int k = 0; k < indexJ; k++){ // Start a for with the amount of jewels found
                        ToRemove(board[i,j+k]); // Send to be removed
                    }
                    combo += 20 + ((indexJ-3)*20); // Score function
                    j += indexJ; // Place the j one position in front of the last jewel found, using the indexJ count 
                }
            }
            for(int j = 0; j < height-2; j++){ // Check Vertically
                string JTAG = board[j,i].tag; // [j,i] j in the columns -> vertical for
                int indexI = 0; // Counts how many jewel were found with the same tag
                while(j+indexI < height && board[j+indexI, i].tag == JTAG){
                    indexI += 1;
                }
                if(indexI >= 3){
                    for(int k = 0; k < indexI; k++){
                        ToRemove(board[j+k, i]);
                    }
                    combo += 20 + ((indexI-3)*20);
                    j += indexI;
                }
            }
        }
        if(JTRAmount > 0){ // If there are jewels to be removed, they have been found
            score += combo;
            if(combo > bestCombo){
                bestCombo = combo;
            }
            canvasManager.setInfo(score, combo, bestCombo);
            RemoveMatches(); // Removes all matching jewels
            StartCoroutine(WaitFor("FILL"));
        }else if(J1 != null){ // If a jewel was passed and no match was found
            J1.GetComponent<Jewel>().destiny = J1.GetComponent<Jewel>().origin; // Make the jewel return to the orign position (column and row before the swap)
            J2.GetComponent<Jewel>().destiny = J2.GetComponent<Jewel>().origin;
            StartCoroutine(WaitFor("MOVE"));
        }else{ // If no match was found release the movements (Player Inputs) again
            combo = 0;
            if(score > scoreLimit){
                StartCoroutine(WaitFor("BONUS"));
                scoreLimit += 1000;
            }else{
                canMove = true;
            }
        }
    }

    private void ToRemove(GameObject jewel){ // Store the jewels that need to be destroyed in an array
        for(int i = 0; i < JTRAmount; i++){
            if(jewelToRemove[i] == jewel){ // If that jewel is already inside of the array, return
                return;
            }
        }
        jewelToRemove[JTRAmount] = jewel; // If not, store the jewel
        JTRAmount += 1; // And increase JTRAmount (Used in FindMatchesWith to verify if have jewel to be removed)
    }

    private void RemoveMatches(){ // Remove/Destroy the jewels
        FindObjectOfType<AudioManager>().Play("JewelDisappear"); // Sfx
        for(int i = 0; i < JTRAmount; i++){
            int c = jewelToRemove[i].GetComponent<Jewel>().column; // Get the row and the column of that jewel
            int r = jewelToRemove[i].GetComponent<Jewel>().row;
            jewelToRemove[i].GetComponent<Jewel>().anim.SetTrigger("wasDestroyed");
            board[r,c] = null; // Set null in the board
        }
        JTRAmount = 0; // Reset JTRAmount -> next time when ToRemove is called, the array will start to be filled in [0]
    }

    public void ReFill(){ // ReFill the board every time matches are removed
        for(int j = 0; j < width; j++){
            for(int i = 0; i < height; i++){
                if(board[i,j] == null){ // If a empty cell was found
                    int k = i;
                    while(k < height){ // Seach for a jewel above
                        if(board[k,j] != null){ // If a jewel was found
                            board[k,j].GetComponent<Jewel>().destiny = new Vector2(j,i); // Set the destiny to the empty cell position
                            board[k,j] = null; // And make that cell empty
                            break; // Stop the loop
                        }
                        if(board[k,j] == null && k == height-1){ // If no jewel was found and k are in the last position
                            int r = Random.Range(0,5);
                            board[k,j] = NewJewel(r,k,j); // Create a new jewel there
                            k = i; // Reset k, to make a new seach in the same column
                        }
                        k++;
                    }
                }
            }
        }
        StartCoroutine(WaitFor("FALL"));
    }

    public void Bonus(){
        int ri = Random.Range(0,height);
        int rj = Random.Range(0,width);
        for(int i = 0; i < height; i++){
            ToRemove(board[i,rj]);
        }
        for(int j = 0; j < width; j++){
            ToRemove(board[ri,j]);
        }
        int amount = (20+(width - 3)*20)+(20+(height - 3)*20);
        scoreLimit += amount;
        score += amount;
        canvasManager.setInfo(score, 0, bestCombo);
        RemoveMatches();
        StartCoroutine(WaitFor("FILL"));
    }

    IEnumerator WaitFor(string act){
        yield return new WaitForSeconds(0.3f);
        if(act == "FALL"){
            FindMatchesWith();
        }else if(act == "MOVE"){
            canMove = true;
        }else if(act == "FILL"){
            ReFill();
        }else if(act == "BONUS"){
            Bonus();
        }
    }
}
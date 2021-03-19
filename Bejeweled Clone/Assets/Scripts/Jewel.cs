using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Jewel : MonoBehaviour
{
    private Vector2 firstTouchPos;
    private Vector2 finalTouchPos;
    private BoardManager BM; // Board reference

    [HideInInspector]
    public int column;
    public int row;
    public Vector2 origin; // Position before the swap
    public Vector2 destiny; // Position the jewel needs to go
    
    public GameObject Border; // Sprite used as jewelry selection border
    public Animator anim;

    public void SetData(string TAG, int COLUMN, int ROW){
        tag = TAG;
        column = COLUMN; // REMEMBER -> More columns = more width, so columns = x
        row = ROW; // REMEMBER -> More rows = more height, so rows = y
        destiny = new Vector2(column, row-1); // Set the destiny one position bellow (The jewel start one position above to give the impression of fall)
        BM = FindObjectOfType<BoardManager>();
    }

    void Update()
    {
        if(destiny != new Vector2(column, row)){ // Everty time a new destiny is seted -> move to there
            MoveJewel();
        }
    }

    private void MoveJewel(){
        if(Vector2.Distance(transform.position, destiny) > 0.2f){
            transform.position = Vector2.Lerp(transform.position, destiny, 0.3f);
        }else{
            transform.position = destiny;
            origin = new Vector2(column, row); // Set origin with the old column and row
            row = (int)destiny.y; // Set new values
            column = (int)destiny.x;
            BM.board[row, column] = gameObject; // Se the jewel in the board
        }
    }

    private void OnMouseDown(){
        if(!CanvasManager.isPaused){
            Border.SetActive(true);
            firstTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    private void OnMouseUp(){
        if(!CanvasManager.isPaused){
            Border.SetActive(false);
            finalTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if(Vector2.Distance(firstTouchPos, finalTouchPos) > 0.6f && BM.canMove){ // Checks the distance to make sure it wasn’t just a touch
                FindObjectOfType<AudioManager>().Play("JewelSlide"); // Sfx
                Vector2 dir = GetDir(AngleBetween(firstTouchPos, finalTouchPos)); // Get the direction of the "drag" -> N S L O
                Vector2 target = new Vector2(column + dir.x, row + dir.y); // Define a target cell
                if((target.x >= 0 && target.x < BM.width) && ((target.y >= 0 && target.y < BM.height))){ // If this target is inside of the board limits
                    BM.board[(int)target.y,(int)target.x].GetComponent<Jewel>().destiny = new Vector2(column, row); // Set the jewel destiny of that target to my cell
                    destiny = target; // Set my destiny to the target cell
                    BM.canMove = false; // Block the movements (Player Inputs)
                    StartCoroutine(WaitForTheMovesOf(gameObject, BM.board[(int)target.y,(int)target.x]));
                }        
            }
        }
    }

    IEnumerator WaitForTheMovesOf(GameObject J1, GameObject J2){ // Wait x seconds to find matches after the jewels swap
        yield return new WaitForSeconds(0.3f);
        BM.FindMatchesWith(J1, J2);
    }
    
    private float AngleBetween(Vector2 P1, Vector2 P2){
        return Mathf.Atan2(P2.y - P1.y, P2.x - P1.x)*(180/Mathf.PI);
    }

    private Vector2 GetDir(float angle){ // Return an unit vector to be used as the target direction 
        if(angle >= 45 && angle < 135){
            return new Vector2(0,1);
        }else if(angle >= 135 || angle < -135){
            return new Vector2(-1,0);
        }else if(angle >= -135 && angle < -45){
            return new Vector2(0,-1);
        }else if(angle >= -45 && angle < 45){
            return new Vector2(1,0);
        }
        return new Vector2(0,0);
    }

    public void Disappear(){
        Destroy(gameObject);
    }

}


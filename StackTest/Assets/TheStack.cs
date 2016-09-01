using UnityEngine;
using System.Collections;

public class TheStack : MonoBehaviour {
    private GameObject[] theStack;
    private const float BOUNDS_SIZE = 3.5f;
    private const float ERROR_MARGIN = 1f;
    private const float STACK_MOVING_SPEED = 2.5f;
    private const float STACKBOUNDS_GAIN = 0.25f;
    private const int COMBO_START_GAIN = 3;
    private int scoreCount = 0;
    private int stackIndex; // we start the stack at the bottom
    private Vector2 stackBounds = new Vector2(BOUNDS_SIZE, BOUNDS_SIZE);
    private bool isMovingonX = true;

    private float tileSpeed = 2.5f, tileTransition = 0.0f;
    private float secondaryPosition; // not sure what this is for...

    private Vector3 desiredPosition; // where the stack should be
    private Vector3 lastTilePosition;
    private bool gameOver = false;
    private int combo = 0;
	// Use this for initialization
	void Start () {
        // our empty GameObject called TheStack
        // contains 12 child cubes.  Each cube is 3.5x, 1y,3.5z
        theStack = new GameObject[transform.childCount];
        for(int i=0;i<transform.childCount;i++)
        {
            theStack[i] = transform.GetChild(i).gameObject;
        }
        // we start the stack at the bottom
        stackIndex = transform.childCount-1;
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            
            if (PlaceTile()) 
            {

                SpawnTile();
                scoreCount++;
            } else
            {
                EndGame();
            }
        }
        MoveTile();
        transform.position = Vector3.Lerp(transform.position, desiredPosition, STACK_MOVING_SPEED * Time.deltaTime);

    }

    private void MoveTile()
    {
        if (gameOver) return;

        tileTransition += Time.deltaTime * tileSpeed;
        if(isMovingonX)
            theStack[stackIndex].transform.localPosition = 
                    new Vector3(Mathf.Sin(tileTransition) * BOUNDS_SIZE,
                                scoreCount, secondaryPosition);
        else
            theStack[stackIndex].transform.localPosition =
                    new Vector3(secondaryPosition, scoreCount,Mathf.Sin(tileTransition) * BOUNDS_SIZE);

    }
    private void EndGame()
    {
        gameOver = true;
        Debug.Log("Lose");
        theStack[stackIndex].gameObject.AddComponent<Rigidbody>();
    }
    private void SpawnTile()
    {
        lastTilePosition = theStack[stackIndex].transform.localPosition;
        stackIndex--;
        if (stackIndex < 0)
            stackIndex = transform.childCount - 1;
        desiredPosition = Vector3.down * scoreCount;
        theStack[stackIndex].transform.localPosition = new Vector3(0, scoreCount, 0);
        theStack[stackIndex].transform.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);

    }
    /// <summary>
    /// Places a tile on the stack. 
    /// </summary>
    /// <returns>true or false if the tile could be placed</returns>
    private bool PlaceTile()
    {
        
        Transform t = theStack[stackIndex].transform;
        if(isMovingonX)
        {
            float deltaX = lastTilePosition.x - t.position.x;
            if(Mathf.Abs(deltaX)> ERROR_MARGIN)
            {
                //CUT THE TILE
                combo = 0; //???
                // resize the current tile accordingly
                stackBounds.x -= Mathf.Abs(deltaX);
                if (stackBounds.x <= 0) return false;
                float middle = lastTilePosition.x + t.localPosition.x / 2;
                t.localScale = new Vector3(stackBounds.x, 1,stackBounds.y);
                t.localPosition = new Vector3(lastTilePosition.x, scoreCount, middle - (lastTilePosition.z / 2));
            }else
            {
                // combo is when you stack correct X times in a row, the stack grows
                // the int combo is # of pieces you need to combo in a row to get the growth
                if (combo > COMBO_START_GAIN)
                {
                    stackBounds.x += STACKBOUNDS_GAIN;
                    stackBounds.y += STACKBOUNDS_GAIN;
                    float middle = lastTilePosition.z + t.localPosition.z / 2;
                    t.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);
                    t.localPosition = new Vector3(lastTilePosition.x, scoreCount, middle - (lastTilePosition.z / 2));
                    combo = 0;

                }
                else
                {
                    combo++;
                    t.localPosition = new Vector3(lastTilePosition.x, scoreCount, lastTilePosition.z);
                }

            }
        } else
        {
            float deltaZ = lastTilePosition.z - t.position.z;
            if (Mathf.Abs(deltaZ) > ERROR_MARGIN)
            {
                //CUT THE TILE
                combo = 0; //???
                // resize the current tile accordingly
                stackBounds.y -= Mathf.Abs(deltaZ);
                if (stackBounds.y <= 0) return false;
                float middle = lastTilePosition.z + t.localPosition.z / 2;
                t.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);
                t.localPosition = new Vector3(lastTilePosition.x, scoreCount, middle - (lastTilePosition.z / 2));
            }
            else
            {
                if (combo > COMBO_START_GAIN)
                {
                    stackBounds.y += STACKBOUNDS_GAIN;
                    stackBounds.x += STACKBOUNDS_GAIN;
                    float middle = lastTilePosition.z + t.localPosition.z / 2;
                    t.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);
                    t.localPosition = new Vector3(lastTilePosition.x, scoreCount, middle - (lastTilePosition.z / 2));
                    combo = 0;
                }
                else
                {
                    combo++;
                    t.localPosition = new Vector3(lastTilePosition.x, scoreCount, lastTilePosition.z);
                }
            }

        }

        // secondary position keeps the traveling tile in line with the stack
        // along the axis that it is traveling on
        secondaryPosition = (isMovingonX) ? t.localPosition.x : t.localPosition.z;
        isMovingonX = !isMovingonX;
        return true;
    }
}

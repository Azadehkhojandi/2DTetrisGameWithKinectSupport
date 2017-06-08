using Assets.Scripts;
using UnityEngine;

public class Group : MonoBehaviour
{

    private bool _isPreview;
    private float _lastFall = 0; // Time since last gravity tick
    private float _lastRotate = 0;// Time since last rotate
    private float _lastMoveRight = 0;// Time since last move right
    private float _lastMoveLeft = 0;// Time since last move left
    private float _lastRotateDelay = 0.3f;
    private float _lastMoveDelay = 0.3f;



    // Use this for initialization
    void Start()
    {

        // Default position not valid? Then it's game over
        if (!IsValidGridPos())
        {
            Debug.Log("GAME OVER");
            FindObjectOfType<GameOver>().PopUp();
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GameOver.IsGameover || _isPreview)
        {
            return;
        }

        // Move Left
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MoveLeft();
        }

        // Move Right
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            MoveRight();
        }

        // Rotate
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Rotate();
        }

        // Move Downwards and Fall
        else if (Input.GetKeyDown(KeyCode.DownArrow) ||
                 Time.time - _lastFall >= 1)
        {
            MoveDownwards();
        }
    }

    public void MoveDownwards()
    {
        // Modify position
        transform.position += new Vector3(0, -1, 0);

        // See if valid
        if (IsValidGridPos())
        {
            // It's valid. Update BlocksGrid.
            UpdateGrid();
        }
        else
        {
            // It's not valid. revert.
            transform.position += new Vector3(0, 1, 0);

            // Clear filled horizontal lines
            Grid.DeleteFullRows();

            // Spawn next Group
            FindObjectOfType<Spawner>().SpawnPreviewShape();

            // Disable script
            enabled = false;
        }

        _lastFall = Time.time;
    }

    public void Rotate(bool addDelay = false)
    {
        if (addDelay)
        {
            if (Time.time - _lastRotate <= _lastRotateDelay)
            {
                return;
            }
        }
        transform.Rotate(0, 0, -90);

        // See if valid
        if (IsValidGridPos())
        {
            // It's valid. Update BlocksGrid.
            UpdateGrid();
        }
        else
        {
            // It's not valid. revert.
            transform.Rotate(0, 0, 90);
        }
        _lastRotate = Time.time;


    }

    public void MoveRight(bool addDelay = false)
    {
        if (addDelay)
        {
            if (Time.time - _lastMoveRight <= _lastMoveDelay)
            {
                return;
            }
        }
        // Modify position
        transform.position += new Vector3(1, 0, 0);

        // See if valid
        if (IsValidGridPos())
        {
            // It's valid. Update BlocksGrid.
            UpdateGrid();
        }
        else
        {
            // It's not valid. revert.
            transform.position += new Vector3(-1, 0, 0);
        }
        _lastMoveRight = Time.time;
    }

    public void MoveLeft(bool addDelay = false)
    {
        if (addDelay)
        {
            if (Time.time - _lastMoveLeft <= _lastMoveDelay)
            {
                return;
            }
        }
        // Modify position
        transform.position += new Vector3(-1, 0, 0);

        // See if valid
        if (IsValidGridPos())
        {
            // It's valid. Update BlocksGrid.
            UpdateGrid();
        }
        else
        {
            // It's not valid. revert.
            transform.position += new Vector3(1, 0, 0);
        }
        _lastMoveLeft = Time.time;
    }

    bool IsValidGridPos()
    {
        //check to make sure it's not preview
        if (transform.position.x == 15f && transform.position.y == 10f)
        {
            _isPreview = true;
            return true;
        }

        foreach (Transform child in transform)
        {
            Vector2 v = Grid.RoundVec2(child.position);

            // Not inside Border?
            if (!Grid.InsideBorder(v))
                return false;

            // Block in BlocksGrid cell (and not part of same group)?
            if (Grid.BlocksGrid[(int)v.x, (int)v.y] != null &&
                Grid.BlocksGrid[(int)v.x, (int)v.y].parent != transform)
                return false;
        }
        return true;
    }
    void UpdateGrid()
    {
        // Remove old children from BlocksGrid
        for (var y = 0; y < Grid.Height; ++y)
        {
            for (var x = 0; x < Grid.Width; ++x)
            {
                if (Grid.BlocksGrid[x, y] != null)
                {
                    if (Grid.BlocksGrid[x, y].parent == transform)
                    {
                        Grid.BlocksGrid[x, y] = null;
                    }
                }
            }
        }

        // Add new children to BlocksGrid
        foreach (Transform child in transform)
        {
            Vector2 v = Grid.RoundVec2(child.position);
            Grid.BlocksGrid[(int)v.x, (int)v.y] = child;
        }
    }

    public bool IsPreviewGroup()
    {
        return _isPreview;
    }

}

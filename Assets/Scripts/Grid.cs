using UnityEngine;

namespace Assets.Scripts
{
    public class Grid : MonoBehaviour {

        // The Grid itself
        public static int Width = 10;
        public static int Height = 20;
        public static Transform[,] BlocksGrid = new Transform[Width, Height];

        // Use this for initialization
        void Start () {
		
        }
	
        // Update is called once per frame
        void Update () {
		
        }

        public static Vector2 RoundVec2(Vector2 v)
        {
            return new Vector2(Mathf.Round(v.x),
                               Mathf.Round(v.y));
        }
        public static bool InsideBorder(Vector2 pos)
        {
            return ((int)pos.x >= 0 &&
                    (int)pos.x < Width &&
                    (int)pos.y >= 0);
        }

        public static void DeleteRow(int y)
        {
            for (int x = 0; x < Width; ++x)
            {
                Destroy(BlocksGrid[x, y].gameObject);
                BlocksGrid[x, y] = null;
            }
        }

        public static void DecreaseRow(int y)
        {
            for (int x = 0; x < Width; ++x)
            {
                if (BlocksGrid[x, y] != null)
                {
                    // Move one towards bottom
                    BlocksGrid[x, y - 1] = BlocksGrid[x, y];
                    BlocksGrid[x, y] = null;

                    // Update Block position
                    BlocksGrid[x, y - 1].position += new Vector3(0, -1, 0);
                }
            }
        }

        public static void DecreaseRowsAbove(int y)
        {
            for (int i = y; i < Height; ++i)
                DecreaseRow(i);
        }
        public static bool IsRowFull(int y)
        {
            for (int x = 0; x < Width; ++x)
            {
                if (BlocksGrid[x, y] == null)
                {
                    return false;
                }
            }
           
            return true;
        }
        public static void DeleteFullRows()
        {
            for (int y = 0; y < Height; ++y)
            {
                if (IsRowFull(y))
                {
                    DeleteRow(y);
                    //add full row score
                    FindObjectOfType<ScoreManager>().AddFullrowScore();
                    DecreaseRowsAbove(y + 1);
                    --y;
                }
            }
        }

    }
}

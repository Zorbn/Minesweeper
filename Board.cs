using Raylib_CsLo;

public class Board
{
    public enum BoardStatus
    {
        PLAYING,
        LOST,
        WON
    }

    private enum TileStatus
    {
        HIDDEN,
        REVEALED,
        FLAGGED
    }

    private static Color StatusToColor(TileStatus status) => status switch
    {
        TileStatus.HIDDEN   => Raylib.GRAY,
        TileStatus.REVEALED => Raylib.LIGHTGRAY,
        TileStatus.FLAGGED  => Raylib.RED,
        _ => throw new ArgumentOutOfRangeException()
    };

    private int[,] grid;
    private TileStatus[,] status;
    private readonly int tileSize;
    private readonly int gap;

    public Board(int size, int tileSize, int gap)
    {
        this.tileSize = tileSize;
        this.gap = gap;

        grid = new int[size, size];
        status = new TileStatus[size, size];
    }

    public void Init()
    {
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                if (Raylib.GetRandomValue(1, 10) == 1)
                {
                    grid[x, y] = -1;
                }
            }
        }

        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                if (grid[x, y] == -1) continue;

                int adjBombs = 0;

                for (int i = 0; i < 9; i++)
                {
                    int rx = x + i / 3 - 1;
                    int ry = y + i % 3 - 1;

                    if (rx < 0 || rx >= grid.GetLength(0) || ry < 0 || ry >= grid.GetLength(1)) continue;
                    if (rx == 0 && ry == 0) continue;
                    if (grid[rx, ry] == -1) adjBombs++;
                }

                grid[x, y] = adjBombs;
            }
        }
    }

    public BoardStatus Update()
    { 
        int mouseX = Raylib.GetMouseX();
        int mouseY = Raylib.GetMouseY();

        int tileX = mouseX / (tileSize + gap);
        int tileY = mouseY / (tileSize + gap);

        if (Raylib.IsMouseButtonPressed(0))
        {

            if (tileX < grid.GetLength(0) && tileY < grid.GetLength(1))
            {
                if (grid[tileX, tileY] == -1)
                {
                    // Die
                    RevealAllBombs();

                    return BoardStatus.LOST;
                }
                else
                {
                    RevealTile(tileX, tileY);
                }
            }
        }

        if (Raylib.IsMouseButtonPressed(1))
        {
            if (status[tileX, tileY] == TileStatus.HIDDEN)  status[tileX, tileY] = TileStatus.FLAGGED;
            else if (status[tileX, tileY] == TileStatus.FLAGGED) status[tileX, tileY] = TileStatus.HIDDEN;
        }

        if (ValidateGrid()) return BoardStatus.WON;
        
        return BoardStatus.PLAYING;
    }

    public void Draw()
    {
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                int worldX = x * (tileSize + gap);
                int worldY = y * (tileSize + gap);
                int textOffset = tileSize / 4;

                Color tileCol = StatusToColor(status[x, y]);

                Raylib.DrawRectangle(worldX, worldY, tileSize, tileSize, tileCol);
                if (status[x, y] == TileStatus.REVEALED) Raylib.DrawText($"{grid[x, y]}", worldX + textOffset, worldY + textOffset, 18, Raylib.BLACK);
            }
        }
    }

    private void RevealTile(int tileX, int tileY)
    {
        status[tileX, tileY] = TileStatus.REVEALED;

        if (grid[tileX, tileY] == 0)
        {
            for (int i = 0; i < 9; i++)
            {
                int x = tileX + i / 3 - 1;
                int y = tileY + i % 3 - 1;

                if (x < 0 || x >= grid.GetLength(0) || y < 0 || y >= grid.GetLength(1)) continue;
                if (status[x, y] == TileStatus.REVEALED) continue;

                RevealTile(x, y);
            }
        }
    }

    private void RevealAllBombs()
    {
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                if (grid[x, y] == -1) status[x, y] = TileStatus.REVEALED;
            }
        }
    }

    private bool ValidateGrid()
    {
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(0); y++)
            {
                if (grid[x, y] != -1 && status[x, y] != TileStatus.REVEALED) return false;
            }
        }
        
        return true;
    }
}

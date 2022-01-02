using Raylib_CsLo;

public static class Program
{
    public static void Main()
    {
        Raylib.InitWindow(1280, 720, "Minesweeper");
        Raylib.SetTargetFPS(165);

        Board.BoardStatus status = Board.BoardStatus.PLAYING;
        const int Size = 16;
        const int TileSize = 32;
        const int Gap = 2;
        Board board = new Board(Size, TileSize, Gap);
        board.Init();

        while (!Raylib.WindowShouldClose())
        {
            string notifText = "";
            Color notifColor = Raylib.WHITE;
            int notifSize = 64;

            if (status == Board.BoardStatus.PLAYING)
            {
                status = board.Update();            }
            else
            {
                notifText = "You lost!";
                notifColor = Raylib.RED;

                if (status == Board.BoardStatus.WON)
                {
                    notifText = "You won!";
                    notifColor = Raylib.GREEN;
                }


                if (Raylib.IsMouseButtonPressed(0))
                {
                    status = Board.BoardStatus.PLAYING;
                    board = new Board(Size, TileSize, Gap);
                    board.Init();
                }
            }

            Raylib.BeginDrawing();
            Raylib.ClearBackground(Raylib.RAYWHITE);

            board.Draw();

            if (notifText != "")
            {
                int textWidth = Raylib.MeasureText(notifText, notifSize);
                Raylib.DrawRectangle(0, 0, textWidth, notifSize, Raylib.BLACK);
                Raylib.DrawText(notifText, 0, 0, notifSize, notifColor);
            }

            Raylib.EndDrawing();
        }

        Raylib.CloseWindow();
    }
}

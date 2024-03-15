using Raylib_cs;
using System.Numerics;

internal class Program
{
    // If you need variables in the Program class (outside functions), you must mark them as static
    static string title = "Game Title";
    //static GamepadDisplay gamepadDisplay0 = new GamepadDisplay(0);
    static List<GamepadDisplay> gamepadDisplays = new List<GamepadDisplay>();

    const int displayGap = 10;
    const int lineGap = 5;
    const int fontSize = 24;

    static void Main(string[] args)
    {
        // Create a window to draw to. The arguments define width and height
        Raylib.InitWindow(800, 600, title);
        // Set the target frames-per-second (FPS)
        Raylib.SetTargetFPS(60);

        // Setup your game. This is a function YOU define.
        Setup();

        // Loop so long as window should not close
        while (!Raylib.WindowShouldClose())
        {
            // Enable drawing to the canvas (window)
            Raylib.BeginDrawing();
            // Clear the canvas with one color
            Raylib.ClearBackground(Color.RayWhite);

            // Your game code here. This is a function YOU define.
            Update();

            // Stop drawing to the canvas, begin displaying the frame
            Raylib.EndDrawing();
        }
        // Close the window
        Raylib.CloseWindow();
    }

    static void Setup()
    {
        // Your one-time setup code here
    }

    static void Update()
    {
        // Your game code run each frame here
        for (int i = 0; i < 4; i++)
        {
            bool exists = Raylib.IsGamepadAvailable(i);
            if (!exists)
                continue;

            bool isTracked = false;
            foreach (GamepadDisplay display in gamepadDisplays)
            {
                if (display.DeviceID == i)
                {
                    isTracked = true;
                    break;
                }
            }

            if (isTracked)
                continue;

            GamepadDisplay gamepadDisplay = new GamepadDisplay(i);
            gamepadDisplays.Add(gamepadDisplay);
        }

        for (int i = 0; i < gamepadDisplays.Count; i++)
        {
            GamepadDisplay gamepadDisplay = gamepadDisplays[i];

            int stride = Raylib.GetScreenWidth() / gamepadDisplay.AxisCount;
            int x = (stride * i) + (displayGap * (i + 1));

            gamepadDisplay.PollInputsForExistence();
            gamepadDisplay.DisplayExistingInputs(x, displayGap, fontSize, lineGap);
        }
    }
}
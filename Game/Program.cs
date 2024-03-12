using Raylib_cs;
using System.Numerics;

internal class Program
{
    // If you need variables in the Program class (outside functions), you must mark them as static
    static string title = "Game Title";
    static GamepadDisplay x = new GamepadDisplay(0);

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
        //x.DrawDeviceInformation(new(10, 10), 32);
        //x.DrawAxis2(new(200, 200), 50, GamepadAxis.LeftX, GamepadAxis.LeftY);
        //x.DrawButtonDown(new(100, 200), 10, GamepadButton.RightFaceRight);
        //x.Draw8bitdoGamepad(new(200, 200), 32, 50, 10);

        x.PollInputsForExistence();
        x.DisplayExistingInputs(new(10, 10), 24, 5);
    }
}
using Raylib_cs;
using System.Numerics;

public class GamepadDisplay
{
    // "Constant" data
    static readonly GamepadButton[] GamepadButtons = Enum.GetValues<GamepadButton>();
    static readonly GamepadAxis[] GamepadAxes = Enum.GetValues<GamepadAxis>();

    // Private state
    private List<GamepadButton> ExistingButtons = new List<GamepadButton>();
    private List<GamepadAxis> ExistingAxes = new List<GamepadAxis>();

    // Instance state
    public int DeviceID { get; private set; }
    public Color ColorFG { get; set; } = Color.Red;
    public Color ColorBG { get; set; } = Color.Black;
    public string DeviceName => Raylib.GetGamepadName_(DeviceID);
    public int AxisCount => Raylib.GetGamepadAxisCount(DeviceID);


    public GamepadDisplay(int deviceID)
    {
        DeviceID = deviceID;
    }

    public void DrawAxis(Vector2 position, Vector2 size, GamepadAxis axis)
    {
        Rectangle outline = new Rectangle(position, size);
        Rectangle fill = outline;
        float value = Raylib.GetGamepadAxisMovement(DeviceID, axis);
        fill.Width *= MathF.Abs(value) / 2f;

        // Set base coordinate for rect start
        if (value >= 0)
        {
            // centre point
            fill.X = position.X + size.X / 2f;
        }
        else
        {
            // centre point minus rect size
            fill.X = position.X + size.X / 2f * (1 + value);
        }
        
        // Override fill width if no input
        if (value == 0)
        {
            fill.Width = 1; // 1px
        }

        Raylib.DrawRectangleRec(fill, ColorFG);
        Raylib.DrawRectangleLinesEx(outline, 1, ColorBG);
    }

    public void DrawButton(Vector2 position, float radius, GamepadButton button, Func<int, GamepadButton, CBool> function)
    {
        bool doDisplay = function.Invoke(DeviceID, button);
        Color color = doDisplay ? ColorFG : Color.Blank;
        Raylib.DrawCircleV(position, radius, color);
        Raylib.DrawCircleLines((int)position.X, (int)position.Y, radius, ColorBG);
    }

    public void DrawButtonDown(Vector2 position, float radius, GamepadButton button)
        => DrawButton(position, radius, button, Raylib.IsGamepadButtonDown);
    public void DrawButtonUp(Vector2 position, float radius, GamepadButton button)
        => DrawButton(position, radius, button, Raylib.IsGamepadButtonUp);
    public void DrawButtonPressed(Vector2 position, float radius, GamepadButton button)
        => DrawButton(position, radius, button, Raylib.IsGamepadButtonPressed);
    public void DrawButtonReleased(Vector2 position, float radius, GamepadButton button)
        => DrawButton(position, radius, button, Raylib.IsGamepadButtonReleased);

    public void PollInputsForExistence()
    {
        foreach (var button in GamepadButtons)
        {
            bool isDown = Raylib.IsGamepadButtonDown(DeviceID, button);
            if (!isDown)
                continue;
            
            bool tracked = ExistingButtons.Contains(button);
            if (tracked)
                continue;

            ExistingButtons.Add(button);
            ExistingButtons.Sort();
        }

        foreach (var axis in GamepadAxes)
        {
            float value = Raylib.GetGamepadAxisMovement(DeviceID, axis);
            if (MathF.Abs(value) <= 0.001f)
                continue;

            bool tracked = ExistingAxes.Contains(axis);
            if (tracked)
                continue;

            ExistingAxes.Add(axis);
            ExistingAxes.Sort();
        }
    }

    public void DisplayExistingInputs(int X, int Y, int fontSize, int gap)
    {
        // buttons / circles
        float radius = fontSize / 2f;
        Vector2 circleOffset = new Vector2(radius, radius);
        // axes / rects
        int axisDisplayWidth = fontSize * 3;
        Vector2 rectSize = new Vector2(axisDisplayWidth, fontSize);
        // info
        string infoID = $"ID: {DeviceID}";
        string infoAxis = $"Axis Count: {AxisCount}";

        // INFORMATION
        Raylib.DrawText(DeviceName, X, Y, fontSize, ColorBG);
        Y += fontSize + gap;
        Raylib.DrawText(infoID, X, Y, fontSize, ColorBG);
        Y += fontSize + gap;
        Raylib.DrawText(infoAxis, X, Y, fontSize, ColorBG);
        Y += fontSize + gap;

        // BUTTONS
        int textX = X + (int)(radius * 2 + gap);
        foreach (var button in ExistingButtons)
        {
            DrawButtonDown(new Vector2(X, Y) + circleOffset, radius, button);
            Raylib.DrawText(button.ToString(), textX, Y, fontSize, ColorBG);
            Y += fontSize + gap;
        }

        // AXIS
        textX = X + axisDisplayWidth + gap;
        foreach (var axis in ExistingAxes)
        {
            DrawAxis(new Vector2(X, Y), rectSize, axis);
            float axisValue = Raylib.GetGamepadAxisMovement(DeviceID, axis);
            string label = $"{axis} {axisValue:0.00}";
            Raylib.DrawText(label, textX, Y, fontSize, ColorBG);
            Y += fontSize + gap;
        }
    }
}

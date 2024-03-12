using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

public class GamepadDisplay
{
    private List<GamepadButton> ExistingButtons = new List<GamepadButton>();
    private List<GamepadAxis> ExistingAxes = new List<GamepadAxis>();
    public int DeviceID { get; private set; }
    public Color ColorFG { get; set; } = Color.Red;
    public Color ColorBG { get; set; } = Color.Black;
    public string DeviceName => Raylib.GetGamepadName_(DeviceID);
    public int AxisCount => Raylib.GetGamepadAxisCount(DeviceID);

    static readonly GamepadButton[] GamepadButtons = Enum.GetValues<GamepadButton>();
    static readonly GamepadAxis[] GamepadAxes = Enum.GetValues<GamepadAxis>();

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
            // centre point 
            fill.X = position.X + size.X / 2f * (1 + value);
        }
        
        // Override fill width if no input
        if (value == 0)
        {
            fill.Width = 1;
        }

        Raylib.DrawRectangleRec(fill, ColorFG);
        Raylib.DrawRectangleLinesEx(outline, 1, ColorBG);
    }

    public void DrawAxis2(Vector2 position, float radius, GamepadAxis x, GamepadAxis y)
    {
        float xAxis = Raylib.GetGamepadAxisMovement(DeviceID, x);
        float yAxis = Raylib.GetGamepadAxisMovement(DeviceID, y);
        Vector2 axis2 = new Vector2(xAxis, yAxis);

        if (axis2.Length() > 1)
            axis2 = Vector2.Normalize(axis2);

        Vector2 axisPosition = axis2 * radius + position;

        Raylib.DrawCircleLines((int)position.X, (int)position.Y, radius, ColorBG);
        Raylib.DrawCircleV(axisPosition, radius / 10f, ColorFG);
        Raylib.DrawLineV(position, axisPosition, ColorBG);
    }

    public void DrawDeviceInformation(Vector2 position, int fontSize)
    {
        int x = (int)position.X;
        int y = (int)position.Y;
        string info = $"ID{DeviceID:00}: {DeviceName}";
        Raylib.DrawText(info, x, y, fontSize, ColorFG);
    }
    public void DrawDeviceInformation2(Vector2 position, int fontSize)
    {
        int x = (int)position.X;
        int y = (int)position.Y;
        string info = $"ID{DeviceID:00}. Axis count: {AxisCount}";
        Raylib.DrawText(DeviceName, x, y, fontSize, ColorFG);
        Raylib.DrawText(info, x, y + fontSize, fontSize, ColorFG);
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

    public void Draw8bitdoGamepad(Vector2 position, int fontSize, float radius, int pad)
    {
        int px = (int)position.X;
        int py = (int)position.Y;
        int w = 200;
        int h = 200;

        // Draw rectangle for bounds
        int totalW = w + pad * 2;
        int totalH = h + pad * 2;
        Raylib.DrawRectangleLines(px, py, totalW, totalH, ColorFG);

        // Draw name + info
        Vector2 textPos = position + new Vector2(pad, pad);
        DrawDeviceInformation(textPos, fontSize);

        //
        Vector2 axisPosition = position + new Vector2(100, 100);
        DrawAxis2(axisPosition, 50, GamepadAxis.LeftX, GamepadAxis.LeftY);

    }

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
        }
    }

    public void DisplayExistingInputs(Vector2 position, int fontSize, int gap)
    {
        // circles
        float radius = fontSize / 2f;
        Vector2 circleOffset = new Vector2(radius, radius);
        // text
        int textX = (int)position.X + (int)(radius * 2 + gap);
        int axisWidthFactor = 3;
        // rect
        Vector2 rectSize = new Vector2(fontSize * axisWidthFactor, fontSize);
        //
        string infoID = $"ID: {DeviceID}";
        string infoAxis = $"Axis Count: {AxisCount}";

        // INFORMATION
        Raylib.DrawText(DeviceName, (int)position.X, (int)position.Y, fontSize, ColorBG);
        position.Y += fontSize + gap;
        Raylib.DrawText(infoID, (int)position.X, (int)position.Y, fontSize, ColorBG);
        position.Y += fontSize + gap;
        Raylib.DrawText(infoAxis, (int)position.X, (int)position.Y, fontSize, ColorBG);
        position.Y += fontSize + gap;

        // BUTTONS
        foreach (var button in ExistingButtons)
        {
            int textY = (int)position.Y;
            DrawButtonDown(position + circleOffset, radius, button);
            Raylib.DrawText(button.ToString(), textX, textY, fontSize, ColorBG);
            position.Y += fontSize + gap;
        }

        // AXIS
        textX = (int)position.X + (fontSize * axisWidthFactor + gap);
        foreach (var axis in ExistingAxes)
        {
            int textY = (int)position.Y;
            DrawAxis(position, rectSize, axis);
            Raylib.DrawText(axis.ToString(), textX, textY, fontSize, ColorBG);
            position.Y += fontSize + gap;
        }
    }
}

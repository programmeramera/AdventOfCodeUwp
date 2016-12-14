using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Input;

namespace AdventOfCodeUwp
{
    public struct Room
    {
        public int X;
        public int Y;
    }

    public sealed partial class Day13
    {
        RadialController myController;
        int steps = 0;
        List<Room> visited = new List<Room>();
        int[,] maze;
        int seed;
        int width;
        int height;

        public Day13()
        {
            this.InitializeComponent();

            myController = RadialController.CreateForCurrentView();
            var icon = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/StoreLogo.png"));
            var myItem = RadialControllerMenuItem.CreateFromIcon("Day13", icon);
            myController.Menu.Items.Add(myItem);
            myController.RotationChanged += MyController_RotationChanged;

            InitMaze(1352, 40, 40);
        }

        void InitMaze(int seed, int width, int height)
        {
            this.seed = seed;
            this.width = width;
            this.height = height;

            maze = new int[width, height];

            ClearMaze();
        }

        void ClearMaze()
        {
            visited.Clear();
            for (int h = 0; h < height; h++) {
                for (int w = 0; w < width; w++) {
                    maze[w, h] = 0;
                }
            }
        }

        void Move(int x, int y, int currentLength, int maxLength)
        {
            if (IsWall(x, y) || IsOutsideBoundary(x, y) || currentLength > maxLength) {
                return;

            } else {
                if (maze[x, y] == 0 || currentLength < maze[x, y]) {

                    var room = new Room { X = x, Y = y };
                    if (!visited.Contains(room)) {
                        visited.Add(room);
                    }

                    maze[x, y] = currentLength;
                    Move(x - 1, y, currentLength + 1, maxLength);
                    Move(x + 1, y, currentLength + 1, maxLength);
                    Move(x, y - 1, currentLength + 1, maxLength);
                    Move(x, y + 1, currentLength + 1, maxLength);
                }
            }
        }

        bool IsOutsideBoundary(int x, int y)
        {
            return x < 0 || y < 0 || x >= this.width || y >= this.height;
        }

        bool IsWall(int x, int y)
        {
            var math = x * x + 3 * x + 2 * x * y + y + y * y;
            var binaryString = Convert.ToString(math + seed, 2);
            return binaryString.Count(digit => digit == '1') % 2 == 1;
        }

        void OnDraw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            float widthFactor = (float)sender.ActualWidth / 40;
            float heightFactor = (float)sender.ActualHeight / 40;
            for (int h = 0; h < this.height; h++) {
                for (int w = 0; w < this.width; w++) {
                    if (IsWall(w, h))
                        args.DrawingSession.FillRectangle(w * widthFactor, h * heightFactor, widthFactor, heightFactor, Colors.Black);
                    else {
                        if (maze[w, h] > 0)
                            args.DrawingSession.FillRectangle(w * widthFactor, h * heightFactor, widthFactor, heightFactor, Colors.Red);
                    }
                }
            }

            args.DrawingSession.DrawText(string.Format("Steps: {0} Rooms: {1}", steps, visited.Count), 10, 10, Colors.White);
        }


        void MyController_RotationChanged(RadialController sender, RadialControllerRotationChangedEventArgs args)
        {
            steps += (int)args.RotationDeltaInDegrees / 10;
            ClearMaze();
            Move(1, 1, 0, steps);
            canvasControl.Invalidate();
        }
    }
}

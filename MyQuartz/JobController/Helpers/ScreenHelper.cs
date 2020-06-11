using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace JobController
{
    public static class ScreenHelper
    {
        // get current taskbar size(width, height), support 4 mode: taskbar bottom/right/up/left
        public static Size GetCurTaskbarSize()
        {
            int width = 0, height = 0;

            if ((Screen.PrimaryScreen.Bounds.Width == Screen.PrimaryScreen.WorkingArea.Width) &&
                (Screen.PrimaryScreen.WorkingArea.Y == 0))
            {
                //taskbar bottom
                width = Screen.PrimaryScreen.WorkingArea.Width;
                height = Screen.PrimaryScreen.Bounds.Height - Screen.PrimaryScreen.WorkingArea.Height;
            }
            else if ((Screen.PrimaryScreen.Bounds.Height == Screen.PrimaryScreen.WorkingArea.Height) &&
                    (Screen.PrimaryScreen.WorkingArea.X == 0))
            {
                //taskbar right
                width = Screen.PrimaryScreen.Bounds.Width - Screen.PrimaryScreen.WorkingArea.Width;
                height = Screen.PrimaryScreen.WorkingArea.Height;
            }
            else if ((Screen.PrimaryScreen.Bounds.Width == Screen.PrimaryScreen.WorkingArea.Width) &&
                    (Screen.PrimaryScreen.WorkingArea.Y > 0))
            {
                //taskbar up
                width = Screen.PrimaryScreen.WorkingArea.Width;
                //height = Screen.PrimaryScreen.WorkingArea.Y;
                height = Screen.PrimaryScreen.Bounds.Height - Screen.PrimaryScreen.WorkingArea.Height;
            }
            else if ((Screen.PrimaryScreen.Bounds.Height == Screen.PrimaryScreen.WorkingArea.Height) &&
                    (Screen.PrimaryScreen.WorkingArea.X > 0))
            {
                //taskbar left
                width = Screen.PrimaryScreen.Bounds.Width - Screen.PrimaryScreen.WorkingArea.Width;
                height = Screen.PrimaryScreen.WorkingArea.Height;
            }

            return new Size(width, height);
        }

        // get current taskbar position(X, Y), support 4 mode: taskbar bottom/right/up/left
        public static Point GetCurTaskbarLocation()
        {
            int xPos = 0, yPos = 0;

            if ((Screen.PrimaryScreen.Bounds.Width == Screen.PrimaryScreen.WorkingArea.Width) &&
                (Screen.PrimaryScreen.WorkingArea.Y == 0))
            {
                //taskbar bottom
                xPos = 0;
                yPos = Screen.PrimaryScreen.WorkingArea.Height;
            }
            else if ((Screen.PrimaryScreen.Bounds.Height == Screen.PrimaryScreen.WorkingArea.Height) &&
                    (Screen.PrimaryScreen.WorkingArea.X == 0))
            {
                //taskbar right
                xPos = Screen.PrimaryScreen.WorkingArea.Width;
                yPos = 0;
            }
            else if ((Screen.PrimaryScreen.Bounds.Width == Screen.PrimaryScreen.WorkingArea.Width) &&
                    (Screen.PrimaryScreen.WorkingArea.Y > 0))
            {
                //taskbar up
                xPos = 0;
                yPos = 0;
            }
            else if ((Screen.PrimaryScreen.Bounds.Height == Screen.PrimaryScreen.WorkingArea.Height) &&
                    (Screen.PrimaryScreen.WorkingArea.X > 0))
            {
                //taskbar left
                xPos = 0;
                yPos = 0;
            }

            return new Point(xPos, yPos);
        }

        // get current right bottom corner position(X, Y), support 4 mode: taskbar bottom/right/up/left
        public static Point GetCornerLocation(Size windowSize) 
        {
            int xPos = 0, yPos = 0;

            if ((Screen.PrimaryScreen.Bounds.Width == Screen.PrimaryScreen.WorkingArea.Width) &&
                (Screen.PrimaryScreen.WorkingArea.Y == 0))
            {
                //taskbar bottom
                xPos = Screen.PrimaryScreen.WorkingArea.Width - windowSize.Width;
                yPos = Screen.PrimaryScreen.WorkingArea.Height - windowSize.Height;
            }
            else if ((Screen.PrimaryScreen.Bounds.Height == Screen.PrimaryScreen.WorkingArea.Height) &&
                    (Screen.PrimaryScreen.WorkingArea.X == 0))
            {
                //taskbar right
                xPos = Screen.PrimaryScreen.WorkingArea.Width - windowSize.Width;
                yPos = Screen.PrimaryScreen.WorkingArea.Height - windowSize.Height;
            }
            else if ((Screen.PrimaryScreen.Bounds.Width == Screen.PrimaryScreen.WorkingArea.Width) &&
                    (Screen.PrimaryScreen.WorkingArea.Y > 0))
            {
                //taskbar up
                xPos = Screen.PrimaryScreen.WorkingArea.Width - windowSize.Width;
                yPos = Screen.PrimaryScreen.WorkingArea.Y;
            }
            else if ((Screen.PrimaryScreen.Bounds.Height == Screen.PrimaryScreen.WorkingArea.Height) &&
                    (Screen.PrimaryScreen.WorkingArea.X > 0))
            {
                //taskbar left
                xPos = Screen.PrimaryScreen.WorkingArea.X;
                yPos = Screen.PrimaryScreen.WorkingArea.Height - windowSize.Height;
            }

            return new Point(xPos, yPos);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace FormFactoryLibrary
{
    public static class ControlFactory
    {
        public static PictureBox GetPictureBox(int x, int y, int width, int height) => GetPictureBox(x, y, width, height, null);
        public static PictureBox GetPictureBox(int x, int y, int width, int height, PaintEventHandler PaintEventHandler) => GetPictureBox(x, y, width, height, PaintEventHandler, null);
        public static PictureBox GetPictureBox(int x, int y, int width, int height, PaintEventHandler PaintEventHandler, object tagObject)
        {
            PictureBox pictureBox = new PictureBox();
            pictureBox.Size = new Size(width, height);
            pictureBox.Location = new Point(x, y);
            if (PaintEventHandler != null) pictureBox.Paint += PaintEventHandler;
            if (tagObject != null) pictureBox.Tag = tagObject;
            return pictureBox;
        }


        public static Button GetButton(int x, int y, int width, int height,string text, EventHandler OnClickEventHandler) => GetButton(x, y, width, height, text, OnClickEventHandler, null);
        public static Button GetButton(int x, int y, int width, int height, string text, EventHandler OnClickEventHandler, object tagObject)
        {
            Button temp = new Button();
            temp.Size = new Size(width, height);
            temp.Location = new Point(x, y);
            temp.Click += OnClickEventHandler;
            temp.Text = text;
            if (tagObject != null) temp.Tag = tagObject;
            return temp;
        }


        public static Label GetLabel(int x, int y, int width, int height, string text) => GetLabel(x, y, width, height, text, null);
        public static Label GetLabel(int x, int y, int width, int height, string text, string name)
        {
            Label temp = new Label();
            temp.Location = new Point(x, y);
            temp.Size = new Size(width, height);
            temp.Text = text;
            temp.Name = name??"Default";
            return temp;
        }

        public static GroupBox GetGroupBox(int x, int y, int width, int height)
        {
            GroupBox temp = new GroupBox();
            temp.Size = new Size(width, height);
            temp.Location = new Point(x, y);
            return temp;
        }
    }
}

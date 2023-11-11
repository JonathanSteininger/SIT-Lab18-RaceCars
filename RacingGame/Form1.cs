using FormFactoryLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RacingGame
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            FormClosed += Form1_FormClosed;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            _RaceManager?.StopPermenant();
        }

        RaceManager _RaceManager;
        private void Form1_Load(object sender, EventArgs e)
        {
            Size = new Size(Screen.PrimaryScreen.Bounds.Width - 200, Screen.PrimaryScreen.Bounds.Height - 200) ;
            DoubleBuffered = true;


            _RaceManager = new RaceManager(5, this, ImageList1.Images["Wall.jpg"], ImageList1.Images["Car.png"], 20f);
            Controls.Add(ControlFactory.GetButton(10, 10, 200, 100, "Start", StartRace));
            Controls.Add(ControlFactory.GetButton(310, 10, 200, 100, "Reset", ResetRace));

        }

        private void ResetRace(object sender, EventArgs e)
        {
            _RaceManager.ResetRace();
        }

        private void StartRace(object sender, EventArgs e)
        {
            _RaceManager.StartRace();
        }
    }
}

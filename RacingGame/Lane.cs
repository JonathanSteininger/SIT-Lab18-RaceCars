using FormFactoryLibrary;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RacingGame
{
    public class Lane
    {
        static Random random;

        //private properties
        static private int TotalInstances;

        private int _laneID;
        private Player _player;

        private GroupBox _groupBox;


        private PictureBox _barPictureBox;
        private Dictionary<string, Label> _barLabels;
        private PictureBox _carPictureBox;

        private Image _backgroundImage;
        private Image _carImage;

        private float _distanceTraveled;


        //public Properties
        public Image BackgroundImage { get { return _backgroundImage; } set { _backgroundImage = value; } }
        public Image CarImage { get { return _carImage; } set { _carImage = value; } }
        public Size Size { get { return _groupBox.Size; } set { SetSize(value); } }
        public Point Location { get{ return _groupBox.Location; } set { _groupBox.Location = value; } }
        public GroupBox _GroupBox { get { return _groupBox; } }
        public float DistanceTraveled { get { return _distanceTraveled; } }
        public bool RaceCompleted { get; set; }


        //constants
        const float HEIGHT_FRACTION = 0.9f;
        const int MIN_BAR_HEIGHT = 10;

        const float CAR_HEIGHT_RATIO = 0.8f;

        const float SCROLL_START_FRACTION = 0.7f;

        private int GetLocalCarHeight => (int)(_carPictureBox.Height * CAR_HEIGHT_RATIO);

        /// <summary>
        /// Shortcut to get the width of the background image
        /// </summary>
        private int BackGroundWidth => GetImageWidth(_carPictureBox.Height, BackgroundImage);

        /// <summary>
        /// Lane Constructor
        /// </summary>
        /// <param name="player">Player info</param>
        /// <param name="x">Xpos relevant to parent</param>
        /// <param name="y">Ypos relevant to parent</param>
        /// <param name="width">width in pixels</param>
        /// <param name="height">height in pixels</param>
        /// <param name="backgroundImage">background image</param>
        /// <param name="carImage">car image</param>
        /// <param name="ParentControllerCollection">Controller collection from parent object</param>
        /// <param name="labels">Any labels that you want to add to the bar. the position is relevant to this lane object</param>
        public Lane(Player player, int x, int y, int width, int height, Image backgroundImage, Image carImage , Control.ControlCollection ParentControllerCollection, params Label[] labels)
        {
            if(random == null) random = new Random();
            _distanceTraveled = 0;
            _laneID = TotalInstances++;
            _player = player;
            _groupBox = ControlFactory.GetGroupBox(0,0,0,0);
            _barPictureBox = ControlFactory.GetPictureBox(0, 0, 0, 0, BarPaint);
            _carPictureBox = ControlFactory.GetPictureBox(0, 0, 0, 0, CarBoxPaint);

            RaceCompleted = false;

            _barLabels = new Dictionary<string, Label>();

            _backgroundImage = backgroundImage;
            _carImage = carImage;

            foreach (Label l in labels) AddLabel(l);

            Size = new Size(width, height);
            Location = new Point(x, y);

            AddDefaultLabels();

            //adding items to controls
            _groupBox.Controls.AddRange(new Control[] {_barPictureBox, _carPictureBox});

            ParentControllerCollection.Add(_groupBox);
            Reset();
        }

        public float SpeedPS { get; set; }

        public float GetDistance => _distanceTraveled;
        public void Reset()
        {
            _distanceTraveled = 0f;
            RacePosition = null;
            _groupBox.Refresh();
            RaceCompleted = false;
            ChangeSpeed();
            PastTime = 0;
        }

        private void ChangeSpeed()
        {
            SpeedPS = (float)random.NextDouble();
            if (SpeedPS < 0.4f) SpeedPS = 0.6f * ((float)random.NextDouble() + 1);

        }

        private void AddDefaultLabels()
        {
            int size = 120;
            int gap = 30;
            Label[] lables = new Label[]
            {
                ControlFactory.GetLabel((size + gap) * 0 , 0, size, MIN_BAR_HEIGHT, string.Empty, "PlayerName"),
                ControlFactory.GetLabel((size + gap) * 1, 0, size, MIN_BAR_HEIGHT, string.Empty, "Distance"),
                ControlFactory.GetLabel((size + gap) * 2, 0, size, MIN_BAR_HEIGHT, string.Empty, "Position"),
                ControlFactory.GetLabel((size + gap) * 3, 0, size, MIN_BAR_HEIGHT, string.Empty, "Time"),
                
                ControlFactory.GetLabel((size + gap) * 4 + 50, 0, size, MIN_BAR_HEIGHT, string.Empty, "Races"),
                ControlFactory.GetLabel((size + gap) * 5 + 50, 0, size, MIN_BAR_HEIGHT, string.Empty, "Gold"),
                ControlFactory.GetLabel((size + gap) * 6 + 50, 0, size, MIN_BAR_HEIGHT, string.Empty, "Silver"),
                ControlFactory.GetLabel((size + gap) * 7 + 50, 0, size, MIN_BAR_HEIGHT, string.Empty, "Bronze")
            };
            lables[0].Paint += (a, b) => (a as Label).Text = $"Name: {_player.Name}";
            lables[1].Paint += (a, b) => (a as Label).Text = $"Distance: {_distanceTraveled:.0} Chunks";
            lables[2].Paint += (a, b) => (a as Label).Text = _distanceTraveled == 0f ? "Race has not started!" : $"Postion: {RacePosition??"None"}";
            //lables[3].Paint += (a, b) => (a as Label).Text = _distanceTraveled == 0f ? "Race has not started!" : $"Postion: {RacePosition??"None"}";

            lables[3].Paint += (a, b) => (a as Label).Text = $"Total Races: {_player.TotalRaces}";
            lables[4].Paint += (a, b) => (a as Label).Text = $"Gold Medals: {_player.Medals.Count(medal => medal == Medal.Gold)}";
            lables[5].Paint += (a, b) => (a as Label).Text = $"Silver Medals: {_player.Medals.Count(medal => medal == Medal.Silver)}";
            lables[6].Paint += (a, b) => (a as Label).Text = $"Bronze Medals: {_player.Medals.Count(medal => medal == Medal.Bronze)}";

            foreach (Label l in lables) AddLabel(l);
        }

        public string RacePosition { get; set; }

        public float LeaderDistance { get; set; }

        public Lane Leader { get; set; }
        

        public void AddMedal(Medal medal)
        {
            _player.AddMedal(medal);
        }

        /// <summary>
        /// Sets the size of all the Controls in the Lane instance
        /// </summary>
        /// <param name="value">The new Size for the whole instance</param>
        /// <exception cref="Exception">if the value parameter value was null.</exception>
        private void SetSize(Size value)
        {
            //Exception
            if (value == null) throw new Exception("New Size given to Lane was Null");

            //Garentees the size is positive
            if (value.Width < 0 || value.Height < 0) value = GetAbsSize(value);
            
            _groupBox.Size = value;
            int FinalBoxSize = (int)((float)value.Height * HEIGHT_FRACTION);
            int FinalBarSize = value.Height - FinalBoxSize;

            
            if (value.Height < MIN_BAR_HEIGHT)//if the height is smaller than the bars minimum. 
            {
                //sets the bar to 0, and sets the picturebox to the entire height.
                FinalBarSize = 0;
                FinalBoxSize = value.Height;
            }else if(FinalBarSize < MIN_BAR_HEIGHT)//if the bars calculated height is smaller than the minimum
            {
                //sets the bar to minimum size, and leave the remaining size to the picturebox
                FinalBarSize = MIN_BAR_HEIGHT;
                FinalBoxSize = value.Height - MIN_BAR_HEIGHT;
            }
            //loops through each label thats in the bar and sets the height to the bar.
            _barLabels.RunForEach(label => label.Size = new Size(label.Size.Width, FinalBarSize));

            //sets the bar size
            _barPictureBox.Size = new Size(value.Width, FinalBarSize);

            //sets the carbox size;
            _carPictureBox.Location = new Point(0, FinalBarSize);
            _carPictureBox.Size = new Size(value.Width, FinalBoxSize);
        }

        /// <summary>
        /// Indexes the Dictionary
        /// </summary>
        /// <param name="key">The name of the Label</param>
        /// <returns>Label</returns>
        public Label GetLabel(string key)
        {
            if (_barLabels == null || !_barLabels.ContainsKey(key)) return null;
            return _barLabels[key];
        }



        //Math functions
        /// <summary>
        /// gets the size ratio of an image
        /// </summary>
        /// <param name="img">the image the ratio is gotten from</param>
        /// <returns>the float ratio</returns>
        private float GetImageRatio(Image img) => img.Width / img.Height;

        /// <summary>
        /// Gets the width compared to the input height and using the images size ratio.
        /// </summary>
        /// <param name="Height">the input height</param>
        /// <param name="img">the image needed to get the ratio</param>
        /// <returns>Final width</returns>
        private int GetImageWidth(int Height, Image img) => (int)(Height * GetImageRatio(img));
        /// <summary>
        /// Gets the Absolute size version of the inputed size
        /// </summary>
        /// <param name="value">the input size needing to be absoluted</param>
        /// <returns>Absolute Size</returns>
        private Size GetAbsSize(Size value) => new Size(value.Width < 0 ? value.Width * -1 : value.Width, value.Height < 0 ? value.Height * -1 : value.Height);


        /// <summary>
        /// Adds a label to the barsLabel dictionary and groupbox control.
        /// Uses the labels name for the dictionary key
        /// </summary>
        /// <param name="label">The label</param>
        public void AddLabel(Label label) => AddLabel(label.Name, label);

        /// <summary>
        /// Adds a label to the barsLabel dictionary and groupbox control
        /// </summary>
        /// <param name="key">the Key used for the dictionary</param>
        /// <param name="label">the label</param>
        public void AddLabel(string key, Label label)
        {
            label.AutoSize = false;
            label.TextAlign = ContentAlignment.MiddleLeft;
            label.Size = new Size(label.Size.Width, _barPictureBox.Size.Height);
            label.TextChanged += (src, evtArgs) => (src as Label).Refresh();
            _barPictureBox.Controls.Add(label);

            _barLabels.Add(key, label);
            _barLabels.RunForEach(item => item.BringToFront());
        }
        /// <summary>
        /// Adds a label to the barsLabel dictionary and groupbox control
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="text"></param>
        public void AddLabel(string key, int x, int y, int width, int height, string text) => AddLabel(key, ControlFactory.GetLabel(x, y, width, height, text));


        /// <summary>
        /// Moves the distance by a fraction of the background image. eg 2 = 2 times the width of the background.
        /// </summary>
        /// <param name="amount"></param>
        
        public void MoveCar(float amount) => _distanceTraveled += amount;
        public void MoveCar(int amount) => _distanceTraveled += amount;

        private long PastTime = 0;

        const long TICKS_PER_SECOND = 10000000;
        public void MoveCar()
        {
            double diffFraction = 0;
            if (PastTime != 0)
            {
                long diff = DateTime.Now.Ticks - PastTime;

                diffFraction = (double)diff / (double)TICKS_PER_SECOND;
            }
            RollSpeed(diffFraction);

            _distanceTraveled += (float)(SpeedPS * diffFraction);

            PastTime = DateTime.Now.Ticks;

        }

        private void RollSpeed(double diffFraction)
        {
            double roll = random.NextDouble();
            if (roll < diffFraction) ChangeSpeed();
        }

        //events

        /// <summary>
        /// Paints the Bar at the top of the lane. and refreshes all bar labels
        /// </summary>
        /// <param name="sender">object that invoked the method</param>
        /// <param name="e"></param>
        private void BarPaint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            if (_barPictureBox.BackColor == null) _barPictureBox.BackColor = Color.Gray;
            g.FillRectangle(new SolidBrush(_barPictureBox.BackColor), 0, 0, _barPictureBox.Width, _barPictureBox.Height);
            _barLabels.RunForEach(label => label.Refresh());
        }



        public void DrawFrame()
        {
            _groupBox.Refresh();
        }
        public void DrawCar()
        {
            _carPictureBox.Refresh();
            _barLabels["Distance"].Refresh();
            _barLabels["Position"].Refresh();
        }



        /// <summary>
        /// Paints the images in the _carPictureBox
        /// </summary>
        /// <param name="sender">object that invoked the method</param>
        /// <param name="e"></param>
        private void CarBoxPaint(object sender, PaintEventArgs e)
        {
            PictureBox box = sender as PictureBox;
            Graphics g = e.Graphics;
            if (Leader != null) LeaderDistance = Leader.GetDistance;
            LoopBackGround(box, g);
            DrawCar(box, g);
        }

        /// <summary>
        /// Draws the car for the current lane instance
        /// </summary>
        /// <param name="box">the picturebox to draw to</param>
        /// <param name="g">the graphics of the picturebox to draw to</param>
        private void DrawCar(PictureBox box, Graphics g)
        {
            /*
            int StartDist = (int)(SCROLL_START_FRACTION * box.Width);


            if (BackGroundWidth * LeaderDistance < SCROLL_START_FRACTION * box.Width) 
            float FinalDistance = _distanceTraveled;
            if(LeaderDistance > _distanceTraveled) FinalDistance -= LeaderDistance - _distanceTraveled;
            */
            if(LeaderDistance <= _distanceTraveled || LeaderDistance * BackGroundWidth < box.Width * SCROLL_START_FRACTION)
            {

                g.DrawImage(

                    _carImage,//image
                    Math.Min(_distanceTraveled * BackGroundWidth, box.Width * SCROLL_START_FRACTION),//xpos
                    box.Height - GetLocalCarHeight,//ypos
                    GetImageWidth(GetLocalCarHeight, _carImage),//width
                    GetLocalCarHeight//height
                    );
            }
            else
            {
                g.DrawImage(

                    _carImage,//image
                    _distanceTraveled * BackGroundWidth - Math.Max(0,LeaderDistance * BackGroundWidth - SCROLL_START_FRACTION * box.Width),//xpos
                    box.Height - GetLocalCarHeight,//ypos
                    GetImageWidth(GetLocalCarHeight, _carImage),//width
                    GetLocalCarHeight//height
                    );
            }
        }

        
        /// <summary>
        /// Draws the background of the current Lane instance
        /// </summary>
        /// <param name="box">the picturebox to draw to</param>
        /// <param name="g">the graphics of the picturebox to draw to</param>
        private void LoopBackGround(PictureBox box, Graphics g)
        {
            float finalDistance = (LeaderDistance > _distanceTraveled) ? LeaderDistance : _distanceTraveled;
            int BgDistancePixels = (int)Math.Max(0, BackGroundWidth * finalDistance - SCROLL_START_FRACTION * box.Width);
            int OffsetLeft = BgDistancePixels % BackGroundWidth;
            for(int i = 0; i < box.Width / BackGroundWidth + 2; i++)
            {
                g.DrawImage(_backgroundImage, i * BackGroundWidth - OffsetLeft, 0, BackGroundWidth, box.Height);
            }
        }

        internal void AddRaceTotal() => _player.AddRace();
    }
}

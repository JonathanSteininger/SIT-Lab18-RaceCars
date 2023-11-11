using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RacingGame
{
    public class LaneWorker : Worker
    {

        private float FinishDistance;
        private EventHandler FinishedRace;

        private ManualResetEvent mre = new ManualResetEvent(false);
        public Lane lane { get; set; }
        public LaneWorker(Lane lane, float finishDistance, EventHandler FinishedraceEventHandler)
        {
            this.lane = lane;
            base.Update += Update;
            base.Start += Start;
            FinishDistance = finishDistance;
            FinishedRace = FinishedraceEventHandler;
        }

        public new void Start()
        {
            lane._GroupBox.BeginInvoke(new Action(() =>
            {
                lane.Reset();
            }));
            TimeOutMS = 1000 / 30;
        }

        public new void Update()
        {
            //tabed items used to be in the invoke. moved out to make the thread stop the moment someone finishes a race.
            mre.WaitOne();
                //if (lane.RaceCompleted) return;
            lane._GroupBox.BeginInvoke(new Action(() =>
            {
                if (lane.RaceCompleted) return;
                lane.MoveCar();
                lane.DrawCar();
                if (FinishDistance <= lane.DistanceTraveled) FinishedRace.Invoke(this, EventArgs.Empty);
            }));
        }

        public void Resume() => mre.Set();
        public void Pause() => mre.Reset();

        public void Stop()
        {
            base.Stop();
            Resume();
        }


    }
}

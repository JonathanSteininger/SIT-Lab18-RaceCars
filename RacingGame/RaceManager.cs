using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;

namespace RacingGame
{
    public class RaceManager
    {
        public List<LaneWorker> Workers {  get; set; }
        public List<Thread> Threads { get; set; }

        public float FinishDistance { get; set; }
        public RaceManager(int AmountOfRacers, Form form, Image BackgroundTile, Image CarImage, float finishDistance)
        {
            Workers = new List<LaneWorker>();
            Threads = new List<Thread>();
            FinishDistance = finishDistance;

            for(int i = 1; i <= AmountOfRacers; i++)
            {
                Lane lane = new Lane(new Player($"Player{i}"), 0,200 * i, form.Size.Width, 200, BackgroundTile, CarImage, form.Controls);
                LaneWorker worker = new LaneWorker(lane, FinishDistance, RaceFinishedEvent);

                Workers.Add(worker);
                Thread thread = new Thread(worker.Run);
                Threads.Add(thread);
            }
            Start();
            UpdatePostions();
        }

        public void StartRace()
        {
            Start();
            Reset();
            Resume();
            RaceRunning = true;
        }
        public void ResetRace()
        {
            Pause();
            Reset();
        }

        public void Start()
        {
            foreach (Thread t in Threads) if(t.ThreadState == ThreadState.Unstarted) t.Start();
        }
        public void Resume()
        {
            foreach (LaneWorker w in Workers) w.Resume();
        }
        public void Pause()
        {
            foreach (LaneWorker w in Workers) w.Pause();
        }
        private bool raceExists = true;
        private bool RaceRunning = false;
        private async void UpdatePostions()
        {
            List<Lane> lanes = new List<Lane>(Workers.ConvertAll(item => item.lane));
            while (raceExists)
            {
                while (RaceRunning)
                {
                    lanes.Sort((b, a) => a.GetDistance.CompareTo(b.GetDistance));
                    Lane leader = lanes[0];
                    for (int i = 0; i < lanes.Count; i++)
                    {
                        if (!raceExists) break;
                        Lane lane = lanes[i];
                        string place = $"{i + 1}{((Positions)((i + 1) >= 4 ? 3 : i)).ToString()}";
                        lane._GroupBox.BeginInvoke(new Action(() => {
                            if (!raceExists) return;
                            lane.RacePosition = place;
                            lane.Leader = leader;
                            }));
                    }
                    await Task.Delay(100);
                }
                await Task.Delay(200);
            }
        }
        public enum Positions
        {
            st,
            nd,
            rd,
            th
        }

        public void StopPermenant()
        {
            foreach (LaneWorker w in Workers) w.Stop();
            foreach (Thread t in Threads) if(t.ThreadState != ThreadState.Unstarted) t.Join();
            raceExists = false;
        }
        
        public void Reset()
        {
            RaceRunning = false;
            Pause();
            foreach (LaneWorker w in Workers) w.lane._GroupBox.BeginInvoke(new Action(() => w.lane.Reset()));
        }
        public void RaceFinishedEvent(object sender, EventArgs args)
        {
            foreach (LaneWorker w in Workers) w.lane.RaceCompleted = true;//added to make the race end the moment a real thread's race finishes
            Pause();

            AwardMedals();
            foreach (LaneWorker w in Workers) w.lane.AddRaceTotal();
            //redraws lanes after alreting stats
            foreach (LaneWorker w in Workers) w.lane._GroupBox.BeginInvoke(new Action(() => w.lane.DrawFrame()));
            //foreach (LaneWorker w in Workers) w.lane.DrawFrame();
            /*
             * just for testing if it is random.
            ResetRace();
            Thread.Sleep(200);
            StartRace();
            */
        }

        private void AwardMedals()
        {
            Workers.Sort((b,a) => a.lane.DistanceTraveled.CompareTo(b.lane.DistanceTraveled));
            if(Workers.Count <= 1) return;

            Award(Workers[0], Medal.Gold);
            int tick = 0;
            for(int i = 1; i < Workers.Count; i++)
            {
                Lane P = Workers[i - 1].lane;
                Lane C = Workers[i].lane;

                if (C.DistanceTraveled != P.DistanceTraveled) tick++;
                if (tick > (int)Medal.Bronze) break;

                Award(Workers[i], (Medal)tick);
            }
        }

        private void Award(LaneWorker w, Medal medal)
        {
            w.lane.AddMedal(medal);
        }
    }
}

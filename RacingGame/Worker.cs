using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace RacingGame
{
    public class Worker
    {
        public bool Completed = false;
        public int TimeOutMS = 1000;

        public SimpleDelegate Update;
        public SimpleDelegate Start;
        public void Run()
        {
            Start?.Invoke();
            while (!Completed)
            {
                Update?.Invoke();
                Thread.Sleep(TimeOutMS);
            }
        }
        public void Stop()
        {
            Completed = true;
        }

        public delegate void SimpleDelegate();
    }
}

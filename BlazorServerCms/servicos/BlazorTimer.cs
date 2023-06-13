using System.Timers;

namespace BlazorServerCms.servicos
{
    public class BlazorTimer
    {
        public System.Timers.Timer? _timer = new System.Timers.Timer(1000);

        public void SetTimer(double interval)
        {
            _timer!.Interval = interval;
            _timer.Enabled = true;
        }       
    }
}

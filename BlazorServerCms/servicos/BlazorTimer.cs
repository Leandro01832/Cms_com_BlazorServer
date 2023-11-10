using System.Timers;

namespace BlazorServerCms.servicos
{
    public class BlazorTimer
    {
        public System.Timers.Timer? _timer = new System.Timers.Timer(1000);
        public System.Timers.Timer? _timerListaFiltro = new System.Timers.Timer(1000);
        public System.Timers.Timer? _timerVitrine = new System.Timers.Timer(1000);
        public System.Timers.Timer? desligarAuto;

        public void SetTimer(double interval)
        {
            _timer!.Interval = interval;
            _timer.Enabled = true;
        }
        
        public void SetTimerListaFiltro(double interval)
        {
            _timerListaFiltro!.Interval = interval;
            _timerListaFiltro.Enabled = true;
        }
        
        public void SetTimerVitrine(double interval)
        {
            _timerVitrine!.Interval = interval;
            _timerVitrine.Enabled = true;
        }

        public void SetTimerAuto()
        {
            desligarAuto = new System.Timers.Timer(900000); // 15 min
            desligarAuto.Enabled = true;
        }
    }
}

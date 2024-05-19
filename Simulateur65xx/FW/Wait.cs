
using System.Windows.Forms;

namespace Simulateur65xx.FW
{
    public class Timing
    {
        public static Timer timer1 = new Timer();

        public static void Wait(int milliseconds)
        {
            if (Main.isClosing) return;
            timer1 = new Timer();
            if (milliseconds == 0 || milliseconds < 0) return;
            timer1.Interval = milliseconds;
            timer1.Enabled = true;
            timer1.Start();
            timer1.Tick += (s, e) =>
            {
                timer1.Enabled = false;
                timer1.Stop();
            };
            while (timer1.Enabled)
            {
                Application.DoEvents();
            }
        }
    }
}

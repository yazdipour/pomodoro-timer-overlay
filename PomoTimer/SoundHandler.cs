using System.Globalization;
using System.Media;

namespace PomoTimer
{
    internal class SoundHandler
    {
        public void PlaySound(string path)
        {
            var player = new SoundPlayer(path);
            player.Load();
            player.Play();
        }

        public static void Beep() => SystemSounds.Beep.Play();
    }
}

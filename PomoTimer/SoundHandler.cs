using System.Media;

namespace PomoTimer
{
    internal class SoundHandler
    {
        public static void PlaySound(string path)
        {
            var player = new SoundPlayer(path);
            player.Load();
            player.Play();
        }
    }
}

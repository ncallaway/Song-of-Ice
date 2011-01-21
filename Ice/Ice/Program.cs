using System;

namespace Sands
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (SongOfIce game = new SongOfIce())
            {
                game.Run();
            }
        }
    }
}


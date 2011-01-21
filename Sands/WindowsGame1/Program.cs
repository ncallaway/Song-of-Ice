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
            using (SandsGame game = new SandsGame())
            {
                game.Run();
            }
        }
    }
}


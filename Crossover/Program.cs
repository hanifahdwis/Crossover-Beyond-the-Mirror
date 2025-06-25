using System;
using System.Windows.Forms;
using Crossover;

namespace SpriteAnimation
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MenuForm());
            Application.Run(new LevelForm());
    
        }
    }
}
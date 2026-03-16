using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PL
{
    internal class Program
    {
        [STAThread]
        public static void Main()
        {
            var app = new App();
            app.InitializeComponent();

            // יצירת מופע של החלון שלך
            var window = new Entrance();

            // הפעלת האפליקציה והצגת החלון הזה
            app.Run(window);
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Unity3DStart
{
    public partial class Form1 : Form
    {
        /// <summary> 
        /// 启动画面本身 
        /// </summary> 
        static Form1 instance;
        /// <summary> 
        /// 显示的图片 
        /// </summary> 
        Bitmap bitmap;
        public static Form1 Instance
        {
            get
            {
                return instance;
            }
            set
            {
                instance = value;
            }
        }
        public Form1()
        {
            InitializeComponent();
            const string showInfo = "启动画面：我们正在努力的加载程序，请稍后...";
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.CenterScreen;
            ShowInTaskbar = false;
            bitmap = new Bitmap(Properties.Resources.Form1);
            ClientSize = bitmap.Size;
            using (Font font = new Font("Consoles", 30))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.DrawString(showInfo, font, Brushes.White, 130, 100);
                }
            }
            BackgroundImage = bitmap;
        }

        public static void ShowSplashScreen()
        {
            instance = new Form1();
            instance.Show();
        }
        public static void HiddenScreen() {
            if (instance != null) {
                instance.Hide();
                instance = null;
            }
        }
    }
}

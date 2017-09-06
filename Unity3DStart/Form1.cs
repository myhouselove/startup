using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
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
        static string url = "http://app-test.orbbec.me/upgrade/package/";
        static string data = "";
        static string data_zip_path = "";
        static string recievePath = "";
        private static string data_name = "";
        static string returnstr = "";


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
            const string showInfo = "Loading...";
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.CenterScreen;
            ShowInTaskbar = false;
            bitmap = new Bitmap("start.jpg");
            int w = bitmap.Width;
            int h = bitmap.Height;
            this.Width = w;
            this.Height = h;
            

            ClientSize = bitmap.Size;
            using (Font font = new Font("Consoles", 30))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.DrawString(showInfo, font, Brushes.White, 130, h-50);
                }
            }
            BackgroundImage = bitmap;

            ReadVersionFromLocal();
            returnstr = HttpGet(url, data);
            Console.WriteLine(returnstr);

            JObject obj = JObject.Parse(returnstr);
            string returncode;
            recievePath = System.Environment.CurrentDirectory;

            if (obj != null)
            {
                returncode = obj["code"].ToString();
                Console.WriteLine("code         =" + obj["code"]);
            }
            else
            {
                returncode = "";
                Console.WriteLine("null-------------- ");
            }

            if (returncode.Equals("0"))
            {
                Console.WriteLine("Need to Upgrade");
                string data_url = obj["data"]["url"].ToString();
                // string new_version_code = obj["data"]["version_code"].ToString();


                DownloadZipFromService(data_url);

                //UnzipAndInstallPackage();
                // WriteToLocal(returnstr);
                //startUnity3DScanner();
               // System.Environment.Exit(0);
            }
            else if (returncode.Equals("99"))
            {
                Console.WriteLine("Already newest!");
                startUnity3DScanner();
                System.Environment.Exit(0);
            }
            else
            {
                Console.WriteLine("Service wrong!");
                startUnity3DScanner();
                System.Environment.Exit(0);
            }


            
        }

        private void DownloadZipFromService(string data_url)
        {
            WebClient client = new WebClient();
            
            Console.WriteLine("recievePath       =" + recievePath);
            data_name = System.IO.Path.GetFileName(data_url);
            Console.WriteLine("dataname       =" + data_name);
            client.DownloadFileAsync(new Uri(data_url), recievePath + data_name);
            client.DownloadProgressChanged += new System.Net.DownloadProgressChangedEventHandler(wc_DownloadProgressChanged);
            client.DownloadFileCompleted += new AsyncCompletedEventHandler(wc_DownloadFileCompleted);
        }

        private static void ReadVersionFromLocal()
        {
            StreamReader sr = new StreamReader("version.ini", Encoding.Default);
            // Console.Write("ini       = "+sr.ReadToEnd());
            string json = sr.ReadToEnd().ToString();
            sr.Close();

            Console.Write("start:");
            Console.Write("" + json);
            Console.Write("end");
            JObject obj = JObject.Parse(json);
            string version_code = obj["data"]["version_code"].ToString();
            string version_name = obj["data"]["version_name"].ToString();
            data = "channel_no=scanner-app&version_code=" + version_code;
        }

        private static void WriteToLocal(string returnstr)
        {
            FileStream fs = new FileStream("version.ini", FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            //开始写入
            sw.Write(returnstr);
            //清空缓冲区
            sw.Flush();
            //关闭流
            sw.Close();
            fs.Close();
        }
        private static void startUnity3DScanner()
        {
            //开启一个新process
            System.Diagnostics.ProcessStartInfo p = null;
            System.Diagnostics.Process proc;

            string exe_p = @"\Unity3D\3DBodyScanner.exe";
            
            p = new System.Diagnostics.ProcessStartInfo(recievePath+ exe_p);
            p.WorkingDirectory = recievePath+@"\Unity3D";//设置此外部程序所在windows目录

            Console.WriteLine("exe path    ="+ recievePath + exe_p);
            Console.WriteLine("p.WorkingDirectory  = "+ p.WorkingDirectory);
            proc = System.Diagnostics.Process.Start(p);//调用外部程序
        }

        private static void UnzipAndInstallPackage(string v, string recievePath)
        {
            ZipHelper.UnZip(v, recievePath);
        }


        public static string HttpGet(string Url, string postDataStr)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url + (postDataStr == "" ? "" : "?") + postDataStr);
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.UTF8);
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();

            return retString;
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

        private void Form1_Load(object sender, EventArgs e)
        {

        }



        public void wc_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            //throw new NotImplementedException();

            UnzipAndInstallPackage(recievePath + data_name, recievePath);
            WriteToLocal(returnstr);
            if (MessageBox.Show("您已经下载成功,是否打开应用程序？", "提示", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                // System.Diagnostics.Process.Start("QQ2010SP3.1.exe");//启动刚下载的程序
                startUnity3DScanner();
                this.Close();
            }
            else
            {
                this.Close(); ;
            }

        }

        void wc_DownloadProgressChanged(object sender, System.Net.DownloadProgressChangedEventArgs e)
        {
            //throw new NotImplementedException();
            progressBar2.Value = e.ProgressPercentage;//将progressbar的值设为下载的百分比
            label2.Text = progressBar2.Value + "%";
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}

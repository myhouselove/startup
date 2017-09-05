using System;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace Unity3DStart
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 
        static string url = "http://app-test.orbbec.me/upgrade/package/";
        static string data = "";

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Form1.ShowSplashScreen();
            // 进行自己的操作：加载组件，加载文件等等 
            // 示例代码为休眠一会 
            System.Threading.Thread.Sleep(3000);
            // 关闭 
            if (Form1.Instance != null)
            {
                Form1.Instance.BeginInvoke(new MethodInvoker(Form1.Instance.Dispose));
                Form1.Instance = null;
            }
            //Application.Run(new FormMain());
            ReadVersionFromLocal();
            //data = "channel_no=scanner-app&version_code=0";
            string returnstr = HttpGet(url, data);
            

            Console.WriteLine(returnstr);

            JObject obj = JObject.Parse(returnstr);
            string returncode;
            if (obj != null)
            {
                Console.WriteLine("data url         =" + obj["data"]["url"]);
                Console.WriteLine("data type         =" + obj["data"]["type"]);
                returncode = obj["code"].ToString();
                Console.WriteLine("code         =" + obj["code"]);
            }
            else {
                returncode = "";
                Console.WriteLine("null-------------- "); 
            }

            if (returncode.Equals("0"))
            {
                Console.WriteLine("Need to Upgrade");
                string data_url = obj["data"]["url"].ToString();
                // string new_version_code = obj["data"]["version_code"].ToString();
                WriteToLocal(returnstr);

                DownloadZipFromService(data_url);
                UnzipAndInstallPackage();
                startUnity3DScanner();
                System.Environment.Exit(0);
            }
            else if (returncode.Equals("99"))
            {
                Console.WriteLine("Already newest!");
            }
            else {
                Console.WriteLine("Service wrong!");
                Application.Exit();
            }

            Application.Run(new Form1());





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

        private static void ReadVersionFromLocal()
        {
            StreamReader sr = new StreamReader("version.ini", Encoding.Default);
           // Console.Write("ini       = "+sr.ReadToEnd());
            string json = sr.ReadToEnd().ToString();
            Console.Write("start:");
            Console.Write("" + json);
            Console.Write("end");
            JObject obj = JObject.Parse(json);
            string version_code = obj["data"]["version_code"].ToString();
            string version_name = obj["data"]["version_name"].ToString();
            data = "channel_no=scanner-app&version_code="+version_code;
        }

        private static void UnzipAndInstallPackage()
        {

            //unzip and install
            ZipHelper.UnZip("e:/data.zip","e:/test-scanner/");

        }

        private static void startUnity3DScanner()
        {
            //开启一个新process
            System.Diagnostics.ProcessStartInfo p = null;
            System.Diagnostics.Process proc;


            p = new System.Diagnostics.ProcessStartInfo("G:/Unity3D/scan-exe/ui2.exe");
            p.WorkingDirectory = "G:/Unity3D/scan-exe/";//设置此外部程序所在windows目录
            proc = System.Diagnostics.Process.Start(p);//调用外部程序
        }

        private static void DownloadZipFromService(string data_url)
        {
            WebClient client = new WebClient();
            string recievePath = @"E:\";
            client.DownloadFile(data_url,recievePath+System.IO.Path.GetFileName(data_url));

           // throw new NotImplementedException();
        }

        public static string  HttpGet(string Url, string postDataStr) {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url+(postDataStr == "" ? "" : "?")+postDataStr);
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

    }



}

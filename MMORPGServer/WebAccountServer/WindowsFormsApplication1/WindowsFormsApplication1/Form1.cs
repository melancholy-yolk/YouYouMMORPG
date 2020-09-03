using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;
using System.Net;
using System.IO;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            label1.Text = "hello";
            int length = await AccessTheWebAsync();
            Image image = await GetImageAsync();

            textBox1.Text += string.Format("\r\n{0}\r\n", length);
            pictureBox1.Image = image;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 异步web请求
        /// </summary>
        /// <returns></returns>
        async Task<int> AccessTheWebAsync()
        {
            HttpClient client = new HttpClient();
            Task<string> getStringTask = client.GetStringAsync("http://msdn.microsoft.com");
            DoIndependentWork();
            string urlContents = await getStringTask;
            return urlContents.Length;
        }

        /// <summary>
        /// 异步web请求图片
        /// </summary>
        /// <returns></returns>
        async Task<Image> GetImageAsync()
        {
            HttpClient client = new HttpClient();
            Task<Stream> getStreamTask = client.GetStreamAsync("http://www.baidu.com/img/baidu_logo.gif");
            Stream stream = await getStreamTask;
            Image image = Image.FromStream(stream);
            return image;
        }

        void DoIndependentWork()
        {
            textBox1.Text += "Working......\r\n";
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        Image DownloadImage()
        {
            Image image = Image.FromStream(WebRequest.Create("http://www.baidu.com/img/baidu_logo.gif").GetResponse().GetResponseStream());
            return image;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}

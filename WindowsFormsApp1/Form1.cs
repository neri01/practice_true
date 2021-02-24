using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Emgu;
using Emgu.CV;
using Emgu.CV.Util;
using Emgu.CV.Structure;
using Emgu.Util;

using DirectShowLib;
using System.IO;
using System.Threading;
using System.Net;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private VideoCapture capture = null;
        private DsDevice[] webCams = null;
        private int selectedCameraId = 0;
        private string filename = "output.mp4";
        private string filename_output=null;
        int backend_idx = 0;
        int fourcc = 0;
        VideoWriter vw;
        bool video_saving = false;
        public Form1()
        {
            InitializeComponent();
        }

        private void video_load(string filename,string http)
        {
            WebClient wc = new WebClient();
            //wc.Headers.Add("Content-Type", "binary/octet-stream");
            Uri uri = new Uri(http);
            wc.UploadFile(uri,filename);
            //string uri = http;
            //string localPath = filename;
            //using (var client = new WebClient())
            //{
            //    client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            //    var responseBytes = client.UploadFile(uri, localPath);
            //    var response = Encoding.UTF8.GetString(responseBytes);
            //}
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            webCams = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);
            for (int i = 0; i < webCams.Length; i++)
            {
                toolStripComboBox1.Items.Add(webCams[i].Name);
            }
            Backend[] backends = CvInvoke.WriterBackends;
            foreach (Backend be in backends)
            {
                if (be.Name.Equals("MSMF"))
                {
                    backend_idx = be.ID;
                    break;
                }
            }
            fourcc = FourCC.H264;
            vw = new VideoWriter(filename, backend_idx, fourcc, 30, new Size(640, 360), true);
        }

        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedCameraId = toolStripComboBox1.SelectedIndex;
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {

            try
            {
                if (webCams.Length == 0)
                {
                    throw new Exception("нет доступных камер");
                }
                else if (toolStripComboBox1.SelectedItem == null)
                {
                    throw new Exception("необходимо выбрать камеру");
                }
                else if (capture != null)
                {
                    capture.Start();
                }
                else
                {
                    capture = new VideoCapture(selectedCameraId);
                    capture.ImageGrabbed += Capture_ImageGrabbed;
                    capture.Start();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Capture_ImageGrabbed(object sender, EventArgs e)
        {
            try
            {
                Mat m = new Mat();
                capture.Retrieve(m);
                pictureBox1.Image = m.ToImage<Bgr, byte>().Flip(Emgu.CV.CvEnum.FlipType.Horizontal).ToBitmap();
                if (video_saving)
                {
                    if (vw == null)
                    {
                        vw = new VideoWriter(filename, backend_idx, fourcc, 30, new Size(640, 360), true);
                    }
                    vw.Write(m);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            try
            {
                if (capture != null)
                {
                    capture.Pause();
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            try
            {
                if (capture != null)
                {
                    capture.Pause();
                    capture.Dispose();
                    capture = null;
                    pictureBox1.Image.Dispose();
                    pictureBox1.Image = null;
                    selectedCameraId = 0;
                    video_saving = false;
                    
                    //FileInfo fileInf = new FileInfo(filename);
                    //fileInf.Delete();
                }
                else
                {
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            try
            {
                Mat m = new Mat();
                capture.Retrieve(m);
                Form2 form = new Form2(m.ToImage<Bgr, byte>().Flip(Emgu.CV.CvEnum.FlipType.Horizontal));
                form.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            if (capture != null)
            {
                video_saving = true;
                capture.Start();
            }
            else
            {
                capture = new VideoCapture("rtsp://kirill:kirill@192.168.88.101:554/stream2");
                capture.ImageGrabbed += Capture_ImageGrabbed;
                video_saving = true;
                capture.Start();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (capture != null)
            {
                capture.Pause();
                string filename1;
                filename1 = $"output_{DateTime.Now.Day}_{DateTime.Now.Month}_{DateTime.Now.Year}_{DateTime.Now.Hour}_{DateTime.Now.Minute}_{DateTime.Now.Second}.mp4";
                vw = new VideoWriter(filename1, backend_idx, fourcc, 30, new Size(640, 360), true);
                if (filename_output != null)
                {
                    video_load(filename_output, "http://127.0.0.1:8000/act");
                }
                filename_output = filename;
                filename = filename1;
                capture.Start();
                
            }
        }
    }
}

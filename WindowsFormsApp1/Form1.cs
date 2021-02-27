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
using System.Net.Http;
using System.Net.Http.Headers;
using System.Globalization;
using System.Net.Http.Handlers;
using VisioForge.MediaFramework.ONVIF;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private VideoCapture capture = null;
        private DsDevice[] webCams = null;
        private int selectedCameraId = 0;
        private string filename = "output.mp4";
        private string filename_output = null;
        int backend_idx = 0;
        int fourcc = 0;
        VideoWriter vw;
        bool video_saving = false;
        public Form1()
        {
            InitializeComponent();
        }


        private async void video_load(string filename, string http)
        {
            //using (HttpClient client = new HttpClient())
            //{
            //    HttpRequestMessage message = new HttpRequestMessage();
            //    message.Method = HttpMethod.Post;
            //    message.RequestUri = new Uri(http);
            //    //message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);


            //    var content =
            //        new MultipartFormDataContent("Upload----" + DateTime.Now.ToString(CultureInfo.InvariantCulture));


            //    var streamContent = new StreamContent(File.OpenRead(filename));
            //    streamContent.Headers.ContentType = new MediaTypeHeaderValue("video/mp4");
            //    content.Add(streamContent, Path.GetFileName(filename), Path.GetFileName(filename));
            //    message.Content = content;

            //    var response =
            //        await client.SendAsync(message);
            //    MessageBox.Show(response.ToString());
            //
            //if (response.IsSuccessStatusCode)
            //{
            //    return await response.Content.ReadAsStringAsync();

            //}
            //else
            //{
            //    throw new ApplicationException($"{response.StatusCode} {response.ReasonPhrase}");
            //}
            //-------------------------------------------------------------------------------------------------------------------
            //}
            ////var files = new List<string>();

            ////files.Add(@"c:\temp\midi.xml");
            ////files.Add(@"c:\temp\maxi.xml");
            ////files.Add(@"c:\temp\efti.xml");
            //var file = new FileStream(filename, FileMode.Open);

            ////var apiUri = string.Format("https://api-dev.qbranch.se/api/customers/{0}/tickets/{1}/attachments",
            ////                           customerId, ticketId);

            //var message = new HttpRequestMessage();
            //message.RequestUri = new Uri(http);
            ////message.Headers.Add("qnet-api-key", "7rBbXytkCFjgo+2GHu6gHyj4VVZeEaVudANlLNl4WLg=");
            //message.Method = HttpMethod.Post;

            //var content = new MultipartFormDataContent();
            //    var fileName = Path.GetFileName(filename);
            //    content.Add(new StreamContent(file), "video", filename);

            //message.Content = content;

            //using (var client = new HttpClient())
            //{
            //    var response = await client.SendAsync(message);
            //    MessageBox.Show(response.ToString());
            //    //return response.StatusCode;
            //}
            //-----------------------------------------------------------------------------------------------------------------
            //{
            //    using (var client = new HttpClient())
            //    {
            //        using (var stream = File.OpenRead(filename))
            //        {
            //            var content = new MultipartFormDataContent();
            //            var file_content = new ByteArrayContent(new StreamContent(stream).ReadAsByteArrayAsync().Result);
            //            file_content.Headers.ContentType = new MediaTypeHeaderValue("video/mp4");
            //            //file_content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            //            //{
            //            //    FileName = "screenshot.png",
            //            //    Name = "foo",
            //            //};
            //            content.Add(file_content);
            //            //client.BaseAddress = new Uri(http);
            //            var response = await client.PostAsync(http, content);
            //            MessageBox.Show(response.ToString());

            //        }
            //    }
            //}

            //------------------------------------------------------------------------------------------------------
            //using (var client = new HttpClient())
            //using (var formData = new MultipartFormDataContent())
            //using (var fileStream = File.OpenRead(filename))
            //{
            //    HttpContent fileStreamContent = new StreamContent(fileStream);

            //    var filename0 = Path.GetFileName(filename);

            //    // эмулируем <input type="file" name="video"/>
            //    formData.Add(fileStreamContent, "video");
            //    formData.Headers.Add("key", "video");
            //    // эмулируем (action="{url}" method="post")
            //    var response = await client.PostAsync(http, formData);
            //    MessageBox.Show(response.ToString());

            // и т. д.
            //}
            //------------------------------------------------------------------------------------------------------
            FileStream stream = File.Open(filename, FileMode.Open);
            HttpContent fileStreamContent = new StreamContent(stream);
            using (var client = new HttpClient())
            using (var formData = new MultipartFormDataContent())
            {
                //formData.Headers.Add("type","file");
                //formData.Headers.Add("id","video");
                formData.Add(fileStreamContent, "video",filename);
                var response = await client.PostAsync(http, formData);
                MessageBox.Show(response.ToString());
            }
            //-----------------------------------------------------------------------
            //WebClient wc = new WebClient();
            //Uri uri = new Uri(http);
            //wc.Headers.Add("name", "video");
            //wc.Headers.Add("id", "video");
            //wc.Headers.Add("type", "file");
            //string responce = (wc.UploadFile(uri, filename)).ToString();

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
            video_load(@"C:\Users\kisum\Downloads\dog.mp4", @"http://127.0.0.1:8000/act");
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
                filename1 = $"output_{System.DateTime.Now.Day}_{System.DateTime.Now.Month}_{System.DateTime.Now.Year}_{System.DateTime.Now.Hour}_{System.DateTime.Now.Minute}_{System.DateTime.Now.Second}.mp4";
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

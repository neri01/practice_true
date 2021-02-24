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
namespace practice_true
{
    public partial class Form1 : Form
    {
       
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            VideoCapture capture = new VideoCapture(/*"rtsp://kirill:kirill@192.168.1.103:554/stream2"*/);
            Mat mat = capture.QueryFrame();
            pictureBox1.Image = mat.ToBitmap();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
        }
    }
}

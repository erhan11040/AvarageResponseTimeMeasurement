using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace requestMaker
{
    public partial class Form1 : Form
    {
        private int timeElapsed = 0;
        private int successfullRequestCount=0;
        private int failedRequestCount=0;
        private List<long> allRequests;
        private bool isWorking = false;
        private Stopwatch watchTotal;
        private Stopwatch watchRequest;

        public Form1()
        {
            InitializeComponent();
            watchRequest = new Stopwatch();
            watchTotal = new Stopwatch();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            isWorking = false;
            watchTotal.Stop();
        }
        private void reset()
        {
            timeElapsed = 0;
            successfullRequestCount = 0;
            failedRequestCount = 0;
            allRequests = new List<long>();
            lwSuccess.Clear();
            lwFail.Clear();
            watchRequest.Stop();
            watchTotal.Stop();
            watchRequest.Reset();
            watchTotal.Reset();
        }
        private void updateLabels()
        {
            double average = allRequests.Count > 0 ? allRequests.Average() : 0.0;
            lbAvarage.Text = average.ToString("0.##");
            lbFailed.Text = failedRequestCount.ToString();
            lbSuccess.Text = successfullRequestCount.ToString();
            lbRequest.Text = allRequests.Count.ToString();
            watchRequest.Reset();
        }
        private string calculateElapsed(Stopwatch watch)
        {
            var ts = watch.Elapsed;
            string elapsedTime = $"{ts.Hours}:{ts.Minutes}:{ts.Seconds}.{ts.Milliseconds / 10}";
            return elapsedTime;
        }
        private async void btnStart_Click(object sender, EventArgs e)
        {
            reset();
            isWorking = true;
            watchTotal.Start();
            while (isWorking)
            {
                long elapsed = 0;
                HttpWebResponse response;
                lbTime.Text = calculateElapsed(watchTotal);
                watchRequest.Start();
                try
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(tbUrl.Text);
                    response = (HttpWebResponse)await request.GetResponseAsync();
                    string content = new StreamReader(response.GetResponseStream()).ReadToEnd();
                    watchRequest.Stop();
                    elapsed = watchRequest.ElapsedMilliseconds;
                }
                catch (Exception ex)
                {
                    elapsed = watchRequest.ElapsedMilliseconds;
                    lwFail.Items.Add(new ListViewItem(elapsed.ToString()));
                    failedRequestCount++;
                    continue;
                }
               
                watchRequest.Stop();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    lwSuccess.Items.Add(new ListViewItem(elapsed.ToString()));
                    successfullRequestCount++;
                }
                else
                {
                    lwFail.Items.Add(new ListViewItem(elapsed.ToString()));
                    failedRequestCount++;
                }
                allRequests.Add(elapsed);
                updateLabels();
            }
        }

    }
}

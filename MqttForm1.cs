using System;
using System.Text;
using System.Windows.Forms;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace WowsMq
{
    public partial class MqttFrom : Form
    {
        public MqttFrom()
        {
            InitializeComponent();
        }
        MqttClient mqttClient;

        private void MqttFrom_Load(object sender, EventArgs e)
        {
            button6.Enabled = false;
            button7.Enabled = false;
            this.textBoxUrl.Text = "mq.wows.shinoaki.com:1883";
            this.textBoxClientId.Text = "mqttx_yuyuko";
            this.textBoxAccountId.Text = "wows-poll";
            this.textBoxPassword.Text = "wows-poll";
        }

        /// <summary>
        /// 连接
        /// </summary>
        private void Connnect()
        {
            this.textBoxClientId.Enabled = false;
            this.textBoxAccountId.Enabled = false;
            this.textBoxPassword.Enabled = false;
            this.textBoxUrl.Enabled = false;
            string[] url =  this.textBoxUrl.Text.Trim().Split(':');
            mqttClient = new MqttClient(url[0],int.Parse(url[1]),false,null,null,MqttSslProtocols.None);
            mqttClient.Connect(this.textBoxClientId.Text.Trim(), this.textBoxAccountId.Text.Trim(), this.textBoxPassword.Text.Trim());
            mqttClient.MqttMsgPublishReceived += client_MqttMsgPublishReceived;
            this.buttonEnable.Text = "断开连接";
            CommandLog("mqtt连接成功");
        }

        private void Disconnect()
        {
            this.textBoxClientId.Enabled = true;
            this.textBoxAccountId.Enabled = true;
            this.textBoxPassword.Enabled = true;
            this.textBoxUrl.Enabled = true;
            mqttClient.Disconnect();
            this.buttonEnable.Text = "连接mqtt";
            CommandLog("mqtt断开连接");
        }

        private void buttonEnable_Click(object sender, EventArgs e)
        {
            if (this.buttonEnable.Text.Equals("连接mqtt"))
            {
                Connnect();
            }
            else
            {
                Disconnect();
            }
        }

      
        private void CommandLog(string data)
        {
            Invoke((new Action(() =>
            {
                this.richTextBox1.AppendText(DateTime.Now.ToString("HH:mm:ss.fff") + " => " + data + "\r\n");
                if (this.richTextBox1.Lines.Length >= 10000)
                {
                    this.richTextBox1.Clear();
                }
                //this.richTextBoxCommandLog.SelectionStart = this.richTextBoxCommandLog.Lines.Length;
                this.richTextBox1.ScrollToCaret();
            })));
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.richTextBox1.Clear();
        }

        private void buttonGzip_Click(object sender, EventArgs e)
        {
            if (this.buttonGzip.Text.Equals("启用Gzip"))
            {
                this.buttonGzip.Text = "禁用Gzip";
            }
            else
            {
                this.buttonGzip.Text = "启用Gzip";
            }
        }

        private void buttonPush_Click(object sender, EventArgs e)
        {
            string data = this.textBoxPushText.Text.Trim();
            //是否压缩
            byte[] dataBytes;
            if (this.buttonGzip.Text.Equals("禁用Gzip"))
            {
                dataBytes = GzipUtils.Compress(data);
            }
            else
            {
                dataBytes = Encoding.UTF8.GetBytes(data);
            }
            mqttClient.Publish(this.textBoxPushTopic.Text.Trim(), dataBytes,byte.Parse(this.textBoxPushQos.Text.Trim()),false);
            CommandLog("推送数据："+data);
        }

         void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            // handle message received 
            string data;
            if (this.buttonGzip.Text.Equals("禁用Gzip"))
            {
                data = Encoding.UTF8.GetString(GzipUtils.Decompress(e.Message));
            }
            else
            {
                data = Encoding.UTF8.GetString(e.Message);
            }
            CommandLog("订阅数据 topic=["+e.Topic+"] data="+data);
        }

        private void buttonSub_Click(object sender, EventArgs e)
        {
            mqttClient.Subscribe(new string[] { this.textBoxSubTopic.Text.Trim() }, new byte[] { byte.Parse(this.textBoxSubQos.Text.Trim()) });
        }
    }
}

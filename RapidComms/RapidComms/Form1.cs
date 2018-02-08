using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Collections.Specialized;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Diagnostics;
using System.Drawing.Imaging;

namespace RapidComms
{
    public partial class Form1 : Form
    {
        [DllImport("User32.dll")]
        protected static extern int SetClipboardViewer(int hWndNewViewer);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern bool ChangeClipboardChain(IntPtr hWndRemove, IntPtr hWndNewNext);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);

        IntPtr nextClipboardViewer;
        bool firstTime = true;

        const string SERVER_IP = "10.0.0.152";
        const int SERVER_PORT = 8889;

        TcpClient clientSocket = new TcpClient();
        bool formClosing = false;

        bool clipboardSet = false;

        System.Windows.Forms.Timer fadeForm = new System.Windows.Forms.Timer();

        public class Message
        {
            public int type;
            public string message;

            public Message(int mType, string mMessage)
            {
                this.type = mType;
                this.message = mMessage;
            }
        }

        List<Message> messageQueue = new List<Message>();

        private void OnClipboardChanged()
        {
            if (firstTime)
            {
                firstTime = false;
            }
            else
            {
                this.Opacity = 1;

                if(fadeForm.Enabled)
                {
                    fadeForm.Stop();
                    fadeForm.Dispose();
                }

                const int duration = 5000;
                const int steps = 500;

                fadeForm = new System.Windows.Forms.Timer();
                fadeForm.Interval = duration / steps;

                int currentStep = 0;

                fadeForm.Tick += (arg1, arg2) =>
                {
                    this.Opacity = 1 - (((double)currentStep) / steps);
                    currentStep++;

                    if(currentStep >= steps)
                    {
                        fadeForm.Stop();
                        fadeForm.Dispose();
                    }
                };

                fadeForm.Start();
            }

            statusIndicator.BackColor = Color.Silver;

            clipboardContents.Text = "";
            clipboardImage.Visible = false;

            if(Clipboard.ContainsFileDropList())
            {
                clipboardType.Text = "FILE";

                StringCollection fileList = Clipboard.GetFileDropList();

                clipboardContents.Text = "";

                for(int fileIndex = 0; fileIndex < fileList.Count; fileIndex++)
                {
                    if(fileIndex > 0)
                    {
                        clipboardContents.Text += ", ";
                    }

                    clipboardContents.Text += fileList[fileIndex];
                }
            }
            else if(Clipboard.ContainsImage())
            {
                clipboardType.Text = "IMAGE";
                clipboardImage.Image = new Bitmap(Clipboard.GetImage());
                clipboardImage.Visible = true;
            }
            else if(Clipboard.ContainsText())
            {
                clipboardType.Text = "TEXT";
                clipboardContents.Text = Clipboard.GetText();
            }
            else
            {
                clipboardType.Text = "UNKNOWN";
                clipboardContents.Text = "";
            }
        }

        private void addMessage(int type, string message)
        {
            messageQueue.Add(new Message(type, message));

            if(messageQueue.Count == 1)
            {
                sendMessage(messageQueue[0].type, messageQueue[0].message);
            }
        }

        private void sendFolder(string folder, string directory)
        {
            if(Directory.Exists(folder))
            {
                string[] fileList = Directory.GetFiles(folder);
                string[] folderList = Directory.GetDirectories(folder);

                foreach(string fileName in fileList)
                {
                    addMessage(Messages.FILE, fileName.Replace(directory, "") + "|" + File.ReadAllText(fileName));
                }

                foreach(string folderName in folderList)
                {
                    addMessage(Messages.FOLDER, folderName.Replace(directory, ""));
                    sendFolder(folderName, directory);
                }
            }
        }

        private Image GetImageFromClipboard()
        {
            if (Clipboard.GetDataObject() == null) return null;
            if (Clipboard.GetDataObject().GetDataPresent(DataFormats.Dib))
            {
                var dib = ((System.IO.MemoryStream)Clipboard.GetData(DataFormats.Dib)).ToArray();
                var width = BitConverter.ToInt32(dib, 4);
                var height = BitConverter.ToInt32(dib, 8);
                var bpp = BitConverter.ToInt16(dib, 14);
                if (bpp == 32)
                {
                    var gch = GCHandle.Alloc(dib, GCHandleType.Pinned);
                    Bitmap bmp = null;
                    try
                    {
                        var ptr = new IntPtr((long)gch.AddrOfPinnedObject() + 40);
                        bmp = new Bitmap(width, height, width * 4, System.Drawing.Imaging.PixelFormat.Format32bppArgb, ptr);
                        bmp.RotateFlip(RotateFlipType.Rotate180FlipX);
                        return new Bitmap(bmp);
                    }
                    finally
                    {
                        gch.Free();
                        if (bmp != null) bmp.Dispose();
                    }
                }
            }
            return Clipboard.ContainsImage() ? Clipboard.GetImage() : null;
        }

        private void sendButton_Click(object sender, EventArgs e)
        {
            if(Clipboard.ContainsFileDropList())
            {
                StringCollection fileList = Clipboard.GetFileDropList();

                for (int fileIndex = 0; fileIndex < fileList.Count; fileIndex++)
                {
                    string directory = Path.GetDirectoryName(fileList[fileIndex]) + "\\";

                    if (File.GetAttributes(fileList[fileIndex]).HasFlag(FileAttributes.Directory)) // Check if file is a folder
                    {
                        addMessage(Messages.FOLDER, fileList[fileIndex].Replace(directory, ""));
                        sendFolder(fileList[fileIndex], directory);
                    }
                    else
                    {
                        addMessage(Messages.FILE, fileList[fileIndex].Replace(directory, "") + "|" + File.ReadAllText(fileList[fileIndex]));
                    }
                }
            }
            else if(Clipboard.ContainsImage())
            {
                Image image = GetImageFromClipboard(); // Fixes Chrome transparency issue

                MemoryStream memoryStream = new MemoryStream();
                image.Save(memoryStream, ImageFormat.Png);

                byte[] imageBytes = memoryStream.ToArray();

                sendMessage(Messages.IMAGE, Convert.ToBase64String(imageBytes));
            }
            else if(Clipboard.ContainsText())
            {
                sendMessage(Messages.TEXT, Clipboard.GetText());
            }

            statusIndicator.BackColor = Color.FromArgb(56, 168, 82);
        }

        //[STAThread]
        private void onReceiveServerMessage(int type, string message)
        {
            string saveDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\";

            if (type != Messages.RECEIVED)
            {
                sendMessage(Messages.RECEIVED);
            }

            switch (type)
            {
                case Messages.RECEIVED:
                    {
                        if(formClosing)
                        {
                            Application.Exit();
                            Environment.Exit(Environment.ExitCode);
                            return;
                        }

                        messageQueue.RemoveAt(0);

                        if (messageQueue.Count > 0)
                        {
                            sendMessage(messageQueue[0].type, messageQueue[0].message);
                        }

                        break;
                    }

                case Messages.FILE:
                    {
                        string[] fileInfo = message.Split(new char[] { '|' }, 2);

                        using (StreamWriter streamWriter = File.CreateText(saveDirectory + fileInfo[0]))
                        {
                            streamWriter.Write(fileInfo[1]);
                        }

                        break;
                    }

                case Messages.FOLDER:
                    {
                        Directory.CreateDirectory(saveDirectory + message);
                        break;
                    }

                case Messages.IMAGE:
                    {
                        byte[] imageBytes = Convert.FromBase64String(message);
                        MemoryStream memoryStream = new MemoryStream(imageBytes);

                        Image image = Image.FromStream(memoryStream);
                        image.Save(saveDirectory + "rc_" + (Int32)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds + ".png");
                        break;
                    }

                case Messages.TEXT:
                    {
                        clipboardSet = true;

                        Thread thread = new Thread(() => Clipboard.SetText(message, TextDataFormat.Text));
                        thread.SetApartmentState(ApartmentState.STA);
                        thread.Start();
                        thread.Join();
                        break;
                    }
            }
        }

        private void sendMessage(int type, string message = "")
        {
            if (clientSocket.Connected)
            {
                NetworkStream serverStream = clientSocket.GetStream();
                byte[] outStream = Encoding.ASCII.GetBytes(type + "|" + message + "$");
                serverStream.Write(outStream, 0, outStream.Length);
            }
        }

        // Internal

        public Form1()
        {
            InitializeComponent();
            nextClipboardViewer = (IntPtr)SetClipboardViewer((int)this.Handle);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.TopMost = true;
            this.Opacity = 0;
            this.Location = new Point(Screen.PrimaryScreen.Bounds.Width - (10 + this.Size.Width), 10);

            try
            {
                clientSocket.Connect(SERVER_IP, SERVER_PORT);

                Thread clientThread = new Thread(doChat);
                clientThread.Start();
            }
            catch
            {
                MessageBox.Show("The server is currently offline. Please try again later.");
                Application.Exit();
                Environment.Exit(Environment.ExitCode);
                return;
            }
        }

        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            // defined in winuser.h
            const int WM_DRAWCLIPBOARD = 0x308;
            const int WM_CHANGECBCHAIN = 0x030D;

            switch (m.Msg)
            {
                case WM_DRAWCLIPBOARD:
                    {
                        if (clipboardSet)
                        {
                            clipboardSet = false;
                        }
                        else
                        {
                            OnClipboardChanged();
                        }

                        SendMessage(nextClipboardViewer, m.Msg, m.WParam, m.LParam);
                        break;
                    }

                case WM_CHANGECBCHAIN:
                    {
                        if (m.WParam == nextClipboardViewer)
                            nextClipboardViewer = m.LParam;
                        else
                            SendMessage(nextClipboardViewer, m.Msg, m.WParam, m.LParam);
                        break;
                    }

                default:
                    {
                        base.WndProc(ref m);
                        break;
                    }
            }
        }

        private void doChat()
        {
            NetworkStream serverStream = clientSocket.GetStream();
            string dataFromServer = null;

            byte[] bytesFrom = new byte[2000000];

            while (clientSocket.Connected)
            {
                try
                {
                    serverStream.Read(bytesFrom, 0, 2000000);

                    dataFromServer = Encoding.ASCII.GetString(bytesFrom);
                    dataFromServer = dataFromServer.Substring(0, dataFromServer.IndexOf("$"));

                    // Received message (dataFromServer)

                    string[] dataSplit = dataFromServer.Split(new char[] { '|' }, 2);

                    onReceiveServerMessage(Convert.ToInt32(dataSplit[0]), dataSplit[1]);
                }
                catch { }
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!formClosing)
            {
                messageQueue.Clear();
                sendMessage(-1, ";disconnect");
                formClosing = true;
                e.Cancel = true;
            }
        }
    }
}

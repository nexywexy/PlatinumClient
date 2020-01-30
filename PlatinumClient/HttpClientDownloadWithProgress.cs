namespace PlatinumClient
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Net.Http;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;

    public class HttpClientDownloadWithProgress : IDisposable
    {
        private readonly string _downloadUrl;
        private readonly string _destinationFilePath;
        private HttpClient _httpClient;

        [field: CompilerGenerated]
        public event ProgressChangedHandler ProgressChanged;

        public HttpClientDownloadWithProgress(string downloadUrl, string destinationFilePath)
        {
            this._downloadUrl = downloadUrl;
            this._destinationFilePath = destinationFilePath;
        }

        public void Dispose()
        {
            if (this._httpClient != null)
            {
                this._httpClient.Dispose();
            }
            else
            {
                HttpClient expressionStack_C_0 = this._httpClient;
            }
        }

        [AsyncStateMachine(typeof(<DownloadFileFromHttpResponseMessage>d__9))]
        private Task DownloadFileFromHttpResponseMessage(HttpResponseMessage response)
        {
            <DownloadFileFromHttpResponseMessage>d__9 d__;
            d__.<>4__this = this;
            d__.response = response;
            d__.<>t__builder = AsyncTaskMethodBuilder.Create();
            d__.<>1__state = -1;
            d__.<>t__builder.Start<<DownloadFileFromHttpResponseMessage>d__9>(ref d__);
            return d__.<>t__builder.Task;
        }

        [AsyncStateMachine(typeof(<ProcessContentStream>d__10))]
        private Task ProcessContentStream(long? totalDownloadSize, Stream contentStream)
        {
            <ProcessContentStream>d__10 d__;
            d__.<>4__this = this;
            d__.totalDownloadSize = totalDownloadSize;
            d__.contentStream = contentStream;
            d__.<>t__builder = AsyncTaskMethodBuilder.Create();
            d__.<>1__state = -1;
            d__.<>t__builder.Start<<ProcessContentStream>d__10>(ref d__);
            return d__.<>t__builder.Task;
        }

        [AsyncStateMachine(typeof(<StartDownload>d__8))]
        public Task StartDownload()
        {
            <StartDownload>d__8 d__;
            d__.<>4__this = this;
            d__.<>t__builder = AsyncTaskMethodBuilder.Create();
            d__.<>1__state = -1;
            d__.<>t__builder.Start<<StartDownload>d__8>(ref d__);
            return d__.<>t__builder.Task;
        }

        private void TriggerProgressChanged(long? totalDownloadSize, long totalBytesRead)
        {
            if (this.ProgressChanged != null)
            {
                double? progressPercentage = null;
                if (totalDownloadSize.HasValue)
                {
                    progressPercentage = new double?(Math.Round((double) ((((double) totalBytesRead) / ((double) totalDownloadSize.Value)) * 100.0), 2));
                }
                this.ProgressChanged(totalDownloadSize, totalBytesRead, progressPercentage);
            }
        }

        [CompilerGenerated]
        private struct <DownloadFileFromHttpResponseMessage>d__9 : IAsyncStateMachine
        {
            public int <>1__state;
            public AsyncTaskMethodBuilder <>t__builder;
            public HttpResponseMessage response;
            public HttpClientDownloadWithProgress <>4__this;
            private long? <totalBytes>5__1;
            private Stream <contentStream>5__2;
            private TaskAwaiter<Stream> <>u__1;
            private TaskAwaiter <>u__2;

            private void MoveNext()
            {
                int num = this.<>1__state;
                try
                {
                    TaskAwaiter<Stream> awaiter;
                    if (num != 0)
                    {
                        if (num != 1)
                        {
                            this.response.EnsureSuccessStatusCode();
                            this.<totalBytes>5__1 = this.response.Content.Headers.ContentLength;
                            awaiter = this.response.Content.ReadAsStreamAsync().GetAwaiter();
                            if (!awaiter.IsCompleted)
                            {
                                this.<>1__state = num = 0;
                                this.<>u__1 = awaiter;
                                this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter<Stream>, HttpClientDownloadWithProgress.<DownloadFileFromHttpResponseMessage>d__9>(ref awaiter, ref this);
                                return;
                            }
                            goto Label_009C;
                        }
                        goto Label_00B3;
                    }
                    awaiter = this.<>u__1;
                    this.<>u__1 = new TaskAwaiter<Stream>();
                    this.<>1__state = num = -1;
                Label_009C:
                    Stream introduced5 = awaiter.GetResult();
                    awaiter = new TaskAwaiter<Stream>();
                    Stream stream = introduced5;
                    this.<contentStream>5__2 = stream;
                Label_00B3:;
                    try
                    {
                        TaskAwaiter awaiter2;
                        if (num != 1)
                        {
                            awaiter2 = this.<>4__this.ProcessContentStream(this.<totalBytes>5__1, this.<contentStream>5__2).GetAwaiter();
                            if (!awaiter2.IsCompleted)
                            {
                                this.<>1__state = num = 1;
                                this.<>u__2 = awaiter2;
                                this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter, HttpClientDownloadWithProgress.<DownloadFileFromHttpResponseMessage>d__9>(ref awaiter2, ref this);
                                return;
                            }
                        }
                        else
                        {
                            awaiter2 = this.<>u__2;
                            this.<>u__2 = new TaskAwaiter();
                            this.<>1__state = num = -1;
                        }
                        awaiter2.GetResult();
                        awaiter2 = new TaskAwaiter();
                    }
                    finally
                    {
                        if ((num < 0) && (this.<contentStream>5__2 != null))
                        {
                            this.<contentStream>5__2.Dispose();
                        }
                    }
                    this.<contentStream>5__2 = null;
                }
                catch (Exception exception)
                {
                    this.<>1__state = -2;
                    this.<>t__builder.SetException(exception);
                    return;
                }
                this.<>1__state = -2;
                this.<>t__builder.SetResult();
            }

            [DebuggerHidden]
            private void SetStateMachine(IAsyncStateMachine stateMachine)
            {
                this.<>t__builder.SetStateMachine(stateMachine);
            }
        }

        [CompilerGenerated]
        private struct <ProcessContentStream>d__10 : IAsyncStateMachine
        {
            public int <>1__state;
            public AsyncTaskMethodBuilder <>t__builder;
            public HttpClientDownloadWithProgress <>4__this;
            public Stream contentStream;
            private byte[] <buffer>5__1;
            public long? totalDownloadSize;
            private long <totalBytesRead>5__2;
            private FileStream <fileStream>5__3;
            private int <bytesRead>5__4;
            private long <readCount>5__5;
            private bool <isMoreToRead>5__6;
            private TaskAwaiter<int> <>u__1;
            private TaskAwaiter <>u__2;

            private void MoveNext()
            {
                int num = this.<>1__state;
                try
                {
                    if ((num != 0) && (num != 1))
                    {
                        this.<totalBytesRead>5__2 = 0L;
                        this.<readCount>5__5 = 0L;
                        this.<buffer>5__1 = new byte[0x2000];
                        this.<isMoreToRead>5__6 = true;
                        this.<fileStream>5__3 = new FileStream(this.<>4__this._destinationFilePath, FileMode.Create, FileAccess.Write, FileShare.None, 0x2000, true);
                    }
                    try
                    {
                        TaskAwaiter<int> awaiter;
                        switch (num)
                        {
                            case 0:
                                goto Label_00B7;

                            case 1:
                                goto Label_0165;
                        }
                    Label_0068:
                        awaiter = this.contentStream.ReadAsync(this.<buffer>5__1, 0, this.<buffer>5__1.Length).GetAwaiter();
                        if (awaiter.IsCompleted)
                        {
                            goto Label_00D3;
                        }
                        this.<>1__state = num = 0;
                        this.<>u__1 = awaiter;
                        this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter<int>, HttpClientDownloadWithProgress.<ProcessContentStream>d__10>(ref awaiter, ref this);
                        return;
                    Label_00B7:
                        awaiter = this.<>u__1;
                        this.<>u__1 = new TaskAwaiter<int>();
                        this.<>1__state = num = -1;
                    Label_00D3:
                        int introduced5 = awaiter.GetResult();
                        awaiter = new TaskAwaiter<int>();
                        int num2 = introduced5;
                        this.<bytesRead>5__4 = num2;
                        if (this.<bytesRead>5__4 == 0)
                        {
                            this.<isMoreToRead>5__6 = false;
                            this.<>4__this.TriggerProgressChanged(this.totalDownloadSize, this.<totalBytesRead>5__2);
                            goto Label_01D9;
                        }
                        TaskAwaiter awaiter2 = this.<fileStream>5__3.WriteAsync(this.<buffer>5__1, 0, this.<bytesRead>5__4).GetAwaiter();
                        if (awaiter2.IsCompleted)
                        {
                            goto Label_0181;
                        }
                        this.<>1__state = num = 1;
                        this.<>u__2 = awaiter2;
                        this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter, HttpClientDownloadWithProgress.<ProcessContentStream>d__10>(ref awaiter2, ref this);
                        return;
                    Label_0165:
                        awaiter2 = this.<>u__2;
                        this.<>u__2 = new TaskAwaiter();
                        this.<>1__state = num = -1;
                    Label_0181:
                        awaiter2.GetResult();
                        awaiter2 = new TaskAwaiter();
                        this.<totalBytesRead>5__2 += this.<bytesRead>5__4;
                        this.<readCount>5__5 += 1L;
                        if ((this.<readCount>5__5 % 100L) == 0)
                        {
                            this.<>4__this.TriggerProgressChanged(this.totalDownloadSize, this.<totalBytesRead>5__2);
                        }
                    Label_01D9:
                        if (this.<isMoreToRead>5__6)
                        {
                            goto Label_0068;
                        }
                    }
                    finally
                    {
                        if ((num < 0) && (this.<fileStream>5__3 != null))
                        {
                            this.<fileStream>5__3.Dispose();
                        }
                    }
                    this.<fileStream>5__3 = null;
                }
                catch (Exception exception)
                {
                    this.<>1__state = -2;
                    this.<>t__builder.SetException(exception);
                    return;
                }
                this.<>1__state = -2;
                this.<>t__builder.SetResult();
            }

            [DebuggerHidden]
            private void SetStateMachine(IAsyncStateMachine stateMachine)
            {
                this.<>t__builder.SetStateMachine(stateMachine);
            }
        }

        [CompilerGenerated]
        private struct <StartDownload>d__8 : IAsyncStateMachine
        {
            public int <>1__state;
            public AsyncTaskMethodBuilder <>t__builder;
            public HttpClientDownloadWithProgress <>4__this;
            private HttpResponseMessage <response>5__1;
            private TaskAwaiter<HttpResponseMessage> <>u__1;
            private TaskAwaiter <>u__2;

            private void MoveNext()
            {
                int num = this.<>1__state;
                try
                {
                    TaskAwaiter<HttpResponseMessage> awaiter;
                    if (num != 0)
                    {
                        if (num != 1)
                        {
                            HttpClient client1 = new HttpClient {
                                Timeout = TimeSpan.FromDays(1.0)
                            };
                            this.<>4__this._httpClient = client1;
                            awaiter = this.<>4__this._httpClient.GetAsync(this.<>4__this._downloadUrl, HttpCompletionOption.ResponseHeadersRead).GetAwaiter();
                            if (!awaiter.IsCompleted)
                            {
                                this.<>1__state = num = 0;
                                this.<>u__1 = awaiter;
                                this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter<HttpResponseMessage>, HttpClientDownloadWithProgress.<StartDownload>d__8>(ref awaiter, ref this);
                                return;
                            }
                            goto Label_00A5;
                        }
                        goto Label_00BC;
                    }
                    awaiter = this.<>u__1;
                    this.<>u__1 = new TaskAwaiter<HttpResponseMessage>();
                    this.<>1__state = num = -1;
                Label_00A5:
                    HttpResponseMessage introduced6 = awaiter.GetResult();
                    awaiter = new TaskAwaiter<HttpResponseMessage>();
                    HttpResponseMessage message = introduced6;
                    this.<response>5__1 = message;
                Label_00BC:;
                    try
                    {
                        TaskAwaiter awaiter2;
                        if (num != 1)
                        {
                            awaiter2 = this.<>4__this.DownloadFileFromHttpResponseMessage(this.<response>5__1).GetAwaiter();
                            if (!awaiter2.IsCompleted)
                            {
                                this.<>1__state = num = 1;
                                this.<>u__2 = awaiter2;
                                this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter, HttpClientDownloadWithProgress.<StartDownload>d__8>(ref awaiter2, ref this);
                                return;
                            }
                        }
                        else
                        {
                            awaiter2 = this.<>u__2;
                            this.<>u__2 = new TaskAwaiter();
                            this.<>1__state = num = -1;
                        }
                        awaiter2.GetResult();
                        awaiter2 = new TaskAwaiter();
                    }
                    finally
                    {
                        if ((num < 0) && (this.<response>5__1 != null))
                        {
                            this.<response>5__1.Dispose();
                        }
                    }
                    this.<response>5__1 = null;
                }
                catch (Exception exception)
                {
                    this.<>1__state = -2;
                    this.<>t__builder.SetException(exception);
                    return;
                }
                this.<>1__state = -2;
                this.<>t__builder.SetResult();
            }

            [DebuggerHidden]
            private void SetStateMachine(IAsyncStateMachine stateMachine)
            {
                this.<>t__builder.SetStateMachine(stateMachine);
            }
        }

        public delegate void ProgressChangedHandler(long? totalFileSize, long totalBytesDownloaded, double? progressPercentage);
    }
}


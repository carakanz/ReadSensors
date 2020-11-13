using Android.Media;
using Android.Views;
using Android.Hardware.Camera2;
using Android.Hardware.Camera2.Params;
using System;
using System.IO;
using Android.Hardware;
using Android.Content;

namespace ReadSensors
{
    public class CameraTest : IDisposable
    {
        private readonly MediaRecorder _recorder;

        public CameraTest()
        {
            _recorder = new MediaRecorder();
        }

        public void Init(Surface surface)
        {
            _recorder.Reset();
            _recorder.SetAudioSource(AudioSource.Mic);
            _recorder.SetVideoSource(VideoSource.Camera);

            _recorder.SetOutputFormat(OutputFormat.ThreeGpp);
            _recorder.SetAudioEncoder(AudioEncoder.Default);
            _recorder.SetVideoEncoder(VideoEncoder.H264);
            _recorder.SetVideoEncodingBitRate(30000000);
            _recorder.SetVideoSize(3840, 2160);
            _recorder.SetOrientationHint(90);
            
            _recorder.SetPreviewDisplay(surface);
        }

        public void Start(string path)
        {
            path = Path.Combine(GetDir(), path);

            _recorder.SetOutputFile(path);
            _recorder.Prepare();
            _recorder.Start();
        }

        public void Stop()
        {
            _recorder.Stop();
        }

        public void Dispose()
        {
            _recorder.Release();
        }

        public string GetDir()
        {
            return Android.OS.Environment
                .GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDocuments)
                .AbsolutePath;
        }
    }
}
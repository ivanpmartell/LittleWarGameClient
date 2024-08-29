using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;
using System.Diagnostics;
using System.Management;

namespace LittleWarGameClient.Handlers
{
    internal class AudioHandler : IAudioSessionEventsHandler
    {
        private MMDevice? mainDevice;
        private AudioSessionControl? currentSession;
        private readonly string formTitle;

        public AudioHandler(string formTitle)
        {
            this.formTitle = formTitle;
            var etor = new MMDeviceEnumerator();
            mainDevice = etor.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            if (mainDevice != null)
            {
                var sessions = mainDevice.AudioSessionManager.Sessions;
                for (int i = 0; i < sessions.Count; i++)
                {
                    var session = sessions[i];
                    ChangeTextAndIcon(session);
                }
                mainDevice.AudioSessionManager.OnSessionCreated += AudioSessionManager_OnSessionCreated;
            }
        }

        internal void ChangeVolume(float value)
        {
            if (currentSession != null)
                currentSession.SimpleAudioVolume.Volume = value;
        }
        internal void DestroySession()
        {
            var sess = currentSession;
            currentSession = null;
            UnregisterFromSession(sess);
            if (mainDevice != null)
            {
                mainDevice.AudioSessionManager.OnSessionCreated -= AudioSessionManager_OnSessionCreated;
                mainDevice.Dispose();
            }
        }

        private void UnregisterFromSession(AudioSessionControl? session)
        {
            if (session != null)
            {
                session.UnRegisterEventClient(this);
                session.Dispose();
            }
        }

        private void ChangeTextAndIcon(AudioSessionControl session)
        {
            if (session.IsSystemSoundsSession)
                return;

            var proc = Process.GetProcessById((int)session.GetProcessID);
            if (proc == null)
                return;

            var currentProc = Process.GetCurrentProcess();
            Process? parentProc = proc;
            bool isSubProc = false;
            while (parentProc != null && parentProc.Id != 0)
            {
                parentProc = GetParentProcess(parentProc);
                if (parentProc != null && parentProc.Id == currentProc.Id)
                {
                    isSubProc = true;
                    break;
                }
            }
            if (!isSubProc)
                return;


            session.DisplayName = formTitle;
            session.IconPath = GetType().Assembly.Location;
            currentSession = session;
            currentSession.RegisterEventClient(this);
        }

        private void AudioSessionManager_OnSessionCreated(object sender, IAudioSessionControl newSession)
        {
            AudioSessionControl managedControl = new AudioSessionControl(newSession);
            ChangeTextAndIcon(managedControl);
        }

        private Process? GetParentProcess(Process process)
        {
            try
            {
                using (var query = new ManagementObjectSearcher(
                  "SELECT * " +
                  "FROM Win32_Process " +
                  "WHERE ProcessId=" + process.Id))
                {
                    using (var collection = query.Get())
                    {
                        var mo = collection.OfType<ManagementObject>().FirstOrDefault();
                        if (mo != null)
                        {
                            using (mo)
                            {
                                var p = Process.GetProcessById((int)(uint)mo["ParentProcessId"]);
                                return p;
                            }
                        }

                    }
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }

        void IAudioSessionEventsHandler.OnVolumeChanged(float volume, bool isMuted) { }
        void IAudioSessionEventsHandler.OnDisplayNameChanged(string displayName) { }
        void IAudioSessionEventsHandler.OnChannelVolumeChanged(uint channelCount, nint newVolumes, uint channelIndex) { }
        void IAudioSessionEventsHandler.OnGroupingParamChanged(ref Guid groupingId) { }
        void IAudioSessionEventsHandler.OnIconPathChanged(string iconPath) { }

        void IAudioSessionEventsHandler.OnStateChanged(AudioSessionState state)
        {
            if (state == AudioSessionState.AudioSessionStateExpired)
            {
                ReleaseSessionDelayed(currentSession);
            }
        }

        void IAudioSessionEventsHandler.OnSessionDisconnected(AudioSessionDisconnectReason disconnectReason)
        {
            ReleaseSessionDelayed(currentSession);
        }

        private void ReleaseSessionDelayed(AudioSessionControl? session)
        {
            var timer = new System.Windows.Forms.Timer();
            void TimerTick(object? sender, EventArgs e)
            {
                timer.Stop();
                timer.Tick -= TimerTick;
                UnregisterFromSession(session);
            };
            timer.Interval = 100;
            timer.Tick += TimerTick;
        }
    }
}

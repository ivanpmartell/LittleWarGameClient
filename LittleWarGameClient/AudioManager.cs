using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace LittleWarGameClient
{
    internal class AudioManager : IAudioSessionEventsHandler
    {
        private MMDevice? mainDevice;
        private AudioSessionControl? currentSession;
        private readonly Form form;

        public AudioManager(Form form)
        {
            this.form = form;
            var etor = new MMDeviceEnumerator();
            this.mainDevice = etor.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
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
        internal void DestroySession()
        {
            var sess = this.currentSession;
            this.currentSession = null;
            this.UnregisterFromSession(sess);
            if (mainDevice != null)
            {
                mainDevice.AudioSessionManager.OnSessionCreated -= this.AudioSessionManager_OnSessionCreated;
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

            session.DisplayName = form.Text;
            session.IconPath = GetType().Assembly.Location;
            this.currentSession = session;
            this.currentSession.RegisterEventClient(this);
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
                this.ReleaseSessionDelayed(this.currentSession);
            }
        }

        void IAudioSessionEventsHandler.OnSessionDisconnected(AudioSessionDisconnectReason disconnectReason)
        {
            this.ReleaseSessionDelayed(this.currentSession);
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

using Cubase.Midi.Sync.Common.Midi;
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading;

namespace Cubase.Midi.Sync.Server.Services.Midi
{
    public class NutstoneDriver : IDisposable
    {
        private nint port;
        private GCHandle callbackHandle;
        private bool disposed;
        private bool isSuspended;   // track system sleep state
        private readonly object portLock = new();

        private const uint TE_VM_FLAGS_NONE = 0x0;
        private const uint TE_VM_FLAGS_WINMM_ONLY = 0x8;
        private const uint TE_VM_FLAGS_WINRT_ONLY = 0x10;

        public string Name { get; private set; }
        public event Action<byte[]>? MidiMessageReceived;

        #region Native Imports
        private delegate void MidiCallback(nint midiPort, nint midiData, uint length, nint userData);

        [DllImport("teVirtualMIDI.dll", CharSet = CharSet.Unicode, EntryPoint = "virtualMIDICreatePortEx2")]
        private static extern nint CreatePort(string portName, MidiCallback callback, nint userData, uint maxSysexLength, uint flags);

        [DllImport("teVirtualMIDI.dll", EntryPoint = "virtualMIDISendData")]
        private static extern void SendData(nint midiPort, byte[] midiData, uint length);

        [DllImport("teVirtualMIDI.dll", EntryPoint = "virtualMIDIClosePort")]
        private static extern void ClosePort(nint midiPort);
        #endregion

        public NutstoneDriver(string name, uint maxSysexLength = 65535)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            StartPort(name, maxSysexLength);
        }

        private void StartPort(string name, uint maxSysexLength = 65535)
        {
            lock (portLock)
            {
                MidiCallback callback = OnMidiData;
                callbackHandle = GCHandle.Alloc(callback); // prevent delegate GC

                port = CreatePort(
                    name,
                    callback,
                    nint.Zero,
                    maxSysexLength,
                    TE_VM_FLAGS_NONE);

                if (port == nint.Zero)
                    throw new InvalidOperationException("Failed to create virtual MIDI port.");
            }
        }

        public void RestartPort()
        {
            lock (portLock)
            {
                if (port != nint.Zero)
                {
                    try
                    {
                        ClosePort(port);
                    }
                    catch { /* ignore if already gone */ }
                    port = nint.Zero;
                }

                Thread.Sleep(1000); // give Cubase time to unregister internally

                // recreate same port name so Cubase auto reconnects
                StartPort(Name);
                SendMessage(MidiCommand.Ready, Name);
            }
        }

        private void DisposePort()
        {
            lock (portLock)
            {
                if (port != nint.Zero)
                {
                    try
                    {
                        // optional: send All Notes Off or cleanup before closing
                        Send(new byte[] { 0xB0, 0x7B, 0x00 });
                        ClosePort(port);
                    }
                    catch { /* ignore if already closed */ }

                    port = nint.Zero;
                }

                if (callbackHandle.IsAllocated)
                    callbackHandle.Free();
            }
        }

        private void OnMidiData(nint midiPort, nint midiData, uint length, nint userData)
        {
            var data = new byte[length];
            Marshal.Copy(midiData, data, 0, (int)length);
            MidiMessageReceived?.Invoke(data);
        }

        public void Send(params byte[] message)
        {
            if (port == nint.Zero)
                throw new ObjectDisposedException(nameof(NutstoneDriver));

            SendData(port, message, (uint)message.Length);
        }

        public void SendNoteOn(int channel, int note, int velocity)
        {
            byte status = (byte)(0x90 | (channel & 0x0F));
            Send(status, (byte)note, (byte)velocity);
        }

        public void SendNoteOff(int channel, int note, int velocity = 0)
        {
            byte status = (byte)(0x80 | (channel & 0x0F));
            Send(status, (byte)note, (byte)velocity);
        }

        public void SendMessage(MidiCommand command, object obj)
        {
            string json = JsonSerializer.Serialize(obj);
            byte[] cmdBytes = Encoding.UTF8.GetBytes(command.ToString());
            byte[] jsonBytes = Encoding.UTF8.GetBytes(json);

            byte[] sysex = new byte[2 + cmdBytes.Length + 1 + jsonBytes.Length + 1];
            sysex[0] = 0xF0;  // start
            sysex[1] = 0x7D;  // non-commercial ID
            Buffer.BlockCopy(cmdBytes, 0, sysex, 2, cmdBytes.Length);
            sysex[2 + cmdBytes.Length] = 0x00; // separator
            Buffer.BlockCopy(jsonBytes, 0, sysex, 3 + cmdBytes.Length, jsonBytes.Length);
            sysex[sysex.Length - 1] = 0xF7; // end

            Send(sysex);
        }

        public void Dispose()
        {
            if (disposed) return;
            disposed = true;
            DisposePort();
            GC.SuppressFinalize(this);
        }
    }
}

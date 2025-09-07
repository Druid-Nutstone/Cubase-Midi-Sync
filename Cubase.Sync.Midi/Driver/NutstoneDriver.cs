using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Cubase.Sync.Midi.Driver
{

    /// <summary>
    /// 
    /// </summary>
    public class NutstoneDriver : IDisposable
    {
        private IntPtr handle;
        private GCHandle callbackHandle;

        private const uint TE_VM_FLAGS_NONE = 0x0;
        private const uint TE_VM_FLAGS_WINMM_ONLY = 0x8;   // Only legacy WinMM
        private const uint TE_VM_FLAGS_WINRT_ONLY = 0x10;  // Only WinRT

        public string Name { get; }
        public event Action<byte[]>? MidiMessageReceived;

        #region Native Imports
        private delegate void MidiCallback(IntPtr midiPort, IntPtr midiData, uint length, IntPtr userData);

        [DllImport("teVirtualMIDI.dll", CharSet = CharSet.Unicode, EntryPoint = "virtualMIDICreatePortEx2")]
        private static extern IntPtr CreatePort(
            string portName,
            MidiCallback callback,
            IntPtr userData,
            uint maxSysexLength,
            uint flags);

        [DllImport("teVirtualMIDI.dll", EntryPoint = "virtualMIDISendData")]
        private static extern void SendData(IntPtr midiPort, byte[] midiData, uint length);

        [DllImport("teVirtualMIDI.dll", EntryPoint = "virtualMIDIClosePort")]
        private static extern void ClosePort(IntPtr midiPort);
        #endregion

        public NutstoneDriver(string name, uint maxSysexLength = 65535)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));

            MidiCallback callback = OnMidiData;
            callbackHandle = GCHandle.Alloc(callback); // prevent delegate GC

            handle = CreatePort(
                name, 
                callback, 
                IntPtr.Zero, 
                maxSysexLength,
                TE_VM_FLAGS_WINMM_ONLY);
            if (handle == IntPtr.Zero)
                throw new InvalidOperationException("Failed to create virtual MIDI port.");
        }

        private void OnMidiData(IntPtr midiPort, IntPtr midiData, uint length, IntPtr userData)
        {
            var data = new byte[length];
            Marshal.Copy(midiData, data, 0, (int)length);
            MidiMessageReceived?.Invoke(data);
        }

        public void Send(params byte[] message)
        {
            if (handle == IntPtr.Zero)
                throw new ObjectDisposedException(nameof(NutstoneDriver));

            SendData(handle, message, (uint)message.Length);
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

        public void Dispose()
        {
            if (handle != IntPtr.Zero)
            {
                ClosePort(handle);
                handle = IntPtr.Zero;
            }
            if (callbackHandle.IsAllocated)
                callbackHandle.Free();
            GC.SuppressFinalize(this);
        }
    }

}

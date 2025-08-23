using Cubase.Sync.Midi.Driver;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cubase.Sync.Midi.Midi
{
    public class MidiManager : IDisposable
    {
        private Thread midiThread;
        private bool running;
        private VirtualMidiPort commandOut;
        private VirtualMidiPort commandIn;

        private string midiInName;
        private string midiOutName;

        // Thread-safe dictionary to store last value per CC (Quick Control)
        private ConcurrentDictionary<int, int> qcValues = new ConcurrentDictionary<int, int>();

        // Optional: queue for all incoming messages if you want to process them
        private BlockingCollection<(int Channel, int CC, int Value)> incomingMessages
            = new BlockingCollection<(int, int, int)>();

        public MidiManager(string midiInName, string midiOutName)
        {
            this.midiInName = midiInName ?? throw new ArgumentNullException(nameof(midiInName));
            this.midiOutName = midiOutName ?? throw new ArgumentNullException(nameof(midiOutName));

            running = true;
            midiThread = new Thread(RunMidiLoop) { IsBackground = true };
            midiThread.Start();
        }

        private void RunMidiLoop()
        {
            // Open virtual MIDI ports
            commandOut = new VirtualMidiPort(midiOutName); // C# -> Cubase
            commandIn = new VirtualMidiPort(midiInName);   // Cubase -> C#

            commandIn.MidiMessageReceived += CommandIn_MidiMessageReceived;

            while (running)
            {
                Thread.Sleep(10); // Keep thread alive
            }

            commandOut.Dispose();
            commandIn.Dispose();
            incomingMessages.CompleteAdding();
        }

        private void CommandIn_MidiMessageReceived(byte[] msg)
        {
            if (msg.Length >= 3 && (msg[0] & 0xF0) == 0xB0)
            {
                int channel = (msg[0] & 0x0F) + 1; // MIDI channels 1-16
                int cc = msg[1];
                int value = msg[2];

                // Update QC dictionary
                qcValues[cc] = value;

                // Optional: keep a log of all messages
                incomingMessages.Add((channel, cc, value));
            }
        }

        /// <summary>
        /// Get the last received value for a Quick Control (0-127).
        /// Returns null if no value received yet.
        /// </summary>
        public int? GetQuickControlValue(int qcNumber)
        {
            if (qcValues.TryGetValue(qcNumber, out int value))
                return value;
            return null;
        }

        /// <summary>
        /// Send a MIDI message to Cubase
        /// </summary>
        public void SendMessage(byte[] message)
        {
            commandOut?.Send(message);
        }

        /// <summary>
        /// Wait for the next incoming message (optional)
        /// </summary>
        public (int Channel, int CC, int Value) WaitForMessage(CancellationToken token)
        {
            return incomingMessages.Take(token);
        }

        public void Stop()
        {
            running = false;
            midiThread.Join();
        }

        public void Dispose()
        {
            Stop();
            incomingMessages.Dispose();
        }
    }
}

using System.Net.Sockets;
using System.Text;

namespace eagle.tunnel.dotnet.core
{
    public class Tunnel
    {
        private Pipe pipeL2R; // pipe from Left socket to Right socket
        private Pipe pipeR2L; // pipe from Right socket to Left socket

        public object IsWaiting
        {
            get
            {
                return pipeL2R.IsWaiting;
            }
            set
            {
                pipeL2R.IsWaiting = value;
                pipeR2L.IsWaiting = value;
            }
        }

        public Socket SocketL
        {
            get
            {
                return pipeL2R.SocketFrom;
            }
            set
            {
                pipeL2R.SocketFrom = value;
                pipeR2L.SocketTo = value;
            }
        }

        public Socket SocketR

        {
            get
            {
                return pipeL2R.SocketTo;
            }
            set
            {
                pipeL2R.SocketTo = value;
                pipeR2L.SocketFrom = value;
            }
        }

        public bool EncryptL
        {
            get
            {
                return pipeL2R.EncryptFrom;
            }
            set
            {
                pipeL2R.EncryptFrom = value;
                pipeR2L.EncryptTo = value;
            }
        }

        public bool EncryptR
        {
            get
            {
                return pipeL2R.EncryptTo;
            }
            set
            {
                pipeL2R.EncryptTo = value;
                pipeR2L.EncryptFrom = value;
            }
        }

        public int BytesTransffered
        {
            get
            {
                return pipeL2R.BytesTransferred + pipeR2L.BytesTransferred;
            }
        }

        public bool IsFlowing
        {
            get
            {
                bool result;
                if (SocketL != null && SocketR != null)
                {
                    result = pipeL2R.IsRunning;
                    result = result && pipeR2L.IsRunning;
                    result = result && SocketL.Connected;
                    result = result && SocketR.Connected;
                }
                else
                {
                    result = false;
                }
                return result;
            }
        }

        public byte EncryptionKey
        {
            get
            {
                return pipeL2R.EncryptionKey;
            }
            set
            {
                pipeL2R.EncryptionKey = value;
                pipeR2L.EncryptionKey = value;
            }
        }

        public bool IsOpening { get; set; }

        public Tunnel (Socket socketl = null, Socket socketr = null, byte encryptionKey = 0)
        {
            pipeL2R = new Pipe (socketl, socketr, null, encryptionKey);
            pipeR2L = new Pipe (socketr, socketl, null, encryptionKey);
            IsOpening = true;
        }

        public void Restore (Socket left = null, Socket right = null, byte encryptionKey = 0)
        {
            pipeL2R.Restore (left, right, null, encryptionKey);
            pipeR2L.Restore (right, left, null, encryptionKey);
            IsOpening = true;
        }

        public void Release ()
        {
            pipeL2R.Release ();
            pipeR2L.Release ();
        }

        public void Flow ()
        {
            pipeL2R.Flow ();
            pipeR2L.Flow ();
            IsOpening = false;
        }

        public void Close ()
        {
            pipeL2R.Close ();
            pipeR2L.Close ();
            IsOpening = false;
        }

        public string ReadStringL ()
        {
            return pipeL2R.ReadString ();
        }

        public string ReadStringR ()
        {
            return pipeR2L.ReadString ();
        }

        public bool WriteL (string msg, Encoding code)
        {
            return pipeR2L.Write (msg, code) > 0;
        }

        public bool WriteL (string msg)
        {
            return WriteL (msg, Encoding.UTF8);
        }

        public bool WriteR (string msg, Encoding code)
        {
            return pipeL2R.Write (msg, code) > 0;
        }

        public bool WriteR (string msg)
        {
            return WriteR (msg, Encoding.UTF8);
        }

        public int ReadL (ByteBuffer buffer)
        {
            return pipeL2R.ReadByte (buffer);
        }

        public int ReadR (ByteBuffer buffer)
        {
            return pipeR2L.ReadByte (buffer);
        }
    }
}
using System.Net.Sockets;

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
                    result = SocketL.Connected;
                    result = SocketR.Connected && result;
                }
                else
                {
                    result = false;
                }
                return result;
            }
        }

        public bool IsOpening { get; set; }

        public Tunnel(Socket socketl = null, Socket socketr = null,byte encryptionKey = 0)
        {
            pipeL2R = new Pipe(socketl, socketr, null, encryptionKey);
            pipeR2L = new Pipe(socketr, socketl, null, encryptionKey);
            IsOpening = true;
        }

        public void Flow()
        {
            pipeL2R.Flow();
            pipeR2L.Flow();
            IsOpening = false;
        }

        public void Close()
        {
            if (SocketL != null)
            {
                if (SocketL.Connected)
                {
                    try
                    {
                        SocketL.Shutdown(SocketShutdown.Both);
                    }
                    catch {; }
                    System.Threading.Thread.Sleep(10);
                    SocketL.Close();
                }
            }
            if (SocketR != null)
            {
                if (SocketR.Connected)
                {
                    try
                    {
                        SocketR.Shutdown(SocketShutdown.Both);
                    }
                    catch {; }
                    System.Threading.Thread.Sleep(10);
                    SocketR.Close();
                }
            }
            IsOpening = false;
        }

        public string ReadStringL()
        {
            return pipeL2R.ReadString();
        }

        public string ReadStringR()
        {
            return pipeR2L.ReadString();
        }

        public bool WriteL(string msg)
        {
            return pipeR2L.Write(msg);
        }

        public bool WriteR(string msg)
        {
            return pipeL2R.Write(msg);
        }

        public byte[] ReadL()
        {
            return pipeL2R.ReadByte();
        }

        public byte[] ReadR()
        {
            return pipeR2L.ReadByte();
        }
    }
}
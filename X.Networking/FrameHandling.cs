using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace X.Networking
{
    internal class NetworkFrame : IEnumerable<byte>
    {
        #region Private Fields

        private byte[] _extPayloadLength;
        private Fin _fin;
        private Mask _mask;
        private byte[] _maskingKey;
        private Opcode _opcode;
        private PayloadData _payloadData;
        private byte _payloadLength;
        private Rsv _rsv1;
        private Rsv _rsv2;
        private Rsv _rsv3;

        #endregion

        #region Internal Fields

        /// <summary>
        /// Represents the ping frame without the payload data as an array of <see cref="byte"/>.
        /// </summary>
        /// <remarks>
        /// The value of this field is created from a non masked frame, so it can only be used to
        /// send a ping from a server.
        /// </remarks>
        internal static readonly byte[] EmptyPingBytes;

        #endregion

        #region Static Constructor

        static NetworkFrame()
        {
            EmptyPingBytes = CreatePingFrame(false).ToArray();
        }

        #endregion

        #region Private Constructors

        private NetworkFrame()
        {
        }

        #endregion

        #region Internal Constructors

        internal NetworkFrame(Opcode opcode, PayloadData payloadData, bool mask)
          : this(Fin.Final, opcode, payloadData, false, mask)
        {
        }

        internal NetworkFrame(Fin fin, Opcode opcode, byte[] data, bool compressed, bool mask)
          : this(fin, opcode, new PayloadData(data), compressed, mask)
        {
        }

        internal NetworkFrame(
          Fin fin, Opcode opcode, PayloadData payloadData, bool compressed, bool mask)
        {
            _fin = fin;
            _rsv1 = opcode.IsData() && compressed ? Rsv.On : Rsv.Off;
            _rsv2 = Rsv.Off;
            _rsv3 = Rsv.Off;
            _opcode = opcode;

            var len = payloadData.Length;
            if (len < 126)
            {
                _payloadLength = (byte)len;
                _extPayloadLength = Bytes.Empty;
            }
            else if (len < 0x010000)
            {
                _payloadLength = (byte)126;
                _extPayloadLength = ((ushort)len).InternalToByteArray(ByteOrder.Big);
            }
            else
            {
                _payloadLength = (byte)127;
                _extPayloadLength = len.InternalToByteArray(ByteOrder.Big);
            }

            if (mask)
            {
                _mask = Mask.On;
                _maskingKey = createMaskingKey();
                payloadData.Mask(_maskingKey);
            }
            else
            {
                _mask = Mask.Off;
                _maskingKey = Bytes.Empty;
            }

            _payloadData = payloadData;
        }

        #endregion

        #region Internal Properties

        internal int ExtendedPayloadLengthCount
        {
            get
            {
                return _payloadLength < 126 ? 0 : (_payloadLength == 126 ? 2 : 8);
            }
        }

        internal ulong FullPayloadLength
        {
            get
            {
                return _payloadLength < 126
                       ? _payloadLength
                       : _payloadLength == 126
                         ? _extPayloadLength.ToUInt16(ByteOrder.Big)
                         : _extPayloadLength.ToUInt64(ByteOrder.Big);
            }
        }

        #endregion

        #region Public Properties

        public byte[] ExtendedPayloadLength
        {
            get
            {
                return _extPayloadLength;
            }
        }

        public Fin Fin
        {
            get
            {
                return _fin;
            }
        }

        public bool IsBinary
        {
            get
            {
                return _opcode == Opcode.Binary;
            }
        }

        public bool IsClose
        {
            get
            {
                return _opcode == Opcode.Close;
            }
        }

        public bool IsCompressed
        {
            get
            {
                return _rsv1 == Rsv.On;
            }
        }

        public bool IsContinuation
        {
            get
            {
                return _opcode == Opcode.Cont;
            }
        }

        public bool IsControl
        {
            get
            {
                return _opcode >= Opcode.Close;
            }
        }

        public bool IsData
        {
            get
            {
                return _opcode == Opcode.Text || _opcode == Opcode.Binary;
            }
        }

        public bool IsFinal
        {
            get
            {
                return _fin == Fin.Final;
            }
        }

        public bool IsFragment
        {
            get
            {
                return _fin == Fin.More || _opcode == Opcode.Cont;
            }
        }

        public bool IsMasked
        {
            get
            {
                return _mask == Mask.On;
            }
        }

        public bool IsPing
        {
            get
            {
                return _opcode == Opcode.Ping;
            }
        }

        public bool IsPong
        {
            get
            {
                return _opcode == Opcode.Pong;
            }
        }

        public bool IsText
        {
            get
            {
                return _opcode == Opcode.Text;
            }
        }

        public ulong Length
        {
            get
            {
                return 2 + (ulong)(_extPayloadLength.Length + _maskingKey.Length) + _payloadData.Length;
            }
        }

        public Mask Mask
        {
            get
            {
                return _mask;
            }
        }

        public byte[] MaskingKey
        {
            get
            {
                return _maskingKey;
            }
        }

        public Opcode Opcode
        {
            get
            {
                return _opcode;
            }
        }

        public PayloadData PayloadData
        {
            get
            {
                return _payloadData;
            }
        }

        public byte PayloadLength
        {
            get
            {
                return _payloadLength;
            }
        }

        public Rsv Rsv1
        {
            get
            {
                return _rsv1;
            }
        }

        public Rsv Rsv2
        {
            get
            {
                return _rsv2;
            }
        }

        public Rsv Rsv3
        {
            get
            {
                return _rsv3;
            }
        }

        #endregion

        #region Private Methods
        internal static readonly RandomNumberGenerator RandomNumber = RandomNumberGenerator.Create();

        private static byte[] createMaskingKey()
        {
            var key = new byte[4];
            RandomNumber.GetBytes(key);

            return key;
        }

        private static string dump(NetworkFrame frame)
        {
            var len = frame.Length;
            var cnt = (long)(len / 4);
            var rem = (int)(len % 4);

            int cntDigit;
            string cntFmt;
            if (cnt < 10000)
            {
                cntDigit = 4;
                cntFmt = "{0,4}";
            }
            else if (cnt < 0x010000)
            {
                cntDigit = 4;
                cntFmt = "{0,4:X}";
            }
            else if (cnt < 0x0100000000)
            {
                cntDigit = 8;
                cntFmt = "{0,8:X}";
            }
            else
            {
                cntDigit = 16;
                cntFmt = "{0,16:X}";
            }

            var spFmt = String.Format("{{0,{0}}}", cntDigit);
            var headerFmt = String.Format(@"
{0} 01234567 89ABCDEF 01234567 89ABCDEF
{0}+--------+--------+--------+--------+\n", spFmt);
            var lineFmt = String.Format("{0}|{{1,8}} {{2,8}} {{3,8}} {{4,8}}|\n", cntFmt);
            var footerFmt = String.Format("{0}+--------+--------+--------+--------+", spFmt);

            var output = new StringBuilder(64);
            Func<Action<string, string, string, string>> linePrinter = () =>
            {
                long lineCnt = 0;
                return (arg1, arg2, arg3, arg4) =>
                  output.AppendFormat(lineFmt, ++lineCnt, arg1, arg2, arg3, arg4);
            };
            var printLine = linePrinter();

            output.AppendFormat(headerFmt, String.Empty);

            var bytes = frame.ToArray();
            for (long i = 0; i <= cnt; i++)
            {
                var j = i * 4;
                if (i < cnt)
                {
                    printLine(
                      Convert.ToString(bytes[j], 2).PadLeft(8, '0'),
                      Convert.ToString(bytes[j + 1], 2).PadLeft(8, '0'),
                      Convert.ToString(bytes[j + 2], 2).PadLeft(8, '0'),
                      Convert.ToString(bytes[j + 3], 2).PadLeft(8, '0'));

                    continue;
                }

                if (rem > 0)
                    printLine(
                      Convert.ToString(bytes[j], 2).PadLeft(8, '0'),
                      rem >= 2 ? Convert.ToString(bytes[j + 1], 2).PadLeft(8, '0') : String.Empty,
                      rem == 3 ? Convert.ToString(bytes[j + 2], 2).PadLeft(8, '0') : String.Empty,
                      String.Empty);
            }

            output.AppendFormat(footerFmt, String.Empty);
            return output.ToString();
        }

        private static string print(NetworkFrame frame)
        {
            // Payload Length
            var payloadLen = frame._payloadLength;

            // Extended Payload Length
            var extPayloadLen = payloadLen > 125 ? frame.FullPayloadLength.ToString() : String.Empty;

            // Masking Key
            var maskingKey = BitConverter.ToString(frame._maskingKey);

            // Payload Data
            var payload = payloadLen == 0
                          ? String.Empty
                          : payloadLen > 125
                            ? "---"
                            : frame.IsText && !(frame.IsFragment || frame.IsMasked || frame.IsCompressed)
                              ? frame._payloadData.ApplicationData.UTF8Decode()
                              : frame._payloadData.ToString();

            var fmt = @"
                    FIN: {0}
                   RSV1: {1}
                   RSV2: {2}
                   RSV3: {3}
                 Opcode: {4}
                   MASK: {5}
         Payload Length: {6}
Extended Payload Length: {7}
            Masking Key: {8}
           Payload Data: {9}";

            return String.Format(
              fmt,
              frame._fin,
              frame._rsv1,
              frame._rsv2,
              frame._rsv3,
              frame._opcode,
              frame._mask,
              payloadLen,
              extPayloadLen,
              maskingKey,
              payload);
        }

        private static NetworkFrame processHeader(byte[] header)
        {
            if (header.Length != 2)
                throw new FrameException("The header of a frame cannot be read from the stream.");

            // FIN
            var fin = (header[0] & 0x80) == 0x80 ? Fin.Final : Fin.More;

            // RSV1
            var rsv1 = (header[0] & 0x40) == 0x40 ? Rsv.On : Rsv.Off;

            // RSV2
            var rsv2 = (header[0] & 0x20) == 0x20 ? Rsv.On : Rsv.Off;

            // RSV3
            var rsv3 = (header[0] & 0x10) == 0x10 ? Rsv.On : Rsv.Off;

            // Opcode
            var opcode = (byte)(header[0] & 0x0f);

            // MASK
            var mask = (header[1] & 0x80) == 0x80 ? Mask.On : Mask.Off;

            // Payload Length
            var payloadLen = (byte)(header[1] & 0x7f);

            var err = !opcode.IsSupported()
                      ? "An unsupported opcode."
                      : !opcode.IsData() && rsv1 == Rsv.On
                        ? "A non data frame is compressed."
                        : opcode.IsControl() && fin == Fin.More
                          ? "A control frame is fragmented."
                          : opcode.IsControl() && payloadLen > 125
                            ? "A control frame has a long payload length."
                            : null;

            if (err != null)
                throw new FrameException(CloseStatusCode.ProtocolError, err);

            var frame = new NetworkFrame();
            frame._fin = fin;
            frame._rsv1 = rsv1;
            frame._rsv2 = rsv2;
            frame._rsv3 = rsv3;
            frame._opcode = (Opcode)opcode;
            frame._mask = mask;
            frame._payloadLength = payloadLen;

            return frame;
        }

        private static NetworkFrame readExtendedPayloadLength(Stream stream, NetworkFrame frame)
        {
            var len = frame.ExtendedPayloadLengthCount;
            if (len == 0)
            {
                frame._extPayloadLength = Bytes.Empty;
                return frame;
            }

            var bytes = stream.ReadBytes(len);
            if (bytes.Length != len)
                throw new FrameException(
                  "The extended payload length of a frame cannot be read from the stream.");

            frame._extPayloadLength = bytes;
            return frame;
        }

        private static void readExtendedPayloadLengthAsync(
          Stream stream,
          NetworkFrame frame,
          Action<NetworkFrame> completed,
          Action<Exception> error)
        {
            var len = frame.ExtendedPayloadLengthCount;
            if (len == 0)
            {
                frame._extPayloadLength = Bytes.Empty;
                completed(frame);

                return;
            }

            stream.ReadBytesAsync(
              len,
              bytes =>
              {
                  if (bytes.Length != len)
                      throw new FrameException(
                  "The extended payload length of a frame cannot be read from the stream.");

                  frame._extPayloadLength = bytes;
                  completed(frame);
              },
              error);
        }

        private static NetworkFrame readHeader(Stream stream)
        {
            return processHeader(stream.ReadBytes(2));
        }

        private static void readHeaderAsync(
          Stream stream, Action<NetworkFrame> completed, Action<Exception> error)
        {
            stream.ReadBytesAsync(2, bytes => completed(processHeader(bytes)), error);
        }

        private static NetworkFrame readMaskingKey(Stream stream, NetworkFrame frame)
        {
            var len = frame.IsMasked ? 4 : 0;
            if (len == 0)
            {
                frame._maskingKey = Bytes.Empty;
                return frame;
            }

            var bytes = stream.ReadBytes(len);
            if (bytes.Length != len)
                throw new FrameException("The masking key of a frame cannot be read from the stream.");

            frame._maskingKey = bytes;
            return frame;
        }

        private static void readMaskingKeyAsync(
          Stream stream,
          NetworkFrame frame,
          Action<NetworkFrame> completed,
          Action<Exception> error)
        {
            var len = frame.IsMasked ? 4 : 0;
            if (len == 0)
            {
                frame._maskingKey = Bytes.Empty;
                completed(frame);

                return;
            }

            stream.ReadBytesAsync(
              len,
              bytes =>
              {
                  if (bytes.Length != len)
                      throw new FrameException(
                  "The masking key of a frame cannot be read from the stream.");

                  frame._maskingKey = bytes;
                  completed(frame);
              },
              error);
        }

        private static NetworkFrame readPayloadData(Stream stream, NetworkFrame frame)
        {
            var len = frame.FullPayloadLength;
            if (len == 0)
            {
                frame._payloadData = PayloadData.Empty;
                return frame;
            }

            if (len > PayloadData.MaxLength)
                throw new FrameException(CloseStatusCode.TooBig, "A frame has a long payload length.");

            var llen = (long)len;
            var bytes = frame._payloadLength < 127
                        ? stream.ReadBytes((int)len)
                        : stream.ReadBytes(llen, 1024);

            //if (bytes.LongLength != llen)
            //    throw new FrameException(
            //      "The payload data of a frame cannot be read from the stream.");
            if (bytes.Length != llen)
                throw new FrameException(
                  "The payload data of a frame cannot be read from the stream.");

            frame._payloadData = new PayloadData(bytes, llen);
            return frame;
        }

        private static void readPayloadDataAsync(
          Stream stream,
          NetworkFrame frame,
          Action<NetworkFrame> completed,
          Action<Exception> error)
        {
            var len = frame.FullPayloadLength;
            if (len == 0)
            {
                frame._payloadData = PayloadData.Empty;
                completed(frame);

                return;
            }

            if (len > PayloadData.MaxLength)
                throw new FrameException(CloseStatusCode.TooBig, "A frame has a long payload length.");

            var llen = (long)len;
            Action<byte[]> compl = bytes =>
            {
                //if (bytes.LongLength != llen)
                //    throw new FrameException(
                //      "The payload data of a frame cannot be read from the stream.");
                if (bytes.Length != llen)
                    throw new FrameException(
                      "The payload data of a frame cannot be read from the stream.");

                frame._payloadData = new PayloadData(bytes, llen);
                completed(frame);
            };

            if (frame._payloadLength < 127)
            {
                stream.ReadBytesAsync((int)len, compl, error);
                return;
            }

            //  stream.ReadBytesAsync(llen, 1024, compl, error);
            stream.ReadBytesAsync((int)llen, compl, error);
        }

        #endregion

        #region Internal Methods

        internal static NetworkFrame CreateCloseFrame(
          PayloadData payloadData, bool mask
        )
        {
            return new NetworkFrame(
                     Fin.Final, Opcode.Close, payloadData, false, mask
                   );
        }

        internal static NetworkFrame CreatePingFrame(bool mask)
        {
            return new NetworkFrame(
                     Fin.Final, Opcode.Ping, PayloadData.Empty, false, mask
                   );
        }

        internal static NetworkFrame CreatePingFrame(byte[] data, bool mask)
        {
            return new NetworkFrame(
                     Fin.Final, Opcode.Ping, new PayloadData(data), false, mask
                   );
        }

        internal static NetworkFrame CreatePongFrame(
          PayloadData payloadData, bool mask
        )
        {
            return new NetworkFrame(
                     Fin.Final, Opcode.Pong, payloadData, false, mask
                   );
        }

        internal static NetworkFrame ReadFrame(Stream stream, bool unmask)
        {
            var frame = readHeader(stream);
            readExtendedPayloadLength(stream, frame);
            readMaskingKey(stream, frame);
            readPayloadData(stream, frame);

            if (unmask)
                frame.Unmask();

            return frame;
        }

        internal static void ReadFrameAsync(
          Stream stream,
          bool unmask,
          Action<NetworkFrame> completed,
          Action<Exception> error
        )
        {
            readHeaderAsync(
              stream,
              frame =>
                readExtendedPayloadLengthAsync(
                  stream,
                  frame,
                  frame1 =>
                    readMaskingKeyAsync(
                      stream,
                      frame1,
                      frame2 =>
                        readPayloadDataAsync(
                          stream,
                          frame2,
                          frame3 =>
                          {
                              if (unmask)
                                  frame3.Unmask();

                              completed(frame3);
                          },
                          error
                        ),
                      error
                    ),
                  error
                ),
              error
            );
        }

        internal void Unmask()
        {
            if (_mask == Mask.Off)
                return;

            _mask = Mask.Off;
            _payloadData.Mask(_maskingKey);
            _maskingKey = Bytes.Empty;
        }

        #endregion

        #region Public Methods

        public IEnumerator<byte> GetEnumerator()
        {
            foreach (var b in ToArray())
                yield return b;
        }

        public void Print(bool dumped)
        {
            Console.WriteLine(dumped ? dump(this) : print(this));
        }

        public string PrintToString(bool dumped)
        {
            return dumped ? dump(this) : print(this);
        }

        public byte[] ToArray()
        {
            using (var buff = new MemoryStream())
            {
                var header = (int)_fin;
                header = (header << 1) + (int)_rsv1;
                header = (header << 1) + (int)_rsv2;
                header = (header << 1) + (int)_rsv3;
                header = (header << 4) + (int)_opcode;
                header = (header << 1) + (int)_mask;
                header = (header << 7) + (int)_payloadLength;
                buff.Write(((ushort)header).InternalToByteArray(ByteOrder.Big), 0, 2);

                if (_payloadLength > 125)
                    buff.Write(_extPayloadLength, 0, _payloadLength == 126 ? 2 : 8);

                if (_mask == Mask.On)
                    buff.Write(_maskingKey, 0, 4);

                if (_payloadLength > 0)
                {
                    var bytes = _payloadData.ToArray();
                    if (_payloadLength < 127)
                        buff.Write(bytes, 0, bytes.Length);
                    else
                        buff.WriteBytes(bytes, 1024);
                }

            //    buff.Close();
                return buff.ToArray();
            }
        }

        public override string ToString()
        {
            return BitConverter.ToString(ToArray());
        }

        #endregion

        #region Explicit Interface Implementations

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
    static class Ext
    {
        internal static byte[] ReadBytes(this Stream stream, int length)
        {
            var buff = new byte[length];
            var offset = 0;
            try
            {
                var nread = 0;
                while (length > 0)
                {
                    nread = stream.Read(buff, offset, length);
                    if (nread == 0)
                        break;

                    offset += nread;
                    length -= nread;
                }
            }
            catch
            {
            }

            return buff.SubArray(0, offset);
        }

        internal static byte[] ReadBytes(this Stream stream, long length, int bufferLength)
        {
            using (var dest = new MemoryStream())
            {
                try
                {
                    var buff = new byte[bufferLength];
                    var nread = 0;
                    while (length > 0)
                    {
                        if (length < bufferLength)
                            bufferLength = (int)length;

                        nread = stream.Read(buff, 0, bufferLength);
                        if (nread == 0)
                            break;

                        dest.Write(buff, 0, nread);
                        length -= nread;
                    }
                }
                catch
                {
                }

               // dest.Close();
                return dest.ToArray();
            }
        }

        private static readonly int _retry = 5;


        internal static void ReadBytesAsync(this Stream stream, int length, Action<byte[]> completed, Action<Exception> error)
        {
            var buffer = new byte[length];
            var offset = 0;
            var retry = 0;

            stream
                .ReadAsync(buffer, 0, length)
                .ContinueWith(x => {
                    var nread = x.Result;

                    if (nread == 0 && retry < _retry)
                    {
                        retry++;
                        // stream.ReadBytesAsync(buffer, offset, length, completed, null);

                        return;
                    }

                    if (nread == 0 || nread == length)
                    {
                        if (completed != null)
                            completed(buffer.SubArray(0, offset + nread));

                        return;
                    }

                    retry = 0;

                    offset += nread;
                    length -= nread;
                });


        }
        internal static void ReadBytesAsync(this Stream stream, byte[] buffer, int offset, int length, Action<byte[]> completed, Action<Exception> error)
        {
            var retry = 0;

            stream
                .ReadAsync(buffer, 0, length)
                .ContinueWith(x => {
                    var nread = x.Result;

                    if (nread == 0 && retry < _retry)
                    {
                        retry++;
                        stream.ReadBytesAsync(buffer, offset, length, completed, null);

                        return;
                    }

                    if (nread == 0 || nread == length)
                    {
                        if (completed != null)
                            completed(buffer.SubArray(0, offset + nread));

                        return;
                    }

                    retry = 0;

                    offset += nread;
                    length -= nread;
                });


        }

        //internal static void ReadBytesAsync(
        //  this Stream stream, int length, Action<byte[]> completed, Action<Exception> error
        //)
        //{
        //    var buff = new byte[length];
        //    var offset = 0;
        //    var retry = 0;

        //    AsyncCallback callback = null;
        //    callback =
        //      ar =>
        //      {
        //          try
        //          {
        //              var nread = stream.EndRead(ar);
        //              if (nread == 0 && retry < _retry)
        //              {
        //                  retry++;
        //                  stream.BeginRead(buff, offset, length, callback, null);

        //                  return;
        //              }

        //              if (nread == 0 || nread == length)
        //              {
        //                  if (completed != null)
        //                      completed(buff.SubArray(0, offset + nread));

        //                  return;
        //              }

        //              retry = 0;

        //              offset += nread;
        //              length -= nread;

        //              stream.BeginRead(buff, offset, length, callback, null);
        //          }
        //          catch (Exception ex)
        //          {
        //              if (error != null)
        //                  error(ex);
        //          }
        //      };

        //    try
        //    {
        //        stream.BeginRead(buff, offset, length, callback, null);
        //    }
        //    catch (Exception ex)
        //    {
        //        if (error != null)
        //            error(ex);
        //    }
        //}

        //internal static void ReadBytesAsync(
        //  this Stream stream,
        //  long length,
        //  int bufferLength,
        //  Action<byte[]> completed,
        //  Action<Exception> error
        //)
        //{
        //    var dest = new MemoryStream();
        //    var buff = new byte[bufferLength];
        //    var retry = 0;

        //    Action<long> read = null;
        //    read =
        //      len =>
        //      {
        //          if (len < bufferLength)
        //              bufferLength = (int)len;

        //          stream.BeginRead(
        //      buff,
        //      0,
        //      bufferLength,
        //      ar =>
        //      {
        //          try
        //          {
        //              var nread = stream.EndRead(ar);
        //              if (nread > 0)
        //                  dest.Write(buff, 0, nread);

        //              if (nread == 0 && retry < _retry)
        //              {
        //                  retry++;
        //                  read(len);

        //                  return;
        //              }

        //              if (nread == 0 || nread == len)
        //              {
        //                  if (completed != null)
        //                  {
        //                      dest.Close();
        //                      completed(dest.ToArray());
        //                  }

        //                  dest.Dispose();
        //                  return;
        //              }

        //              retry = 0;
        //              read(len - nread);
        //          }
        //          catch (Exception ex)
        //          {
        //              dest.Dispose();
        //              if (error != null)
        //                  error(ex);
        //          }
        //      },
        //      null
        //    );
        //      };

        //    try
        //    {
        //        read(length);
        //    }
        //    catch (Exception ex)
        //    {
        //        dest.Dispose();
        //        if (error != null)
        //            error(ex);
        //    }
        //}
        internal static bool IsControl(this byte opcode)
        {
            return opcode > 0x7 && opcode < 0x10;
        }

        internal static bool IsControl(this Opcode opcode)
        {
            return opcode >= Opcode.Close;
        }

        internal static bool IsData(this byte opcode)
        {
            return opcode == 0x1 || opcode == 0x2;
        }

        internal static bool IsData(this Opcode opcode)
        {
            return opcode == Opcode.Text || opcode == Opcode.Binary;
        }

        internal static bool IsPortNumber(this int value)
        {
            return value > 0 && value < 65536;
        }

        internal static bool IsReserved(this ushort code)
        {
            return code == 1004
                   || code == 1005
                   || code == 1006
                   || code == 1015;
        }

        internal static bool IsSupported(this byte opcode)
        {
            return Enum.IsDefined(typeof(Opcode), opcode);
        }
        public static bool Contains(this string value, params char[] chars)
        {
            return chars == null || chars.Length == 0
                   ? true
                   : value == null || value.Length == 0
                     ? false
                     : value.IndexOfAny(chars) > -1;
        }
        internal static void WriteBytes(this Stream stream, byte[] bytes, int bufferLength)
        {
            using (var input = new MemoryStream(bytes))
                input.CopyTo(stream, bufferLength);
        }
        internal static string GetMessage(this CloseStatusCode code)
        {
            return code == CloseStatusCode.ProtocolError
                   ? "A WebSocket protocol error has occurred."
                   : code == CloseStatusCode.UnsupportedData
                     ? "Unsupported data has been received."
                     : code == CloseStatusCode.Abnormal
                       ? "An exception has occurred."
                       : code == CloseStatusCode.InvalidData
                         ? "Invalid data has been received."
                         : code == CloseStatusCode.PolicyViolation
                           ? "A policy violation has occurred."
                           : code == CloseStatusCode.TooBig
                             ? "A too big message has been received."
                             : code == CloseStatusCode.MandatoryExtension
                               ? "WebSocket client didn't receive expected extension(s)."
                               : code == CloseStatusCode.ServerError
                                 ? "WebSocket server got an internal error."
                                 : code == CloseStatusCode.TlsHandshakeFailure
                                   ? "An error has occurred during a TLS handshake."
                                   : String.Empty;
        }


        internal static bool IsText(this string value)
        {
            var len = value.Length;
            for (var i = 0; i < len; i++)
            {
                var c = value[i];
                if (c < 0x20 && !"\r\n\t".Contains(c))
                    return false;

                if (c == 0x7f)
                    return false;

                if (c == '\n' && ++i < len)
                {
                    c = value[i];
                    if (!" \t".Contains(c))
                        return false;
                }
            }

            return true;
        }

        internal static string UTF8Decode(this byte[] bytes)
        {
            try
            {
                return Encoding.UTF8.GetString(bytes);
            }
            catch
            {
                return null;
            }
        }

        internal static byte[] UTF8Encode(this string s)
        {
            return Encoding.UTF8.GetBytes(s);
        }
        internal static byte[] Append(this ushort code, string reason)
        {
            var ret = code.InternalToByteArray(ByteOrder.Big);
            if (reason != null && reason.Length > 0)
            {
                var buff = new List<byte>(ret);
                buff.AddRange(Encoding.UTF8.GetBytes(reason));
                ret = buff.ToArray();
            }

            return ret;
        }
        internal static byte[] InternalToByteArray(this ushort value, ByteOrder order)
        {
            var bytes = BitConverter.GetBytes(value);
            if (!order.IsHostOrder())
                Array.Reverse(bytes);

            return bytes;
        }

        internal static byte[] InternalToByteArray(this ulong value, ByteOrder order)
        {
            var bytes = BitConverter.GetBytes(value);
            if (!order.IsHostOrder())
                Array.Reverse(bytes);

            return bytes;
        }
        internal static T[] Reverse<T>(this T[] array)
        {
            var len = array.Length;
            var reverse = new T[len];

            var end = len - 1;
            for (var i = 0; i <= end; i++)
                reverse[i] = array[end - i];

            return reverse;
        }
        public static bool IsHostOrder(this ByteOrder order)
        {
            // true: !(true ^ true) or !(false ^ false)
            // false: !(true ^ false) or !(false ^ true)
            return !(BitConverter.IsLittleEndian ^ (order == ByteOrder.Little));
        }
        internal static ushort ToUInt16(this byte[] source, ByteOrder sourceOrder)
        {
            return BitConverter.ToUInt16(source.ToHostOrder(sourceOrder), 0);
        }

        internal static ulong ToUInt64(this byte[] source, ByteOrder sourceOrder)
        {
            return BitConverter.ToUInt64(source.ToHostOrder(sourceOrder), 0);
        }
        public static byte[] ToHostOrder(this byte[] source, ByteOrder sourceOrder)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            return source.Length > 1 && !sourceOrder.IsHostOrder() ? source.Reverse() : source;
        }

        internal static byte[] ReadNBytes(this Stream stream, ulong length)
        {
            using (var dest = new MemoryStream())
            {
                try
                {
                    var bufferLength = 1024;
                    var buff = new byte[bufferLength];
                    var nread = 0;
                    while (length > 0)
                    {
                        if (length < (ulong)bufferLength)
                            bufferLength = (int)length;

                        nread = stream.Read(buff, 0, bufferLength);
                        if (nread == 0)
                            break;

                        dest.Write(buff, 0, nread);
                        length -= (ulong)nread;
                    }
                }
                catch
                {
                }

                return dest.ToArray();
            }
        }
        internal static void ReadNBytes(this Stream stream, byte[] targetArray, int targetPosition, int length)
        {
            var bytesRead = stream.Read(targetArray, targetPosition, length);
            if (bytesRead > 0)
            {
                if (bytesRead < length)
                {
                    targetPosition += bytesRead;
                    stream.ReadNBytes(targetArray, targetPosition, length - bytesRead);
                }
            }
            else throw new Exception("Incomplete data stream, connection broken");
        }


        //internal static byte[] ReadBytes(this Stream stream, int length)
        //{
        //    var buff = new byte[length];
        //    var offset = 0;
        //    try
        //    {
        //        var nread = 0;
        //        while (length > 0)
        //        {
        //            nread = stream.Read(buff, offset, length);
        //            if (nread == 0)
        //                break;

        //            offset += nread;
        //            length -= nread;
        //        }
        //    }
        //    catch
        //    {
        //    }

        //    return buff.SubArray(0, offset);
        //}

        public static T[] SubArray<T>(this T[] array, long startIndex, long length)
        {
            long len;
            // if (array == null || (len = array.LongLength) == 0)
            if (array == null || (len = array.Length) == 0)
                return new T[0];

            if (startIndex < 0 || length <= 0 || startIndex + length > len)
                return new T[0];

            if (startIndex == 0 && length == len)
                return array;

            var subArray = new T[length];

            // Array.Copy(array, startIndex, subArray, 0, length);
            Array.Copy(array, (int)startIndex, subArray, 0, (int) length);

            return subArray;
        }
        public static T[] SubArray<T>(this T[] array, int startIndex, int length)
        {
            long len;
            if (array == null || (len = array.Length) == 0)
                return new T[0];

            if (startIndex < 0 || length <= 0 || startIndex + length > len)
                return new T[0];

            if (startIndex == 0 && length == len)
                return array;

            var subArray = new T[length];
            Array.Copy(array, startIndex, subArray, 0, length);

            return subArray;
        }
    }
    internal enum ByteOrder
    {
        /// <summary>
        /// Specifies Little-endian.
        /// </summary>
        Little,
        /// <summary>
        /// Specifies Big-endian.
        /// </summary>
        Big
    }
    internal enum Opcode : byte
    {
        /// <summary>
        /// Equivalent to numeric value 0. Indicates continuation frame.
        /// </summary>
        Cont = 0x0,
        /// <summary>
        /// Equivalent to numeric value 1. Indicates text frame.
        /// </summary>
        Text = 0x1,
        /// <summary>
        /// Equivalent to numeric value 2. Indicates binary frame.
        /// </summary>
        Binary = 0x2,
        /// <summary>
        /// Equivalent to numeric value 8. Indicates connection close frame.
        /// </summary>
        Close = 0x8,
        /// <summary>
        /// Equivalent to numeric value 9. Indicates ping frame.
        /// </summary>
        Ping = 0x9,
        /// <summary>
        /// Equivalent to numeric value 10. Indicates pong frame.
        /// </summary>
        Pong = 0xa
    }
    internal enum Rsv : byte
    {
        /// <summary>
        /// Equivalent to numeric value 0. Indicates zero.
        /// </summary>
        Off = 0x0,
        /// <summary>
        /// Equivalent to numeric value 1. Indicates non-zero.
        /// </summary>
        On = 0x1
    }
    internal enum Mask : byte
    {
        /// <summary>
        /// Equivalent to numeric value 0. Indicates not masked.
        /// </summary>
        Off = 0x0,
        /// <summary>
        /// Equivalent to numeric value 1. Indicates masked.
        /// </summary>
        On = 0x1
    }
    internal enum Fin : byte
    {
        /// <summary>
        /// Equivalent to numeric value 0. Indicates more frames of a message follow.
        /// </summary>
        More = 0x0,
        /// <summary>
        /// Equivalent to numeric value 1. Indicates the final frame of a message.
        /// </summary>
        Final = 0x1
    }
    public static class Bytes
    {
        public static readonly byte[] Empty = new byte[0];
    }
    internal class PayloadData : IEnumerable<byte>
    {
        #region Private Fields

        private ushort _code;
        private bool _codeSet;
        private byte[] _data;
        private long _extDataLength;
        private long _length;
        private string _reason;
        private bool _reasonSet;

        #endregion

        #region Public Fields

        /// <summary>
        /// Represents the empty payload data.
        /// </summary>
        public static readonly PayloadData Empty;

        /// <summary>
        /// Represents the allowable max length.
        /// </summary>
        /// <remarks>
        ///   <para>
        ///   A <see cref="FrameException"/> will occur if the payload data length is
        ///   greater than the value of this field.
        ///   </para>
        ///   <para>
        ///   If you would like to change the value, you must set it to a value between
        ///   <c>WebSocket.FragmentLength</c> and <c>Int64.MaxValue</c> inclusive.
        ///   </para>
        /// </remarks>
        public static readonly ulong MaxLength;

        #endregion

        #region Static Constructor

        static PayloadData()
        {
            Empty = new PayloadData();
            MaxLength = Int64.MaxValue;
        }

        #endregion

        #region Internal Constructors

        internal PayloadData()
        {
            _code = 1005;
            _reason = String.Empty;

            _data = Bytes.Empty;

            _codeSet = true;
            _reasonSet = true;
        }

        //internal PayloadData(byte[] data)
        //  : this(data, data.LongLength)
        //{
        //}

        internal PayloadData(byte[] data)
          : this(data, data.Length)
        {
        }
        internal PayloadData(byte[] data, long length)
        {
            _data = data;
            _length = length;
        }

        internal PayloadData(ushort code, string reason)
        {
            _code = code;
            _reason = reason ?? String.Empty;

            _data = code.Append(reason);
            //  _length = _data.LongLength;
            _length = _data.Length;

            _codeSet = true;
            _reasonSet = true;
        }

        #endregion

        #region Internal Properties

        internal ushort Code
        {
            get
            {
                if (!_codeSet)
                {
                    _code = _length > 1
                            ? _data.SubArray(0, 2).ToUInt16(ByteOrder.Big)
                            : (ushort)1005;

                    _codeSet = true;
                }

                return _code;
            }
        }

        internal long ExtensionDataLength
        {
            get
            {
                return _extDataLength;
            }

            set
            {
                _extDataLength = value;
            }
        }

        internal bool HasReservedCode
        {
            get
            {
                return _length > 1 && Code.IsReserved();
            }
        }

        internal string Reason
        {
            get
            {
                if (!_reasonSet)
                {
                    _reason = _length > 2
                              ? _data.SubArray(2, _length - 2).UTF8Decode()
                              : String.Empty;

                    _reasonSet = true;
                }

                return _reason;
            }
        }

        #endregion

        #region Public Properties

        public byte[] ApplicationData
        {
            get
            {
                return _extDataLength > 0
                      ? _data.SubArray(_extDataLength, _length - _extDataLength)
                       : _data;
            }
        }

        public byte[] ExtensionData
        {
            get
            {
                return _extDataLength > 0
                       ? _data.SubArray(0, _extDataLength)
                       : Bytes.Empty;
            }
        }

        public ulong Length
        {
            get
            {
                return (ulong)_length;
            }
        }

        #endregion

        #region Internal Methods

        internal void Mask(byte[] key)
        {
            for (long i = 0; i < _length; i++)
                _data[i] = (byte)(_data[i] ^ key[i % 4]);
        }

        #endregion

        #region Public Methods

        public IEnumerator<byte> GetEnumerator()
        {
            foreach (var b in _data)
                yield return b;
        }

        public byte[] ToArray()
        {
            return _data;
        }

        public override string ToString()
        {
            return BitConverter.ToString(_data);
        }

        #endregion

        #region Explicit Interface Implementations

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
    public class FrameException: Exception
    {
        #region Private Fields

        private CloseStatusCode _code;

        #endregion

        #region Internal Constructors

        internal FrameException()
          : this(CloseStatusCode.Abnormal, null, null)
        {
        }

        internal FrameException(Exception innerException)
          : this(CloseStatusCode.Abnormal, null, innerException)
        {
        }

        internal FrameException(string message)
          : this(CloseStatusCode.Abnormal, message, null)
        {
        }

        internal FrameException(CloseStatusCode code)
          : this(code, null, null)
        {
        }

        internal FrameException(string message, Exception innerException)
          : this(CloseStatusCode.Abnormal, message, innerException)
        {
        }

        internal FrameException(CloseStatusCode code, Exception innerException)
          : this(code, null, innerException)
        {
        }

        internal FrameException(CloseStatusCode code, string message)
          : this(code, message, null)
        {
        }

        internal FrameException(
          CloseStatusCode code, string message, Exception innerException
        )
          : base(message ?? code.GetMessage(), innerException)
        {
            _code = code;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the status code indicating the cause of the exception.
        /// </summary>
        /// <value>
        /// One of the <see cref="CloseStatusCode"/> enum values that represents
        /// the status code indicating the cause of the exception.
        /// </value>
        public CloseStatusCode Code
        {
            get
            {
                return _code;
            }
        }

        #endregion
    }
    public enum CloseStatusCode : ushort
    {
        /// <summary>
        /// Equivalent to close status 1000. Indicates normal close.
        /// </summary>
        Normal = 1000,
        /// <summary>
        /// Equivalent to close status 1001. Indicates that an endpoint is
        /// going away.
        /// </summary>
        Away = 1001,
        /// <summary>
        /// Equivalent to close status 1002. Indicates that an endpoint is
        /// terminating the connection due to a protocol error.
        /// </summary>
        ProtocolError = 1002,
        /// <summary>
        /// Equivalent to close status 1003. Indicates that an endpoint is
        /// terminating the connection because it has received a type of
        /// data that it cannot accept.
        /// </summary>
        UnsupportedData = 1003,
        /// <summary>
        /// Equivalent to close status 1004. Still undefined. A Reserved value.
        /// </summary>
        Undefined = 1004,
        /// <summary>
        /// Equivalent to close status 1005. Indicates that no status code was
        /// actually present. A Reserved value.
        /// </summary>
        NoStatus = 1005,
        /// <summary>
        /// Equivalent to close status 1006. Indicates that the connection was
        /// closed abnormally. A Reserved value.
        /// </summary>
        Abnormal = 1006,
        /// <summary>
        /// Equivalent to close status 1007. Indicates that an endpoint is
        /// terminating the connection because it has received a message that
        /// contains data that is not consistent with the type of the message.
        /// </summary>
        InvalidData = 1007,
        /// <summary>
        /// Equivalent to close status 1008. Indicates that an endpoint is
        /// terminating the connection because it has received a message that
        /// violates its policy.
        /// </summary>
        PolicyViolation = 1008,
        /// <summary>
        /// Equivalent to close status 1009. Indicates that an endpoint is
        /// terminating the connection because it has received a message that
        /// is too big to process.
        /// </summary>
        TooBig = 1009,
        /// <summary>
        /// Equivalent to close status 1010. Indicates that a client is
        /// terminating the connection because it has expected the server to
        /// negotiate one or more extension, but the server did not return
        /// them in the handshake response.
        /// </summary>
        MandatoryExtension = 1010,
        /// <summary>
        /// Equivalent to close status 1011. Indicates that a server is
        /// terminating the connection because it has encountered an unexpected
        /// condition that prevented it from fulfilling the request.
        /// </summary>
        ServerError = 1011,
        /// <summary>
        /// Equivalent to close status 1015. Indicates that the connection was
        /// closed due to a failure to perform a TLS handshake. A Reserved value.
        /// </summary>
        TlsHandshakeFailure = 1015
    }
}

using System;
using System.Runtime.InteropServices;

namespace unit_test_sample_changer
{
	class CUart
	{
		#region Импортирование функций
		/**/
		[DllImport("kernel32.dll")]
		private static extern int CreateFile(
			string lpFileName,
			uint dwDesiredAccess,
			int dwShareMode,
			int lpSecurityAttributes,
			int dwCreationDisposition,
			int dwFlagsAndAttributes,
			int hTemplateFile
			);
		/**/
		[DllImport("kernel32.dll")]
		private static extern bool GetCommState(
			int hFile,		// дескриптор файла (порта)
			ref DCB lpDCB   // структура DCB
			);
		/**/
		[DllImport("kernel32.dll")]
		private static extern bool SetCommState(
			int hFile,		// дескриптор файла (порта)
			ref DCB lpDCB   // структура DCB
			);
		/**/
		[DllImport("kernel32.dll")]
		private static extern bool GetCommTimeouts(
			int hFile,		// дескриптор файла (порта)
			ref COMMTIMEOUTS lpCommTimeouts
			);
		/**/
		[DllImport("kernel32.dll")]
		private static extern bool SetCommTimeouts(
			int hFile,		// дескриптор файла (порта)
			ref COMMTIMEOUTS lpCommTimeouts
			);
		/**/
		[DllImport("kernel32.dll")]
		private static extern bool ReadFile(
			int hFile,					// дескриптор файла (порта)
			byte[] lpBuffer,            // буфер
			int nNumberOfBytesToRead,	// размер буфера
			ref int lpNumberOfBytesRead,// реально прочитано
			int lpOverlapped			// 0 для синхронных операций
		);
		/**/
		[DllImport("kernel32.dll")]
		private static extern bool WriteFile(
			int hFile,						// дескриптор файла (порта)
			byte[] lpBuffer,                // буфер данных
			int nNumberOfBytesToWrite,      // число байт данных
			ref int lpNumberOfBytesWritten, // реально переданное число байт
			int lpOverlapped				// 0 для синхронных операций
		);
		/**/
		[DllImport("kernel32.dll")]
		private static extern bool CloseHandle(
			int hObject   // дескриптор файла (порта)
		);
		/**/
		[DllImport("kernel32.dll")]
		private static extern bool PurgeComm(
			int hFile,
			int dwFlags
			);
		[StructLayout(LayoutKind.Sequential)]
		public struct DCB
		{

			/// <summary>
			/// Length of the structure, in bytes
			/// </summary>
			public uint DCBlength;

			/// <summary>
			/// Baud rate at which the communications device operates
			/// </summary>
			public uint BaudRate;

			/// <summary>
			/// Communication flags
			/// </summary>
			public uint Flags;

			/// <summary>
			/// Reserved; must be zero
			/// </summary>
			public ushort wReserved;

			/// <summary>
			/// Minimum number of bytes allowed in the input buffer before flow control is activated to inhibit the sender
			/// </summary>
			public ushort XonLim;

			/// <summary>
			/// Maximum number of bytes allowed in the input buffer before flow control is activated to allow 
			/// transmission by the sender
			/// </summary>
			public ushort XoffLim;

			/// <summary>
			/// Number of bits in the bytes transmitted and received
			/// </summary>
			public byte ByteSize;

			/// <summary>
			/// Parity scheme to be used
			/// </summary>
			public byte Parity;

			/// <summary>
			/// Number of stop bits to be used
			/// </summary>
			public byte StopBits;

			/// <summary>
			/// Value of the XON character for both transmission and reception
			/// </summary>
			public sbyte XonChar;

			/// <summary>
			/// Value of the XOFF character for both transmission and reception
			/// </summary>
			public sbyte XoffChar;

			/// <summary>
			/// Value of the character used to replace bytes received with a parity error
			/// </summary>
			public sbyte ErrorChar;

			/// <summary>
			/// Value of the character used to signal the end of data
			/// </summary>
			public sbyte EofChar;

			/// <summary>
			/// Value of the character used to signal an event
			/// </summary>
			public sbyte EvtChar;

			/// <summary>
			/// Reserved; do not use
			/// </summary>
			public ushort wReserved1;


			/// <summary>
			/// If this member is TRUE, binary mode is enabled
			/// </summary>
			public uint fBinary
			{
				get { return Flags & 0x0001; }
				set { Flags = Flags & ~1U | value; }
			}

			/// <summary>
			/// If this member is TRUE, parity checking is performed and errors are reported
			/// </summary>
			public uint fParity
			{
				get { return (Flags >> 1) & 1; }
				set { Flags = Flags & ~(1U << 1) | (value << 1); }
			}

			/// <summary>
			/// If this member is TRUE, the CTS (clear-to-send) signal is monitored for output flow control. 
			/// If this member is TRUE and CTS is turned off, output is suspended until CTS is sent again
			/// </summary>
			public uint fOutxCtsFlow
			{
				get { return (Flags >> 2) & 1; }
				set { Flags = Flags & ~(1U << 2) | (value << 2); }
			}

			/// <summary>
			/// If this member is TRUE, the DSR (data-set-ready) signal is monitored for output flow control. 
			/// If this member is TRUE and DSR is turned off, output is suspended until DSR is sent again
			/// </summary>
			public uint fOutxDsrFlow
			{
				get { return (Flags >> 3) & 1; }
				set { Flags = Flags & ~(1U << 3) | (value << 3); }
			}

			/// <summary>
			/// DTR (data-terminal-ready) flow control
			/// </summary>
			public uint fDtrControl
			{
				get { return (Flags >> 4) & 3; }
				set { Flags = Flags & ~(3U << 4) | (value << 4); }
			}

			/// <summary>
			/// If this member is TRUE, the communications driver is sensitive to the state of the DSR signal. 
			/// The driver ignores any bytes received, unless the DSR modem input line is high
			/// </summary>
			public uint fDsrSensitivity
			{
				get { return (Flags >> 6) & 1; }
				set { Flags = Flags & ~(1U << 6) | (value << 6); }
			}

			/// <summary>
			/// If this member is TRUE, transmission continues after the input buffer has come within XoffLim bytes of 
			/// being full and the driver has transmitted the XoffChar character to stop receiving bytes. 
			/// If this member is FALSE, transmission does not continue until the input buffer is within XonLim bytes 
			/// of being empty and the driver has transmitted the XonChar character to resume reception
			/// </summary>
			public uint fTXContinueOnXoff
			{
				get { return (Flags >> 7) & 1; }
				set { Flags = Flags & ~(1U << 7) | (value << 7); }
			}

			/// <summary>
			/// Indicates whether XON/XOFF flow control is used during transmission
			/// </summary>
			public uint fOutX
			{
				get { return (Flags >> 8) & 1; }
				set { Flags = Flags & ~(1U << 8) | (value << 8); }
			}

			/// <summary>
			/// Indicates whether XON/XOFF flow control is used during reception
			/// </summary>
			public uint fInX
			{
				get { return (Flags >> 9) & 1; }
				set { Flags = Flags & ~(1U << 9) | (value << 9); }
			}

			/// <summary>
			/// Indicates whether bytes received with parity errors are replaced with the character specified by the 
			/// ErrorChar member
			/// </summary>
			public uint fErrorChar
			{
				get { return (Flags >> 10) & 1; }
				set { Flags = Flags & ~(1U << 10) | (value << 10); }
			}

			/// <summary>
			/// If this member is TRUE, null bytes are discarded when received
			/// </summary>
			public uint fNull
			{
				get { return (Flags >> 11) & 1; }
				set { Flags = Flags & ~(1U << 11) | (value << 11); }
			}

			/// <summary>
			/// RTS (request-to-send) flow control
			/// </summary>
			public uint fRtsControl
			{
				get { return (Flags >> 12) & 3; }
				set { Flags = Flags & ~(3U << 12) | (value << 12); }
			}

			/// <summary>
			/// If this member is TRUE, the driver terminates all read and write operations with an error status 
			/// if an error occurs
			/// </summary>
			public uint fAbortOnError
			{
				get { return (Flags >> 14) & 1; }
				set { Flags = Flags & ~(1U << 14) | (value << 14); }
			}

		}

		[StructLayout(LayoutKind.Sequential)]
		public struct COMMTIMEOUTS
		{
			public uint ReadIntervalTimeout;
			public uint ReadTotalTimeoutMultiplier;
			public uint ReadTotalTimeoutConstant;
			public uint WriteTotalTimeoutMultiplier;
			public uint WriteTotalTimeoutConstant;
		};

		// Описание констант Win32 API
		private const uint GENERIC_READ = 0x80000000;
		private const uint GENERIC_WRITE = 0x40000000;
		private const int OPEN_EXISTING = 3;
		private const int INVALID_HANDLE_VALUE = -1;

		private enum Parity : byte
		{
			No = 0,
			Odd = 1,
			Even = 2,
			Mark = 3,
			Space = 4
		}

		private enum StopBits : byte
		{
			Bits1 = 0,
			Bits1_5 = 1,
			Bits2 = 2
		}

		private const int PURGE_TXABORT = 0x0001;
		private const int PURGE_RXABORT = 0x0002;
		private const int PURGE_TXCLEAR = 0x0004;
		private const int PURGE_RXCLEAR = 0x0008;

		private const int DTR_CONTROL_DISABLE = 0x00;
		private const int DTR_CONTROL_ENABLE = 0x01;
		private const int DTR_CONTROL_HANDSHAKE = 0x02;

		private const int RTS_CONTROL_DISABLE = 0x00;
		private const int RTS_CONTROL_ENABLE = 0x01;
		private const int RTS_CONTROL_HANDSHAKE = 0x02;
		private const int RTS_CONTROL_TOGGLE = 0x03;

		#endregion

		#region Destructor section
		private bool disposed = false;
		protected virtual void Dispose(bool disposing)
		{
			if (!disposed)
			{
				if (disposing)
				{
					if (IsOpen)
						Close();
				}
				disposed = true;
			}
		}

		public void Dispose()
		{
			Dispose(true);
		}
		#endregion

		private int m_hPortHanle = INVALID_HANDLE_VALUE;
		private bool m_GetIsOpen()
		{
			return (m_hPortHanle != INVALID_HANDLE_VALUE);
		}
		/// <summary>
		/// Is current port is open
		/// </summary>
		public bool IsOpen
		{
			get { return m_GetIsOpen(); }
		}

		public void Close()
		{
			if (IsOpen)
				CloseHandle(m_hPortHanle);
			m_hPortHanle = INVALID_HANDLE_VALUE;
		}

		private uint m_PortSpeed;
		public uint Speed
		{
			get { return m_PortSpeed; }
			set { m_PortSpeed = value; }
		}

		private string m_PortName;
		public string Name
		{
			get { return m_PortName; }
			set { m_PortName = value; }
		}
		#region Constructor
		/// <summary>
		/// Default constructor
		/// </summary>
		public CUart()
		{
			m_hPortHanle = INVALID_HANDLE_VALUE;
			m_PortName = "COM1";
			m_PortSpeed = 921600;
		}
		/// <summary>
		/// Overloaded constructor
		/// </summary>
		/// <param name="Name">Port name</param>
		/// <param name="Speed">Port speed</param>
		public CUart(string Name, uint Speed)
		{
			m_hPortHanle = INVALID_HANDLE_VALUE;
			m_PortName = Name;
			m_PortSpeed = Speed;
		}
		#endregion

		public void Reset()
		{
			if (m_hPortHanle != INVALID_HANDLE_VALUE)
				PurgeComm(m_hPortHanle, PURGE_TXABORT | PURGE_RXABORT | PURGE_TXCLEAR | PURGE_RXCLEAR);
		}

		public void Open()
		{
			if (m_hPortHanle != INVALID_HANDLE_VALUE)
				Close();
			/*"\\\\.\\" + */
			m_hPortHanle = CreateFile("\\\\.\\" + m_PortName,
																GENERIC_READ | GENERIC_WRITE,
																0,
																0,
																OPEN_EXISTING,
																0,
																0);
			if (m_hPortHanle == INVALID_HANDLE_VALUE)
				return;
			DCB dcb = new DCB();
			if (!GetCommState(m_hPortHanle, ref dcb))
			{
				Close();
				return;
			}
			dcb.BaudRate = this.m_PortSpeed;
			dcb.ByteSize = 8;
			dcb.Parity = (byte)Parity.No;
			dcb.StopBits = (byte)StopBits.Bits1;
			dcb.fDtrControl = DTR_CONTROL_DISABLE;
			dcb.fRtsControl = RTS_CONTROL_DISABLE;
			if (!SetCommState(m_hPortHanle, ref dcb))
			{
				Close();
				return;
			}
			COMMTIMEOUTS timeouts = new COMMTIMEOUTS();
			if (!GetCommTimeouts(m_hPortHanle, ref timeouts))
			{
				Close();
				return;
			}
			timeouts.ReadIntervalTimeout = 150;//15000;//
			timeouts.ReadTotalTimeoutConstant = 250;// 15000;//
			timeouts.ReadTotalTimeoutMultiplier = 5;// 5;
			timeouts.WriteTotalTimeoutConstant = 150;
			timeouts.WriteTotalTimeoutMultiplier = 5;
			if (!SetCommTimeouts(m_hPortHanle, ref timeouts))
			{
				Close();
				return;
			}
		}

		public enum MyTimeouts
		{
			ShortTimeouts,
			LongTimeouts
		}

		public bool ChangeTimeouts(MyTimeouts t)
		{
			COMMTIMEOUTS timeouts = new COMMTIMEOUTS();
			if (!GetCommTimeouts(m_hPortHanle, ref timeouts))
			{
				Close();
				return false;
			}
			if (t == MyTimeouts.ShortTimeouts)
			{
				timeouts.ReadIntervalTimeout = 150;
				timeouts.ReadTotalTimeoutConstant = 250;
				timeouts.ReadTotalTimeoutMultiplier = 5;
				timeouts.WriteTotalTimeoutConstant = 150;
				timeouts.WriteTotalTimeoutMultiplier = 5;
			}
			else if (t == MyTimeouts.LongTimeouts)
			{
				timeouts.ReadIntervalTimeout = 15000;
				timeouts.ReadTotalTimeoutConstant = 15000;
				timeouts.ReadTotalTimeoutMultiplier = 5;
				timeouts.WriteTotalTimeoutConstant = 150;
				timeouts.WriteTotalTimeoutMultiplier = 5;
			}
			if (!SetCommTimeouts(m_hPortHanle, ref timeouts))
			{
				Close();
				return false;
			}
			return true;
		}

		public bool Read(ref byte[] buffer, int offset, int length)
		{
			int readedBytes = 0;
			byte[] data = new byte[length];
			if (!ReadFile(m_hPortHanle, data, length, ref readedBytes, 0))
				return false;
			if (readedBytes != length)
				return false;
			Buffer.BlockCopy(data, 0, buffer, offset, length);
			return true;
		}

		public bool Write(byte[] buffer)
		{
			int writenBytes = 0;
			if (!WriteFile(m_hPortHanle, buffer, buffer.Length, ref writenBytes, 0))
				return false;
			if (writenBytes != buffer.Length)
				return false;
			return true;
		}
	}
}

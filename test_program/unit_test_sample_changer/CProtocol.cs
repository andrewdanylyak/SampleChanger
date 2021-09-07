using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace unit_test_sample_changer
{
	class CProtocol : IDisposable
	{
		CUart uart = null;

		#region IDisposable Support
		private bool disposedValue = false;

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					if (uart.IsOpen)
						uart.Close();
					uart.Dispose();
				}
				disposedValue = true;
			}
		}

		~CProtocol()
		{
			Dispose(false);
		}
		void IDisposable.Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		#endregion

		#region Constructor
		public CProtocol()
		{
			uart = new CUart();
			port_names = new List<string>();
		}
		#endregion

		#region Helper functions
		public static string DecodeString(byte[] data)
		{
			string result = Encoding.ASCII.GetString(data, 0, data.Length);//Encoding.ASCII.GetString(data);
			for (int i = 0; i < result.Length; i++)
			{
				if (result[i] == '\0')
				{
					result = result.Remove(i, result.Length - i);
					break;
				}
			}
			while ((result.Length > 0) && (result[result.Length - 1] == '\0'))
				result = result.Remove(result.Length - 1, 1);
			return result;
		}
		#endregion

		#region Check all ports in system
		List<string> port_names;
		public int AvaiblePorts
		{
			get { return port_names.Count; }
		}

		public void CheckPorts()
		{
			if (uart == null)
				return;
			port_names.Clear();
			for (int i = 0; i < 0xff; i++)
			{
				string str = "COM" + i.ToString()/* + ":"*/;
				uart.Name = str;
				uart.Speed = 921600;
				uart.Open();
				if (uart.IsOpen)
				{
					port_names.Add(str);
					uart.Close();
				}
			}
		}
		#endregion

		#region Protocol commands
		public enum ProtocolCommands
		{
			appRunBootloader = 0x5f,
			bootloaderGetId = 0x00ff,
			bootloaderGetChipSn = 0x0100,
			bootloaderGetApplicationStartAddress = 0x0101,
			bootloaderGetApplicationEndAddress = 0x0102,
			bootloaderReadFlash = 0x0103,
			bootloaderWriteFlash = 0x0104,
			bootloaderEraseSectors = 0x0105,
			bootloaderFlashUnlock = 0x0106,
			bootloaderFlashLock = 0x0107,
			bootloaderRunUserApp = 0x0108,
		}

		private enum ProtocolElvaxCommands
		{
			cmdElvaxGetSpectrum = 0x01,
			cmdElvaxClear,//0x02
			cmdElvaxStart,//0x03
			cmdElvaxStop,//0x04
			cmdElvaxGetTimers,//0x05
			cmdElvaxSetDAC,//0x06
			cmdElvaxGetADC,//0x07
			cmdElvaxGetLid,//0x08
			cmdElvaxSetTask,//0x09
			cmdElvaxPut,//0x0A   Set Filter position
			cmdElvaxSetCore,//0x0B
			cmdElvaxWriteSettings,//0x0C
			cmdElvaxReadSettings,//0x0D
			cmdElvaxAmplifier_ON,  // 0x0E  Sent by tuning programm to turn ON ADC,Amplifier
			cmdElvaxGetFirmwareVersion = 0x0F,//0x0F
			cmdElvaxChangeSpeed,//0x10
			cmdElvaxSetActiveSlot,//0x11
			cmdElvaxGetActiveSlot,//0x12
			cmdElvaxReadSettingsSlot,//0x13
			cmdElvaxWriteSettingsSlot,//0x14
			cmdElvaxStartTube,//0x15
			cmdElvaxStopTube,//0x16
			cmdElvaxStartFsa,//0x17
			cmdElvaxStopFsa,//0x18
			cmdElvaxGetDeviceState = 0x1A,//0x1A
			cmdElvaxGetBatteryState,//0x1B
			cmdElvaxSetUltimateTemperatures,//0x1C
			cmdElvaxGetExAdc,//0x1D
			cmdElvaxGetTubeTimers,//0x1E
			cmdElvaxGetEEpromBlockNumbers = 0x20,//0x20
			cmdElvaxReadEEpromBlock,//0x21
			cmdElvaxWriteEEpromBlock,//0x22
			cmdElvaxGetServiceTimers,//0x23
			cmdElvaxSetAutoStopCondition,//0x24
			cmdElvaxGetAutoStopStatus,//0x25
			cmdElvaxSetOsciloscopeMode,//0x26
			cmdElvaxGetFirmwareRevision,//0x27
			cmdElvaxCamera,//0x28
			cmdElvaxSetPpcChargeEnable = 0x30,//0x30
			cmdElvaxGetPpcChargeEnable,//0x31
			cmdElvaxSetBlockEnable,//0x32
			cmdElvaxGetBlockEnable,//0x33
			cmdElvaxBeforeDisconnect,//0x34
			cmdElvaxAfterDisconnect,//0x35
			cmdElvaxClearWorkingTime = 0x40,
			cmdElvaxGetWorkingTime,//0x41
			cmdElvaxSetAllowedWorkingTime,//0x42
			cmdElvaxConnect = 0xA5,
			cmdElvaxHVExtendedIo = 0xFB,
			cmdElvaxGetInternallVersion = 0xFD,
			cmdElvaxGetConsoleId = 0xFE,//0xFE
			cmdElvaxGetID = 0xFF,
			cmdElvaxSetCollimator = 0x60,
			cmdElvaxTurnOFF = 0x64,
			cmdElvaxPSGetStringId = 0x70,
			cmdElvaxPSHVEnablePowerPin,//0x71
			cmdElvaxPSHVI2CExCommand,//0x72
			cmdElvaxPSReadSettings,//0x73
			cmdElvaxPSWriteSettings,//0x74
			cmdElvaxPSReadHVId,//0x75
			cmdElvaXReadFloatTemperature = 0x80,
			cmdElvaXReadDppExtendedSettings,
			cmdElvaXWriteDppExtendedSettings,//0x82
			cmdGetAdcT4ValueCount = 0xB0,
			cmdGetAdcT4ValueInfo = 0xB2,
			cmdGetAdcT4Values = 0xB4,
			cmdSetDacT4 = 0xB5,
			cmdElvax2SetHandPosition = 0xBA,
			cmdElvax2GetSampleChangerState = 0xBB,
			cmdElvax2SetRotatorSpeed = 0xBE,
			cmdSetStabilizationParams = 0x50,
			cmdGetStabilizationResult = 0x51,
			cmdGetSequrityId = 0x52,
			cmdRunBootloader = 0x53,
		}
		#endregion

		#region Communication functions

		/// <summary>
		/// Calculate control sum from buffer
		/// </summary>
		/// <param name="buffer"></param>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <returns>control sum</returns>
		private byte CalculateCRC(byte[] buffer, uint from, uint to)
		{
			byte crc = 0;
			for (uint i = from; i < to; i++)
				crc = (byte)(crc + buffer[i]);
			crc = (byte)(0 - crc);
			return crc;
		}

		/// <summary>
		/// Send long packet command to device
		/// </summary>
		/// <param name="command"></param>
		/// <param name="buffer"></param>
		/// <returns>Boolean result</returns>
		private bool SendPacketLongCommands(ProtocolCommands command, byte[] buffer)
		{
			if (buffer == null)
				buffer = new byte[0];
			byte[] packet = new byte[buffer.Length + 6];
			packet[0] = 0xaa;
			packet[1] = (byte)((buffer.Length + 5) >> 0);
			packet[2] = (byte)((buffer.Length + 5) >> 8);
			packet[3] = (byte)((UInt16)command >> 0);
			packet[4] = (byte)((UInt16)command >> 8);
			Buffer.BlockCopy(buffer, 0, packet, 5, buffer.Length);
			byte checksum = 0;
			for (int i = 1; i < (buffer.Length + 5); i++)
				checksum += packet[i];
			packet[buffer.Length + 5] -= checksum;
			int buffer_length = 0;
			for (int i = 1; i < buffer.Length + 6; i++)
			{
				if ((packet[i] == 0xaa) || (packet[i] == 0x55))
					buffer_length++;
				buffer_length++;
			}
			buffer_length++;
			int transfer_length = buffer_length;
			byte[] new_buffer = new byte[transfer_length];
			buffer_length = 0;
			new_buffer[buffer_length++] = packet[0];
			for (int i = 1; i < packet.Length; i++)
			{
				switch (packet[i])
				{
					case 0xaa:
						new_buffer[buffer_length++] = 0x55;
						new_buffer[buffer_length++] = 0x01;
						break;
					case 0x55:
						new_buffer[buffer_length++] = 0x55;
						new_buffer[buffer_length++] = 0x00;
						break;
					default:
						new_buffer[buffer_length] = packet[i];
						buffer_length++;
						break;
				}
			}
			if (uart.IsOpen)
			{
				if (!uart.Write(new_buffer))
					return false;
			}
			return true;
		}

		private bool SendPacketShortCommands(ProtocolElvaxCommands command, byte[] buffer)
		{
			if (buffer == null)
				buffer = new byte[0];
			byte[] packet = new byte[buffer.Length + 4];
			packet[0] = 0xaa;
			packet[1] = (byte)((buffer.Length + 3) >> 0);
			packet[2] = (byte)((UInt16)command >> 0);
			Buffer.BlockCopy(buffer, 0, packet, 3, buffer.Length);
			byte checksum = 0;
			for (int i = 1; i < (buffer.Length + 3); i++)
				checksum += packet[i];
			packet[buffer.Length + 3] -= checksum;
			int buffer_length = 0;
			for (int i = 1; i < buffer.Length + 4; i++)
			{
				if ((packet[i] == 0xaa) || (packet[i] == 0x55))
					buffer_length++;
				buffer_length++;
			}
			buffer_length++;
			int transfer_length = buffer_length;
			byte[] new_buffer = new byte[transfer_length];
			buffer_length = 0;
			new_buffer[buffer_length++] = packet[0];
			for (int i = 1; i < packet.Length; i++)
			{
				switch (packet[i])
				{
					case 0xaa:
						new_buffer[buffer_length++] = 0x55;
						new_buffer[buffer_length++] = 0x01;
						break;
					case 0x55:
						new_buffer[buffer_length++] = 0x55;
						new_buffer[buffer_length++] = 0x00;
						break;
					default:
						new_buffer[buffer_length] = packet[i];
						buffer_length++;
						break;
				}
			}
			if (uart.IsOpen)
			{
				if (!uart.Write(new_buffer))
					return false;
			}
			return true;
		}

		/// <summary>
		/// Resive long packet from device
		/// </summary>
		/// <param name="command"></param>
		/// <param name="buffer"></param>
		/// <returns>Boolean result</returns>
		private bool RecivePacketLongCommands(ProtocolCommands command, ref byte[] buffer)
		{
			/**/
			if (buffer == null)
				buffer = new byte[0];
			byte[] recive_buffer = new byte[5000];
			int size_to_read = buffer.Length + 7;
			int ptr_to_read = 0;
			int i = 0;
			while (size_to_read != 0)
			{
				if (!uart.Read(ref recive_buffer, ptr_to_read, size_to_read))
					return false;
				int extrabytes = 0;
				for (i = ptr_to_read; i < ptr_to_read + size_to_read; i++)
					if (recive_buffer[i] == 0x55) extrabytes++;
				ptr_to_read += size_to_read;
				size_to_read = extrabytes;
			}
			byte[] output_buffer = new byte[2048];
			uint packet_size = 0;

			for (i = 0; i < ptr_to_read; i++)
			{
				if (recive_buffer[i] == 0x55)
				{
					i++;
					switch (recive_buffer[i])
					{
						case 0x00:
							output_buffer[packet_size++] = 0x55;
							break;
						case 0x01:
							output_buffer[packet_size++] = 0xAA;
							break;
					}
				}
				else output_buffer[packet_size++] = recive_buffer[i];
			}
			if (output_buffer[0] != 0xAA)
				return false;
			UInt16 size = BitConverter.ToUInt16(output_buffer, 1);
			if (size != buffer.Length + 6)
				return false;
			byte sts = output_buffer[3];
			if (sts != 0)
				return false;
			UInt16 cmd = BitConverter.ToUInt16(output_buffer, 4);
			if (cmd != (UInt16)command)
				return false;
			byte crc = CalculateCRC(output_buffer, 1, (uint)(packet_size - 1));
			if (crc != output_buffer[packet_size - 1])
				return false;
			if (buffer.Length != 0)
				Buffer.BlockCopy(output_buffer, 6, buffer, 0, size - 6);
			return true;
		}

		private bool RecivePacketShortCommands(ProtocolElvaxCommands command, ref byte[] buffer)
		{
			if (buffer == null)
				buffer = new byte[0];
			byte[] recive_buffer = new byte[5000];
			int size_to_read = buffer.Length + 5;
			int ptr_to_read = 0;
			int i = 0;
			while (size_to_read != 0)
			{
				if (!uart.Read(ref recive_buffer, ptr_to_read, size_to_read))
					return false;
				int extrabytes = 0;
				for (i = ptr_to_read; i < ptr_to_read + size_to_read; i++)
					if (recive_buffer[i] == 0x55) extrabytes++;
				ptr_to_read += size_to_read;
				size_to_read = extrabytes;
			}
			byte[] output_buffer = new byte[2048];
			uint packet_size = 0;

			for (i = 0; i < ptr_to_read; i++)
			{
				if (recive_buffer[i] == 0x55)
				{
					i++;
					switch (recive_buffer[i])
					{
						case 0x00:
							output_buffer[packet_size++] = 0x55;
							break;
						case 0x01:
							output_buffer[packet_size++] = 0xAA;
							break;
					}
				}
				else output_buffer[packet_size++] = recive_buffer[i];
			}
			if (output_buffer[0] != 0xAA)
				return false;
			UInt16 size = output_buffer[1];
			if (size != buffer.Length + 4)
				return false;
			byte sts = output_buffer[2];
			if (sts != 0)
				return false;
			UInt16 cmd = output_buffer[3];
			if (cmd != (UInt16)command)
				return false;
			byte crc = CalculateCRC(output_buffer, 1, (uint)(packet_size - 1));
			if (crc != output_buffer[packet_size - 1])
				return false;
			if (buffer.Length != 0)
				Buffer.BlockCopy(output_buffer, 4, buffer, 0, size - 4);
			return true;
		}

		/// <summary>
		/// Blocked function send and recive data from device
		/// </summary>
		/// <param name="command">2 bytes long command</param>
		/// <param name="tx_buffer">buffer to send</param>
		/// <param name="rx_buffer">buffer to recive</param>
		/// <param name="delay">delay between operations in ms</param>
		/// <returns>Boolean result</returns>
		private bool ExecuteLongCommand(ProtocolCommands command, byte[] tx_buffer, ref byte[] rx_buffer, int delay)
		{
			int num_try = 0;
			while (num_try < 10)
			{
				if (num_try > 0)
					uart.Reset();
				num_try++;
				if (!SendPacketLongCommands(command, tx_buffer))
					continue;
				if (delay != 0)
					System.Threading.Thread.Sleep(delay);
				if (!RecivePacketLongCommands(command, ref rx_buffer))
					continue;
				return true;
			}
			return false;
		}

		private bool ExecuteShortCommand(ProtocolElvaxCommands command, byte[] tx_buffer, ref byte[] rx_buffer, int delay)
		{
			int num_try = 0;
			while(num_try < 10)
			{
				if (num_try > 0)
					uart.Reset();
				num_try++;
			if (!SendPacketShortCommands(command, tx_buffer))
				continue;
			if (delay != 0)
				System.Threading.Thread.Sleep(delay);
			if (!RecivePacketShortCommands(command, ref rx_buffer))
				continue;
			return true;
			}
			return false;
		}
		#endregion

		#region Bootloader fuctions
		private enum BootloaderStatus
		{
			HAL_OK = 0x00,
			HAL_ERROR = 0x01,
			HAL_BUSY = 0x02,
			HAL_TIMEOUT = 0x03
		}

		private string dev_id = "";
		public string DeviceId
		{
			get { return dev_id; }
		}

		public bool BootloaderGetDeviceId()
		{
			byte[] rx_buffer = new byte[48];
			if (!ExecuteLongCommand(ProtocolCommands.bootloaderGetId, null, ref rx_buffer, 0))
				return false;
			dev_id = DecodeString(rx_buffer);
			return true;
		}

		public bool ElvaxGetDeviceId()
		{
			byte[] rx_buffer = new byte[24];
			if (!ExecuteShortCommand(ProtocolElvaxCommands.cmdElvaxGetID, null, ref rx_buffer, 0))
				return false;
			dev_id = DecodeString(rx_buffer);
			return true;
		}

		public bool IsConnected() {
			if (!uart.IsOpen)
				return false;
			return ElvaxGetDeviceId();
		}

		private string port_name = "";

		public string PortName
		{
			get { return port_name; }
		}

		public bool SearchDeviceBootloader()
		{
			CheckPorts();
			if (AvaiblePorts == 0)
				return false;
			for (int i = 0; i < AvaiblePorts; i++)
			{
				uart.Speed = 921600;
				uart.Name = port_names[i];
				uart.Open();
				if (BootloaderGetDeviceId())
				{
					port_name = uart.Name;
					return true;
				}
				uart.Close();
			}
			return false;
		}

		public bool SearchDeviceElvax()
		{
			CheckPorts();
			if (AvaiblePorts == 0)
				return false;
			for (int i = 0; i < AvaiblePorts; i++)
			{
				uart.Speed = 921600;
				uart.Name = port_names[i];
				uart.Open();
				uart.ChangeTimeouts(CUart.MyTimeouts.ShortTimeouts);
				if (ElvaxGetDeviceId())
				{
					port_name = uart.Name;
					return true;
				}
				uart.Close();
			}
			return false;
		}

		public void CloseConnection()
		{
			uart.Close();
		}

		private UInt32 chip_sn_uid0 = 0;
		private UInt32 chip_sn_uid1 = 0;
		private UInt32 chip_sn_uid2 = 0;
		private UInt32 chip_sn_uid3 = 0;

		public UInt32 ChipSerialNumberUID0
		{
			get { return chip_sn_uid0; }
		}

		public UInt32 ChipSerialNumberUID1
		{
			get { return chip_sn_uid1; }
		}

		public UInt32 ChipSerialNumberUID2
		{
			get { return chip_sn_uid2; }
		}

		public UInt32 ChipSerialNumberUID3
		{
			get { return chip_sn_uid3; }
		}

		public bool GetChipSerialNumberBootloader()
		{
			byte[] rx_buffer = new byte[4 * 4];
			if (!ExecuteLongCommand(ProtocolCommands.bootloaderGetChipSn, null, ref rx_buffer, 0))
				return false;
			chip_sn_uid0 = BitConverter.ToUInt32(rx_buffer, 0);
			chip_sn_uid1 = BitConverter.ToUInt32(rx_buffer, 4);
			chip_sn_uid2 = BitConverter.ToUInt32(rx_buffer, 8);
			chip_sn_uid3 = BitConverter.ToUInt32(rx_buffer, 12);
			return true;
		}

		private UInt32 app_address = 0;

		public UInt32 AppStartAddress
		{
			get { return app_address; }
		}

		public bool GetApplicationStartAddress()
		{
			byte[] rx_buffer = new byte[4];
			if (!ExecuteLongCommand(ProtocolCommands.bootloaderGetApplicationStartAddress, null, ref rx_buffer, 0))
				return false;
			app_address = BitConverter.ToUInt32(rx_buffer, 0);
			return true;
		}

		private UInt32 app_end_addr = 0;

		public UInt32 AppEndAddress
		{
			get { return app_end_addr; }
		}

		public bool GetApplicationEndAddress()
		{
			byte[] rx_buffer = new byte[4];
			if (!ExecuteLongCommand(ProtocolCommands.bootloaderGetApplicationEndAddress, null, ref rx_buffer, 0))
				return false;
			app_end_addr = BitConverter.ToUInt32(rx_buffer, 0);
			return true;
		}

		public bool GetApplicationDump(UInt32 address, UInt32 length, UInt32 offset, ref byte[] data)
		{
			byte[] tx_buffer = new byte[8];
			byte[] rx_buffer = new byte[length];
			byte[] temp = BitConverter.GetBytes(address);
			Buffer.BlockCopy(temp, 0, tx_buffer, 0, 4);
			temp = BitConverter.GetBytes(length);
			Buffer.BlockCopy(temp, 0, tx_buffer, 4, 4);
			if (!ExecuteLongCommand(ProtocolCommands.bootloaderReadFlash, tx_buffer, ref rx_buffer, 0))
			{
				return false;
			}
			Buffer.BlockCopy(rx_buffer, 0, data, (int)offset, (int)length);
			return true;
		}

		public bool GetApplicationDump(UInt32 address, UInt32 length, ref byte[] data)
		{
			byte[] tx_buffer = new byte[8];
			byte[] rx_buffer = new byte[length];
			byte[] temp = BitConverter.GetBytes(address);
			Buffer.BlockCopy(temp, 0, tx_buffer, 0, 4);
			temp = BitConverter.GetBytes(length);
			Buffer.BlockCopy(temp, 0, tx_buffer, 4, 4);
			if (!ExecuteLongCommand(ProtocolCommands.bootloaderReadFlash, tx_buffer, ref rx_buffer, 100))
			{
				return false;
			}
			Buffer.BlockCopy(rx_buffer, 0, data, 0, (int)length);
			return true;
		}

		public bool WriteApplicationFile(UInt32 address, UInt32 length, UInt32 offset, byte[] data)
		{
			byte[] tx_buffer = new byte[8 + length];
			byte[] rx_buffer = new byte[4];
			byte[] temp = BitConverter.GetBytes(address);
			Buffer.BlockCopy(temp, 0, tx_buffer, 0, 4);
			temp = BitConverter.GetBytes(length);
			Buffer.BlockCopy(temp, 0, tx_buffer, 4, 4);
			Buffer.BlockCopy(data, (int)offset, tx_buffer, 8, (int)length);
			if (!ExecuteLongCommand(ProtocolCommands.bootloaderWriteFlash, tx_buffer, ref rx_buffer, 0))
				return false;
			return true;
		}

		public bool WriteApplicationFile(UInt32 address, UInt32 length, byte[] data)
		{
			byte[] tx_buffer = new byte[8 + length];
			byte[] rx_buffer = new byte[4];
			byte[] temp = BitConverter.GetBytes(address);
			Buffer.BlockCopy(temp, 0, tx_buffer, 0, 4);
			temp = BitConverter.GetBytes(length);
			Buffer.BlockCopy(temp, 0, tx_buffer, 4, 4);
			Buffer.BlockCopy(data, 0, tx_buffer, 8, (int)length);
			if (!ExecuteLongCommand(ProtocolCommands.bootloaderWriteFlash, tx_buffer, ref rx_buffer, 0))
				return false;
			return true;
		}

		public bool EraseUserFlash()
		{
			byte[] rx_buffer = new byte[4];
			uart.ChangeTimeouts(CUart.MyTimeouts.LongTimeouts);
			if (!ExecuteLongCommand(ProtocolCommands.bootloaderEraseSectors, null, ref rx_buffer, 0))
			{
				uart.ChangeTimeouts(CUart.MyTimeouts.ShortTimeouts);
				return false;
			}
			uart.ChangeTimeouts(CUart.MyTimeouts.ShortTimeouts);
			return true;
		}

		public bool UnlockFlash()
		{
			byte[] rx_buffer = new byte[4];
			if (!ExecuteLongCommand(ProtocolCommands.bootloaderFlashUnlock, null, ref rx_buffer, 0))
				return false;
			return true;
		}

		public bool LockFlash()
		{
			byte[] rx_buffer = new byte[4];
			if (!ExecuteLongCommand(ProtocolCommands.bootloaderFlashLock, null, ref rx_buffer, 0))
				return false;
			return true;
		}

		public void StartUserApplication()
		{
			SendPacketLongCommands(ProtocolCommands.bootloaderRunUserApp, null);
		}

		public void StartBootloader()
		{
			SendPacketShortCommands(ProtocolElvaxCommands.cmdRunBootloader, null);
		}


		public bool StartAutomaticSearchBootloader()
		{
			if (SearchDeviceBootloader())
			{
				Console.WriteLine("Found device on\n:{0}\n:{1}", PortName, DeviceId);
				return true;
			}
			else if (SearchDeviceElvax())
			{
				Console.WriteLine("Found device on\n:{0}\n:{1}", PortName, DeviceId);
				Console.WriteLine("Run bootloader");
				this.StartBootloader();
				System.Threading.Thread.Sleep(50);
				if (SearchDeviceBootloader())
				{
					Console.WriteLine("Found device on\n:{0}\n:{1}", PortName, DeviceId);
					return true;
				}
			}
			return false;
		}
		#endregion

		public bool SetSpinner(bool en, byte pwm)
		{
			byte[] tx_buffer = new byte[2];
			tx_buffer[0] = (en == true)? (byte)0 : (byte)1;
			tx_buffer[1] = pwm;
			byte[] rx_buffer = new byte[0];
			if (!ExecuteShortCommand(ProtocolElvaxCommands.cmdElvax2SetHandPosition, tx_buffer, ref rx_buffer, 0))
				return false;
			return true;
		}

		public bool SetLift(bool en)
		{
			byte[] tx_buffer = new byte[1];
			tx_buffer[0] = (en == true) ? (byte)18 : (byte)19;
			byte[] rx_buffer = new byte[0];
			if (!ExecuteShortCommand(ProtocolElvaxCommands.cmdElvaxPut, tx_buffer, ref rx_buffer, 0))
				return false;
			return true;
		}

		public bool SetDiskPosition(byte pos)
		{
			byte[] tx_buffer = new byte[1] { pos };
			byte[] rx_buffer = new byte[0];
			if (!ExecuteShortCommand(ProtocolElvaxCommands.cmdElvaxPut, tx_buffer, ref rx_buffer, 0))
				return false;
			return true;
		}

		public bool GetDiskState(ref DeviceState deviceState)
		{
			byte[] rx_buffer = new byte[4];
			if (!ExecuteShortCommand(ProtocolElvaxCommands.cmdElvax2GetSampleChangerState, null, ref rx_buffer, 0))
				return false;
			deviceState.cell_state = (CellState)rx_buffer[0];
			deviceState.lift_state = (LiftState)rx_buffer[1];
			deviceState.disk_state = (DiskState)rx_buffer[2];
			return true;
		}
	}
}

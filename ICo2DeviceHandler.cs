using LibUsbDotNet;

namespace co2monitor {
	public interface ICo2DeviceHandler {
		//null or UsbDevice
		UsbDevice openDevice(int vendor, int productID);

		void setIfWhole(UsbDevice usbDevice);

		//device won't send anything before receiving this packet
		int sendReport(UsbDevice usbDevice, byte[] report);

		//0 or more bytes read, readBuffer changed
		int readData(UsbDevice usbDevice, out byte[] readBuffer);

		void closeDevice(UsbDevice usbDevice);

		void exit();
	}
}
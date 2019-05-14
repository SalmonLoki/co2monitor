using LibUsbDotNet;

namespace co2monitor {
	public interface ICo2Device {
		UsbDevice openDevice(int vendor, int productID);

		void setIfWhole(UsbDevice usbDevice);

		int readData(UsbDevice usbDevice, out byte[] readBuffer);

		void closeDevice(UsbDevice usbDevice);
	}
}
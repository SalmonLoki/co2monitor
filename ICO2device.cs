using LibUsbDotNet;

namespace co2monitor {
	public interface ICO2device {
		int init();
		
		void exit();
		
		UsbDevice openDevice();

		UsbDevice openPathDevice(string path);

		void closeDevice(UsbDevice device);
		
		int readData();
	}
}
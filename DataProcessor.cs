using System;

namespace co2monitor {
	public class DataProcessor : IDataProcessor {
		public bool decryptData(ref byte[] dataBuffer, out byte[] data) {
			data = dataBuffer;
			return true;
		}
		
		public bool checkEndOfMessage(ref byte[] data) {
			return data[4] == 0x0d;
		}
		
		public bool checkCheckSum(ref byte[] data) {
			return data[0] + data[1] + data[2] == data[3];
		}
		
		private double decodeTemperature(int t) {
			return t * 0.0625 - 273.15;
		}
				
		private void writeHeartbeat() {
			string curTimeLong = DateTime.Now.ToLongTimeString();
			Console.WriteLine(curTimeLong);
		}

		public void dataProcessing(ref byte[] data) {
			int Data = (data[1] << 8) + data[2];
			
			switch ((int)data[0]) {
				case 0x50d:					
					Console.WriteLine("Relative Concentration of CO2: " + Data);
					writeHeartbeat();
					break;	
				
				case 0x42d:					
					Console.WriteLine("Ambient Temperature: " + decodeTemperature(Data));
					writeHeartbeat();
					break;
			}
		}		
	}
}
using System;

namespace co2monitor {
	public class DataProcessor : IDataProcessor {
		public int[] decryptData(byte[] key, ref byte[] dataBuffer) {
			var cstate =
				new[] { 0x48, 0x74, 0x65, 0x6D, 0x70, 0x39, 0x39, 0x65 };
			var shuffle = new[] { 2, 4, 0, 7, 1, 6, 5, 3 };
			
			var phase1 = new[] { 0, 0, 0, 0, 0, 0, 0, 0 };
			for (var counter = 0; counter < shuffle.Length; counter++) {
				phase1[shuffle[counter]] = dataBuffer[counter];
			}
			
			var phase2 = new[] { 0, 0, 0, 0, 0, 0, 0, 0 };
			for (var i = 0; i < 8; i++) {
				phase2[i] = phase1[i] ^ key[i];
			}
			
			var phase3 = new[] { 0, 0, 0, 0, 0, 0, 0, 0 };
			for (var i = 0; i < 8; i++) {
				phase3[i] = ((phase2[i] >> 3) | (phase2[(i - 1 + 8) % 8] << 5)) & 0xff;
			}
			
			var tmp = new[] { 0, 0, 0, 0, 0, 0, 0, 0 };
			for (var i = 0; i < 8; i++) {
				tmp[i] = ((cstate[i] >> 4) | (cstate[i] << 4)) & 0xff;
			}
			
			var result = new[] { 0, 0, 0, 0, 0, 0, 0, 0 };
			for (var i = 0; i < 8; i++) {
				result[i] = (0x100 + phase3[i] - tmp[i]) & 0xff;
			}
			return result;
		}

		public bool checkEndOfMessage(ref int[] data) {
			return data[4] == 0x0d;
		}
		
		public bool checkCheckSum(ref int[] data) {
			return data[0] + data[1] + data[2] == data[3];
		}
		
		private double decodeTemperature(int t) {
			return t * 0.0625 - 273.15;
		}
				
		private void writeHeartbeat() {
			string curTimeLong = DateTime.Now.ToLongTimeString();
			Console.WriteLine(curTimeLong);
		}

		public void dataProcessing(ref int[] data) {
			int Data = (data[1] << 8) + data[2];
			
			switch (data[0]) {
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
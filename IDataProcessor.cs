namespace co2monitor {
	public interface IDataProcessor {
		bool decryptData(ref byte[] dataBuffer, out byte[] data);
		
		bool checkEndOfMessage(ref byte[] data);

		bool checkCheckSum(ref byte[] data);

		void dataProcessing(ref byte[] data);
	}
}
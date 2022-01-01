using LINQtoCSV;

namespace hello_s3
{
    public record class TimeRecord
    {
        [CsvColumn(Name = "Last", FieldIndex = 1)]
        public string LastName { get; set; } = string.Empty;
        [CsvColumn(Name = "First", FieldIndex = 2)]
        public string FirstName { get; set; } = string.Empty;
        [CsvColumn(Name = "Date", FieldIndex = 3, OutputFormat = "yyyy-MM-dd")]
        public DateTime Date { get; set; } = DateTime.MinValue;
        [CsvColumn(Name = "Hours", FieldIndex = 4)]
        public int Hours { get; set; } = 0;
        [CsvColumn(Name = "Client", FieldIndex = 5)]
        public string Client { get; set; } = string.Empty;
        [CsvColumn(Name = "Project", FieldIndex = 6)]
        public string Project { get; set; } = string.Empty;

        public TimeRecord() { }


        public static List<TimeRecord> ReadCsvFile(string filename)
        {
            var csvFileDescription = new CsvFileDescription
            {
                FirstLineHasColumnNames = true,
                IgnoreUnknownColumns = true,
                SeparatorChar = ',',
                UseFieldIndexForReadingData = false
            };

            var csvContext = new CsvContext();
            var records = csvContext.Read<TimeRecord>(filename, csvFileDescription);

            return records.ToList<TimeRecord>();
        }

        public static async void WriteCsvFile(string filename, List<TimeRecord> records)
        {
            var csvFileDescription = new CsvFileDescription
            {
                FirstLineHasColumnNames = true,
                IgnoreUnknownColumns = true,
                SeparatorChar = ',',
                UseFieldIndexForReadingData = false
            };

            var csvContext = new CsvContext();

            csvContext.Write<TimeRecord>(records, filename, csvFileDescription);
        }
    }
}
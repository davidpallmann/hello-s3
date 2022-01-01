using Amazon.S3;
using Amazon.S3.Model;

namespace hello_s3
{
    public class TimeAndBillingReport
    {
        const string BUCKET_NAME = "time-and-billing";

        List<TimeRecord> _records { get; set; } = new List<TimeRecord>();
        AmazonS3Client _client;

        public TimeAndBillingReport()
        {
            Console.WriteLine("Creating S3 client");

            AmazonS3Config config = new AmazonS3Config()
            {
                RegionEndpoint = Amazon.RegionEndpoint.USWest1
            };

            _client = new AmazonS3Client(config);
        }

        /// <summary>
        /// Read S3 timesheet CSV files from S3 and return as a TimeRecord collection.
        /// </summary>
        /// <returns></returns>
        public async Task ReadTimesheetsFromBucket()
        {
            _records = new List<TimeRecord>();

            Console.WriteLine("Listing objects in bucket");

            ListObjectsRequest Request = new ListObjectsRequest
            {
                BucketName = BUCKET_NAME,
            };

            ListObjectsResponse result;

            List<string> releases = new List<string>();
            do
            {
                result = await _client.ListObjectsAsync(Request);
                foreach (S3Object o in result.S3Objects)
                {
                    if (o.Key.Contains("timesheet", StringComparison.OrdinalIgnoreCase))
                    {
                        var response = await _client.GetObjectAsync(BUCKET_NAME, o.Key);
                        StreamReader reader = new StreamReader(response.ResponseStream);
                        string content = reader.ReadToEnd();
                        File.WriteAllText(o.Key, content);
                        _records.AddRange(TimeRecord.ReadCsvFile(o.Key));
                        File.Delete(o.Key);
                    }
                }
            }
            while (result.IsTruncated);
        }

        /// <summary>
        /// Generate monthly time and billing report. Creates file billing-report-[year]-[month].csv and uploaded to S3 bucket.
        /// </summary>
        /// <returns></returns>
        public async Task GenerateReport()
        {

            foreach (var record in _records)
            {
                Console.WriteLine($"date:{record.Date:yyyy-MM-dd} name:{record.LastName},{record.FirstName} hours:{record.Hours} client:{record.Client} project:{record.Project}");
            }

            var filename = $"billing-report-{DateTime.Today.Year}-{DateTime.Today.Month}.csv";

            Console.WriteLine("Creating " + filename);

            var sortedRecords = _records.OrderBy(r => r.Date).ThenBy(r => r.LastName).ThenBy(r => r.FirstName);

            TimeRecord.WriteCsvFile(filename, sortedRecords.ToList());

            await _client.PutObjectAsync(new PutObjectRequest { BucketName = BUCKET_NAME, Key = filename, ContentBody = File.ReadAllText(filename) });

            File.Delete(filename);
        }

    }
}
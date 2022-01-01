using hello_s3;

var report = new TimeAndBillingReport();

Console.WriteLine("reading records");
await report.ReadTimesheetsFromBucket();

Console.WriteLine("creating report");
await report.GenerateReport();

Console.WriteLine("done");
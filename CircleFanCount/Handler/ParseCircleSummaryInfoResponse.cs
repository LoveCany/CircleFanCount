using CircleFanCount.Gallop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircleFanCount.Handler
{
    public static partial class Handlers
    {
        public static void ParseCircleSummaryInfoResponse(CircleInfoResponse @event)
        {
            var data = @event.data.summary_user_info_array;
            var line = new List<string>{};
            foreach (var i in data)
            {
                long viewer_id = i.viewer_id;
                string name = i.name;
                long fan = i.fan;
                line.Add($"{viewer_id},{name},{fan}");
            }

            // Output to CSV file in user document
            var docPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "CircleFanCount", "circles");
            if (!Directory.Exists(docPath))
            {
                Directory.CreateDirectory(docPath);
            }
            string filename = @$"{docPath}\{DateTime.Now:yy-MM-dd HH-mm-ss-fff}_CircleSummaryInfo.csv";
            File.WriteAllLines(filename, line);
            Console.WriteLine(@$"Circle info saved to {filename}");
        }
    }
}

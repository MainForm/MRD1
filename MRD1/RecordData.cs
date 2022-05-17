using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenCvSharp;

using MySql.Data.MySqlClient;

namespace MRD1
{
    public class Measurement
    {
        public int ID { get; init; }
        public int Patient_ID { get; init; }

        public DateTime date { get; set; }

        public LedPosition Led_Position { get; set; }

        public void InsertDB(MySqlConnection connection)
        {
            string query = $"insert into measurement (Patient_ID,LED_Position,date) " +
                            $"VALUE({Patient_ID},\"{Led_Position.ToString()}\",\"{date.ToString("yyyy-MM-dd")}\")";

            using MySqlCommand cmd = new MySqlCommand(query, connection);

            cmd.ExecuteNonQuery();
        }
    }

    public class RecordData
    {
        public int ID { get; init; }

        public int index { get; init; }
        public Point pupuil_center { get; set; }
        public int pupil_radius { get; set; }

        public Mat image { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using OpenCvSharp;

using MySql.Data.MySqlClient;
using MySql.Data.Types;

namespace MRD1
{
    public interface MySQLusable
    {
        public void InsertDB(MySqlConnection connection);
    }

    public class Measurement : MySQLusable
    {
        public int? ID { get; set; }
        public int Patient_ID { get; init; }

        public DateTime date { get; set; }

        public LedPosition Led_Position { get; set; }

        public void InsertDB(MySqlConnection connection)
        {
            if (connection == null)
                throw new ArgumentNullException();

            string query = $"insert into measurement (Patient_ID,LED_Position,date) " +
                            $"VALUE({Patient_ID},\"{Led_Position.ToString()}\",\"{date.ToString("yyyy-MM-dd")}\")";

            using MySqlCommand cmd = new MySqlCommand(query, connection);

            cmd.ExecuteNonQuery();

            query = "select max(ID) from measurement";
            using MySqlCommand cmd_id = new MySqlCommand(query, connection);
            using MySqlDataReader reader = cmd_id.ExecuteReader();

            reader.Read();

            ID = reader.GetInt32(0);
        }
    }

    public class RecordData : MySQLusable
    {
        public int ID { get; init; }

        public int? Measurement_ID { get; init; }
        public CameraPosition Eye_Position { get; init; }
        public int index { get; init; }
        public Point pupil_center { get; set; }
        public int pupil_radius { get; set; }

        public int mrd1 { get; set; }
        public Mat image { get; set; }

        public Mat drawResult(int thinkness = 3)
        {
            if (image.Empty())
                return null;

            if (pupil_center == null)
                return null;

            Mat result = new Mat();

            image.CopyTo(result);

            result.Circle(pupil_center, pupil_radius, new Scalar(0, 0, 255), thinkness);
            result.DrawMarker(pupil_center, new Scalar(0, 255, 255),markerSize: 40,thickness: thinkness);

            Point ptMRD1 = new Point(pupil_center.X, pupil_center.Y - mrd1);

            result.DrawMarker(ptMRD1, new Scalar(0, 255, 255), markerSize: 40, thickness: thinkness);
            result.Line(pupil_center, ptMRD1, new Scalar(255, 0, 0),thinkness);
            
            return result;
        }

        public void InsertDB(MySqlConnection connection)
        {
            if (connection == null)
                throw new ArgumentNullException();

            Cv2.ImEncode(".jpg", image, out byte[] buf);

            string query = $"insert into record_data " +
                $"(Measurement_ID,Eye_Position,record_index,pupil_center,pupil_radius,image_size, image,MRD1) " +
                $"VALUE(@Measurement_ID,@Eye_Position,@record_index,@pupil_center,@pupil_radius,@image_size,@image,@MRD1)";

            using MySqlCommand cmd = new MySqlCommand(query, connection);

            cmd.Parameters.AddWithValue("@Measurement_ID", Measurement_ID);
            cmd.Parameters.AddWithValue("@Eye_Position", Eye_Position.ToString());
            cmd.Parameters.AddWithValue("@record_index", index);
            cmd.Parameters.AddWithValue("@pupil_center", new MySqlGeometry(pupil_center.X,pupil_center.Y));
            cmd.Parameters.AddWithValue("@pupil_radius", pupil_radius);
            cmd.Parameters.AddWithValue("@image_size", buf.Length);
            cmd.Parameters.AddWithValue("@image", buf);
            cmd.Parameters.AddWithValue("@MRD1", mrd1);

            cmd.ExecuteNonQuery();
        }

        public void updateDB(MySqlConnection connection)
        {
            if (connection == null)
                throw new ArgumentNullException();

            string query = $"UPDATE record_data SET " +
                $"Eye_Position=@Eye_Position, pupil_center=@pupil_center, pupil_radius=@pupil_radius, MRD1=@MRD1 " +
                $"WHERE ID={ID}";

            using MySqlCommand cmd = new MySqlCommand(query, connection);

            cmd.Parameters.AddWithValue("@Eye_Position", Eye_Position.ToString());
            cmd.Parameters.AddWithValue("@pupil_center", new MySqlGeometry(pupil_center.X, pupil_center.Y));
            cmd.Parameters.AddWithValue("@pupil_radius", pupil_radius);
            cmd.Parameters.AddWithValue("@MRD1", mrd1);

            cmd.ExecuteNonQuery();
        }

        public void deleteDB(MySqlConnection connection)
        {
            if (connection == null)
                throw new ArgumentNullException();

            string query = $"DELETE FROM record_data WHERE ID={ID}";

            using MySqlCommand cmd = new MySqlCommand(query, connection);

            cmd.ExecuteNonQuery();
        }

        public RecordData Clone()
        {
            return new RecordData()
            {
                index = index,
                Eye_Position = Eye_Position,
                ID = ID,
                image = image,
                Measurement_ID = Measurement_ID,
                mrd1 = mrd1,
                pupil_center = pupil_center,
                pupil_radius = pupil_radius,
            };
        }
    }
}

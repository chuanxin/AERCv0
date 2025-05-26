using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Dry.Models.CommonCls
{
    public class ImageExif
    {
        public string Path { set; get; }
        /// <summary>
        /// 緯度
        /// </summary>
        public double Latitude { get; set; }
        /// <summary>
        /// 經度
        /// </summary>
        public double Longitude { get; set; }
        public ImageExif(string Path)
        {
            double[] Latlng = GetLatlng(Path);
            Latitude = Latlng[0];
            Longitude = Latlng[1];
        }
        private double[] GetLatlng(string Path)
        {
            double lat = 0;
            double lng = 0;
            Image img = Image.FromFile(Path);
            
            var imgProp = img.PropertyItems.OrderBy(x => x.Id);
            foreach (PropertyItem item in imgProp)
            {
                switch (item.Id)
                {
                    case 0x0002:
                        if (item.Value.Length == 24)
                        {
                            lat = CaculatLatlng(item.Value);
                        }
                        break;
                    case 0x0004:
                        if (item.Value.Length == 24)
                        {
                            lng = CaculatLatlng(item.Value);
                        }
                        break;
                }
            }

            return new double[] { lat, lng };
        }
        private double CaculatLatlng(byte[] values)
        {
            double degree = (double)BitConverter.ToUInt32(values, 0) / BitConverter.ToUInt32(values, 4);
            double minutes = (double)BitConverter.ToUInt32(values, 8) / BitConverter.ToUInt32(values, 12);
            double seconds = (double)BitConverter.ToUInt32(values, 16) / BitConverter.ToUInt32(values, 20);
            return (seconds / 60 + minutes) / 60 + degree;
        }
    }
}

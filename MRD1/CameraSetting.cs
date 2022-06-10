using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRD1
{
    public class CameraSetting
    {
        public int index { get; set; }
        public int brightness { get; set; } = 0;

        public int frame_width { get; set; } = 1280;
        public int frame_height { get; set; } = 720;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventMapHpViewer.Models
{
    public struct TransportCapacity
    {
        private int _s;

        public int S
        {
            get { return _s; }
            set
            {
                _s = value;
                this.A = (int)Math.Floor(value * 0.7);
            }
        }

        public int A { get; private set; }

        public TransportCapacity(int s)
        {
            A = 0;
            _s = 0;
            S = s;
        }
    }
}

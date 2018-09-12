using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventMapHpViewer.Models
{
    public struct TransportCapacity
    {
        private decimal _s;

        public decimal S
        {
            get { return _s; }
            set
            {
                _s = value;
                this.A = Math.Floor(value * 0.7m);
            }
        }

        public decimal A { get; private set; }

        public TransportCapacity(decimal s)
        {
            A = 0;
            _s = 0;
            S = s;
        }
    }
}

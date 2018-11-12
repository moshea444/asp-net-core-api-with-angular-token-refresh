using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class AspNetUserToken
    {
        public string UserId { get; set; }

        public string LoginProvider { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }

        public string Device { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Db.Shared;

public enum EnIdentityProvider
{
    Unknown = 0,
    Azure = 1,
    AWS = 2,
    GoogleCloud = 3,
    Okta = 4,
    Auth0 = 5,
    Custom = 6
}

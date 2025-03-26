using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretManagementService.Models;
public class ContactMethod
{
    public bool IsEmail { get; set; }
    public bool IsSMS { get; set; }
    public bool IsApiEndpoint { get; set; }
    public List<string> Emails { get; set; } = [];
    public List<string> PhoneNumbers { get; set; } = [];
    public ApiInfo? ApiInfo { get; set; }
}
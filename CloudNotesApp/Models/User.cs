using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudNotesApp.Models
{
    public class User
    {
        public string LocalId { get; set; } 
        public string Email { get; set; }
        public string IdToken { get; set; } 
    }
}

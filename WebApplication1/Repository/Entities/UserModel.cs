using Microsoft.EntityFrameworkCore;

using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Repository.Entities
{
    [Table("user")]
    public class UserModel
    {
        public long id { get; set; }
        public string name { get; set; }
    }
}

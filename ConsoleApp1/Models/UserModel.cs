#pragma warning disable CS8618 // null을 허용하지 않는 필드는 생성자를 종료할 때 null이 아닌 값을 포함해야 합니다. 'required' 한정자를 추가하거나 nullable로 선언하는 것이 좋습니다.
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.Models
{
    [Table("nickname")]
    public class NickNameModel
    {
        public long user_id { get; set; }
        public string nick_name { get; set; }
    }
}

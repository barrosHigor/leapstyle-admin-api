using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    [Table("logs_erros")]
    public class LogErro
    {
        [Key]
        public int id { get; set; }
        public string erro { get; set; }
        public string query { get; set; }
        public string metodo { get; set; }
        public DateTime data_erro { get; set; }
    }
}

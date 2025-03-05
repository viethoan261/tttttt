using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebFilm.Core.Enitites
{
    public class BaseEntity
    {
        [Key]
        public int id { get; set; }

        #region Prop
        public DateTime? createdDate { get; set; }

        public DateTime? modifiedDate { get; set; }

        #endregion
    }
}

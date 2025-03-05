using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace WebFilm.Core.Enitites.User
{
    public class Users : BaseEntity
    {
        #region Prop
        public string username { get; set; }

        public string? fullname { get; set; }

        public string role { get; set; }

        public string password { get; set; }
        #endregion
    }
}

using System.ComponentModel.DataAnnotations;

namespace HelpDeskCore.Data.Entities
{
    public class UserAvatar : UserBase
    {
        public int Id { get; set; }
        public byte[] ImageData { get; set; }
    }
}

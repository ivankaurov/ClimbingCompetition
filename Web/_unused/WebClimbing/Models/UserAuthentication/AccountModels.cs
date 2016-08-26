using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;
using System.Text;
using System.Linq;
using WebClimbing.ServiceClasses;

namespace WebClimbing.Models.UserAuthentication
{
    //public class UsersContext : DbContext
    //{
    //    public UsersContext()
    //        : base("DefaultConnection")
    //    {
    //    }

    //    public DbSet<UserProfile> UserProfiles { get; set; }
    //}

    [EnumCustomDisplay(NullName="Нет доступа")]
    public enum RoleEnum : int
    {
        [Display(Name="Пользователь")]
        User = 1,
        [Display(Name="Администратор")]
        Admin = 2
    }

    public enum RightsEnum : byte
    {
        None = 0x00,
        View = 0x01,
        Edit = (0x02 | 0x01)
    }

    public enum RightsType : byte { Competition, Database }

    [Table("MVC_UA_Users"), DisplayColumn("Name")]
    public class UserProfileModel : IComparable<UserProfileModel>
    {
        public override string ToString()
        {
            return this.Name;
        }

        public const int TOKEN_LENGTH = 50;

        [Key, Column("iid"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Iid { get; set; }

        [Required(AllowEmptyStrings = true), Column("user_name"), MaxLength(255), Display(Name="Имя пользователя")]
        public string Name { get; set; }

        [Column("region_id"), ForeignKey("Region")]
        public long? RegionId { get; set; }
        [Display(Name = "Регион")]
        public virtual RegionModel Region { get; set; }

        [Column("password"/*, TypeName = "image"*/)]
        public string Password { get; set; }

        [Column("salt", TypeName="image")]
        public byte[] Salt { get; set; }

        [Required, Column("inactive"), Display(Name = "Неактивен")]
        public bool Inactive { get; set; }

        [Column("token"), MaxLength(TOKEN_LENGTH)]
        public String Token { get; set; }

        [Column("email"), MaxLength(255)]
        public String Email { get; set; }

        [Column("password_token_expiration")]
        public DateTime? PasswordTokenExpirationTime { get; set; }

        public void SetPassword(String newPassword)
        {
            if (newPassword == null)
                newPassword = String.Empty;
            Random rand = new Random(unchecked((int)DateTime.Now.Ticks));
            byte[] newSalt = new byte[rand.Next(10, 20)];
            rand.NextBytes(newSalt);
            this.Password = ComputeHash(newPassword, newSalt);
            this.Salt = newSalt;
        }

        private string ComputeHash(string password, byte[] salt)
        {
            byte[] newPasswordBytes = Encoding.Unicode.GetBytes(password);
            byte[] newEncodingValue = new byte[newPasswordBytes.Length + salt.Length];
            newPasswordBytes.CopyTo(newEncodingValue, 0);
            salt.CopyTo(newEncodingValue, newPasswordBytes.Length);
            return Encoding.ASCII.GetString((new System.Security.Cryptography.SHA512CryptoServiceProvider()).ComputeHash(newEncodingValue));
        }

        public bool CheckPassword(string passwordToCheck)
        {
            if (Salt == null)
                return String.IsNullOrEmpty(passwordToCheck);
            
            var pswToCheck = ComputeHash((passwordToCheck == null ? String.Empty : passwordToCheck), this.Salt);
            return pswToCheck.Equals(this.Password);
        }

        public virtual ICollection<UserRoleModel> Roles { get; set; }

        public virtual ICollection<UserPublicKey> PublicKeys { get; set; }

        public override bool Equals(object obj)
        {
            var other = obj as UserProfileModel;
            if (other == null)
                return false;
            return this.Iid.Equals(other.Iid);
        }

        public override int GetHashCode()
        {
            return this.Iid.GetHashCode();
        }

        public int CompareTo(UserProfileModel other)
        {
            if (other == null)
                return 1;

            if (this.RegionId == null && other.RegionId != null)
                return -1;
            if (this.RegionId != null && other.RegionId == null)
                return 1;
            if (this.RegionId != null && other.RegionId != null)
            {
                int n = this.Region.CompareTo(other.Region);
                if (n != 0)
                    return n;
            }
            return this.Name.CompareTo(other.Name);
        }

    }

    [Table("MVC_UA_Keys")]
    public class UserPublicKey
    {
        [Key, Column("iid"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Iid { get; set; }

        [Required, Column("iid_parent"), ForeignKey("User")]
        public int UserId { get; set; }

        [Display(Name="Пользователь")]
        public virtual UserProfileModel User { get; set; }

        [Required, Column("date_b")]
        public DateTime StartDate { get; set; }
        [Required, Column("date_e")]
        public DateTime StopDate { get; set; }
    }

    [Table("MVC_UA_Roles"), DisplayColumn("Name")]
    public class RoleModel
    {
        [Key, Column("iid")]
        public int Iid { get; set; }

        [Required(AllowEmptyStrings = false), Column("role_name"), Display(Name = "Наименование")]
        public string Name { get; set; }

        [NotMapped]
        public RoleEnum RoleCode
        {
            get { return (RoleEnum)this.Iid; }
            set { this.Iid = (int)value; }
        }

        public virtual ICollection<UserRoleModel> Users { get; set; }
        //public virtual ICollection<RoleRightsModel> Rights { get; set; }

        public override bool Equals(object obj)
        {
            RoleModel other = obj as RoleModel;
            if (other == null)
                return false;
            return this.Iid.Equals(other.Iid);
        }

        public override int GetHashCode()
        {
            return this.Iid.GetHashCode();
        }
    }

    [Table("MVC_UA_UserRoles")]
    public class UserRoleModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity), Column("iid")]
        public long Iid { get; set; }

        [Required, Column("user_id"), ForeignKey("User")]
        public int UserId { get; set; }
        [Display(Name = "Пользователь")]
        public virtual UserProfileModel User { get; set; }

        [Required, Column("role_id"), ForeignKey("Role")]
        public int RoleId { get; set; }
        [Display(Name = "Роль")]
        public virtual RoleModel Role { get; set; }
        
        [Column("comp_id"), ForeignKey("Competition")]
        public long? CompID { get; set; }
        [Display(Name = "Соревнования")]
        public virtual CompetitionModel Competition { get; set; }

        [Column("region_id"), ForeignKey("Region")]
        public long? RegionID { get; set; }
        [Display(Name = "Регион")]
        public virtual RegionModel Region { get; set; }

        public override bool Equals(object obj)
        {
            UserRoleModel other = obj as UserRoleModel;
            if (other == null)
                return false;
            return this.Iid.Equals(other.Iid);
        }

        public override int GetHashCode()
        {
            return this.Iid.GetHashCode();
        }
    }
    /*
    [Table("MVC_UA_RoleRights")]
    public class RoleRightsModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity), Column("iid")]
        public int Iid { get; set; }

        [Required, Column("role_id"), ForeignKey("Role")]
        public int RoleId { get; set; }
        public virtual RoleModel Role { get; set; }

        [Required, Column("rights")]
        public byte RightCode { get; set; }
        [NotMapped]
        public RightsEnum Rights { get { return (RightsEnum)RightCode; } set { RightCode = (byte)value; } }

        [Required, Column("rights_type")]
        public byte RightType { get; set; }
        [NotMapped]
        public RightsType Type { get { return (RightsType)RightType; } set { RightType = (byte)value; } }
    }*/

    public class RegisterExternalLoginModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        public string ExternalLoginData { get; set; }
    }

    public class LocalPasswordModel
    {
        [Required, Display(Name = "Имя пользователя")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        //[Required]
        //[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required, DataType(DataType.EmailAddress, ErrorMessage = "Пожалуйста, введите действительный E-mail"), Display(Name = "E-mail")]
        public string Email { get; set; }
    }

    public class LoginModel
    {
        [Required]
        [Display(Name = "Имя пользователя или E-mail")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Display(Name = "Запомнить меня на этом ПК?")]
        public bool RememberMe { get; set; }
    }

    public class DeleteModel
    {
        public int Iid { get; set; }
        [Display(Name = "Имя пользователя")]
        public string UserName { get; set; }
        [Display(Name = "Регион")]
        public string RegionName { get; set; }
    }

    public class RegisterModel
    {
        public RegisterModel()
        {
            this.Iid = 0;
            this.RegionId = null;
        }

        public RegisterModel(UserProfileModel user)
        {
            this.Iid = user.Iid;
            this.UserName = user.Name;
            this.Email = user.Email;
            this.RegionId = user.RegionId;
        }

        public int Iid { get; set; }

        [Required]
        [Display(Name = "Имя пользователя")]
        public string UserName { get; set; }

        /*
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }*/

        [Required, DataType(DataType.EmailAddress, ErrorMessage = "Пожалуйста, введите действительный E-mail"), Display(Name = "E-mail")]
        public string Email { get; set; }

        /*[DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }*/

        [Display(Name="Регион")]
        public long? RegionId { get; set; }
    }

    public class ExternalLogin
    {
        public string Provider { get; set; }
        public string ProviderDisplayName { get; set; }
        public string ProviderUserId { get; set; }
    }

    public class ForgottenPasswordModel
    {
        public ForgottenPasswordModel()
        {
            this.ModelSuccess = false;
        }

        [Required(AllowEmptyStrings = false), Display(Name = "Имя пользователя или e-mail")]
        public String UserName { get; set; }

        public bool ModelSuccess { get; set; }
    }

    public class ValidationModel
    {
        [Required, Display(Name="Имя пользователя")]
        public string UserName { get; set; }

        [DataType(DataType.Password),
         Display(Name = "Новый пароль"),
         Required(AllowEmptyStrings = false),
         StringLength(100, ErrorMessage = "{0} должен быть как минимум {2} символов.", MinimumLength = 1)]
        public string Password { get; set; }

        [DataType(DataType.Password),
         Display(Name = "Подтверждение пароля"),
         Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string ConfirmationPassword { get; set; }

        public string Token { get; set; }
    }
}

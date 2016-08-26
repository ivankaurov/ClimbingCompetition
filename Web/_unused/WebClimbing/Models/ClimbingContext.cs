#if DEBUG
#undef DEBUG
#endif
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using WebClimbing.Models.UserAuthentication;
using System.ComponentModel.DataAnnotations;
using WebClimbing.ServiceClasses;

namespace WebClimbing.Models
{
    [EnumCustomDisplay]
    public enum DbResult
    {
        [Display(Name = "создан")]
        Created,
        [Display(Name = "обновлен")]
        Updated,
        [Display(Name = "ошибка")]
        Error,
        [Display(Name = "удален")]
        Deleted
    }

    public class ClimbingContext : DbContext
    {
        public ClimbingContext()
            : base("RemoteDbConnection")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<LicenseModel>()
            //    .HasMany(l => l.Competitions)
            //    .WithRequired(c => c.Person)
            //    .HasForeignKey(c => c.PersonId)B
            //    .WillCascadeOnDelete(false);
            modelBuilder.Entity<CompetitionModel>()
                .HasMany(c => c.Climbers)
                .WithRequired(c => c.Competition)
                .HasForeignKey(c => c.CompId)
                .WillCascadeOnDelete(false);
            modelBuilder.Entity<Comp_AgeGroupModel>()
                .HasMany(c => c.Competitiors)
                .WithRequired(c => c.CompAgeGroup)
                .HasForeignKey(c => c.GroupId)
                .WillCascadeOnDelete(false);
            //modelBuilder.Entity<RegionModel>().HasMany(r => r.AdditionalPeople)
            //                                  .WithRequired(c => c.Region)
            //                                  .HasForeignKey(c => c.RegionIid)
            //                                  .WillCascadeOnDelete(false);

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<CompetitionModel> Competitions { get; set; }
        public DbSet<CompetitionParameterModel> CompetitionParameters { get; set; }
        public DbSet<RegionModel> Regions { get; set; }
        public DbSet<PersonModel> People { get; set; }
        public DbSet<PersonPictureModel> Pictures { get; set; }
        public DbSet<UserProfileModel> UserProfiles { get; set; }
        public DbSet<UserRoleModel> UserRoles { get; set; }
        public DbSet<UserPublicKey> UserPublicKeys { get; set; }
        public DbSet<RoleModel> Roles { get; set; }
        //public DbSet<RoleRightsModel> RoleRights { get; set; }
        public DbSet<AgeGroupModel> AgeGroups { get; set; }


        public DbSet<Comp_AgeGroupModel> CompetitionAgeGroups { get; set; }
        public DbSet<Comp_CompetitiorRegistrationModel> CompetitionClimbers { get; set; }
        public DbSet<Comp_ClimberTeam> CompetitionClimberTeams { get; set; }

        public DbSet<ListHeaderModel> Lists { get; set; }
        public DbSet<ListLineModel> Results { get; set; }
        public DbSet<BoulderResultRoute> BoulderRoutes { get; set; }
    }

    public class ClimbingContextInitializer :
#if DEBUG
#if DROPALWAYS
        DropCreateDatabaseAlways
#else
 DropCreateDatabaseIfModelChanges
#endif
#else
 System.Data.Entity.CreateDatabaseIfNotExists
#endif
<ClimbingContext>
    {
        protected override void Seed(ClimbingContext context)
        {
            //RoleModel admRole = null, compAdmRole = null;
            Dictionary<RoleEnum, RoleModel> roles = new Dictionary<RoleEnum, RoleModel>();
            foreach (var e in Enum.GetValues(typeof(RoleEnum)).Cast<RoleEnum>())
            {
                var tmp = context.Roles.Add(new RoleModel
                {
                    Name = e.GetFriendlyValue(),
                    RoleCode = e
                });
                roles.Add(e, tmp);
            }
           

            var usr = context.UserProfiles.Add(new UserProfileModel
            {
                Name = "admin",
                Inactive = false,
                Email = "ivan.kaurov@gmail.com",
                Roles = new List<UserRoleModel>{
                    new UserRoleModel{Role = roles[RoleEnum.Admin]}}
            });
            usr.SetPassword("0");

            #region AgeGroups
            context.AgeGroups.Add(new AgeGroupModel
            {
                FullName = "Мужчины",
                SecretaryName = "Мужчины",
                GenderProperty = Gender.Male
            });
            context.AgeGroups.Add(new AgeGroupModel
            {
                FullName = "Женщины",
                SecretaryName = "Женщины",
                GenderProperty = Gender.Female
            });
            context.AgeGroups.Add(new AgeGroupModel
            {
                FullName = "Юниоры (18-19 лет)",
                SecretaryName = "Юниоры",
                GenderProperty = Gender.Male,
                MinAge = 18,
                MaxAge = 19
            });
            context.AgeGroups.Add(new AgeGroupModel
            {
                FullName = "Юниорки (18-19 лет)",
                SecretaryName = "Юниорки",
                GenderProperty = Gender.Female,
                MinAge = 18,
                MaxAge = 19
            });
            context.AgeGroups.Add(new AgeGroupModel
            {
                FullName = "Старшие юноши (16-17 лет)",
                SecretaryName = "Старшие юноши",
                GenderProperty = Gender.Male,
                MinAge = 16,
                MaxAge = 17
            });
            context.AgeGroups.Add(new AgeGroupModel
            {
                FullName = "Старшие девушки (16-17 лет)",
                SecretaryName = "Старшие девушки",
                GenderProperty = Gender.Female,
                MinAge = 16,
                MaxAge = 17
            });
            context.AgeGroups.Add(new AgeGroupModel
            {
                FullName = "Младшие юноши (14-15 лет)",
                SecretaryName = "Младшие юноши",
                GenderProperty = Gender.Male,
                MinAge = 14,
                MaxAge = 15
            });
            context.AgeGroups.Add(new AgeGroupModel
            {
                FullName = "Младшие девушки (14-15 лет)",
                SecretaryName = "Младшие девушки",
                GenderProperty = Gender.Female,
                MinAge = 14,
                MaxAge = 15
            });
            context.AgeGroups.Add(new AgeGroupModel
            {
                FullName = "Подростки мальчики (10-13 лет)",
                SecretaryName = "Подростки мальчики",
                GenderProperty = Gender.Male,
                MinAge = 10,
                MaxAge = 13
            });
            context.AgeGroups.Add(new AgeGroupModel
            {
                FullName = "Подростки девочки (10-13 лет)",
                SecretaryName = "Подростки девочки",
                GenderProperty = Gender.Female,
                MinAge = 10,
                MaxAge = 13
            });
            context.AgeGroups.Add(new AgeGroupModel
            {
                FullName = "Подростки мальчики (9-13 лет)",
                SecretaryName = "Подростки мальчики",
                GenderProperty = Gender.Male,
                MinAge = 9,
                MaxAge = 13
            });
            context.AgeGroups.Add(new AgeGroupModel
            {
                FullName = "Подростки девочки (9-13 лет)",
                SecretaryName = "Подростки девочки",
                GenderProperty = Gender.Female,
                MinAge = 9,
                MaxAge = 13
            });
            #endregion

            base.Seed(context);
        }
    }
}
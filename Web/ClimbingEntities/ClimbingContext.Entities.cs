using ClimbingEntities.AgeGroups;
using ClimbingEntities.Competitions;
using ClimbingEntities.Lists;
using ClimbingEntities.People;
using ClimbingEntities.Teams;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace ClimbingEntities
{
    partial class ClimbingContext2
    {
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Team>()
                        .HasMany(t => t.ChildTeams)
                        .WithOptional(t => t.ParentTeam)
                        .HasForeignKey(t => t.IidParent)
                        .WillCascadeOnDelete(false);
            modelBuilder.Entity<Team>()
                        .HasMany(t => t.TeamMainUsers)
                        .WithOptional(u => u.Team)
                        .HasForeignKey(t => t.TeamId)
                        .WillCascadeOnDelete(false);

            modelBuilder.Entity<Competition>()
                        .HasMany(c => c.Parameters)
                        .WithRequired(p => p.Competition)
                        .HasForeignKey(p => p.CompId)
                        .WillCascadeOnDelete(true);
            modelBuilder.Entity<Competition>()
                .HasRequired(c => c.Organizer)
                .WithMany(o => o.OrganizedCompetitions)
                .HasForeignKey(c => c.OrganizerId)
                .WillCascadeOnDelete(true);
            modelBuilder.Entity<Competition>()
                .HasMany(c => c.Competitors)
                .WithRequired(c => c.Competition)
                .HasForeignKey(c => c.CompId)
                .WillCascadeOnDelete(false);
            modelBuilder.Entity<Competition>()
                .HasMany(c => c.AgeGroups)
                .WithRequired(g => g.Competition)
                .HasForeignKey(g => g.CompId)
                .WillCascadeOnDelete(true);
            modelBuilder.Entity<ClimberOnCompetition>()
                .HasMany(c => c.Teams)
                .WithRequired(t => t.Climber)
                .HasForeignKey(t => t.ClimberOnCompId)
                .WillCascadeOnDelete(true);
            modelBuilder.Entity<AgeGroupOnCompetition>()
                .HasMany(g => g.Climbers)
                .WithRequired(c => c.AgeGroup)
                .HasForeignKey(c => c.AgeGroupId)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Person>()
                .HasMany(p => p.ClimbersLicenses)
                .WithRequired(c => c.Person)
                .HasForeignKey(c => c.PersonId)
                .WillCascadeOnDelete(true);
            modelBuilder.Entity<Person>()
                .HasMany(p => p.CompetitionAppearances)
                .WithRequired(c => c.Person)
                .HasForeignKey(c => c.PersonId)
                .WillCascadeOnDelete(false);
            modelBuilder.Entity<Climber>()
                .HasRequired(c => c.Team)
                .WithMany(t => t.TeamClimbers)
                .HasForeignKey(c => c.TeamId)
                .WillCascadeOnDelete(true);
            modelBuilder.Entity<Climber>()
                .HasMany(c => c.CompetitionAppearances)
                .WithRequired(c => c.TeamLicense)
                .HasForeignKey(c => c.TeamLicenseId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<AgeGroup>()
                .HasMany(g => g.AgeGroupAppearances)
                .WithRequired(g => g.AgeGroup)
                .HasForeignKey(g => g.AgeGroupId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ListHeader>()
                .HasRequired(l => l.Competition)
                .WithMany(c => c.ResultLists)
                .HasForeignKey(l => l.CompId)
                .WillCascadeOnDelete(true);
            modelBuilder.Entity<ListHeader>()
                .HasMany(l => l.Children)
                .WithOptional(lc => lc.Parent)
                .HasForeignKey(lc => lc.IidParent)
                .WillCascadeOnDelete(false);
            modelBuilder.Entity<ListHeader>()
                .HasMany(l => l.NextRounds)
                .WithOptional(ln => ln.PreviousRound)
                .HasForeignKey(ln => ln.PrevRoundIid)
                .WillCascadeOnDelete(false);
            modelBuilder.Entity<ListHeader>()
                .HasOptional(h => h.AgeGroup)
                .WithMany(g => g.ResultLists)
                .HasForeignKey(h => h.GroupId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ListLine>()
                .HasRequired(l => l.Header)
                .WithMany(h => h.Results)
                .HasForeignKey(l => l.ListId)
                .WillCascadeOnDelete(true);
            modelBuilder.Entity<ListLine>()
                .HasRequired(l => l.Climber)
                .WithMany(c => c.Results)
                .HasForeignKey(c => c.ClimberId)
                .WillCascadeOnDelete(false);
            modelBuilder.Entity<ListLineBoulderRoute>()
                .HasRequired(r => r.Result)
                .WithMany(l => l.Routes)
                .HasForeignKey(r => r.ResultIid)
                .WillCascadeOnDelete(true);
        }
        public DbSet<Team> Teams { get; set; }
        public DbSet<UserWithTeam> UsersWithTeam { get; set; }

        public DbSet<Competition> Competitions { get; set; }
        public DbSet<CompetitionParameter> CompetitionParameters { get; set; }
        public DbSet<ClimberOnCompetition> ClimbersOnCompetition { get; set; }
        public DbSet<ClimberTeamOnCompetition> ClimberTeamsOnCompetition { get; set; }
        public DbSet<AgeGroupOnCompetition> AgeGroupsOnCompetition { get; set; }

        public DbSet<Person> People { get; set; }
        public DbSet<Climber> Climbers { get; set; }
        public DbSet<AgeGroup> AgeGroups { get; set; }

        public DbSet<ListHeader> ResultLists { get; set; }
        public DbSet<ListLine> AllResultLines { get; set; }
        public DbSet<ListLineLead> ResultsLead { get; set; }
        public DbSet<ListLineSpeed> ResultsSpeed { get; set; }
        public DbSet<ListLineBoulder> ResultsBoulder { get; set; }
        public DbSet<ListLineBoulderRoute> ResultsBoulderRoutes { get; set; }
    }
}
